using System.ComponentModel;

namespace Gallio.Icarus.ControlPanel
{
    internal partial class ProgressMonitoringPane
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
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.groupBoxGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // showProgressDialogs
            // 
            this.showProgressDialogs.AutoSize = true;
            this.showProgressDialogs.Location = new System.Drawing.Point(17, 29);
            this.showProgressDialogs.Name = "showProgressDialogs";
            this.showProgressDialogs.Size = new System.Drawing.Size(132, 17);
            this.showProgressDialogs.TabIndex = 0;
            this.showProgressDialogs.Text = "Show progress dialogs";
            this.showProgressDialogs.UseVisualStyleBackColor = true;
            this.showProgressDialogs.CheckedChanged += new System.EventHandler(this.showProgressDialogs_CheckedChanged);
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Controls.Add(this.showProgressDialogs);
            this.groupBoxGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxGeneral.Location = new System.Drawing.Point(0, 0);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(450, 320);
            this.groupBoxGeneral.TabIndex = 3;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "General";
            // 
            // ProgressMonitoringPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.groupBoxGeneral);
            this.Name = "ProgressMonitoringPane";
            this.Size = new System.Drawing.Size(450, 320);
            this.groupBoxGeneral.ResumeLayout(false);
            this.groupBoxGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckBox showProgressDialogs;

        #endregion

        private System.Windows.Forms.GroupBox groupBoxGeneral;
    }
}
