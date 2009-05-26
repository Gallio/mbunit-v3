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
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.Icarus.ControlPanel
{
    internal partial class TestExplorerPane : PreferencePane
    {
        private readonly IOptionsController optionsController;

        public TestExplorerPane(IOptionsController optionsController)
        {
            this.optionsController = optionsController;

            InitializeComponent();

            alwaysReloadAssembliesCheckBox.Checked = optionsController.AlwaysReloadAssemblies;
            runTestsAfterReloadCheckBox.Enabled = optionsController.AlwaysReloadAssemblies;
            runTestsAfterReloadCheckBox.Checked = optionsController.RunTestsAfterReload;
            splitNamespacesCheckBox.Checked = optionsController.TestTreeSplitNamespaces;
        }

        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            optionsController.AlwaysReloadAssemblies = alwaysReloadAssembliesCheckBox.Checked;
            optionsController.RunTestsAfterReload = runTestsAfterReloadCheckBox.Checked;
            optionsController.TestTreeSplitNamespaces = splitNamespacesCheckBox.Checked;
        }

        private void alwaysReloadAssembliesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            runTestsAfterReloadCheckBox.Enabled = alwaysReloadAssembliesCheckBox.Checked;

            PendingSettingsChanges = true;
        }

        private void runTestsAfterReloadCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PendingSettingsChanges = true;
        }

        private void splitNamespacesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PendingSettingsChanges = true;
        }
    }
}
