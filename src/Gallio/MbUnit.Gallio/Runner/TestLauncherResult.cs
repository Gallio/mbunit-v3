// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using MbUnit.Runner.Reports;

namespace MbUnit.Runner
{
    /// <summary>
    /// Contains the result of a complete test run.
    /// </summary>
    public class TestLauncherResult
    {
        private readonly Report report;
        private readonly Dictionary<string, ReportContext> reportContexts;

        private int resultCode = Runner.ResultCode.Success;

        /// <summary>
        /// Creates a launcher result object.
        /// </summary>
        /// <param name="report">The test report</param>
        public TestLauncherResult(Report report)
        {
            this.report = report;

            reportContexts = new Dictionary<string, ReportContext>();
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
        /// The report might be incomplete in varying degrees depending on how much
        /// of the test execution was successfully performed.
        /// </remarks>
        public Report Report
        {
            get { return report; }
        }

        /// <summary>
        /// A short summary with the number of test that passed, failed and so on.
        /// </summary>
        public string ResultSummary
        {
            get
            {
                return Statistics.FormatTestCaseResultSummary();
            }
        }

        /// <summary>
        /// The statistics of the execution.
        /// </summary>
        public PackageRunStatistics Statistics
        {
            get
            {
                return report != null && report.PackageRun != null ? report.PackageRun.Statistics : new PackageRunStatistics();
            }
        }

        /// <summary>
        /// Gets the context of the generated report for the given report format.
        /// </summary>
        /// <param name="reportFormat">The report format name</param>
        /// <returns>The context of the generated report, or <c>null</c> if no report
        /// was generated for the given report format</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportFormat"/> is null</exception>
        public ReportContext GetReportContext(string reportFormat)
        {
            if (reportFormat == null)
                throw new ArgumentNullException(@"reportFormat");

            ReportContext reportContext;
            reportContexts.TryGetValue(CanonicalizeReportFormat(reportFormat), out reportContext);
            return reportContext;
        }

        /// <summary>
        /// Sets the result code of the result.
        /// </summary>
        /// <param name="value">The new result code</param>
        public void SetResultCode(int value)
        {
            resultCode = value;
        }

        /// <summary>
        /// Adds a new report context.
        /// </summary>
        /// <param name="reportFormat">The report format</param>
        /// <param name="reportContext">The report context</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportFormat"/> or <
        /// <paramref name="reportContext"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already a context
        /// registered for the specified <paramref name="reportFormat"/></exception>
        public void AddReportContext(string reportFormat, ReportContext reportContext)
        {
            if (reportFormat == null)
                throw new ArgumentNullException(@"reportFormat");
            if (reportContext == null)
                throw new ArgumentNullException(@"reportContext");

            reportContexts.Add(CanonicalizeReportFormat(reportFormat), reportContext);
        }

        private static string CanonicalizeReportFormat(string reportFormat)
        {
            return reportFormat.ToLower(CultureInfo.InvariantCulture);
        }
    }
}
