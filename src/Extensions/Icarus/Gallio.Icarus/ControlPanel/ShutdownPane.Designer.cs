using System.ComponentModel;

namespace Gallio.Icarus.ControlPanel
{
    internal partial class ShutdownPane
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
            this.autoSave = new System.Windows.Forms.CheckBox();
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.groupBoxGeneral.SuspendLayout();
            this.SuspendLayout();
            this.Controls.Add(this.groupBoxGeneral);
            // 
            // autoSave
            // 
            this.autoSave.AutoSize = true;
            this.autoSave.Location = new System.Drawing.Point(12, 21);
            this.autoSave.Name = "autoSave";
            this.autoSave.Size = new System.Drawing.Size(147, 17);
            this.autoSave.TabIndex = 0;
            this.autoSave.Text = "Auto-Save Project Settings";
            this.autoSave.UseVisualStyleBackColor = true;
            this.autoSave.CheckedChanged += new System.EventHandler(this.autoSave_CheckedChanged);
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Controls.Add(this.autoSave);
            this.groupBoxGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxGeneral.Location = new System.Drawing.Point(0, 0);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(450, 350);
            this.groupBoxGeneral.TabIndex = 3;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "General";
            // 
            // ShutdownOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ShutdownOptions";
            this.groupBoxGeneral.ResumeLayout(false);
            this.groupBoxGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox autoSave;
        private System.Windows.Forms.GroupBox groupBoxGeneral;
    }
}
