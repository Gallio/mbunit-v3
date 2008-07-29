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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.ProgressMonitoring;
using Gallio.Icarus.Core.Reports;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Icarus.Core.Model
{
    public class TestRunnerModel : ITestRunnerModel
    {
        private readonly ITestRunner testRunner;
        private readonly IReportManager reportManager;
        private readonly TestExplorationOptions testExplorationOptions = new TestExplorationOptions();
        private readonly TestExecutionOptions testExecutionOptions = new TestExecutionOptions();

        private StatusStripProgressMonitorProvider progressMonitorProvider = new StatusStripProgressMonitorProvider();

        private string reportNameFormat = "test-report-{0}-{1}";
        private string reportFolder;
        private string executionLogFolder;
        private Report previousReportFromUnloadedPackage;

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        public event EventHandler<TestStepFinishedEventArgs> TestStepFinished;

        public string ReportFolder
        {
            set { reportFolder = value; }
        }

        public Report Report
        {
            get
            {
                if (previousReportFromUnloadedPackage != null)
                    return previousReportFromUnloadedPackage;
                return testRunner.Report;
            }
        }

        public TestRunnerModel(ITestRunner testRunner, IReportManager reportManager)
        {
            this.testRunner = testRunner;
            this.testRunner.Events.TestStepFinished += HandleTestStepFinished;
            this.reportManager = reportManager;

            // hook up status strip
            ((StatusStripProgressMonitorProvider)progressMonitorProvider).ProgressUpdate += delegate(object sender, ProgressUpdateEventArgs e) {
                if (ProgressUpdate != null) ProgressUpdate(this, e); };

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
            previousReportFromUnloadedPackage = null;

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

            return Report.TestModel;
        }

        public void Run()
        {
            SetupExecutionLog();

            // run tests
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.Run(testExecutionOptions, progressMonitor);
            });
        }

        public void CancelOperation()
        {
            if (progressMonitorProvider.ProgressMonitor != null)
                progressMonitorProvider.ProgressMonitor.Cancel();
        }

        public string GenerateReport()
        {
            string reportPath = string.Empty;
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                IReportContainer reportContainer = CreateReportContainer(Report);
                IReportWriter reportWriter = reportManager.CreateReportWriter(Report, reportContainer);

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

        public Stream GetExecutionLog(IList<string> testIds, TestModelData testModelData)
        {
            if (Report.TestPackageRun != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                TestStepReportWriter.RenderReportHeader(xmlTextWriter, executionLogFolder);
                foreach (TestStepRun testStepRun in Report.TestPackageRun.AllTestStepRuns)
                {
                    if (testIds.Contains(testStepRun.Step.TestId))
                        TestStepReportWriter.RenderTestStepRun(xmlTextWriter, testStepRun, testModelData);
                }
                memoryStream.Position = 0;
                return memoryStream;
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
                using (progressMonitor.BeginTask("Generating report.", 100))
                {
                    IReportContainer reportContainer = new FileSystemReportContainer(Path.GetDirectoryName(fileName),
                        Path.GetFileNameWithoutExtension(fileName));
                    IReportWriter reportWriter = reportManager.CreateReportWriter(Report, reportContainer);

                    // Delete the report if it exists already.
                    reportContainer.DeleteReport();

                    // Format the report in all of the desired ways.
                    using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(100))
                        reportManager.Format(reportWriter, format, new NameValueCollection(), subProgressMonitor);

                    progressMonitor.SetStatus("Report saved.");
                }
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
            previousReportFromUnloadedPackage = testRunner.Report;

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                testRunner.Unload(progressMonitor);
            });
        }

        private void HandleTestStepFinished(object sender, TestStepFinishedEventArgs e)
        {
            // bubble event up to presenter
            if (TestStepFinished != null)
                TestStepFinished(this, e);

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
