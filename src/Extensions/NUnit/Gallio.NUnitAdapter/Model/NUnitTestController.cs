// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Threading;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Common.Markup;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.NUnitAdapter.Properties;
using NUnit.Core;
using TestResult = Gallio.Model.TestResult;

namespace Gallio.NUnitAdapter.Model
{
    /// <summary>
    /// Controls the execution of NUnit tests.
    /// </summary>
    internal class NUnitTestController : TestController
    {
        private readonly TestRunner runner;

        /// <summary>
        /// Creates a test controller.
        /// </summary>
        /// <param name="runner">The test runner.</param>
        public NUnitTestController(TestRunner runner)
        {
            this.runner = runner;
        }

        /// <inheritdoc />
        protected override TestResult RunImpl(ITestCommand rootTestCommand, TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            ThrowIfDisposed();

            IList<ITestCommand> testCommands = rootTestCommand.GetAllCommands();
            using (progressMonitor.BeginTask(Resources.NUnitTestController_RunningNUnitTests, testCommands.Count))
            {
                if (progressMonitor.IsCanceled)
                    return new TestResult(TestOutcome.Canceled);

                if (options.SkipTestExecution)
                {
                    return SkipAll(rootTestCommand, parentTestStep);
                }
                else
                {
                    using (RunMonitor monitor = new RunMonitor(runner, testCommands, parentTestStep, progressMonitor))
                    {
                        return monitor.Run();
                    }
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (runner == null)
                throw new ObjectDisposedException(Resources.NUnitTestController_ControllerWasDisposedException);
        }

        private class RunMonitor : EventListener, ITestFilter, IDisposable
        {
            private readonly IProgressMonitor progressMonitor;
            private readonly TestRunner runner;
            private readonly IList<ITestCommand> testCommands;
            private readonly TestStep topTestStep;

            private Dictionary<TestName, ITestCommand> testCommandsByTestName;
            private Stack<ITestContext> testContextStack;
            private TestResult topResult;
            private NUnitTestFilter nUnitTestFilter;

            public RunMonitor(TestRunner runner, IList<ITestCommand> testCommands, TestStep topTestStep,
                IProgressMonitor progressMonitor)
            {
                this.progressMonitor = progressMonitor;
                this.runner = runner;
                this.testCommands = testCommands;
                this.topTestStep = topTestStep;

                Initialize();
            }

            public void Dispose()
            {
                progressMonitor.Canceled -= HandleCanceled;
            }

            public TestResult Run()
            {
                try
                {
                    runner.Run(this, this);
                }
                catch (ThreadAbortException)
                {
                    // NUnit cancelation is nasty!  It does a Thread.Abort on the test runner's
                    // thread.  If we were aborted due to cancelation then we try to recover here.
                    // This happened when we were using the SimpleTestRunner but it looks like the
                    // RemoteTestRunner is immune.  I'm leaving this in just in case.
                    if (progressMonitor.IsCanceled)
                        Thread.ResetAbort();
                }

                return topResult ?? new TestResult(TestOutcome.Error);
            }

            private void Initialize()
            {
                progressMonitor.Canceled += HandleCanceled;

                // Build a reverse mapping from NUnit tests.
                testCommandsByTestName = new Dictionary<TestName, ITestCommand>();
                foreach (ITestCommand testCommand in testCommands)
                {
                    NUnitTest test = (NUnitTest) testCommand.Test;
                    test.ProcessTestNames(delegate(NUnit.Core.TestName testName)
                    {
                        testCommandsByTestName[testName] = testCommand;
                    }); 
                }
                nUnitTestFilter = new NUnitTestFilter(testCommandsByTestName);

                testContextStack = new Stack<ITestContext>();
            }

            #region EventListener Members
            void EventListener.RunStarted(string name, int testCount)
            {
                // Note: This handles a possible race condition involving Cancel wherein the
                //       cancelation may not occur if we abort too soon.
                CheckCanceled();
            }

            void EventListener.RunFinished(NUnit.Core.TestResult result)
            {
            }

            void EventListener.RunFinished(Exception exception)
            {
            }

            void EventListener.TestStarted(TestName testName)
            {
                HandleTestOrSuiteStarted(testName);
            }

#if NUNIT248
            void EventListener.TestFinished(TestCaseResult nunitResult)
#elif (NUNIT253 || NUNITLATEST)
            void EventListener.TestFinished(NUnit.Core.TestResult nunitResult)
#else
#error "Unrecognized NUnit framework version."
#endif
            {
                HandleTestOrSuiteFinished(nunitResult);
            }

            void EventListener.TestOutput(TestOutput testOutput)
            {
                if (testContextStack.Count == 0)
                    return;

                ITestContext testContext = testContextStack.Peek();

                string streamName;
                switch (testOutput.Type)
                {
                    default:
                    case TestOutputType.Out:
                        streamName = MarkupStreamNames.ConsoleOutput;
                        break;
                    case TestOutputType.Error:
                        streamName = MarkupStreamNames.ConsoleError;
                        break;
                    case TestOutputType.Trace:
                        streamName = MarkupStreamNames.DebugTrace;
                        break;
                }

                testContext.LogWriter[streamName].Write(testOutput.Text);
            }

            void EventListener.SuiteStarted(TestName testName)
            {
                HandleTestOrSuiteStarted(testName);
            }

#if NUNIT248
            void EventListener.SuiteFinished(TestSuiteResult nunitResult)
#elif (NUNIT253 || NUNITLATEST)
            void EventListener.SuiteFinished(NUnit.Core.TestResult nunitResult)
#else
#error "Unrecognized NUnit framework version."
#endif
            {
                HandleTestOrSuiteFinished(nunitResult);
            }

            void EventListener.UnhandledException(Exception exception)
            {
                LogException(exception);
            }

            private void LogException(Exception exception)
            {
                if (testContextStack.Count == 0)
                    return;

                ITestContext testContext = testContextStack.Peek();

                testContext.LogWriter.Failures.WriteException(exception, Resources.NUnitTestController_UnhandledExceptionSectionName);
            }

            private void HandleTestOrSuiteStarted(TestName testName)
            {
                ITestCommand testCommand;
                if (!testCommandsByTestName.TryGetValue(testName, out testCommand))
                    return;

                progressMonitor.SetStatus(testCommand.Test.Name);

                TestStep parentTestStep = testContextStack.Count != 0 ? testContextStack.Peek().TestStep : topTestStep;
                ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
                testContextStack.Push(testContext);

                testContext.LifecyclePhase = LifecyclePhases.Execute;
            }

            private void HandleTestOrSuiteFinished(NUnit.Core.TestResult nunitResult)
            {
                if (testContextStack.Count == 0)
                    return;

                ITestContext testContext = testContextStack.Peek();
                NUnitTest test = (NUnitTest) testContext.TestStep.Test;
                if (test.Test.TestName != nunitResult.Test.TestName)
                    return;

                testContextStack.Pop();

                progressMonitor.Worked(1);

                string logStreamName = nunitResult.ResultState == ResultState.Success ? MarkupStreamNames.Warnings : MarkupStreamNames.Failures;

                MarkupDocumentWriter logWriter = testContext.LogWriter;
                if (nunitResult.Message != null)
                {
                    using (logWriter[logStreamName].BeginSection(Resources.NUnitTestController_ResultMessageSectionName))
                        logWriter[logStreamName].Write(nunitResult.Message);
                }
                if (nunitResult.StackTrace != null)
                {
                    using (logWriter[logStreamName].BeginSection(Resources.NUnitTestController_ResultStackTraceSectionName))
                        using (logWriter[logStreamName].BeginMarker(Marker.StackTrace))
                            logWriter[logStreamName].Write(nunitResult.StackTrace);
                }

                testContext.AddAssertCount(nunitResult.AssertCount);
                TestResult result = testContext.FinishStep(CreateOutcomeFromResult(nunitResult), null);
                if (testContextStack.Count == 0)
                    topResult = result;
            }

            private static TestOutcome CreateOutcomeFromResult(NUnit.Core.TestResult nunitResult)
            {
#if NUNIT248
                switch (nunitResult.RunState)
                {
                    case RunState.Executed:
                        switch (nunitResult.ResultState)
                        {
                            case ResultState.Success:
                                return TestOutcome.Passed;
                            case ResultState.Failure:
                                return TestOutcome.Failed;
                            default:
                            case ResultState.Error:
                                return TestOutcome.Error;
                        }

                    case RunState.Ignored:
                        return TestOutcome.Ignored;

                    default:
                    case RunState.NotRunnable:
                    case RunState.Runnable:
                    case RunState.Skipped:
                    case RunState.Explicit:
                        return TestOutcome.Skipped;
                }
#elif (NUNIT253 || NUNITLATEST)
                switch (nunitResult.ResultState)
                {
                    case ResultState.Success:
                        return TestOutcome.Passed;
                    case ResultState.Failure:
                        return TestOutcome.Failed;
                    case ResultState.Ignored:
                        return TestOutcome.Ignored;
                    case ResultState.Inconclusive:
                        return TestOutcome.Inconclusive;
                    case ResultState.NotRunnable:
                    case ResultState.Skipped:
                        return TestOutcome.Skipped;
                    default:
                    case ResultState.Error:
                        return TestOutcome.Error;
                }
#else
#error "Unrecognized NUnit framework version."
#endif
            }
            #endregion

            #region ITestFilter Members
            bool ITestFilter.Pass(NUnit.Core.ITest test)
            {
                return nUnitTestFilter.Pass(test);
            }

            bool ITestFilter.Match(NUnit.Core.ITest test)
            {
                return nUnitTestFilter.Match(test);
            }

            bool ITestFilter.IsEmpty
            {
                get { return nUnitTestFilter.IsEmpty; }
            }
            #endregion

            private void Cancel()
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    runner.CancelRun();
                });
            }

            private void CheckCanceled()
            {
                if (progressMonitor.IsCanceled)
                    Cancel();
            }

            private void HandleCanceled(object sender, EventArgs e)
            {
                Cancel();
            }
        }
    }
}
