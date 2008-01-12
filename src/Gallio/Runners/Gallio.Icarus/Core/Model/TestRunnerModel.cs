// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Hosting;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Monitors;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Core.Model
{
    public class TestRunnerModel : ITestRunnerModel
    {
        private ReportMonitor reportMonitor = null; 
        private IProjectPresenter projectPresenter = null;
        private IProgressMonitorProvider progressMonitorProvider = null;
        private IProgressMonitor runTestsProgressMonitor = null;
        private TestRunnerMonitor testRunnerMonitor = null;
        private IReportManager reportManager;

        public TestRunnerModel()
        {
            reportManager = Runtime.Instance.Resolve<IReportManager>();
        }

        public IProjectPresenter ProjectPresenter
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                projectPresenter = value;
                progressMonitorProvider = new StatusStripProgressMonitorProvider(projectPresenter);
            }
        }

        public void LoadPackage(TestPackageConfig testPackageConfig)
        {
            // attach report monitor to test runner
            reportMonitor = new ReportMonitor();
            reportMonitor.Attach(projectPresenter.TestRunner);

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                projectPresenter.TestRunner.LoadTestPackage(testPackageConfig, progressMonitor);
            });
        }

        public TestModelData BuildTests()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                projectPresenter.TestRunner.BuildTestModel(progressMonitor);
            });
 
            return projectPresenter.TestRunner.TestModelData;
        }

        public void RunTests()
        {
            testRunnerMonitor = new TestRunnerMonitor(projectPresenter, reportMonitor);
            testRunnerMonitor.Attach(projectPresenter.TestRunner);

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runTestsProgressMonitor = progressMonitor;

                projectPresenter.TestRunner.RunTests(progressMonitor);
            });

            testRunnerMonitor.Detach();
        }

        public void StopTests()
        {
            if (runTestsProgressMonitor != null)
            {
                runTestsProgressMonitor.Cancel();
            }
        }

        public IList<string> GetAvailableLogStreams(string testId)
        {
            if (testRunnerMonitor != null)
                return testRunnerMonitor.GetAvailableLogStreams(testId);
            else
                return new List<string>();
        }

        public string GetLogStream(string logStream, string testId)
        {
            return testRunnerMonitor.GetLogStream(logStream, testId);
        }

        public void GenerateReport()
        {
            string reportName = "";
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                progressMonitor.BeginTask("Generating report.", 100);

                string reportDirectory = GetReportDirectory();
                Report report = reportMonitor.Report;
                IReportContainer reportContainer = new FileSystemReportContainer(reportDirectory, "MbUnit-Report");
                IReportWriter reportWriter = reportManager.CreateReportWriter(report, reportContainer);

                // Delete the report if it exists already.
                reportContainer.DeleteReport();

                // Format the report as html.
                reportManager.Format(reportWriter, "html", new NameValueCollection(), progressMonitor.CreateSubProgressMonitor(90));

                progressMonitor.SetStatus("Displaying report.");
                if (reportWriter.ReportDocumentPaths.Count == 1)
                    reportName = Path.Combine(reportDirectory, reportWriter.ReportDocumentPaths[0]);
            });
            projectPresenter.ReportPath = reportName;
        }

        private static string GetReportDirectory()
        {
            string reportDirectory = System.Configuration.ConfigurationManager.AppSettings["reportDirectory"];

            if (reportDirectory.Length == 0)
            {
                reportDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MbUnit\Reports");
            }

            return reportDirectory;
        }

        public IList<string> GetReportTypes()
        {
            return reportManager.GetFormatterNames();
        }

        public void SaveReportAs(string fileName, string format)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                progressMonitor.BeginTask("Generating report.", 100);

                Report report = reportMonitor.Report;
                IReportContainer reportContainer = new FileSystemReportContainer(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
                IReportWriter reportWriter = reportManager.CreateReportWriter(report, reportContainer);

                // Delete the report if it exists already.
                reportContainer.DeleteReport();

                // Format the report in all of the desired ways.
                reportManager.Format(reportWriter, format, new NameValueCollection(),
                    progressMonitor.CreateSubProgressMonitor(100));

                progressMonitor.SetStatus("Report saved.");
            });
        }
    }
}