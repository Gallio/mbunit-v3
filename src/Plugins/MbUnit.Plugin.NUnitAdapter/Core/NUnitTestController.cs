using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Results;
using MbUnit.Framework.Services.ExecutionLogs;
using NUnit.Core;

using ITest = MbUnit.Framework.Kernel.Model.ITest;
using TestResult = MbUnit.Framework.Kernel.Results.TestResult;

namespace MbUnit.Plugin.NUnitAdapter.Core
{
    /// <summary>
    /// Controls the execution of NUnit tests.
    /// </summary>
    public class NUnitTestController : ITestController
    {
        private bool aborted;
        private TestRunner runner;
        private RunMonitor monitor;

        /// <summary>
        /// Creates a test controller.
        /// </summary>
        /// <param name="runner">The test runner</param>
        public NUnitTestController(TestRunner runner)
        {
            this.runner = runner;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            lock (this)
            {
                runner = null;
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
                        return; // TODO: Should we mark tests aborted or something?

                    monitor = new RunMonitor(runner, options, listener, tests);
                }

                monitor.Run();
            }
            finally
            {
                monitor = null;
            }
        }

        /// <inheritdoc />
        public void Abort()
        {
            lock (this)
            {
                ThrowIfDisposed();

                aborted = true;

                if (monitor != null)
                    monitor.Abort();
            }
        }

        private void ThrowIfDisposed()
        {
            if (runner == null)
                throw new ObjectDisposedException("The test controller has been disposed.");
        }

        private class RunMonitor : EventListener, ITestFilter
        {
            private bool aborted;
            private TestRunner runner;
            private TestExecutionOptions options;
            private IEventListener listener;
            private IList<ITest> tests;

            private Dictionary<TestName, NUnitTest> testsByTestName;
            private Stack<NUnitTest> testStack;

            public RunMonitor(TestRunner runner,
                TestExecutionOptions options, IEventListener listener, IList<ITest> tests)
            {
                this.runner = runner;
                this.options = options;
                this.listener = listener;
                this.tests = tests;

                Initialize();
            }

            public void Run()
            {
                runner.Run(this, this);
            }

            public void Abort()
            {
                aborted = true;
                runner.CancelRun();
            }

            private void Initialize()
            {
                // Build a reverse mapping from NUnit tests.
                testsByTestName = new Dictionary<TestName, NUnitTest>();
                foreach (NUnitTest test in tests)
                {
                    if (test.Test != null)
                        testsByTestName[test.Test.TestName] = test;
                }

                testStack = new Stack<NUnitTest>();
            }

            #region EventListener Members
            void EventListener.RunStarted(string name, int testCount)
            {
                // Note: Must handle a possible race condition involving Abort wherein the
                //       cancelation may not occur if we abort too soon.
                if (aborted)
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        runner.CancelRun();
                    });
                }
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

            void EventListener.TestFinished(TestCaseResult nunitResult)
            {
                HandleTestOrSuiteFinished(nunitResult);
            }

            void EventListener.TestOutput(TestOutput testOutput)
            {
                if (testStack.Count == 0)
                    return;

                NUnitTest test = testStack.Peek();

                string streamName;
                switch (testOutput.Type)
                {
                    case TestOutputType.Out:
                        streamName = ExecutionLogStreams.ConsoleOutput;
                        break;
                    case TestOutputType.Error:
                        streamName = ExecutionLogStreams.ConsoleError;
                        break;
                    case TestOutputType.Trace:
                        streamName = ExecutionLogStreams.Trace;
                        break;
                    default:
                        streamName = "NUnit TestOutputType(" + testOutput.Type + ")";
                        break;
                }

                listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateWriteTextEvent(test.Id, streamName, testOutput.Text));
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
                if (testStack.Count == 0)
                    return;

                NUnitTest test = testStack.Peek();

                listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateBeginSectionEvent(test.Id, ExecutionLogStreams.Failures, "Unhandled Exception"));
                listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateWriteTextEvent(test.Id, ExecutionLogStreams.Failures, exception.ToString()));
                listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateEndSectionEvent(test.Id, ExecutionLogStreams.Failures));
            }

            private void HandleTestOrSuiteStarted(TestName testName)
            {
                NUnitTest test;
                if (!testsByTestName.TryGetValue(testName, out test))
                    return;

                testStack.Push(test);
                listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateStartEvent(test.Id));
            }

            private void HandleTestOrSuiteFinished(NUnit.Core.TestResult nunitResult)
            {
                NUnitTest test;
                if (!testsByTestName.TryGetValue(nunitResult.Test.TestName, out test))
                    return;

                if (nunitResult.Message != null)
                {
                    listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateBeginSectionEvent(test.Id, ExecutionLogStreams.Failures, "Failure Message"));
                    listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateWriteTextEvent(test.Id, ExecutionLogStreams.Failures, nunitResult.Message));
                    listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateEndSectionEvent(test.Id, ExecutionLogStreams.Failures));
                }
                if (nunitResult.StackTrace != null)
                {
                    listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateBeginSectionEvent(test.Id, ExecutionLogStreams.Failures, "Failure Stack Trace"));
                    listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateWriteTextEvent(test.Id, ExecutionLogStreams.Failures, nunitResult.StackTrace));
                    listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateEndSectionEvent(test.Id, ExecutionLogStreams.Failures));
                }

                listener.NotifyTestExecutionLogEvent(TestExecutionLogEventArgs.CreateCloseEvent(test.Id));

                TestResult result = CreateTestResultFromNUnitTestResult(nunitResult);

                testStack.Pop();
                listener.NotifyTestLifecycleEvent(TestLifecycleEventArgs.CreateFinishEvent(test.Id, result));
            }

            private TestResult CreateTestResultFromNUnitTestResult(NUnit.Core.TestResult nunitResult)
            {
                TestResult result = new TestResult();
                result.Duration = TimeSpan.FromSeconds(nunitResult.Time);

                switch (nunitResult.ResultState)
                {
                    case ResultState.Success:
                        result.Outcome = TestOutcome.Passed;
                        break;

                    case ResultState.Failure:
                    case ResultState.Error:
                        result.Outcome = TestOutcome.Failed;
                        break;
                }

                switch (nunitResult.RunState)
                {
                    case RunState.Executed:
                        result.State = TestState.Completed;
                        break;

                    case RunState.Skipped:
                    case RunState.Explicit:
                        result.State = TestState.Skipped;
                        result.Outcome = TestOutcome.Inconclusive;
                        break;

                    case RunState.Ignored:
                        result.State = TestState.Ignored;
                        break;

                    case RunState.NotRunnable:
                    case RunState.Runnable:
                        result.State = TestState.NotRun;
                        break;
                }

                return result;
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
                return testsByTestName.ContainsKey(test.TestName);
            }
            #endregion
        }
    }
}
