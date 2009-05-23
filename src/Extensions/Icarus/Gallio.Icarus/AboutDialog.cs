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
using System.Windows.Forms;
using Gallio.Icarus.Controllers;

namespace Gallio.Icarus
{
    internal partial class AboutDialog : Form
    {
        public AboutDialog(IAboutController aboutController)
        {
            InitializeComponent();

            // Set the application version.
            versionLabel.Text = aboutController.Version;

            int imgIndex = 0;

            // add the list of available test frameworks
            foreach (var testFramework in aboutController.TestFrameworks)
            {
                var item = new ListViewItem(testFramework.Name);
                if (testFramework.Icon != null)
                {
                    testFrameworkIcons.Images.Add(testFramework.Icon);
                    item.ImageIndex = imgIndex;
                    imgIndex++;
                }
                testFrameworksList.Items.Add(item);
            }
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