// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus
{
    public partial class Options : Form
    {
        private IProjectAdapterView projectAdapterView;
        private Settings settings;

        public Options(IProjectAdapterView projectAdapterView)
        {
            this.projectAdapterView = projectAdapterView;
            settings = projectAdapterView.Settings.Clone();
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            optionCategoryTree.Sort();
            optionCategoryTree.ExpandAll();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            projectAdapterView.Settings = settings;
            Close();
        }

        private void optionCategoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            optionsPanel.Controls.Clear();
            switch (e.Node.Name)
            {
                case "statusBar":
                    break;
                case "startup":
                    CreateStartupControls();
                    break;
            }
        }

        private void CreateStartupControls()
        {
            // restorePreviousSession
            CheckBox restorePreviousSession;
            restorePreviousSession = new System.Windows.Forms.CheckBox();
            restorePreviousSession.AutoSize = true;
            restorePreviousSession.Location = new System.Drawing.Point(15, 13);
            restorePreviousSession.Name = "restorePreviousSession";
            restorePreviousSession.Size = new System.Drawing.Size(144, 17);
            restorePreviousSession.TabIndex = 0;
            restorePreviousSession.Text = "Restore previous session";
            restorePreviousSession.UseVisualStyleBackColor = true;
            restorePreviousSession.Checked = settings.RestorePreviousSettings;
            restorePreviousSession.CheckedChanged += delegate { settings.RestorePreviousSettings = restorePreviousSession.Checked; };
            optionsPanel.Controls.Add(restorePreviousSession);
        }
    }
}