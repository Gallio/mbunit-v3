using System.ComponentModel;

namespace Gallio.Icarus.Options
{
    internal partial class ReportOptions
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private const IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.generateReportAfterTestRun = new System.Windows.Forms.CheckBox();
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.panel.SuspendLayout();
            this.groupBoxGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.groupBoxGeneral);
            this.panel.Size = new System.Drawing.Size(450, 105);
            // 
            // generateReportAfterTestRun
            // 
            this.generateReportAfterTestRun.AutoSize = true;
            this.generateReportAfterTestRun.Location = new System.Drawing.Point(6, 19);
            this.generateReportAfterTestRun.Name = "generateReportAfterTestRun";
            this.generateReportAfterTestRun.Size = new System.Drawing.Size(162, 17);
            this.generateReportAfterTestRun.TabIndex = 0;
            this.generateReportAfterTestRun.Text = "Generate report after test run";
            this.generateReportAfterTestRun.UseVisualStyleBackColor = true;
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGeneral.Controls.Add(this.generateReportAfterTestRun);
            this.groupBoxGeneral.Location = new System.Drawing.Point(3, 3);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(444, 97);
            this.groupBoxGeneral.TabIndex = 3;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "General";
            // 
            // ReportOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ReportOptions";
            this.Size = new System.Drawing.Size(450, 105);
            this.panel.ResumeLayout(false);
            this.groupBoxGeneral.ResumeLayout(false);
            this.groupBoxGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckBox generateReportAfterTestRun;

        #endregion

        private System.Windows.Forms.GroupBox groupBoxGeneral;
    }
}
