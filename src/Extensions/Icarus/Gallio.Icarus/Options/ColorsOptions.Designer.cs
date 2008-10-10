using System.ComponentModel;

namespace Gallio.Icarus.Options
{
    internal partial class ColorsOptions
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
            this.passedColorLabel = new System.Windows.Forms.Label();
            this.passedColor = new System.Windows.Forms.Label();
            this.failedColor = new System.Windows.Forms.Label();
            this.failedColorLabel = new System.Windows.Forms.Label();
            this.skippedColor = new System.Windows.Forms.Label();
            this.skippedColorLabel = new System.Windows.Forms.Label();
            this.inconclusiveColor = new System.Windows.Forms.Label();
            this.inconclusiveColorLabel = new System.Windows.Forms.Label();
            this.groupBoxStatusColors = new System.Windows.Forms.GroupBox();
            this.panel.SuspendLayout();
            this.groupBoxStatusColors.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.groupBoxStatusColors);
            // 
            // passedColorLabel
            // 
            this.passedColorLabel.AutoSize = true;
            this.passedColorLabel.Location = new System.Drawing.Point(6, 23);
            this.passedColorLabel.Name = "passedColorLabel";
            this.passedColorLabel.Size = new System.Drawing.Size(42, 13);
            this.passedColorLabel.TabIndex = 1;
            this.passedColorLabel.Text = "Passed";
            // 
            // passedColor
            // 
            this.passedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passedColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.passedColor.Location = new System.Drawing.Point(90, 18);
            this.passedColor.Name = "passedColor";
            this.passedColor.Size = new System.Drawing.Size(100, 23);
            this.passedColor.TabIndex = 2;
            this.passedColor.Click += new System.EventHandler(this.color_Click);
            // 
            // failedColor
            // 
            this.failedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.failedColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.failedColor.Location = new System.Drawing.Point(90, 46);
            this.failedColor.Name = "failedColor";
            this.failedColor.Size = new System.Drawing.Size(100, 23);
            this.failedColor.TabIndex = 4;
            this.failedColor.Click += new System.EventHandler(this.color_Click);
            // 
            // failedColorLabel
            // 
            this.failedColorLabel.AutoSize = true;
            this.failedColorLabel.Location = new System.Drawing.Point(6, 51);
            this.failedColorLabel.Name = "failedColorLabel";
            this.failedColorLabel.Size = new System.Drawing.Size(35, 13);
            this.failedColorLabel.TabIndex = 3;
            this.failedColorLabel.Text = "Failed";
            // 
            // skippedColor
            // 
            this.skippedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.skippedColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.skippedColor.Location = new System.Drawing.Point(90, 102);
            this.skippedColor.Name = "skippedColor";
            this.skippedColor.Size = new System.Drawing.Size(100, 23);
            this.skippedColor.TabIndex = 6;
            this.skippedColor.Click += new System.EventHandler(this.color_Click);
            // 
            // skippedColorLabel
            // 
            this.skippedColorLabel.AutoSize = true;
            this.skippedColorLabel.Location = new System.Drawing.Point(6, 107);
            this.skippedColorLabel.Name = "skippedColorLabel";
            this.skippedColorLabel.Size = new System.Drawing.Size(46, 13);
            this.skippedColorLabel.TabIndex = 5;
            this.skippedColorLabel.Text = "Skipped";
            // 
            // inconclusiveColor
            // 
            this.inconclusiveColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inconclusiveColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.inconclusiveColor.Location = new System.Drawing.Point(90, 74);
            this.inconclusiveColor.Name = "inconclusiveColor";
            this.inconclusiveColor.Size = new System.Drawing.Size(100, 23);
            this.inconclusiveColor.TabIndex = 8;
            this.inconclusiveColor.Click += new System.EventHandler(this.color_Click);
            // 
            // inconclusiveColorLabel
            // 
            this.inconclusiveColorLabel.AutoSize = true;
            this.inconclusiveColorLabel.Location = new System.Drawing.Point(6, 79);
            this.inconclusiveColorLabel.Name = "inconclusiveColorLabel";
            this.inconclusiveColorLabel.Size = new System.Drawing.Size(67, 13);
            this.inconclusiveColorLabel.TabIndex = 7;
            this.inconclusiveColorLabel.Text = "Inconclusive";
            // 
            // groupBoxStatusColors
            // 
            this.groupBoxStatusColors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxStatusColors.Controls.Add(this.passedColor);
            this.groupBoxStatusColors.Controls.Add(this.skippedColorLabel);
            this.groupBoxStatusColors.Controls.Add(this.inconclusiveColorLabel);
            this.groupBoxStatusColors.Controls.Add(this.inconclusiveColor);
            this.groupBoxStatusColors.Controls.Add(this.passedColorLabel);
            this.groupBoxStatusColors.Controls.Add(this.failedColorLabel);
            this.groupBoxStatusColors.Controls.Add(this.skippedColor);
            this.groupBoxStatusColors.Controls.Add(this.failedColor);
            this.groupBoxStatusColors.Location = new System.Drawing.Point(3, 3);
            this.groupBoxStatusColors.Name = "groupBoxStatusColors";
            this.groupBoxStatusColors.Size = new System.Drawing.Size(444, 138);
            this.groupBoxStatusColors.TabIndex = 9;
            this.groupBoxStatusColors.TabStop = false;
            this.groupBoxStatusColors.Text = "Status Colors";
            // 
            // ColorsOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ColorsOptions";
            this.panel.ResumeLayout(false);
            this.groupBoxStatusColors.ResumeLayout(false);
            this.groupBoxStatusColors.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label passedColorLabel;
        private System.Windows.Forms.Label passedColor;
        private System.Windows.Forms.Label inconclusiveColor;
        private System.Windows.Forms.Label inconclusiveColorLabel;
        private System.Windows.Forms.Label skippedColor;
        private System.Windows.Forms.Label skippedColorLabel;
        private System.Windows.Forms.Label failedColor;
        private System.Windows.Forms.Label failedColorLabel;
        private System.Windows.Forms.GroupBox groupBoxStatusColors;

    }
}
