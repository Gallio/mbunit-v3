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

namespace Gallio.Icarus
{
    partial class AboutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.buttonClose = new System.Windows.Forms.Button();
            this.versionLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.testFrameworksList = new System.Windows.Forms.ListView();
            this.frameworkName = new System.Windows.Forms.ColumnHeader();
            this.testFrameworkIcons = new System.Windows.Forms.ImageList(this.components);
            this.websiteLink = new System.Windows.Forms.LinkLabel();
            this.frameworkVersion = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(333, 308);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(88, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "&OK";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.BackColor = System.Drawing.Color.Transparent;
            this.versionLabel.Location = new System.Drawing.Point(12, 130);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(202, 13);
            this.versionLabel.TabIndex = 3;
            this.versionLabel.Text = "Gallio Icarus - Version {0}.{1}.{2} build {3}";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Available test frameworks:";
            // 
            // testFrameworksList
            // 
            this.testFrameworksList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.frameworkName,
            this.frameworkVersion});
            this.testFrameworksList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.testFrameworksList.Location = new System.Drawing.Point(12, 168);
            this.testFrameworksList.Name = "testFrameworksList";
            this.testFrameworksList.Size = new System.Drawing.Size(409, 134);
            this.testFrameworksList.SmallImageList = this.testFrameworkIcons;
            this.testFrameworksList.TabIndex = 5;
            this.testFrameworksList.UseCompatibleStateImageBehavior = false;
            this.testFrameworksList.View = System.Windows.Forms.View.Details;
            // 
            // frameworkName
            // 
            this.frameworkName.Text = "Name";
            this.frameworkName.Width = 200;
            // 
            // testFrameworkIcons
            // 
            this.testFrameworkIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.testFrameworkIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.testFrameworkIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // websiteLink
            // 
            this.websiteLink.AutoSize = true;
            this.websiteLink.BackColor = System.Drawing.Color.Transparent;
            this.websiteLink.LinkArea = new System.Windows.Forms.LinkArea(9, 38);
            this.websiteLink.Location = new System.Drawing.Point(15, 308);
            this.websiteLink.Name = "websiteLink";
            this.websiteLink.Size = new System.Drawing.Size(157, 17);
            this.websiteLink.TabIndex = 6;
            this.websiteLink.TabStop = true;
            this.websiteLink.Text = "Website: http://www.gallio.org/";
            this.websiteLink.UseCompatibleTextRendering = true;
            this.websiteLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.websiteLink_LinkClicked);
            // 
            // frameworkVersion
            // 
            this.frameworkVersion.Text = "Version";
            this.frameworkVersion.Width = 150;
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(433, 343);
            this.Controls.Add(this.websiteLink);
            this.Controls.Add(this.testFrameworksList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Gallio Icarus";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView testFrameworksList;
        private System.Windows.Forms.LinkLabel websiteLink;
        private System.Windows.Forms.ImageList testFrameworkIcons;
        private System.Windows.Forms.ColumnHeader frameworkName;
        private System.Windows.Forms.ColumnHeader frameworkVersion;
    }
}