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
using System.Collections.Specialized;
using System.IO;
using Gallio.Runner.Reports;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner
{
    /// <summary>
    /// Contains the result of a complete test run.
    /// </summary>
    [Serializable]
    public class TestLauncherResult
    {
        private readonly Report report;
        private readonly List<string> reportDocumentPaths;

        private int resultCode = Runner.ResultCode.Success;

        /// <summary>
        /// Creates a launcher result object.
        /// </summary>
        /// <param name="report">The test report.</param>
        public TestLauncherResult(Report report)
        {
            this.report = report;

            reportDocumentPaths = new List<string>();
        }

        /// <summary>
        /// Gets the result code.
        /// </summary>
        public int ResultCode
        {
            get { return resultCode; }
        }

        /// <summary>
        /// Gets the test report.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The report might be incomplete in varying degrees depending on how much
        /// of the test execution was successfully performed.
        /// </para>
        /// </remarks>
        public Report Report
        {
            get { return report; }
        }

        /// <summary>
        /// Gets a short summary with the number of test that passed, failed and so on.
        /// </summary>
        public string ResultSummary
        {
            get { return Statistics.FormatTestCaseResultSummary(); }
        }

        /// <summary>
        /// Gets the statistics of the execution from the report
        /// or an empty statistics object if none.
        /// </summary>
        public Statistics Statistics
        {
            get
            {
                return report != null && report.TestPackageRun != null ? report.TestPackageRun.Statistics : new Statistics();
            }
        }

        /// <summary>
        /// Gets the full paths of the formatted report documents.
        /// </summary>
        /// <returns>The full paths of the formatted report documents.</returns>
        public IList<string> ReportDocumentPaths
        {
            get { return reportDocumentPaths.ToArray(); }
        }

        /// <summary>
        /// Sets the result code of the result.
        /// </summary>
        /// <param name="value">The new result code.</param>
        public void SetResultCode(int value)
        {
            resultCode = value;
        }

        /// <summary>
        /// Adds a new report document path.
        /// </summary>
        /// <param name="reportDocumentPath">The report document path.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportDocumentPath"/> is null.</exception>
        public void AddReportDocumentPath(string reportDocumentPath)
        {
            if (reportDocumentPath == null)
                throw new ArgumentNullException(@"reportDocumentPath");

            reportDocumentPaths.Add(reportDocumentPath);
        }

        /// <summary>
        /// Generates reports of the desired forms.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method adds the paths of the generated report documents to <see cref="ReportDocumentPaths" />.
        /// </para>
        /// </remarks>
        /// <param name="reportDirectory">The report directory.</param>
        /// <param name="reportName">The report name.</param>
        /// <param name="reportArchive">Determines whether to enclose the resulting test report in a compressed archive file.</param>
        /// <param name="reportFormats">The report formats to generate.</param>
        /// <param name="reportFormatOptions">The report formatter options.</param>
        /// <param name="reportManager">The report manager.</param>
        /// <param name="progressMonitor">A progress monitor for the operation.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportDirectory"/>,
        /// <paramref name="reportName"/>, <paramref name="reportFormats"/>, <paramref name="reportFormatOptions"/>,
        /// <paramref name="reportManager"/>, or <paramref name="progressMonitor"/> is null.</exception>
        public void GenerateReports(string reportDirectory, string reportName, ReportArchive reportArchive, IList<string> reportFormats,
            ReportFormatterOptions reportFormatOptions, IReportManager reportManager, IProgressMonitor progressMonitor)
        {
            if (reportDirectory == null)
                throw new ArgumentNullException("reportDirectory");
            if (reportName == null)
                throw new ArgumentNullException("reportName");
            if (reportFormats == null)
                throw new ArgumentNullException("reportFormats");
            if (reportFormatOptions == null)
                throw new ArgumentNullException("reportFormatOptions");
            if (reportManager == null)
                throw new ArgumentNullException("reportManager");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            var reportContainerFactory = new ReportContainerFactory(reportDirectory, reportName);

            using (progressMonitor.BeginTask("Generating reports.", reportFormats.Count))
            using (IReportContainer reportContainer = reportContainerFactory.MakeForSaving(reportArchive))
            {
                IReportWriter reportWriter = reportManager.CreateReportWriter(report, reportContainer);

                // Delete the report if it exists already.
                reportContainer.DeleteReport();

                // Format the report in all of the desired ways.
                foreach (string reportFormat in reportFormats)
                {
                    using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                        reportManager.Format(reportWriter, reportFormat, reportFormatOptions, subProgressMonitor);
                }

                // Save the full paths of the documents.
                foreach (string reportDocumentPath in reportWriter.ReportDocumentPaths)
                    AddReportDocumentPath(Path.Combine(reportDirectory, reportDocumentPath));
            }
        }

        /// <summary>
        /// Shows the report documents enumerated in the launcher result.
        /// </summary>
        /// <returns>True if the report documents were successfully opened.</returns>
        public bool ShowReportDocuments()
        {
            bool success = true;
            foreach (string reportDocumentPath in reportDocumentPaths)
            {
                if (!TestRunnerUtils.ShowReportDocument(reportDocumentPath))
                    success = false;
            }

            return success;
        }
    }
}
