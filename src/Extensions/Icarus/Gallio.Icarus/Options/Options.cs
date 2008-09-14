// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Icarus.Options
{
    public partial class Options : Form
    {
        private readonly IOptionsController controller;
        private string currentPanel;

        public Options(IOptionsController controller)
        {
            this.controller = controller;
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            optionCategoryTree.Sort();
            optionCategoryTree.ExpandAll();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            controller.Cancel();
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            controller.Save();
            Close();
        }

        private void optionCategoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Name)
            {
                case "appearance":
                    AddPanel(new AppearanceOptions(controller));
                    break;
                case "colors":
                    AddPanel(new ColorsOptions(controller));
                    break;
                case "pluginDirectories":
                    AddPanel(new PluginDirectoriesOptions(controller));
                    break;
                case "startup":
                    AddPanel(new StartupOptions(controller));
                    break;
                case "testExplorer":
                    AddPanel(new TestExplorerOptions(controller));
                    break;
                default:
                    ClearPanel();
                    break;
            }
        }

        private void ClearPanel()
        {
            if (Controls.ContainsKey(currentPanel))
                Controls.RemoveByKey(currentPanel);
        }

        private void AddPanel(Control optionPanel)
        {
            if (optionPanel == null)
                throw new ArgumentNullException("optionPanel");

            ClearPanel();

            Controls.Add(optionPanel);
            currentPanel = optionPanel.Name;
            optionPanel.Location = new System.Drawing.Point(195, 15);
        }
    }
}