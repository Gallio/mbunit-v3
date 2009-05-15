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
using Gallio.Icarus.Mediator.Interfaces;
using Gallio.Icarus.Models.ProjectTreeNodes;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus
{
    public partial class ProjectExplorer : UserControl
    {
        private readonly IProjectController projectController;
        private readonly IReportController reportController;

        public ProjectExplorer(IProjectController projectController, IReportController reportController)
        {
            this.projectController = projectController;

            InitializeComponent();

            projectTree.Model = projectController.Model;
            projectTree.ExpandAll();

            SetupReportMenus();
        }

        private void SetupReportMenus()
        {
            viewReportAsMenuItem.DropDownItems.Clear();

            // add a menu item for each report type (View report as)
            var reportTypes = new List<string>();
            reportTypes.AddRange(reportController.ReportTypes);
            reportTypes.Sort();

            foreach (string reportType in reportTypes)
            {
                var menuItem = new ToolStripMenuItem { Text = reportType };
                menuItem.Click += delegate
                {
                    mediator.ConvertSavedReport((string)((Node)projectTree.SelectedNode.Tag).Tag, menuItem.Text);
                };
                viewReportAsMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private void projectTree_SelectionChanged(object sender, EventArgs e)
        {
            if (projectTree.SelectedNode == null) 
                return;

            Node node = (Node)projectTree.SelectedNode.Tag;
            if (node != null && node.Text == "Assemblies")
                projectTree.ContextMenuStrip = assembliesNodeMenuStrip;
            else if (node is AssemblyNode)
                projectTree.ContextMenuStrip = assemblyNodeMenuStrip;
            else if (node is ReportNode)
                projectTree.ContextMenuStrip = reportNodeMenuStrip;
            else
                projectTree.ContextMenuStrip = null;
        }

        private void removeAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectTree.SelectedNode == null)
                return;

            Node node = (Node)projectTree.SelectedNode.Tag;
            string fileName = (string)node.Tag;
            mediator.RemoveAssembly(fileName);
        }

        private void removeAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediator.RemoveAllAssemblies();
        }

        private void addAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ParentForm != null)
                ((Main)ParentForm).AddAssembliesToTree();
        }

        private void projectTree_DoubleClick(object sender, EventArgs e)
        {
            if (projectTree.SelectedNode == null)
                return;

            var selectedNode = (Node) projectTree.SelectedNode.Tag;
            if (selectedNode is PropertiesNode && ParentForm != null)
                ((Main) ParentForm).ShowWindow("propertiesToolStripMenuItem");
            else if (selectedNode is ReportNode)
                OpenReport();
        }

        private void OpenReport()
        {
            string filename = (string) ((Node) projectTree.SelectedNode.Tag).Tag;
            Process.Start(filename);
        }

        private void deleteReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediator.DeleteReport((string)((Node)projectTree.SelectedNode.Tag).Tag);
        }

        private void propertiesToolStripButton_Click(object sender, EventArgs e)
        {
            if (ParentForm != null) 
                ((Main)ParentForm).ShowWindow("propertiesToolStripMenuItem");
        }
    }
}