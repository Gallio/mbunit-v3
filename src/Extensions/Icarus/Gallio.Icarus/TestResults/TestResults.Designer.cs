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

namespace Gallio.Icarus.TestResults
{
    partial class TestResults
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestResults));
            this.resultsFilterPanel = new System.Windows.Forms.Panel();
            this.testProgressStatusBar = new Gallio.Icarus.Controls.TestStatusBar();
            this.testResultsList = new Gallio.Icarus.Controls.TestResultsList();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.resultsFilterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // resultsFilterPanel
            // 
            this.resultsFilterPanel.Controls.Add(this.testProgressStatusBar);
            this.resultsFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.resultsFilterPanel.Location = new System.Drawing.Point(0, 0);
            this.resultsFilterPanel.Name = "resultsFilterPanel";
            this.resultsFilterPanel.Size = new System.Drawing.Size(555, 58);
            this.resultsFilterPanel.TabIndex = 2;
            // 
            // testProgressStatusBar
            // 
            this.testProgressStatusBar.BackColor = System.Drawing.Color.White;
            this.testProgressStatusBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.testProgressStatusBar.ElapsedTime = System.TimeSpan.Parse("00:00:00");
            this.testProgressStatusBar.Failed = 0;
            this.testProgressStatusBar.FailedColor = System.Drawing.Color.Red;
            this.testProgressStatusBar.Font = new System.Drawing.Font("Verdana", 8F);
            this.testProgressStatusBar.Inconclusive = 0;
            this.testProgressStatusBar.InconclusiveColor = System.Drawing.Color.Gold;
            this.testProgressStatusBar.Location = new System.Drawing.Point(0, 0);
            this.testProgressStatusBar.Name = "testProgressStatusBar";
            this.testProgressStatusBar.Passed = 0;
            this.testProgressStatusBar.PassedColor = System.Drawing.Color.Green;
            this.testProgressStatusBar.Size = new System.Drawing.Size(555, 56);
            this.testProgressStatusBar.Skipped = 0;
            this.testProgressStatusBar.SkippedColor = System.Drawing.Color.SlateGray;
            this.testProgressStatusBar.TabIndex = 5;
            this.testProgressStatusBar.Text = "{0} tests - {1} passed - {2} failed - {3} inconclusive - {4} skipped - {5}";
            this.testProgressStatusBar.Total = 0;
            // 
            // testResultsList
            // 
            this.testResultsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testResultsList.FullRowSelect = true;
            this.testResultsList.Location = new System.Drawing.Point(0, 58);
            this.testResultsList.Name = "testResultsList";
            this.testResultsList.Size = new System.Drawing.Size(555, 215);
            this.testResultsList.SmallImageList = this.imageList1;
            this.testResultsList.TabIndex = 3;
            this.testResultsList.UseCompatibleStateImageBehavior = false;
            this.testResultsList.View = System.Windows.Forms.View.Details;
            this.testResultsList.VirtualMode = true;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "tick.png");
            this.imageList1.Images.SetKeyName(1, "cross.png");
            this.imageList1.Images.SetKeyName(2, "error.png");
            // 
            // TestResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 273);
            this.Controls.Add(this.testResultsList);
            this.Controls.Add(this.resultsFilterPanel);
            this.Name = "TestResults";
            this.Text = "Test Results";
            this.resultsFilterPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel resultsFilterPanel;
        private Gallio.Icarus.Controls.TestResultsList testResultsList;
        private Gallio.Icarus.Controls.TestStatusBar testProgressStatusBar;
        private System.Windows.Forms.ImageList imageList1;
    }
}