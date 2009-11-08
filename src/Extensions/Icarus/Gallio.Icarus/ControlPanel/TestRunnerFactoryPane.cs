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
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;
using Gallio.UI.ControlPanel.Preferences;
using Gallio.Runtime;
using Gallio.Common.Collections;
using Gallio.Runner;

namespace Gallio.Icarus.ControlPanel
{
    internal partial class TestRunnerFactoryPane : PreferencePane
    {
        private readonly IOptionsController optionsController;

        public TestRunnerFactoryPane(IOptionsController optionsController)
        {
            this.optionsController = optionsController;

            InitializeComponent();

            // retrieve list of possible factories
            var testRunnerManager = RuntimeAccessor.ServiceLocator.Resolve<ITestRunnerManager>();
            string[] factories = GenericCollectionUtils.ConvertAllToArray(testRunnerManager.TestRunnerFactoryHandles,
                h => h.GetTraits().Name);

            testRunnerFactories.Items.AddRange(factories);
            testRunnerFactories.Text = optionsController.TestRunnerFactory;
        }

        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);
            optionsController.TestRunnerFactory.Value = testRunnerFactories.Text;
            optionsController.Save();
        }

        private void testRunnerFactories_SelectedIndexChanged(object sender, EventArgs e)
        {
            PendingSettingsChanges = true;
        }
    }
}
