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
            this.filesNodeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.propertiesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.reportNodeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewReportAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteReportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNodeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.filesNodeMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.reportNodeMenuStrip.SuspendLayout();
            this.fileNodeMenuStrip.SuspendLayout();
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
            this.projectTree.ContextMenuStrip = this.filesNodeMenuStrip;
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
            // filesNodeMenuStrip
            // 
            this.filesNodeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFilesToolStripMenuItem,
            this.removeAllFilesToolStripMenuItem});
            this.filesNodeMenuStrip.Name = "classTreeMenuStrip";
            this.filesNodeMenuStrip.Size = new System.Drawing.Size(149, 48);
            // 
            // addFilesToolStripMenuItem
            // 
            this.addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
            this.addFilesToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.addFilesToolStripMenuItem.Text = "Add Files...";
            this.addFilesToolStripMenuItem.Click += new System.EventHandler(this.addFilesToolStripMenuItem_Click);
            // 
            // removeAllFilesToolStripMenuItem
            // 
            this.removeAllFilesToolStripMenuItem.Name = "removeAllFilesToolStripMenuItem";
            this.removeAllFilesToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.removeAllFilesToolStripMenuItem.Text = "Remove Files";
            this.removeAllFilesToolStripMenuItem.Click += new System.EventHandler(this.removeAllFilesToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertiesToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(33, 25);
            this.toolStrip1.TabIndex = 5;
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
            // reportNodeMenuStrip
            // 
            this.reportNodeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewReportAsMenuItem,
            this.deleteReportMenuItem});
            this.reportNodeMenuStrip.Name = "classTreeMenuStrip";
            this.reportNodeMenuStrip.Size = new System.Drawing.Size(159, 48);
            // 
            // viewReportAsMenuItem
            // 
            this.viewReportAsMenuItem.Name = "viewReportAsMenuItem";
            this.viewReportAsMenuItem.Size = new System.Drawing.Size(158, 22);
            this.viewReportAsMenuItem.Text = "View Report As";
            // 
            // deleteReportMenuItem
            // 
            this.deleteReportMenuItem.Name = "deleteReportMenuItem";
            this.deleteReportMenuItem.Size = new System.Drawing.Size(158, 22);
            this.deleteReportMenuItem.Text = "Delete Report";
            this.deleteReportMenuItem.Click += new System.EventHandler(this.deleteReportToolStripMenuItem_Click);
            // 
            // fileNodeMenuStrip
            // 
            this.fileNodeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeFileMenuItem});
            this.fileNodeMenuStrip.Name = "classTreeMenuStrip";
            this.fileNodeMenuStrip.Size = new System.Drawing.Size(144, 26);
            // 
            // removeFileMenuItem
            // 
            this.removeFileMenuItem.Name = "removeFileMenuItem";
            this.removeFileMenuItem.Size = new System.Drawing.Size(143, 22);
            this.removeFileMenuItem.Text = "Remove File";
            this.removeFileMenuItem.Click += new System.EventHandler(this.removeFileToolStripMenuItem_Click);
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
            this.filesNodeMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.reportNodeMenuStrip.ResumeLayout(false);
            this.fileNodeMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private Controls.ProjectTreeView projectTree;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ContextMenuStrip filesNodeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllFilesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip reportNodeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewReportAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteReportMenuItem;
        private System.Windows.Forms.ContextMenuStrip fileNodeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeFileMenuItem;
        private System.Windows.Forms.ToolStripButton propertiesToolStripButton;
    }
}