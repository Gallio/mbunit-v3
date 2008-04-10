// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Monitors;
using Gallio.Runner.Reports;
using Gallio.Icarus.Core.Reports;
using Gallio.Runner;
using Gallio.Runner.Domains;
using Gallio.Model.Filters;

namespace Gallio.Icarus.Core.Model
{
    public class TestRunnerModel : ITestRunnerModel
    {
        private readonly ITestRunner testRunner;
        private ReportMonitor reportMonitor = null;
        private IProjectPresenter projectPresenter = null;
        private IProgressMonitorProvider progressMonitorProvider = null;
        private IProgressMonitor runTestsProgressMonitor = null;
        private TestRunnerMonitor testRunnerMonitor = null;
        private IReportManager reportManager;

        public TestRunnerModel()
        {
#if XDEBUG
            testRunner = new DomainTestRunner(new LocalTestDomainFactory());
#else
            testRunner = RuntimeAccessor.Instance.Resolve<ITestRunnerManager>().CreateTestRunner(StandardTestRunnerFactoryNames.IsolatedProcess, new NameValueCollection());
#endif
            reportManager = RuntimeAccessor.Instance.Resolve<IReportManager>();
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

        public TestModelData LoadTestPackage(TestPackageConfig testPackageConfig)
        {
            // attach report monitor to test runner
            reportMonitor = new ReportMonitor();
            reportMonitor.Attach(testRunner);

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.LoadTestPackage(testPackageConfig, progressMonitor);
            });

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.BuildTestModel(progressMonitor);
            });

            return testRunner.TestModelData;
        }

        public void RunTests()
        {
            testRunnerMonitor = new TestRunnerMonitor(projectPresenter, reportMonitor);
            testRunnerMonitor.Attach(testRunner);

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runTestsProgressMonitor = progressMonitor;
                testRunner.RunTests(progressMonitor);
            });

            testRunnerMonitor.Detach();
        }

        public void StopTests()
        {
            if (runTestsProgressMonitor != null)
                runTestsProgressMonitor.Cancel();
        }

        public void GenerateReport()
        {
            string reportName = "";
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                progressMonitor.BeginTask("Generating report.", 100);

                string reportDirectory = Path.GetTempPath();
                IReportContainer reportContainer = new FileSystemReportContainer(reportDirectory, "MbUnit-Report");
                IReportWriter reportWriter = reportManager.CreateReportWriter(reportMonitor.Report, reportContainer);

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

        public string GetExecutionLog(string testId)
        {
            if (reportMonitor.Report.PackageRun != null)
            {
                foreach (TestStepRun testStepRun in reportMonitor.Report.PackageRun.AllTestStepRuns)
                {
                    if (testStepRun.Step.TestId == testId)
                    {
                        string reportDirectory = Path.Combine(Path.GetTempPath(), @"Gallio\Icarus\ExecutionLog");
                        IReportContainer reportContainer = new FileSystemReportContainer(reportDirectory, "ExecutionLog");
                        reportContainer.DeleteReport();
                        IReportWriter reportWriter = new TestStepReportWriter(reportMonitor.Report, testStepRun, reportContainer);
                        reportManager.Format(reportWriter, "ExecutionLog", new NameValueCollection(), NullProgressMonitor.CreateInstance());
                        return Path.Combine(reportDirectory, reportWriter.ReportDocumentPaths[0]);
                    }
                }
            }
            return "about:blank";
        }

        public IList<string> GetReportTypes()
        {
            return reportManager.FormatterResolver.GetNames();
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

        public IList<string> GetTestFrameworks()
        {
            List<string> frameworks = new List<string>();
            foreach (ITestFramework framework in RuntimeAccessor.Instance.ResolveAll<ITestFramework>())
                frameworks.Add(framework.Name);
            return frameworks;
        }

        public void SetFilter(Filter<ITest> filter)
        {
            testRunner.TestExecutionOptions.Filter = filter;
        }

        public void UnloadTestPackage()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.UnloadTestPackage(progressMonitor);
            });
        }
    }
}