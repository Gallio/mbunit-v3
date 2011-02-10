namespace Gallio.UI.Reports.Preferences
{
    partial class ReportPreferencePane
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxHtmlReportSplitting = new System.Windows.Forms.GroupBox();
            this.numericUpDownPageSize = new System.Windows.Forms.NumericUpDown();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelPageSize = new System.Windows.Forms.Label();
            this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
            this.groupBoxHtmlReportSplitting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPageSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxHtmlReportSplitting
            // 
            this.groupBoxHtmlReportSplitting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxHtmlReportSplitting.Controls.Add(this.numericUpDownPageSize);
            this.groupBoxHtmlReportSplitting.Controls.Add(this.labelInfo);
            this.groupBoxHtmlReportSplitting.Controls.Add(this.labelPageSize);
            this.groupBoxHtmlReportSplitting.Controls.Add(this.checkBoxEnabled);
            this.groupBoxHtmlReportSplitting.Location = new System.Drawing.Point(3, 3);
            this.groupBoxHtmlReportSplitting.Name = "groupBoxHtmlReportSplitting";
            this.groupBoxHtmlReportSplitting.Size = new System.Drawing.Size(294, 118);
            this.groupBoxHtmlReportSplitting.TabIndex = 7;
            this.groupBoxHtmlReportSplitting.TabStop = false;
            this.groupBoxHtmlReportSplitting.Text = "HTML Report Splitting";
            // 
            // numericUpDownPageSize
            // 
            this.numericUpDownPageSize.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownPageSize.Location = new System.Drawing.Point(71, 42);
            this.numericUpDownPageSize.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownPageSize.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownPageSize.Name = "numericUpDownPageSize";
            this.numericUpDownPageSize.Size = new System.Drawing.Size(70, 20);
            this.numericUpDownPageSize.TabIndex = 4;
            this.numericUpDownPageSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownPageSize.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownPageSize.ValueChanged += new System.EventHandler(this.numericUpDownPageSize_ValueChanged);
            // 
            // labelInfo
            // 
            this.labelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelInfo.Location = new System.Drawing.Point(6, 74);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(282, 37);
            this.labelInfo.TabIndex = 1;
            this.labelInfo.Text = "When enabled, report generation engine will split large HTML reports into several" +
                " files of a smaller size.\r\n";
            // 
            // labelPageSize
            // 
            this.labelPageSize.AutoSize = true;
            this.labelPageSize.Location = new System.Drawing.Point(7, 46);
            this.labelPageSize.Name = "labelPageSize";
            this.labelPageSize.Size = new System.Drawing.Size(58, 13);
            this.labelPageSize.TabIndex = 2;
            this.labelPageSize.Text = "Page &Size:";
            // 
            // checkBoxEnabled
            // 
            this.checkBoxEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxEnabled.Location = new System.Drawing.Point(6, 19);
            this.checkBoxEnabled.Name = "checkBoxEnabled";
            this.checkBoxEnabled.Size = new System.Drawing.Size(80, 24);
            this.checkBoxEnabled.TabIndex = 0;
            this.checkBoxEnabled.Text = "&Enabled:";
            this.checkBoxEnabled.UseVisualStyleBackColor = true;
            this.checkBoxEnabled.CheckedChanged += new System.EventHandler(this.checkBoxEnabled_CheckedChanged);
            // 
            // ReportPreferencePane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxHtmlReportSplitting);
            this.Name = "ReportPreferencePane";
            this.Load += new System.EventHandler(this.ReportsPreferencePane_Load);
            this.groupBoxHtmlReportSplitting.ResumeLayout(false);
            this.groupBoxHtmlReportSplitting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPageSize)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.GroupBox groupBoxHtmlReportSplitting;
        
        #endregion
        private System.Windows.Forms.NumericUpDown numericUpDownPageSize;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelPageSize;
        private System.Windows.Forms.CheckBox checkBoxEnabled;
    }
}
