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
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runner;
using Gallio.Runner.Extensions;
using Gallio.UI.ControlPanel.Preferences;
using Gallio.UI.ErrorReporting;

namespace Gallio.Icarus.ControlPanel
{
    internal partial class TestRunnerExtensionsPane : PreferencePane
    {
        private readonly IOptionsController optionsController;
        private readonly IList<string> testRunnerExtensions;

        public TestRunnerExtensionsPane(IOptionsController optionsController)
        {
            if (optionsController == null) 
                throw new ArgumentNullException("optionsController");

            this.optionsController = optionsController;

            InitializeComponent();

            testRunnerExtensions = new List<string>(optionsController.TestRunnerExtensions);
            foreach (var testRunnerExtension in testRunnerExtensions)
                testRunnerExtensionsListBox.Items.Add(testRunnerExtension);
        }

        private void removeExtensionButton_Click(object sender, EventArgs e)
        {
            testRunnerExtensions.Remove((string)testRunnerExtensionsListBox.SelectedItem);
            PendingSettingsChanges = true;
            testRunnerExtensionsListBox.Items.Remove(testRunnerExtensionsListBox.SelectedItem);
        }

        private void addExtensionButton_Click(object sender, EventArgs e)
        {
            try
            {
                TestRunnerExtensionUtils.CreateExtensionFromSpecification(newExtensionTextBox.Text);
            }
            catch (RunnerException ex)
            {
                ErrorDialog.Show(this, "Add Extension", 
                    "Could not instantiate an extension from the given specification. The extension has not been added.", 
                    ex.ToString());
                return;
            }

            testRunnerExtensions.Add(newExtensionTextBox.Text);
            PendingSettingsChanges = true;
            testRunnerExtensionsListBox.Items.Add(newExtensionTextBox.Text);
            newExtensionTextBox.Text = string.Empty;
        }

        private void testRunnerExtensionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeExtensionButton.Enabled = (testRunnerExtensionsListBox.SelectedItems.Count > 0);
        }

        public override void ApplyPendingSettingsChanges(Runtime.Security.IElevationContext elevationContext, 
            Runtime.ProgressMonitoring.IProgressMonitor progressMonitor)
        {
            base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);

            optionsController.TestRunnerExtensions.Clear();
            foreach (var testRunnerExtension in testRunnerExtensions)
                optionsController.TestRunnerExtensions.Add(testRunnerExtension);

            optionsController.Save();
        }
    }
}
