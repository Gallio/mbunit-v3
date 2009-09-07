using System.ComponentModel;

namespace Gallio.Icarus.ControlPanel
{
    internal partial class TestStatusPane
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
            this.testStatusColorsGroupBox = new System.Windows.Forms.GroupBox();
            this.testProgressBarStyle = new System.Windows.Forms.ComboBox();
            this.testProgressBarStyleLabel = new System.Windows.Forms.Label();
            this.testProgressGroupBox = new System.Windows.Forms.GroupBox();
            this.testStatusColorsGroupBox.SuspendLayout();
            this.testProgressGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // passedColorLabel
            // 
            this.passedColorLabel.AutoSize = true;
            this.passedColorLabel.Location = new System.Drawing.Point(16, 28);
            this.passedColorLabel.Name = "passedColorLabel";
            this.passedColorLabel.Size = new System.Drawing.Size(42, 13);
            this.passedColorLabel.TabIndex = 1;
            this.passedColorLabel.Text = "Passed";
            // 
            // passedColor
            // 
            this.passedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passedColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.passedColor.Location = new System.Drawing.Point(100, 23);
            this.passedColor.Name = "passedColor";
            this.passedColor.Size = new System.Drawing.Size(100, 23);
            this.passedColor.TabIndex = 2;
            this.passedColor.Click += new System.EventHandler(this.color_Click);
            // 
            // failedColor
            // 
            this.failedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.failedColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.failedColor.Location = new System.Drawing.Point(100, 51);
            this.failedColor.Name = "failedColor";
            this.failedColor.Size = new System.Drawing.Size(100, 23);
            this.failedColor.TabIndex = 4;
            this.failedColor.Click += new System.EventHandler(this.color_Click);
            // 
            // failedColorLabel
            // 
            this.failedColorLabel.AutoSize = true;
            this.failedColorLabel.Location = new System.Drawing.Point(16, 56);
            this.failedColorLabel.Name = "failedColorLabel";
            this.failedColorLabel.Size = new System.Drawing.Size(35, 13);
            this.failedColorLabel.TabIndex = 3;
            this.failedColorLabel.Text = "Failed";
            // 
            // skippedColor
            // 
            this.skippedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.skippedColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.skippedColor.Location = new System.Drawing.Point(100, 107);
            this.skippedColor.Name = "skippedColor";
            this.skippedColor.Size = new System.Drawing.Size(100, 23);
            this.skippedColor.TabIndex = 6;
            this.skippedColor.Click += new System.EventHandler(this.color_Click);
            // 
            // skippedColorLabel
            // 
            this.skippedColorLabel.AutoSize = true;
            this.skippedColorLabel.Location = new System.Drawing.Point(16, 112);
            this.skippedColorLabel.Name = "skippedColorLabel";
            this.skippedColorLabel.Size = new System.Drawing.Size(46, 13);
            this.skippedColorLabel.TabIndex = 5;
            this.skippedColorLabel.Text = "Skipped";
            // 
            // inconclusiveColor
            // 
            this.inconclusiveColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inconclusiveColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.inconclusiveColor.Location = new System.Drawing.Point(100, 79);
            this.inconclusiveColor.Name = "inconclusiveColor";
            this.inconclusiveColor.Size = new System.Drawing.Size(100, 23);
            this.inconclusiveColor.TabIndex = 8;
            this.inconclusiveColor.Click += new System.EventHandler(this.color_Click);
            // 
            // inconclusiveColorLabel
            // 
            this.inconclusiveColorLabel.AutoSize = true;
            this.inconclusiveColorLabel.Location = new System.Drawing.Point(16, 84);
            this.inconclusiveColorLabel.Name = "inconclusiveColorLabel";
            this.inconclusiveColorLabel.Size = new System.Drawing.Size(67, 13);
            this.inconclusiveColorLabel.TabIndex = 7;
            this.inconclusiveColorLabel.Text = "Inconclusive";
            // 
            // testStatusColorsGroupBox
            // 
            this.testStatusColorsGroupBox.Controls.Add(this.passedColor);
            this.testStatusColorsGroupBox.Controls.Add(this.skippedColorLabel);
            this.testStatusColorsGroupBox.Controls.Add(this.inconclusiveColorLabel);
            this.testStatusColorsGroupBox.Controls.Add(this.inconclusiveColor);
            this.testStatusColorsGroupBox.Controls.Add(this.passedColorLabel);
            this.testStatusColorsGroupBox.Controls.Add(this.failedColorLabel);
            this.testStatusColorsGroupBox.Controls.Add(this.skippedColor);
            this.testStatusColorsGroupBox.Controls.Add(this.failedColor);
            this.testStatusColorsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.testStatusColorsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.testStatusColorsGroupBox.Name = "testStatusColorsGroupBox";
            this.testStatusColorsGroupBox.Size = new System.Drawing.Size(300, 144);
            this.testStatusColorsGroupBox.TabIndex = 9;
            this.testStatusColorsGroupBox.TabStop = false;
            this.testStatusColorsGroupBox.Text = "Test Status Colors";
            // 
            // testProgressBarStyle
            // 
            this.testProgressBarStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.testProgressBarStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testProgressBarStyle.DropDownWidth = 200;
            this.testProgressBarStyle.FormattingEnabled = true;
            this.testProgressBarStyle.Items.AddRange(new object[] {
            "Integration",
            "Unit"});
            this.testProgressBarStyle.Location = new System.Drawing.Point(139, 28);
            this.testProgressBarStyle.Name = "testProgressBarStyle";
            this.testProgressBarStyle.Size = new System.Drawing.Size(140, 21);
            this.testProgressBarStyle.TabIndex = 10;
            this.testProgressBarStyle.SelectedIndexChanged += new System.EventHandler(this.testProgressBarStyle_SelectedIndexChanged);
            // 
            // testProgressBarStyleLabel
            // 
            this.testProgressBarStyleLabel.AutoSize = true;
            this.testProgressBarStyleLabel.Location = new System.Drawing.Point(16, 28);
            this.testProgressBarStyleLabel.Name = "testProgressBarStyleLabel";
            this.testProgressBarStyleLabel.Size = new System.Drawing.Size(117, 13);
            this.testProgressBarStyleLabel.TabIndex = 9;
            this.testProgressBarStyleLabel.Text = "Test Progress Bar Style";
            // 
            // testProgressGroupBox
            // 
            this.testProgressGroupBox.Controls.Add(this.testProgressBarStyleLabel);
            this.testProgressGroupBox.Controls.Add(this.testProgressBarStyle);
            this.testProgressGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testProgressGroupBox.Location = new System.Drawing.Point(0, 144);
            this.testProgressGroupBox.Name = "testProgressGroupBox";
            this.testProgressGroupBox.Size = new System.Drawing.Size(300, 176);
            this.testProgressGroupBox.TabIndex = 11;
            this.testProgressGroupBox.TabStop = false;
            this.testProgressGroupBox.Text = "Test Progress";
            // 
            // TestStatusPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.testProgressGroupBox);
            this.Controls.Add(this.testStatusColorsGroupBox);
            this.Name = "TestStatusPane";
            this.testStatusColorsGroupBox.ResumeLayout(false);
            this.testStatusColorsGroupBox.PerformLayout();
            this.testProgressGroupBox.ResumeLayout(false);
            this.testProgressGroupBox.PerformLayout();
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
        private System.Windows.Forms.GroupBox testStatusColorsGroupBox;
        private System.Windows.Forms.ComboBox testProgressBarStyle;
        private System.Windows.Forms.Label testProgressBarStyleLabel;
        private System.Windows.Forms.GroupBox testProgressGroupBox;

    }
}
