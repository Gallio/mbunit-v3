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
    partial class ReportWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportWindow));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.reportViewer = new System.Windows.Forms.WebBrowser();
            this.panel4 = new System.Windows.Forms.Panel();
            this.reportTypes = new System.Windows.Forms.ComboBox();
            this.btnSaveReportAs = new System.Windows.Forms.Button();
            this.lblReportType = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.reloadReport = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.reportViewer);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel4);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(292, 248);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(292, 273);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // reportViewer
            // 
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer.Location = new System.Drawing.Point(0, 42);
            this.reportViewer.MinimumSize = new System.Drawing.Size(20, 20);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.Size = new System.Drawing.Size(292, 206);
            this.reportViewer.TabIndex = 6;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.reportTypes);
            this.panel4.Controls.Add(this.btnSaveReportAs);
            this.panel4.Controls.Add(this.lblReportType);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(292, 42);
            this.panel4.TabIndex = 5;
            // 
            // reportTypes
            // 
            this.reportTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.reportTypes.FormattingEnabled = true;
            this.reportTypes.Location = new System.Drawing.Point(66, 11);
            this.reportTypes.Name = "reportTypes";
            this.reportTypes.Size = new System.Drawing.Size(121, 21);
            this.reportTypes.TabIndex = 5;
            // 
            // btnSaveReportAs
            // 
            this.btnSaveReportAs.AutoSize = true;
            this.btnSaveReportAs.Enabled = false;
            this.btnSaveReportAs.Location = new System.Drawing.Point(193, 11);
            this.btnSaveReportAs.Name = "btnSaveReportAs";
            this.btnSaveReportAs.Size = new System.Drawing.Size(51, 23);
            this.btnSaveReportAs.TabIndex = 4;
            this.btnSaveReportAs.Text = "Save...";
            this.btnSaveReportAs.UseVisualStyleBackColor = true;
            // 
            // lblReportType
            // 
            this.lblReportType.AutoSize = true;
            this.lblReportType.Location = new System.Drawing.Point(11, 15);
            this.lblReportType.Name = "lblReportType";
            this.lblReportType.Size = new System.Drawing.Size(49, 13);
            this.lblReportType.TabIndex = 3;
            this.lblReportType.Text = "Save as:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadReport});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(64, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // reloadReport
            // 
            this.reloadReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reloadReport.Image = ((System.Drawing.Image)(resources.GetObject("reloadReport.Image")));
            this.reloadReport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadReport.Name = "reloadReport";
            this.reloadReport.Size = new System.Drawing.Size(23, 22);
            this.reloadReport.Text = "Reload";
            this.reloadReport.Click += new System.EventHandler(this.reloadReport_Click);
            // 
            // ReportWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportWindow";
            this.TabText = "Report";
            this.Text = "Report";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.WebBrowser reportViewer;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ComboBox reportTypes;
        private System.Windows.Forms.Button btnSaveReportAs;
        private System.Windows.Forms.Label lblReportType;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton reloadReport;

    }
}