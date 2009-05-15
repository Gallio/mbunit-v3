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
using System.Globalization;
using System.IO;
using Gallio.Common.IO;
using Gallio.Common.Policies;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Icarus.Utilities;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Controllers
{
    class ReportController : IReportController
    {
        private readonly IReportService reportService;
        private readonly IFileSystem fileSystem;

        private const string reportNameFormat = "test-report-{0}-{1}";

        public ReportController(IReportService reportService, IFileSystem fileSystem)
        {
            this.reportService = reportService;
            this.fileSystem = fileSystem;
        }

        public IList<string> ReportTypes
        {
            get { return reportService.ReportTypes; }
        }

        public void DeleteReport(string fileName, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Deleting report", 100))
                fileSystem.DeleteFile(fileName);
        }

        public void GenerateReport(Report report, string reportDirectory, IProgressMonitor progressMonitor)
        {
            string fileName = Path.Combine(reportDirectory, GenerateReportName(report));
            reportService.SaveReportAs(report, fileName, "xml", progressMonitor);
        }

        private static string GenerateReportName(Report report)
        {
            DateTime reportTime = report.TestPackageRun != null ? report.TestPackageRun.StartTime : DateTime.Now;

            return String.Format(CultureInfo.InvariantCulture, reportNameFormat,
                reportTime.ToString(@"yyyyMMdd"),
                reportTime.ToString(@"HHmmss"));
        }

        public string ShowReport(Report report, string reportType, IProgressMonitor progressMonitor)
        {
            return reportService.SaveReportAs(report, Path.GetTempFileName(), reportType, progressMonitor);
        }

        public string ConvertSavedReport(string fileName, string format, IProgressMonitor progressMonitor)
        {
            return reportService.ConvertSavedReport(fileName, format, progressMonitor);
        }
    }
}
