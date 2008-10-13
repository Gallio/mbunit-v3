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
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Model;

namespace Gallio.Icarus
{
    public partial class TestExplorer : DockWindow
    {
        private readonly IProjectController projectController;
        private readonly ITestController testController;

        public TestExplorer(IProjectController projectController, ITestController testController, 
            IOptionsController optionsController)
        {
            this.projectController = projectController;
            this.testController = testController;

            InitializeComponent();

            if (treeViewComboBox.ComboBox != null)
            {
                treeViewComboBox.ComboBox.BindingContext = BindingContext;
                treeViewComboBox.ComboBox.DataSource = optionsController.SelectedTreeViewCategories;
            }
            treeViewComboBox.SelectedIndex = 0;

            testTree.Model = testController.Model;

            testController.LoadStarted += delegate { testTree.EditEnabled = false; };
            testController.LoadFinished += delegate
            {
                testTree.EditEnabled = true;
                testTree.ExpandAll();
            };

            testController.RunStarted += delegate { testTree.EditEnabled = false; };
            testController.RunFinished += delegate { testTree.EditEnabled = true; };

            filterPassedTestsToolStripMenuItem.DataBindings.Add("Checked", testController, "Model.FilterPassed", false, DataSourceUpdateMode.OnPropertyChanged);
            filterPassedTestsToolStripButton.DataBindings.Add("Checked", testController, "Model.FilterPassed", false, DataSourceUpdateMode.OnPropertyChanged);
            filterFailedTestsToolStripMenuItem.DataBindings.Add("Checked", testController, "Model.FilterFailed", false, DataSourceUpdateMode.OnPropertyChanged);
            filterFailedTestsToolStripButton.DataBindings.Add("Checked", testController, "Model.FilterFailed", false, DataSourceUpdateMode.OnPropertyChanged);
            filterSkippedTestsToolStripMenuItem.DataBindings.Add("Checked", testController, "Model.FilterSkipped", false, DataSourceUpdateMode.OnPropertyChanged);
            filterSkippedTestsToolStripButton.DataBindings.Add("Checked", testController, "Model.FilterSkipped", false, DataSourceUpdateMode.OnPropertyChanged);

            sortAscToolStripButton.DataBindings.Add("Checked", testController, "Model.SortAsc", false, DataSourceUpdateMode.OnPropertyChanged);
            sortDescToolStripButton.DataBindings.Add("Checked", testController, "Model.SortDesc", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void removeAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(testTree.SelectedNode.Tag is TestTreeNode))
                return;

            TestTreeNode node = (TestTreeNode)testTree.SelectedNode.Tag;
            projectController.RemoveAssembly(node.Name);
        }

        private void treeViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            testController.TreeViewCategory = (string)treeViewComboBox.SelectedItem;
            testController.Reload();
        }

        private void resetTestsMenuItem_Click(object sender, EventArgs e)
        {
            testController.ResetTests();
        }

        private void expandAllMenuItem_Click(object sender, EventArgs e)
        {
            testTree.ExpandAll();
        }

        private void collapseAllMenuItem_Click(object sender, EventArgs e)
        {
            testTree.CollapseAll();
        }

        private void viewSourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testController.ViewSourceCode(((TestTreeNode)testTree.SelectedNode.Tag).Name);
        }

        private void expandPassedTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Expand(TestStatus.Passed);
        }

        private void expandFailedTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Expand(TestStatus.Failed);
        }

        private void expandInconclusiveTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Expand(TestStatus.Inconclusive);
        }

        private void addAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ParentForm != null)
                ((Main)ParentForm).AddAssembliesToTree();
        }

        private void removeAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            projectController.RemoveAllAssemblies();
        }

        private void testTree_SelectionChanged(object sender, EventArgs e)
        {
            testController.SelectedTests.Clear();
            if (testTree.SelectedNode != null)
            {
                TestTreeNode testTreeNode = (TestTreeNode)testTree.SelectedNode.Tag;
                removeAssemblyToolStripMenuItem.Enabled = testTreeNode.NodeType == TestKinds.Assembly;
                viewSourceCodeToolStripMenuItem.Enabled = testTreeNode.SourceCodeAvailable;
                if (testTreeNode.NodeType == TestKinds.Namespace)
                {
                    foreach (Node n in testTreeNode.Nodes)
                        if (n != null) // don't know how this happens yet -- JB
                            testController.SelectedTests.Add((TestTreeNode)n);
                }
                else
                    testController.SelectedTests.Add(testTreeNode);
            }
            else
            {
                removeAssemblyToolStripMenuItem.Enabled = false;
                viewSourceCodeToolStripMenuItem.Enabled = false;
            }
        }
    }
}