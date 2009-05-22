namespace Gallio.Icarus.ControlPanel
{
    internal partial class TestExplorerPane
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
            this.alwaysReloadAssembliesCheckBox = new System.Windows.Forms.CheckBox();
            this.runTestsAfterReloadCheckBox = new System.Windows.Forms.CheckBox();
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.splitNamespacesCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.generalGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // alwaysReloadAssembliesCheckBox
            // 
            this.alwaysReloadAssembliesCheckBox.AutoSize = true;
            this.alwaysReloadAssembliesCheckBox.Location = new System.Drawing.Point(17, 30);
            this.alwaysReloadAssembliesCheckBox.Name = "alwaysReloadAssembliesCheckBox";
            this.alwaysReloadAssembliesCheckBox.Size = new System.Drawing.Size(145, 17);
            this.alwaysReloadAssembliesCheckBox.TabIndex = 5;
            this.alwaysReloadAssembliesCheckBox.Text = "Always reload assemblies";
            this.alwaysReloadAssembliesCheckBox.UseVisualStyleBackColor = true;
            this.alwaysReloadAssembliesCheckBox.CheckedChanged += new System.EventHandler(this.alwaysReloadAssembliesCheckBox_CheckedChanged);
            // 
            // runTestsAfterReloadCheckBox
            // 
            this.runTestsAfterReloadCheckBox.AutoSize = true;
            this.runTestsAfterReloadCheckBox.Enabled = false;
            this.runTestsAfterReloadCheckBox.Location = new System.Drawing.Point(35, 53);
            this.runTestsAfterReloadCheckBox.Name = "runTestsAfterReloadCheckBox";
            this.runTestsAfterReloadCheckBox.Size = new System.Drawing.Size(127, 17);
            this.runTestsAfterReloadCheckBox.TabIndex = 7;
            this.runTestsAfterReloadCheckBox.Text = "Run tests after reload";
            this.runTestsAfterReloadCheckBox.UseVisualStyleBackColor = true;
            this.runTestsAfterReloadCheckBox.CheckedChanged += new System.EventHandler(this.runTestsAfterReloadCheckBox_CheckedChanged);
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Controls.Add(this.splitNamespacesCheckBox);
            this.generalGroupBox.Controls.Add(this.checkBox1);
            this.generalGroupBox.Controls.Add(this.runTestsAfterReloadCheckBox);
            this.generalGroupBox.Controls.Add(this.alwaysReloadAssembliesCheckBox);
            this.generalGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalGroupBox.Location = new System.Drawing.Point(0, 0);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(450, 350);
            this.generalGroupBox.TabIndex = 8;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "General";
            // 
            // splitNamespacesCheckBox
            // 
            this.splitNamespacesCheckBox.AutoSize = true;
            this.splitNamespacesCheckBox.Location = new System.Drawing.Point(17, 85);
            this.splitNamespacesCheckBox.Name = "splitNamespacesCheckBox";
            this.splitNamespacesCheckBox.Size = new System.Drawing.Size(109, 17);
            this.splitNamespacesCheckBox.TabIndex = 9;
            this.splitNamespacesCheckBox.Text = "Split namespaces";
            this.splitNamespacesCheckBox.UseVisualStyleBackColor = true;
            this.splitNamespacesCheckBox.CheckedChanged += new System.EventHandler(this.splitNamespacesCheckBox_CheckedChanged);
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
            // TestExplorerPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.generalGroupBox);
            this.Name = "TestExplorerPane";
            this.Size = new System.Drawing.Size(450, 350);
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox alwaysReloadAssembliesCheckBox;
        private System.Windows.Forms.CheckBox runTestsAfterReloadCheckBox;
        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.CheckBox splitNamespacesCheckBox;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
