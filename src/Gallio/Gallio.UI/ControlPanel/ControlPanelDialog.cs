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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gallio.Common;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Presents a dialog with control panel options.
    /// </summary>
    public partial class ControlPanelDialog : Form
    {
        /// <summary>
        /// Creates a control panel dialog.
        /// </summary>
        public ControlPanelDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds a tab.
        /// </summary>
        /// <param name="name">The tab name</param>
        /// <param name="tabFactory">The tab factory</param>
        public void AddTab(string name, Func<ControlPanelTab> tabFactory)
        {
            TabPage tabPage = new TabPage(name);
            tabPage.Tag = tabFactory;

            controlPanelTabControl.TabPages.Add(tabPage);
        }

        private void EnsureTabCreated()
        {
            TabPage tabPage = controlPanelTabControl.SelectedTab;
            if (tabPage != null)
            {
                if (tabPage.Controls.Count == 0)
                {
                    Func<ControlPanelTab> tabFactory = (Func<ControlPanelTab>)tabPage.Tag;
                    ControlPanelTab tab = tabFactory();
                    tab.Dock = DockStyle.Fill;
                    tab.Margin = new Padding(0, 0, 0, 0);
                    tab.AutoSize = true;
                    tab.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    tab.SettingsChanged += tab_SettingsChanged;
                    tabPage.Controls.Add(tab);
                }
            }
        }

        private void ApplySettingsChanges()
        {
            foreach (TabPage tabPage in controlPanelTabControl.TabPages)
            {
                ControlPanelTab tab = (ControlPanelTab)tabPage.Controls[0];
                tab.ApplySettingsChanges();
            }
        }

        private void tab_SettingsChanged(object sender, EventArgs e)
        {
            applyButton.Enabled = true;
        }

        private void controlPanelTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnsureTabCreated();
        }

        private void ControlPanelDialog_Load(object sender, EventArgs e)
        {
            EnsureTabCreated();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            ApplySettingsChanges();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ApplySettingsChanges();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ControlPanelDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
