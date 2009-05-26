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
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Presents a dialog with control panel options.
    /// </summary>
    internal partial class ControlPanelDialog : Form
    {
        private bool requiresElevation;

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

        /// <summary>
        /// Gets or sets the elevation manager used to obtain an elevation context.
        /// </summary>
        public IElevationManager ElevationManager { get; set; }

        /// <summary>
        /// Gets or sets the progress monitor provider to use.
        /// </summary>
        public IProgressMonitorProvider ProgressMonitorProvider { get; set; }

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
                    tab.PendingSettingsChangesChanged += tab_PropertiesChanged;
                    tab.RequiresElevationChanged += tab_PropertiesChanged;
                    tabPage.Controls.Add(tab);

                    RefreshActionState();
                }
            }
        }

        private void ApplySettingsChanges()
        {
            ProgressMonitorProvider.Run(progressMonitor =>
            {
                if (requiresElevation)
                {
                    ElevationManager.TryElevate(elevationContext => ApplySettingsChanges(elevationContext, progressMonitor),
                        "Administrative access required to apply certain settings changes.");
                }
                else
                {
                    ApplySettingsChanges(null, progressMonitor);
                }
            });
        }

        private bool ApplySettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Applying changes.", 1))
            {
                var tabs = new List<ControlPanelTab>(GetControlPanelTabs());
                if (tabs.Count == 0)
                    return true;

                double workPerTab = 1.0 / tabs.Count;

                foreach (ControlPanelTab tab in tabs)
                {
                    if (progressMonitor.IsCanceled)
                        return false;

                    if (tab.PendingSettingsChanges)
                    {
                        tab.ApplyPendingSettingsChanges(tab.RequiresElevation ? elevationContext : null, progressMonitor.CreateSubProgressMonitor(workPerTab));
                    }
                    else
                    {
                        progressMonitor.Worked(workPerTab);
                    }
                }
            }

            return true;
        }

        private IEnumerable<ControlPanelTab> GetControlPanelTabs()
        {
            foreach (TabPage tabPage in controlPanelTabControl.TabPages)
            {
                if (tabPage.Controls.Count != 0)
                    yield return (ControlPanelTab)tabPage.Controls[0];
            }
        }

        private void RefreshActionState()
        {
            bool combinedPendingSettingsChanges = false;
            bool combinedRequiresElevation = false;
            foreach (ControlPanelTab tab in GetControlPanelTabs())
            {
                combinedPendingSettingsChanges = combinedPendingSettingsChanges || tab.PendingSettingsChanges;
                combinedRequiresElevation = combinedRequiresElevation || (tab.RequiresElevation && tab.PendingSettingsChanges);
            }

            applyButton.Enabled = combinedPendingSettingsChanges;
            applyButton.Shield = combinedRequiresElevation;
            okButton.Shield = combinedRequiresElevation;

            requiresElevation = combinedRequiresElevation;
        }

        private void tab_PropertiesChanged(object sender, EventArgs e)
        {
            RefreshActionState();
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

            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
