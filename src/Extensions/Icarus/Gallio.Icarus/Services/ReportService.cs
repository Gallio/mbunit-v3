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
using System.IO;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Runner.Reports;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Services
{
    internal class ReportService : IReportService
    {
        private readonly IReportManager reportManager;

        public ReportService(IReportManager reportManager)
        {
            this.reportManager = reportManager;
        }

        public IList<string> ReportTypes
        {
            get
            {
                return GenericCollectionUtils.ConvertAllToArray(reportManager.FormatterHandles, 
                    h => h.GetTraits().Name);
            }
        }

        public string SaveReportAs(Report report, string fileName, string format, IProgressMonitor progressMonitor)
        {
            var file = string.Empty;
            using (progressMonitor.BeginTask("Generating report", 100))
            {
                var folderName = Path.GetDirectoryName(fileName);
                var reportContainer = new FileSystemReportContainer(folderName, 
                    Path.GetFileNameWithoutExtension(fileName));
                var reportWriter = reportManager.CreateReportWriter(report, reportContainer);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                // Delete the report if it already exists
                reportContainer.DeleteReport();

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                progressMonitor.Worked(10);

                // Format the report
                var reportFormatterOptions = new ReportFormatterOptions();
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(90))
                    reportManager.Format(reportWriter, format, reportFormatterOptions, subProgressMonitor);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                if (reportWriter.ReportDocumentPaths.Count > 0)
                    file = Path.Combine(folderName, reportWriter.ReportDocumentPaths[0]);
            }
            return file;
        }

        public string ConvertSavedReport(string fileName, string format, 
            IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Converting saved report", 100))
            {
                var reportContainer = new FileSystemReportContainer(Path.GetDirectoryName(fileName), 
                    Path.GetFileNameWithoutExtension(fileName));

                var reportReader = reportManager.CreateReportReader(reportContainer);

                Report report;
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(20))
                    report = reportReader.LoadReport(true, subProgressMonitor);

                var tempFilePath = SpecialPathPolicy.For<ReportService>().CreateTempDirectoryWithUniqueName().FullName;
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(80))
                    return SaveReportAs(report, tempFilePath, format, subProgressMonitor);
            }
        }
    }
}