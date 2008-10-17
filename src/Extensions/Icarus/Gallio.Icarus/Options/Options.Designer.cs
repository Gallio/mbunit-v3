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

namespace Gallio.Icarus.Options
{
    partial class Options
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Colors");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Appearance", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Plugin directories");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Runtime", new System.Windows.Forms.TreeNode[] {
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Startup");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Test Explorer");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.optionCategoryTree = new System.Windows.Forms.TreeView();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.seperatorBar = new System.Windows.Forms.Label();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // optionCategoryTree
            // 
            this.optionCategoryTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.optionCategoryTree.HideSelection = false;
            this.optionCategoryTree.Location = new System.Drawing.Point(13, 12);
            this.optionCategoryTree.Name = "optionCategoryTree";
            treeNode1.Name = "colors";
            treeNode1.Text = "Colors";
            treeNode2.Name = "appearance";
            treeNode2.Text = "Appearance";
            treeNode3.Name = "pluginDirectories";
            treeNode3.Text = "Plugin directories";
            treeNode4.Name = "runtime";
            treeNode4.Text = "Runtime";
            treeNode5.Name = "startup";
            treeNode5.Text = "Startup";
            treeNode6.Name = "testExplorer";
            treeNode6.Text = "Test Explorer";
            this.optionCategoryTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode4,
            treeNode5,
            treeNode6});
            this.optionCategoryTree.Size = new System.Drawing.Size(175, 257);
            this.optionCategoryTree.TabIndex = 0;
            this.optionCategoryTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.optionCategoryTree_AfterSelect);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(416, 283);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(335, 283);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // seperatorBar
            // 
            this.seperatorBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.seperatorBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.seperatorBar.Location = new System.Drawing.Point(195, 275);
            this.seperatorBar.Name = "seperatorBar";
            this.seperatorBar.Size = new System.Drawing.Size(296, 2);
            this.seperatorBar.TabIndex = 5;
            // 
            // panelContainer
            // 
            this.panelContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContainer.Location = new System.Drawing.Point(195, 12);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(296, 257);
            this.panelContainer.TabIndex = 6;
            // 
            // Options
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(503, 318);
            this.Controls.Add(this.panelContainer);
            this.Controls.Add(this.seperatorBar);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.optionCategoryTree);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(511, 352);
            this.Name = "Options";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Options_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView optionCategoryTree;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label seperatorBar;
        private System.Windows.Forms.Panel panelContainer;
    }
}