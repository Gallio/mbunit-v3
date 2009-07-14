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
using Gallio.Common.IO;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models.ProjectTreeNodes;
using Gallio.Icarus.Utilities;
using Gallio.Runtime;
using Gallio.Icarus.Packages;
using Gallio.UI.Progress;

namespace Gallio.Icarus
{
    internal partial class ProjectExplorer : DockWindow
    {
        private readonly IProjectController projectController;
        private readonly ITestController testController;
        private readonly IReportController reportController;
        private readonly ITaskManager taskManager;

        public ProjectExplorer(IProjectController projectController, ITestController testController, 
            IReportController reportController, ITaskManager taskManager)
        {
            this.projectController = projectController;
            this.testController = testController;
            this.taskManager = taskManager;
            this.reportController = reportController;

            InitializeComponent();

            projectTree.Model = projectController.Model;
            projectTree.ExpandAll();

            SetupReportMenus();
        }

        private void SetupReportMenus()
        {
            // add a menu item for each report type (View report as)
            var reportTypes = new List<string>();
            reportTypes.AddRange(reportController.ReportTypes);
            reportTypes.Sort();
            foreach (string reportType in reportTypes)
            {
                var menuItem = new ToolStripMenuItem { Text = reportType };
                menuItem.Click += delegate
                {
                    var reportNode = (ReportNode)projectTree.SelectedNode.Tag;
                    var command = new ConvertSavedReportCommand(reportController, 
                        reportNode.FileName, menuItem.Text, new FileSystem());
                    taskManager.QueueTask(command);
                };
                viewReportAsMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private void projectTree_SelectionChanged(object sender, EventArgs e)
        {
            if (projectTree.SelectedNode == null) 
                return;

            Node node = (Node)projectTree.SelectedNode.Tag;
            if (node is FilesNode)
                projectTree.ContextMenuStrip = filesNodeMenuStrip;
            else if (node is FileNode)
                projectTree.ContextMenuStrip = fileNodeMenuStrip;
            else if (node is ReportNode)
                projectTree.ContextMenuStrip = reportNodeMenuStrip;
            else
                projectTree.ContextMenuStrip = null;
        }

        private void removeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectTree.SelectedNode == null || !(projectTree.SelectedNode.Tag is FileNode))
                return;

            FileNode node = (FileNode)projectTree.SelectedNode.Tag;

            var cmd = new RemoveFileCommand(projectController, testController);
            cmd.FileName = node.FileName;
            taskManager.QueueTask(cmd);
        }

        private void removeAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cmd = new RemoveAllFilesCommand(testController, projectController);
            taskManager.QueueTask(cmd);
        }

        private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = Dialogs.CreateAddFilesDialog())
            {
                if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                var command = new AddFilesCommand(projectController, testController);
                command.Files = openFileDialog.FileNames;
                taskManager.QueueTask(command);
            }
        }

        private void projectTree_DoubleClick(object sender, EventArgs e)
        {
            if (projectTree.SelectedNode == null)
                return;

            var selectedNode = (Node)projectTree.SelectedNode.Tag;

            if (selectedNode is PropertiesNode)
            {
                ShowPropertiesWindow();
            }
            else if (selectedNode is ReportNode)
            {
                OpenReport();
            }
        }

        private void OpenReport()
        {
            if (projectTree.SelectedNode == null || !(projectTree.SelectedNode.Tag is ReportNode))
                return;

            var reportNode = (ReportNode)projectTree.SelectedNode.Tag;
            Process.Start(reportNode.FileName);
        }

        private void deleteReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectTree.SelectedNode == null || !(projectTree.SelectedNode.Tag is ReportNode))
                return;

            var reportNode = (ReportNode)projectTree.SelectedNode.Tag;
            var cmd = new DeleteReportCommand(reportController);
            cmd.FileName = reportNode.FileName;
            taskManager.QueueTask(cmd);
        }

        private void propertiesToolStripButton_Click(object sender, EventArgs e)
        {
            ShowPropertiesWindow();
        }

        private static void ShowPropertiesWindow()
        {
            var windowManager = RuntimeAccessor.ServiceLocator.Resolve<IWindowManager>();
            windowManager.Show(ProjectsPackage.ProjectPropertiesWindowId);
        }
    }
}