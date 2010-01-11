// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Icarus.TestExplorer
{
    internal partial class View
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(View));
            this.testTree = new Gallio.Icarus.Controls.TestTreeView();
            this.testTreeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.expandMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandPassedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandFailedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandSkippedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterPassedTestsToolStripMenuItem = new Gallio.UI.Controls.ToolStripMenuItem();
            this.filterFailedTestsToolStripMenuItem = new Gallio.UI.Controls.ToolStripMenuItem();
            this.filterInconclusiveTestsToolStripMenuItem = new Gallio.UI.Controls.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectPassedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFailedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectInconclusiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.resetTestsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.treeViewComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.filterPassedTestsToolStripButton = new Gallio.UI.Controls.ToolStripButton();
            this.filterFailedTestsToolStripButton = new Gallio.UI.Controls.ToolStripButton();
            this.filterInconclusiveTestsToolStripButton = new Gallio.UI.Controls.ToolStripButton();
            this.sortAscToolStripButton = new Gallio.UI.Controls.ToolStripButton();
            this.sortDescToolStripButton = new Gallio.UI.Controls.ToolStripButton();
            this.testTreeMenuStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // testTree
            // 
            this.testTree.AllowDrop = true;
            this.testTree.BackColor = System.Drawing.SystemColors.Window;
            this.testTree.ContextMenuStrip = this.testTreeMenuStrip;
            this.testTree.DefaultToolTipProvider = null;
            this.testTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testTree.DragDropMarkColor = System.Drawing.Color.Black;
            this.testTree.EditEnabled = true;
            this.testTree.FailedColor = System.Drawing.Color.Empty;
            this.testTree.InconclusiveColor = System.Drawing.Color.Empty;
            this.testTree.LineColor = System.Drawing.SystemColors.ControlDark;
            this.testTree.Location = new System.Drawing.Point(0, 0);
            this.testTree.Model = null;
            this.testTree.Name = "testTree";
            this.testTree.PassedColor = System.Drawing.Color.Empty;
            this.testTree.SelectedNode = null;
            this.testTree.Size = new System.Drawing.Size(281, 248);
            this.testTree.SkippedColor = System.Drawing.Color.Empty;
            this.testTree.TabIndex = 5;
            this.testTree.SelectionChanged += new System.EventHandler(this.testTree_SelectionChanged);
            this.testTree.DoubleClick += new System.EventHandler(this.testTree_DoubleClick);
            // 
            // testTreeMenuStrip
            // 
            this.testTreeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllMenuItem,
            this.collapseAllMenuItem,
            this.toolStripSeparator7,
            this.expandMenuItem,
            this.filterToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator9,
            this.resetTestsMenuItem,
            this.toolStripSeparator10,
            this.filesToolStripMenuItem,
            this.viewSourceCodeToolStripMenuItem});
            this.testTreeMenuStrip.Name = "classTreeMenuStrip";
            this.testTreeMenuStrip.Size = new System.Drawing.Size(172, 198);
            // 
            // expandAllMenuItem
            // 
            this.expandAllMenuItem.Name = "expandAllMenuItem";
            this.expandAllMenuItem.Size = new System.Drawing.Size(171, 22);
            this.expandAllMenuItem.Text = "&Expand All";
            this.expandAllMenuItem.Click += new System.EventHandler(this.expandAllMenuItem_Click);
            // 
            // collapseAllMenuItem
            // 
            this.collapseAllMenuItem.Name = "collapseAllMenuItem";
            this.collapseAllMenuItem.Size = new System.Drawing.Size(171, 22);
            this.collapseAllMenuItem.Text = "&Collapse All";
            this.collapseAllMenuItem.Click += new System.EventHandler(this.collapseAllMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(168, 6);
            // 
            // expandMenuItem
            // 
            this.expandMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandPassedTestsToolStripMenuItem,
            this.expandFailedTestsToolStripMenuItem,
            this.expandSkippedTestsToolStripMenuItem});
            this.expandMenuItem.Name = "expandMenuItem";
            this.expandMenuItem.Size = new System.Drawing.Size(171, 22);
            this.expandMenuItem.Text = "Expand";
            // 
            // expandPassedTestsToolStripMenuItem
            // 
            this.expandPassedTestsToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.tick;
            this.expandPassedTestsToolStripMenuItem.Name = "expandPassedTestsToolStripMenuItem";
            this.expandPassedTestsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.expandPassedTestsToolStripMenuItem.Text = "Passed";
            this.expandPassedTestsToolStripMenuItem.Click += new System.EventHandler(this.expandPassedTestsToolStripMenuItem_Click);
            // 
            // expandFailedTestsToolStripMenuItem
            // 
            this.expandFailedTestsToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.cross;
            this.expandFailedTestsToolStripMenuItem.Name = "expandFailedTestsToolStripMenuItem";
            this.expandFailedTestsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.expandFailedTestsToolStripMenuItem.Text = "Failed";
            this.expandFailedTestsToolStripMenuItem.Click += new System.EventHandler(this.expandFailedTestsToolStripMenuItem_Click);
            // 
            // expandSkippedTestsToolStripMenuItem
            // 
            this.expandSkippedTestsToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.error;
            this.expandSkippedTestsToolStripMenuItem.Name = "expandSkippedTestsToolStripMenuItem";
            this.expandSkippedTestsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.expandSkippedTestsToolStripMenuItem.Text = "Inconclusive";
            this.expandSkippedTestsToolStripMenuItem.Click += new System.EventHandler(this.expandInconclusiveTestsToolStripMenuItem_Click);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterPassedTestsToolStripMenuItem,
            this.filterFailedTestsToolStripMenuItem,
            this.filterInconclusiveTestsToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.filterToolStripMenuItem.Text = "Filter";
            // 
            // filterPassedTestsToolStripMenuItem
            // 
            this.filterPassedTestsToolStripMenuItem.CheckOnClick = true;
            this.filterPassedTestsToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.FilterPassed;
            this.filterPassedTestsToolStripMenuItem.Name = "filterPassedTestsToolStripMenuItem";
            this.filterPassedTestsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.filterPassedTestsToolStripMenuItem.Text = "Passed";
            // 
            // filterFailedTestsToolStripMenuItem
            // 
            this.filterFailedTestsToolStripMenuItem.CheckOnClick = true;
            this.filterFailedTestsToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.FilterFailed;
            this.filterFailedTestsToolStripMenuItem.Name = "filterFailedTestsToolStripMenuItem";
            this.filterFailedTestsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.filterFailedTestsToolStripMenuItem.Text = "Failed";
            // 
            // filterInconclusiveTestsToolStripMenuItem
            // 
            this.filterInconclusiveTestsToolStripMenuItem.CheckOnClick = true;
            this.filterInconclusiveTestsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("filterInconclusiveTestsToolStripMenuItem.Image")));
            this.filterInconclusiveTestsToolStripMenuItem.Name = "filterInconclusiveTestsToolStripMenuItem";
            this.filterInconclusiveTestsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.filterInconclusiveTestsToolStripMenuItem.Text = "Inconclusive";
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectPassedToolStripMenuItem,
            this.selectFailedToolStripMenuItem,
            this.selectInconclusiveToolStripMenuItem});
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.selectAllToolStripMenuItem.Text = "Select";
            // 
            // selectPassedToolStripMenuItem
            // 
            this.selectPassedToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.tick;
            this.selectPassedToolStripMenuItem.Name = "selectPassedToolStripMenuItem";
            this.selectPassedToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.selectPassedToolStripMenuItem.Text = "Passed";
            this.selectPassedToolStripMenuItem.Click += new System.EventHandler(this.selectPassedToolStripMenuItem_Click);
            // 
            // selectFailedToolStripMenuItem
            // 
            this.selectFailedToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.cross;
            this.selectFailedToolStripMenuItem.Name = "selectFailedToolStripMenuItem";
            this.selectFailedToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.selectFailedToolStripMenuItem.Text = "Failed";
            this.selectFailedToolStripMenuItem.Click += new System.EventHandler(this.selectFailedToolStripMenuItem_Click);
            // 
            // selectInconclusiveToolStripMenuItem
            // 
            this.selectInconclusiveToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.error;
            this.selectInconclusiveToolStripMenuItem.Name = "selectInconclusiveToolStripMenuItem";
            this.selectInconclusiveToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.selectInconclusiveToolStripMenuItem.Text = "Inconclusive";
            this.selectInconclusiveToolStripMenuItem.Click += new System.EventHandler(this.selectInconclusiveToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(168, 6);
            // 
            // resetTestsMenuItem
            // 
            this.resetTestsMenuItem.Name = "resetTestsMenuItem";
            this.resetTestsMenuItem.Size = new System.Drawing.Size(171, 22);
            this.resetTestsMenuItem.Text = "&Reset Tests";
            this.resetTestsMenuItem.Click += new System.EventHandler(this.resetTestsMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(168, 6);
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFilesToolStripMenuItem,
            this.removeFileToolStripMenuItem,
            this.removeAllFilesToolStripMenuItem});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.filesToolStripMenuItem.Text = "Files";
            // 
            // addFilesToolStripMenuItem
            // 
            this.addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
            this.addFilesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.addFilesToolStripMenuItem.Text = "Add Files...";
            this.addFilesToolStripMenuItem.Click += new System.EventHandler(this.addFilesToolStripMenuItem_Click);
            // 
            // removeFileToolStripMenuItem
            // 
            this.removeFileToolStripMenuItem.Enabled = false;
            this.removeFileToolStripMenuItem.Name = "removeFileToolStripMenuItem";
            this.removeFileToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.removeFileToolStripMenuItem.Text = "Remove File";
            this.removeFileToolStripMenuItem.Click += new System.EventHandler(this.removeFileToolStripMenuItem_Click);
            // 
            // removeAllFilesToolStripMenuItem
            // 
            this.removeAllFilesToolStripMenuItem.Name = "removeAllFilesToolStripMenuItem";
            this.removeAllFilesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.removeAllFilesToolStripMenuItem.Text = "Remove All Files";
            this.removeAllFilesToolStripMenuItem.Click += new System.EventHandler(this.removeAllFilesToolStripMenuItem_Click);
            // 
            // viewSourceCodeToolStripMenuItem
            // 
            this.viewSourceCodeToolStripMenuItem.Enabled = false;
            this.viewSourceCodeToolStripMenuItem.Name = "viewSourceCodeToolStripMenuItem";
            this.viewSourceCodeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.viewSourceCodeToolStripMenuItem.Text = "View Source Code";
            this.viewSourceCodeToolStripMenuItem.Click += new System.EventHandler(this.viewSourceCodeToolStripMenuItem_Click);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.AllowDrop = true;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AllowDrop = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.testTree);
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
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.treeViewComboBox,
            this.filterPassedTestsToolStripButton,
            this.filterFailedTestsToolStripButton,
            this.filterInconclusiveTestsToolStripButton,
            this.sortAscToolStripButton,
            this.sortDescToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(278, 25);
            this.toolStrip1.TabIndex = 5;
            // 
            // treeViewComboBox
            // 
            this.treeViewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.treeViewComboBox.Name = "treeViewComboBox";
            this.treeViewComboBox.Size = new System.Drawing.Size(121, 25);
            this.treeViewComboBox.SelectedIndexChanged += new System.EventHandler(this.treeViewComboBox_SelectedIndexChanged);
            // 
            // filterPassedTestsToolStripButton
            // 
            this.filterPassedTestsToolStripButton.CheckOnClick = true;
            this.filterPassedTestsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.filterPassedTestsToolStripButton.Image = global::Gallio.Icarus.Properties.Resources.FilterPassed;
            this.filterPassedTestsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filterPassedTestsToolStripButton.Name = "filterPassedTestsToolStripButton";
            this.filterPassedTestsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.filterPassedTestsToolStripButton.Text = "Filter passed tests";
            // 
            // filterFailedTestsToolStripButton
            // 
            this.filterFailedTestsToolStripButton.CheckOnClick = true;
            this.filterFailedTestsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.filterFailedTestsToolStripButton.Image = global::Gallio.Icarus.Properties.Resources.FilterFailed;
            this.filterFailedTestsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filterFailedTestsToolStripButton.Name = "filterFailedTestsToolStripButton";
            this.filterFailedTestsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.filterFailedTestsToolStripButton.Text = "Filter failed tests";
            // 
            // filterInconclusiveTestsToolStripButton
            // 
            this.filterInconclusiveTestsToolStripButton.CheckOnClick = true;
            this.filterInconclusiveTestsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.filterInconclusiveTestsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("filterInconclusiveTestsToolStripButton.Image")));
            this.filterInconclusiveTestsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filterInconclusiveTestsToolStripButton.Name = "filterInconclusiveTestsToolStripButton";
            this.filterInconclusiveTestsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.filterInconclusiveTestsToolStripButton.Text = "Filter inconclusive tests";
            // 
            // sortAscToolStripButton
            // 
            this.sortAscToolStripButton.Checked = true;
            this.sortAscToolStripButton.CheckOnClick = true;
            this.sortAscToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sortAscToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sortAscToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("sortAscToolStripButton.Image")));
            this.sortAscToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortAscToolStripButton.Name = "sortAscToolStripButton";
            this.sortAscToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.sortAscToolStripButton.Text = "Sort tree (asc)";
            // 
            // sortDescToolStripButton
            // 
            this.sortDescToolStripButton.CheckOnClick = true;
            this.sortDescToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sortDescToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("sortDescToolStripButton.Image")));
            this.sortDescToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortDescToolStripButton.Name = "sortDescToolStripButton";
            this.sortDescToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.sortDescToolStripButton.Text = "Sort tree (desc)";
            // 
            // TestExplorer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 273);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestExplorer";
            this.TabText = "Test Explorer";
            this.Text = "Test Explorer";
            this.testTreeMenuStrip.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private Controls.TestTreeView testTree;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox treeViewComboBox;
        private UI.Controls.ToolStripButton filterPassedTestsToolStripButton;
        private UI.Controls.ToolStripButton filterInconclusiveTestsToolStripButton;
        private UI.Controls.ToolStripButton filterFailedTestsToolStripButton;
        private System.Windows.Forms.ContextMenuStrip testTreeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem expandAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem expandMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem resetTestsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private Gallio.UI.Controls.ToolStripButton sortDescToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem expandPassedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandFailedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandSkippedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewSourceCodeToolStripMenuItem;
        private Gallio.UI.Controls.ToolStripMenuItem filterPassedTestsToolStripMenuItem;
        private Gallio.UI.Controls.ToolStripMenuItem filterFailedTestsToolStripMenuItem;
        private Gallio.UI.Controls.ToolStripMenuItem filterInconclusiveTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllFilesToolStripMenuItem;
        private Gallio.UI.Controls.ToolStripButton sortAscToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectPassedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectFailedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectInconclusiveToolStripMenuItem;
    }
}