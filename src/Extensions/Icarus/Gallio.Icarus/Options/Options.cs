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
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus.Options
{
    internal partial class Options : Form
    {
        private readonly IOptionsController controller;

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

        private void Options_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                controller.Save();
            }
            else
            {
                controller.Cancel();
            }
        }

        private void optionCategoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var panels = new Dictionary<string, Type>
            {
                { "appearanceNode", typeof(AppearanceOptions) },
                { "testStatusColorsNode", typeof(ColorsOptions) },
                { "runtimeNode", typeof(PluginDirectoriesOptions) },
                { "pluginDirectoriesNode", typeof(PluginDirectoriesOptions) },
                { "startupNode", typeof(StartupOptions) },
                { "testExplorerNode", typeof(TestExplorerOptions) },
                { "testExplorerGeneralNode", typeof(TestExplorerOptions) },
                { "treeViewCategoriesNode", typeof(TreeViewCategoryOptions) },
                { "reportsNode", typeof(ReportOptions) },
                { "testRunnerExtensionsNode", typeof(TestRunnerExtensionOptions) }
            };

            Type panelType;

            if (panels.TryGetValue(e.Node.Name, out panelType))
            {
                AddPanel((Control)Activator.CreateInstance(panelType, controller));
            }
            else
            {
                ClearPanel();
            }
        }

        private void ClearPanel()
        {
            panelContainer.Controls.Clear();
        }

        private void AddPanel(Control optionPanel)
        {
            if (optionPanel == null)
                throw new ArgumentNullException("optionPanel");

            ClearPanel();
            panelContainer.Controls.Add(optionPanel);
            optionPanel.Dock = DockStyle.Fill;
        }
    }
}