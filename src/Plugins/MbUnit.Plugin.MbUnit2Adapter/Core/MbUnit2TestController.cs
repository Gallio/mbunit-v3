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

extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Results;
using MbUnit.Framework.Services.ExecutionLogs;
using MbUnit2::MbUnit.Core;
using MbUnit2::MbUnit.Core.Remoting;
using MbUnit2::MbUnit.Core.Filters;
using MbUnit2::MbUnit.Core.Reports.Serialization;

using TestState = MbUnit.Framework.Kernel.Results.TestState;

namespace MbUnit.Plugin.MbUnit2Adapter.Core
{
    /// <summary>
    /// Controls the execution of MbUnit v2 tests.
    /// </summary>
    public class MbUnit2TestController : ITestController
    {
        private FixtureExplorer fixtureExplorer;

        private InstrumentedFixtureRunner fixtureRunner;
        private bool aborted;

        /// <summary>
        /// Creates a runner.
        /// </summary>
        /// <param name="fixtureExplorer">The fixture explorer</param>
        public MbUnit2TestController(FixtureExplorer fixtureExplorer)
        {
            this.fixtureExplorer = fixtureExplorer;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            lock (this)
            {
                if (fixtureRunner != null)
                    throw new InvalidOperationException("Dispose cannot be called while tests are still running.");

                fixtureExplorer = null;
            }
        }

        /// <inheritdoc />
        public void Run(TestExecutionOptions options, IEventListener listener, IList<ITest> tests)
        {
            try
            {
                lock (this)
                {
                    ThrowIfDisposed();

                    if (aborted)
                    {
                        // TODO: Should we mark tests aborted here?
                        return;
                    }

                    fixtureRunner = new InstrumentedFixtureRunner(this, options, listener, tests);
                    fixtureRunner.Run();
                }
            }
            finally
            {
                lock (this)
                {
                    fixtureRunner = null;
                }
            }
        }

        /// <inheritdoc />
        public void Abort()
        {
            lock (this)
            {
                ThrowIfDisposed();

                aborted = true;

                if (fixtureRunner != null)
                    fixtureRunner.Abort();
            }
        }

        private void CheckAbort()
        {
            // Note: This check resolves a possible race condition while aborting tests
            //       that can happen because the fixture runner resets the AbortPending
            //       flag when its Run method is invoked.  It is possible that this
            //       will prevent the abort from succeeding if it happens too early.
            lock (this)
            {
                if (aborted && fixtureRunner != null)
                    fixtureRunner.Abort();
            }
        }

        private void ThrowIfDisposed()
        {
            if (fixtureExplorer == null)
                throw new ObjectDisposedException("The test controller has been disposed.");
        }

        private class InstrumentedFixtureRunner : DependencyFixtureRunner, IFixtureFilter, IRunPipeFilter, IRunPipeListener
        {
            private MbUnit2TestController controller;
            private TestExecutionOptions options;
            private IEventListener listener;
            private IList<ITest> tests;

            private Dictionary<Type, bool> includedFixtureTypes;
            private Dictionary<Fixture, MbUnit2Test> fixtureTestsByFixture;
            private Dictionary<RunPipe, MbUnit2Test> testsByRunPipe;
            private MbUnit2Test assemblyTest;

            private Stopwatch assemblyStopwatch;
            private Stopwatch fixtureStopwatch;

            private Stack<ITest> testStack;

            public InstrumentedFixtureRunner(MbUnit2TestController controller,
                TestExecutionOptions options, IEventListener listener, IList<ITest> tests)
            {
                this.controller = controller;
                this.options = options;
                this.listener = listener;
                this.tests = tests;

                Initialize();
            }

            private void Initialize()
            {
                // Build a reverse mapping from types and run-pipes to tests.
                includedFixtureTypes = new Dictionary<Type, bool>();
                fixtureTestsByFixture = new Dictionary<Fixture, MbUnit2Test>();
                testsByRunPipe = new Dictionary<RunPipe, MbUnit2Test>();

                foreach (MbUnit2Test test in tests)
                {
                    Fixture fixture = test.Fixture;
                    RunPipe runPipe = test.RunPipe;

                    if (fixture == null)
                    {
                        assemblyTest = test;
                    }
                    else if (runPipe == null)
                    {
                        includedFixtureTypes[fixture.Type] = true;
                        fixtureTestsByFixture[fixture] = test;
                    }
                    else
                    {
                        testsByRunPipe[runPipe] = test;
                    }
                }

                testStack = new Stack<ITest>();

                // Set options
                IsExplicit = options.IsExplicit;
                FixtureFilter = this;
                RunPipeFilter = this;
            }

            public void Run()
            {
                ReportListener reportListener = new ReportListener();
                Run(controller.fixtureExplorer, reportListener);

                // TODO: Do we need to do anyhing with the result in the report listener?
            }

