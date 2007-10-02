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
using MbUnit.Core.Model.Events;
using MbUnit.Core.Reporting;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Core.Model;

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
    /// For example, to obtain the result associated with a test failure, a test runner
    /// can listen for the <see cref="StepFinished"/> event which will include the test
    /// result as part of its <see cref="ReportStepEventArgs" />.
    /// </para>
    /// </summary>
    public class ReportMonitor : BaseTestRunnerMonitor
    {
        private readonly Report report;
        private readonly Dictionary<string, StepState> stepDataMap;

        private EventHandler<ReportStepEventArgs> stepStarting;
        private EventHandler<ReportStepEventArgs> stepFinished;

        /// <summary>
        /// Creates a test summary tracker initially with no contents.
        /// </summary>
        public ReportMonitor()
        {
            report = new Report();
            stepDataMap = new Dictionary<string, StepState>();
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
                        TestData testData = Runner.TestModel.Tests[e.StepData.TestId];
                        StepRun stepRun = new StepRun(e.StepId, e.StepData.Name, e.StepData.FullName);

                        StepState stepState;
                        if (e.StepData.ParentId == null)
                        {
                            TestRun testRun = new TestRun(e.StepData.TestId, stepRun);
                            report.PackageRun.TestRuns.Add(testRun);

                            stepState = new StepState(testData, testRun, stepRun);
                        }
                        else
                        {
                            StepState parentStepState = GetStepData(e.StepData.ParentId);
                            parentStepState.StepRun.Children.Add(stepRun);

                            stepState = new StepState(testData, parentStepState.TestRun, stepRun);
                        }
                        stepDataMap.Add(e.StepId, stepState);
                        stepRun.StartTime = DateTime.Now;

                        NotifyStepStarting(stepState);
                        break;
                    }

                    case LifecycleEventType.SetPhase:
                        break;

                    case LifecycleEventType.Finish:
                    {
                        StepState stepState = GetStepData(e.StepId);
                        stepState.StepRun.EndTime = DateTime.Now;
                        stepState.StepRun.Result = e.Result;
                        report.PackageRun.Statistics.MergeStepStatistics(stepState.StepRun, stepState.TestData.IsTestCase);

                        stepState.ExecutionLogWriter.Close();

                        NotifyStepFinished(stepState);
                        break;
                    }
                }
            }
        }

        private void HandleExecutionLogEvent(object sender, LogEventArgs e)
        {
            lock (report)
            {
                StepState stepState = GetStepData(e.StepId);

                e.ApplyToLogWriter(stepState.ExecutionLogWriter);
            }
        }

        private StepState GetStepData(string stepId)
        {
            return stepDataMap[stepId];
        }

        private void NotifyStepStarting(StepState stepState)
        {
            if (stepStarting != null)
            {
                try
                {
                    stepStarting(this, new ReportStepEventArgs(report, stepState.TestRun, stepState.StepRun));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Step starting event handler threw an exception: {0}", ex));
                }
            }
        }

        private void NotifyStepFinished(StepState stepState)
        {
            if (stepFinished != null)
            {
                try
                {
                    stepFinished(this, new ReportStepEventArgs(report, stepState.TestRun, stepState.StepRun));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Step finished event handler threw an exception: {0}", ex));
                }
            }
        }

        private sealed class StepState
        {
            public readonly TestData TestData;
            public readonly TestRun TestRun;
            public readonly StepRun StepRun;
            public readonly ExecutionLogWriter ExecutionLogWriter;

            public StepState(TestData testData, TestRun testRun, StepRun stepRun)
            {
                this.TestData = testData;
                this.TestRun = testRun;
                this.StepRun = stepRun;

                ExecutionLogWriter = new ExecutionLogWriter();
                stepRun.ExecutionLog = ExecutionLogWriter.ExecutionLog;
            }
        }
    }
}