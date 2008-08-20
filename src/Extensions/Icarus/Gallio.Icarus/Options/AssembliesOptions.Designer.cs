using System.ComponentModel;

namespace Gallio.Icarus.Options
{
    internal partial class AssembliesOptions
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
            this.alwaysReloadAssemblies = new System.Windows.Forms.CheckBox();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.alwaysReloadAssemblies);
            // 
            // alwaysReloadAssemblies
            // 
            this.alwaysReloadAssemblies.AutoSize = true;
            this.alwaysReloadAssemblies.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.alwaysReloadAssemblies.Location = new System.Drawing.Point(15, 13);
            this.alwaysReloadAssemblies.Name = "alwaysReloadAssemblies";
            this.alwaysReloadAssemblies.Size = new System.Drawing.Size(148, 17);
            this.alwaysReloadAssemblies.TabIndex = 0;
            this.alwaysReloadAssemblies.Text = "Always reload assemblies:";
            this.alwaysReloadAssemblies.UseVisualStyleBackColor = true;
            // 
            // AssembliesOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "AssembliesOptions";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckBox alwaysReloadAssemblies;

        #endregion


    }
}
