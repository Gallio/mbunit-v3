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
extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Collections;
using Gallio.MbUnit2Adapter.Properties;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Model.Execution;
using MbUnit2::MbUnit.Core;
using MbUnit2::MbUnit.Core.Remoting;
using MbUnit2::MbUnit.Core.Filters;
using MbUnit2::MbUnit.Core.Reports.Serialization;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Controls the execution of MbUnit v2 tests.
    /// </summary>
    internal class MbUnit2TestController : ITestController
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
        public void RunTests(IProgressMonitor progressMonitor, ITestCommand rootTestCommand,
            ITestInstance parentTestInstance)
        {
            ThrowIfDisposed();

            using (progressMonitor)
            {
                progressMonitor.BeginTask(Resources.MbUnit2TestController_RunningMbUnitTests, 1);

                if (progressMonitor.IsCanceled)
                    return;

                IList<ITestCommand> testCommands = rootTestCommand.GetAllCommands();

                using (InstrumentedFixtureRunner fixtureRunner = new InstrumentedFixtureRunner(fixtureExplorer,
                    testCommands, progressMonitor, parentTestInstance))
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
            private readonly IList<ITestCommand> testCommands;
            private readonly IProgressMonitor progressMonitor;
            private readonly ITestInstance topTestInstance;

            private HashSet<Type> includedFixtureTypes;

            private ITestCommand assemblyTestCommand;
            private Dictionary<Fixture, ITestCommand> fixtureTestCommands;
            private Dictionary<RunPipe, ITestCommand> runPipeTestCommands;

            private Dictionary<ITestCommand, ITestContext> activeTestContexts;

            private double workUnit;

            public InstrumentedFixtureRunner(FixtureExplorer fixtureExplorer,
                IList<ITestCommand> testCommands, IProgressMonitor progressMonitor, ITestInstance topTestInstance)
            {
                this.fixtureExplorer = fixtureExplorer;
                this.progressMonitor = progressMonitor;
                this.testCommands = testCommands;
                this.topTestInstance = topTestInstance;

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
                includedFixtureTypes = new HashSet<Type>();
                fixtureTestCommands = new Dictionary<Fixture, ITestCommand>();
                runPipeTestCommands = new Dictionary<RunPipe, ITestCommand>();
                activeTestContexts = new Dictionary<ITestCommand, ITestContext>();

                bool isExplicit = false;
                foreach (ITestCommand testCommand in testCommands)
                {
                    if (testCommand.IsExplicit)
                        isExplicit = true;

                    MbUnit2Test test = (MbUnit2Test)testCommand.Test;
                    Fixture fixture = test.Fixture;
                    RunPipe runPipe = test.RunPipe;

                    if (fixture == null)
                    {
                        assemblyTestCommand = testCommand;
                    }
                    else if (runPipe == null)
                    {
                        includedFixtureTypes.Add(fixture.Type);
                        fixtureTestCommands[fixture] = testCommand;

                        if (fixture.HasSetUp)
                            totalWork += 1;
                        if (fixture.HasTearDown)
                            totalWork += 1;
                    }
                    else
                    {
                        runPipeTestCommands[runPipe] = testCommand;
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
                    ITestContext assemblyTestContext = activeTestContexts[assemblyTestCommand];
                    assemblyTestContext.LifecyclePhase = LifecyclePhases.SetUp;
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

                if (Explorer.HasAssemblyTearDown && assemblyTestCommand != null)
                {
                    ITestContext assemblyTestContext = activeTestContexts[assemblyTestCommand];
                    assemblyTestContext.LifecyclePhase = LifecyclePhases.TearDown;
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

                ITestCommand fixtureTestCommand;
                if (fixture.HasSetUp && fixtureTestCommands.TryGetValue(fixture, out fixtureTestCommand))
                {
                    ITestContext fixtureTestContext = activeTestContexts[fixtureTestCommand];
                    fixtureTestContext.LifecyclePhase = LifecyclePhases.SetUp;
                }

                object result = base.RunFixtureSetUp(fixture, fixtureInstance);

                progressMonitor.Worked(workUnit);
                return result;
            }

            protected override void RunFixtureTearDown(Fixture fixture, object fixtureInstance)
            {
                CheckCanceled();

                progressMonitor.SetStatus(String.Format(Resources.MbUnit2TestController_StatusMessage_RunningFixtureTearDown, fixture.Name));

                ITestCommand fixtureTestCommand;
                if (fixture.HasSetUp && fixtureTestCommands.TryGetValue(fixture, out fixtureTestCommand))
                {
                    ITestContext fixtureTestContext = activeTestContexts[fixtureTestCommand];
                    fixtureTestContext.LifecyclePhase = LifecyclePhases.TearDown;
                }

                base.RunFixtureTearDown(fixture, fixtureInstance);

                progressMonitor.Worked(workUnit);
            }
            #endregion

            #region IFixtureFilter
            bool IFixtureFilter.Filter(Type type)
            {
                return includedFixtureTypes.Contains(type);
            }
            #endregion

            #region IRunPipeFilter
            bool IRunPipeFilter.Filter(RunPipe runPipe)
            {
                return runPipeTestCommands.ContainsKey(runPipe);
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
                if (assemblyTestCommand == null)
                    return;

                ITestContext assemblyTestContext = assemblyTestCommand.StartRootStep(topTestInstance);
                activeTestContexts.Add(assemblyTestCommand, assemblyTestContext);
            }

            private void HandleAssemblyFinish(TestOutcome outcome)
            {
                if (assemblyTestCommand == null)
                    return;

                ITestContext assemblyTestContext = activeTestContexts[assemblyTestCommand];
                activeTestContexts.Remove(assemblyTestCommand);

                assemblyTestContext.FinishStep(TestStatus.Executed, outcome, null);
            }

            private void HandleFixtureStart(Fixture fixture)
            {
                ITestCommand fixtureTestCommand;
                if (!fixtureTestCommands.TryGetValue(fixture, out fixtureTestCommand))
                    return;
                ITestContext assemblyTestContext;
                if (!activeTestContexts.TryGetValue(assemblyTestCommand, out assemblyTestContext))
                    return;

                ITestContext fixtureTestContext = fixtureTestCommand.StartRootStep(assemblyTestContext.TestStep.TestInstance);
                activeTestContexts.Add(fixtureTestCommand, fixtureTestContext);
            }

            private void HandleFixtureFinish(Fixture fixture, ReportRunResult reportRunResult)
            {
                ITestCommand fixtureTestCommand;
                if (!fixtureTestCommands.TryGetValue(fixture, out fixtureTestCommand))
                    return;

                ITestContext fixtureTestContext = activeTestContexts[fixtureTestCommand];
                activeTestContexts.Remove(fixtureTestCommand);

                FinishStepWithReportRunResult(fixtureTestContext, reportRunResult);
            }

            private void HandleTestStart(RunPipe runPipe)
            {
                progressMonitor.SetStatus(String.Format(Resources.MbUnit2TestController_StatusMessage_RunningTest, runPipe.ShortName));

                ITestCommand runPipeTestCommand;
                if (!runPipeTestCommands.TryGetValue(runPipe, out runPipeTestCommand))
                    return;
                ITestCommand fixtureTestCommand;
                if (!fixtureTestCommands.TryGetValue(runPipe.Fixture, out fixtureTestCommand))
                    return;
                ITestContext fixtureTestContext;
                if (!activeTestContexts.TryGetValue(fixtureTestCommand, out fixtureTestContext))
                    return;

                ITestContext runPipeTestContext = runPipeTestCommand.StartRootStep(fixtureTestContext.TestStep.TestInstance);
                activeTestContexts.Add(runPipeTestCommand, runPipeTestContext);
            }

            private void HandleTestFinish(RunPipe runPipe, ReportRun reportRun)
            {
                ITestCommand runPipeTestCommand;
                if (runPipeTestCommands.TryGetValue(runPipe, out runPipeTestCommand))
                {
                    ITestContext testContext = activeTestContexts[runPipeTestCommand];
                    activeTestContexts.Remove(runPipeTestCommand);

                    // Output all execution log contents.
                    // Note: ReportRun.Asserts is not actually populated by MbUnit so we ignore it.
                    if (reportRun.ConsoleOut.Length != 0)
                    {
                        testContext.LogWriter[LogStreamNames.ConsoleOutput].Write(reportRun.ConsoleOut);
                    }
                    if (reportRun.ConsoleError.Length != 0)
                    {
                        testContext.LogWriter[LogStreamNames.ConsoleError].Write(reportRun.ConsoleError);
                    }
                    foreach (ReportWarning warning in reportRun.Warnings)
                    {
                        testContext.LogWriter[LogStreamNames.Warnings].BeginSection("Warning");
                        testContext.LogWriter[LogStreamNames.Warnings].WriteLine(warning.Text);
                        testContext.LogWriter[LogStreamNames.Warnings].EndSection();
                    }
                    if (reportRun.Exception != null)
                    {
                        testContext.LogWriter[LogStreamNames.Failures].BeginSection("Exception");
                        testContext.LogWriter[LogStreamNames.Failures].Write(FormatReportException(reportRun.Exception));
                        testContext.LogWriter[LogStreamNames.Failures].EndSection();
                    }

                    // Finish up...
                    testContext.AddAssertCount(reportRun.AssertCount);
                    FinishStepWithReportRunResult(testContext, reportRun.Result);
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

            private static void FinishStepWithReportRunResult(ITestContext testContext, ReportRunResult reportRunResult)
            {
                switch (reportRunResult)
                {
                    case ReportRunResult.NotRun:
                        testContext.FinishStep(TestStatus.NotRun, TestOutcome.Inconclusive, null);
                        break;

                    case ReportRunResult.Skip:
                        testContext.FinishStep(TestStatus.Skipped, TestOutcome.Inconclusive, null);
                        break;

                    case ReportRunResult.Ignore:
                        testContext.FinishStep(TestStatus.Ignored, TestOutcome.Inconclusive, null);
                        break;

                    case ReportRunResult.Success:
                        testContext.FinishStep(TestStatus.Executed, TestOutcome.Passed, null);
                        break;

                    case ReportRunResult.Failure:
                        testContext.FinishStep(TestStatus.Executed, TestOutcome.Failed, null);
                        break;
                }
            }
        }
    }
}
