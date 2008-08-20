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
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.inconclusiveColor);
            this.panel.Controls.Add(this.inconclusiveColorLabel);
            this.panel.Controls.Add(this.skippedColor);
            this.panel.Controls.Add(this.skippedColorLabel);
            this.panel.Controls.Add(this.failedColor);
            this.panel.Controls.Add(this.failedColorLabel);
            this.panel.Controls.Add(this.passedColor);
            this.panel.Controls.Add(this.passedColorLabel);
            // 
            // passedColorLabel
            // 
            this.passedColorLabel.AutoSize = true;
            this.passedColorLabel.Location = new System.Drawing.Point(15, 15);
            this.passedColorLabel.Name = "passedColorLabel";
            this.passedColorLabel.Size = new System.Drawing.Size(71, 13);
            this.passedColorLabel.TabIndex = 1;
            this.passedColorLabel.Text = "Passed color:";
            // 
            // passedColor
            // 
            this.passedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passedColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.passedColor.Location = new System.Drawing.Point(118, 14);
            this.passedColor.Name = "passedColor";
            this.passedColor.Size = new System.Drawing.Size(100, 23);
            this.passedColor.TabIndex = 2;
            this.passedColor.Click += new System.EventHandler(this.color_Click);
            // 
            // failedColor
            // 
            this.failedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.failedColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.failedColor.Location = new System.Drawing.Point(118, 50);
            this.failedColor.Name = "failedColor";
            this.failedColor.Size = new System.Drawing.Size(100, 23);
            this.failedColor.TabIndex = 4;
            this.failedColor.Click += new System.EventHandler(this.color_Click);
            // 
            // failedColorLabel
            // 
            this.failedColorLabel.AutoSize = true;
            this.failedColorLabel.Location = new System.Drawing.Point(15, 51);
            this.failedColorLabel.Name = "failedColorLabel";
            this.failedColorLabel.Size = new System.Drawing.Size(64, 13);
            this.failedColorLabel.TabIndex = 3;
            this.failedColorLabel.Text = "Failed color:";
            // 
            // skippedColor
            // 
            this.skippedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.skippedColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.skippedColor.Location = new System.Drawing.Point(118, 122);
            this.skippedColor.Name = "skippedColor";
            this.skippedColor.Size = new System.Drawing.Size(100, 23);
            this.skippedColor.TabIndex = 6;
            this.skippedColor.Click += new System.EventHandler(this.color_Click);
            // 
            // skippedColorLabel
            // 
            this.skippedColorLabel.AutoSize = true;
            this.skippedColorLabel.Location = new System.Drawing.Point(15, 123);
            this.skippedColorLabel.Name = "skippedColorLabel";
            this.skippedColorLabel.Size = new System.Drawing.Size(75, 13);
            this.skippedColorLabel.TabIndex = 5;
            this.skippedColorLabel.Text = "Skipped color:";
            // 
            // inconclusiveColor
            // 
            this.inconclusiveColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inconclusiveColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.inconclusiveColor.Location = new System.Drawing.Point(118, 86);
            this.inconclusiveColor.Name = "inconclusiveColor";
            this.inconclusiveColor.Size = new System.Drawing.Size(100, 23);
            this.inconclusiveColor.TabIndex = 8;
            this.inconclusiveColor.Click += new System.EventHandler(this.color_Click);
            // 
            // inconclusiveColorLabel
            // 
            this.inconclusiveColorLabel.AutoSize = true;
            this.inconclusiveColorLabel.Location = new System.Drawing.Point(15, 87);
            this.inconclusiveColorLabel.Name = "inconclusiveColorLabel";
            this.inconclusiveColorLabel.Size = new System.Drawing.Size(96, 13);
            this.inconclusiveColorLabel.TabIndex = 7;
            this.inconclusiveColorLabel.Text = "Inconclusive color:";
            // 
            // ColorsOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ColorsOptions";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
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

    }
}
