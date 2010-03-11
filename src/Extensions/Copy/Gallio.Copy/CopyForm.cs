// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Windows.Forms;
using Gallio.Copy.Properties;

namespace Gallio.Copy
{
    internal partial class CopyForm : Form
    {
        private readonly ICopyController controller;

        public CopyForm(ICopyController controller)
        {
            this.controller = controller;

            InitializeComponent();

            controller.ProgressUpdate += (s, e) =>
            {
                toolStripProgressBar.ProgressChanged(controller.ProgressMonitor);
                taskNameStatusLabel.ProgressChanged(controller.ProgressMonitor);
            };

            controller.SourcePluginFolder.PropertyChanged +=
                (s, e) => sourcePluginFolderTextBox.Text = controller.SourcePluginFolder;
            controller.TargetPluginFolder.PropertyChanged +=
                (s, e) => targetPluginFolderTextBox.Text = controller.TargetPluginFolder;

            sourcePluginTreeView.Model = controller.SourcePlugins;
            targetPluginTreeView.Model = controller.TargetPlugins;
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            controller.Shutdown();
            Close();
        }

        private void CopyButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(targetPluginFolderTextBox.Text))
            {
                MessageBox.Show(Resources.CopyForm_Please_select_a_target_folder_, 
                    Resources.CopyForm_Copy, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            controller.CopyPlugins();
        }

        private void SelectSourcePluginFolderButtonClick(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = Resources.CopyForm_Select_a_source_plugin_folder,
                SelectedPath = sourcePluginFolderTextBox.Text
            };

            using (folderBrowserDialog)
            {
                var result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                    UpdateSourcePluginFolder(folderBrowserDialog.SelectedPath);
            }
        }

        private void UpdateSourcePluginFolder(string pluginFolder)
        {
            sourcePluginFolderTextBox.Text = pluginFolder;
            controller.UpdateSourcePluginFolder(pluginFolder);
        }

        private void SelectTargetPluginFolderButtonClick(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = Resources.CopyForm_Select_a_target_plugin_folder,
                SelectedPath = targetPluginFolderTextBox.Text
            };

            using (folderBrowserDialog)
            {
                var result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                    UpdateTargetPluginFolder(folderBrowserDialog.SelectedPath);
            }
        }

        private void UpdateTargetPluginFolder(string pluginFolder)
        {
            targetPluginFolderTextBox.Text = pluginFolder;
            controller.UpdateTargetPluginFolder(pluginFolder);
        }

        private void CopyForm_Load(object sender, EventArgs e)
        {
            controller.Load();
        }
    }
}
