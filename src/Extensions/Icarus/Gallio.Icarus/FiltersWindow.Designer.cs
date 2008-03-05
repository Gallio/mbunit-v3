// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
    partial class FiltersWindow
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
            this.removeFilterButton = new System.Windows.Forms.Button();
            this.applyFilterButton = new System.Windows.Forms.Button();
            this.filterNameTextBox = new System.Windows.Forms.TextBox();
            this.saveFilterButton = new System.Windows.Forms.Button();
            this.filtersListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // removeFilterButton
            // 
            this.removeFilterButton.Enabled = false;
            this.removeFilterButton.Location = new System.Drawing.Point(84, 222);
            this.removeFilterButton.Name = "removeFilterButton";
            this.removeFilterButton.Size = new System.Drawing.Size(59, 23);
            this.removeFilterButton.TabIndex = 5;
            this.removeFilterButton.Text = "Delete";
            this.removeFilterButton.UseVisualStyleBackColor = true;
            this.removeFilterButton.Click += new System.EventHandler(this.removeFilterButton_Click);
            // 
            // applyFilterButton
            // 
            this.applyFilterButton.Enabled = false;
            this.applyFilterButton.Location = new System.Drawing.Point(12, 222);
            this.applyFilterButton.Name = "applyFilterButton";
            this.applyFilterButton.Size = new System.Drawing.Size(57, 23);
            this.applyFilterButton.TabIndex = 6;
            this.applyFilterButton.Text = "Apply";
            this.applyFilterButton.UseVisualStyleBackColor = true;
            this.applyFilterButton.Click += new System.EventHandler(this.applyFilterButton_Click);
            // 
            // filterNameTextBox
            // 
            this.filterNameTextBox.Location = new System.Drawing.Point(12, 12);
            this.filterNameTextBox.Name = "filterNameTextBox";
            this.filterNameTextBox.Size = new System.Drawing.Size(217, 20);
            this.filterNameTextBox.TabIndex = 7;
            this.filterNameTextBox.TextChanged += new System.EventHandler(this.filterNameTextBox_TextChanged);
            // 
            // saveFilterButton
            // 
            this.saveFilterButton.Enabled = false;
            this.saveFilterButton.Location = new System.Drawing.Point(244, 12);
            this.saveFilterButton.Name = "saveFilterButton";
            this.saveFilterButton.Size = new System.Drawing.Size(57, 23);
            this.saveFilterButton.TabIndex = 8;
            this.saveFilterButton.Text = "Save";
            this.saveFilterButton.UseVisualStyleBackColor = true;
            this.saveFilterButton.Click += new System.EventHandler(this.saveFilterButton_Click);
            // 
            // filtersListBox
            // 
            this.filtersListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filtersListBox.FormattingEnabled = true;
            this.filtersListBox.Location = new System.Drawing.Point(12, 47);
            this.filtersListBox.Name = "filtersListBox";
            this.filtersListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.filtersListBox.Size = new System.Drawing.Size(649, 160);
            this.filtersListBox.TabIndex = 3;
            this.filtersListBox.SelectedIndexChanged += new System.EventHandler(this.filtersListBox_SelectedIndexChanged);
            // 
            // FiltersWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 500);
            this.Controls.Add(this.saveFilterButton);
            this.Controls.Add(this.filterNameTextBox);
            this.Controls.Add(this.applyFilterButton);
            this.Controls.Add(this.removeFilterButton);
            this.Controls.Add(this.filtersListBox);
            this.Name = "FiltersWindow";
            this.TabText = "Test Filters";
            this.Text = "Test Filters";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button removeFilterButton;
        private System.Windows.Forms.Button applyFilterButton;
        private System.Windows.Forms.TextBox filterNameTextBox;
        private System.Windows.Forms.Button saveFilterButton;
        private System.Windows.Forms.ListBox filtersListBox;
    }
}