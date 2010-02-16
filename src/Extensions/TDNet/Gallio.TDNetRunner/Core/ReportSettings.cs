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
using System.Text;
using Gallio.Common.Collections;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.Preferences;

namespace Gallio.TDNetRunner.Core
{
    /// <summary>
    /// Immutable settings for the TestDriven.Net test report output.
    /// </summary>
    public sealed class ReportSettings
    {
        private const string CondensedSuffix = "-Condensed";
        private readonly string reportType;
        private readonly int autoCondenseThreshold;

        /// <summary>
        /// Gets the report type.
        /// </summary>
        public string ReportType
        {
            get
            {
                return reportType;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the report auto-condensing is enabled.
        /// </summary>
        public bool AutoCondenseEnabled
        {
            get
            {
                return autoCondenseThreshold > 0;
            }
        }

        /// <summary>
        /// Gets the threshold for report auto-condensing.
        /// </summary>
        public int AutoCondenseThreshold
        {
            get
            {
                return autoCondenseThreshold;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current report type supports auto-condensing.
        /// </summary>
        public bool SupportsAutoCondense(List<string> allReportTypes)
        {
            if (allReportTypes == null)
                throw new ArgumentNullException("allReportTypes");

            string condensing = reportType + CondensedSuffix;
            return !reportType.EndsWith(CondensedSuffix, StringComparison.OrdinalIgnoreCase)
                && allReportTypes.Find(x => x.Equals(condensing, StringComparison.OrdinalIgnoreCase)) != null;
        }

        /// <summary>
        /// Default settings.
        /// </summary>
        public readonly static ReportSettings Default = new ReportSettings("Html", 100);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reportType">The report type.</param>
        /// <param name="autoCondenseThreshold">The threshold for report auto-condensing, or zero.</param>
        public ReportSettings(string reportType, int autoCondenseThreshold)
        {
            if (reportType == null)
                throw new ArgumentNullException("reportType");
            if (autoCondenseThreshold < 0)
                throw new ArgumentOutOfRangeException("autoCondenseThreshold", "Must be greater than or equal to zero.");

            this.reportType = reportType;
            this.autoCondenseThreshold = autoCondenseThreshold;
        }

        /// <summary>
        /// Reads the report settings from the specified preference set.
        /// </summary>
        /// <param name="preferenceSet">The preference set which contains the data to read.</param>
        /// <returns>The resulting report settings.</returns>
        public static ReportSettings ReadFrom(IPreferenceSet preferenceSet)
        {
            if (preferenceSet == null)
                throw new ArgumentNullException("preferenceSet");

            return new ReportSettings(
                preferenceSet.Read(reader => reader.GetSetting(new Key<string>("ReportSettingsReportType"), Default.ReportType)),
                preferenceSet.Read(reader => reader.GetSetting(new Key<int>("ReportSettingsAutoCondenseThreadhold"), Default.AutoCondenseThreshold)));
        }

        /// <summary>
        /// Writes the content of the report settings to the specified preference set.
        /// </summary>
        /// <param name="preferenceSet">The preference set to write data in.</param>
        public void WriteTo(IPreferenceSet preferenceSet)
        {
            if (preferenceSet == null)
                throw new ArgumentNullException("preferenceSet");

            preferenceSet.Write(writer => writer.SetSetting(new Key<string>("ReportSettingsReportType"), reportType));
            preferenceSet.Write(writer => writer.SetSetting(new Key<int>("ReportSettingsAutoCondenseThreadhold"), autoCondenseThreshold));
        }

        /// <summary>
        /// Returns the effective report format.
        /// </summary>
        /// <param name="report">The report which contains the test results.</param>
        /// <returns>The effective format for that report.</returns>
        public string DetermineReportFormat(Report report)
        {
            return (report.TestPackageRun != null && AutoCondenseEnabled && report.TestPackageRun.Statistics.RunCount >= AutoCondenseThreshold)
                ? reportType + CondensedSuffix
                : reportType;
        }
    }
}
