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
using System.Collections.Generic;
using System.Diagnostics;
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using Gallio.Utilities;

namespace Gallio.Runner.Monitors
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
    /// can listen for the <see cref="TestStepFinished"/> event which will include the test
    /// result as part of its <see cref="TestStepRunEventArgs" />.
    /// </para>
    /// </summary>
    public class ReportMonitor : BaseTestRunnerMonitor
    {
        private readonly Dictionary<string, TestStepState> states;
        private readonly Dictionary<string, TestInstanceData> testInstances;
        private readonly Dictionary<string, TestInstanceRun> testInstanceRuns;

        private Report report;
        private Stopwatch durationStopwatch;

        private EventHandler<TestStepRunEventArgs> testStepStarting;
        private EventHandler<TestStepRunEventArgs> testStepLifecyclePhaseChanged;
        private EventHandler<TestStepRunEventArgs> testStepFinished;

        /// <summary>
        /// Creates a test summary tracker initially with no contents.
        /// </summary>
        public ReportMonitor()
        {
            states = new Dictionary<string, TestStepState>();
            testInstances = new Dictionary<string, TestInstanceData>();
            testInstanceRuns = new Dictionary<string, TestInstanceRun>();
            report = new Report();
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
        /// The event fired when a test step is starting.
        /// </summary>
        /// <remarks>
        /// The <see cref="Report" /> instance is locked for the duration of this
        /// event to prevent race conditions.  Do not perform long-running operations
        /// when processing this event.  Also beware of potential deadlocks!
        /// </remarks>
        public event EventHandler<TestStepRunEventArgs> TestStepStarting
        {
            add { lock (report) testStepStarting += value; }
            remove { lock (report) testStepStarting -= value; }
        }

        /// <summary>
        /// The event fired when a test step transitions between phases.
        /// </summary>
        /// <remarks>
        /// The <see cref="Report" /> instance is locked for the duration of this
        /// event to prevent race conditions.  Do not perform long-running operations
        /// when processing this event.  Also beware of potential deadlocks!
        /// </remarks>
        public event EventHandler<TestStepRunEventArgs> TestStepLifecyclePhaseChanged
        {
            add { lock (report) testStepLifecyclePhaseChanged += value; }
            remove { lock (report) testStepLifecyclePhaseChanged -= value; }
        }

        /// <summary>
        /// The event fired when a test step is finished.
        /// </summary>
        /// <remarks>
        /// The <see cref="Report" /> instance is locked for the duration of this
        /// event to prevent race conditions.  Do not perform long-running operations
        /// when processing this event.  Also beware of potential deadlocks!
        /// </remarks>
        public event EventHandler<TestStepRunEventArgs> TestStepFinished
        {
            add { lock (report) testStepFinished += value; }
            remove { lock (report) testStepFinished -= value; }
        }

        /// <summary>
        /// Sets <see cref="Report" /> to a fresh instance so that the monitor can be reused
        /// as part of a new test run while preserving the report that was previously created.
        /// </summary>
        /// <remarks>
        /// The current event handlers are not affected.
        /// </remarks>
        public void ResetReport()
        {
            lock (report)
            {
                report = new Report();
            }
        }

        /// <inheritdoc />
        protected override void OnAttach()
        {
            base.OnAttach();

            Runner.TestPackageChanged += HandleTestPackageChanged;
            Runner.BuildTestModelComplete += HandleBuildTestModelComplete;
            Runner.RunTestsStarting += HandleRunTestsStarting;
            Runner.RunTestsComplete += HandleRunTestsComplete;
            Runner.EventDispatcher.Lifecycle += HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog += HandleExecutionLogEvent;
        }

        /// <inheritdoc />
        protected override void OnDetach()
        {
            base.OnDetach();

            Runner.TestPackageChanged -= HandleTestPackageChanged;
            Runner.BuildTestModelComplete -= HandleBuildTestModelComplete;
            Runner.RunTestsStarting -= HandleRunTestsStarting;
            Runner.RunTestsComplete -= HandleRunTestsComplete;
            Runner.EventDispatcher.Lifecycle -= HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog -= HandleExecutionLogEvent;
        }

        private void HandleTestPackageChanged(object sender, EventArgs e)
        {
            lock (report)
            {
                TestPackageData data = Runner.TestPackageData;
                report.PackageConfig = data != null ? data.Config.Copy() : null;
                report.TestModelData = null;
                report.PackageRun = null;
            }
        }

        private void HandleBuildTestModelComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                report.TestModelData = Runner.TestModelData;
                report.PackageRun = null;
            }
        }

        private void HandleRunTestsStarting(object sender, EventArgs e)
        {
            lock (report)
            {
                Reset();

                report.PackageRun = new PackageRun();
                report.PackageRun.StartTime = DateTime.Now;
                durationStopwatch = Stopwatch.StartNew();
            }
        }

        private void HandleRunTestsComplete(object sender, EventArgs e)
        {
            lock (report)
            {
                Reset();

                report.PackageRun.EndTime = DateTime.Now;
                report.PackageRun.Statistics.Duration = durationStopwatch.Elapsed.TotalSeconds;
            }
        }

        private void Reset()
        {
            states.Clear();
            testInstances.Clear();
            testInstanceRuns.Clear();
        }

        private void HandleLifecycleEvent(object sender, LifecycleEventArgs e)
        {
            lock (report)
            {
                switch (e.EventType)
                {
                    case LifecycleEventType.NewInstance:
                    {
                        TestInstanceData testInstanceData = e.TestInstanceData;
                        if (testInstanceData.ParentId != null)
                            EnsureTestInstanceIdIsValid(testInstanceData.ParentId);
                        EnsureTestIdIsValid(testInstanceData.TestId);

                        testInstances.Add(testInstanceData.Id, testInstanceData);
                        break;
                    }

                    case LifecycleEventType.Start:
                    {
                        TestInstanceData testInstanceData = GetTestInstanceData(e.TestStepData.TestInstanceId);
                        TestData testData = GetTestData(testInstanceData.TestId);

                        TestStepRun testStepRun = new TestStepRun(e.TestStepData);

                        TestStepState state;
                        if (e.TestStepData.ParentId == null)
                        {
                            TestInstanceRun testInstanceRun = new TestInstanceRun(testInstanceData, testStepRun);
                            testInstanceRuns.Add(testInstanceData.Id, testInstanceRun);

                            if (testInstanceData.ParentId != null)
                            {
                                TestInstanceRun parentTestInstanceRun = testInstanceRuns[testInstanceData.ParentId];
                                parentTestInstanceRun.Children.Add(testInstanceRun);
                            }
                            else
                            {
                                report.PackageRun.RootTestInstanceRun = testInstanceRun;
                            }

                            state = new TestStepState(testData, testInstanceRun, testStepRun);
                        }
                        else
                        {
                            TestStepState parentState = GetTestStepState(e.TestStepData.ParentId);
                            parentState.TestStepRun.Children.Add(testStepRun);

                            state = new TestStepState(testData, parentState.TestInstanceRun, testStepRun);
                        }
                        states.Add(e.StepId, state);
                        testStepRun.StartTime = DateTime.Now;

                        NotifyStepStarting(state);
                        break;
                    }

                    case LifecycleEventType.SetPhase:
                    {
                        TestStepState state = GetTestStepState(e.StepId);
                        NotifyStepLifecyclePhaseChanged(state, e.PhaseName);
                        break;
                    }

                    case LifecycleEventType.AddMetadata:
                    {
                        TestStepState state = GetTestStepState(e.StepId);
                        state.TestStepRun.Step.Metadata.Add(e.MetadataKey, e.MetadataValue);
                        break;
                    }

                    case LifecycleEventType.Finish:
                    {
                        TestStepState state = GetTestStepState(e.StepId);
                        state.TestStepRun.EndTime = DateTime.Now;
                        state.TestStepRun.Result = e.Result;
                        report.PackageRun.Statistics.MergeStepStatistics(state.TestStepRun,
                            state.TestStepRun.Step.ParentId == null && state.TestData.IsTestCase);

                        state.ExecutionLogWriter.Close();

                        NotifyStepFinished(state);
                        break;
                    }
                }
            }
        }

        private void HandleExecutionLogEvent(object sender, LogEventArgs e)
        {
            lock (report)
            {
                TestStepState state = GetTestStepState(e.StepId);

                e.ApplyToLogWriter(state.ExecutionLogWriter);
            }
        }

        private void EnsureTestIdIsValid(string testId)
        {
            GetTestData(testId);
        }

        private void EnsureTestInstanceIdIsValid(string testInstanceId)
        {
            GetTestInstanceData(testInstanceId);
        }

        private TestData GetTestData(string testId)
        {
            TestData testData;
            if (!Runner.TestModelData.Tests.TryGetValue(testId, out testData))
                throw new InvalidOperationException("The test id was not recognized.  It may belong to an earlier test run that has since completed.");
            return testData;
        }

        private TestInstanceData GetTestInstanceData(string testInstanceId)
        {
            TestInstanceData testInstanceData;
            if (!testInstances.TryGetValue(testInstanceId, out testInstanceData))
                throw new InvalidOperationException("The test instance id was not recognized.  It may belong to an earlier test run that has since completed.");
            return testInstanceData;
        }

        private TestStepState GetTestStepState(string testStepId)
        {
            TestStepState testStepData;
            if (!states.TryGetValue(testStepId, out testStepData))
                throw new InvalidOperationException("The test step id was not recognized.  It may belong to an earlier test run that has since completed.");
            return testStepData;
        }

        private void NotifyStepStarting(TestStepState state)
        {
            EventHandlerUtils.SafeInvoke(testStepStarting, this, new TestStepRunEventArgs(report, state.TestData, state.TestInstanceRun, state.TestStepRun, ""));
        }

        private void NotifyStepLifecyclePhaseChanged(TestStepState state, string lifecyclePhase)
        {
            EventHandlerUtils.SafeInvoke(testStepLifecyclePhaseChanged, this, new TestStepRunEventArgs(report, state.TestData, state.TestInstanceRun, state.TestStepRun, lifecyclePhase));
        }

        private void NotifyStepFinished(TestStepState state)
        {
            EventHandlerUtils.SafeInvoke(testStepFinished, this, new TestStepRunEventArgs(report, state.TestData, state.TestInstanceRun, state.TestStepRun, ""));
        }

        private sealed class TestStepState
        {
            public readonly TestData TestData;
            public readonly TestInstanceRun TestInstanceRun;
            public readonly TestStepRun TestStepRun;
            public readonly ExecutionLogWriter ExecutionLogWriter;

            public TestStepState(TestData testData, TestInstanceRun testInstanceRun, TestStepRun testStepRun)
            {
                TestData = testData;
                TestInstanceRun = testInstanceRun;
                TestStepRun = testStepRun;

                ExecutionLogWriter = new ExecutionLogWriter();
                testStepRun.ExecutionLog = ExecutionLogWriter.ExecutionLog;
            }
        }
    }
}