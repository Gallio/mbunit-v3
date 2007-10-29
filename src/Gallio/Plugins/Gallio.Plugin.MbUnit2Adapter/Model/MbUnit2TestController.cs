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
using System.Text;
using Gallio.Plugin.MbUnit2Adapter.Properties;
using Gallio.Core.ProgressMonitoring;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Model.Execution;
using MbUnit2::MbUnit.Core;
using MbUnit2::MbUnit.Core.Remoting;
using MbUnit2::MbUnit.Core.Filters;
using MbUnit2::MbUnit.Core.Reports.Serialization;

namespace Gallio.Plugin.MbUnit2Adapter.Model
{
    /// <summary>
    /// Controls the execution of MbUnit v2 tests.
    /// </summary>
    public class MbUnit2TestController : ITestController
    {
        private FixtureExplorer fixtureExplorer;

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
            fixtureExplorer = null;
        }

        /// <inheritdoc />
        public void RunTests(IProgressMonitor progressMonitor, ITestMonitor rootTestMonitor)
        {
            ThrowIfDisposed();

            using (progressMonitor)
            {
                progressMonitor.BeginTask(Resources.MbUnit2TestController_RunningMbUnitTests, 1);

                if (progressMonitor.IsCanceled)
                    return;

                IList<ITestMonitor> testMonitors = rootTestMonitor.GetAllMonitors();

                using (InstrumentedFixtureRunner fixtureRunner = new InstrumentedFixtureRunner(fixtureExplorer,
                    testMonitors, progressMonitor))
                {
                    fixtureRunner.Run();
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (fixtureExplorer == null)
                throw new ObjectDisposedException(Resources.MbUnit2TestController_ControlledWasDisposedException);
        }

        private class InstrumentedFixtureRunner : DependencyFixtureRunner, IDisposable, IFixtureFilter, IRunPipeFilter, IRunPipeListener
        {
            private readonly FixtureExplorer fixtureExplorer;
            private readonly IList<ITestMonitor> testMonitors;
            private readonly IProgressMonitor progressMonitor;

            private Dictionary<Type, bool> includedFixtureTypes;

            private ITestMonitor assemblyTestMonitor;
            private Dictionary<Fixture, ITestMonitor> fixtureTestMonitors;
            private Dictionary<RunPipe, ITestMonitor> runPipeTestMonitors;

            private Dictionary<ITestMonitor, IStepMonitor> activeStepMonitors;

            private double workUnit;

            public InstrumentedFixtureRunner(FixtureExplorer fixtureExplorer,
                IList<ITestMonitor> testMonitors, IProgressMonitor progressMonitor)
            {
                this.fixtureExplorer = fixtureExplorer;
                this.progressMonitor = progressMonitor;
                this.testMonitors = testMonitors;

                Initialize();
            }

            public void Dispose()
            {
                progressMonitor.Canceled -= HandleCanceled;
            }

            private void Initialize()
            {
                progressMonitor.Canceled += HandleCanceled;
                progressMonitor.SetStatus(Resources.MbUnit2TestController_InitializingMbUnitTestRunner);

                int totalWork = 1;
                if (fixtureExplorer.HasAssemblySetUp)
                    totalWork += 1;
                if (fixtureExplorer.HasAssemblyTearDown)
                    totalWork += 1;

                // Build a reverse mapping from types and run-pipes to tests.
                includedFixtureTypes = new Dictionary<Type, bool>();
                fixtureTestMonitors = new Dictionary<Fixture, ITestMonitor>();
                runPipeTestMonitors = new Dictionary<RunPipe, ITestMonitor>();
                activeStepMonitors = new Dictionary<ITestMonitor, IStepMonitor>();

                bool isExplicit = false;
                foreach (ITestMonitor testMonitor in testMonitors)
                {
                    if (testMonitor.IsExplicit)
                        isExplicit = true;

                    MbUnit2Test test = (MbUnit2Test)testMonitor.Test;
                    Fixture fixture = test.Fixture;
                    RunPipe runPipe = test.RunPipe;

                    if (fixture == null)
                    {
                        assemblyTestMonitor = testMonitor;
                    }
                    else if (runPipe == null)
                    {
                        includedFixtureTypes[fixture.Type] = true;
                        fixtureTestMonitors[fixture] = testMonitor;

                        if (fixture.HasSetUp)
                            totalWork += 1;
                        if (fixture.HasTearDown)
                            totalWork += 1;
                    }
                    else
                    {
                        runPipeTestMonitors[runPipe] = testMonitor;
                        totalWork += 1;
                    }
                }

                // Set options
                IsExplicit = isExplicit;
                FixtureFilter = this;
                RunPipeFilter = this;

                workUnit = 1.0 / totalWork;
                progressMonitor.Worked(workUnit);
            }

            public void Run()
            {
                ReportListener reportListener = new ReportListener();
                Run(fixtureExplorer, reportListener);

                // TODO: Do we need to do anyhing with the result in the report listener?
            }

            #region Overrides to track assembly and fixture lifecycle
            protected override bool RunAssemblySetUp()
            {
                CheckCanceled();

                progressMonitor.SetStatus(String.Format(Resources.MbUnit2TestController_StatusMessage_RunningAssemblySetUp, Explorer.AssemblyName));

                HandleAssemblyStart();

                if (Explorer.HasAssemblySetUp)
                {
                    IStepMonitor assemblyStepMonitor = activeStepMonitors[assemblyTestMonitor];
                    assemblyStepMonitor.LifecyclePhase = LifecyclePhases.SetUp;
                }

                bool success = base.RunAssemblySetUp();

                // Note: MbUnit won't call RunAssemblyTearDown itself if the assembly setup fails
                //       so we need to make sure we finish things up ourselves.
                if (!success)
                    HandleAssemblyFinish(TestOutcome.Failed);

                progressMonitor.Worked(workUnit);
                return success;
            }

            protected override bool RunAssemblyTearDown()
            {
                progressMonitor.SetStatus(String.Format(Resources.MbUnit2TestController_StatusMessage_RunningAssemblyTearDown, Explorer.AssemblyName));

                if (Explorer.HasAssemblyTearDown && assemblyTestMonitor != null)
                {
                    IStepMonitor assemblyStepMonitor = activeStepMonitors[assemblyTestMonitor];
                    assemblyStepMonitor.LifecyclePhase = LifecyclePhases.TearDown;
                }

                bool success = base.RunAssemblyTearDown();
                HandleAssemblyFinish(success ? TestOutcome.Passed : TestOutcome.Failed);

                progressMonitor.Worked(workUnit);
                return success;
            }

            protected override ReportRunResult RunFixture(Fixture fixture)
            {
                CheckCanceled();

                try
                {
                    HandleFixtureStart(fixture);

                    foreach (RunPipeStarter starter in fixture.Starters)
                        starter.Listeners.Add(this);

                    ReportRunResult reportRunResult = base.RunFixture(fixture);
                    HandleFixtureFinish(fixture, reportRunResult);

                    return reportRunResult;
                }
                finally
                {
                    foreach (RunPipeStarter starter in fixture.Starters)
                    {
                        if (starter.Listeners.Contains(this))
                            starter.Listeners.Remove(this);
                    }
                }
            }

            protected override void SkipStarters(Fixture fixture, Exception ex)
            {
                CheckCanceled();

                HandleFixtureStart(fixture);

                base.SkipStarters(fixture, ex);

                HandleFixtureFinish(fixture, ReportRunResult.Skip);
            }

            protected override object RunFixtureSetUp(Fixture fixture, object fixtureInstance)
            {
                CheckCanceled();

                progressMonitor.SetStatus(String.Format(Resources.MbUnit2TestController_StatusMessage_RunningFixtureSetUp, fixture.Name));

                ITestMonitor fixtureTestMonitor;
                if (fixture.HasSetUp && fixtureTestMonitors.TryGetValue(fixture, out fixtureTestMonitor))
                {
                    IStepMonitor fixtureStepMonitor = activeStepMonitors[fixtureTestMonitor];
                    fixtureStepMonitor.LifecyclePhase = LifecyclePhases.SetUp;
                }

                object result = base.RunFixtureSetUp(fixture, fixtureInstance);

                progressMonitor.Worked(workUnit);
                return result;
            }

            protected override void RunFixtureTearDown(Fixture fixture, object fixtureInstance)
            {
                CheckCanceled();

                progressMonitor.SetStatus(String.Format(Resources.MbUnit2TestController_StatusMessage_RunningFixtureTearDown, fixture.Name));

                ITestMonitor fixtureTestMonitor;
                if (fixture.HasSetUp && fixtureTestMonitors.TryGetValue(fixture, out fixtureTestMonitor))
                {
                    IStepMonitor fixtureStepMonitor = activeStepMonitors[fixtureTestMonitor];
                    fixtureStepMonitor.LifecyclePhase = LifecyclePhases.TearDown;
                }

                base.RunFixtureTearDown(fixture, fixtureInstance);

                progressMonitor.Worked(workUnit);
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
                return runPipeTestMonitors.ContainsKey(runPipe);
            }
            #endregion

            #region IRunPipeListener
            void IRunPipeListener.Start(RunPipe pipe)
            {
                CheckCanceled();

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
                if (assemblyTestMonitor == null)
                    return;

                IStepMonitor assemblyStepMonitor = assemblyTestMonitor.StartRootStep();
                activeStepMonitors.Add(assemblyTestMonitor, assemblyStepMonitor);
            }

            private void HandleAssemblyFinish(TestOutcome outcome)
            {
                if (assemblyTestMonitor == null)
                    return;

                IStepMonitor assemblyStepMonitor = activeStepMonitors[assemblyTestMonitor];
                activeStepMonitors.Remove(assemblyTestMonitor);

                assemblyStepMonitor.FinishStep(TestStatus.Executed, outcome, null);
            }

            private void HandleFixtureStart(Fixture fixture)
            {
                ITestMonitor fixtureTestMonitor;
                if (!fixtureTestMonitors.TryGetValue(fixture, out fixtureTestMonitor))
                    return;

                IStepMonitor fixtureStepMonitor = fixtureTestMonitor.StartRootStep();
                activeStepMonitors.Add(fixtureTestMonitor, fixtureStepMonitor);
            }

            private void HandleFixtureFinish(Fixture fixture, ReportRunResult reportRunResult)
            {
                ITestMonitor fixtureTestMonitor;
                if (!fixtureTestMonitors.TryGetValue(fixture, out fixtureTestMonitor))
                    return;

                IStepMonitor fixtureStepMonitor = activeStepMonitors[fixtureTestMonitor];
                activeStepMonitors.Remove(fixtureTestMonitor);

                FinishStepWithReportRunResult(fixtureStepMonitor, reportRunResult);
            }

            private void HandleTestStart(RunPipe runPipe)
            {
                progressMonitor.SetStatus(String.Format(Resources.MbUnit2TestController_StatusMessage_RunningTest, runPipe.ShortName));

                ITestMonitor runPipeTestMonitor;
                if (!runPipeTestMonitors.TryGetValue(runPipe, out runPipeTestMonitor))
                    return;

                IStepMonitor runPipeStepMonitor = runPipeTestMonitor.StartRootStep();
                activeStepMonitors.Add(runPipeTestMonitor, runPipeStepMonitor);
            }

            private void HandleTestFinish(RunPipe runPipe, ReportRun reportRun)
            {
                ITestMonitor runPipeTestMonitor;
                if (runPipeTestMonitors.TryGetValue(runPipe, out runPipeTestMonitor))
                {
                    IStepMonitor stepMonitor = activeStepMonitors[runPipeTestMonitor];
                    activeStepMonitors.Remove(runPipeTestMonitor);

                    // Output all execution log contents.
                    // Note: ReportRun.Asserts is not actually populated by MbUnit so we ignore it.
                    if (reportRun.ConsoleOut.Length != 0)
                    {
                        stepMonitor.LogWriter[LogStreamNames.ConsoleOutput].Write(reportRun.ConsoleOut);
                    }
                    if (reportRun.ConsoleError.Length != 0)
                    {
                        stepMonitor.LogWriter[LogStreamNames.ConsoleError].Write(reportRun.ConsoleError);
                    }
                    foreach (ReportWarning warning in reportRun.Warnings)
                    {
                        stepMonitor.LogWriter[LogStreamNames.Warnings].BeginSection("Warning");
                        stepMonitor.LogWriter[LogStreamNames.Warnings].WriteLine(warning.Text);
                        stepMonitor.LogWriter[LogStreamNames.Warnings].EndSection();
                    }
                    if (reportRun.Exception != null)
                    {
                        stepMonitor.LogWriter[LogStreamNames.Failures].BeginSection("Exception");
                        stepMonitor.LogWriter[LogStreamNames.Failures].Write(FormatReportException(reportRun.Exception));
                        stepMonitor.LogWriter[LogStreamNames.Failures].EndSection();
                    }

                    // Finish up...
                    stepMonitor.Context.AddAssertCount(reportRun.AssertCount);
                    FinishStepWithReportRunResult(stepMonitor, reportRun.Result);
                }

                progressMonitor.Worked(workUnit);
            }

            private void HandleCanceled(object sender, EventArgs e)
            {
                Abort();
            }

            /// <summary>
            /// MbUnit's handling of Abort() isn't very robust.  It is susceptible to
            /// race conditions in various placed.  For example, the fixture runner resets
            /// its AbortPending flag when Run is invoked.  It is possible that this
            /// will prevent the abort from succeeding if it happens too early.
            /// </summary>
            private void CheckCanceled()
            {
                if (progressMonitor.IsCanceled)
                    Abort();
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
                    result.Append(@": ").Append(ex.Message);

                if (ex.Exception != null)
                {
                    result.Append(@" ---> ")
                        .Append(FormatReportException(ex.Exception))
                        .Append(Environment.NewLine)
                        .Append(@"   --- ")
                        .Append(Resources.MbUnit2TestController_EndOfInnerExceptionStackTrace)
                        .Append(@" ---");
                }

                if (! String.IsNullOrEmpty(ex.StackTrace))
                {
                    result.Append(Environment.NewLine).Append(ex.StackTrace);
                }

                return result.ToString();
            }

            private static void FinishStepWithReportRunResult(IStepMonitor stepMonitor, ReportRunResult reportRunResult)
            {
                switch (reportRunResult)
                {
                    case ReportRunResult.NotRun:
                        stepMonitor.FinishStep(TestStatus.NotRun, TestOutcome.Inconclusive, null);
                        break;

                    case ReportRunResult.Skip:
                        stepMonitor.FinishStep(TestStatus.Skipped, TestOutcome.Inconclusive, null);
                        break;

                    case ReportRunResult.Ignore:
                        stepMonitor.FinishStep(TestStatus.Ignored, TestOutcome.Inconclusive, null);
                        break;

                    case ReportRunResult.Success:
                        stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Passed, null);
                        break;

                    case ReportRunResult.Failure:
                        stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Failed, null);
                        break;
                }
            }
        }
    }
}
