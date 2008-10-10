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
            this.pluginDirectoriesListBox = new System.Windows.Forms.ListBox();
            this.groupBoxPluginDirectories = new System.Windows.Forms.GroupBox();
            this.panel.SuspendLayout();
            this.groupBoxPluginDirectories.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.groupBoxPluginDirectories);
            // 
            // removePluginDirectoryButton
            // 
            this.removePluginDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removePluginDirectoryButton.Enabled = false;
            this.removePluginDirectoryButton.Image = global::Gallio.Icarus.Properties.Resources.cross;
            this.removePluginDirectoryButton.Location = new System.Drawing.Point(408, 74);
            this.removePluginDirectoryButton.Name = "removePluginDirectoryButton";
            this.removePluginDirectoryButton.Size = new System.Drawing.Size(30, 28);
            this.removePluginDirectoryButton.TabIndex = 11;
            this.removePluginDirectoryButton.UseVisualStyleBackColor = true;
            this.removePluginDirectoryButton.Click += new System.EventHandler(this.removePluginDirectoryButton_Click);
            // 
            // addFolderButton
            // 
            this.addFolderButton.Location = new System.Drawing.Point(6, 45);
            this.addFolderButton.Name = "addFolderButton";
            this.addFolderButton.Size = new System.Drawing.Size(81, 23);
            this.addFolderButton.TabIndex = 10;
            this.addFolderButton.Text = "Add Folder";
            this.addFolderButton.UseVisualStyleBackColor = true;
            this.addFolderButton.Click += new System.EventHandler(this.addFolderButton_Click);
            // 
            // findPluginDirectoryButton
            // 
            this.findPluginDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findPluginDirectoryButton.Location = new System.Drawing.Point(410, 19);
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
            this.newPluginDirectoryTextBox.Location = new System.Drawing.Point(6, 19);
            this.newPluginDirectoryTextBox.Name = "newPluginDirectoryTextBox";
            this.newPluginDirectoryTextBox.Size = new System.Drawing.Size(398, 20);
            this.newPluginDirectoryTextBox.TabIndex = 7;
            // 
            // pluginDirectoriesListBox
            // 
            this.pluginDirectoriesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pluginDirectoriesListBox.FormattingEnabled = true;
            this.pluginDirectoriesListBox.Location = new System.Drawing.Point(6, 74);
            this.pluginDirectoriesListBox.Name = "pluginDirectoriesListBox";
            this.pluginDirectoriesListBox.Size = new System.Drawing.Size(396, 160);
            this.pluginDirectoriesListBox.TabIndex = 9;
            this.pluginDirectoriesListBox.SelectedIndexChanged += new System.EventHandler(this.pluginDirectoriesListBox_SelectedIndexChanged);
            // 
            // groupBoxPluginDirectories
            // 
            this.groupBoxPluginDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPluginDirectories.Controls.Add(this.newPluginDirectoryTextBox);
            this.groupBoxPluginDirectories.Controls.Add(this.removePluginDirectoryButton);
            this.groupBoxPluginDirectories.Controls.Add(this.findPluginDirectoryButton);
            this.groupBoxPluginDirectories.Controls.Add(this.pluginDirectoriesListBox);
            this.groupBoxPluginDirectories.Controls.Add(this.addFolderButton);
            this.groupBoxPluginDirectories.Location = new System.Drawing.Point(3, 3);
            this.groupBoxPluginDirectories.Name = "groupBoxPluginDirectories";
            this.groupBoxPluginDirectories.Size = new System.Drawing.Size(444, 250);
            this.groupBoxPluginDirectories.TabIndex = 12;
            this.groupBoxPluginDirectories.TabStop = false;
            this.groupBoxPluginDirectories.Text = "Plugin Directories";
            // 
            // PluginDirectoriesOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "PluginDirectoriesOptions";
            this.panel.ResumeLayout(false);
            this.groupBoxPluginDirectories.ResumeLayout(false);
            this.groupBoxPluginDirectories.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxPluginDirectories;
    }
}
