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
using System.ComponentModel;
using System.Windows.Forms;
using Gallio.Model;
using Gallio.Runtime.Installer;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;
using Gallio.TDNetRunner.Core;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.TDNetRunner.UI.Preferences
{
    public partial class TDNetRunnerInstallationPreferencePane : PreferencePane
    {
        private bool frameworksPopulated;

        public TDNetRunnerInstallationPreferencePane()
        {
            InitializeComponent();

            RequiresElevation = true;
        }

        public ITestFrameworkManager FrameworkManager { get; set; }

        public TDNetPreferenceManager PreferenceManager { get; set; }

        public IInstallerManager InstallerManager { get; set; }

        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Saving TestDriven.Net preferences.", 2))
            {
                base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);

                foreach (DataGridViewRow row in frameworkGridView.Rows)
                {
                    var installationMode = InstallationModeFromString((string)row.Cells[1].Value);
                    var frameworkId = (string)row.Tag;

                    PreferenceManager.SetInstallationModeForFramework(frameworkId, installationMode);
                }
                progressMonitor.Worked(1);

                InstallerManager.Install(new[] {TDNetRunnerInstaller.InstallerId}, elevationContext, progressMonitor.CreateSubProgressMonitor(1));
            }
        }

        private void TDNetPreferencePane_Load(object sender, EventArgs e)
        {
            foreach (var frameworkHandle in FrameworkManager.FrameworkHandles)
            {
                TestFrameworkTraits traits = frameworkHandle.GetTraits();
                string frameworkId = frameworkHandle.Id;
                TDNetRunnerInstallationMode installationMode = PreferenceManager.GetInstallationModeForFramework(frameworkId);
                int index = frameworkGridView.Rows.Add(traits.Name, InstallationModeToString(installationMode));
                DataGridViewRow row = frameworkGridView.Rows[index];
                row.Tag = frameworkId;
            }

            frameworkGridView.Sort(FrameworkNameColumn, ListSortDirection.Ascending);
            frameworksPopulated = true;
        }

        private void frameworkGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (! frameworksPopulated)
                return; // This method may be called during InitializeComponent.

            RefreshSettingsChanges();
        }

        private void RefreshSettingsChanges()
        {
            foreach (DataGridViewRow row in frameworkGridView.Rows)
            {
                var installationMode = InstallationModeFromString((string) row.Cells[1].Value);
                var frameworkId = (string) row.Tag;

                if (installationMode != PreferenceManager.GetInstallationModeForFramework(frameworkId))
                {
                    PendingSettingsChanges = true;
                    return;
                }
            }

            PendingSettingsChanges = false;
        }

        private static string InstallationModeToString(TDNetRunnerInstallationMode mode)
        {
            switch (mode)
            {
                case TDNetRunnerInstallationMode.Disabled:
                    return "Disabled";

                case TDNetRunnerInstallationMode.Preferred:
                    return "Preferred";

                default:
                    return "Default";
            }
        }

        private static TDNetRunnerInstallationMode InstallationModeFromString(string str)
        {
            switch (str)
            {
                default:
                    return TDNetRunnerInstallationMode.Default;

                case "Preferred":
                    return TDNetRunnerInstallationMode.Preferred;

                case "Disabled":
                    return TDNetRunnerInstallationMode.Disabled;
            }
        }
    }
}
