using System;
using System.IO;
using System.Windows.Forms;
using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus.Options
{
    internal partial class PluginDirectoriesOptions : OptionsPanel
    {
        private readonly IOptionsController optionsController;

        public PluginDirectoriesOptions(IOptionsController optionsController)
        {
            this.optionsController = optionsController;
            InitializeComponent();
            pluginDirectoriesListBox.DataSource = optionsController.PluginDirectories;
        }

        private void findPluginDirectoryButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    newPluginDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void removePluginDirectoryButton_Click(object sender, EventArgs e)
        {
            if (optionsController != null)
                optionsController.RemovePluginDirectory((string) pluginDirectoriesListBox.SelectedItem);
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(newPluginDirectoryTextBox.Text))
            {
                if (!pluginDirectoriesListBox.Items.Contains(newPluginDirectoryTextBox.Text))
                {
                    optionsController.AddPluginDirectory(newPluginDirectoryTextBox.Text);
                    newPluginDirectoryTextBox.Clear();
                    // TODO: move to resources for localisation
                    const string message =
                        "Added plugin folder. Plugins in this folder will not be loaded until the next time the application starts.";
                    const string title = "Added plugin folder";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // TODO: move to resources for localisation
                    const string message = "That folder is already being monitored. Please select a different folder.";
                    const string title = "Duplicate folder path";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // TODO: move to resources for localisation
                const string message = "Folder path does not exist. Please select a valid folder path.";
                const string title = "Invalid folder path";
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pluginDirectoriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removePluginDirectoryButton.Enabled = (pluginDirectoriesListBox.SelectedItems.Count > 0);
        }
    }
}
