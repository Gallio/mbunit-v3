namespace Gallio.TDNetRunner.UI.Preferences
{
    partial class TDNetRunnerReportPreferencePane
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
            this.SuspendLayout();
            // 
            // TDNetRunnerReportPreferencePane
            // 
            this.comboBoxOutputReportType = new System.Windows.Forms.ComboBox();
            this.checkBoxAutoCondense = new System.Windows.Forms.CheckBox();
            this.labelAutoCondense = new System.Windows.Forms.Label();
            this.groupBoxAutoCondense = new System.Windows.Forms.GroupBox();
            this.numericUpDownAutoCondenseThreshold = new System.Windows.Forms.NumericUpDown();
            this.labelDetails = new System.Windows.Forms.Label();
            this.groupBoxOutputReport = new System.Windows.Forms.GroupBox();
            this.panelAutoCondenseThreshold = new System.Windows.Forms.Panel();
            this.groupBoxAutoCondense.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutoCondenseThreshold)).BeginInit();
            this.groupBoxOutputReport.SuspendLayout();
            this.panelAutoCondenseThreshold.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxOutputReportType
            // 
            this.comboBoxOutputReportType.DisplayMember = "Name";
            this.comboBoxOutputReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOutputReportType.FormattingEnabled = true;
            this.comboBoxOutputReportType.Location = new System.Drawing.Point(6, 19);
            this.comboBoxOutputReportType.Name = "comboBoxOutputReportType";
            this.comboBoxOutputReportType.Size = new System.Drawing.Size(156, 21);
            this.comboBoxOutputReportType.Sorted = true;
            this.comboBoxOutputReportType.TabIndex = 0;
            this.comboBoxOutputReportType.SelectedIndexChanged += new System.EventHandler(this.comboBoxOutputReportType_SelectedIndexChanged);
            // 
            // checkBoxAutoCondense
            // 
            this.checkBoxAutoCondense.Location = new System.Drawing.Point(9, -2);
            this.checkBoxAutoCondense.Name = "checkBoxAutoCondense";
            this.checkBoxAutoCondense.Size = new System.Drawing.Size(13, 18);
            this.checkBoxAutoCondense.TabIndex = 1;
            this.checkBoxAutoCondense.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.checkBoxAutoCondense.UseVisualStyleBackColor = true;
            this.checkBoxAutoCondense.CheckedChanged += new System.EventHandler(this.checkBoxAutoCondense_CheckedChanged);
            // 
            // labelAutoCondense
            // 
            this.labelAutoCondense.AutoSize = true;
            this.labelAutoCondense.Location = new System.Drawing.Point(3, 5);
            this.labelAutoCondense.Name = "labelAutoCondense";
            this.labelAutoCondense.Size = new System.Drawing.Size(156, 13);
            this.labelAutoCondense.TabIndex = 3;
            this.labelAutoCondense.Text = "When test count is greater than";
            // 
            // groupBoxAutoCondense
            // 
            this.groupBoxAutoCondense.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAutoCondense.Controls.Add(this.panelAutoCondenseThreshold);
            this.groupBoxAutoCondense.Controls.Add(this.checkBoxAutoCondense);
            this.groupBoxAutoCondense.Location = new System.Drawing.Point(3, 87);
            this.groupBoxAutoCondense.Name = "groupBoxAutoCondense";
            this.groupBoxAutoCondense.Size = new System.Drawing.Size(294, 45);
            this.groupBoxAutoCondense.TabIndex = 5;
            this.groupBoxAutoCondense.TabStop = false;
            this.groupBoxAutoCondense.Text = "      Auto-Condense Report";
            // 
            // numericUpDownAutoCondenseThreshold
            // 
            this.numericUpDownAutoCondenseThreshold.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownAutoCondenseThreshold.Location = new System.Drawing.Point(162, 3);
            this.numericUpDownAutoCondenseThreshold.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownAutoCondenseThreshold.Name = "numericUpDownAutoCondenseThreshold";
            this.numericUpDownAutoCondenseThreshold.Size = new System.Drawing.Size(63, 20);
            this.numericUpDownAutoCondenseThreshold.TabIndex = 4;
            this.numericUpDownAutoCondenseThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownAutoCondenseThreshold.ValueChanged += new System.EventHandler(this.numericUpDownAutoCondenseThreshold_ValueChanged);
            // 
            // labelDetails
            // 
            this.labelDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDetails.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelDetails.Location = new System.Drawing.Point(6, 43);
            this.labelDetails.Name = "labelDetails";
            this.labelDetails.Size = new System.Drawing.Size(282, 32);
            this.labelDetails.TabIndex = 6;
            // 
            // groupBoxOutputReport
            // 
            this.groupBoxOutputReport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOutputReport.Controls.Add(this.comboBoxOutputReportType);
            this.groupBoxOutputReport.Controls.Add(this.labelDetails);
            this.groupBoxOutputReport.Location = new System.Drawing.Point(3, 3);
            this.groupBoxOutputReport.Name = "groupBoxOutputReport";
            this.groupBoxOutputReport.Size = new System.Drawing.Size(294, 78);
            this.groupBoxOutputReport.TabIndex = 7;
            this.groupBoxOutputReport.TabStop = false;
            this.groupBoxOutputReport.Text = "Output Report";
            // 
            // panelAutoCondenseThreshold
            // 
            this.panelAutoCondenseThreshold.Controls.Add(this.numericUpDownAutoCondenseThreshold);
            this.panelAutoCondenseThreshold.Controls.Add(this.labelAutoCondense);
            this.panelAutoCondenseThreshold.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAutoCondenseThreshold.Location = new System.Drawing.Point(3, 16);
            this.panelAutoCondenseThreshold.Name = "panelAutoCondenseThreshold";
            this.panelAutoCondenseThreshold.Size = new System.Drawing.Size(288, 26);
            this.panelAutoCondenseThreshold.TabIndex = 8;
            // 
            // TDNetRunnerReportsPreferencePane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxOutputReport);
            this.Controls.Add(this.groupBoxAutoCondense);
            this.Name = "TDNetRunnerReportsPreferencePane";
            this.Load += new System.EventHandler(this.TDNetRunnerReportsPreferencePane_Load);
            this.groupBoxAutoCondense.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutoCondenseThreshold)).EndInit();
            this.groupBoxOutputReport.ResumeLayout(false);
            this.panelAutoCondenseThreshold.ResumeLayout(false);
            this.panelAutoCondenseThreshold.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.ComboBox comboBoxOutputReportType;
        private System.Windows.Forms.CheckBox checkBoxAutoCondense;
        private System.Windows.Forms.Label labelAutoCondense;
        private System.Windows.Forms.GroupBox groupBoxAutoCondense;
        private System.Windows.Forms.NumericUpDown numericUpDownAutoCondenseThreshold;
        private System.Windows.Forms.Label labelDetails;
        private System.Windows.Forms.GroupBox groupBoxOutputReport;
        private System.Windows.Forms.Panel panelAutoCondenseThreshold;
        
        #endregion
    }
}
