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
            this.hintDirectoriesLabel = new System.Windows.Forms.Label();
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
            this.SuspendLayout();
            // 
            // hintDirectoriesLabel
            // 
            this.hintDirectoriesLabel.AutoSize = true;
            this.hintDirectoriesLabel.Location = new System.Drawing.Point(12, 9);
            this.hintDirectoriesLabel.Name = "hintDirectoriesLabel";
            this.hintDirectoriesLabel.Size = new System.Drawing.Size(80, 13);
            this.hintDirectoriesLabel.TabIndex = 0;
            this.hintDirectoriesLabel.Text = "Hint directories:";
            // 
            // newHintDirectoryTextBox
            // 
            this.newHintDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newHintDirectoryTextBox.Location = new System.Drawing.Point(15, 26);
            this.newHintDirectoryTextBox.Name = "newHintDirectoryTextBox";
            this.newHintDirectoryTextBox.Size = new System.Drawing.Size(614, 20);
            this.newHintDirectoryTextBox.TabIndex = 1;
            // 
            // findHintDirectoryButton
            // 
            this.findHintDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findHintDirectoryButton.Location = new System.Drawing.Point(636, 23);
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
            this.hintDirectoriesListBox.Location = new System.Drawing.Point(15, 81);
            this.hintDirectoriesListBox.Name = "hintDirectoriesListBox";
            this.hintDirectoriesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.hintDirectoriesListBox.Size = new System.Drawing.Size(614, 95);
            this.hintDirectoriesListBox.TabIndex = 3;
            this.hintDirectoriesListBox.SelectedIndexChanged += new System.EventHandler(this.hintDirectoriesListBox_SelectedIndexChanged);
            // 
            // addFolderButton
            // 
            this.addFolderButton.Location = new System.Drawing.Point(15, 52);
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
            this.removeHintDirectoryButton.Image = global::Gallio.Icarus.Properties.Resources.ClearAll;
            this.removeHintDirectoryButton.Location = new System.Drawing.Point(636, 81);
            this.removeHintDirectoryButton.Name = "removeHintDirectoryButton";
            this.removeHintDirectoryButton.Size = new System.Drawing.Size(33, 28);
            this.removeHintDirectoryButton.TabIndex = 5;
            this.removeHintDirectoryButton.UseVisualStyleBackColor = true;
            this.removeHintDirectoryButton.Click += new System.EventHandler(this.removeHintDirectoryButton_Click);
            // 
            // applicationBaseDirectoryLabel
            // 
            this.applicationBaseDirectoryLabel.AutoSize = true;
            this.applicationBaseDirectoryLabel.Location = new System.Drawing.Point(15, 189);
            this.applicationBaseDirectoryLabel.Name = "applicationBaseDirectoryLabel";
            this.applicationBaseDirectoryLabel.Size = new System.Drawing.Size(131, 13);
            this.applicationBaseDirectoryLabel.TabIndex = 6;
            this.applicationBaseDirectoryLabel.Text = "Application base directory:";
            // 
            // findApplicationBaseDirectoryButton
            // 
            this.findApplicationBaseDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findApplicationBaseDirectoryButton.Location = new System.Drawing.Point(636, 202);
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
            this.applicationBaseDirectoryTextBox.Location = new System.Drawing.Point(15, 205);
            this.applicationBaseDirectoryTextBox.Name = "applicationBaseDirectoryTextBox";
            this.applicationBaseDirectoryTextBox.Size = new System.Drawing.Size(614, 20);
            this.applicationBaseDirectoryTextBox.TabIndex = 7;
            this.applicationBaseDirectoryTextBox.TextChanged += new System.EventHandler(this.applicationBaseDirectoryTextBox_TextChanged);
            // 
            // findWorkingDirectoryButton
            // 
            this.findWorkingDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findWorkingDirectoryButton.Location = new System.Drawing.Point(636, 253);
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
            this.workingDirectoryTextBox.Location = new System.Drawing.Point(15, 256);
            this.workingDirectoryTextBox.Name = "workingDirectoryTextBox";
            this.workingDirectoryTextBox.Size = new System.Drawing.Size(614, 20);
            this.workingDirectoryTextBox.TabIndex = 10;
            this.workingDirectoryTextBox.TextChanged += new System.EventHandler(this.workingDirectoryTextBox_TextChanged);
            // 
            // workingDirectoryLabel
            // 
            this.workingDirectoryLabel.AutoSize = true;
            this.workingDirectoryLabel.Location = new System.Drawing.Point(15, 240);
            this.workingDirectoryLabel.Name = "workingDirectoryLabel";
            this.workingDirectoryLabel.Size = new System.Drawing.Size(93, 13);
            this.workingDirectoryLabel.TabIndex = 9;
            this.workingDirectoryLabel.Text = "Working directory:";
            // 
            // shadowCopyCheckBox
            // 
            this.shadowCopyCheckBox.AutoSize = true;
            this.shadowCopyCheckBox.Location = new System.Drawing.Point(15, 296);
            this.shadowCopyCheckBox.Name = "shadowCopyCheckBox";
            this.shadowCopyCheckBox.Size = new System.Drawing.Size(91, 17);
            this.shadowCopyCheckBox.TabIndex = 12;
            this.shadowCopyCheckBox.Text = "Shadow copy";
            this.shadowCopyCheckBox.UseVisualStyleBackColor = true;
            this.shadowCopyCheckBox.CheckedChanged += new System.EventHandler(this.shadowCopyCheckBox_CheckedChanged);
            // 
            // PropertiesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 500);
            this.Controls.Add(this.shadowCopyCheckBox);
            this.Controls.Add(this.findWorkingDirectoryButton);
            this.Controls.Add(this.workingDirectoryTextBox);
            this.Controls.Add(this.workingDirectoryLabel);
            this.Controls.Add(this.findApplicationBaseDirectoryButton);
            this.Controls.Add(this.applicationBaseDirectoryTextBox);
            this.Controls.Add(this.applicationBaseDirectoryLabel);
            this.Controls.Add(this.removeHintDirectoryButton);
            this.Controls.Add(this.addFolderButton);
            this.Controls.Add(this.hintDirectoriesListBox);
            this.Controls.Add(this.findHintDirectoryButton);
            this.Controls.Add(this.newHintDirectoryTextBox);
            this.Controls.Add(this.hintDirectoriesLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PropertiesWindow";
            this.TabText = "Properties";
            this.Text = "Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label hintDirectoriesLabel;
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
    }
}