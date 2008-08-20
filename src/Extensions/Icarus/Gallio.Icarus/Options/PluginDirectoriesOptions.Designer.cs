namespace Gallio.Icarus.Options
{
    internal partial class PluginDirectoriesOptions
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button removePluginDirectoryButton;
        private System.Windows.Forms.Button addFolderButton;
        private System.Windows.Forms.Button findPluginDirectoryButton;
        private System.Windows.Forms.TextBox newPluginDirectoryTextBox;
        private System.Windows.Forms.Label pluginDirectoriesLabel;
        private System.Windows.Forms.ListBox pluginDirectoriesListBox;

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
            this.removePluginDirectoryButton = new System.Windows.Forms.Button();
            this.addFolderButton = new System.Windows.Forms.Button();
            this.findPluginDirectoryButton = new System.Windows.Forms.Button();
            this.newPluginDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.pluginDirectoriesLabel = new System.Windows.Forms.Label();
            this.pluginDirectoriesListBox = new System.Windows.Forms.ListBox();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.removePluginDirectoryButton);
            this.panel.Controls.Add(this.addFolderButton);
            this.panel.Controls.Add(this.pluginDirectoriesListBox);
            this.panel.Controls.Add(this.findPluginDirectoryButton);
            this.panel.Controls.Add(this.newPluginDirectoryTextBox);
            this.panel.Controls.Add(this.pluginDirectoriesLabel);
            // 
            // removePluginDirectoryButton
            // 
            this.removePluginDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removePluginDirectoryButton.Enabled = false;
            this.removePluginDirectoryButton.Image = global::Gallio.Icarus.Properties.Resources.cross;
            this.removePluginDirectoryButton.Location = new System.Drawing.Point(406, 84);
            this.removePluginDirectoryButton.Name = "removePluginDirectoryButton";
            this.removePluginDirectoryButton.Size = new System.Drawing.Size(30, 28);
            this.removePluginDirectoryButton.TabIndex = 11;
            this.removePluginDirectoryButton.UseVisualStyleBackColor = true;
            this.removePluginDirectoryButton.Click += new System.EventHandler(this.removePluginDirectoryButton_Click);
            // 
            // addFolderButton
            // 
            this.addFolderButton.Location = new System.Drawing.Point(16, 53);
            this.addFolderButton.Name = "addFolderButton";
            this.addFolderButton.Size = new System.Drawing.Size(67, 23);
            this.addFolderButton.TabIndex = 10;
            this.addFolderButton.Text = "Add Folder";
            this.addFolderButton.UseVisualStyleBackColor = true;
            this.addFolderButton.Click += new System.EventHandler(this.addFolderButton_Click);
            // 
            // findPluginDirectoryButton
            // 
            this.findPluginDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findPluginDirectoryButton.Location = new System.Drawing.Point(408, 24);
            this.findPluginDirectoryButton.Name = "findPluginDirectoryButton";
            this.findPluginDirectoryButton.Size = new System.Drawing.Size(28, 20);
            this.findPluginDirectoryButton.TabIndex = 8;
            this.findPluginDirectoryButton.Text = "...";
            this.findPluginDirectoryButton.UseVisualStyleBackColor = true;
            this.findPluginDirectoryButton.Click += new System.EventHandler(this.findPluginDirectoryButton_Click);
            // 
            // newPluginDirectoryTextBox
            // 
            this.newPluginDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newPluginDirectoryTextBox.Location = new System.Drawing.Point(16, 24);
            this.newPluginDirectoryTextBox.Name = "newPluginDirectoryTextBox";
            this.newPluginDirectoryTextBox.Size = new System.Drawing.Size(386, 20);
            this.newPluginDirectoryTextBox.TabIndex = 7;
            // 
            // pluginDirectoriesLabel
            // 
            this.pluginDirectoriesLabel.AutoSize = true;
            this.pluginDirectoriesLabel.Location = new System.Drawing.Point(13, 10);
            this.pluginDirectoriesLabel.Name = "pluginDirectoriesLabel";
            this.pluginDirectoriesLabel.Size = new System.Drawing.Size(90, 13);
            this.pluginDirectoriesLabel.TabIndex = 6;
            this.pluginDirectoriesLabel.Text = "Plugin directories:";
            // 
            // pluginDirectoriesListBox
            // 
            this.pluginDirectoriesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pluginDirectoriesListBox.FormattingEnabled = true;
            this.pluginDirectoriesListBox.Location = new System.Drawing.Point(16, 84);
            this.pluginDirectoriesListBox.Name = "pluginDirectoriesListBox";
            this.pluginDirectoriesListBox.Size = new System.Drawing.Size(384, 173);
            this.pluginDirectoriesListBox.TabIndex = 9;
            this.pluginDirectoriesListBox.SelectedIndexChanged += new System.EventHandler(this.pluginDirectoriesListBox_SelectedIndexChanged);
            // 
            // PluginDirectoriesOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "PluginDirectoriesOptions";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
