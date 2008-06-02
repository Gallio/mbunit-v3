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
    partial class TestExplorer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestExplorer));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
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
            this.filterPassedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterFailedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterSkippedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.resetTestsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.assembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.treeViewComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.filterPassedTestsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.filterFailedTestsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.filterSkippedTestsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sortToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sortUpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.testTreeMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
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
            // testTree
            // 
            this.testTree.BackColor = System.Drawing.SystemColors.Window;
            this.testTree.ContextMenuStrip = this.testTreeMenuStrip;
            this.testTree.DefaultToolTipProvider = null;
            this.testTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testTree.DragDropMarkColor = System.Drawing.Color.Black;
            this.testTree.LineColor = System.Drawing.SystemColors.ControlDark;
            this.testTree.Location = new System.Drawing.Point(0, 0);
            this.testTree.Model = null;
            this.testTree.Name = "testTree";
            this.testTree.SelectedNode = null;
            this.testTree.Size = new System.Drawing.Size(281, 248);
            this.testTree.TabIndex = 5;
            this.testTree.SelectionChanged += new System.EventHandler(this.testTree_SelectionChanged);
            // 
            // testTreeMenuStrip
            // 
            this.testTreeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllMenuItem,
            this.collapseAllMenuItem,
            this.toolStripSeparator7,
            this.expandMenuItem,
            this.filterToolStripMenuItem,
            this.toolStripSeparator9,
            this.resetTestsMenuItem,
            this.toolStripSeparator10,
            this.assembliesToolStripMenuItem,
            this.viewSourceCodeToolStripMenuItem});
            this.testTreeMenuStrip.Name = "classTreeMenuStrip";
            this.testTreeMenuStrip.Size = new System.Drawing.Size(169, 176);
            // 
            // expandAllMenuItem
            // 
            this.expandAllMenuItem.Name = "expandAllMenuItem";
            this.expandAllMenuItem.Size = new System.Drawing.Size(168, 22);
            this.expandAllMenuItem.Text = "&Expand all";
            this.expandAllMenuItem.Click += new System.EventHandler(this.expandAllMenuItem_Click);
            // 
            // collapseAllMenuItem
            // 
            this.collapseAllMenuItem.Name = "collapseAllMenuItem";
            this.collapseAllMenuItem.Size = new System.Drawing.Size(168, 22);
            this.collapseAllMenuItem.Text = "&Collapse all";
            this.collapseAllMenuItem.Click += new System.EventHandler(this.collapseAllMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(165, 6);
            // 
            // expandMenuItem
            // 
            this.expandMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandPassedTestsToolStripMenuItem,
            this.expandFailedTestsToolStripMenuItem,
            this.expandSkippedTestsToolStripMenuItem});
            this.expandMenuItem.Name = "expandMenuItem";
            this.expandMenuItem.Size = new System.Drawing.Size(168, 22);
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
            this.filterSkippedTestsToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.filterToolStripMenuItem.Text = "Filter";
            // 
            // filterPassedTestsToolStripMenuItem
            // 
            this.filterPassedTestsToolStripMenuItem.CheckOnClick = true;
            this.filterPassedTestsToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.FilterPassed;
            this.filterPassedTestsToolStripMenuItem.Name = "filterPassedTestsToolStripMenuItem";
            this.filterPassedTestsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.filterPassedTestsToolStripMenuItem.Text = "Passed";
            this.filterPassedTestsToolStripMenuItem.Click += new System.EventHandler(this.filterPassedTestsToolStripMenuItem_Click);
            // 
            // filterFailedTestsToolStripMenuItem
            // 
            this.filterFailedTestsToolStripMenuItem.CheckOnClick = true;
            this.filterFailedTestsToolStripMenuItem.Image = global::Gallio.Icarus.Properties.Resources.FilterFailed;
            this.filterFailedTestsToolStripMenuItem.Name = "filterFailedTestsToolStripMenuItem";
            this.filterFailedTestsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.filterFailedTestsToolStripMenuItem.Text = "Failed";
            this.filterFailedTestsToolStripMenuItem.Click += new System.EventHandler(this.filterFailedTestsToolStripMenuItem_Click);
            // 
            // filterSkippedTestsToolStripMenuItem
            // 
            this.filterSkippedTestsToolStripMenuItem.CheckOnClick = true;
            this.filterSkippedTestsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("filterSkippedTestsToolStripMenuItem.Image")));
            this.filterSkippedTestsToolStripMenuItem.Name = "filterSkippedTestsToolStripMenuItem";
            this.filterSkippedTestsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.filterSkippedTestsToolStripMenuItem.Text = "Skipped";
            this.filterSkippedTestsToolStripMenuItem.Click += new System.EventHandler(this.filterSkippedTestsToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(165, 6);
            // 
            // resetTestsMenuItem
            // 
            this.resetTestsMenuItem.Name = "resetTestsMenuItem";
            this.resetTestsMenuItem.Size = new System.Drawing.Size(168, 22);
            this.resetTestsMenuItem.Text = "&Reset tests";
            this.resetTestsMenuItem.Click += new System.EventHandler(this.resetTestsMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(165, 6);
            // 
            // assembliesToolStripMenuItem
            // 
            this.assembliesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAssembliesToolStripMenuItem,
            this.removeAssemblyToolStripMenuItem,
            this.removeAssembliesToolStripMenuItem});
            this.assembliesToolStripMenuItem.Name = "assembliesToolStripMenuItem";
            this.assembliesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.assembliesToolStripMenuItem.Text = "Assemblies";
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
            this.removeAssemblyToolStripMenuItem.Enabled = false;
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
            // viewSourceCodeToolStripMenuItem
            // 
            this.viewSourceCodeToolStripMenuItem.Enabled = false;
            this.viewSourceCodeToolStripMenuItem.Name = "viewSourceCodeToolStripMenuItem";
            this.viewSourceCodeToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.viewSourceCodeToolStripMenuItem.Text = "View source code";
            this.viewSourceCodeToolStripMenuItem.Click += new System.EventHandler(this.viewSourceCodeToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.treeViewComboBox,
            this.filterPassedTestsToolStripButton,
            this.filterFailedTestsToolStripButton,
            this.filterSkippedTestsToolStripButton,
            this.sortToolStripButton,
            this.sortUpToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(278, 25);
            this.toolStrip1.TabIndex = 5;
            // 
            // treeViewComboBox
            // 
            this.treeViewComboBox.Items.AddRange(new object[] {
            "Namespaces",
            "Authors",
            "Categories",
            "Importance",
            "TestsOn"});
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
            this.filterPassedTestsToolStripButton.Click += new System.EventHandler(this.filterPassedTestsToolStripButton_Click);
            // 
            // filterFailedTestsToolStripButton
            // 
            this.filterFailedTestsToolStripButton.CheckOnClick = true;
            this.filterFailedTestsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.filterFailedTestsToolStripButton.Image = global::Gallio.Icarus.Properties.Resources.FilterFailed;
            this.filterFailedTestsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filterFailedTestsToolStripButton.Name = "filterFailedTestsToolStripButton";
            this.filterFailedTestsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.filterFailedTestsToolStripButton.Click += new System.EventHandler(this.filterFailedTestsToolStripButton_Click);
            // 
            // filterSkippedTestsToolStripButton
            // 
            this.filterSkippedTestsToolStripButton.CheckOnClick = true;
            this.filterSkippedTestsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.filterSkippedTestsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("filterSkippedTestsToolStripButton.Image")));
            this.filterSkippedTestsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filterSkippedTestsToolStripButton.Name = "filterSkippedTestsToolStripButton";
            this.filterSkippedTestsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.filterSkippedTestsToolStripButton.Click += new System.EventHandler(this.filterSkippedTestsToolStripButton_Click);
            // 
            // sortToolStripButton
            // 
            this.sortToolStripButton.CheckOnClick = true;
            this.sortToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sortToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("sortToolStripButton.Image")));
            this.sortToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortToolStripButton.Name = "sortToolStripButton";
            this.sortToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.sortToolStripButton.Text = "Sort";
            this.sortToolStripButton.Click += new System.EventHandler(this.sortTree_Click);
            // 
            // sortUpToolStripButton
            // 
            this.sortUpToolStripButton.CheckOnClick = true;
            this.sortUpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sortUpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("sortUpToolStripButton.Image")));
            this.sortUpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sortUpToolStripButton.Name = "sortUpToolStripButton";
            this.sortUpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.sortUpToolStripButton.Text = "Sort Up";
            this.sortUpToolStripButton.Click += new System.EventHandler(this.sortUpToolStripButton_Click);
            // 
            // TestExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 273);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestExplorer";
            this.TabText = "Test Explorer";
            this.Text = "Test Explorer";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.testTreeMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private Gallio.Icarus.Controls.TestTreeView testTree;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox treeViewComboBox;
        private System.Windows.Forms.ToolStripButton filterPassedTestsToolStripButton;
        private System.Windows.Forms.ToolStripButton filterSkippedTestsToolStripButton;
        private System.Windows.Forms.ToolStripButton filterFailedTestsToolStripButton;
        private System.Windows.Forms.ContextMenuStrip testTreeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem expandAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem expandMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem resetTestsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripButton sortToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem expandPassedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandFailedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandSkippedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewSourceCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterPassedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterFailedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterSkippedTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assembliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAssembliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAssemblyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAssembliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton sortUpToolStripButton;
    }
}