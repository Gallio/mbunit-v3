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
using Gallio.AutoCAD.Preferences;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.AutoCAD.UI.ControlPanel
{
    /// <summary>
    /// Preference pane for AutoCAD startup options.
    /// </summary>
    public partial class StartupPreferencePane : PreferencePane
    {
        private readonly IAcadPreferenceManager preferenceManager;
        private readonly bool isInitializing;

        /// <summary>
        /// Creates a new <see cref="StartupPreferencePane"/>.
        /// </summary>
        /// <param name="preferenceManager">The AutoCAD preference manager.</param>
        public StartupPreferencePane(IAcadPreferenceManager preferenceManager)
        {
            if (preferenceManager == null)
                throw new ArgumentNullException("preferenceManager");

            isInitializing = true;
            this.preferenceManager = preferenceManager;

            InitializeComponent();

            SelectedStartupAction = preferenceManager.StartupAction;
            executable.Text = preferenceManager.UserSpecifiedExecutable;
            arguments.Text = preferenceManager.CommandLineArguments;
            workingDir.Text = preferenceManager.WorkingDirectory;

            executable.DataBindings.Add("Enabled", startSpecified, "Checked");
            executableBrowse.DataBindings.Add("Enabled", startSpecified, "Checked");

            isInitializing = false;
        }

        /// <inheritdoc/>
        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            preferenceManager.CommandLineArguments = arguments.Text;
            preferenceManager.StartupAction = SelectedStartupAction;
            preferenceManager.UserSpecifiedExecutable = executable.Text;
            preferenceManager.WorkingDirectory = workingDir.Text;

            base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);
        }

        private StartupAction SelectedStartupAction
        {
            get
            {
                if (startRecent.Checked)
                    return StartupAction.StartMostRecentlyUsed;
                if (startSpecified.Checked)
                    return StartupAction.StartUserSpecified;
                if (startAttach.Checked)
                    return StartupAction.AttachToExisting;

                throw new InvalidOperationException("Preference pane not initialized.");
            }
            set
            {
                if (value == StartupAction.StartMostRecentlyUsed)
                    startRecent.Checked = true;
                else if (value == StartupAction.StartUserSpecified)
                    startSpecified.Checked = true;
                else if (value == StartupAction.AttachToExisting)
                    startAttach.Checked = true;
                else
                    throw new ArgumentException("Unknown startup action.", "value");
            }
        }

        private void PreferenceChangedHandler(object sender, EventArgs e)
        {
            startOptionsGroupBox.Enabled = !startAttach.Checked;

            if (!isInitializing)
            {
                PendingSettingsChanges = true;
            }
        }

        private void workingDirBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                dialog.Description = "Select the working directory for AutoCAD.";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    workingDir.Text = dialog.SelectedPath;
                }
            }
        }

        private void executableBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "acad.exe|acad.exe|All files (*.*)|*.*";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    executable.Text = dialog.FileName;
                }
            }
        }
    }
}
