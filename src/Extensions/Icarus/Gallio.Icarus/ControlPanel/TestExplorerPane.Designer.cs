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
            this.alwaysReloadFilesCheckBox = new System.Windows.Forms.CheckBox();
            this.runTestsAfterReloadCheckBox = new System.Windows.Forms.CheckBox();
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.splitNamespacesCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.runtimeGroupBox = new System.Windows.Forms.GroupBox();
            this.testRunnerFactoryLabel = new System.Windows.Forms.Label();
            this.testRunnerFactories = new System.Windows.Forms.ComboBox();
            this.generalGroupBox.SuspendLayout();
            this.runtimeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // alwaysReloadFilesCheckBox
            // 
            this.alwaysReloadFilesCheckBox.AutoSize = true;
            this.alwaysReloadFilesCheckBox.Location = new System.Drawing.Point(17, 28);
            this.alwaysReloadFilesCheckBox.Name = "alwaysReloadFilesCheckBox";
            this.alwaysReloadFilesCheckBox.Size = new System.Drawing.Size(145, 17);
            this.alwaysReloadFilesCheckBox.TabIndex = 5;
            this.alwaysReloadFilesCheckBox.Text = "Always reload files";
            this.alwaysReloadFilesCheckBox.UseVisualStyleBackColor = true;
            this.alwaysReloadFilesCheckBox.CheckedChanged += new System.EventHandler(this.alwaysReloadFilesCheckBox_CheckedChanged);
            // 
            // runTestsAfterReloadCheckBox
            // 
            this.runTestsAfterReloadCheckBox.AutoSize = true;
            this.runTestsAfterReloadCheckBox.Enabled = false;
            this.runTestsAfterReloadCheckBox.Location = new System.Drawing.Point(35, 51);
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
            this.generalGroupBox.Controls.Add(this.alwaysReloadFilesCheckBox);
            this.generalGroupBox.Location = new System.Drawing.Point(0, 0);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(450, 116);
            this.generalGroupBox.TabIndex = 8;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "General";
            // 
            // splitNamespacesCheckBox
            // 
            this.splitNamespacesCheckBox.AutoSize = true;
            this.splitNamespacesCheckBox.Location = new System.Drawing.Point(17, 83);
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
            // runtimeGroupBox
            // 
            this.runtimeGroupBox.Controls.Add(this.testRunnerFactoryLabel);
            this.runtimeGroupBox.Controls.Add(this.testRunnerFactories);
            this.runtimeGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.runtimeGroupBox.Location = new System.Drawing.Point(0, 122);
            this.runtimeGroupBox.Name = "runtimeGroupBox";
            this.runtimeGroupBox.Size = new System.Drawing.Size(450, 228);
            this.runtimeGroupBox.TabIndex = 9;
            this.runtimeGroupBox.TabStop = false;
            this.runtimeGroupBox.Text = "Runtime";
            // 
            // testRunnerFactoryLabel
            // 
            this.testRunnerFactoryLabel.AutoSize = true;
            this.testRunnerFactoryLabel.Location = new System.Drawing.Point(14, 28);
            this.testRunnerFactoryLabel.Name = "testRunnerFactoryLabel";
            this.testRunnerFactoryLabel.Size = new System.Drawing.Size(102, 13);
            this.testRunnerFactoryLabel.TabIndex = 4;
            this.testRunnerFactoryLabel.Text = "Test runner factory: ";
            // 
            // testRunnerFactories
            // 
            this.testRunnerFactories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testRunnerFactories.DropDownWidth = 200;
            this.testRunnerFactories.FormattingEnabled = true;
            this.testRunnerFactories.Location = new System.Drawing.Point(122, 25);
            this.testRunnerFactories.Name = "testRunnerFactories";
            this.testRunnerFactories.Size = new System.Drawing.Size(233, 21);
            this.testRunnerFactories.TabIndex = 3;
            this.testRunnerFactories.SelectedIndexChanged += new System.EventHandler(this.testRunnerFactories_SelectedIndexChanged);
            // 
            // TestExplorerPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.runtimeGroupBox);
            this.Controls.Add(this.generalGroupBox);
            this.Name = "TestExplorerPane";
            this.Size = new System.Drawing.Size(450, 350);
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.runtimeGroupBox.ResumeLayout(false);
            this.runtimeGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox alwaysReloadFilesCheckBox;
        private System.Windows.Forms.CheckBox runTestsAfterReloadCheckBox;
        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.CheckBox splitNamespacesCheckBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox runtimeGroupBox;
        private System.Windows.Forms.Label testRunnerFactoryLabel;
        private System.Windows.Forms.ComboBox testRunnerFactories;
    }
}
