// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Gallio.Icarus.Controls;
using Gallio.Icarus.Interfaces;

using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus
{
    public partial class TestExplorer : DockContent
    {
        private IProjectAdapterView projectAdapterView;

        public string TreeFilter
        {
            get
            {
                if (InvokeRequired)
                {
                    string treeFilter = "";
                    Invoke((MethodInvoker)delegate()
                    {
                        treeFilter = TreeFilter;
                    });
                    return treeFilter;
                }
                else
                {
                    return (string)treeViewComboBox.SelectedItem;
                }
            }
        }

        public TestExplorer(IProjectAdapterView projectAdapterView)
        {
            this.projectAdapterView = projectAdapterView;
            InitializeComponent();
            treeViewComboBox.SelectedIndex = 0;
        }

        public void Reset()
        {
            testTree.Reset();
        }

        public void ExpandTree(TestStates state)
        {
            testTree.BeginUpdate();
            testTree.CollapseAll();
            TestNodes(testTree.Nodes[0], state);
            testTree.EndUpdate();
        }

        private void TestNodes(TreeNode node, TestStates state)
        {
            if (node is TestTreeNode)
            {
                if (((TestTreeNode)node).TestState == state)
                    ExpandNode(node);
            }

            // Loop though all the child nodes and expand them if they
            // meet the test state.
            foreach (TreeNode tNode in node.Nodes)
                TestNodes(tNode, state);
        }

        private void ExpandNode(TreeNode node)
        {
            // Loop through all parent nodes that are not already
            // expanded and expand them.
            if (node.Parent != null && !node.Parent.IsExpanded)
                ExpandNode(node.Parent);

            node.Expand();
        }

        public void DataBind(TreeNode[] nodes)
        {
            testTree.Nodes.Clear();
            testTree.Nodes.AddRange(nodes);
        }

        public void UpdateTestState(string testId, TestStates testState)
        {
            testTree.UpdateTestState(testId, testState);
        }

        private void removeAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestTreeNode node = (TestTreeNode)testTree.SelectedNode;
            projectAdapterView.ThreadedRemoveAssembly(node.Name);
        }

        private void testTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            removeAssemblyToolStripMenuItem.Enabled = (e.Node.SelectedImageIndex == 2);
            viewSourceCodeToolStripMenuItem.Enabled = (((TestTreeNode)e.Node).SourceCodeAvailable);
        }

        private void testTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
                projectAdapterView.CreateFilter(testTree.Nodes);
        }

        public TreeNode[] FindNodes(string key)
        {
            return testTree.Nodes.Find(key, true);
        }

        private void treeViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (projectAdapterView != null)
                projectAdapterView.ReloadTree();
        }

        private void filterPassedTestsToolStripButton_Click(object sender, EventArgs e)
        {
            testTree.FilterPassed = filterPassedTestsToolStripMenuItem.Checked = filterPassedTestsToolStripButton.Checked;
        }

        private void filterFailedTestsToolStripButton_Click(object sender, EventArgs e)
        {
            testTree.FilterFailed = filterFailedTestsToolStripMenuItem.Checked = filterFailedTestsToolStripButton.Checked;
        }

        private void filterInconclusiveTestsToolStripButton_Click(object sender, EventArgs e)
        {
            testTree.FilterInconclusive = filterInconclusiveTestsToolStripMenuItem.Checked = filterInconclusiveTestsToolStripButton.Checked;
        }

        private void resetTestsMenuItem_Click(object sender, EventArgs e)
        {
            projectAdapterView.Reset();
        }

        private void expandAllMenuItem_Click(object sender, EventArgs e)
        {
            testTree.ExpandAll();
        }

        private void collapseAllMenuItem_Click(object sender, EventArgs e)
        {
            testTree.CollapseAll();
        }

        private void sortTree_Click(object sender, EventArgs e)
        {
            testTree.Sorted = sortTree.Checked;
        }

        private void viewSourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            projectAdapterView.ViewSourceCode(testTree.SelectedNode.Name);
        }

        private void expandPassedTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExpandTree(TestStates.Success);
        }

        private void expandFailedTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExpandTree(TestStates.Failed);
        }

        private void expandInconclusiveTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExpandTree(TestStates.Inconclusive);
        }

        private void filterPassedTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.FilterPassed = filterPassedTestsToolStripButton.Checked = filterPassedTestsToolStripMenuItem.Checked;
        }

        private void filterFailedTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.FilterFailed = filterFailedTestsToolStripButton.Checked = filterFailedTestsToolStripMenuItem.Checked;
        }

        private void filterInconclusiveTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.FilterInconclusive = filterInconclusiveTestsToolStripButton.Checked = filterInconclusiveTestsToolStripMenuItem.Checked;
        }
    }
}