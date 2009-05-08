// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Models
{
    public class TestTreeModel : TreeModelBase, ITestTreeModel
    {
        private readonly TreeModel inner;
        private readonly List<TestStatus> filterStatuses = new List<TestStatus>();
        private readonly TestTreeSorter testTreeSorter;

        public event PropertyChangedEventHandler PropertyChanged;

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
                return filterStatuses.Contains(TestStatus.Skipped);
            }
        }

        public bool SortAsc
        {
            get
            {
                return (testTreeSorter.SortOrder == SortOrder.Ascending);
            }
        }

        public bool SortDesc
        {
            get
            {
                return (testTreeSorter.SortOrder == SortOrder.Descending);
            }
        }

        public int TestCount
        {
            get
            {
                int count = 0;
                foreach (Node node in inner.Nodes)
                    count += CountTests(node);
                return count;
            }
        }

        public int Passed { get; private set; }

        public int Failed { get; private set; }

        public int Skipped { get; private set; }

        public int Inconclusive { get; private set; }

        public TestTreeModel()
        {
            inner = new TreeModel();
            testTreeSorter = new TestTreeSorter { SortOrder = SortOrder.Ascending };

            inner.NodesChanged += (sender, e) => OnNodesChanged(e);
            inner.NodesInserted += (sender, e) => OnNodesInserted(e);
            inner.NodesRemoved += (sender, e) => OnNodesRemoved(e);
            inner.StructureChanged += (sender, e) => OnStructureChanged(e);
        }

        private static int CountTests(Node node)
        {
            int count = 0;
            if (node is TestTreeNode && ((TestTreeNode)node).IsTest && node.IsChecked)
                count += 1;
            foreach (Node n in node.Nodes)
                count += CountTests(n);
            return count;
        }

        public TestTreeNode Root
        {
            get
            {
                if (inner.Root.Nodes.Count > 0)
                    return (TestTreeNode)inner.Root.Nodes[0];
                return null;
            }
        }

        public TreePath GetPath(Node node)
        {
            return inner.GetPath(node);
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
                ResetCounters();

                foreach (Node node in inner.Root.Nodes)
                    ((TestTreeNode)node).Reset();

                OnNodesChanged(new TreeModelEventArgs(TreePath.Empty, new object[] {}));

                FilterTree();
            }
        }

        private void ResetCounters()
        {
            Passed = 0;
            OnPropertyChanged(new PropertyChangedEventArgs("Passed"));
            Failed = 0;
            OnPropertyChanged(new PropertyChangedEventArgs("Failed"));
            Skipped = 0;
            OnPropertyChanged(new PropertyChangedEventArgs("Skipped"));
            Inconclusive = 0;
            OnPropertyChanged(new PropertyChangedEventArgs("Inconclusive"));            
        }

        public void SetFilter(TestStatus testStatus)
        {
            filterStatuses.Add(testStatus);
            FilterTree();
        }

        public void SetSortOrder(SortOrder sortOrder)
        {
            testTreeSorter.SortOrder = sortOrder;
            OnStructureChanged(new TreePathEventArgs(TreePath.Empty));
        }

        public void UpdateTestStatus(TestData testData, TestStepRun testStepRun)
        {
            if (Root == null)
                return;

            var nodes = Root.Find(testData.Id, true);
            foreach (var node in nodes) // there should only be one
            {
                node.AddTestStepRun(testStepRun);
                Filter(node);
            }

            // only update test status count if the test is complete
            if (!testStepRun.Step.IsPrimary || (!testStepRun.Step.IsTestCase && !testData.IsTestCase))
                return;

            switch (testStepRun.Result.Outcome.Status)
            {
                case TestStatus.Passed:
                    Passed++;
                    OnPropertyChanged(new PropertyChangedEventArgs("Passed"));
                    break;
                case TestStatus.Failed:
                    Failed++;
                    OnPropertyChanged(new PropertyChangedEventArgs("Failed"));
                    break;
                case TestStatus.Skipped:
                    Skipped++;
                    OnPropertyChanged(new PropertyChangedEventArgs("Skipped"));
                    break;
                case TestStatus.Inconclusive:
                    Inconclusive++;
                    OnPropertyChanged(new PropertyChangedEventArgs("Inconclusive"));
                    break;
            }
        }

        private void FilterTree()
        {
            foreach (Node node in inner.Root.Nodes)
                Filter(node);

            OnStructureChanged(new TreePathEventArgs(TreePath.Empty));
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
                filterNode = new TestTreeNode(key, key, key) { TestStatus = testStatus };
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

            OnStructureChanged(new TreePathEventArgs(TreePath.Empty));
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (testTreeSorter.SortOrder != SortOrder.None)
            {
                ArrayList list = new ArrayList();
                IEnumerable res = inner.GetChildren(treePath);
                if (res != null)
                {
                    foreach (object obj in res)
                        list.Add(obj);

                    list.Sort(testTreeSorter);
                    return list;
                }
                return null;
            }

            return inner.GetChildren(treePath);
        }

        public override bool IsLeaf(TreePath treePath)
        {
            Node node = FindNode(treePath);
            if (node == null)
                throw new ArgumentException("treePath");
            return node.IsLeaf;
        }

        public void BuildTestTree(IProgressMonitor progressMonitor, TestModelData testModelData, 
            string treeViewCategory)
        {
            int count = CountTestData(testModelData.RootTest);

            using (progressMonitor.BeginTask("Building test tree", count))
            {
                ResetCounters();

                inner.Root.Nodes.Clear();

                TestTreeNode root = new TestTreeNode(testModelData.RootTest.Name, 
                    testModelData.RootTest.Id, TestKinds.Root);

                inner.Root.Nodes.Add(root);

                progressMonitor.Worked(1);

                if (treeViewCategory == "Namespace")
                    PopulateNamespaceTree(progressMonitor, testModelData.RootTest.Children, root);
                else
                    PopulateMetadataTree(progressMonitor, treeViewCategory,
                                         testModelData.RootTest.Children, root);

                OnStructureChanged(new TreePathEventArgs(TreePath.Empty));
            }
        }

        private static int CountTestData(TestData testData)
        {
            int count = 1;
            foreach (var td in testData.Children)
                count += CountTestData(td);
            return count;
        }

        public Node FindNode(TreePath path)
        {
            return inner.FindNode(path);
        }

        private static void PopulateNamespaceTree(IProgressMonitor progressMonitor, 
            IList<TestData> list, TestTreeNode parent)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                string componentKind = td.Metadata.GetValue(MetadataKeys.TestKind);
                if (componentKind == null)
                    continue;
                TestTreeNode ttnode;
                if (componentKind != TestKinds.Fixture)
                {
                    // create an appropriate node
                    ttnode = new TestTreeNode(td.Name, td.Id, componentKind);
                    parent.Nodes.Add(ttnode);
                }
                else
                {
                    // fixtures need special treatment to insert the namespace layer!
                    string @namespace = td.CodeReference.NamespaceName ?? "";

                    // find the namespace node (or add if it doesn't exist)
                    TestTreeNode nsNode;
                    List<TestTreeNode> nodes = parent.Find(@namespace, true);
                    if (nodes.Count > 0)
                        nsNode = nodes[0];
                    else
                    {
                        nsNode = new TestTreeNode(@namespace, @namespace, "Namespace");
                        parent.Nodes.Add(nsNode);
                    }

                    // add the fixture to the namespace
                    ttnode = new TestTreeNode(td.Name, td.Id, componentKind);
                    nsNode.Nodes.Add(ttnode);
                }
                ttnode.SourceCodeAvailable = (td.CodeLocation != CodeLocation.Unknown);
                ttnode.IsTest = td.IsTestCase;

                // process child nodes
                PopulateNamespaceTree(progressMonitor, td.Children, ttnode);

                progressMonitor.Worked(1);
            }
        }

        private void PopulateMetadataTree(IProgressMonitor progressMonitor, string key, 
            IList<TestData> list, Node parent)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TestData td = list[i];
                string componentKind = td.Metadata.GetValue(MetadataKeys.TestKind);
                if (componentKind == null)
                    continue;
                switch (componentKind)
                {
                    case TestKinds.Fixture:
                    case TestKinds.Test:
                        IList<string> metadata = td.Metadata[key];
                        if (metadata.Count == 0)
                            metadata = new List<string> { "None" };

                        foreach (string m in metadata)
                        {
                            // find metadata node (or add if it doesn't exist)
                            TestTreeNode metadataNode;
                            List<TestTreeNode> nodes = Root.Find(m, false);
                            if (nodes.Count > 0)
                                metadataNode = nodes[0];
                            else
                            {
                                metadataNode = new TestTreeNode(m, m, key);
                                Root.Nodes.Add(metadataNode);
                            }

                            // add node in the appropriate place
                            if (componentKind == TestKinds.Fixture)
                            {
                                TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, componentKind);
                                metadataNode.Nodes.Add(ttnode);
                                PopulateMetadataTree(progressMonitor, key, td.Children, ttnode);
                            }
                            else
                            {
                                // test
                                TestTreeNode ttnode = new TestTreeNode(td.Name, td.Id, componentKind)
                                                          {
                                                              SourceCodeAvailable =
                                                                  (td.CodeLocation != CodeLocation.Unknown),
                                                              IsTest = td.IsTestCase
                                                          };
                                if (m != "None")
                                    metadataNode.Nodes.Add(ttnode);
                                else
                                    parent.Nodes.Add(ttnode);
                            }
                        }
                        break;
                }
                if (componentKind != TestKinds.Fixture)
                    PopulateMetadataTree(progressMonitor, key, td.Children, parent);

                progressMonitor.Worked(1);
            }
        }

        public void ApplyFilterSet(FilterSet<ITest> filterSet)
        {
            if (Root == null)
                return;

            if (filterSet.IsEmpty || filterSet.Rules[0].Filter is AnyFilter<ITest>)
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

        private void RecursivelyApplyFilter(Filter<ITest> filter)
        {
            if (filter is NoneFilter<ITest>)
                return;
            if (filter is OrFilter<ITest>)
            {
                OrFilter<ITest> orFilter = (OrFilter<ITest>)filter;
                foreach (Filter<ITest> childFilter in orFilter.Filters)
                    RecursivelyApplyFilter(childFilter);
            }
            else if (filter is PropertyFilter<ITest>)
            {
                PropertyFilter<ITest> propertyFilter = (PropertyFilter<ITest>)filter;
                EqualityFilter<string> equalityFilter = (EqualityFilter<string>)propertyFilter.ValueFilter;
                foreach (TestTreeNode n in Root.Find(equalityFilter.Comparand, true))
                {
                    n.CheckState = CheckState.Checked;
                    n.UpdateStateOfRelatedNodes();
                }
            }
        }

        public FilterSet<ITest> GenerateFilterSetFromSelectedTests()
        {
            if (Root == null || Root.CheckState == CheckState.Checked)
                return FilterSet<ITest>.Empty;

            Filter<ITest> filter = Root.CheckState == CheckState.Unchecked ? new NoneFilter<ITest>() : CreateFilter(inner.Root.Nodes);
            return new FilterSet<ITest>(filter);
        }

        private static Filter<ITest> CreateFilter(IEnumerable<Node> nodes)
        {
            List<Filter<ITest>> filters = new List<Filter<ITest>>();
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
                            switch (node.NodeType)
                            {
                                case TestKinds.Namespace:
                                    filters.Add(new NamespaceFilter<ITest>(equalityFilter));
                                    break;

                                case TestKinds.Fixture:
                                case TestKinds.Test:
                                    filters.Add(new IdFilter<ITest>(equalityFilter));
                                    break;

                                default:
                                    if (typeof(MetadataKeys).GetField(node.NodeType) != null && node.Name != "None")
                                        filters.Add(new MetadataFilter<ITest>(node.NodeType, equalityFilter));
                                    else
                                    {
                                        Filter<ITest> childFilters = CreateFilter(node.Nodes);
                                        if (childFilters != null)
                                            filters.Add(childFilters);
                                    }
                                    break;
                            }
                            break;
                        }
                    case CheckState.Indeterminate:
                        {
                            Filter<ITest> childFilters = CreateFilter(node.Nodes);
                            if (childFilters != null)
                                filters.Add(childFilters);
                            break;
                        }
                }
            }
            return filters.Count > 1 ? new OrFilter<ITest>(filters) : filters[0];
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
                (node.CheckState == CheckState.Checked && node.NodeType == "Namespace"))
            {
                foreach (var n in node.Nodes)
                    selected.AddRange(GetSelectedTests((TestTreeNode)n));
            }
            else if (node.CheckState == CheckState.Checked)
                selected.Add(node);
            return selected;
        }

        private class TestTreeSorter : IComparer
        {
            /// <summary>
            /// Specifies the order in which to sort (i.e. 'Ascending').
            /// </summary>
            private SortOrder sortOrder = SortOrder.None;
            /// <summary>
            /// Case insensitive comparer object
            /// </summary>
            private readonly CaseInsensitiveComparer caseInsensitiveComparer = new CaseInsensitiveComparer();

            /// <summary>
            /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
            /// </summary>
            public SortOrder SortOrder
            {
                get { return sortOrder; }
                set { sortOrder = value; }
            }

            public int Compare(object x, object y)
            {
                // Cast the objects to be compared to ListViewItem objects
                TestTreeNode left = (TestTreeNode)x;
                TestTreeNode right = (TestTreeNode)y;

                // standard text sort (ci)
                int compareResult = caseInsensitiveComparer.Compare(left.Text, right.Text);

                // Calculate correct return value based on object comparison
                if (SortOrder == SortOrder.Ascending)
                {
                    // Ascending sort is selected, return normal result of compare operation
                    return compareResult;
                }
                if (SortOrder == SortOrder.Descending)
                {
                    // Descending sort is selected, return negative result of compare operation
                    return (-compareResult);
                }
                // Return '0' to indicate they are equal
                return 0;
            }
        }
    }
}
