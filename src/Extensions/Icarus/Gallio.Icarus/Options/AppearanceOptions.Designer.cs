using System.ComponentModel;

namespace Gallio.Icarus.Options
{
    internal partial class AppearanceOptions
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
            this.showProgressDialogs = new System.Windows.Forms.CheckBox();
            this.testProgressBarStyleLabel = new System.Windows.Forms.Label();
            this.testProgressBarStyle = new System.Windows.Forms.ComboBox();
            this.groupBoxProgressFeedback = new System.Windows.Forms.GroupBox();
            this.panel.SuspendLayout();
            this.groupBoxProgressFeedback.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.groupBoxProgressFeedback);
            // 
            // showProgressDialogs
            // 
            this.showProgressDialogs.AutoSize = true;
            this.showProgressDialogs.Location = new System.Drawing.Point(19, 29);
            this.showProgressDialogs.Name = "showProgressDialogs";
            this.showProgressDialogs.Size = new System.Drawing.Size(135, 17);
            this.showProgressDialogs.TabIndex = 0;
            this.showProgressDialogs.Text = "Show Progress Dialogs";
            this.showProgressDialogs.UseVisualStyleBackColor = true;
            // 
            // testProgressBarStyleLabel
            // 
            this.testProgressBarStyleLabel.AutoSize = true;
            this.testProgressBarStyleLabel.Location = new System.Drawing.Point(16, 62);
            this.testProgressBarStyleLabel.Name = "testProgressBarStyleLabel";
            this.testProgressBarStyleLabel.Size = new System.Drawing.Size(117, 13);
            this.testProgressBarStyleLabel.TabIndex = 1;
            this.testProgressBarStyleLabel.Text = "Test Progress Bar Style";
            // 
            // testProgressBarStyle
            // 
            this.testProgressBarStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testProgressBarStyle.DropDownWidth = 200;
            this.testProgressBarStyle.FormattingEnabled = true;
            this.testProgressBarStyle.Items.AddRange(new object[] {
            "Integration",
            "Unit"});
            this.testProgressBarStyle.Location = new System.Drawing.Point(139, 62);
            this.testProgressBarStyle.Name = "testProgressBarStyle";
            this.testProgressBarStyle.Size = new System.Drawing.Size(242, 21);
            this.testProgressBarStyle.TabIndex = 2;
            // 
            // groupBoxProgressFeedback
            // 
            this.groupBoxProgressFeedback.Controls.Add(this.showProgressDialogs);
            this.groupBoxProgressFeedback.Controls.Add(this.testProgressBarStyle);
            this.groupBoxProgressFeedback.Controls.Add(this.testProgressBarStyleLabel);
            this.groupBoxProgressFeedback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProgressFeedback.Location = new System.Drawing.Point(0, 0);
            this.groupBoxProgressFeedback.Name = "groupBoxProgressFeedback";
            this.groupBoxProgressFeedback.Size = new System.Drawing.Size(450, 350);
            this.groupBoxProgressFeedback.TabIndex = 3;
            this.groupBoxProgressFeedback.TabStop = false;
            this.groupBoxProgressFeedback.Text = "Progress Feedback";
            // 
            // AppearanceOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "AppearanceOptions";
            this.panel.ResumeLayout(false);
            this.groupBoxProgressFeedback.ResumeLayout(false);
            this.groupBoxProgressFeedback.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckBox showProgressDialogs;

        #endregion
        private System.Windows.Forms.ComboBox testProgressBarStyle;
        private System.Windows.Forms.Label testProgressBarStyleLabel;
        private System.Windows.Forms.GroupBox groupBoxProgressFeedback;

    }
}
