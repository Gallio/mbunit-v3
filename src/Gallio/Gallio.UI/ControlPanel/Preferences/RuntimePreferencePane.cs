// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;

namespace Gallio.UI.ControlPanel.Preferences
{
    internal partial class RuntimePreferencePane : PreferencePane
    {
        private const string CommitterId = "Gallio.UI.RuntimePreferencePaneCommitterElevatedCommand";
        private InstallationConfiguration configuration;

        public RuntimePreferencePane()
        {
            InitializeComponent();

            RequiresElevation = true;
        }

        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            elevationContext.Execute(CommitterId, configuration, progressMonitor);

            PendingSettingsChanges = false;
        }

        private void PathPreferencePane_Load(object sender, EventArgs e)
        {
            configuration = InstallationConfiguration.LoadFromRegistry();

            runtimePathTextBox.Text = configuration.InstallationFolder;

            foreach (string path in configuration.AdditionalPluginDirectories)
                pluginPathListBox.Items.Add(path);
            UpdateRemovePluginPathButtonVisibility();
        }

        private void addPluginPathButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                dialog.Description = "Select a directory to add to the plugin path.";
                DialogResult result = dialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    string newPath = dialog.SelectedPath;
                    if (!configuration.AdditionalPluginDirectories.Contains(newPath))
                    {
                        configuration.AdditionalPluginDirectories.Add(newPath);
                        pluginPathListBox.Items.Add(newPath);
                        PendingSettingsChanges = true;
                    }
                }
            }
        }

        private void removePluginPathButton_Click(object sender, EventArgs e)
        {
            if (pluginPathListBox.SelectedItem != null)
            {
                configuration.AdditionalPluginDirectories.Remove((string) pluginPathListBox.SelectedItem);
                pluginPathListBox.Items.RemoveAt(pluginPathListBox.SelectedIndex);
                PendingSettingsChanges = true;
            }
        }

        private void UpdateRemovePluginPathButtonVisibility()
        {
            removePluginPathButton.Enabled = pluginPathListBox.SelectedIndex >= 0;
        }

        private void pluginPathListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRemovePluginPathButtonVisibility();
        }
    }
}
