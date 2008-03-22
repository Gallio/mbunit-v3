// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Threading;
using Gallio.Model.Execution;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Model;
using Gallio.NUnitAdapter.Properties;
using NUnit.Core;
using NUnitTestResult=NUnit.Core.TestResult;
using NUnitTestName = NUnit.Core.TestName;

namespace Gallio.NUnitAdapter.Model
{
    /// <summary>
    /// Controls the execution of NUnit tests.
    /// </summary>
    internal class NUnitTestController : BaseTestController
    {
        private TestRunner runner;

        /// <summary>
        /// Creates a test controller.
        /// </summary>
        /// <param name="runner">The test runner</param>
        public NUnitTestController(TestRunner runner)
        {
            this.runner = runner;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            runner = null;
        }

        /// <inheritdoc />
        protected override void RunTestsInternal(ITestCommand rootTestCommand, ITestStep parentTestStep,
            TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            ThrowIfDisposed();

            using (progressMonitor)
            {
                IList<ITestCommand> testCommands = rootTestCommand.GetAllCommands();

                progressMonitor.BeginTask(Resources.NUnitTestController_RunningNUnitTests, testCommands.Count);
                if (progressMonitor.IsCanceled)
                    return;

                if (options.SkipTestExecution)
                {
                    SkipAll(rootTestCommand, parentTestStep);
                }
                else
                {
                    using (RunMonitor monitor = new RunMonitor(runner, testCommands, parentTestStep, progressMonitor))
                    {
                        monitor.Run();
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
            private readonly ITestStep topTestStep;

            private Dictionary<TestName, ITestCommand> testCommandsByTestName;
            private Stack<ITestContext> testContextStack;

            public RunMonitor(TestRunner runner, IList<ITestCommand> testCommands, ITestStep topTestStep,
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

            public void Run()
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
            }

            private void Initialize()
            {
                progressMonitor.Canceled += HandleCanceled;

                // Build a reverse mapping from NUnit tests.
                testCommandsByTestName = new Dictionary<TestName, ITestCommand>();
                foreach (ITestCommand testCommand in testCommands)
                {
                    NUnitTest test = (NUnitTest) testCommand.Test;
                    test.ProcessTestNames(delegate(NUnitTestName testName)
                    {
                        testCommandsByTestName[testName] = testCommand;
                    }); 
                }

                testContextStack = new Stack<ITestContext>();
            }

            #region EventListener Members
            void EventListener.RunStarted(string name, int testCount)
            {
                // Note: This handles a possible race condition involving Cancel wherein the
                //       cancelation may not occur if we abort too soon.
                CheckCanceled();
            }

            void EventListener.RunFinished(NUnitTestResult result)
            {
            }

            void EventListener.RunFinished(Exception exception)
            {
            }

            void EventListener.TestStarted(TestName testName)
            {
                HandleTestOrSuiteStarted(testName);
            }

            void EventListener.TestFinished(TestCaseResult nunitResult)
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
                        streamName = LogStreamNames.ConsoleOutput;
                        break;
                    case TestOutputType.Error:
                        streamName = LogStreamNames.ConsoleError;
                        break;
                    case TestOutputType.Trace:
                        streamName = LogStreamNames.DebugTrace;
                        break;
                }

                testContext.LogWriter.Write(streamName, testOutput.Text);
            }

            void EventListener.SuiteStarted(TestName testName)
            {
                HandleTestOrSuiteStarted(testName);
            }

            void EventListener.SuiteFinished(TestSuiteResult nunitResult)
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

                TestLogWriterUtils.WriteException(testContext.LogWriter, LogStreamNames.Failures, exception, Resources.NUnitTestController_UnhandledExceptionSectionName);
            }

            private void HandleTestOrSuiteStarted(TestName testName)
            {
                ITestCommand testCommand;
                if (!testCommandsByTestName.TryGetValue(testName, out testCommand))
                    return;

                progressMonitor.SetStatus(testCommand.Test.Name);

                ITestStep parentTestStep = testContextStack.Count != 0 ? testContextStack.Peek().TestStep : topTestStep;
                ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
                testContextStack.Push(testContext);

                testContext.LifecyclePhase = LifecyclePhases.Execute;
            }

            private void HandleTestOrSuiteFinished(NUnitTestResult nunitResult)
            {
                if (testContextStack.Count == 0)
                    return;

                ITestContext testContext = testContextStack.Peek();
                NUnitTest test = (NUnitTest) testContext.TestStep.Test;
                if (test.Test.TestName != nunitResult.Test.TestName)
                    return;

                testContextStack.Pop();

                progressMonitor.Worked(1);

                string logStreamName = nunitResult.ResultState == ResultState.Success ? LogStreamNames.Warnings : LogStreamNames.Failures;

                ITestLogWriter logWriter = testContext.LogWriter;
                if (nunitResult.Message != null)
                {
                    logWriter.BeginSection(logStreamName, Resources.NUnitTestController_ResultMessageSectionName);
                    logWriter.Write(logStreamName, nunitResult.Message);
                    logWriter.EndSection(logStreamName);
                }
                if (nunitResult.StackTrace != null)
                {
                    logWriter.BeginSection(logStreamName, Resources.NUnitTestController_ResultStackTraceSectionName);
                    logWriter.Write(logStreamName, nunitResult.StackTrace);
                    logWriter.EndSection(logStreamName);
                }

                testContext.AddAssertCount(nunitResult.AssertCount);
                testContext.FinishStep(CreateOutcomeFromResult(nunitResult), null);
            }

            private static TestOutcome CreateOutcomeFromResult(NUnitTestResult nunitResult)
            {
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
            }
            #endregion

            #region ITestFilter Members
            bool ITestFilter.Pass(NUnit.Core.ITest test)
            {
                return FilterTest(test);
            }

            bool ITestFilter.Match(NUnit.Core.ITest test)
            {
                return FilterTest(test);
            }

            bool ITestFilter.IsEmpty
            {
                get { return false; }
            }

            private bool FilterTest(NUnit.Core.ITest test)
            {
                return testCommandsByTestName.ContainsKey(test.TestName);
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
