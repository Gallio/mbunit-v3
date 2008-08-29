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
            if (String.IsNullOrEmpty(assemblyLocation))
                throw new ArgumentNullException("assemblyLocation");

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

            private int failures;
            private Thread runnerThread;

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
                    runnerThread = Thread.CurrentThread;
                    failures = 0;

                    RunTests(assemblyPath);

                    return 0 == failures ? TestOutcome.Passed : TestOutcome.Failed;
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
                // Note: Make use of AppDomain
                //   This implementation does not make use of a separate 
                //   AppDomain to run the tests. Alternatively we can 
                //   create a new app domain and use the method
                //     RemoteLoader.CreateInstance( appDomain )
                //   to create the loader in that domain.
            }

            #region ITestListener Members

            public void OnAssemblyLoaded(object sender, AssemblyEventArgs args)
            {
            }

            public void OnAssemblyStarted(object sender, AssemblyEventArgs args)
            {
                ITestCommand testCommand;
                string testName = args.AssemblyFullName.Split(',')[0];
                if (testCommandsByName.TryGetValue(testName, out testCommand))
                {
                    progressMonitor.SetStatus(testCommand.Test.Name);

                    ITestContext testContext = testCommand.StartPrimaryChildStep(topTestStep);
                    testContext.LifecyclePhase = LifecyclePhases.Execute;
                    testContextStack.Push(testContext);
                }
            }

            public void OnTestsAborted(object sender, AssemblyEventArgs args)
            {
            }

            public void OnAssemblyFinished(object sender, AssemblyEventArgs args)
            {
                ITestCommand testCommand;
                string testName = args.AssemblyFullName.Split(',')[0];
                if (testCommandsByName.TryGetValue(testName, out testCommand))
                {
                    progressMonitor.Worked(1);

                    ITestContext testContext = testContextStack.Pop();
                    testContext.FinishStep(failures == 0 ? TestOutcome.Passed : TestOutcome.Failed, null);
                }
            }

            public void OnTestStarted(object sender, TestResultEventArgs args)
            {
                ITestCommand testCommand;
                string testName = args.ClassName + "." + args.MethodName;
                if (testCommandsByName.TryGetValue(testName, out testCommand))
                {
                    progressMonitor.SetStatus(args.MethodName);

                    ITestContext parentContext = testContextStack.Peek();
                    ITestContext testContext = testCommand.StartPrimaryChildStep(parentContext.TestStep);
                    testContext.LifecyclePhase = LifecyclePhases.Execute;
                    testContextStack.Push(testContext);
                }
            }

            public void OnTestPassed(object sender, TestResultEventArgs args)
            {
                string testName = args.ClassName + "." + args.MethodName;
                TestFinished(testName, TestOutcome.Passed, args.AssertCount, args.Duration, null);
            }

            public void OnTestFailed(object sender, TestResultEventArgs args)
            {
                Interlocked.Increment(ref failures);
                string testName = args.ClassName + "." + args.MethodName;
                TestFinished(testName, TestOutcome.Failed, args.AssertCount, args.Duration, 
                    delegate(ITestContext testContext)
                    {
                        if (args.Failure != null)
                        {
                            TestLogStreamWriter log = testContext.LogWriter.Failures;

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
            }

            public void OnTestError(object sender, TestResultEventArgs args)
            {
                string testName = args.ClassName + "." + args.MethodName;
                TestFinished(testName, TestOutcome.Error, args.AssertCount, args.Duration, 
                    delegate(ITestContext testContext)
                    {
                        if (!String.IsNullOrEmpty(args.Reason))
                        {
                            TestLogStreamWriter log = testContext.LogWriter.Failures;

                            using (log.BeginSection(Resources.CSUnitTestController_ResultMessageSectionName))
                            {
                                log.Write(args.Reason);
                            }
                        }                        
                    });
            }

            public void OnTestSkipped(object sender, TestResultEventArgs args)
            {
                ITestCommand testCommand;
                string testName = args.ClassName + "." + args.MethodName;
                if (testCommandsByName.TryGetValue(testName, out testCommand))
                {
                    progressMonitor.SetStatus(args.MethodName);

                    ITestContext parentContext = testContextStack.Peek();
                    ITestContext testContext = testCommand.StartPrimaryChildStep(parentContext.TestStep);
                    testContext.LifecyclePhase = LifecyclePhases.Execute;

                    progressMonitor.Worked(1);

                    testContext.AddAssertCount(args.AssertCount);
                    testContext.FinishStep(TestOutcome.Ignored, null);
                }
            }

            #endregion

            private delegate void TestContextWorker(ITestContext context);

            private void TestFinished(string testName, TestOutcome outcome, int assertCount, ulong duration_nanosec, TestContextWorker worker)
            {
                ITestCommand testCommand;
                if (testCommandsByName.TryGetValue(testName, out testCommand))
                {
                    progressMonitor.Worked(1);

                    TimeSpan? duration = null;
                    if (duration_nanosec > 0)
                    {
                        // A tick is equal to 100 nanoseconds
                        duration = TimeSpan.FromTicks((long)(duration_nanosec / 100UL));
                    }

                    ITestContext testContext = testContextStack.Pop();

                    if (worker != null)
                        worker(testContext);

                    testContext.AddAssertCount(assertCount);
                    testContext.FinishStep(outcome, duration);
                }
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
