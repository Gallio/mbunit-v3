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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.Installer;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;
using Gallio.UI.ControlPanel.Preferences;
using Gallio.Runner.Reports.Preferences;

namespace Gallio.UI.Reports.Preferences
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ReportPreferencePane : PreferencePane
    {
        private bool initialized;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReportPreferencePane()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public ReportPreferenceManager PreferenceManager
        {
            get;
            set;
        }

        private void ReportsPreferencePane_Load(object sender, EventArgs e)
        {
            Preset(PreferenceManager.HtmlReportSplitSettings);
            initialized = true;
        }

        private void Preset(HtmlReportSplitSettings settings)
        {
            checkBoxEnabled.Checked = settings.Enabled;
            numericUpDownPageSize.Value = settings.PageSize;
            EnablePageSizeControls(settings.Enabled);
        }

        private void EnablePageSizeControls(bool enabled)
        {
            numericUpDownPageSize.Enabled = enabled;
            labelPageSize.Enabled = enabled;
        }

        private HtmlReportSplitSettings CurrentSettings
        {
            get
            {
                return new HtmlReportSplitSettings 
                {
                    Enabled = checkBoxEnabled.Checked,
                    PageSize = (int)numericUpDownPageSize.Value,
                };
            }
        }

        /// <inheritdoc />
        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            PreferenceManager.HtmlReportSplitSettings = CurrentSettings;
            base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);
        }

        private void checkBoxEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                PendingSettingsChanges = true;
                EnablePageSizeControls(checkBoxEnabled.Checked);
            }
        }

        private void numericUpDownPageSize_ValueChanged(object sender, EventArgs e)
        {
            if (initialized)
                PendingSettingsChanges = true;
        }
    }
}
