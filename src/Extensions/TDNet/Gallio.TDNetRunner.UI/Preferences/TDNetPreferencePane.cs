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
using Gallio.Model;
using Gallio.Runtime.Installer;
using Gallio.Runtime.Preferences;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;
using Gallio.TDNetRunner.Core;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.TDNetRunner.UI.Preferences
{
    public partial class TDNetPreferencePane : PreferencePane
    {
        private Guid[] frameworkIds;

        public TDNetPreferencePane()
        {
            InitializeComponent();

            RequiresElevation = true;
        }

        public ITestFrameworkManager FrameworkManager { get; set; }

        public IPreferenceManager PreferenceManager { get; set; }

        public IInstallerManager InstallerManager { get; set; }

        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Saving TestDriven.Net preferences.", 2))
            {
                base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);

                for (int i = 0; i < frameworkIds.Length; i++)
                {
                    var installationMode = InstallationModeFromString((string) frameworkGridView.Rows[i].Cells[1].Value);
                    SetInstallationModeForFramework(frameworkIds[i], installationMode);
                }
                progressMonitor.Worked(1);

                InstallerManager.Install(new[] {TDNetRunnerInstaller.InstallerId}, elevationContext, progressMonitor.CreateSubProgressMonitor(1));
            }
        }

        private void TDNetPreferencePane_Load(object sender, EventArgs e)
        {
            var frameworkHandles = FrameworkManager.FrameworkHandles;
            frameworkIds = new Guid[frameworkHandles.Count];

            for (int i = 0; i < frameworkHandles.Count; i++)
            {
                TestFrameworkTraits traits = frameworkHandles[i].GetTraits();
                Guid frameworkId = traits.Id;
                TDNetRunnerInstallationMode installationMode = GetInstallationModeForFramework(frameworkId);
                frameworkGridView.Rows.Add(traits.Name, InstallationModeToString(installationMode));
                frameworkIds[i] = frameworkId;
            }
        }

        private void frameworkGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (frameworkIds == null)
                return; // This method may be called during InitializeComponent.

            RefreshSettingsChanges();
        }

        private void RefreshSettingsChanges()
        {
            for (int i = 0; i < frameworkIds.Length; i++)
            {
                var installationMode = InstallationModeFromString((string) frameworkGridView.Rows[i].Cells[1].Value);
                if (installationMode != GetInstallationModeForFramework(frameworkIds[i]))
                {
                    PendingSettingsChanges = true;
                    return;
                }
            }

            PendingSettingsChanges = false;
        }

        private string InstallationModeToString(TDNetRunnerInstallationMode mode)
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

        private TDNetRunnerInstallationMode InstallationModeFromString(string str)
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

        private TDNetRunnerInstallationMode GetInstallationModeForFramework(Guid frameworkId)
        {
            return TDNetRunnerInstallationMode.Default;
        }

        private void SetInstallationModeForFramework(Guid frameworkId, TDNetRunnerInstallationMode mode)
        {
        }
    }
}
