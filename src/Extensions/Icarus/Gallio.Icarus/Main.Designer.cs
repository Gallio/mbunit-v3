// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using Gallio.Icarus.Controls;
using TestStatusBar = Gallio.Icarus.Controls.TestStatusBar;
using TestTreeView = Gallio.Icarus.Controls.TestTreeView;
using TestResultsList = Gallio.Icarus.Controls.TestResultsList;
using TestResultsGraph = Gallio.Icarus.Controls.TestResultsGraph;

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
            this.fileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assemblyListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performanceMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleInputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleErrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.warningsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.failuresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runtimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlineHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.newProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadToolbarButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.startButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolbarButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.mainToolStrip.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.assembliesToolStripMenuItem,
            this.testsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
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
            // fileExit
            // 
            this.fileExit.Name = "fileExit";
            this.fileExit.Size = new System.Drawing.Size(188, 22);
            this.fileExit.Text = "&Exit";
            this.fileExit.Click += new System.EventHandler(this.fileExit_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testExplorerToolStripMenuItem,
            this.assemblyListToolStripMenuItem,
            this.reportToolStripMenuItem,
            this.performanceMonitorToolStripMenuItem,
            this.testResultsToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.testFiltersToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // testExplorerToolStripMenuItem
            // 
            this.testExplorerToolStripMenuItem.Name = "testExplorerToolStripMenuItem";
            this.testExplorerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.testExplorerToolStripMenuItem.Text = "Test Explorer";
            this.testExplorerToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // assemblyListToolStripMenuItem
            // 
            this.assemblyListToolStripMenuItem.Name = "assemblyListToolStripMenuItem";
            this.assemblyListToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.assemblyListToolStripMenuItem.Text = "Modules";
            this.assemblyListToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // reportToolStripMenuItem
            // 
            this.reportToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reportToolStripMenuItem.Image")));
            this.reportToolStripMenuItem.Name = "reportToolStripMenuItem";
            this.reportToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.reportToolStripMenuItem.Text = "Report";
            this.reportToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // performanceMonitorToolStripMenuItem
            // 
            this.performanceMonitorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("performanceMonitorToolStripMenuItem.Image")));
            this.performanceMonitorToolStripMenuItem.Name = "performanceMonitorToolStripMenuItem";
            this.performanceMonitorToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.performanceMonitorToolStripMenuItem.Text = "Performance monitor";
            this.performanceMonitorToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // testResultsToolStripMenuItem
            // 
            this.testResultsToolStripMenuItem.Name = "testResultsToolStripMenuItem";
            this.testResultsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.testResultsToolStripMenuItem.Text = "Test results";
            this.testResultsToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("propertiesToolStripMenuItem.Image")));
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // testFiltersToolStripMenuItem
            // 
            this.testFiltersToolStripMenuItem.Name = "testFiltersToolStripMenuItem";
            this.testFiltersToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.testFiltersToolStripMenuItem.Text = "Test filters";
            this.testFiltersToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // assembliesToolStripMenuItem
            // 
            this.assembliesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAssemblyToolStripMenuItem,
            this.removeAssembliesToolStripMenuItem,
            this.reloadToolStripMenuItem});
            this.assembliesToolStripMenuItem.Name = "assembliesToolStripMenuItem";
            this.assembliesToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.assembliesToolStripMenuItem.Text = "&Assemblies";
            // 
            // addAssemblyToolStripMenuItem
            // 
            this.addAssemblyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addAssemblyToolStripMenuItem.Image")));
            this.addAssemblyToolStripMenuItem.Name = "addAssemblyToolStripMenuItem";
            this.addAssemblyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.addAssemblyToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.addAssemblyToolStripMenuItem.Text = "&Add Assemblies...";
            this.addAssemblyToolStripMenuItem.Click += new System.EventHandler(this.addAssemblyToolStripMenuItem_Click);
            // 
            // removeAssembliesToolStripMenuItem
            // 
            this.removeAssembliesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeAssembliesToolStripMenuItem.Image")));
            this.removeAssembliesToolStripMenuItem.Name = "removeAssembliesToolStripMenuItem";
            this.removeAssembliesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.removeAssembliesToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.removeAssembliesToolStripMenuItem.Text = "&Remove Assemblies";
            this.removeAssembliesToolStripMenuItem.Click += new System.EventHandler(this.removeAssembliesToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reloadToolStripMenuItem.Image")));
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.reloadToolStripMenuItem.Text = "R&eload";
            // 
            // testsToolStripMenuItem
            // 
            this.testsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTestsToolStripMenuItem,
            this.stopTestsToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.toolStripSeparator9,
            this.windowsToolStripMenuItem});
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
            this.startTestsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.startTestsToolStripMenuItem.Text = "Start";
            this.startTestsToolStripMenuItem.Click += new System.EventHandler(this.startTestsToolStripMenuItem_Click);
            // 
            // stopTestsToolStripMenuItem
            // 
            this.stopTestsToolStripMenuItem.Enabled = false;
            this.stopTestsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("stopTestsToolStripMenuItem.Image")));
            this.stopTestsToolStripMenuItem.Name = "stopTestsToolStripMenuItem";
            this.stopTestsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.stopTestsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.stopTestsToolStripMenuItem.Text = "Stop";
            this.stopTestsToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(153, 6);
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem,
            this.consoleInputToolStripMenuItem,
            this.consoleOutputToolStripMenuItem,
            this.consoleErrorToolStripMenuItem,
            this.debugTraceToolStripMenuItem,
            this.warningsToolStripMenuItem,
            this.failuresToolStripMenuItem,
            this.runtimeToolStripMenuItem});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.windowsToolStripMenuItem.Text = "Windows";
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // consoleInputToolStripMenuItem
            // 
            this.consoleInputToolStripMenuItem.Name = "consoleInputToolStripMenuItem";
            this.consoleInputToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.consoleInputToolStripMenuItem.Text = "Console input";
            this.consoleInputToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // consoleOutputToolStripMenuItem
            // 
            this.consoleOutputToolStripMenuItem.Name = "consoleOutputToolStripMenuItem";
            this.consoleOutputToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.consoleOutputToolStripMenuItem.Text = "Console output";
            this.consoleOutputToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // consoleErrorToolStripMenuItem
            // 
            this.consoleErrorToolStripMenuItem.Name = "consoleErrorToolStripMenuItem";
            this.consoleErrorToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.consoleErrorToolStripMenuItem.Text = "Console error";
            this.consoleErrorToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // debugTraceToolStripMenuItem
            // 
            this.debugTraceToolStripMenuItem.Name = "debugTraceToolStripMenuItem";
            this.debugTraceToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.debugTraceToolStripMenuItem.Text = "Debug trace";
            this.debugTraceToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // warningsToolStripMenuItem
            // 
            this.warningsToolStripMenuItem.Name = "warningsToolStripMenuItem";
            this.warningsToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.warningsToolStripMenuItem.Text = "Warnings";
            this.warningsToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // failuresToolStripMenuItem
            // 
            this.failuresToolStripMenuItem.Name = "failuresToolStripMenuItem";
            this.failuresToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.failuresToolStripMenuItem.Text = "Failures";
            this.failuresToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
            // 
            // runtimeToolStripMenuItem
            // 
            this.runtimeToolStripMenuItem.Name = "runtimeToolStripMenuItem";
            this.runtimeToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.runtimeToolStripMenuItem.Text = "Runtime";
            this.runtimeToolStripMenuItem.Click += new System.EventHandler(this.showWindowToolStripMenuItem_Click);
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
            this.optionsMenuItem.Name = "optionsMenuItem";
            this.optionsMenuItem.Size = new System.Drawing.Size(152, 22);
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
            // mainToolStrip
            // 
            this.mainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripButton,
            this.openProjectToolStripButton,
            this.toolStripSeparator1,
            this.reloadToolbarButton,
            this.toolStripSeparator6,
            this.startButton,
            this.stopButton,
            this.toolStripSeparator2,
            this.helpToolbarButton});
            this.mainToolStrip.Location = new System.Drawing.Point(3, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(256, 25);
            this.mainToolStrip.TabIndex = 3;
            this.mainToolStrip.Text = "Main Menu";
            // 
            // newProjectToolStripButton
            // 
            this.newProjectToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newProjectToolStripButton.Image")));
            this.newProjectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newProjectToolStripButton.Name = "newProjectToolStripButton";
            this.newProjectToolStripButton.Size = new System.Drawing.Size(48, 22);
            this.newProjectToolStripButton.Text = "New";
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
            this.openProjectToolStripButton.Text = "Open";
            this.openProjectToolStripButton.ToolTipText = "Open Project";
            this.openProjectToolStripButton.Click += new System.EventHandler(this.openProject_Click);
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
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
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
            // stopButton
            // 
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Enabled = false;
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(23, 22);
            this.stopButton.Text = "Stop";
            this.stopButton.ToolTipText = "Stop Tests";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // helpToolbarButton
            // 
            this.helpToolbarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolbarButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolbarButton.Image")));
            this.helpToolbarButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpToolbarButton.Name = "helpToolbarButton";
            this.helpToolbarButton.Size = new System.Drawing.Size(23, 22);
            this.helpToolbarButton.Text = "Help";
            this.helpToolbarButton.Click += new System.EventHandler(this.helpToolbarButton_Click);
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1003, 0);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(1003, 25);
            this.toolStripContainer.TabIndex = 5;
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.mainToolStrip);
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockLeftPortion = 0.33;
            this.dockPanel.DockRightPortion = 0.33;
            this.dockPanel.DockTopPortion = 0.15;
            this.dockPanel.Location = new System.Drawing.Point(0, 49);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(1003, 636);
            this.dockPanel.TabIndex = 8;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 707);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolStripContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Main";
            this.Text = "Gallio Icarus {0}.{1}";
            this.Load += new System.EventHandler(this.Form_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
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
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton newProjectToolStripButton;
        private System.Windows.Forms.ToolStripButton openProjectToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton startButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton helpToolbarButton;
        private System.Windows.Forms.ToolStripMenuItem assembliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem addAssemblyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAssembliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlineHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton reloadToolbarButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsMenuItem;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripMenuItem testsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopTestsToolStripMenuItem;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assemblyListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consoleInputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consoleOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consoleErrorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugTraceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem warningsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem failuresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem performanceMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runtimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testFiltersToolStripMenuItem;
    }
}


