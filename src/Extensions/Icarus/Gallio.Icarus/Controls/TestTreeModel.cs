// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Windows.Forms;

using Aga.Controls.Tree;

using Gallio.Icarus.Controls.Interfaces;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Controls
{
    public class TestTreeModel : TreeModel, ITestTreeModel
    {
        private bool filterPassed, filterFailed, filterSkipped;
        private TestTreeSorter testTreeSorter = new TestTreeSorter();

        public event EventHandler<EventArgs> TestCountChanged;
        public event EventHandler<TestResultEventArgs> TestResult;

        public bool FilterPassed
        {
            set
            {
                filterPassed = value;
                if (value)
                    FilterTree();
                else
                    ClearFilter(TestStatus.Passed);
            }
        }

        public bool FilterFailed
        {
            set
            {
                filterFailed = value;
                if (value)
                    FilterTree();
                else
                    ClearFilter(TestStatus.Failed);
            }
        }

        public bool FilterSkipped
        {
            set
            {
                filterSkipped = value;
                if (value)
                    FilterTree();
                else
                    ClearFilter(TestStatus.Skipped);
            }
        }

        public SortOrder SortOrder
        {
            get { return testTreeSorter.SortOrder; }
            set
            {
                testTreeSorter.SortOrder = value;
                OnStructureChanged(new TreePathEventArgs(TreePath.Empty));
            }
        }

        public int TestCount
        {
            get
            {
                int count = 0;
                foreach (Node node in Nodes)
                    count += CountTests(node);
                return count;
            }
        }

        private int CountTests(Node node)
        {
            int count = 0;
            if (node is TestTreeNode && ((TestTreeNode)node).IsTest && ((TestTreeNode)node).IsChecked)
                count += 1;
            foreach (Node n in node.Nodes)
                count += CountTests(n);
            return count;
        }

        public new TestTreeNode Root
        {
            get { return (TestTreeNode)Nodes[0]; }
        }

        public void ResetTestStatus()
        {
            foreach (Node node in Nodes)
                ResetTestStatus(node);
        }

        private void ResetTestStatus(Node node)
        {
            ((TestTreeNode)node).ClearTestStepRuns();
            foreach (Node n in node.Nodes)
                ResetTestStatus(n);
        }

        public void UpdateTestStatus(TestData testData, TestStepRun testStepRun)
        {
            List<TestTreeNode> nodes = Root.Find(testData.Id, true);
            foreach (TestTreeNode node in nodes)
            {
                node.AddTestStepRun(testStepRun);
                Filter(node);
            }
            if (TestResult != null)
                TestResult(this, new TestResultEventArgs(testData, testStepRun));
        }

        public void FilterTree()
        {
            foreach (Node node in Nodes)
                Filter(node);
        }

        private bool Filter(Node n)
        {
            if (n is TestTreeNode && (filterPassed || filterFailed || filterSkipped))
            {
                TestTreeNode node = (TestTreeNode)n;
                
                // only filter leaf nodes
                if (n.Nodes.Count == 0)
                {
                    switch (node.TestStatus)
                    {
                        case TestStatus.Passed:
                            if (filterPassed)
                            {
                                FilterNode(node, "Passed", TestStatus.Passed, "FilterPassed");
                                return false;
                            }
                            break;
                        case TestStatus.Skipped:
                            if (filterSkipped)
                            {
                                FilterNode(node, "Skipped", TestStatus.Skipped, "FilterSkipped");
                                return false;
                            }
                            break;
                        case TestStatus.Failed:
                            if (filterFailed)
                            {
                                FilterNode(node, "Failed", TestStatus.Failed, "FilterFailed");
                                return false;
                            }
                            break;
                    }
                }
                else if (node.Name != TestStatus.Passed.ToString() && node.Name != TestStatus.Failed.ToString()
                    && node.Name != TestStatus.Inconclusive.ToString())
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

        private void FilterNode(TestTreeNode node, string text, TestStatus testStatus, string nodeType)
        {
            string key = testStatus.ToString();
            TestTreeNode filterNode;
            List<TestTreeNode> nodes = ((TestTreeNode)node.Parent).Find(key, true);
            if (nodes.Count > 0)
                filterNode = nodes[0];
            else
            {
                filterNode = new TestTreeNode(text, key, nodeType);
                filterNode.TestStatus = testStatus;
                node.Parent.Nodes.Add(filterNode);
            }
            node.Parent.Nodes.Remove(node);
            filterNode.Nodes.Add(node);
        }

        private void ClearFilter(TestStatus testStatus)
        {
            foreach (TestTreeNode filterNode in Root.Find(testStatus.ToString(), true))
            {
                Node[] nodes = new Node[filterNode.Nodes.Count];
                filterNode.Nodes.CopyTo(nodes, 0);
                foreach (Node n in nodes)
                    filterNode.Parent.Nodes.Add(n);
                filterNode.Parent.Nodes.Remove(filterNode);
            }
        }

        public void OnTestCountChanged(EventArgs e)
        {
            if (TestCountChanged != null)
                TestCountChanged(this, e);
        }

        public new IEnumerable GetChildren(TreePath treePath)
        {
            if (testTreeSorter.SortOrder != SortOrder.None)
            {
                ArrayList list = new ArrayList();
                IEnumerable res = base.GetChildren(treePath);
                if (res != null)
                {
                    foreach (object obj in res)
                        list.Add(obj);
                    list.Sort(testTreeSorter);
                    return list;
                }
                else
                    return null;
            }
            else
                return base.GetChildren(treePath);
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
            private CaseInsensitiveComparer caseInsensitiveComparer = new CaseInsensitiveComparer();

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
                int compareResult;

                // Cast the objects to be compared to ListViewItem objects
                TestTreeNode X = (TestTreeNode)x;
                TestTreeNode Y = (TestTreeNode)y;

                // standard text sort (ci)
                compareResult = caseInsensitiveComparer.Compare(X.Text, Y.Text);

                // Calculate correct return value based on object comparison
                if (SortOrder == SortOrder.Ascending)
                {
                    // Ascending sort is selected, return normal result of compare operation
                    return compareResult;
                }
                else if (SortOrder == SortOrder.Descending)
                {
                    // Descending sort is selected, return negative result of compare operation
                    return (-compareResult);
                }
                else
                {
                    // Return '0' to indicate they are equal
                    return 0;
                }
            }
        }
    }
}
