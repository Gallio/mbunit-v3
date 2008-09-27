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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Runner.Reports;
using Gallio.Utilities;

namespace Gallio.Icarus.Controllers
{
    class ReportController : IReportController
    {
        readonly IReportService reportService;
        const string reportNameFormat = "test-report-{0}-{1}";
        readonly TaskManager taskManager = new TaskManager();

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        public ReportController(IReportService reportService)
        {
            this.reportService = reportService;

            // bubble progress up
            reportService.ProgressUpdate += delegate(object sender, ProgressUpdateEventArgs e)
            {
                EventHandlerUtils.SafeInvoke(ProgressUpdate, this, e);
            };
        }

        public IList<string> ReportTypes
        {
            get { return reportService.ReportTypes; }
        }

        public void GenerateReport(Report report, string reportDirectory)
        {
            taskManager.StartTask(delegate
            {
                string fileName = Path.Combine(reportDirectory, GenerateReportName(report));
                reportService.SaveReportAs(report, fileName, "xml");
            });
        }

        private static string GenerateReportName(Report report)
        {
            DateTime reportTime = report.TestPackageRun != null ? report.TestPackageRun.StartTime : DateTime.Now;

            return String.Format(CultureInfo.InvariantCulture, reportNameFormat,
                reportTime.ToString(@"yyyyMMdd"),
                reportTime.ToString(@"HHmmss"));
        }

        public void ShowReport(Report report, string reportType)
        {
            taskManager.StartTask(delegate
            {
                string fileName = reportService.SaveReportAs(report, Path.GetTempFileName(), reportType);
                if (!string.IsNullOrEmpty(fileName))
                    Process.Start(fileName);
            });
        }
    }
}
