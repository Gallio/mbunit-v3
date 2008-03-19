// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Drawing;

using Aga.Controls.Tree;

using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Model;

namespace Gallio.Icarus.Controls
{
    public class TestTreeModel : TreeModel
    {
        private bool filterPassed, filterFailed, filterSkipped;

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

        private Image GetTestStatusIcon(TestStatus testStatus)
        {
            switch (testStatus)
            {
                case TestStatus.Failed:
                    return global::Gallio.Icarus.Properties.Resources.cross;
                case TestStatus.Passed:
                    return global::Gallio.Icarus.Properties.Resources.tick;
                case TestStatus.Skipped:
                    return global::Gallio.Icarus.Properties.Resources.error;
                default:
                    return null;
            }
        }

        public void ResetTestStatus()
        {
            foreach (Node node in Nodes)
            {
                if (node is TestTreeNode)
                    ((TestTreeNode)node).TestStatus = TestStatus.Inconclusive;
            }
        }

        public void UpdateTestStatus(string testId, TestStatus testStatus)
        {
            List<TestTreeNode> nodes = Root.Find(testId, true);
            foreach (TestTreeNode node in nodes)
            {
                node.TestStatus = testStatus;
                node.TestStatusIcon = GetTestStatusIcon(testStatus);
                Filter(node);
            }
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
                                FilterNode(node, "Passed", TestStatus.Passed, global::Gallio.Icarus.Properties.Resources.FilterPassed);
                                return false;
                            }
                            break;
                        case TestStatus.Skipped:
                            if (filterSkipped)
                            {
                                FilterNode(node, "Skipped", TestStatus.Skipped, global::Gallio.Icarus.Properties.Resources.FilterSkipped);
                                return false;
                            }
                            break;
                        case TestStatus.Failed:
                            if (filterFailed)
                            {
                                FilterNode(node, "Failed", TestStatus.Failed, global::Gallio.Icarus.Properties.Resources.FilterFailed);
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

        private void FilterNode(TestTreeNode node, string text, TestStatus testStatus, Image image)
        {
            string key = testStatus.ToString();
            TestTreeNode filterNode;
            List<TestTreeNode> nodes = ((TestTreeNode)node.Parent).Find(key, true);
            if (nodes.Count > 0)
                filterNode = nodes[0];
            else
            {
                filterNode = new TestTreeNode(text, key);
                filterNode.NodeTypeIcon = image;
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

        public void OnTestResult(TestResultEventArgs e)
        {
            if (TestResult != null)
                TestResult(this, e);
        }
    }
}
