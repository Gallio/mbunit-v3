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
            this.namespaceHierarchyLabel = new System.Windows.Forms.Label();
            this.namespaceHierarchyComboBox = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.generalGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // alwaysReloadFilesCheckBox
            // 
            this.alwaysReloadFilesCheckBox.AutoSize = true;
            this.alwaysReloadFilesCheckBox.Location = new System.Drawing.Point(17, 28);
            this.alwaysReloadFilesCheckBox.Name = "alwaysReloadFilesCheckBox";
            this.alwaysReloadFilesCheckBox.Size = new System.Drawing.Size(112, 17);
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
            this.generalGroupBox.Controls.Add(this.namespaceHierarchyLabel);
            this.generalGroupBox.Controls.Add(this.namespaceHierarchyComboBox);
            this.generalGroupBox.Controls.Add(this.checkBox1);
            this.generalGroupBox.Controls.Add(this.runTestsAfterReloadCheckBox);
            this.generalGroupBox.Controls.Add(this.alwaysReloadFilesCheckBox);
            this.generalGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalGroupBox.Location = new System.Drawing.Point(0, 0);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(397, 320);
            this.generalGroupBox.TabIndex = 8;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "General";
            // 
            // namespaceHierarchyLabel
            // 
            this.namespaceHierarchyLabel.AutoSize = true;
            this.namespaceHierarchyLabel.Location = new System.Drawing.Point(14, 89);
            this.namespaceHierarchyLabel.Name = "namespaceHierarchyLabel";
            this.namespaceHierarchyLabel.Size = new System.Drawing.Size(113, 13);
            this.namespaceHierarchyLabel.TabIndex = 10;
            this.namespaceHierarchyLabel.Text = "Namespace hierarchy:";
            // 
            // namespaceHierarchyComboBox
            // 
            this.namespaceHierarchyComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.namespaceHierarchyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.namespaceHierarchyComboBox.FormattingEnabled = true;
            this.namespaceHierarchyComboBox.Location = new System.Drawing.Point(134, 86);
            this.namespaceHierarchyComboBox.Name = "namespaceHierarchyComboBox";
            this.namespaceHierarchyComboBox.Size = new System.Drawing.Size(245, 21);
            this.namespaceHierarchyComboBox.TabIndex = 9;
            this.namespaceHierarchyComboBox.SelectedIndexChanged += new System.EventHandler(this.namespaceHierarchyComboBox_SelectedIndexChanged);
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
            this.Size = new System.Drawing.Size(397, 320);
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox alwaysReloadFilesCheckBox;
        private System.Windows.Forms.CheckBox runTestsAfterReloadCheckBox;
        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label namespaceHierarchyLabel;
        private System.Windows.Forms.ComboBox namespaceHierarchyComboBox;
    }
}
