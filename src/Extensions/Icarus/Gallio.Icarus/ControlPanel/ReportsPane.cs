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

using Gallio.Icarus.Controllers.Interfaces;
using Gallio.UI.ControlPanel.Preferences;
using Gallio.Runtime.Security;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.ControlPanel
{
    internal partial class ReportsPane : PreferencePane
    {
        private readonly IOptionsController optionsController;

        public ReportsPane(IOptionsController optionsController)
        {
            this.optionsController = optionsController;

            InitializeComponent();

            generateReportAfterTestRun.Checked = optionsController.GenerateReportAfterTestRun;
        }

        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            optionsController.GenerateReportAfterTestRun = generateReportAfterTestRun.Checked;

            optionsController.Save();

            base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);
        }

        private void generateReportAfterTestRun_CheckedChanged(object sender, System.EventArgs e)
        {
            if (generateReportAfterTestRun.Checked != optionsController.GenerateReportAfterTestRun)
                PendingSettingsChanges = true;
        }
    }
}
