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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Common.Reflection;

namespace Gallio.Icarus
{
    public partial class AboutDialog : Form
    {
        public AboutDialog(ITestController testController)
        {
            InitializeComponent();

            // Set the application version.
            Version appVersion = AssemblyUtils.GetApplicationVersion(Assembly.GetExecutingAssembly());
            versionLabel.Text = String.Format(CultureInfo.CurrentCulture, versionLabel.Text, 
                appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);

            // add the list of available test frameworks
            // TODO: display other trait information
            componentList.Items.Clear();

            foreach (var testFramework in testController.TestFrameworks)
                componentList.Items.Add(testFramework.Name);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void websiteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://www.gallio.org");
                websiteLink.LinkVisited = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open the gallio.org website.\n" + ex);
            }
        }
    }
}