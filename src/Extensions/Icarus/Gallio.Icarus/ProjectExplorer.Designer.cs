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

namespace Gallio.Icarus
{
    partial class ProjectExplorer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.projectTree = new Gallio.Icarus.Controls.ProjectTreeView();
            this.projectTreeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.viewReportAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.projectTreeMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.projectTree);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(281, 248);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(281, 273);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // projectTree
            // 
            this.projectTree.BackColor = System.Drawing.SystemColors.Window;
            this.projectTree.ContextMenuStrip = this.projectTreeMenuStrip;
            this.projectTree.DefaultToolTipProvider = null;
            this.projectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectTree.DragDropMarkColor = System.Drawing.Color.Black;
            this.projectTree.LineColor = System.Drawing.SystemColors.ControlDark;
            this.projectTree.Location = new System.Drawing.Point(0, 0);
            this.projectTree.Model = null;
            this.projectTree.Name = "projectTree";
            this.projectTree.SelectedNode = null;
            this.projectTree.Size = new System.Drawing.Size(281, 248);
            this.projectTree.TabIndex = 5;
            this.projectTree.SelectionChanged += new System.EventHandler(this.projectTree_SelectionChanged);
            this.projectTree.DoubleClick += new System.EventHandler(this.projectTree_DoubleClick);
            // 
            // projectTreeMenuStrip
            // 
            this.projectTreeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAssembliesToolStripMenuItem,
            this.removeAssemblyToolStripMenuItem,
            this.removeAssembliesToolStripMenuItem,
            this.viewReportAsToolStripMenuItem,
            this.deleteReportToolStripMenuItem});
            this.projectTreeMenuStrip.Name = "classTreeMenuStrip";
            this.projectTreeMenuStrip.Size = new System.Drawing.Size(179, 136);
            // 
            // addAssembliesToolStripMenuItem
            // 
            this.addAssembliesToolStripMenuItem.Name = "addAssembliesToolStripMenuItem";
            this.addAssembliesToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.addAssembliesToolStripMenuItem.Text = "Add assemblies...";
            this.addAssembliesToolStripMenuItem.Click += new System.EventHandler(this.addAssembliesToolStripMenuItem_Click);
            // 
            // removeAssemblyToolStripMenuItem
            // 
            this.removeAssemblyToolStripMenuItem.Name = "removeAssemblyToolStripMenuItem";
            this.removeAssemblyToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.removeAssemblyToolStripMenuItem.Text = "Remove assembly";
            this.removeAssemblyToolStripMenuItem.Click += new System.EventHandler(this.removeAssemblyToolStripMenuItem_Click);
            // 
            // removeAssembliesToolStripMenuItem
            // 
            this.removeAssembliesToolStripMenuItem.Name = "removeAssembliesToolStripMenuItem";
            this.removeAssembliesToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.removeAssembliesToolStripMenuItem.Text = "Remove assemblies";
            this.removeAssembliesToolStripMenuItem.Click += new System.EventHandler(this.removeAssembliesToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(109, 25);
            this.toolStrip1.TabIndex = 5;
            // 
            // viewReportAsToolStripMenuItem
            // 
            this.viewReportAsToolStripMenuItem.Name = "viewReportAsToolStripMenuItem";
            this.viewReportAsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.viewReportAsToolStripMenuItem.Text = "View report as";
            // 
            // deleteReportToolStripMenuItem
            // 
            this.deleteReportToolStripMenuItem.Name = "deleteReportToolStripMenuItem";
            this.deleteReportToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.deleteReportToolStripMenuItem.Text = "Delete report";
            this.deleteReportToolStripMenuItem.Click += new System.EventHandler(this.deleteReportToolStripMenuItem_Click);
            // 
            // ProjectExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 273);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "ProjectExplorer";
            this.TabText = "Project Explorer";
            this.Text = "Project Explorer";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.projectTreeMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private Controls.ProjectTreeView projectTree;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ContextMenuStrip projectTreeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeAssemblyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAssembliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAssembliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewReportAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteReportToolStripMenuItem;
    }
}