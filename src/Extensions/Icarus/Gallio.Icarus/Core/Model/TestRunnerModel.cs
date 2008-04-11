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
using System.Globalization;
using System.Reflection;

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
        private string reportFolder;
        private string executionLogFolder;
        private string reportNameFormat = "test-report-{0}-{1}";

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

        public string ReportFolder
        {
            set { reportFolder = value; }
        }

        public TestRunnerModel()
        {
            // create test runner
#if XDEBUG
            testRunner = new DomainTestRunner(new LocalTestDomainFactory());
#else
            testRunner = RuntimeAccessor.Instance.Resolve<ITestRunnerManager>().CreateTestRunner(StandardTestRunnerFactoryNames.IsolatedProcess, new NameValueCollection());
#endif
            // attach report monitor to test runner
            reportMonitor = new ReportMonitor();
            reportMonitor.Attach(testRunner);

            // get a report manager
            reportManager = RuntimeAccessor.Instance.Resolve<IReportManager>();
            executionLogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Gallio\Icarus\ExecutionLog");
            reportFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Gallio\Icarus\Reports");
            SetupExecutionLog();
        }

        private void SetupExecutionLog()
        {
            if (!Directory.Exists(executionLogFolder))
                Directory.CreateDirectory(executionLogFolder);

            // output css file
            Stream css = Assembly.GetExecutingAssembly().GetManifestResourceStream("Gallio.Icarus.Core.Reports.ExecutionLog.css");
            FileStream fs = File.Open(Path.Combine(executionLogFolder, "ExecutionLog.css"), FileMode.Create, FileAccess.Write);
            const int size = 4096;
            byte[] bytes = new byte[size];
            int numBytes;
            while ((numBytes = css.Read(bytes, 0, size)) > 0)
                fs.Write(bytes, 0, numBytes);
        }

        public void LoadTestPackage(TestPackageConfig testPackageConfig)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.LoadTestPackage(testPackageConfig, progressMonitor);
            });
        }

        public TestModelData BuildTestModel()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.BuildTestModel(progressMonitor);
            });
            return testRunner.TestModelData;
        }

        public void RunTests()
        {
            // tidy up last run
            reportMonitor.ResetReport();
            // clear old attachments
            DirectoryInfo di = new DirectoryInfo(executionLogFolder);
            foreach (DirectoryInfo attachmentFolder in di.GetDirectories())
                attachmentFolder.Delete(true);

            // attach test runner monitor
            testRunnerMonitor = new TestRunnerMonitor(projectPresenter, reportMonitor, executionLogFolder);
            testRunnerMonitor.Attach(testRunner);

            // run tests
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runTestsProgressMonitor = progressMonitor;
                testRunner.RunTests(progressMonitor);
            });

            // detach test runner monitor
            testRunnerMonitor.Detach();
        }

        public void StopTests()
        {
            if (runTestsProgressMonitor != null)
                runTestsProgressMonitor.Cancel();
        }

        public string GenerateReport()
        {
            string reportPath = string.Empty;
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                IReportContainer reportContainer = CreateReportContainer(reportMonitor.Report);
                IReportWriter reportWriter = reportManager.CreateReportWriter(reportMonitor.Report, reportContainer);

                // format the report as xml
                reportManager.Format(reportWriter, "html", new NameValueCollection(), progressMonitor);
                reportPath = Path.Combine(reportFolder, reportWriter.ReportDocumentPaths[0]);
            });
            return reportPath;
        }

        private IReportContainer CreateReportContainer(Report report)
        {
            string reportName = GenerateReportName(report);
            return new FileSystemReportContainer(reportFolder, reportName);
        }

        private string GenerateReportName(Report report)
        {
            DateTime reportTime = report.PackageRun != null ? report.PackageRun.StartTime : DateTime.Now;

            return String.Format(CultureInfo.InvariantCulture, reportNameFormat,
                reportTime.ToString(@"yyyyMMdd"),
                reportTime.ToString(@"HHmmss"));
        }

        public Stream GetExecutionLog(string testId, TestModelData testModelData)
        {
            if (reportMonitor.Report.PackageRun != null)
            {
                foreach (TestStepRun testStepRun in reportMonitor.Report.PackageRun.AllTestStepRuns)
                {
                    if (testStepRun.Step.TestId == testId)
                        return TestStepReportWriter.OutputReport(testStepRun, testModelData, executionLogFolder);
                }
            }
            return null;
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
