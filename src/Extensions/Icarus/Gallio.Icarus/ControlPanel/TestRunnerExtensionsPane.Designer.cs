namespace Gallio.Icarus.ControlPanel
{
    internal partial class TestRunnerExtensionsPane
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button removeExtensionButton;
        private System.Windows.Forms.Button addExtensionButton;
        private System.Windows.Forms.TextBox newExtensionTextBox;
        private System.Windows.Forms.ListBox testRunnerExtensionsListBox;

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
            this.removeExtensionButton = new System.Windows.Forms.Button();
            this.addExtensionButton = new System.Windows.Forms.Button();
            this.newExtensionTextBox = new System.Windows.Forms.TextBox();
            this.testRunnerExtensionsListBox = new System.Windows.Forms.ListBox();
            this.groupBoxPluginDirectories = new System.Windows.Forms.GroupBox();
            this.groupBoxPluginDirectories.SuspendLayout();
            this.SuspendLayout();
            // 
            // removeExtensionButton
            // 
            this.removeExtensionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeExtensionButton.Enabled = false;
            this.removeExtensionButton.Image = global::Gallio.Icarus.Properties.Resources.cross;
            this.removeExtensionButton.Location = new System.Drawing.Point(414, 74);
            this.removeExtensionButton.Name = "removeExtensionButton";
            this.removeExtensionButton.Size = new System.Drawing.Size(30, 28);
            this.removeExtensionButton.TabIndex = 11;
            this.removeExtensionButton.UseVisualStyleBackColor = true;
            this.removeExtensionButton.Click += new System.EventHandler(this.removeExtensionButton_Click);
            // 
            // addExtensionButton
            // 
            this.addExtensionButton.AutoSize = true;
            this.addExtensionButton.Location = new System.Drawing.Point(6, 45);
            this.addExtensionButton.Name = "addExtensionButton";
            this.addExtensionButton.Size = new System.Drawing.Size(85, 23);
            this.addExtensionButton.TabIndex = 10;
            this.addExtensionButton.Text = "Add Extension";
            this.addExtensionButton.UseVisualStyleBackColor = true;
            this.addExtensionButton.Click += new System.EventHandler(this.addExtensionButton_Click);
            // 
            // newExtensionTextBox
            // 
            this.newExtensionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newExtensionTextBox.Location = new System.Drawing.Point(6, 19);
            this.newExtensionTextBox.Name = "newExtensionTextBox";
            this.newExtensionTextBox.Size = new System.Drawing.Size(404, 20);
            this.newExtensionTextBox.TabIndex = 7;
            // 
            // testRunnerExtensionsListBox
            // 
            this.testRunnerExtensionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.testRunnerExtensionsListBox.FormattingEnabled = true;
            this.testRunnerExtensionsListBox.Location = new System.Drawing.Point(6, 74);
            this.testRunnerExtensionsListBox.Name = "testRunnerExtensionsListBox";
            this.testRunnerExtensionsListBox.Size = new System.Drawing.Size(402, 264);
            this.testRunnerExtensionsListBox.TabIndex = 9;
            this.testRunnerExtensionsListBox.SelectedIndexChanged += new System.EventHandler(this.testRunnerExtensionsListBox_SelectedIndexChanged);
            // 
            // groupBoxPluginDirectories
            // 
            this.groupBoxPluginDirectories.Controls.Add(this.newExtensionTextBox);
            this.groupBoxPluginDirectories.Controls.Add(this.removeExtensionButton);
            this.groupBoxPluginDirectories.Controls.Add(this.testRunnerExtensionsListBox);
            this.groupBoxPluginDirectories.Controls.Add(this.addExtensionButton);
            this.groupBoxPluginDirectories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPluginDirectories.Location = new System.Drawing.Point(0, 0);
            this.groupBoxPluginDirectories.Name = "groupBoxPluginDirectories";
            this.groupBoxPluginDirectories.Size = new System.Drawing.Size(450, 341);
            this.groupBoxPluginDirectories.TabIndex = 12;
            this.groupBoxPluginDirectories.TabStop = false;
            this.groupBoxPluginDirectories.Text = "Test Runner Extensions";
            // 
            // TestRunnerExtensionsPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.groupBoxPluginDirectories);
            this.Name = "TestRunnerExtensionsPane";
            this.Size = new System.Drawing.Size(450, 341);
            this.groupBoxPluginDirectories.ResumeLayout(false);
            this.groupBoxPluginDirectories.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxPluginDirectories;
    }
}