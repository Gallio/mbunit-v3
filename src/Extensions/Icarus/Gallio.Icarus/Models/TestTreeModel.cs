// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.Models
{
    internal class TestTreeModel : TreeModelDecorator, ITestTreeModel
    {
        private readonly IList<ITreeBuilder> treeBuilders;
        private readonly List<TestStatus> filterStatuses = new List<TestStatus>();

        public bool FilterPassed
        {
            get
            {
                return filterStatuses.Contains(TestStatus.Passed);
            }
        }

        public bool FilterFailed
        {
            get
            {
                return filterStatuses.Contains(TestStatus.Failed);
            }
        }

        public bool FilterInconclusive
        {
            get
            {
                return filterStatuses.Contains(TestStatus.Inconclusive);
            }
        }

        public Observable<int> TestCount
        {
            get;
            private set;
        }

        public TestTreeModel(ITreeBuilder[] treeBuilders)
            : base(new TreeModel())
        {
            this.treeBuilders = treeBuilders;

            TestCount = new Observable<int>();
        }

        public void UpdateTestCount()
        {
            // (this event is fired when a node checkstate changes)
            var testCount = 0;
            foreach (Node node in innerTreeModel.GetChildren(new TreePath()))
                testCount += CountTests(node);
            TestCount.Value = testCount;
        }

        private static int CountTests(Node node)
        {
            int count = 0;
            
            if (((TestTreeNode)node).IsTest && node.IsChecked)
                count += 1;

            foreach (var n in node.Nodes)
                count += CountTests(n);

            return count;
        }

        public TestTreeNode Root
        {
            get
            {
                return (TestTreeNode)GetRoot();
            }
        }

        public void RemoveFilter(TestStatus testStatus)
        {
            filterStatuses.Remove(testStatus);
            ClearFilter(testStatus);
        }

        public void ResetTestStatus(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Resetting test statuses", 100))
            {
                foreach (Node node in innerTreeModel.GetChildren(new TreePath()))
                    ((TestTreeNode)node).Reset();

                OnNodesChanged(new TreeModelEventArgs(new TreePath(Root), new object[] {}));

                FilterTree();
            }
        }

        public void SetFilter(TestStatus testStatus)
        {
            filterStatuses.Add(testStatus);
            FilterTree();
        }

        public void TestStepFinished(TestData testData, TestStepRun testStepRun)
        {
            if (Root == null)
                return;

            var nodes = Root.Find(testData.Id, true);
            foreach (var node in nodes) // there should only be one
            {
                node.AddTestStepRun(testStepRun);
                Filter(node);
            }
        }

        private void FilterTree()
        {
            foreach (Node node in innerTreeModel.GetChildren(new TreePath()))
                Filter(node);

            OnStructureChanged(new TreePathEventArgs(new TreePath(Root)));
        }

        private bool Filter(Node n)
        {
            if (n is TestTreeNode && filterStatuses.Count > 0)
            {
                TestTreeNode node = (TestTreeNode)n;
                
                // only filter leaf nodes
                if (n.Nodes.Count == 0 && filterStatuses.Contains(node.TestStatus))
                {
                    FilterNode(node, node.TestStatus);
                    return false;
                }
                if (!Enum.IsDefined(typeof(TestStatus), node.Name))
                {
                    int i = 0;
                    while (i < node.Nodes.Count)
                    {
                        if (Filter(node.Nodes[i]))
                            i++;
                    }
                }
            }
            return true;
        }

        private static void FilterNode(Node node, TestStatus testStatus)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            string key = testStatus.ToString();
            TestTreeNode filterNode;
            List<TestTreeNode> nodes = ((TestTreeNode)node.Parent).Find(key, true);
            if (nodes.Count > 0)
                filterNode = nodes[0];
            else
            {
                filterNode = new FilterNode(testStatus);
                node.Parent.Nodes.Add(filterNode);
            }
            node.Parent.Nodes.Remove(node);
            filterNode.Nodes.Add(node);
        }

        private void ClearFilter(TestStatus testStatus)
        {
            if (Root == null)
                return;

            foreach (TestTreeNode filterNode in Root.Find(testStatus.ToString(), true))
            {
                Node[] nodes = new Node[filterNode.Nodes.Count];
                filterNode.Nodes.CopyTo(nodes, 0);
                foreach (Node n in nodes)
                    filterNode.Parent.Nodes.Add(n);
                filterNode.Parent.Nodes.Remove(filterNode);
            }

            OnStructureChanged(new TreePathEventArgs(new TreePath(Root)));
        }

        public void BuildTestTree(IProgressMonitor progressMonitor, TestModelData testModelData, 
            TreeBuilderOptions options)
        {
            int count = CountTestData(testModelData.RootTest);

            using (progressMonitor.BeginTask("Building test tree", count))
            {
                ((TreeModel)innerTreeModel).Root.Nodes.Clear();

                TestTreeNode root = null;
                foreach (var treeBuilder in treeBuilders)
                {
                    if (!treeBuilder.CanHandle(options.TreeViewCategory)) 
                        continue;

                    root = treeBuilder.BuildTree(progressMonitor, testModelData, options);
                    break;
                }

                if (root == null)
                    throw new Exception(string.Format("Could not find a tree builder for {0}", 
                        options.TreeViewCategory));

                ((TreeModel)innerTreeModel).Root.Nodes.Add(root);

                OnStructureChanged(new TreePathEventArgs(new TreePath(root)));
            }
        }

        private static int CountTestData(TestData testData)
        {
            int count = 1;
            foreach (var td in testData.Children)
                count += CountTestData(td);
            return count;
        }

        public void ApplyFilterSet(FilterSet<ITestDescriptor> filterSet)
        {
            if (Root == null)
                return;

            if (filterSet.IsEmpty || filterSet.Rules[0].Filter is AnyFilter<ITestDescriptor>)
            {
                Root.CheckState = CheckState.Checked;
                Root.UpdateStateOfRelatedNodes();
                return;
            }

            Root.CheckState = CheckState.Unchecked;
            Root.UpdateStateOfRelatedNodes();

            // FIXME: should be considering inclusion / exclusion rules in more detail
            RecursivelyApplyFilter(filterSet.Rules[0].Filter);
        }

        private void RecursivelyApplyFilter(Filter<ITestDescriptor> filter)
        {
            if (filter is NoneFilter<ITestDescriptor>)
                return;
            if (filter is OrFilter<ITestDescriptor>)
            {
                OrFilter<ITestDescriptor> orFilter = (OrFilter<ITestDescriptor>)filter;
                foreach (Filter<ITestDescriptor> childFilter in orFilter.Filters)
                    RecursivelyApplyFilter(childFilter);
            }
            else if (filter is PropertyFilter<ITestDescriptor>)
            {
                PropertyFilter<ITestDescriptor> propertyFilter = (PropertyFilter<ITestDescriptor>)filter;
                EqualityFilter<string> equalityFilter = (EqualityFilter<string>)propertyFilter.ValueFilter;
                foreach (TestTreeNode n in Root.Find(equalityFilter.Comparand, true))
                {
                    n.CheckState = CheckState.Checked;
                    n.UpdateStateOfRelatedNodes();
                }
            }
        }

        public FilterSet<ITestDescriptor> GenerateFilterSetFromSelectedTests()
        {
            if (Root == null || Root.CheckState == CheckState.Checked)
                return FilterSet<ITestDescriptor>.Empty;

            var filter = Root.CheckState == CheckState.Unchecked ? new NoneFilter<ITestDescriptor>() 
                : CreateFilter(Root.Nodes);
            return new FilterSet<ITestDescriptor>(filter);
        }

        private static Filter<ITestDescriptor> CreateFilter(IEnumerable<Node> nodes)
        {
            List<Filter<ITestDescriptor>> filters = new List<Filter<ITestDescriptor>>();
            foreach (Node n in nodes)
            {
                if (!(n is TestTreeNode))
                    continue;

                TestTreeNode node = (TestTreeNode)n;
                switch (node.CheckState)
                {
                    case CheckState.Checked:
                        {
                            EqualityFilter<string> equalityFilter = new EqualityFilter<string>(node.Name);
                            if (node is NamespaceNode)
                            {
                                filters.Add(new NamespaceFilter<ITestDescriptor>(equalityFilter));
                            }
                            else if (node is TestDataNode)
                            {
                                filters.Add(new IdFilter<ITestDescriptor>(equalityFilter));
                            }
                            else if (node is MetadataNode && node.Name != "None")
                            {
                                filters.Add(new MetadataFilter<ITestDescriptor>(node.Name, equalityFilter));
                            }
                            else
                            {
                                Filter<ITestDescriptor> childFilters = CreateFilter(node.Nodes);
                                if (childFilters != null)
                                    filters.Add(childFilters);
                            }
                        }
                        break;
                    case CheckState.Indeterminate:
                        {
                            Filter<ITestDescriptor> childFilters = CreateFilter(node.Nodes);
                            if (childFilters != null)
                                filters.Add(childFilters);
                            break;
                        }
                }
            }
            return filters.Count > 1 ? new OrFilter<ITestDescriptor>(filters) : filters[0];
        }

        public IList<TestTreeNode> GetSelectedTests()
        {
            if (Root == null || Root.CheckState == CheckState.Unchecked)
                return new List<TestTreeNode>();

            if (Root.CheckState == CheckState.Checked)
                return new List<TestTreeNode>(new[] { Root });

            List <TestTreeNode> selected = new List<TestTreeNode>();
            selected.AddRange(GetSelectedTests(Root));
            return selected;
        }

        public IList<TestTreeNode> GetSelectedTests(TestTreeNode node)
        {
            List<TestTreeNode> selected = new List<TestTreeNode>();

            // special case for namespaces, as they don't really exist!
            // i.e. they don't have an id we can use, so we must add all 
            // their children instead.
            if (node.CheckState == CheckState.Indeterminate || 
                (node.CheckState == CheckState.Checked && node is NamespaceNode))
            {
                foreach (var n in node.Nodes)
                    selected.AddRange(GetSelectedTests((TestTreeNode)n));
            }
            else if (node.CheckState == CheckState.Checked)
                selected.Add(node);
            return selected;
        }
    }
}
