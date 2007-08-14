using System;
using System.Collections.Generic;
using MbUnit.Core.Reporting;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner.Monitors
{
    /// <summary>
    /// A test summary monitor tracks <see cref="ITestRunner" /> events and builds
    /// a <see cref="Report" />.
    /// </summary>
    public class ReportMonitor : BaseTestRunnerMonitor
    {
        private Report report;
        private Dictionary<string, TestRunData> testRunDataMap;

        /// <summary>
        /// Creates a test summary tracker initially with no contents.
        /// </summary>
        public ReportMonitor()
        {
            report = new Report();
            testRunDataMap = new Dictionary<string, TestRunData>();
        }

        /// <summary>
        /// Gets the generated report.
        /// </summary>
        /// <remarks>
        /// The report should be locked if it is being accessed while tests are
        /// running to avoid possible collisions among concurrent threads.
        /// </remarks>
        public Report Report
        {
            get { return report; }
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            Runner.LoadPackageComplete += HandleLoadPackageComplete;
            Runner.BuildTemplatesComplete += HandleBuildTemplatesComplete;
            Runner.BuildTestsComplete += HandleBuildTestsComplete;
            Runner.RunStarting += HandleRunStarting;
            Runner.RunComplete += HandleRunComplete;

            Runner.EventDispatcher.TestLifecycle += HandleTestLifecycleEvent;
            Runner.EventDispatcher.TestExecutionLog += HandleTestExecutionLogEvent;
        }

        private void HandleLoadPackageComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                report.Package = Runner.Package;
                report.TemplateModel = null;
                report.TestModel = null;
                report.PackageRun = null;
            }
        }

        private void HandleBuildTemplatesComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                report.TemplateModel = Runner.TemplateModel;
                report.TestModel = null;
                report.PackageRun = null;
            }
        }

        private void HandleBuildTestsComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                report.TestModel = Runner.TestModel;
                report.PackageRun = null;
            }
        }

        private void HandleRunStarting(object sender, EventArgs e)
        {
            lock (report)
            {
                testRunDataMap.Clear();

                report.PackageRun = PackageRun.Create(DateTime.Now);
            }
        }

        private void HandleRunComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                testRunDataMap.Clear();

                report.PackageRun.EndTime = DateTime.Now;
                report.PackageRun.Statistics.Duration = (report.PackageRun.EndTime - report.PackageRun.StartTime).TotalSeconds;
            }
        }

        private void HandleTestLifecycleEvent(object sender, TestLifecycleEventArgs e)
        {
            lock (report)
            {
                TestRunData runData = GetOrCreateTestRunData(e.TestId);

                switch (e.EventType)
                {
                    case TestLifecycleEventType.Start:
                        runData.Run.StartTime = DateTime.Now;
                        break;

                    case TestLifecycleEventType.Step:
                        break;

                    case TestLifecycleEventType.Finish:
                        runData.Run.EndTime = DateTime.Now;
                        runData.Run.Result = e.Result;

                        runData.ExecutionLogWriter.Close(); // just in case
                        break;
                }
            }
        }

        private void HandleTestExecutionLogEvent(object sender, TestExecutionLogEventArgs e)
        {
            lock (report)
            {
                TestRunData runData = GetOrCreateTestRunData(e.TestId);

                switch (e.EventType)
                {
                    case TestExecutionLogEventType.WriteText:
                        runData.ExecutionLogWriter.WriteText(e.StreamName, e.Text);
                        break;

                    case TestExecutionLogEventType.WriteAttachment:
                        runData.ExecutionLogWriter.WriteAttachment(e.StreamName, e.Attachment);
                        break;

                    case TestExecutionLogEventType.BeginSection:
                        runData.ExecutionLogWriter.BeginSection(e.StreamName, e.SectionName);
                        break;

                    case TestExecutionLogEventType.EndSection:
                        runData.ExecutionLogWriter.EndSection(e.StreamName);
                        break;

                    case TestExecutionLogEventType.Close:
                        runData.ExecutionLogWriter.Close();
                        break;
                }
            }
        }

        private TestRunData GetOrCreateTestRunData(string testId)
        {
            TestRunData data;
            if (!testRunDataMap.TryGetValue(testId, out data))
            {
                data = new TestRunData(testId);
                testRunDataMap.Add(testId, data);


            }

            return data;
        }

        private class TestRunData
        {
            public readonly TestRun Run;
            public readonly ExecutionLogWriter ExecutionLogWriter;

            public TestRunData(string testId)
            {
                Run = new TestRun();
                ExecutionLogWriter = new ExecutionLogWriter();

                Run.TestId = testId;
                Run.ExecutionLog = ExecutionLogWriter.ExecutionLog;
            }
        }
    }
}