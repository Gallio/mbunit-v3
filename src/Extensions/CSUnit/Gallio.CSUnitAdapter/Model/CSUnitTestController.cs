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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using csUnit.Core;
using csUnit.Interfaces;
using Gallio.Common.Collections;
using Gallio.CSUnitAdapter.Properties;
using Gallio.Model;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using Gallio.Model.Environments;
using Gallio.Runtime;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Common.Remoting;
using ITestListener=csUnit.Interfaces.ITestListener;

namespace Gallio.CSUnitAdapter.Model
{
    internal class CSUnitTestController : TestController
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
        protected override TestResult RunImpl(ITestCommand rootTestCommand, TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            IList<ITestCommand> testCommands = rootTestCommand.GetAllCommands();
            using (progressMonitor.BeginTask(Resources.CSUnitTestController_RunningCSUnitTests, testCommands.Count))
            {
                if (progressMonitor.IsCanceled)
                {
                    return new TestResult(TestOutcome.Canceled);
                }

                if (options.SkipTestExecution)
                {
                    return SkipAll(rootTestCommand, parentTestStep);
                }

                using (RunnerMonitor monitor = new RunnerMonitor(testCommands, parentTestStep, progressMonitor))
                {
                    return monitor.Run(assemblyLocation);
                }
            }            
        }

        public class RunnerMonitor : LongLivedMarshalByRefObject, ITestListener, IDisposable
        {
            private readonly IProgressMonitor progressMonitor;
            private readonly TestStep topTestStep;

            private readonly Stack<ITestContext> testContextStack;
            private readonly Dictionary<string, ITestCommand> testCommandsByName;

            private IList<ITestCommand> listOfTestCommands;
            private Thread runnerThread;

            private int assemblyFailureCount;
            private int fixtureFailureCount;
            private int assemblyErrorCount;
            private int fixtureErrorCount;

            private TestResult topResult;

            public RunnerMonitor(IList<ITestCommand> testCommands, TestStep topTestStep, IProgressMonitor progressMonitor)
            {
                if (topTestStep == null)
                    throw new ArgumentNullException("topTestStep");
                if (progressMonitor == null)
                    throw new ArgumentNullException("progressMonitor");

                this.progressMonitor = progressMonitor;
                this.topTestStep = topTestStep;

                testContextStack = new Stack<ITestContext>();
                testCommandsByName = new Dictionary<string, ITestCommand>();

                Initialize(testCommands);

                progressMonitor.Canceled += Canceled;
            }

            public void Initialize(IList<ITestCommand> testCommands)
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

            public bool IncludesTest(string testName)
            {
                return testCommandsByName.ContainsKey(testName);
            }

            public void Canceled(object sender, EventArgs e)
            {
                if (runnerThread != null)
                {
                    runnerThread.Abort();
                }
            }

            public TestResult Run(string assemblyPath)
            {
                try
                {
                    // Save the thread to allow us to abort later
                    runnerThread = Thread.CurrentThread;

                    return RunTests(assemblyPath);
                }
                catch (ThreadAbortException)
                {
                    if (progressMonitor.IsCanceled)
                    {
                        Thread.ResetAbort();
                        return new TestResult(TestOutcome.Canceled);
                    }
                    return new TestResult(TestOutcome.Error);
                }
                finally
                {
                    runnerThread = null;
                }
            }

            private TestResult RunTests(string assemblyPath)
            {
                AssemblyMetadata assemblyMetadata = AssemblyUtils.GetAssemblyMetadata(assemblyPath, AssemblyMetadataFields.Default);
                if (assemblyMetadata == null)
                {
                    ITestContext testContext = listOfTestCommands[0].StartPrimaryChildStep(topTestStep);
                    testContext.LifecyclePhase = LifecyclePhases.Execute;
                    testContext.LogWriter.Failures.WriteLine("Test assembly does not exist or is not a valid .Net assembly. [{0}]", assemblyPath ?? String.Empty);
                    return testContext.FinishStep(TestOutcome.Error, null);
                }

                // Remark: We cannot use the RemoteLoader directly from this AppDomain.
                //   csUnit v2.5 contains a bug in its detection of the NUnitAdapter.  It tries
                //   to enumerate ALL types in ALL assemblies that are loaded in the AppDomain.
                //   Bad news for us because some of these types derived from other types in
                //   assemblies that cannot be loaded (eg. VisualStudio APIs).
                //   So csUnit promptly blows up.  The workaround is to create our own AppDomain.
                //   We cannot use the csUnit Loader because it does things like report
                //   events asynchronously and possibly out of order or even in parallel. -- Jeff.
                // See also: http://sourceforge.net/tracker/index.php?func=detail&aid=2111390&group_id=23919&atid=380010
                HostSetup hostSetup = new HostSetup();
                hostSetup.ApplicationBaseDirectory = Path.GetDirectoryName(assemblyPath);
                hostSetup.WorkingDirectory = hostSetup.ApplicationBaseDirectory;
                hostSetup.ShadowCopy = true;
                hostSetup.ConfigurationFileLocation = ConfigurationFileLocation.AppBase;
                hostSetup.ProcessorArchitecture = assemblyMetadata.ProcessorArchitecture;

                string configFile = assemblyPath + ".config";
                if (File.Exists(configFile))
                    hostSetup.Configuration.ConfigurationXml = File.ReadAllText(configFile);

                var hostFactory = (IHostFactory)RuntimeAccessor.ServiceLocator.ResolveByComponentId(IsolatedAppDomainHostFactory.ComponentId);
                using (IHost host = hostFactory.CreateHost(hostSetup, RuntimeAccessor.Logger))
                {
                    HostAssemblyResolverHook.InstallCallback(host);

                    Type loaderType = typeof(RemoteLoader);

                    using (RemoteLoader loader = (RemoteLoader) host.GetHostService().CreateInstance(
                        loaderType.Assembly.FullName,
                        loaderType.FullName).Unwrap())
                    {
                        // Attach ourself to get feedback
                        loader.Listener = this;

                        // Load the test assembly
                        loader.LoadAssembly(assemblyPath);

                        // Run the tests of that assembly
                        TextWriter consoleOutputWriter = new ContextualLogTextWriter(MarkupStreamNames.ConsoleOutput);
                        var spec = new CallbackTestSpec(this);
                        loader.RunTests(spec, consoleOutputWriter);
                    }
                }

                return topResult ?? new TestResult(TestOutcome.Error);
            }

