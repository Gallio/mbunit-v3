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
using System.Diagnostics;
using System.IO;
using Gallio.UI.ControlPanel.Preferences;
using Gallio.UI.ErrorReporting;

namespace Gallio.Ambience.UI.Preferences
{
    public partial class AmbienceServerPreferencePane : PreferencePane
    {
        public AmbienceServerPreferencePane()
        {
            InitializeComponent();
        }

        private void launchServicesTool_Click(object sender, EventArgs e)
        {
            string toolPath = Path.Combine(Environment.SystemDirectory, "Services.msc");
            try
            {
                Process.Start(toolPath);
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Error Launching Services Admin Tool",
                    String.Format("Could not start '{0}'.", toolPath),
                    ex.ToString());
            }
        }
    }
}