            #region Overrides to track assembly and fixture lifecycle
            protected override bool RunAssemblySetUp()
            {
                // Note: This fixes a race condition involving Abort
                controller.CheckAbort();

                HandleAssemblyStart();

                if (Explorer.HasAssemblySetUp)
                    listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateStepEvent(assemblyTest.Id, TestStep.SetUp));

                bool success = base.RunAssemblySetUp();

                // Note: MbUnit won't call RunAssemblyTearDown itself if the assembly setup fails
                //       so we need to make sure we finish things up ourselves.
                if (!success)
                    HandleAssemblyFinish(TestState.Failed);

                return success;
            }

            protected override bool RunAssemblyTearDown()
            {
                if (Explorer.HasAssemblyTearDown && assemblyTest != null)
                    listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateStepEvent(assemblyTest.Id, TestStep.TearDown));

                bool success = base.RunAssemblyTearDown();
                HandleAssemblyFinish(success ? TestState.Completed : TestState.Failed);

                return success;
            }

            protected override ReportRunResult RunFixture(Fixture fixture)
            {
                try
                {
                    HandleFixtureStart(fixture);

                    MbUnit2Test fixtureTest;
                    fixtureTestsByFixture.TryGetValue(fixture, out fixtureTest);

                    foreach (RunPipeStarter starter in fixture.Starters)
                        starter.Listeners.Add(this);

                    ReportRunResult reportRunResult = base.RunFixture(fixture);
                    HandleFixtureFinish(fixture, reportRunResult);

                    return reportRunResult;
                }
                finally
                {
                    foreach (RunPipeStarter starter in fixture.Starters)
                        starter.Listeners.Remove(this);
                }
            }

            protected override void SkipStarters(Fixture fixture, Exception ex)
            {
                HandleFixtureStart(fixture);

                base.SkipStarters(fixture, ex);

                HandleFixtureFinish(fixture, ReportRunResult.Skip);
            }

            protected override object RunFixtureSetUp(Fixture fixture, object fixtureInstance)
            {
                MbUnit2Test fixtureTest;
                if (fixture.HasSetUp && fixtureTestsByFixture.TryGetValue(fixture, out fixtureTest))
                {
                    listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateStepEvent(fixtureTest.Id, TestStep.SetUp));
                }

