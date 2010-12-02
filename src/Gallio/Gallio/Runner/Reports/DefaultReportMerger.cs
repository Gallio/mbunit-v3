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
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using Gallio.Common.Collections;
using Gallio.Common.Markup;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Schema;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Default implementation of a report merger.
    /// </summary>
    public class DefaultReportMerger : IReportMerger
    {
        private readonly IList<Report> reports;

        /// <summary>
        /// Creates a report merger.
        /// </summary>
        /// <param name="reports">The reports to be merged.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reports"/> is null.</exception>
        public DefaultReportMerger(IEnumerable<Report> reports)
        {
            if (reports == null)
                throw new ArgumentNullException("reports");

            this.reports = new List<Report>(reports);
        }

        /// <inheritdoc />
        public Report Merge(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor.BeginTask("Merging reports.", 1))
            {
                var report = new Report 
                {
                    TestModel = MergeTestModelData(),
                    TestPackageRun = MergeTestPackageRun(),
                    TestPackage = MergeTestPackageData(),
                };

                report.LogEntries.AddRange(MergeLogEntries());
                return report;
            }
        }

        private TestModelData MergeTestModelData()
        {
            var merged = new TestModelData();

            foreach (Report report in reports)
                merged.MergeWith(report.TestModel);

            return merged;
        }

        private TestPackageRun MergeTestPackageRun()
        {
            var merged = new TestPackageRun();

            foreach (Report report in reports)
                merged.MergeWith(report.TestPackageRun);

            return merged;
        }

        private TestPackageData MergeTestPackageData()
        {
            var merged = new TestPackage();
            
            foreach (Report report in reports)
                merged.MergeWith(report.TestPackage.ToTestPackage());

            return new TestPackageData(merged);
        }

        private IEnumerable<LogEntry> MergeLogEntries()
        {
            foreach (Report report in reports)
            foreach (LogEntry logEntry in report.LogEntries)
                yield return logEntry;
        }
    }
}
