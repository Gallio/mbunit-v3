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
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.Icarus.ControlPanel
{
    internal partial class TestStatusPane : PreferencePane
    {
        private readonly IOptionsController optionsController;

        public TestStatusPane(IOptionsController optionsController)
        {
            this.optionsController = optionsController;

            InitializeComponent();

            passedColor.BackColor = optionsController.PassedColor;
            failedColor.BackColor = optionsController.FailedColor;
            inconclusiveColor.BackColor = optionsController.InconclusiveColor;
            skippedColor.BackColor = optionsController.SkippedColor;

            testProgressBarStyle.Text = optionsController.TestStatusBarStyle;
        }

        private void color_Click(object sender, EventArgs e)
        {
            var label = (Label)sender;
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = label.BackColor;

                if (colorDialog.ShowDialog() != DialogResult.OK)
                    return;

                label.BackColor = colorDialog.Color;
                PendingSettingsChanges = true;
            }
        }

        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            optionsController.PassedColor = passedColor.BackColor;
            optionsController.FailedColor = failedColor.BackColor;
            optionsController.InconclusiveColor = inconclusiveColor.BackColor;
            optionsController.SkippedColor = skippedColor.BackColor;

            optionsController.TestStatusBarStyle = testProgressBarStyle.Text;

            optionsController.Save();

            base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);
        }

        private void testProgressBarStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            PendingSettingsChanges = true;
        }
    }
}
