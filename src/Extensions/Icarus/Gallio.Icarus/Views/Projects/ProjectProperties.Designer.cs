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

namespace Gallio.Icarus.Views.Projects
{
    partial class ProjectProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectProperties));
            this.propertiesTabControl = new Gallio.Icarus.Controls.PropertiesTabControl();
            this.hintDirectoriesTabPage = new System.Windows.Forms.TabPage();
            this.hintDirectoriesGroupBox = new System.Windows.Forms.GroupBox();
            this.hintDirectoriesListBox = new System.Windows.Forms.ListBox();
            this.newHintDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.addFolderButton = new System.Windows.Forms.Button();
            this.removeHintDirectoryButton = new System.Windows.Forms.Button();
            this.findHintDirectoryButton = new System.Windows.Forms.Button();
            this.testPackageTabPage = new System.Windows.Forms.TabPage();
            this.testPackageConfigGroupBox = new System.Windows.Forms.GroupBox();
            this.shadowCopyCheckBox = new System.Windows.Forms.CheckBox();
            this.applicationBaseDirectoryLabel = new System.Windows.Forms.Label();
            this.findWorkingDirectoryButton = new System.Windows.Forms.Button();
            this.workingDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.applicationBaseDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.workingDirectoryLabel = new System.Windows.Forms.Label();
            this.findApplicationBaseDirectoryButton = new System.Windows.Forms.Button();
            this.runnerExtensionsTabPage = new System.Windows.Forms.TabPage();
            this.testRunnerExtensionsGroupBox = new System.Windows.Forms.GroupBox();
            this.newExtensionTextBox = new System.Windows.Forms.TextBox();
            this.removeExtensionButton = new System.Windows.Forms.Button();
            this.testRunnerExtensionsListBox = new System.Windows.Forms.ListBox();
            this.addExtensionButton = new System.Windows.Forms.Button();
            this.reportsTabPage = new System.Windows.Forms.TabPage();
            this.reportsGroupBox = new System.Windows.Forms.GroupBox();
            this.reportNameFormatTextBox = new System.Windows.Forms.TextBox();
            this.reportDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.reportNameFormatLabel = new System.Windows.Forms.Label();
            this.reportDirectoryLabel = new System.Windows.Forms.Label();
            this.propertiesTabControl.SuspendLayout();
            this.hintDirectoriesTabPage.SuspendLayout();
            this.hintDirectoriesGroupBox.SuspendLayout();
            this.testPackageTabPage.SuspendLayout();
            this.testPackageConfigGroupBox.SuspendLayout();
            this.runnerExtensionsTabPage.SuspendLayout();
            this.testRunnerExtensionsGroupBox.SuspendLayout();
            this.reportsTabPage.SuspendLayout();
            this.reportsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertiesTabControl
            // 
            this.propertiesTabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.propertiesTabControl.Controls.Add(this.hintDirectoriesTabPage);
            this.propertiesTabControl.Controls.Add(this.testPackageTabPage);
            this.propertiesTabControl.Controls.Add(this.runnerExtensionsTabPage);
            this.propertiesTabControl.Controls.Add(this.reportsTabPage);
            this.propertiesTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.propertiesTabControl.ItemSize = new System.Drawing.Size(35, 125);
            this.propertiesTabControl.Location = new System.Drawing.Point(0, 0);
            this.propertiesTabControl.Multiline = true;
            this.propertiesTabControl.Name = "propertiesTabControl";
            this.propertiesTabControl.SelectedIndex = 0;
            this.propertiesTabControl.Size = new System.Drawing.Size(739, 604);
            this.propertiesTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.propertiesTabControl.TabIndex = 0;
            // 
            // hintDirectoriesTabPage
            // 
            this.hintDirectoriesTabPage.Controls.Add(this.hintDirectoriesGroupBox);
            this.hintDirectoriesTabPage.Location = new System.Drawing.Point(129, 4);
            this.hintDirectoriesTabPage.Name = "hintDirectoriesTabPage";
            this.hintDirectoriesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.hintDirectoriesTabPage.Size = new System.Drawing.Size(606, 596);
            this.hintDirectoriesTabPage.TabIndex = 0;
            this.hintDirectoriesTabPage.Text = "Hint Directories";
            this.hintDirectoriesTabPage.UseVisualStyleBackColor = true;
            // 
            // hintDirectoriesGroupBox
            // 
            this.hintDirectoriesGroupBox.Controls.Add(this.hintDirectoriesListBox);
            this.hintDirectoriesGroupBox.Controls.Add(this.newHintDirectoryTextBox);
            this.hintDirectoriesGroupBox.Controls.Add(this.addFolderButton);
            this.hintDirectoriesGroupBox.Controls.Add(this.removeHintDirectoryButton);
            this.hintDirectoriesGroupBox.Controls.Add(this.findHintDirectoryButton);
            this.hintDirectoriesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hintDirectoriesGroupBox.Location = new System.Drawing.Point(3, 3);
            this.hintDirectoriesGroupBox.Name = "hintDirectoriesGroupBox";
            this.hintDirectoriesGroupBox.Size = new System.Drawing.Size(600, 590);
            this.hintDirectoriesGroupBox.TabIndex = 15;
            this.hintDirectoriesGroupBox.TabStop = false;
            this.hintDirectoriesGroupBox.Text = "Hint Directories";
            // 
            // hintDirectoriesListBox
            // 
            this.hintDirectoriesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hintDirectoriesListBox.FormattingEnabled = true;
            this.hintDirectoriesListBox.Location = new System.Drawing.Point(17, 89);
            this.hintDirectoriesListBox.Name = "hintDirectoriesListBox";
            this.hintDirectoriesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.hintDirectoriesListBox.Size = new System.Drawing.Size(528, 238);
            this.hintDirectoriesListBox.TabIndex = 3;
            this.hintDirectoriesListBox.SelectedIndexChanged += new System.EventHandler(this.hintDirectoriesListBox_SelectedIndexChanged);
            // 
            // newHintDirectoryTextBox
            // 
            this.newHintDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newHintDirectoryTextBox.Location = new System.Drawing.Point(17, 29);
            this.newHintDirectoryTextBox.Name = "newHintDirectoryTextBox";
            this.newHintDirectoryTextBox.Size = new System.Drawing.Size(528, 20);
            this.newHintDirectoryTextBox.TabIndex = 1;
            // 
            // addFolderButton
            // 
            this.addFolderButton.Location = new System.Drawing.Point(17, 55);
            this.addFolderButton.Name = "addFolderButton";
            this.addFolderButton.Size = new System.Drawing.Size(66, 23);
            this.addFolderButton.TabIndex = 4;
            this.addFolderButton.Text = "Add Folder";
            this.addFolderButton.UseVisualStyleBackColor = true;
            this.addFolderButton.Click += new System.EventHandler(this.addFolderButton_Click);
            // 
            // removeHintDirectoryButton
            // 
            this.removeHintDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeHintDirectoryButton.Enabled = false;
            this.removeHintDirectoryButton.Image = ((System.Drawing.Image)(resources.GetObject("removeHintDirectoryButton.Image")));
            this.removeHintDirectoryButton.Location = new System.Drawing.Point(551, 89);
            this.removeHintDirectoryButton.Name = "removeHintDirectoryButton";
            this.removeHintDirectoryButton.Size = new System.Drawing.Size(33, 28);
            this.removeHintDirectoryButton.TabIndex = 5;
            this.removeHintDirectoryButton.UseVisualStyleBackColor = true;
            this.removeHintDirectoryButton.Click += new System.EventHandler(this.removeHintDirectoryButton_Click);
            // 
            // findHintDirectoryButton
            // 
            this.findHintDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findHintDirectoryButton.Location = new System.Drawing.Point(551, 29);
            this.findHintDirectoryButton.Name = "findHintDirectoryButton";
            this.findHintDirectoryButton.Size = new System.Drawing.Size(33, 23);
            this.findHintDirectoryButton.TabIndex = 2;
            this.findHintDirectoryButton.Text = "...";
            this.findHintDirectoryButton.UseVisualStyleBackColor = true;
            this.findHintDirectoryButton.Click += new System.EventHandler(this.findHintDirectoryButton_Click);
            // 
            // testPackageTabPage
            // 
            this.testPackageTabPage.Controls.Add(this.testPackageConfigGroupBox);
            this.testPackageTabPage.Location = new System.Drawing.Point(129, 4);
            this.testPackageTabPage.Name = "testPackageTabPage";
            this.testPackageTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.testPackageTabPage.Size = new System.Drawing.Size(606, 596);
            this.testPackageTabPage.TabIndex = 1;
            this.testPackageTabPage.Text = "Test Package";
            this.testPackageTabPage.UseVisualStyleBackColor = true;
            // 
            // testPackageConfigGroupBox
            // 
            this.testPackageConfigGroupBox.Controls.Add(this.shadowCopyCheckBox);
            this.testPackageConfigGroupBox.Controls.Add(this.applicationBaseDirectoryLabel);
            this.testPackageConfigGroupBox.Controls.Add(this.findWorkingDirectoryButton);
            this.testPackageConfigGroupBox.Controls.Add(this.workingDirectoryTextBox);
            this.testPackageConfigGroupBox.Controls.Add(this.applicationBaseDirectoryTextBox);
            this.testPackageConfigGroupBox.Controls.Add(this.workingDirectoryLabel);
            this.testPackageConfigGroupBox.Controls.Add(this.findApplicationBaseDirectoryButton);
            this.testPackageConfigGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testPackageConfigGroupBox.Location = new System.Drawing.Point(3, 3);
            this.testPackageConfigGroupBox.Name = "testPackageConfigGroupBox";
            this.testPackageConfigGroupBox.Size = new System.Drawing.Size(600, 590);
            this.testPackageConfigGroupBox.TabIndex = 16;
            this.testPackageConfigGroupBox.TabStop = false;
            this.testPackageConfigGroupBox.Text = "Test Package Config";
            // 
            // shadowCopyCheckBox
            // 
            this.shadowCopyCheckBox.AutoSize = true;
            this.shadowCopyCheckBox.Location = new System.Drawing.Point(19, 128);
            this.shadowCopyCheckBox.Name = "shadowCopyCheckBox";
            this.shadowCopyCheckBox.Size = new System.Drawing.Size(91, 17);
            this.shadowCopyCheckBox.TabIndex = 12;
            this.shadowCopyCheckBox.Text = "Shadow copy";
            this.shadowCopyCheckBox.UseVisualStyleBackColor = true;
            // 
            // applicationBaseDirectoryLabel
            // 
            this.applicationBaseDirectoryLabel.AutoSize = true;
            this.applicationBaseDirectoryLabel.Location = new System.Drawing.Point(15, 25);
            this.applicationBaseDirectoryLabel.Name = "applicationBaseDirectoryLabel";
            this.applicationBaseDirectoryLabel.Size = new System.Drawing.Size(131, 13);
            this.applicationBaseDirectoryLabel.TabIndex = 6;
            this.applicationBaseDirectoryLabel.Text = "Application base directory:";
            // 
            // findWorkingDirectoryButton
            // 
            this.findWorkingDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findWorkingDirectoryButton.Location = new System.Drawing.Point(553, 86);
            this.findWorkingDirectoryButton.Name = "findWorkingDirectoryButton";
            this.findWorkingDirectoryButton.Size = new System.Drawing.Size(33, 23);
            this.findWorkingDirectoryButton.TabIndex = 11;
            this.findWorkingDirectoryButton.Text = "...";
            this.findWorkingDirectoryButton.UseVisualStyleBackColor = true;
            this.findWorkingDirectoryButton.Click += new System.EventHandler(this.findWorkingDirectoryButton_Click);
            // 
            // workingDirectoryTextBox
            // 
            this.workingDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.workingDirectoryTextBox.Location = new System.Drawing.Point(19, 89);
            this.workingDirectoryTextBox.Name = "workingDirectoryTextBox";
            this.workingDirectoryTextBox.Size = new System.Drawing.Size(517, 20);
            this.workingDirectoryTextBox.TabIndex = 10;
            // 
            // applicationBaseDirectoryTextBox
            // 
            this.applicationBaseDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationBaseDirectoryTextBox.Location = new System.Drawing.Point(18, 41);
            this.applicationBaseDirectoryTextBox.Name = "applicationBaseDirectoryTextBox";
            this.applicationBaseDirectoryTextBox.Size = new System.Drawing.Size(517, 20);
            this.applicationBaseDirectoryTextBox.TabIndex = 7;
            // 
            // workingDirectoryLabel
            // 
            this.workingDirectoryLabel.AutoSize = true;
            this.workingDirectoryLabel.Location = new System.Drawing.Point(15, 73);
            this.workingDirectoryLabel.Name = "workingDirectoryLabel";
            this.workingDirectoryLabel.Size = new System.Drawing.Size(93, 13);
            this.workingDirectoryLabel.TabIndex = 9;
            this.workingDirectoryLabel.Text = "Working directory:";
            // 
            // findApplicationBaseDirectoryButton
            // 
            this.findApplicationBaseDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findApplicationBaseDirectoryButton.Location = new System.Drawing.Point(552, 38);
            this.findApplicationBaseDirectoryButton.Name = "findApplicationBaseDirectoryButton";
            this.findApplicationBaseDirectoryButton.Size = new System.Drawing.Size(33, 23);
            this.findApplicationBaseDirectoryButton.TabIndex = 8;
            this.findApplicationBaseDirectoryButton.Text = "...";
            this.findApplicationBaseDirectoryButton.UseVisualStyleBackColor = true;
            this.findApplicationBaseDirectoryButton.Click += new System.EventHandler(this.findApplicationBaseDirectoryButton_Click);
            // 
            // runnerExtensionsTabPage
            // 
            this.runnerExtensionsTabPage.Controls.Add(this.testRunnerExtensionsGroupBox);
            this.runnerExtensionsTabPage.Location = new System.Drawing.Point(129, 4);
            this.runnerExtensionsTabPage.Name = "runnerExtensionsTabPage";
            this.runnerExtensionsTabPage.Size = new System.Drawing.Size(606, 596);
            this.runnerExtensionsTabPage.TabIndex = 2;
            this.runnerExtensionsTabPage.Text = "Runner Extensions";
            this.runnerExtensionsTabPage.UseVisualStyleBackColor = true;
            // 
            // testRunnerExtensionsGroupBox
            // 
            this.testRunnerExtensionsGroupBox.Controls.Add(this.newExtensionTextBox);
            this.testRunnerExtensionsGroupBox.Controls.Add(this.removeExtensionButton);
            this.testRunnerExtensionsGroupBox.Controls.Add(this.testRunnerExtensionsListBox);
            this.testRunnerExtensionsGroupBox.Controls.Add(this.addExtensionButton);
            this.testRunnerExtensionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testRunnerExtensionsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.testRunnerExtensionsGroupBox.Name = "testRunnerExtensionsGroupBox";
            this.testRunnerExtensionsGroupBox.Size = new System.Drawing.Size(606, 596);
            this.testRunnerExtensionsGroupBox.TabIndex = 14;
            this.testRunnerExtensionsGroupBox.TabStop = false;
            this.testRunnerExtensionsGroupBox.Text = "Test Runner Extensions";
            // 
            // newExtensionTextBox
            // 
            this.newExtensionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newExtensionTextBox.Location = new System.Drawing.Point(18, 29);
            this.newExtensionTextBox.Name = "newExtensionTextBox";
            this.newExtensionTextBox.Size = new System.Drawing.Size(523, 20);
            this.newExtensionTextBox.TabIndex = 7;
            // 
            // removeExtensionButton
            // 
            this.removeExtensionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeExtensionButton.Enabled = false;
            this.removeExtensionButton.Image = global::Gallio.Icarus.Properties.Resources.cross;
            this.removeExtensionButton.Location = new System.Drawing.Point(560, 98);
            this.removeExtensionButton.Name = "removeExtensionButton";
            this.removeExtensionButton.Size = new System.Drawing.Size(30, 28);
            this.removeExtensionButton.TabIndex = 11;
            this.removeExtensionButton.UseVisualStyleBackColor = true;
            // 
            // testRunnerExtensionsListBox
            // 
            this.testRunnerExtensionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.testRunnerExtensionsListBox.FormattingEnabled = true;
            this.testRunnerExtensionsListBox.Location = new System.Drawing.Point(20, 98);
            this.testRunnerExtensionsListBox.Name = "testRunnerExtensionsListBox";
            this.testRunnerExtensionsListBox.Size = new System.Drawing.Size(523, 251);
            this.testRunnerExtensionsListBox.TabIndex = 9;
            this.testRunnerExtensionsListBox.SelectedIndexChanged += new System.EventHandler(this.testRunnerExtensionsListBox_SelectedIndexChanged);
            // 
            // addExtensionButton
            // 
            this.addExtensionButton.AutoSize = true;
            this.addExtensionButton.Location = new System.Drawing.Point(18, 55);
            this.addExtensionButton.Name = "addExtensionButton";
            this.addExtensionButton.Size = new System.Drawing.Size(85, 23);
            this.addExtensionButton.TabIndex = 10;
            this.addExtensionButton.Text = "Add Extension";
            this.addExtensionButton.UseVisualStyleBackColor = true;
            // 
            // reportsTabPage
            // 
            this.reportsTabPage.Controls.Add(this.reportsGroupBox);
            this.reportsTabPage.Location = new System.Drawing.Point(129, 4);
            this.reportsTabPage.Name = "reportsTabPage";
            this.reportsTabPage.Size = new System.Drawing.Size(606, 596);
            this.reportsTabPage.TabIndex = 3;
            this.reportsTabPage.Text = "Reports";
            this.reportsTabPage.UseVisualStyleBackColor = true;
            // 
            // reportsGroupBox
            // 
            this.reportsGroupBox.Controls.Add(this.reportNameFormatTextBox);
            this.reportsGroupBox.Controls.Add(this.reportDirectoryTextBox);
            this.reportsGroupBox.Controls.Add(this.reportNameFormatLabel);
            this.reportsGroupBox.Controls.Add(this.reportDirectoryLabel);
            this.reportsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.reportsGroupBox.Name = "reportsGroupBox";
            this.reportsGroupBox.Size = new System.Drawing.Size(606, 596);
            this.reportsGroupBox.TabIndex = 0;
            this.reportsGroupBox.TabStop = false;
            this.reportsGroupBox.Text = "Reports";
            // 
            // reportNameFormatTextBox
            // 
            this.reportNameFormatTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.reportNameFormatTextBox.Location = new System.Drawing.Point(21, 124);
            this.reportNameFormatTextBox.Name = "reportNameFormatTextBox";
            this.reportNameFormatTextBox.Size = new System.Drawing.Size(563, 20);
            this.reportNameFormatTextBox.TabIndex = 3;
            // 
            // reportDirectoryTextBox
            // 
            this.reportDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.reportDirectoryTextBox.Location = new System.Drawing.Point(21, 52);
            this.reportDirectoryTextBox.Name = "reportDirectoryTextBox";
            this.reportDirectoryTextBox.ReadOnly = true;
            this.reportDirectoryTextBox.Size = new System.Drawing.Size(563, 20);
            this.reportDirectoryTextBox.TabIndex = 2;
            // 
            // reportNameFormatLabel
            // 
            this.reportNameFormatLabel.AutoSize = true;
            this.reportNameFormatLabel.Location = new System.Drawing.Point(18, 99);
            this.reportNameFormatLabel.Name = "reportNameFormatLabel";
            this.reportNameFormatLabel.Size = new System.Drawing.Size(111, 13);
            this.reportNameFormatLabel.TabIndex = 1;
            this.reportNameFormatLabel.Text = "Report Name Format: ";
            // 
            // reportDirectoryLabel
            // 
            this.reportDirectoryLabel.AutoSize = true;
            this.reportDirectoryLabel.Location = new System.Drawing.Point(18, 26);
            this.reportDirectoryLabel.Name = "reportDirectoryLabel";
            this.reportDirectoryLabel.Size = new System.Drawing.Size(90, 13);
            this.reportDirectoryLabel.TabIndex = 0;
            this.reportDirectoryLabel.Text = "Report Directory: ";
            // 
            // ProjectProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertiesTabControl);
            this.Name = "ProjectProperties";
            this.Size = new System.Drawing.Size(739, 604);
            this.propertiesTabControl.ResumeLayout(false);
            this.hintDirectoriesTabPage.ResumeLayout(false);
            this.hintDirectoriesGroupBox.ResumeLayout(false);
            this.hintDirectoriesGroupBox.PerformLayout();
            this.testPackageTabPage.ResumeLayout(false);
            this.testPackageConfigGroupBox.ResumeLayout(false);
            this.testPackageConfigGroupBox.PerformLayout();
            this.runnerExtensionsTabPage.ResumeLayout(false);
            this.testRunnerExtensionsGroupBox.ResumeLayout(false);
            this.testRunnerExtensionsGroupBox.PerformLayout();
            this.reportsTabPage.ResumeLayout(false);
            this.reportsGroupBox.ResumeLayout(false);
            this.reportsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Gallio.Icarus.Controls.PropertiesTabControl propertiesTabControl;
        private System.Windows.Forms.TabPage hintDirectoriesTabPage;
        private System.Windows.Forms.TabPage testPackageTabPage;
        private System.Windows.Forms.TabPage runnerExtensionsTabPage;
        private System.Windows.Forms.TabPage reportsTabPage;
        private System.Windows.Forms.GroupBox hintDirectoriesGroupBox;
        private System.Windows.Forms.ListBox hintDirectoriesListBox;
        private System.Windows.Forms.TextBox newHintDirectoryTextBox;
        private System.Windows.Forms.Button addFolderButton;
        private System.Windows.Forms.Button removeHintDirectoryButton;
        private System.Windows.Forms.Button findHintDirectoryButton;
        private System.Windows.Forms.GroupBox testPackageConfigGroupBox;
        private System.Windows.Forms.CheckBox shadowCopyCheckBox;
        private System.Windows.Forms.Label applicationBaseDirectoryLabel;
        private System.Windows.Forms.Button findWorkingDirectoryButton;
        private System.Windows.Forms.TextBox workingDirectoryTextBox;
        private System.Windows.Forms.TextBox applicationBaseDirectoryTextBox;
        private System.Windows.Forms.Label workingDirectoryLabel;
        private System.Windows.Forms.Button findApplicationBaseDirectoryButton;
        private System.Windows.Forms.GroupBox testRunnerExtensionsGroupBox;
        private System.Windows.Forms.TextBox newExtensionTextBox;
        private System.Windows.Forms.Button removeExtensionButton;
        private System.Windows.Forms.ListBox testRunnerExtensionsListBox;
        private System.Windows.Forms.Button addExtensionButton;
        private System.Windows.Forms.GroupBox reportsGroupBox;
        private System.Windows.Forms.Label reportDirectoryLabel;
        private System.Windows.Forms.TextBox reportNameFormatTextBox;
        private System.Windows.Forms.TextBox reportDirectoryTextBox;
        private System.Windows.Forms.Label reportNameFormatLabel;


    }
}