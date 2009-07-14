// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Collections;
using Gallio.MbUnit2Adapter.Properties;
using Gallio.Common.Diagnostics;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Common.Markup;
using MbUnit2::MbUnit.Core;
using MbUnit2::MbUnit.Core.Remoting;
using MbUnit2::MbUnit.Core.Filters;
using MbUnit2::MbUnit.Core.Reports.Serialization;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Controls the execution of MbUnit v2 tests.
    /// </summary>
    internal class MbUnit2TestController : TestController
    {
        private readonly FixtureExplorer fixtureExplorer;

        /// <summary>
        /// Creates a runner.
        /// </summary>
        /// <param name="fixtureExplorer">The fixture explorer</param>
        public MbUnit2TestController(FixtureExplorer fixtureExplorer)
        {
            this.fixtureExplorer = fixtureExplorer;
        }

        /// <inheritdoc />
        protected override TestResult RunImpl(ITestCommand rootTestCommand, TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            ThrowIfDisposed();

            using (progressMonitor.BeginTask(Resources.MbUnit2TestController_RunningMbUnitTests, 1))
            {
                if (progressMonitor.IsCanceled)
                    return new TestResult(TestOutcome.Canceled);

                if (options.SkipTestExecution)
                {
                    return SkipAll(rootTestCommand, parentTestStep);
                }
                else
                {
                    IList<ITestCommand> testCommands = rootTestCommand.GetAllCommands();

                    using (InstrumentedFixtureRunner fixtureRunner = new InstrumentedFixtureRunner(fixtureExplorer,
                        testCommands, progressMonitor, parentTestStep))
                    {
                        return fixtureRunner.Run();
                    }
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
            private readonly TestStep topTestStep;

            private HashSet<Type> includedFixtureTypes;

            private TestOutcome assemblyTestOutcome;
            private TestResult assemblyTestResult;
            private ITestCommand assemblyTestCommand;
            private Dictionary<Fixture, ITestCommand> fixtureTestCommands;
            private Dictionary<RunPipe, ITestCommand> runPipeTestCommands;

            private Dictionary<ITestCommand, ITestContext> activeTestContexts;

            private double workUnit;

            public InstrumentedFixtureRunner(FixtureExplorer fixtureExplorer,
                IList<ITestCommand> testCommands, IProgressMonitor progressMonitor, TestStep topTestStep)
            {
                this.fixtureExplorer = fixtureExplorer;
                this.progressMonitor = progressMonitor;
                this.testCommands = testCommands;
                this.topTestStep = topTestStep;

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

            public TestResult Run()
            {
                assemblyTestOutcome = TestOutcome.Passed;

                if (assemblyTestCommand != null)
                {
                    ReportListener reportListener = new ReportListener();
                    Run(fixtureExplorer, reportListener);

                    // TODO: Do we need to do anyhing with the result in the report listener?
                }

                return assemblyTestResult ?? new TestResult(TestOutcome.Error);
            }

            #region Overrides to track assembly and fixture lifecycle
            protected override bool RunAssemblySetUp()
            {
                CheckCanceled();

                progressMonitor.SetStatus(assemblyTestCommand.Test.Name);

                HandleAssemblyStart();

                ITestContext assemblyTestContext = activeTestContexts[assemblyTestCommand];
                if (Explorer.HasAssemblySetUp)
                    assemblyTestContext.LifecyclePhase = LifecyclePhases.SetUp;
 
                bool success = base.RunAssemblySetUp();

                // Note: MbUnit won't call RunAssemblyTearDown itself if the assembly setup fails
                //       so we need to make sure we finish things up ourselves.
                if (!success)
                {
                    assemblyTestContext.LogWriter.Failures.Write("The test assembly setup failed.\n");
                    HandleAssemblyFinish(TestOutcome.Failed);
                }
                else
                {
                    assemblyTestContext.LifecyclePhase = LifecyclePhases.Execute;
                }

                progressMonitor.Worked(workUnit);
                return success;
            }

            protected override bool RunAssemblyTearDown()
            {
                progressMonitor.SetStatus(assemblyTestCommand.Test.Name);

                ITestContext assemblyTestContext = activeTestContexts[assemblyTestCommand];
                if (Explorer.HasAssemblyTearDown)
                    assemblyTestContext.LifecyclePhase = LifecyclePhases.TearDown;

                bool success = base.RunAssemblyTearDown();

                if (success)
                {
                    HandleAssemblyFinish(TestOutcome.Passed);
                }
                else
                {
                    assemblyTestContext.LogWriter.Failures.Write("The test assembly teardown failed.\n");
                    HandleAssemblyFinish(TestOutcome.Failed);
                }

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

                ITestCommand fixtureTestCommand;
                ITestContext fixtureTestContext = null;
                if (fixtureTestCommands.TryGetValue(fixture, out fixtureTestCommand))
                {
                    progressMonitor.SetStatus(fixtureTestCommand.Test.Name);

                    fixtureTestContext = activeTestContexts[fixtureTestCommand];
                    fixtureTestContext.LifecyclePhase = LifecyclePhases.SetUp;
                }

                object result = base.RunFixtureSetUp(fixture, fixtureInstance);

                if (fixtureTestContext != null)
                    fixtureTestContext.LifecyclePhase = LifecyclePhases.Execute;

                progressMonitor.Worked(workUnit);
                return result;
            }

            protected override void RunFixtureTearDown(Fixture fixture, object fixtureInstance)
            {
                CheckCanceled();

                ITestCommand fixtureTestCommand;
                if (fixtureTestCommands.TryGetValue(fixture, out fixtureTestCommand))
                {
                    progressMonitor.SetStatus(fixtureTestCommand.Test.Name);

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
                ITestContext assemblyTestContext = assemblyTestCommand.StartPrimaryChildStep(topTestStep);
                activeTestContexts.Add(assemblyTestCommand, assemblyTestContext);
            }

            private void HandleAssemblyFinish(TestOutcome outcome)
            {
                ITestContext assemblyTestContext = activeTestContexts[assemblyTestCommand];
                activeTestContexts.Remove(assemblyTestCommand);

                // only update status if more severe
                if (outcome.Status > assemblyTestOutcome.Status)
                    assemblyTestOutcome = outcome;

                assemblyTestResult = assemblyTestContext.FinishStep(assemblyTestOutcome, null);
            }

            private void HandleFixtureStart(Fixture fixture)
            {
                ITestCommand fixtureTestCommand;
                if (!fixtureTestCommands.TryGetValue(fixture, out fixtureTestCommand))
                    return;
                ITestContext assemblyTestContext;
                if (!activeTestContexts.TryGetValue(assemblyTestCommand, out assemblyTestContext))
                    return;

                ITestContext fixtureTestContext = fixtureTestCommand.StartPrimaryChildStep(assemblyTestContext.TestStep);
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
                ITestCommand runPipeTestCommand;
                if (!runPipeTestCommands.TryGetValue(runPipe, out runPipeTestCommand))
                    return;
                ITestCommand fixtureTestCommand;
                if (!fixtureTestCommands.TryGetValue(runPipe.Fixture, out fixtureTestCommand))
                    return;
                ITestContext fixtureTestContext;
                if (!activeTestContexts.TryGetValue(fixtureTestCommand, out fixtureTestContext))
                    return;

                progressMonitor.SetStatus(runPipeTestCommand.Test.Name);

                ITestContext runPipeTestContext = runPipeTestCommand.StartPrimaryChildStep(fixtureTestContext.TestStep);
                activeTestContexts.Add(runPipeTestCommand, runPipeTestContext);

                runPipeTestContext.LifecyclePhase = LifecyclePhases.Execute;
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
                        testContext.LogWriter.ConsoleOutput.Write(reportRun.ConsoleOut);
                    }
                    if (reportRun.ConsoleError.Length != 0)
                    {
                        testContext.LogWriter.ConsoleError.Write(reportRun.ConsoleError);
                    }
                    foreach (ReportWarning warning in reportRun.Warnings)
                    {
                        testContext.LogWriter.Warnings.WriteLine(warning.Text);
                    }
                    if (reportRun.Exception != null)
                    {
                        testContext.LogWriter.Failures.WriteException(GetExceptionDataFromReportException(reportRun.Exception), "Exception");
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

            private void FinishStepWithReportRunResult(ITestContext testContext, ReportRunResult reportRunResult)
            {
                TestOutcome outcome = GetOutcomeFromReportRunResult(reportRunResult);
                testContext.FinishStep(outcome, null);

                // only update assembly status if more severe
                if (outcome.Status > assemblyTestOutcome.Status)
                    assemblyTestOutcome = outcome;
            }

            private static TestOutcome GetOutcomeFromReportRunResult(ReportRunResult reportRunResult)
            {
                switch (reportRunResult)
                {
                    case ReportRunResult.NotRun:
                    case ReportRunResult.Skip:
                        return TestOutcome.Skipped;

                    case ReportRunResult.Ignore:
                        return TestOutcome.Ignored;

                    case ReportRunResult.Success:
                        return TestOutcome.Passed;

                    case ReportRunResult.Failure:
                        return TestOutcome.Failed;

                    default:
                        throw new ArgumentException("Unsupported report run result.", "reportRunResult");
                }
            }

            private static ExceptionData GetExceptionDataFromReportException(ReportException ex)
            {
                return new ExceptionData(ex.Type ?? "", ex.Message ?? "", ex.StackTrace ?? "",
                    ex.Exception != null ? GetExceptionDataFromReportException(ex.Exception) : null);
            }
        }
    }
}
