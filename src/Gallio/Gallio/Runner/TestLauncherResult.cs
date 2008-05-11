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
using Gallio.Runner.Reports;

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
        /// <param name="report">The test report</param>
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
        /// The report might be incomplete in varying degrees depending on how much
        /// of the test execution was successfully performed.
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
        /// <returns>The full paths of the formatted report documents</returns>
        public IList<string> ReportDocumentPaths
        {
            get { return reportDocumentPaths.ToArray(); }
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
        /// Adds a new report document path.
        /// </summary>
        /// <param name="reportDocumentPath">The report document path</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportDocumentPath"/> is null</exception>
        public void AddReportDocumentPath(string reportDocumentPath)
        {
            if (reportDocumentPath == null)
                throw new ArgumentNullException(@"reportDocumentPath");

            reportDocumentPaths.Add(reportDocumentPath);
        }
    }
}
