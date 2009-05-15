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
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.panel.SuspendLayout();
            this.groupBoxGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.groupBoxGeneral);
            // 
            // restorePreviousSession
            // 
            this.restorePreviousSession.AutoSize = true;
            this.restorePreviousSession.Location = new System.Drawing.Point(12, 21);
            this.restorePreviousSession.Name = "restorePreviousSession";
            this.restorePreviousSession.Size = new System.Drawing.Size(147, 17);
            this.restorePreviousSession.TabIndex = 0;
            this.restorePreviousSession.Text = "Restore Previous Session";
            this.restorePreviousSession.UseVisualStyleBackColor = true;
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGeneral.Controls.Add(this.restorePreviousSession);
            this.groupBoxGeneral.Location = new System.Drawing.Point(3, 3);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(444, 52);
            this.groupBoxGeneral.TabIndex = 3;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "General";
            // 
            // StartupOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "StartupOptions";
            this.panel.ResumeLayout(false);
            this.groupBoxGeneral.ResumeLayout(false);
            this.groupBoxGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckBox restorePreviousSession;

        #endregion

        private System.Windows.Forms.GroupBox groupBoxGeneral;
    }
}
