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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Icarus.Services
{
    class ReportService : IReportService
    {
        readonly IReportManager reportManager;
        const string reportNameFormat = "test-report-{0}-{1}";
        readonly ProgressMonitorProvider progressMonitorProvider = new ProgressMonitorProvider();

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        public IList<string> ReportTypes
        {
            get { return reportManager.FormatterResolver.GetNames(); }
        }

        public ReportService(IReportManager reportManager)
        {
            this.reportManager = reportManager;

            // hook up progress monitor
            progressMonitorProvider.ProgressUpdate += delegate(object sender, ProgressUpdateEventArgs e)
            {
                EventHandlerUtils.SafeInvoke(ProgressUpdate, this, e);
            };
        }

        public void GenerateReport(Report report, string reportFolder)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                IReportContainer reportContainer = CreateReportContainer(report, reportFolder);
                IReportWriter reportWriter = reportManager.CreateReportWriter(report, reportContainer);

                // format the report as xml
                reportManager.Format(reportWriter, "xml", new NameValueCollection(), progressMonitor);
            });
        }

        private static IReportContainer CreateReportContainer(Report report, string reportFolder)
        {
            string reportName = GenerateReportName(report);
            return new FileSystemReportContainer(reportFolder, reportName);
        }

        private static string GenerateReportName(Report report)
        {
            DateTime reportTime = report.TestPackageRun != null ? report.TestPackageRun.StartTime : DateTime.Now;

            return String.Format(CultureInfo.InvariantCulture, reportNameFormat,
                reportTime.ToString(@"yyyyMMdd"),
                reportTime.ToString(@"HHmmss"));
        }

        public void SaveReportAs(Report report, string fileName, string format)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                using (progressMonitor.BeginTask("Generating report.", 100))
                {
                    IReportContainer reportContainer = new FileSystemReportContainer(Path.GetDirectoryName(fileName),
                        Path.GetFileNameWithoutExtension(fileName));
                    IReportWriter reportWriter = reportManager.CreateReportWriter(report, reportContainer);

                    // Delete the report if it exists already.
                    reportContainer.DeleteReport();

                    // Format the report in all of the desired ways.
                    using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(100))
                        reportManager.Format(reportWriter, format, new NameValueCollection(), subProgressMonitor);

                    progressMonitor.SetStatus("Report saved.");
                }
            });
        }
    }
}
