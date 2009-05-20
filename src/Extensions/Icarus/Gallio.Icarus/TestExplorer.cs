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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Utilities;
using Gallio.Model;

namespace Gallio.Icarus
{
    public partial class TestExplorer : DockWindow
    {
        private readonly IOptionsController optionsController;
        private readonly IProjectController projectController;
        private readonly ITestController testController;
        private readonly ISourceCodeController sourceCodeController;
        private readonly ITaskManager taskManager;
        private readonly bool updateFlag;

        public TestExplorer(IOptionsController optionsController, IProjectController projectController, 
            ITestController testController, ISourceCodeController sourceCodeController, 
            ITaskManager taskManager)
        {
            this.optionsController = optionsController;
            this.projectController = projectController;
            this.testController = testController;
            this.sourceCodeController = sourceCodeController;
            this.taskManager = taskManager;

            InitializeComponent();

            testTree.PassedColor = optionsController.PassedColor;
            testTree.FailedColor = optionsController.FailedColor;
            testTree.InconclusiveColor = optionsController.InconclusiveColor;
            testTree.SkippedColor = optionsController.SkippedColor;

            if (treeViewComboBox.ComboBox != null)
            {
                updateFlag = true;
                treeViewComboBox.ComboBox.BindingContext = BindingContext;
                treeViewComboBox.ComboBox.DataSource = optionsController.SelectedTreeViewCategories;
                treeViewComboBox.ComboBox.DataBindings.Add("SelectedItem", projectController, "TreeViewCategory");
                updateFlag = false;
            }

            testTree.Model = testController.Model;

            testController.ExploreStarted += delegate { testTree.EditEnabled = false; };
            testController.ExploreFinished += delegate
            {
                testTree.EditEnabled = true;
                RestoreState();
            };

            testController.RunStarted += delegate { testTree.EditEnabled = false; };
            testController.RunFinished += delegate { testTree.EditEnabled = true; };

            filterPassedTestsToolStripMenuItem.DataBindings.Add("Checked", testController, "FilterPassed", 
                false, DataSourceUpdateMode.OnPropertyChanged);
            filterPassedTestsToolStripButton.DataBindings.Add("Checked", testController, "FilterPassed", 
                false, DataSourceUpdateMode.OnPropertyChanged);
            filterFailedTestsToolStripMenuItem.DataBindings.Add("Checked", testController, "FilterFailed", 
                false, DataSourceUpdateMode.OnPropertyChanged);
            filterFailedTestsToolStripButton.DataBindings.Add("Checked", testController, "FilterFailed", 
                false, DataSourceUpdateMode.OnPropertyChanged);
            filterInconclusiveTestsToolStripMenuItem.DataBindings.Add("Checked", testController, "FilterInconclusive", 
                false, DataSourceUpdateMode.OnPropertyChanged);
            filterInconclusiveTestsToolStripButton.DataBindings.Add("Checked", testController, "FilterInconclusive", 
                false, DataSourceUpdateMode.OnPropertyChanged);

            sortAscToolStripButton.DataBindings.Add("Checked", testController, "SortAsc", 
                false, DataSourceUpdateMode.OnPropertyChanged);
            sortDescToolStripButton.DataBindings.Add("Checked", testController, "SortDesc", 
                false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public override sealed BindingContext BindingContext
        {
            get { return base.BindingContext; }
            set { base.BindingContext = value; }
        }

        private void removeAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (testTree.SelectedNode == null || !(testTree.SelectedNode.Tag is TestTreeNode))
                return;

            var node = (TestTreeNode)testTree.SelectedNode.Tag;

            var cmd = new RemoveAssemblyCommand(projectController);
            cmd.FileName = node.Name;
            taskManager.QueueTask(cmd);
        }

        private void treeViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            testController.TreeViewCategory = (string)treeViewComboBox.SelectedItem;

            // if updateFlag is set, then the index has changed because
            // we are populating the list, so no need to refresh!
            if (updateFlag)
                return;

            SaveState();

            var cmd = new RefreshTestTreeCommand(testController);
            taskManager.QueueTask(cmd);

            RestoreState();
        }

        private void resetTestsMenuItem_Click(object sender, EventArgs e)
        {
            var cmd = new ResetTestsCommand(testController);
            taskManager.QueueTask(cmd);
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
            ViewSourceCode();
        }

        private void ViewSourceCode()
        {
            if (testTree.SelectedNode == null || !(testTree.SelectedNode.Tag is TestTreeNode))
                return;

            var node = (TestTreeNode)testTree.SelectedNode.Tag;
            var cmd = new ViewSourceCodeCommand(sourceCodeController);
            cmd.TestId = node.Name;
            taskManager.QueueTask(cmd);
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
            using (var openFileDialog = Dialogs.OpenDialog)
            {
                if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                var addAssembliesCommand = new AddAssembliesCommand(projectController, testController);
                addAssembliesCommand.AssemblyFiles = openFileDialog.FileNames;
                taskManager.QueueTask(addAssembliesCommand);
            }
        }

        private void removeAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cmd = new RemoveAllAssembliesCommand(testController, projectController);
            taskManager.QueueTask(cmd);
        }

        private void testTree_SelectionChanged(object sender, EventArgs e)
        {
            var nodes = new List<TestTreeNode>();

            if (testTree.SelectedNode != null)
            {
                TestTreeNode testTreeNode = (TestTreeNode)testTree.SelectedNode.Tag;
                removeAssemblyToolStripMenuItem.Enabled = (testTreeNode.NodeType == TestKinds.Assembly);
                viewSourceCodeToolStripMenuItem.Enabled = testTreeNode.SourceCodeAvailable;

                if (testTreeNode.NodeType == TestKinds.Namespace)
                {
                    foreach (Node n in testTreeNode.Nodes)
                        if (n != null) // don't know how this happens yet -- JB
                        {
                            nodes.Add((TestTreeNode) n);
                        }
                        else
                        {
                            Debugger.Break();
                        }
                }
                else
                {
                    nodes.Add(testTreeNode);
                }
            }
            else
            {
                removeAssemblyToolStripMenuItem.Enabled = false;
                viewSourceCodeToolStripMenuItem.Enabled = false;
            }

            testController.SetSelection(nodes);
        }

        internal void SaveState()
        {
            // save current test selection
            var command = new SaveFilterCommand(testController, projectController, "AutoSave");
            taskManager.QueueTask(command);
            
            // save current category for tree
            projectController.TreeViewCategory = (string)treeViewComboBox.SelectedItem;

            // save state of tree
            projectController.CollapsedNodes = testTree.CollapsedNodes;
        }

        private void RestoreState()
        {
            testTree.CollapseNodes(projectController.CollapsedNodes);
        }

        private void testTree_DoubleClick(object sender, EventArgs e)
        {
            ViewSourceCode();
        }

        private void selectFailedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Select(TestStatus.Failed);
        }

        private void selectPassedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Select(TestStatus.Passed);
        }

        private void selectInconclusiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testTree.Select(TestStatus.Inconclusive);
        }
    }
}
