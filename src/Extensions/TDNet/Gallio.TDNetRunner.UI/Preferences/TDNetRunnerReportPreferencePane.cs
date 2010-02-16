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
using Gallio.TDNetRunner.Core;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.TDNetRunner.UI.Preferences
{
    public partial class TDNetRunnerReportPreferencePane : PreferencePane
    {
        private List<ReportFormatterTraits> types;
        private List<string> typeNames;
        private bool initialized;

        public TDNetRunnerReportPreferencePane()
        {
            InitializeComponent();
        }

        public ITestFrameworkManager TestFrameworkManager
        {
            get;
            set;
        }

        public TDNetPreferenceManager PreferenceManager
        {
            get;
            set;
        }

        public IInstallerManager InstallerManager
        {
            get;
            set;
        }

        private void TDNetRunnerReportsPreferencePane_Load(object sender, EventArgs e)
        {
            InitializeAvailableReportTypes();
            Preset(PreferenceManager.ReportSettings);
            initialized = true;
        }

        private void InitializeAvailableReportTypes()
        {
            IReportManager reportManager = RuntimeAccessor.ServiceLocator.Resolve<IReportManager>();
            types = new List<ReportFormatterTraits>(GenericCollectionUtils.Select(reportManager.FormatterHandles, x => x.GetTraits()));
            typeNames = new List<string>(GenericCollectionUtils.Select(types, x => x.Name));
            comboBoxOutputReportType.Items.AddRange(GenericCollectionUtils.ToArray(types));
        }

        private void Preset(ReportSettings settings)
        {
            var preset = types.Find(x => x.Name.Equals(settings.ReportType, StringComparison.OrdinalIgnoreCase));
            comboBoxOutputReportType.SelectedItem = preset;
            labelDetails.Text = preset.Description;
            checkBoxAutoCondense.Checked = settings.AutoCondenseEnabled;
            groupBoxAutoCondense.Enabled = settings.AutoCondenseEnabled;
            numericUpDownAutoCondenseThreshold.Value = settings.AutoCondenseThreshold;
            UpdateAutoCondenseGroup();
        }

        private ReportSettings CurrentSettings
        {
            get
            {
                var item = (ReportFormatterTraits)comboBoxOutputReportType.SelectedItem;
                return new ReportSettings(item.Name, (int)numericUpDownAutoCondenseThreshold.Value);
            }
        }

        private void UpdateAutoCondenseGroup()
        {
            bool supportsAutoCondense = CurrentSettings.SupportsAutoCondense(typeNames);

            if (groupBoxAutoCondense.Enabled && !supportsAutoCondense)
            {
                groupBoxAutoCondense.Enabled = false;
                checkBoxAutoCondense.Checked = false;
                numericUpDownAutoCondenseThreshold.Value = 0;
            }

            if (!groupBoxAutoCondense.Enabled && supportsAutoCondense)
            {
                groupBoxAutoCondense.Enabled = true;
                checkBoxAutoCondense.Checked = ReportSettings.Default.AutoCondenseEnabled;
                panelAutoCondenseThreshold.Enabled = ReportSettings.Default.AutoCondenseEnabled;
                numericUpDownAutoCondenseThreshold.Value = ReportSettings.Default.AutoCondenseThreshold;
            }
        }

        public override void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            PreferenceManager.ReportSettings = CurrentSettings;
            base.ApplyPendingSettingsChanges(elevationContext, progressMonitor);
        }

        private void comboBoxOutputReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                var selected = (ReportFormatterTraits)comboBoxOutputReportType.SelectedItem;
                labelDetails.Text = selected.Description;
                PendingSettingsChanges = true;
                UpdateAutoCondenseGroup();
            }
        }

        private void checkBoxAutoCondense_CheckedChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                numericUpDownAutoCondenseThreshold.Value = checkBoxAutoCondense.Checked ? ReportSettings.Default.AutoCondenseThreshold : 0;
                panelAutoCondenseThreshold.Enabled = checkBoxAutoCondense.Checked;
                PendingSettingsChanges = true;
            }
        }

        private void numericUpDownAutoCondenseThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                PendingSettingsChanges = true;
            }
        }
    }
}
