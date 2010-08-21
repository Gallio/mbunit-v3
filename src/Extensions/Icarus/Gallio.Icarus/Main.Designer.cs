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

using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.recentProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.testResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.executionLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runtimeLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.annotationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startWithDebuggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlineHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new Gallio.UI.ProgressMonitoring.ToolStripProgressBar();
            this.projectToolStrip = new System.Windows.Forms.ToolStrip();
            this.newProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.testsToolStrip = new System.Windows.Forms.ToolStrip();
            this.startButton = new System.Windows.Forms.ToolStripButton();
            this.startTestsWithDebuggerButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.filesToolStrip = new System.Windows.Forms.ToolStrip();
            this.addFilesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.removeAllFilesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadToolbarButton = new System.Windows.Forms.ToolStripButton();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.projectToolStrip.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.testsToolStrip.SuspendLayout();
            this.filesToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.projectToolStripMenuItem,
            this.testsToolStripMenuItem,
            this.reportToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.ShowItemToolTips = true;
            this.menuStrip.Size = new System.Drawing.Size(1003, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "Main Menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectMenuItem,
            this.toolStripSeparator3,
            this.saveProjectToolStripMenuItem,
            this.saveProjectAsToolStripMenuItem,
            this.toolStripSeparator4,
            this.recentProjectsToolStripMenuItem,
            this.toolStripSeparator12,
            this.fileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProjectToolStripMenuItem.Image")));
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.newProjectToolStripMenuItem.Text = "&New Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // openProjectMenuItem
            // 
            this.openProjectMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openProjectMenuItem.Image")));
            this.openProjectMenuItem.Name = "openProjectMenuItem";
            this.openProjectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openProjectMenuItem.Size = new System.Drawing.Size(188, 22);
            this.openProjectMenuItem.Text = "&Open Project";
            this.openProjectMenuItem.Click += new System.EventHandler(this.openProject_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(185, 6);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveProjectToolStripMenuItem.Image")));
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveProjectToolStripMenuItem.Text = "&Save Project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // saveProjectAsToolStripMenuItem
            // 
            this.saveProjectAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveProjectAsToolStripMenuItem.Image")));
            this.saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
            this.saveProjectAsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveProjectAsToolStripMenuItem.Text = "Save Project &As...";
            this.saveProjectAsToolStripMenuItem.Click += new System.EventHandler(this.saveProjectAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(185, 6);
            // 
            // recentProjectsToolStripMenuItem
            // 
            this.recentProjectsToolStripMenuItem.Name = "recentProjectsToolStripMenuItem";
            this.recentProjectsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.recentProjectsToolStripMenuItem.Text = "Recent Projects";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(185, 6);
            // 
            // fileExit
            // 
            this.fileExit.Name = "fileExit";
            this.fileExit.Size = new System.Drawing.Size(188, 22);
            this.fileExit.Text = "E&xit";
            this.fileExit.Click += new System.EventHandler(this.fileExit_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testExplorerToolStripMenuItem,
            this.projectExplorerToolStripMenuItem,
            this.toolStripSeparator9,
            this.testResultsToolStripMenuItem,
            this.toolStripSeparator10,
            this.executionLogToolStripMenuItem,
            this.runtimeLogToolStripMenuItem,
            this.toolStripSeparator11,
            this.annotationsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // testExplorerToolStripMenuItem
            // 
            this.testExplorerToolStripMenuItem.Name = "testExplorerToolStripMenuItem";
            this.testExplorerToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.testExplorerToolStripMenuItem.Text = "Test Explorer";
            this.testExplorerToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // projectExplorerToolStripMenuItem
            // 
            this.projectExplorerToolStripMenuItem.Name = "projectExplorerToolStripMenuItem";
            this.projectExplorerToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.projectExplorerToolStripMenuItem.Text = "Project Explorer";
            this.projectExplorerToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(159, 6);
            // 
            // testResultsToolStripMenuItem
            // 
            this.testResultsToolStripMenuItem.Name = "testResultsToolStripMenuItem";
            this.testResultsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.testResultsToolStripMenuItem.Text = "Test Results";
            this.testResultsToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(159, 6);
            // 
            // executionLogToolStripMenuItem
            // 
            this.executionLogToolStripMenuItem.Name = "executionLogToolStripMenuItem";
            this.executionLogToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.executionLogToolStripMenuItem.Text = "Execution Log";
            this.executionLogToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // runtimeLogToolStripMenuItem
            // 
            this.runtimeLogToolStripMenuItem.Name = "runtimeLogToolStripMenuItem";
            this.runtimeLogToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.runtimeLogToolStripMenuItem.Text = "Runtime Log";
            this.runtimeLogToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(159, 6);
            // 
            // annotationsToolStripMenuItem
            // 
            this.annotationsToolStripMenuItem.Name = "annotationsToolStripMenuItem";
            this.annotationsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.annotationsToolStripMenuItem.Text = "Annotations";
            this.annotationsToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFilesToolStripMenuItem,
            this.removeAllFilesToolStripMenuItem,
            this.toolStripSeparator8,
            this.reloadToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.projectToolStripMenuItem.Text = "&Project";
            // 
            // addFilesToolStripMenuItem
            // 
            this.addFilesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addFilesToolStripMenuItem.Image")));
            this.addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
            this.addFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.addFilesToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.addFilesToolStripMenuItem.Text = "&Add Files...";
            this.addFilesToolStripMenuItem.Click += new System.EventHandler(this.addFilesToolStripMenuItem_Click);
            // 
            // removeAllFilesToolStripMenuItem
            // 
            this.removeAllFilesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeAllFilesToolStripMenuItem.Image")));
            this.removeAllFilesToolStripMenuItem.Name = "removeAllFilesToolStripMenuItem";
            this.removeAllFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.removeAllFilesToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.removeAllFilesToolStripMenuItem.Text = "&Remove All Files";
            this.removeAllFilesToolStripMenuItem.Click += new System.EventHandler(this.removeAllFiles_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(198, 6);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reloadToolStripMenuItem.Image")));
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.reloadToolStripMenuItem.Text = "R&eload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolbarButton_Click);
            // 
            // testsToolStripMenuItem
            // 
            this.testsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTestsToolStripMenuItem,
            this.startWithDebuggerToolStripMenuItem,
            this.stopTestsToolStripMenuItem,
            this.resetToolStripMenuItem});
            this.testsToolStripMenuItem.Name = "testsToolStripMenuItem";
            this.testsToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.testsToolStripMenuItem.Text = "Tests";
            // 
            // startTestsToolStripMenuItem
            // 
            this.startTestsToolStripMenuItem.Enabled = false;
            this.startTestsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("startTestsToolStripMenuItem.Image")));
            this.startTestsToolStripMenuItem.Name = "startTestsToolStripMenuItem";
            this.startTestsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.startTestsToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.startTestsToolStripMenuItem.Text = "Start";
            this.startTestsToolStripMenuItem.Click += new System.EventHandler(this.startTestsToolStripMenuItem_Click);
            // 
            // startWithDebuggerToolStripMenuItem
            // 
            this.startWithDebuggerToolStripMenuItem.Name = "startWithDebuggerToolStripMenuItem";
            this.startWithDebuggerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.startWithDebuggerToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.startWithDebuggerToolStripMenuItem.Text = "Start With Debugger";
            this.startWithDebuggerToolStripMenuItem.Click += new System.EventHandler(this.startWithDebuggerToolStripMenuItem_Click);
            // 
            // stopTestsToolStripMenuItem
            // 
            this.stopTestsToolStripMenuItem.Enabled = false;
            this.stopTestsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("stopTestsToolStripMenuItem.Image")));
            this.stopTestsToolStripMenuItem.Name = "stopTestsToolStripMenuItem";
            this.stopTestsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.stopTestsToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.stopTestsToolStripMenuItem.Text = "Stop";
            this.stopTestsToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // reportToolStripMenuItem
            // 
            this.reportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewAsToolStripMenuItem});
            this.reportToolStripMenuItem.Name = "reportToolStripMenuItem";
            this.reportToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.reportToolStripMenuItem.Text = "Report";
            // 
            // viewAsToolStripMenuItem
            // 
            this.viewAsToolStripMenuItem.Name = "viewAsToolStripMenuItem";
            this.viewAsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.viewAsToolStripMenuItem.Text = "View As";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("optionsMenuItem.Image")));
            this.optionsMenuItem.Name = "optionsMenuItem";
            this.optionsMenuItem.Size = new System.Drawing.Size(134, 22);
            this.optionsMenuItem.Text = "&Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showOnlineHelpToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // showOnlineHelpToolStripMenuItem
            // 
            this.showOnlineHelpToolStripMenuItem.Name = "showOnlineHelpToolStripMenuItem";
            this.showOnlineHelpToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.showOnlineHelpToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.showOnlineHelpToolStripMenuItem.Text = "Online &Help";
            this.showOnlineHelpToolStripMenuItem.Click += new System.EventHandler(this.showOnlineHelpToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(155, 6);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(158, 22);
            this.aboutMenuItem.Text = "&About...";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.AutoSize = false;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 685);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1003, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.AutoSize = false;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(886, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // projectToolStrip
            // 
            this.projectToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.projectToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripButton,
            this.openProjectToolStripButton,
            this.saveProjectToolStripButton});
            this.projectToolStrip.Location = new System.Drawing.Point(3, 25);
            this.projectToolStrip.Name = "projectToolStrip";
            this.projectToolStrip.Size = new System.Drawing.Size(79, 25);
            this.projectToolStrip.TabIndex = 3;
            this.projectToolStrip.Text = "Project";
            // 
            // newProjectToolStripButton
            // 
            this.newProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newProjectToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newProjectToolStripButton.Image")));
            this.newProjectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newProjectToolStripButton.Name = "newProjectToolStripButton";
            this.newProjectToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newProjectToolStripButton.Text = "New Project";
            this.newProjectToolStripButton.ToolTipText = "New Project";
            this.newProjectToolStripButton.Click += new System.EventHandler(this.newProjectToolStripButton_Click);
            // 
            // openProjectToolStripButton
            // 
            this.openProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openProjectToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openProjectToolStripButton.Image")));
            this.openProjectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openProjectToolStripButton.Name = "openProjectToolStripButton";
            this.openProjectToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openProjectToolStripButton.Text = "Open Project";
            this.openProjectToolStripButton.ToolTipText = "Open Project";
            this.openProjectToolStripButton.Click += new System.EventHandler(this.openProject_Click);
            // 
            // saveProjectToolStripButton
            // 
            this.saveProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveProjectToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveProjectToolStripButton.Image")));
            this.saveProjectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveProjectToolStripButton.Name = "saveProjectToolStripButton";
            this.saveProjectToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveProjectToolStripButton.Text = "Save Project";
            this.saveProjectToolStripButton.Click += new System.EventHandler(this.saveProjectToolStripButton_Click);
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.dockPanel);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1003, 611);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(1003, 661);
            this.toolStripContainer.TabIndex = 5;
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.projectToolStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.filesToolStrip);
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.testsToolStrip);
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.AllowDrop = true;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockLeftPortion = 0.33;
            this.dockPanel.DockRightPortion = 0.33;
            this.dockPanel.DockTopPortion = 0.15;
            this.dockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.dockPanel.Location = new System.Drawing.Point(0, 0);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(1003, 611);
            this.dockPanel.TabIndex = 8;
            // 
            // testsToolStrip
            // 
            this.testsToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.testsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startButton,
            this.startTestsWithDebuggerButton,
            this.stopButton});
            this.testsToolStrip.Location = new System.Drawing.Point(3, 0);
            this.testsToolStrip.Name = "testsToolStrip";
            this.testsToolStrip.Size = new System.Drawing.Size(168, 25);
            this.testsToolStrip.TabIndex = 12;
            this.testsToolStrip.Text = "Tests";
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Image = ((System.Drawing.Image)(resources.GetObject("startButton.Image")));
            this.startButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(51, 22);
            this.startButton.Text = "Start";
            this.startButton.ToolTipText = "Start Tests";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // startTestsWithDebuggerButton
            // 
            this.startTestsWithDebuggerButton.Enabled = false;
            this.startTestsWithDebuggerButton.Image = ((System.Drawing.Image)(resources.GetObject("startTestsWithDebuggerButton.Image")));
            this.startTestsWithDebuggerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startTestsWithDebuggerButton.Name = "startTestsWithDebuggerButton";
            this.startTestsWithDebuggerButton.Size = new System.Drawing.Size(58, 22);
            this.startTestsWithDebuggerButton.Text = "Debug";
            this.startTestsWithDebuggerButton.ToolTipText = "Start Tests With Debugger";
            this.startTestsWithDebuggerButton.Click += new System.EventHandler(this.startWithDebuggerToolStripMenuItem_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(49, 22);
            this.stopButton.Text = "Stop";
            this.stopButton.ToolTipText = "Stop Tests";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // filesToolStrip
            // 
            this.filesToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.filesToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFilesToolStripButton,
            this.removeAllFilesToolStripButton,
            this.toolStripSeparator1,
            this.reloadToolbarButton});
            this.filesToolStrip.Location = new System.Drawing.Point(82, 25);
            this.filesToolStrip.Name = "filesToolStrip";
            this.filesToolStrip.Size = new System.Drawing.Size(250, 25);
            this.filesToolStrip.TabIndex = 11;
            this.filesToolStrip.Text = "Files";
            // 
            // addFilesToolStripButton
            // 
            this.addFilesToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("addFilesToolStripButton.Image")));
            this.addFilesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addFilesToolStripButton.Name = "addFilesToolStripButton";
            this.addFilesToolStripButton.Size = new System.Drawing.Size(70, 22);
            this.addFilesToolStripButton.Text = "Add Files";
            this.addFilesToolStripButton.Click += new System.EventHandler(this.addFilesToolStripButton_Click);
            // 
            // removeAllFilesToolStripButton
            // 
            this.removeAllFilesToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("removeAllFilesToolStripButton.Image")));
            this.removeAllFilesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeAllFilesToolStripButton.Name = "removeAllFilesToolStripButton";
            this.removeAllFilesToolStripButton.Size = new System.Drawing.Size(104, 22);
            this.removeAllFilesToolStripButton.Text = "Remove All Files";
            this.removeAllFilesToolStripButton.Click += new System.EventHandler(this.removeAllFiles_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // reloadToolbarButton
            // 
            this.reloadToolbarButton.Image = ((System.Drawing.Image)(resources.GetObject("reloadToolbarButton.Image")));
            this.reloadToolbarButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadToolbarButton.Name = "reloadToolbarButton";
            this.reloadToolbarButton.Size = new System.Drawing.Size(60, 22);
            this.reloadToolbarButton.Text = "Reload";
            this.reloadToolbarButton.Click += new System.EventHandler(this.reloadToolbarButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 707);
            this.Controls.Add(this.toolStripContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Main";
            this.Text = "Gallio Icarus {0}.{1}.{2} build {3}";
            this.Load += new System.EventHandler(this.Form_Load);
            this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.projectToolStrip.ResumeLayout(false);
            this.projectToolStrip.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.testsToolStrip.ResumeLayout(false);
            this.testsToolStrip.PerformLayout();
            this.filesToolStrip.ResumeLayout(false);
            this.filesToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileExit;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStrip projectToolStrip;
        private System.Windows.Forms.ToolStripButton newProjectToolStripButton;
        private System.Windows.Forms.ToolStripButton openProjectToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem addFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlineHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsMenuItem;
        private ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripMenuItem testsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopTestsToolStripMenuItem;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem executionLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runtimeLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem annotationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem reportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startWithDebuggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentProjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton saveProjectToolStripButton;
        private System.Windows.Forms.ToolStrip filesToolStrip;
        private System.Windows.Forms.ToolStripButton addFilesToolStripButton;
        private System.Windows.Forms.ToolStripButton removeAllFilesToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton reloadToolbarButton;
        private System.Windows.Forms.ToolStrip testsToolStrip;
        private System.Windows.Forms.ToolStripButton startButton;
        private System.Windows.Forms.ToolStripButton startTestsWithDebuggerButton;
        private System.Windows.Forms.ToolStripButton stopButton;
    }
}


