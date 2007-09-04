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
using System.Diagnostics;
using MbUnit.Core.Reporting;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Runner.Monitors
{
    /// <summary>
    /// <para>
    /// A report monitor tracks <see cref="ITestRunner" /> events and builds
    /// a <see cref="Report" />.
    /// </para>
    /// <para>
    /// The report monitor also provides reinterpreted events regarding the lifecycle of
    /// tests in terms of report elements that have been generated.
    /// For example, to obtain the stack traces associated with a test failure, a test runner
    /// can listen for <
    /// 
    /// examine
    /// the contents of the <see cref="ExecutionLogStreamName.Failures" /> execution
    /// log stream.  Likewise, console output can be derived in this manner.
    /// </para>
    /// </summary>
    public class ReportMonitor : BaseTestRunnerMonitor
    {
        private readonly Report report;
        private readonly Dictionary<string, StepData> stepDataMap;

        private EventHandler<ReportStepEventArgs> stepStarting;
        private EventHandler<ReportStepEventArgs> stepFinished;

        /// <summary>
        /// Creates a test summary tracker initially with no contents.
        /// </summary>
        public ReportMonitor()
        {
            report = new Report();
            stepDataMap = new Dictionary<string, StepData>();
        }

        /// <summary>
        /// Gets the generated report.
        /// </summary>
        /// <remarks>
        /// The report instance should be locked if it is being accessed while tests are
        /// running to avoid possible collisions among concurrent threads.
        /// </remarks>
        public Report Report
        {
            get { return report; }
        }

        /// <summary>
        /// The event fired when a step is starting.
        /// </summary>
        /// <remarks>
        /// The <see cref="Report" /> instance is locked for the duration of this
        /// event to prevent race conditions.  Do not perform long-running operations
        /// when processing this event.  Also beware of potential deadlocks!
        /// </remarks>
        public event EventHandler<ReportStepEventArgs> StepStarting
        {
            add { lock (report) stepStarting += value; }
            remove { lock (report) stepStarting -= value; }
        }

        /// <summary>
        /// The event fired when a step is finished.
        /// </summary>
        /// <remarks>
        /// The <see cref="Report" /> instance is locked for the duration of this
        /// event to prevent race conditions.  Do not perform long-running operations
        /// when processing this event.  Also beware of potential deadlocks!
        /// </remarks>
        public event EventHandler<ReportStepEventArgs> StepFinished
        {
            add { lock (report) stepFinished += value; }
            remove { lock (report) stepFinished -= value; }
        }

        /// <inheritdoc />
        protected override void OnAttach()
        {
            base.OnAttach();

            Runner.LoadPackageComplete += HandleLoadPackageComplete;
            Runner.BuildTemplatesComplete += HandleBuildTemplatesComplete;
            Runner.BuildTestsComplete += HandleBuildTestsComplete;
            Runner.RunStarting += HandleRunStarting;
            Runner.RunComplete += HandleRunComplete;
            Runner.EventDispatcher.Lifecycle += HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog += HandleExecutionLogEvent;
        }

        /// <inheritdoc />
        protected override void OnDetach()
        {
            base.OnDetach();

            Runner.LoadPackageComplete -= HandleLoadPackageComplete;
            Runner.BuildTemplatesComplete -= HandleBuildTemplatesComplete;
            Runner.BuildTestsComplete -= HandleBuildTestsComplete;
            Runner.RunStarting -= HandleRunStarting;
            Runner.RunComplete -= HandleRunComplete;
            Runner.EventDispatcher.Lifecycle -= HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog -= HandleExecutionLogEvent;
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
                stepDataMap.Clear();

                report.PackageRun = new PackageRun();
                report.PackageRun.StartTime = DateTime.Now;
            }
        }

        private void HandleRunComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                stepDataMap.Clear();

                report.PackageRun.EndTime = DateTime.Now;
                report.PackageRun.Statistics.Duration = (report.PackageRun.EndTime - report.PackageRun.StartTime).TotalSeconds;
            }
        }

        private void HandleLifecycleEvent(object sender, LifecycleEventArgs e)
        {
            lock (report)
            {
                switch (e.EventType)
                {
                    case LifecycleEventType.Start:
                    {
                        TestInfo testInfo = Runner.TestModel.Tests[e.StepInfo.TestId];
                        StepRun stepRun = new StepRun(e.StepId, e.StepInfo.Name, e.StepInfo.FullName);

                        StepData stepData;
                        if (e.StepInfo.ParentId == null)
                        {
                            TestRun testRun = new TestRun(e.StepInfo.TestId, stepRun);
                            report.PackageRun.TestRuns.Add(testRun);

                            stepData = new StepData(testInfo, testRun, stepRun);
                        }
                        else
                        {
                            StepData parentStepData = GetStepData(e.StepInfo.ParentId);
                            parentStepData.StepRun.Children.Add(stepRun);

                            stepData = new StepData(testInfo, parentStepData.TestRun, stepRun);
                        }
                        stepDataMap.Add(e.StepId, stepData);
                        stepRun.StartTime = DateTime.Now;

                        NotifyStepStarting(stepData);
                        break;
                    }

                    case LifecycleEventType.EnterPhase:
                        break;

                    case LifecycleEventType.Finish:
                    {
                        StepData stepData = GetStepData(e.StepId);
                        stepData.StepRun.EndTime = DateTime.Now;
                        stepData.StepRun.Result = e.Result;
                        report.PackageRun.Statistics.MergeStepStatistics(stepData.StepRun, stepData.TestInfo.IsTestCase);

                        stepData.ExecutionLogWriter.Close(); // just in case

                        NotifyStepFinished(stepData);
                        break;
                    }
                }
            }
        }

        private void HandleExecutionLogEvent(object sender, ExecutionLogEventArgs e)
        {
            lock (report)
            {
                StepData stepData = GetStepData(e.StepId);

                switch (e.EventType)
                {
                    case ExecutionLogEventType.WriteText:
                        stepData.ExecutionLogWriter.WriteText(e.StreamName, e.Text);
                        break;

                    case ExecutionLogEventType.WriteAttachment:
                        stepData.ExecutionLogWriter.WriteAttachment(e.StreamName, e.Attachment);
                        break;

                    case ExecutionLogEventType.BeginSection:
                        stepData.ExecutionLogWriter.BeginSection(e.StreamName, e.SectionName);
                        break;

                    case ExecutionLogEventType.EndSection:
                        stepData.ExecutionLogWriter.EndSection(e.StreamName);
                        break;

                    case ExecutionLogEventType.Close:
                        stepData.ExecutionLogWriter.Close();
                        break;
                }
            }
        }

        private StepData GetStepData(string stepId)
        {
            return stepDataMap[stepId];
        }

        private void NotifyStepStarting(StepData stepData)
        {
            if (stepStarting != null)
            {
                try
                {
                    stepStarting(this, new ReportStepEventArgs(report, stepData.TestRun, stepData.StepRun));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Step starting event handler threw an exception: {0}", ex));
                }
            }
        }

        private void NotifyStepFinished(StepData stepData)
        {
            if (stepFinished != null)
            {
                try
                {
                    stepFinished(this, new ReportStepEventArgs(report, stepData.TestRun, stepData.StepRun));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Step finished event handler threw an exception: {0}", ex));
                }
            }
        }

        private sealed class StepData
        {
            public readonly TestInfo TestInfo;
            public readonly TestRun TestRun;
            public readonly StepRun StepRun;
            public readonly ExecutionLogWriter ExecutionLogWriter;

            public StepData(TestInfo testInfo, TestRun testRun, StepRun stepRun)
            {
                TestInfo = testInfo;
                this.TestRun = testRun;
                this.StepRun = stepRun;

                ExecutionLogWriter = new ExecutionLogWriter();
                stepRun.ExecutionLog = ExecutionLogWriter.ExecutionLog;
            }
        }
    }
}