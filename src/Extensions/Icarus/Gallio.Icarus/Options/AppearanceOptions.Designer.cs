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
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.testProgressBarStyle);
            this.panel.Controls.Add(this.testProgressBarStyleLabel);
            this.panel.Controls.Add(this.showProgressDialogs);
            // 
            // showProgressDialogs
            // 
            this.showProgressDialogs.AutoSize = true;
            this.showProgressDialogs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.showProgressDialogs.Location = new System.Drawing.Point(15, 13);
            this.showProgressDialogs.Name = "showProgressDialogs";
            this.showProgressDialogs.Size = new System.Drawing.Size(135, 17);
            this.showProgressDialogs.TabIndex = 0;
            this.showProgressDialogs.Text = "Show progress dialogs:";
            this.showProgressDialogs.UseVisualStyleBackColor = true;
            // 
            // testProgressBarStyleLabel
            // 
            this.testProgressBarStyleLabel.AutoSize = true;
            this.testProgressBarStyleLabel.Location = new System.Drawing.Point(15, 37);
            this.testProgressBarStyleLabel.Name = "testProgressBarStyleLabel";
            this.testProgressBarStyleLabel.Size = new System.Drawing.Size(116, 13);
            this.testProgressBarStyleLabel.TabIndex = 1;
            this.testProgressBarStyleLabel.Text = "Test progress bar style:";
            // 
            // testProgressBarStyle
            // 
            this.testProgressBarStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testProgressBarStyle.DropDownWidth = 200;
            this.testProgressBarStyle.FormattingEnabled = true;
            this.testProgressBarStyle.Items.AddRange(new object[] {
            "Integration",
            "Unit"});
            this.testProgressBarStyle.Location = new System.Drawing.Point(137, 37);
            this.testProgressBarStyle.Name = "testProgressBarStyle";
            this.testProgressBarStyle.Size = new System.Drawing.Size(178, 21);
            this.testProgressBarStyle.TabIndex = 2;
            // 
            // AppearanceOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "AppearanceOptions";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckBox showProgressDialogs;

        #endregion
        private System.Windows.Forms.ComboBox testProgressBarStyle;
        private System.Windows.Forms.Label testProgressBarStyleLabel;

    }
}
