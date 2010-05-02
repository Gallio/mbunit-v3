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
using Gallio.Common.Concurrency;
using Gallio.Copy.Properties;
using Gallio.UI.Common.Synchronization;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Copy
{
    internal partial class CopyForm : Form
    {
        private readonly ICopyController copyController;
        private readonly IProgressController progressController;

        public CopyForm(ICopyController copyController, IProgressController progressController)
        {
            this.copyController = copyController;
            this.progressController = progressController;

            InitializeComponent();

            copyController.SourcePluginFolder.PropertyChanged +=
                (s, e) => sourcePluginFolderTextBox.Text = copyController.SourcePluginFolder;
            copyController.TargetPluginFolder.PropertyChanged +=
                (s, e) => targetPluginFolderTextBox.Text = copyController.TargetPluginFolder;

            sourcePluginTreeView.Model = copyController.SourcePlugins;
            targetPluginTreeView.Model = copyController.TargetPlugins;

            progressController.Status.PropertyChanged += (s, e) =>
            {
                taskNameStatusLabel.Text = progressController.Status;
            };
            progressController.TotalWork.PropertyChanged += (s, e) =>
            {
                toolStripProgressBar.TotalWork = progressController.TotalWork;
            };
            progressController.CompletedWork.PropertyChanged += (s, e) =>
            {
                toolStripProgressBar.CompletedWork = progressController.CompletedWork;
            };
            progressController.DisplayProgressDialog += (s, e) => Sync.Invoke(this, () =>
                new ProgressMonitorDialog(e.ProgressMonitor).Show(this));
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            progressController.Cancel();
            copyController.Shutdown();
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

            copyController.CopyPlugins();
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
            copyController.UpdateSourcePluginFolder(pluginFolder);
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
            copyController.UpdateTargetPluginFolder(pluginFolder);
        }

        private void CopyForm_Load(object sender, EventArgs e)
        {
            // provide WindowsFormsSynchronizationContext for cross-thread databinding
            SynchronizationContext.Current = System.Threading.SynchronizationContext.Current;

            copyController.Load();
        }
    }
}
