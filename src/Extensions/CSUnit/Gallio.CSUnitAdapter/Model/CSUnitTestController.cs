// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Threading;
using csUnit.Core;
using csUnit.Interfaces;
using Gallio.CSUnitAdapter.Properties;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.CSUnitAdapter.Model
{
    internal class CSUnitTestController : BaseTestController
    {
        private readonly string assemblyLocation;

        /// <summary>
        /// Create a test controller
        /// </summary>
        public CSUnitTestController(string assemblyLocation)
        {
            this.assemblyLocation = assemblyLocation;
        }

        /// <inheritdoc />
        protected override TestOutcome RunTestsImpl(ITestCommand rootTestCommand, ITestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            IList<ITestCommand> testCommands = rootTestCommand.GetAllCommands();
            using (progressMonitor.BeginTask(Resources.CSUnitTestController_RunningCSUnitTests, testCommands.Count))
            {
                if (progressMonitor.IsCanceled)
                {
                    return TestOutcome.Canceled;
                }

                if (options.SkipTestExecution)
                {
                    SkipAll(rootTestCommand, parentTestStep);
                    return TestOutcome.Skipped;
                }

                using (RunnerMonitor monitor = new RunnerMonitor(testCommands, parentTestStep, progressMonitor))
                {
                    return monitor.Run(assemblyLocation);
                }
            }            
        }

        public class RunnerMonitor : csUnit.Interfaces.ITestListener, IDisposable
        {
            private readonly IProgressMonitor progressMonitor;
            private readonly ITestStep topTestStep;

            private readonly Stack<ITestContext> testContextStack;
            private readonly Dictionary<string, ITestCommand> testCommandsByName;

            private IList<ITestCommand> listOfTestCommands;
            private Thread runnerThread;

            private int assemblyFailureCount;
            private int fixtureFailureCount;
            private int assemblyErrorCount;
            private int fixtureErrorCount;

            public RunnerMonitor(IList<ITestCommand> testCommands, ITestStep topTestStep, IProgressMonitor progressMonitor)
            {
                if (topTestStep == null)
                    throw new ArgumentNullException("topTestStep");
                if (progressMonitor == null)
                    throw new ArgumentNullException("progressMonitor");

                this.progressMonitor = progressMonitor;
                this.topTestStep = topTestStep;

                testContextStack = new Stack<ITestContext>();
                testCommandsByName = new Dictionary<string, ITestCommand>();

                Initialise(testCommands);

                progressMonitor.Canceled += Canceled;
            }

            public void Initialise(IList<ITestCommand> testCommands)
            {
                if (testCommands == null)
                    throw new ArgumentNullException("testCommands");

                runnerThread = null;

                testContextStack.Clear();
                testCommandsByName.Clear();

                listOfTestCommands = testCommands;

                // Build a reverse mapping from the tests.
                foreach (ITestCommand testCommand in testCommands)
                {
                    testCommandsByName[testCommand.Test.LocalId] = testCommand;
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                progressMonitor.Canceled -= Canceled;
            }

            #endregion

            public void Canceled(object sender, EventArgs e)
            {
                if (runnerThread != null)
                {
                    runnerThread.Abort();
                }
            }

            public TestOutcome Run(string assemblyPath)
            {
                try
                {
                    // Save the thread to allow us to abort later
                    runnerThread = Thread.CurrentThread;

                    RunTests(assemblyPath);

                    return CalculateOutcome(assemblyFailureCount, assemblyErrorCount);
                }
                catch (ThreadAbortException)
                {
                    if (progressMonitor.IsCanceled)
                    {
                        Thread.ResetAbort();
                        return TestOutcome.Canceled;
                    }
                    return TestOutcome.Error;
                }
                finally
                {
                    runnerThread = null;
                }
            }

            private void RunTests(string assemblyPath)
            {
                if (!File.Exists(assemblyPath))
                {
                    ITestContext testContext = listOfTestCommands[0].StartPrimaryChildStep(topTestStep);
                    testContext.LifecyclePhase = LifecyclePhases.Execute;
                    testContext.LogWriter.Failures.WriteLine("Unable to find the test assembly to run. [{0}]", assemblyPath ?? String.Empty);
                    testContext.FinishStep(TestOutcome.Error, null);
                    return;
                }
                using (RemoteLoader loader = new RemoteLoader())
                {
                    // Attach ourself to get feedback
                    loader.Listener = this;

                    // Load the test assembly
                    loader.LoadAssembly(assemblyPath);

                    // Run the tests of that assembly
                    ITestSpec testSpec = new PartialTestSpec(testCommandsByName);
                    loader.RunTests(testSpec, TextWriter.Null);
                }
            }

            #region ITestListener Members

            public void OnAssemblyLoaded(object sender, AssemblyEventArgs args)
            {
            }

            public void OnAssemblyStarted(object sender, AssemblyEventArgs args)
            {
                ITestCommand testCommand;
                string testName = args.AssemblyFullName.Split(',')[0];
                if (!testCommandsByName.TryGetValue(testName, out testCommand))
                    return;

                progressMonitor.SetStatus(testCommand.Test.Name);

                ITestContext testContext = testCommand.StartPrimaryChildStep(topTestStep);
                testContext.LifecyclePhase = LifecyclePhases.Execute;
                testContextStack.Push(testContext);
                assemblyFailureCount = 0;
                assemblyErrorCount = 0;
            }

            public void OnTestsAborted(object sender, AssemblyEventArgs args)
            {
            }

            public void OnAssemblyFinished(object sender, AssemblyEventArgs args)
            {
                ITestCommand testCommand;
                string testName = args.AssemblyFullName.Split(',')[0];
                if (!testCommandsByName.TryGetValue(testName, out testCommand))
                    return;

                if (testContextStack.Count <= 0)
                    return;

                ITestContext testContext = testContextStack.Pop();

                
                while (testContext.TestStep.Test != testCommand.Test)
                {
                    progressMonitor.Worked(1);

                    testContext.FinishStep(GetFixtureOutcome(fixtureFailureCount, fixtureErrorCount), null);
                    testContext = testContextStack.Pop();
                }

                progressMonitor.Worked(1);
                testContext.FinishStep(CalculateOutcome(assemblyFailureCount, assemblyErrorCount), null);
            }            

            public void OnTestStarted(object sender, TestResultEventArgs args)
            {
                ITestCommand fixtureCommand;
                if (!testCommandsByName.TryGetValue(args.ClassName, out fixtureCommand))
                    return;

                ITestCommand testCommand;
                string testName = args.ClassName + @"." + args.MethodName;
                if (!testCommandsByName.TryGetValue(testName, out testCommand))
                    return;

                ITestContext fixtureContext = GetFixtureContext(fixtureCommand);
                ITestContext testContext = testCommand.StartPrimaryChildStep(fixtureContext.TestStep);
                testContext.LifecyclePhase = LifecyclePhases.Execute;
                progressMonitor.SetStatus(testCommand.Test.Name);
                testContextStack.Push(testContext);
            }

            public void OnTestPassed(object sender, TestResultEventArgs args)
            {
                TestFinished(TestOutcome.Passed, args.ClassName, args.MethodName, args.AssertCount, args.Duration, args.Reason, null);
            }

            public void OnTestFailed(object sender, TestResultEventArgs args)
            {
                TestFinished(TestOutcome.Failed, args.ClassName, args.MethodName, args.AssertCount, args.Duration, args.Reason,
                    delegate(ITestContext context)
                    {
                        if (args.Failure != null)
                        {
                            TestLogStreamWriter log = context.LogWriter.Failures;

                            using (log.BeginSection(Resources.CSUnitTestController_ResultMessageSectionName))
                            {
                                if (!String.IsNullOrEmpty(args.Failure.Expected))
                                    log.WriteLine(args.Failure.Expected);
                                if (!String.IsNullOrEmpty(args.Failure.Actual))
                                    log.WriteLine(args.Failure.Actual);
                                if (!String.IsNullOrEmpty(args.Failure.Message))
                                    log.WriteLine(args.Failure.Message);
                            }

                            using (log.BeginSection(Resources.CSUnitTestController_ResultStackTraceSectionName))
                            {
                                using (log.BeginMarker(Marker.StackTrace))
                                {
                                    log.Write(args.Failure.StackTrace);
                                }
                            }
                        }
                    });
                Interlocked.Increment(ref fixtureFailureCount);
            }

            public void OnTestError(object sender, TestResultEventArgs args)
            {
                TestFinished(TestOutcome.Error, args.ClassName, args.MethodName, args.AssertCount, args.Duration, args.Reason,
                    delegate(ITestContext context)
                    {
                        if (!String.IsNullOrEmpty(args.Reason))
                        {
                            TestLogStreamWriter log = context.LogWriter.Failures;

                            using (log.BeginSection(Resources.CSUnitTestController_ResultMessageSectionName))
                            {
                                log.Write(args.Reason);
                            }
                        }
                    });
                Interlocked.Increment(ref fixtureErrorCount);
            }

            public void OnTestSkipped(object sender, TestResultEventArgs args)
            {
                TestFinished(TestOutcome.Skipped, args.ClassName, args.MethodName, args.AssertCount, args.Duration, args.Reason, null);
            }

            #endregion

            private delegate void TestContextCallback(ITestContext context);

            private void TestFinished(TestOutcome outcome, string fixtureName, string testName, int assertCount, ulong durationNanosec, string reason, TestContextCallback callback)
            {
                ITestCommand fixtureCommand;
                if (!testCommandsByName.TryGetValue(fixtureName, out fixtureCommand))
                    return;

                ITestCommand testCommand;
                ITestContext testContext = null;
                if (testCommandsByName.TryGetValue(fixtureName + @"." + testName, out testCommand))
                {
                    if (testContextStack.Peek().TestStep.Test == testCommand.Test)
                    {
                        // Remove our test context from the stack
                        testContext = testContextStack.Pop();
                    }
                }

                ITestContext fixtureContext = GetFixtureContext(fixtureCommand);

                if (testCommand != null)
                {
                    if (testContext == null)
                    {
                        testContext = testCommand.StartPrimaryChildStep(fixtureContext.TestStep);
                        testContext.LifecyclePhase = LifecyclePhases.Execute;
                        progressMonitor.SetStatus(testCommand.Test.Name);
                    }

                    progressMonitor.Worked(1);

                    TimeSpan? duration = null;
                    if (durationNanosec > 0)
                    {
                        // A tick is equal to 100 nanoseconds
                        duration = TimeSpan.FromTicks((long)(durationNanosec / 100UL));
                    }

                    if (callback != null)
                        callback(testContext);

                    testContext.AddAssertCount(assertCount);
                    testContext.FinishStep(outcome, duration);
                }
                else if (!String.IsNullOrEmpty(reason))
                {
                    TestLogStreamWriter log = fixtureContext.LogWriter.Failures;

                    using (log.BeginSection(Resources.CSUnitTestController_ResultMessageSectionName))
                    {
                        log.Write(reason);
                    }
                }
            }

            private ITestContext GetFixtureContext(ITestCommand fixtureCommand)
            {
                ITestContext parentContext = testContextStack.Peek();
                if (parentContext.TestStep.Test != fixtureCommand.Test)
                {
                    while (((BaseTest)parentContext.TestStep.Test).Kind != "Assembly")
                    {
                        testContextStack.Pop();

                        TestOutcome outcome = GetFixtureOutcome(fixtureFailureCount, fixtureErrorCount);

                        progressMonitor.Worked(1);
                        parentContext.FinishStep(outcome, null);

                        parentContext = testContextStack.Peek();
                    }
                    
                    parentContext = fixtureCommand.StartPrimaryChildStep(parentContext.TestStep);
                    parentContext.LifecyclePhase = LifecyclePhases.Execute;
                    progressMonitor.SetStatus(fixtureCommand.Test.Name);
                    
                    testContextStack.Push(parentContext);                    
                    
                    fixtureFailureCount = 0;
                    fixtureErrorCount = 0;
                }
                return parentContext;
            }

            private TestOutcome GetFixtureOutcome(int failureCount, int errorCount)
            {
                TestOutcome outcome;

                if (errorCount > 0)
                {
                    Interlocked.Add(ref assemblyErrorCount, errorCount);
                    outcome = TestOutcome.Error;
                }
                else if (failureCount > 0)
                {
                    Interlocked.Add(ref assemblyFailureCount, errorCount);
                    outcome = TestOutcome.Failed;
                }
                else
                {
                    outcome = TestOutcome.Passed;
                }

                return outcome;
            }

            private static TestOutcome CalculateOutcome(int failureCount, int errorCount)
            {
                TestOutcome outcome;

                if (errorCount > 0)
                    outcome = TestOutcome.Error;
                else if (failureCount > 0)
                    outcome = TestOutcome.Failed;
                else
                    outcome = TestOutcome.Passed;

                return outcome;
            }
        }

        [Serializable]
        private class PartialTestSpec : ITestSpec
        {
            private readonly Dictionary<string, ITestCommand> testCommands;

            public PartialTestSpec(Dictionary<string, ITestCommand> testCommands)
            {
                if (testCommands == null)
                    throw new ArgumentNullException("testCommands");

                this.testCommands = testCommands;
            }

            #region ITestSpec Members

            public bool Includes(ITestFixture testFixture)
            {
                return testCommands.ContainsKey(testFixture.FullName);
            }

            public bool Includes(ITestMethod testMethod)
            {
                return testCommands.ContainsKey(testMethod.FullName);
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Empty
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsAssemblyConfigured(string assemblyName)
            {
                throw new NotImplementedException();
            }

            public bool IsFixtureConfigured(string assemblyName, string fixtureFullName)
            {
                throw new NotImplementedException();
            }

            public bool IsTestConfigured(string assemblyName, string fixtureFullName, string methodName)
            {
                throw new NotImplementedException();
            }

            public csUnit.Set<ISelector> Selectors
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public TestRunKind TestRunKind
            {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }
    }
}
