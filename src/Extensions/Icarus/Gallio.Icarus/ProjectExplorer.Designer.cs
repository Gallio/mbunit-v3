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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectExplorer));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.projectTree = new Gallio.Icarus.Controls.ProjectTreeView();
            this.assembliesNodeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.reportNodeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewReportAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteReportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assemblyNodeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeAssemblyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.assembliesNodeMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.reportNodeMenuStrip.SuspendLayout();
            this.assemblyNodeMenuStrip.SuspendLayout();
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
            this.projectTree.ContextMenuStrip = this.assembliesNodeMenuStrip;
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
            // assembliesNodeMenuStrip
            // 
            this.assembliesNodeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAssembliesToolStripMenuItem,
            this.removeAssembliesToolStripMenuItem});
            this.assembliesNodeMenuStrip.Name = "classTreeMenuStrip";
            this.assembliesNodeMenuStrip.Size = new System.Drawing.Size(179, 48);
            // 
            // addAssembliesToolStripMenuItem
            // 
            this.addAssembliesToolStripMenuItem.Name = "addAssembliesToolStripMenuItem";
            this.addAssembliesToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.addAssembliesToolStripMenuItem.Text = "Add assemblies...";
            this.addAssembliesToolStripMenuItem.Click += new System.EventHandler(this.addAssembliesToolStripMenuItem_Click);
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
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertiesToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(64, 25);
            this.toolStrip1.TabIndex = 5;
            // 
            // reportNodeMenuStrip
            // 
            this.reportNodeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewReportAsMenuItem,
            this.deleteReportMenuItem});
            this.reportNodeMenuStrip.Name = "classTreeMenuStrip";
            this.reportNodeMenuStrip.Size = new System.Drawing.Size(155, 48);
            // 
            // viewReportAsMenuItem
            // 
            this.viewReportAsMenuItem.Name = "viewReportAsMenuItem";
            this.viewReportAsMenuItem.Size = new System.Drawing.Size(154, 22);
            this.viewReportAsMenuItem.Text = "View report as";
            // 
            // deleteReportMenuItem
            // 
            this.deleteReportMenuItem.Name = "deleteReportMenuItem";
            this.deleteReportMenuItem.Size = new System.Drawing.Size(154, 22);
            this.deleteReportMenuItem.Text = "Delete report";
            this.deleteReportMenuItem.Click += new System.EventHandler(this.deleteReportToolStripMenuItem_Click);
            // 
            // assemblyNodeMenuStrip
            // 
            this.assemblyNodeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeAssemblyMenuItem});
            this.assemblyNodeMenuStrip.Name = "classTreeMenuStrip";
            this.assemblyNodeMenuStrip.Size = new System.Drawing.Size(172, 26);
            // 
            // removeAssemblyMenuItem
            // 
            this.removeAssemblyMenuItem.Name = "removeAssemblyMenuItem";
            this.removeAssemblyMenuItem.Size = new System.Drawing.Size(171, 22);
            this.removeAssemblyMenuItem.Text = "Remove assembly";
            this.removeAssemblyMenuItem.Click += new System.EventHandler(this.removeAssemblyToolStripMenuItem_Click);
            // 
            // propertiesToolStripButton
            // 
            this.propertiesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.propertiesToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("propertiesToolStripButton.Image")));
            this.propertiesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.propertiesToolStripButton.Name = "propertiesToolStripButton";
            this.propertiesToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.propertiesToolStripButton.Click += new System.EventHandler(this.propertiesToolStripButton_Click);
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
            this.assembliesNodeMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.reportNodeMenuStrip.ResumeLayout(false);
            this.assemblyNodeMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private Controls.ProjectTreeView projectTree;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ContextMenuStrip assembliesNodeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addAssembliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAssembliesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip reportNodeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewReportAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteReportMenuItem;
        private System.Windows.Forms.ContextMenuStrip assemblyNodeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeAssemblyMenuItem;
        private System.Windows.Forms.ToolStripButton propertiesToolStripButton;
    }
}