            #region ITestListener Members

            void ITestListener.OnAssemblyLoaded(object sender, AssemblyEventArgs args)
            {
            }

            void ITestListener.OnAssemblyStarted(object sender, AssemblyEventArgs args)
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

            void ITestListener.OnTestsAborted(object sender, AssemblyEventArgs args)
            {
            }

            void ITestListener.OnAssemblyFinished(object sender, AssemblyEventArgs args)
            {
                ITestCommand testCommand;
                string testName = args.AssemblyFullName.Split(',')[0];
                if (!testCommandsByName.TryGetValue(testName, out testCommand))
                    return;

                if (testContextStack.Count == 0)
                    return;

                ITestContext testContext = testContextStack.Pop();
                
                while (testContext.TestStep.Test != testCommand.Test)
                {
                    testContext.FinishStep(GetFixtureOutcome(fixtureFailureCount, fixtureErrorCount), null);
                    testContext = testContextStack.Pop();

                    progressMonitor.Worked(1);
                }

                topResult = testContext.FinishStep(CalculateOutcome(assemblyFailureCount, assemblyErrorCount), null);

                progressMonitor.Worked(1);
            }            

            void ITestListener.OnTestStarted(object sender, TestResultEventArgs args)
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

            void ITestListener.OnTestPassed(object sender, TestResultEventArgs args)
            {
                TestFinished(TestOutcome.Passed, args.ClassName, args.MethodName, args.AssertCount, args.Duration, args.Reason, null);
            }

            void ITestListener.OnTestFailed(object sender, TestResultEventArgs args)
            {
                TestFinished(TestOutcome.Failed, args.ClassName, args.MethodName, args.AssertCount, args.Duration, args.Reason,
                    delegate(ITestContext context)
                    {
                        if (args.Failure != null)
                        {
                            MarkupStreamWriter log = context.LogWriter.Failures;

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

            void ITestListener.OnTestError(object sender, TestResultEventArgs args)
            {
                TestFinished(TestOutcome.Error, args.ClassName, args.MethodName, args.AssertCount, args.Duration, args.Reason,
                    delegate(ITestContext context)
                    {
                        if (!String.IsNullOrEmpty(args.Reason))
                        {
                            MarkupStreamWriter log = context.LogWriter.Failures;

                            using (log.BeginSection(Resources.CSUnitTestController_ResultMessageSectionName))
                            {
                                log.Write(args.Reason);
                            }
                        }
                    });
                Interlocked.Increment(ref fixtureErrorCount);
            }

            void ITestListener.OnTestSkipped(object sender, TestResultEventArgs args)
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

                    progressMonitor.Worked(1);
                }
                else if (!String.IsNullOrEmpty(reason))
                {
                    MarkupStreamWriter log = fixtureContext.LogWriter.Failures;

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
                    while (parentContext.TestStep.Test.Kind != CSUnitTestExplorer.AssemblyKind)
                    {
                        testContextStack.Pop();

                        TestOutcome outcome = GetFixtureOutcome(fixtureFailureCount, fixtureErrorCount);

                        parentContext.FinishStep(outcome, null);
                        progressMonitor.Worked(1);

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
        private sealed class CallbackTestSpec : ITestSpec
        {
            private readonly RunnerMonitor runnerMonitor;

            public CallbackTestSpec(RunnerMonitor runnerMonitor)
            {
                this.runnerMonitor = runnerMonitor;
            }

            bool ITestSpec.Includes(ITestFixture testFixture)
            {
                return runnerMonitor.IncludesTest(testFixture.FullName);
            }

            bool ITestSpec.Includes(ITestMethod testMethod)
            {
                return runnerMonitor.IncludesTest(testMethod.FullName);
            }

            void ITestSpec.Clear()
            {
                throw new NotSupportedException();
            }

            bool ITestSpec.Empty
            {
                get { throw new NotSupportedException(); }
            }

            bool ITestSpec.IsAssemblyConfigured(string assemblyName)
            {
                throw new NotSupportedException();
            }

            bool ITestSpec.IsFixtureConfigured(string assemblyName, string fixtureFullName)
            {
                throw new NotSupportedException();
            }

            bool ITestSpec.IsTestConfigured(string assemblyName, string fixtureFullName, string methodName)
            {
                throw new NotSupportedException();
            }

            csUnit.Set<ISelector> ITestSpec.Selectors
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            TestRunKind ITestSpec.TestRunKind
            {
                get { throw new NotSupportedException(); }
            }
        }
    }
}
