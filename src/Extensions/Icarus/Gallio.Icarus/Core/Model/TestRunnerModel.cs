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
using System.Text;
using Gallio.Model.Execution;
using Gallio.Runner.Events;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using Gallio.Icarus.Core.Reports;
using Gallio.Runner;
using Gallio.Model.Filters;
using System.Globalization;
using System.Reflection;
using Gallio.Utilities;

namespace Gallio.Icarus.Core.Model
{
    public class TestRunnerModel : ITestRunnerModel
    {
        private readonly ITestRunner testRunner;
        private readonly IReportManager reportManager;
        private readonly TestExplorationOptions testExplorationOptions = new TestExplorationOptions();
        private readonly TestExecutionOptions testExecutionOptions = new TestExecutionOptions();

        private IProjectPresenter projectPresenter;

        private IProgressMonitorProvider progressMonitorProvider = NullProgressMonitorProvider.Instance;
        private IProgressMonitor runTestsProgressMonitor;

        private string reportNameFormat = "test-report-{0}-{1}";
        private string reportFolder;
        private string executionLogFolder;

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

        public TestRunnerModel(ITestRunner testRunner, IReportManager reportManager)
        {
            this.testRunner = testRunner;
            this.reportManager = reportManager;

            testRunner.Events.TestStepFinished += HandleTestStepFinished;

            // set up reports
            executionLogFolder = Path.Combine(Paths.IcarusAppDataFolder, @"ExecutionLog");
            reportFolder = Path.Combine(Paths.IcarusAppDataFolder, @"Reports");
        }

        public void Initialize()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                TestRunnerOptions options = new TestRunnerOptions();
                ILogger logger = RuntimeAccessor.Logger;
                testRunner.Initialize(options, logger, progressMonitor);
            });
        }

        public void Dispose()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.Dispose(progressMonitor);
            });
        }

        public void Load(TestPackageConfig testPackageConfig)
        {
            Unload();

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.Load(testPackageConfig, progressMonitor);
            });
        }

        public TestModelData Explore()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.Explore(testExplorationOptions, progressMonitor);
            });
            return testRunner.Report.TestModel;
        }

        public void Run()
        {
            SetupExecutionLog();

            // run tests
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runTestsProgressMonitor = progressMonitor;

                testRunner.Run(testExecutionOptions, progressMonitor);
            });
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
                IReportContainer reportContainer = CreateReportContainer(testRunner.Report);
                IReportWriter reportWriter = reportManager.CreateReportWriter(testRunner.Report, reportContainer);

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
            DateTime reportTime = report.TestPackageRun != null ? report.TestPackageRun.StartTime : DateTime.Now;

            return String.Format(CultureInfo.InvariantCulture, reportNameFormat,
                reportTime.ToString(@"yyyyMMdd"),
                reportTime.ToString(@"HHmmss"));
        }

        public Stream GetExecutionLog(string testId, TestModelData testModelData)
        {
            if (testRunner.Report.TestPackageRun != null)
            {
                foreach (TestStepRun testStepRun in testRunner.Report.TestPackageRun.AllTestStepRuns)
                {
                    if (testStepRun.Step.TestId == testId)
                        return TestStepReportWriter.OutputReport(testStepRun, testModelData, executionLogFolder);
                }
            }
            return null;
        }

        private void SetupExecutionLog()
        {
            if (!Directory.Exists(executionLogFolder))
                Directory.CreateDirectory(executionLogFolder);

            // clear old attachments
            DirectoryInfo di = new DirectoryInfo(executionLogFolder);
            foreach (DirectoryInfo attachmentFolder in di.GetDirectories())
                attachmentFolder.Delete(true);

            // output css file
            string cssFile = Path.Combine(executionLogFolder, "ExecutionLog.css");
            if (!File.Exists(cssFile))
            {
                Stream css = Assembly.GetExecutingAssembly().GetManifestResourceStream("Gallio.Icarus.Core.Reports.ExecutionLog.css");
                FileStream fs = File.Open(cssFile, FileMode.Create, FileAccess.Write);
                const int size = 4096;
                byte[] bytes = new byte[size];
                int numBytes;
                while ((numBytes = css.Read(bytes, 0, size)) > 0)
                    fs.Write(bytes, 0, numBytes);
            }
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

                Report report = testRunner.Report;
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
            testExecutionOptions.Filter = filter;
        }

        public void Unload()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.Unload(progressMonitor);
            });
        }

        private void HandleTestStepFinished(object sender, TestStepFinishedEventArgs e)
        {
            projectPresenter.Update(e.Test, e.TestStepRun);

            // store attachments as we go along for the execution log viewer!
            string attachmentDirectory = string.Empty;
            if (e.TestStepRun.ExecutionLog.Attachments.Count > 0)
            {
                attachmentDirectory = Path.Combine(reportFolder, FileUtils.EncodeFileName(e.TestStepRun.Step.Id));
                if (!Directory.Exists(attachmentDirectory))
                    Directory.CreateDirectory(attachmentDirectory);
            }

            foreach (ExecutionLogAttachment ela in e.TestStepRun.ExecutionLog.Attachments)
            {
                string fileName = Path.Combine(attachmentDirectory, FileUtils.EncodeFileName(ela.Name));
                using (FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
                    ela.SaveContents(fs, Encoding.Default);
            }
        }
    }
}
