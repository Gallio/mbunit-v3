// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus
{
    public partial class Options : Form
    {
        private IProjectAdapterView projectAdapterView;
        private Settings settings;

        public Options(IProjectAdapterView projectAdapterView)
        {
            this.projectAdapterView = projectAdapterView;
            settings = projectAdapterView.Settings.Clone();
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            optionCategoryTree.Sort();
            optionCategoryTree.ExpandAll();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            projectAdapterView.Settings = settings;
            Close();
        }

        private void optionCategoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            optionsPanel.Controls.Clear();
            switch (e.Node.Name)
            {
                case "pluginDirectories":
                    CreatePluginDirectoriesControls();
                    break;
                case "statusBar":
                    CreateStatusBarControls();
                    break;
                case "startup":
                    CreateStartupControls();
                    break;
            }
        }

        private void CreateStatusBarControls()
        {
        }

        private void CreatePluginDirectoriesControls()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PropertiesWindow));
            Button removePluginDirectoryButton = new Button();
            Button addFolderButton = new Button();
            Button findPluginDirectoryButton = new Button();
            TextBox newPluginDirectoryTextBox = new TextBox();
            Label pluginDirectoriesLabel = new Label();
            ListBox pluginDirectoriesListBox = new ListBox();
            // 
            // removePluginDirectoryButton
            // 
            removePluginDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            removePluginDirectoryButton.Enabled = false;
            removePluginDirectoryButton.Image = ((System.Drawing.Image)(resources.GetObject("removePluginDirectoryButton.Image")));
            removePluginDirectoryButton.Location = new System.Drawing.Point(397, 82);
            removePluginDirectoryButton.Name = "removePluginDirectoryButton";
            removePluginDirectoryButton.Size = new System.Drawing.Size(30, 28);
            removePluginDirectoryButton.TabIndex = 11;
            removePluginDirectoryButton.UseVisualStyleBackColor = true;
            removePluginDirectoryButton.Click += delegate
            {
                while (pluginDirectoriesListBox.SelectedItems.Count > 0)
                {
                    settings.PluginDirectories.Remove((string)pluginDirectoriesListBox.SelectedItem);
                    pluginDirectoriesListBox.Items.Remove(pluginDirectoriesListBox.SelectedItem);
                }
            };
            // 
            // addFolderButton
            // 
            addFolderButton.Location = new System.Drawing.Point(16, 53);
            addFolderButton.Name = "addFolderButton";
            addFolderButton.Size = new System.Drawing.Size(67, 23);
            addFolderButton.TabIndex = 10;
            addFolderButton.Text = "Add Folder";
            addFolderButton.UseVisualStyleBackColor = true;
            addFolderButton.Click += delegate
            {
                if (Directory.Exists(newPluginDirectoryTextBox.Text))
                {
                    if (!pluginDirectoriesListBox.Items.Contains(newPluginDirectoryTextBox.Text))
                    {
                        pluginDirectoriesListBox.Items.Add(newPluginDirectoryTextBox.Text);
                        settings.PluginDirectories.Add(newPluginDirectoryTextBox.Text);
                        newPluginDirectoryTextBox.Clear();
                        // TODO: move to resources for localisation
                        string message = "Added plugin folder. Plugins in this folder will not be loaded until the next time the application starts.";
                        string title = "Added plugin folder";
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // TODO: move to resources for localisation
                        string message = "That folder is already monitored. Please select another folder.";
                        string title = "Duplicate folder path";
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // TODO: move to resources for localisation
                    string message = "Folder path does not exist." + Environment.NewLine + "Please select a valid folder path.";
                    string title = "Invalid folder path";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // 
            // findPluginDirectoryButton
            // 
            findPluginDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            findPluginDirectoryButton.Location = new System.Drawing.Point(397, 25);
            findPluginDirectoryButton.Name = "findPluginDirectoryButton";
            findPluginDirectoryButton.Size = new System.Drawing.Size(28, 23);
            findPluginDirectoryButton.TabIndex = 8;
            findPluginDirectoryButton.Text = "...";
            findPluginDirectoryButton.UseVisualStyleBackColor = true;
            findPluginDirectoryButton.Click += delegate
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    newPluginDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
            };
            // 
            // newPluginDirectoryTextBox
            // 
            newPluginDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            newPluginDirectoryTextBox.Location = new System.Drawing.Point(16, 27);
            newPluginDirectoryTextBox.Name = "newPluginDirectoryTextBox";
            newPluginDirectoryTextBox.Size = new System.Drawing.Size(375, 20);
            newPluginDirectoryTextBox.TabIndex = 7;
            // 
            // pluginDirectoriesLabel
            // 
            pluginDirectoriesLabel.AutoSize = true;
            pluginDirectoriesLabel.Location = new System.Drawing.Point(13, 10);
            pluginDirectoriesLabel.Name = "pluginDirectoriesLabel";
            pluginDirectoriesLabel.Size = new System.Drawing.Size(90, 13);
            pluginDirectoriesLabel.TabIndex = 6;
            pluginDirectoriesLabel.Text = "Plugin directories:";
            // 
            // pluginDirectoriesListBox
            // 
            pluginDirectoriesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            pluginDirectoriesListBox.FormattingEnabled = true;
            pluginDirectoriesListBox.Location = new System.Drawing.Point(16, 84);
            pluginDirectoriesListBox.Name = "pluginDirectoriesListBox";
            pluginDirectoriesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            pluginDirectoriesListBox.Size = new System.Drawing.Size(366, 95);
            pluginDirectoriesListBox.TabIndex = 9;
            pluginDirectoriesListBox.SelectedIndexChanged += delegate
            {
                removePluginDirectoryButton.Enabled = (pluginDirectoriesListBox.SelectedItems.Count > 0);
            };
            foreach (string dir in settings.PluginDirectories)
                pluginDirectoriesListBox.Items.Add(dir);
            
            optionsPanel.Controls.Add(removePluginDirectoryButton);
            optionsPanel.Controls.Add(addFolderButton);
            optionsPanel.Controls.Add(pluginDirectoriesListBox);
            optionsPanel.Controls.Add(findPluginDirectoryButton);
            optionsPanel.Controls.Add(newPluginDirectoryTextBox);
            optionsPanel.Controls.Add(pluginDirectoriesLabel);
        }

        private void CreateStartupControls()
        {
            // restorePreviousSession
            CheckBox restorePreviousSession;
            restorePreviousSession = new System.Windows.Forms.CheckBox();
            restorePreviousSession.AutoSize = true;
            restorePreviousSession.Location = new System.Drawing.Point(15, 13);
            restorePreviousSession.Name = "restorePreviousSession";
            restorePreviousSession.Size = new System.Drawing.Size(144, 17);
            restorePreviousSession.TabIndex = 0;
            restorePreviousSession.Text = "Restore previous session";
            restorePreviousSession.UseVisualStyleBackColor = true;
            restorePreviousSession.Checked = settings.RestorePreviousSettings;
            restorePreviousSession.CheckedChanged += delegate { settings.RestorePreviousSettings = restorePreviousSession.Checked; };
            optionsPanel.Controls.Add(restorePreviousSession);
        }
    }
}