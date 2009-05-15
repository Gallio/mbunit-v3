namespace Gallio.Icarus.Options
{
    internal partial class TestExplorerOptions
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.runTestsAfterReload = new System.Windows.Forms.CheckBox();
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.splitNamespacesCheckBox = new System.Windows.Forms.CheckBox();
            this.panel.SuspendLayout();
            this.generalGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.panel.Controls.Add(this.generalGroupBox);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Size = new System.Drawing.Size(450, 252);
            // 
            // alwaysReloadAssemblies
            // 
            this.alwaysReloadAssemblies.AutoSize = true;
            this.alwaysReloadAssemblies.Location = new System.Drawing.Point(17, 30);
            this.alwaysReloadAssemblies.Name = "alwaysReloadAssemblies";
            this.alwaysReloadAssemblies.Size = new System.Drawing.Size(145, 17);
            this.alwaysReloadAssemblies.TabIndex = 5;
            this.alwaysReloadAssemblies.Text = "Always reload assemblies";
            this.alwaysReloadAssemblies.UseVisualStyleBackColor = true;
            // 
            // runTestsAfterReload
            // 
            this.runTestsAfterReload.AutoSize = true;
            this.runTestsAfterReload.Enabled = false;
            this.runTestsAfterReload.Location = new System.Drawing.Point(35, 53);
            this.runTestsAfterReload.Name = "runTestsAfterReload";
            this.runTestsAfterReload.Size = new System.Drawing.Size(127, 17);
            this.runTestsAfterReload.TabIndex = 7;
            this.runTestsAfterReload.Text = "Run tests after reload";
            this.runTestsAfterReload.UseVisualStyleBackColor = true;
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Controls.Add(this.splitNamespacesCheckBox);
            this.generalGroupBox.Controls.Add(this.checkBox1);
            this.generalGroupBox.Controls.Add(this.runTestsAfterReload);
            this.generalGroupBox.Controls.Add(this.alwaysReloadAssemblies);
            this.generalGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalGroupBox.Location = new System.Drawing.Point(0, 0);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(450, 252);
            this.generalGroupBox.TabIndex = 8;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "General";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(-15, -15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // splitNamespacesCheckBox
            // 
            this.splitNamespacesCheckBox.AutoSize = true;
            this.splitNamespacesCheckBox.Location = new System.Drawing.Point(17, 94);
            this.splitNamespacesCheckBox.Name = "splitNamespacesCheckBox";
            this.splitNamespacesCheckBox.Size = new System.Drawing.Size(109, 17);
            this.splitNamespacesCheckBox.TabIndex = 9;
            this.splitNamespacesCheckBox.Text = "Split namespaces";
            this.splitNamespacesCheckBox.UseVisualStyleBackColor = true;
            // 
            // TestExplorerOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "TestExplorerOptions";
            this.Size = new System.Drawing.Size(450, 252);
            this.panel.ResumeLayout(false);
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox alwaysReloadAssemblies;
        private System.Windows.Forms.CheckBox runTestsAfterReload;
        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.CheckBox splitNamespacesCheckBox;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
