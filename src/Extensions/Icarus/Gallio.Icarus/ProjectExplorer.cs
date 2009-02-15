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
using Gallio.Utilities;

namespace Gallio.Icarus
{
    public partial class ProjectExplorer : DockWindow
    {
        private readonly IMediator mediator;

        public ProjectExplorer(IMediator mediator)
        {
            this.mediator = mediator;

            InitializeComponent();

            projectTree.Model = mediator.ProjectController.Model;
            projectTree.ExpandAll();

            SetupReportMenus();
        }

        private void SetupReportMenus()
        {
            // add a menu item for each report type (View report as)
            var reportTypes = new List<string>();
            reportTypes.AddRange(mediator.ReportController.ReportTypes);
            reportTypes.Sort();
            foreach (string reportType in reportTypes)
            {
                var menuItem = new ToolStripMenuItem { Text = reportType };
                menuItem.Click += delegate
                {
                    mediator.ConvertSavedReport((string)((Node)projectTree.SelectedNode.Tag).Tag, menuItem.Text);
                };
                viewReportAsToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private void projectTree_SelectionChanged(object sender, EventArgs e)
        {
            Sync.Invoke(this, delegate
                                  {
                                      if (projectTree.SelectedNode != null)
                                      {
                                          Node node = (Node) projectTree.SelectedNode.Tag;
                                          if (node != null && node.Text == "Assemblies")
                                          {
                                              addAssembliesToolStripMenuItem.Visible = true;
                                              addAssembliesToolStripMenuItem.Enabled = true;
                                              removeAssemblyToolStripMenuItem.Visible = true;
                                              removeAssemblyToolStripMenuItem.Enabled = false;
                                              removeAssembliesToolStripMenuItem.Visible = true;
                                              removeAssembliesToolStripMenuItem.Enabled = true;
                                              viewReportAsToolStripMenuItem.Visible = false;
                                              deleteReportToolStripMenuItem.Visible = false;
                                              return;
                                          }
                                          if (projectTree.SelectedNode.Parent != null)
                                          {
                                              node = (Node) projectTree.SelectedNode.Parent.Tag;
                                              if (node != null && node.Text == "Assemblies")
                                              {
                                                  addAssembliesToolStripMenuItem.Visible = true;
                                                  addAssembliesToolStripMenuItem.Enabled = true;
                                                  removeAssemblyToolStripMenuItem.Visible = true;
                                                  removeAssemblyToolStripMenuItem.Enabled = true;
                                                  removeAssembliesToolStripMenuItem.Visible = true;
                                                  removeAssembliesToolStripMenuItem.Enabled = true;
                                                  viewReportAsToolStripMenuItem.Visible = false;
                                                  deleteReportToolStripMenuItem.Visible = false;
                                                  return;
                                              }
                                              if (node != null && node.Text == "Reports")
                                              {
                                                  addAssembliesToolStripMenuItem.Visible = false;
                                                  addAssembliesToolStripMenuItem.Enabled = false;
                                                  removeAssemblyToolStripMenuItem.Visible = false;
                                                  removeAssemblyToolStripMenuItem.Enabled = false;
                                                  removeAssembliesToolStripMenuItem.Visible = false;
                                                  removeAssembliesToolStripMenuItem.Enabled = false;
                                                  viewReportAsToolStripMenuItem.Visible = true;
                                                  deleteReportToolStripMenuItem.Visible = true;
                                                  return;
                                              }
                                          }
                                      }
                                      addAssembliesToolStripMenuItem.Visible = false;
                                      removeAssemblyToolStripMenuItem.Visible = false;
                                      removeAssembliesToolStripMenuItem.Visible = false;
                                      viewReportAsToolStripMenuItem.Visible = false;
                                      deleteReportToolStripMenuItem.Visible = false;
                                  });
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

            if (((Node)projectTree.SelectedNode.Tag).Text == "Properties")
            {
                if (ParentForm != null)
                    ((Main) ParentForm).ShowWindow("propertiesToolStripMenuItem");
            }
            else if (projectTree.SelectedNode.Parent != null && ((Node) projectTree.SelectedNode.Parent.Tag).Text == "Reports")
            {
                OpenReport();
            }
        }

        private void OpenReport()
        {
            Process.Start((string)((Node)projectTree.SelectedNode.Tag).Tag);
        }

        private void deleteReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediator.DeleteReport((string)((Node)projectTree.SelectedNode.Tag).Tag);
        }
    }
}