                return base.RunFixtureSetUp(fixture, fixtureInstance);
            }

            protected override void RunFixtureTearDown(Fixture fixture, object fixtureInstance)
            {
                MbUnit2Test fixtureTest;
                if (fixture.HasSetUp && fixtureTestsByFixture.TryGetValue(fixture, out fixtureTest))
                {
                    listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateStepEvent(fixtureTest.Id, TestStep.TearDown));
                }

                base.RunFixtureTearDown(fixture, fixtureInstance);
            }
            #endregion

            #region IFixtureFilter
            bool IFixtureFilter.Filter(Type type)
            {
                return includedFixtureTypes.ContainsKey(type);
            }
            #endregion

            #region IRunPipeFilter
            bool IRunPipeFilter.Filter(RunPipe runPipe)
            {
                return testsByRunPipe.ContainsKey(runPipe);
            }
            #endregion

            #region IRunPipeListener
            void IRunPipeListener.Start(RunPipe pipe)
            {
                HandleTestStart(pipe);
            }

            void IRunPipeListener.Success(RunPipe pipe, ReportRun result)
            {
                HandleTestFinish(pipe, result);
            }

            void IRunPipeListener.Failure(RunPipe pipe, ReportRun result)
            {
                HandleTestFinish(pipe, result);
            }

            void IRunPipeListener.Ignore(RunPipe pipe, ReportRun result)
            {
                HandleTestFinish(pipe, result);
            }

            void IRunPipeListener.Skip(RunPipe pipe, ReportRun result)
            {
                HandleTestFinish(pipe, result);
            }
            #endregion

            private void HandleAssemblyStart()
            {
                if (assemblyTest == null)
                    return;

                listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateStartEvent(assemblyTest.Id));
                testStack.Push(assemblyTest);

                assemblyStopwatch = Stopwatch.StartNew();
            }

            private void HandleAssemblyFinish(TestState state)
            {
                if (assemblyTest == null)
                    return;

                TestResult result = new TestResult();
                result.Duration = assemblyStopwatch.Elapsed;
                result.State = state;
                result.Outcome = state == TestState.Completed ? TestOutcome.Passed : TestOutcome.Failed;

                testStack.Pop();
                listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateFinishEvent(assemblyTest.Id, result));
            }

            private void HandleFixtureStart(Fixture fixture)
            {
                MbUnit2Test fixtureTest;
                if (!fixtureTestsByFixture.TryGetValue(fixture, out fixtureTest))
                    return;

                listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateStartEvent(fixtureTest.Id));
                testStack.Push(fixtureTest);

                fixtureStopwatch = Stopwatch.StartNew();
            }

            private void HandleFixtureFinish(Fixture fixture, ReportRunResult reportRunResult)
            {
                MbUnit2Test fixtureTest;
                if (!fixtureTestsByFixture.TryGetValue(fixture, out fixtureTest))
                    return;

                TestResult result = new TestResult();
                result.Duration = fixtureStopwatch.Elapsed;
                SetTestResultStateAndOutcomeFromReportRunResult(result, reportRunResult);

                testStack.Pop();
                listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateFinishEvent(fixtureTest.Id, result));
            }

            private void HandleTestStart(RunPipe runPipe)
            {
                MbUnit2Test test;
                if (!testsByRunPipe.TryGetValue(runPipe, out test))
                    return;

                listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateStartEvent(test.Id));
                testStack.Push(test);
            }

            private void HandleTestFinish(RunPipe runPipe, ReportRun reportRun)
            {
                MbUnit2Test test;
                if (!testsByRunPipe.TryGetValue(runPipe, out test))
                    return;

                // Produce the final result.
                TestResult result = new TestResult();
                result.Duration = TimeSpan.FromMilliseconds(reportRun.Duration);
                SetTestResultStateAndOutcomeFromReportRunResult(result, reportRun.Result);

                // Output all execution log contents.
                // Note: ReportRun.Asserts is not actually populated by MbUnit so we ignore it.
                if (reportRun.ConsoleOut.Length != 0)
                {
                    listener.NotifyTestExecutionLogEvent(
                        TestExecutionLogEventArgs.CreateWriteTextEvent(test.Id,
                        ExecutionLogStreams.ConsoleOutput, reportRun.ConsoleOut));
                }
                if (reportRun.ConsoleError.Length != 0)
                {
                    listener.NotifyTestExecutionLogEvent(
                        TestExecutionLogEventArgs.CreateWriteTextEvent(test.Id,
                        ExecutionLogStreams.ConsoleError, reportRun.ConsoleError));
                }
                foreach (ReportWarning warning in reportRun.Warnings)
                {
                    listener.NotifyTestExecutionLogEvent(
                        TestExecutionLogEventArgs.CreateWriteTextEvent(test.Id,
                        ExecutionLogStreams.Warnings, warning.Text + "\n"));
                }
                if (reportRun.Exception != null)
                {
                    listener.NotifyTestExecutionLogEvent(
                        TestExecutionLogEventArgs.CreateWriteTextEvent(test.Id,
                        ExecutionLogStreams.Failures, FormatReportException(reportRun.Exception)));
                }

                listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateCloseEvent(test.Id));

                // Finish up...
                testStack.Pop();
                listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateFinishEvent(test.Id, result));
            }

            /// <summary>
            /// Formats an MbUnit v2 report exception in the same manner as the system's
            /// Exception.ToString() method. 
            /// </summary>
            /// <param name="ex">The exception to format</param>
            /// <returns>The formatted result</returns>
            private static string FormatReportException(ReportException ex)
            {
                StringBuilder result = new StringBuilder();

                result.Append(ex.Type);

                if (ex.Message.Length != 0)
                    result.Append(": ").Append(ex.Message);

                if (ex.Exception != null)
                {
                    result.Append(" ---> ")
                        .Append(FormatReportException(ex.Exception))
                        .Append(Environment.NewLine)
                        .Append("   --- End of inner exception stack trace ---"); // TODO: Localize me!
                }

                if (ex.StackTrace.Length != 0)
                {
                    result.Append(Environment.NewLine).Append(ex.StackTrace);
                }

                return result.ToString();
            }

            private static void SetTestResultStateAndOutcomeFromReportRunResult(TestResult result, ReportRunResult reportRunResult)
            {
                switch (reportRunResult)
                {
                    case ReportRunResult.NotRun:
                        result.State = TestState.NotRun;
                        result.Outcome = TestOutcome.Inconclusive;
                        break;

                    case ReportRunResult.Skip:
                        result.State = TestState.Skipped;
                        result.Outcome = TestOutcome.Inconclusive;
                        break;

                    case ReportRunResult.Ignore:
                        result.State = TestState.Ignored;
                        result.Outcome = TestOutcome.Inconclusive;
                        break;

                    case ReportRunResult.Success:
                        result.State = TestState.Completed;
                        result.Outcome = TestOutcome.Passed;
                        break;

                    case ReportRunResult.Failure:
                        result.State = TestState.Failed;
                        result.Outcome = TestOutcome.Failed;
                        break;
                }
            }
        }
    }
}
