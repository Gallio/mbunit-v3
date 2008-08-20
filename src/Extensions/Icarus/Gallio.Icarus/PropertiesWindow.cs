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
using System.IO;
using System.Windows.Forms;

using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus
{
    public partial class PropertiesWindow : DockWindow
    {
        private readonly IProjectAdapterView projectAdapterView;

        public IList<string> HintDirectories
        {
            set
            {
                hintDirectoriesListBox.Items.Clear();
                foreach (string s in value)
                    hintDirectoriesListBox.Items.Add(s);
            }
        }

        public string ApplicationBaseDirectory
        {
            set { applicationBaseDirectoryTextBox.Text = value ?? ""; }
        }

        public string WorkingDirectory
        {
            set { workingDirectoryTextBox.Text = value ?? ""; }
        }

        public bool ShadowCopy
        {
            set { shadowCopyCheckBox.Checked = value; }
        }

        public PropertiesWindow(IProjectAdapterView projectAdapterView)
        {
            this.projectAdapterView = projectAdapterView;
            InitializeComponent();
        }

        private void findHintDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                newHintDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(newHintDirectoryTextBox.Text))
            {
                hintDirectoriesListBox.Items.Add(newHintDirectoryTextBox.Text);
                newHintDirectoryTextBox.Clear();
                UpdateHintDirectories();
            }
            else
            {
                // TODO: move to resources for localisation
                string message = "Folder path does not exist." + Environment.NewLine + "Please select a valid folder path.";
                const string title = "Invalid folder path";
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void hintDirectoriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeHintDirectoryButton.Enabled = (hintDirectoriesListBox.SelectedItems.Count > 0);
        }

        private void removeHintDirectoryButton_Click(object sender, EventArgs e)
        {
            while (hintDirectoriesListBox.SelectedItems.Count > 0)
                hintDirectoriesListBox.Items.Remove(hintDirectoriesListBox.SelectedItem);
            UpdateHintDirectories();
        }

        private void UpdateHintDirectories()
        {
            List<string> list = new List<string>();
            foreach (object o in hintDirectoriesListBox.Items)
                list.Add((string)o);
            projectAdapterView.UpdateHintDirectories(list);
        }

        private void findApplicationBaseDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                applicationBaseDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void applicationBaseDirectoryTextBox_TextChanged(object sender, EventArgs e)
        {
            projectAdapterView.UpdateApplicationBaseDirectory(applicationBaseDirectoryTextBox.Text.Length == 0 ? null : applicationBaseDirectoryTextBox.Text);
        }

        private void findWorkingDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                workingDirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void workingDirectoryTextBox_TextChanged(object sender, EventArgs e)
        {
            projectAdapterView.UpdateWorkingDirectory(workingDirectoryTextBox.Text.Length == 0 ? null : workingDirectoryTextBox.Text);
        }

        private void shadowCopyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            projectAdapterView.UpdateShadowCopy(shadowCopyCheckBox.Checked);
        }
    }
}
