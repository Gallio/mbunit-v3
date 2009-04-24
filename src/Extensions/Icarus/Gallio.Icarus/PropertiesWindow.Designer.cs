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
    partial class PropertiesWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesWindow));
            this.newHintDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.findHintDirectoryButton = new System.Windows.Forms.Button();
            this.hintDirectoriesListBox = new System.Windows.Forms.ListBox();
            this.addFolderButton = new System.Windows.Forms.Button();
            this.removeHintDirectoryButton = new System.Windows.Forms.Button();
            this.applicationBaseDirectoryLabel = new System.Windows.Forms.Label();
            this.findApplicationBaseDirectoryButton = new System.Windows.Forms.Button();
            this.applicationBaseDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.findWorkingDirectoryButton = new System.Windows.Forms.Button();
            this.workingDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.workingDirectoryLabel = new System.Windows.Forms.Label();
            this.shadowCopyCheckBox = new System.Windows.Forms.CheckBox();
            this.testRunnerExtensionsGroupBox = new System.Windows.Forms.GroupBox();
            this.newExtensionTextBox = new System.Windows.Forms.TextBox();
            this.removeExtensionButton = new System.Windows.Forms.Button();
            this.testRunnerExtensionsListBox = new System.Windows.Forms.ListBox();
            this.addExtensionButton = new System.Windows.Forms.Button();
            this.hintDirectoriesGroupBox = new System.Windows.Forms.GroupBox();
            this.testPackageConfigGroupBox = new System.Windows.Forms.GroupBox();
            this.testRunnerExtensionsGroupBox.SuspendLayout();
            this.hintDirectoriesGroupBox.SuspendLayout();
            this.testPackageConfigGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // newHintDirectoryTextBox
            // 
            this.newHintDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newHintDirectoryTextBox.Location = new System.Drawing.Point(18, 19);
            this.newHintDirectoryTextBox.Name = "newHintDirectoryTextBox";
            this.newHintDirectoryTextBox.Size = new System.Drawing.Size(585, 20);
            this.newHintDirectoryTextBox.TabIndex = 1;
            // 
            // findHintDirectoryButton
            // 
            this.findHintDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findHintDirectoryButton.Location = new System.Drawing.Point(609, 19);
            this.findHintDirectoryButton.Name = "findHintDirectoryButton";
            this.findHintDirectoryButton.Size = new System.Drawing.Size(33, 23);
            this.findHintDirectoryButton.TabIndex = 2;
            this.findHintDirectoryButton.Text = "...";
            this.findHintDirectoryButton.UseVisualStyleBackColor = true;
            this.findHintDirectoryButton.Click += new System.EventHandler(this.findHintDirectoryButton_Click);
            // 
            // hintDirectoriesListBox
            // 
            this.hintDirectoriesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hintDirectoriesListBox.FormattingEnabled = true;
            this.hintDirectoriesListBox.Location = new System.Drawing.Point(18, 79);
            this.hintDirectoriesListBox.Name = "hintDirectoriesListBox";
            this.hintDirectoriesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.hintDirectoriesListBox.Size = new System.Drawing.Size(585, 95);
            this.hintDirectoriesListBox.TabIndex = 3;
            this.hintDirectoriesListBox.SelectedIndexChanged += new System.EventHandler(this.hintDirectoriesListBox_SelectedIndexChanged);
            // 
            // addFolderButton
            // 
            this.addFolderButton.Location = new System.Drawing.Point(18, 45);
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
            this.removeHintDirectoryButton.Location = new System.Drawing.Point(609, 79);
            this.removeHintDirectoryButton.Name = "removeHintDirectoryButton";
            this.removeHintDirectoryButton.Size = new System.Drawing.Size(33, 28);
            this.removeHintDirectoryButton.TabIndex = 5;
            this.removeHintDirectoryButton.UseVisualStyleBackColor = true;
            this.removeHintDirectoryButton.Click += new System.EventHandler(this.removeHintDirectoryButton_Click);
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
            // findApplicationBaseDirectoryButton
            // 
            this.findApplicationBaseDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findApplicationBaseDirectoryButton.Location = new System.Drawing.Point(609, 38);
            this.findApplicationBaseDirectoryButton.Name = "findApplicationBaseDirectoryButton";
            this.findApplicationBaseDirectoryButton.Size = new System.Drawing.Size(33, 23);
            this.findApplicationBaseDirectoryButton.TabIndex = 8;
            this.findApplicationBaseDirectoryButton.Text = "...";
            this.findApplicationBaseDirectoryButton.UseVisualStyleBackColor = true;
            this.findApplicationBaseDirectoryButton.Click += new System.EventHandler(this.findApplicationBaseDirectoryButton_Click);
            // 
            // applicationBaseDirectoryTextBox
            // 
            this.applicationBaseDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.applicationBaseDirectoryTextBox.Location = new System.Drawing.Point(18, 41);
            this.applicationBaseDirectoryTextBox.Name = "applicationBaseDirectoryTextBox";
            this.applicationBaseDirectoryTextBox.Size = new System.Drawing.Size(585, 20);
            this.applicationBaseDirectoryTextBox.TabIndex = 7;
            // 
            // findWorkingDirectoryButton
            // 
            this.findWorkingDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findWorkingDirectoryButton.Location = new System.Drawing.Point(609, 80);
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
            this.workingDirectoryTextBox.Location = new System.Drawing.Point(18, 83);
            this.workingDirectoryTextBox.Name = "workingDirectoryTextBox";
            this.workingDirectoryTextBox.Size = new System.Drawing.Size(585, 20);
            this.workingDirectoryTextBox.TabIndex = 10;
            // 
            // workingDirectoryLabel
            // 
            this.workingDirectoryLabel.AutoSize = true;
            this.workingDirectoryLabel.Location = new System.Drawing.Point(14, 67);
            this.workingDirectoryLabel.Name = "workingDirectoryLabel";
            this.workingDirectoryLabel.Size = new System.Drawing.Size(93, 13);
            this.workingDirectoryLabel.TabIndex = 9;
            this.workingDirectoryLabel.Text = "Working directory:";
            // 
            // shadowCopyCheckBox
            // 
            this.shadowCopyCheckBox.AutoSize = true;
            this.shadowCopyCheckBox.Location = new System.Drawing.Point(19, 116);
            this.shadowCopyCheckBox.Name = "shadowCopyCheckBox";
            this.shadowCopyCheckBox.Size = new System.Drawing.Size(91, 17);
            this.shadowCopyCheckBox.TabIndex = 12;
            this.shadowCopyCheckBox.Text = "Shadow copy";
            this.shadowCopyCheckBox.UseVisualStyleBackColor = true;
            // 
            // testRunnerExtensionsGroupBox
            // 
            this.testRunnerExtensionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.testRunnerExtensionsGroupBox.Controls.Add(this.newExtensionTextBox);
            this.testRunnerExtensionsGroupBox.Controls.Add(this.removeExtensionButton);
            this.testRunnerExtensionsGroupBox.Controls.Add(this.testRunnerExtensionsListBox);
            this.testRunnerExtensionsGroupBox.Controls.Add(this.addExtensionButton);
            this.testRunnerExtensionsGroupBox.Location = new System.Drawing.Point(12, 371);
            this.testRunnerExtensionsGroupBox.Name = "testRunnerExtensionsGroupBox";
            this.testRunnerExtensionsGroupBox.Size = new System.Drawing.Size(657, 191);
            this.testRunnerExtensionsGroupBox.TabIndex = 13;
            this.testRunnerExtensionsGroupBox.TabStop = false;
            this.testRunnerExtensionsGroupBox.Text = "Test Runner Extensions";
            // 
            // newExtensionTextBox
            // 
            this.newExtensionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newExtensionTextBox.Location = new System.Drawing.Point(18, 19);
            this.newExtensionTextBox.Name = "newExtensionTextBox";
            this.newExtensionTextBox.Size = new System.Drawing.Size(585, 20);
            this.newExtensionTextBox.TabIndex = 7;
            // 
            // removeExtensionButton
            // 
            this.removeExtensionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeExtensionButton.Enabled = false;
            this.removeExtensionButton.Image = global::Gallio.Icarus.Properties.Resources.cross;
            this.removeExtensionButton.Location = new System.Drawing.Point(609, 74);
            this.removeExtensionButton.Name = "removeExtensionButton";
            this.removeExtensionButton.Size = new System.Drawing.Size(30, 28);
            this.removeExtensionButton.TabIndex = 11;
            this.removeExtensionButton.UseVisualStyleBackColor = true;
            this.removeExtensionButton.Click += new System.EventHandler(this.removeExtensionButton_Click);
            // 
            // testRunnerExtensionsListBox
            // 
            this.testRunnerExtensionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.testRunnerExtensionsListBox.FormattingEnabled = true;
            this.testRunnerExtensionsListBox.Location = new System.Drawing.Point(18, 74);
            this.testRunnerExtensionsListBox.Name = "testRunnerExtensionsListBox";
            this.testRunnerExtensionsListBox.Size = new System.Drawing.Size(585, 95);
            this.testRunnerExtensionsListBox.TabIndex = 9;
            this.testRunnerExtensionsListBox.SelectedIndexChanged += new System.EventHandler(this.testRunnerExtensionsListBox_SelectedIndexChanged);
            // 
            // addExtensionButton
            // 
            this.addExtensionButton.AutoSize = true;
            this.addExtensionButton.Location = new System.Drawing.Point(18, 45);
            this.addExtensionButton.Name = "addExtensionButton";
            this.addExtensionButton.Size = new System.Drawing.Size(85, 23);
            this.addExtensionButton.TabIndex = 10;
            this.addExtensionButton.Text = "Add Extension";
            this.addExtensionButton.UseVisualStyleBackColor = true;
            this.addExtensionButton.Click += new System.EventHandler(this.addExtensionButton_Click);
            // 
            // hintDirectoriesGroupBox
            // 
            this.hintDirectoriesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hintDirectoriesGroupBox.Controls.Add(this.hintDirectoriesListBox);
            this.hintDirectoriesGroupBox.Controls.Add(this.newHintDirectoryTextBox);
            this.hintDirectoriesGroupBox.Controls.Add(this.addFolderButton);
            this.hintDirectoriesGroupBox.Controls.Add(this.removeHintDirectoryButton);
            this.hintDirectoriesGroupBox.Controls.Add(this.findHintDirectoryButton);
            this.hintDirectoriesGroupBox.Location = new System.Drawing.Point(12, 12);
            this.hintDirectoriesGroupBox.Name = "hintDirectoriesGroupBox";
            this.hintDirectoriesGroupBox.Size = new System.Drawing.Size(657, 189);
            this.hintDirectoriesGroupBox.TabIndex = 14;
            this.hintDirectoriesGroupBox.TabStop = false;
            this.hintDirectoriesGroupBox.Text = "Hint Directories";
            // 
            // testPackageConfigGroupBox
            // 
            this.testPackageConfigGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.testPackageConfigGroupBox.Controls.Add(this.shadowCopyCheckBox);
            this.testPackageConfigGroupBox.Controls.Add(this.applicationBaseDirectoryLabel);
            this.testPackageConfigGroupBox.Controls.Add(this.findWorkingDirectoryButton);
            this.testPackageConfigGroupBox.Controls.Add(this.workingDirectoryTextBox);
            this.testPackageConfigGroupBox.Controls.Add(this.applicationBaseDirectoryTextBox);
            this.testPackageConfigGroupBox.Controls.Add(this.workingDirectoryLabel);
            this.testPackageConfigGroupBox.Controls.Add(this.findApplicationBaseDirectoryButton);
            this.testPackageConfigGroupBox.Location = new System.Drawing.Point(12, 212);
            this.testPackageConfigGroupBox.Name = "testPackageConfigGroupBox";
            this.testPackageConfigGroupBox.Size = new System.Drawing.Size(657, 149);
            this.testPackageConfigGroupBox.TabIndex = 15;
            this.testPackageConfigGroupBox.TabStop = false;
            this.testPackageConfigGroupBox.Text = "Test Package Config";
            // 
            // PropertiesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 584);
            this.Controls.Add(this.testPackageConfigGroupBox);
            this.Controls.Add(this.testRunnerExtensionsGroupBox);
            this.Controls.Add(this.hintDirectoriesGroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PropertiesWindow";
            this.TabText = "Properties";
            this.Text = "Properties";
            this.testRunnerExtensionsGroupBox.ResumeLayout(false);
            this.testRunnerExtensionsGroupBox.PerformLayout();
            this.hintDirectoriesGroupBox.ResumeLayout(false);
            this.hintDirectoriesGroupBox.PerformLayout();
            this.testPackageConfigGroupBox.ResumeLayout(false);
            this.testPackageConfigGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox newHintDirectoryTextBox;
        private System.Windows.Forms.Button findHintDirectoryButton;
        private System.Windows.Forms.ListBox hintDirectoriesListBox;
        private System.Windows.Forms.Button addFolderButton;
        private System.Windows.Forms.Button removeHintDirectoryButton;
        private System.Windows.Forms.Label applicationBaseDirectoryLabel;
        private System.Windows.Forms.Button findApplicationBaseDirectoryButton;
        private System.Windows.Forms.TextBox applicationBaseDirectoryTextBox;
        private System.Windows.Forms.Button findWorkingDirectoryButton;
        private System.Windows.Forms.TextBox workingDirectoryTextBox;
        private System.Windows.Forms.Label workingDirectoryLabel;
        private System.Windows.Forms.CheckBox shadowCopyCheckBox;
        private System.Windows.Forms.GroupBox testRunnerExtensionsGroupBox;
        private System.Windows.Forms.TextBox newExtensionTextBox;
        private System.Windows.Forms.Button removeExtensionButton;
        private System.Windows.Forms.ListBox testRunnerExtensionsListBox;
        private System.Windows.Forms.Button addExtensionButton;
        private System.Windows.Forms.GroupBox hintDirectoriesGroupBox;
        private System.Windows.Forms.GroupBox testPackageConfigGroupBox;
    }
}