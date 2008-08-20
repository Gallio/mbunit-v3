using System.ComponentModel;

namespace Gallio.Icarus.Options
{
    internal partial class StartupOptions
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
            this.restorePreviousSession = new System.Windows.Forms.CheckBox();
            this.testRunnerFactoryLabel = new System.Windows.Forms.Label();
            this.testRunnerFactories = new System.Windows.Forms.ComboBox();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.testRunnerFactories);
            this.panel.Controls.Add(this.testRunnerFactoryLabel);
            this.panel.Controls.Add(this.restorePreviousSession);
            // 
            // restorePreviousSession
            // 
            this.restorePreviousSession.AutoSize = true;
            this.restorePreviousSession.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.restorePreviousSession.Location = new System.Drawing.Point(15, 13);
            this.restorePreviousSession.Name = "restorePreviousSession";
            this.restorePreviousSession.Size = new System.Drawing.Size(147, 17);
            this.restorePreviousSession.TabIndex = 0;
            this.restorePreviousSession.Text = "Restore previous session:";
            this.restorePreviousSession.UseVisualStyleBackColor = true;
            // 
            // testRunnerFactoryLabel
            // 
            this.testRunnerFactoryLabel.AutoSize = true;
            this.testRunnerFactoryLabel.Location = new System.Drawing.Point(15, 37);
            this.testRunnerFactoryLabel.Name = "testRunnerFactoryLabel";
            this.testRunnerFactoryLabel.Size = new System.Drawing.Size(99, 13);
            this.testRunnerFactoryLabel.TabIndex = 1;
            this.testRunnerFactoryLabel.Text = "Test runner factory:";
            // 
            // testRunnerFactories
            // 
            this.testRunnerFactories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testRunnerFactories.DropDownWidth = 200;
            this.testRunnerFactories.FormattingEnabled = true;
            this.testRunnerFactories.Location = new System.Drawing.Point(121, 37);
            this.testRunnerFactories.Name = "testRunnerFactories";
            this.testRunnerFactories.Size = new System.Drawing.Size(178, 21);
            this.testRunnerFactories.TabIndex = 2;
            // 
            // StartupOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckBox restorePreviousSession;

        #endregion
        private System.Windows.Forms.Label testRunnerFactoryLabel;
        private System.Windows.Forms.ComboBox testRunnerFactories;
    }
}
