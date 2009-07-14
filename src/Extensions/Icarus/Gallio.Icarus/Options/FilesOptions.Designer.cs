using System.ComponentModel;

namespace Gallio.Icarus.Options
{
    internal partial class FilesOptions
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
            this.alwaysReloadFiles = new System.Windows.Forms.CheckBox();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.alwaysReloadFiles);
            // 
            // alwaysReloadFiles
            // 
            this.alwaysReloadFiles.AutoSize = true;
            this.alwaysReloadFiles.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.alwaysReloadFiles.Location = new System.Drawing.Point(15, 13);
            this.alwaysReloadFiles.Name = "alwaysReloadFiles";
            this.alwaysReloadFiles.Size = new System.Drawing.Size(148, 17);
            this.alwaysReloadFiles.TabIndex = 0;
            this.alwaysReloadFiles.Text = "Always reload files:";
            this.alwaysReloadFiles.UseVisualStyleBackColor = true;
            // 
            // FilesOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "FilesOptions";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckBox alwaysReloadFiles;

        #endregion


    }
}
