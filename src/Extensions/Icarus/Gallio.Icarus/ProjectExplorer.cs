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

using Aga.Controls.Tree;
using Gallio.Icarus.Mediator.Interfaces;

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
        }

        private void projectTree_SelectionChanged(object sender, System.EventArgs e)
        {
            if (projectTree.SelectedNode != null)
            {
                Node node = (Node)projectTree.SelectedNode.Tag;
                if (node != null && node.Text == "Assemblies")
                {
                    addAssembliesToolStripMenuItem.Visible = true;
                    addAssembliesToolStripMenuItem.Enabled = true;
                    removeAssemblyToolStripMenuItem.Visible = true;
                    removeAssemblyToolStripMenuItem.Enabled = false;
                    removeAssembliesToolStripMenuItem.Visible = true;
                    removeAssembliesToolStripMenuItem.Enabled = true;
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
                        return;
                    }
                }
            }
            addAssembliesToolStripMenuItem.Visible = false;
            removeAssemblyToolStripMenuItem.Visible = false;
            removeAssembliesToolStripMenuItem.Visible = false;
        }

        private void removeAssemblyToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (projectTree.SelectedNode == null)
                return;

            Node node = (Node)projectTree.SelectedNode.Tag;
            string fileName = (string)node.Tag;
            mediator.RemoveAssembly(fileName);
        }

        private void removeAssembliesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mediator.RemoveAllAssemblies();
        }

        private void addAssembliesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (ParentForm != null)
                ((Main)ParentForm).AddAssembliesToTree();
        }

        private void projectTree_DoubleClick(object sender, System.EventArgs e)
        {
            if (projectTree.SelectedNode != null && ((Node) projectTree.SelectedNode.Tag).Text == "Properties")
                if (ParentForm != null)
                    ((Main)ParentForm).ShowWindow("propertiesToolStripMenuItem");
        }
    }
}