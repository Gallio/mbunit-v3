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
using System.Xml;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Model;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Tree;
using Gallio.MSTestAdapter.Model;
using Gallio.MSTestAdapter.Properties;
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Common.Markup;

namespace Gallio.MSTestAdapter.Wrapper
{
    internal abstract class MSTestRunner
    {
        internal const string PreferredTestDir = "TestDir";

        protected static readonly Guid RootTestListGuid = new Guid("8c43106b-9dc1-4907-a29f-aa66a61bf5b6");
        protected static readonly Guid SelectedTestListGuid = new Guid("05EF261C-0065-4c5f-9DE3-3D068277A643");
        protected const string SelectedTestListName = "SelectedTests";

        public static MSTestRunner GetRunnerForFrameworkVersion(Version frameworkVersion)
        {
            if (frameworkVersion.Major == 8 && frameworkVersion.Minor == 0)
                return new MSTestRunner2005();
            if (frameworkVersion.Major == 9 && frameworkVersion.Minor == 0)
                return new MSTestRunner2008();
            if (frameworkVersion.Major == 10 && frameworkVersion.Minor == 0)
                return new MSTestRunner2010();

            throw new NotSupportedException(string.Format("MSTest v{0}.{1} is not supported at this time.", frameworkVersion.Major, frameworkVersion.Minor));
        }

        public TestOutcome RunSession(ITestContext assemblyTestContext, MSTestAssembly assemblyTest,
            ITestCommand assemblyTestCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            DirectoryInfo tempDir = SpecialPathPolicy.For("MSTestAdapter").CreateTempDirectoryWithUniqueName();
            try
            {
                // Set the test results path.  Among other things, the test results path
                // will determine where the deployed test files go.
                string testResultsPath = Path.Combine(tempDir.FullName, "tests.trx");

                // Set the test results root directory.
                // This path determines both where MSTest searches for test files which
                // is used to resolve relative paths to test files in the "*.vsmdi" file.
                string searchPathRoot = Path.GetDirectoryName(assemblyTest.AssemblyFilePath);

                // Set the test metadata and run config paths.  These are just temporary
                // files that can go anywhere on the filesystem.  It happens to be convenient
                // to store them in the same temporary directory as the test results.
                string testMetadataPath = Path.Combine(tempDir.FullName, "tests.vsmdi");
                string runConfigPath = Path.Combine(tempDir.FullName, "tests.runconfig");

                progressMonitor.SetStatus("Generating test metadata file.");
                CreateTestMetadataFile(testMetadataPath,
                    GetTestsFromCommands(assemblyTestCommand.PreOrderTraversal), assemblyTest.AssemblyFilePath);

                progressMonitor.SetStatus("Generating run config file.");
                CreateRunConfigFile(runConfigPath);

                progressMonitor.SetStatus("Executing tests.");
                Executor executor = new Executor(this, assemblyTestContext, assemblyTestCommand);
                TestOutcome outcome = executor.Execute(testMetadataPath, testResultsPath,
                    runConfigPath, searchPathRoot);
                return outcome;
            }
            finally
            {
                try
                {
                    tempDir.Delete(true);
                }
                catch
                {
                    // Ignore I/O exceptions deleting temporary files.
                    // They will probably be deleted by the OS later on during a file cleanup.
                }
            }
        }

        protected abstract string GetVisualStudioVersion();

        protected abstract void WriteTestMetadata(XmlWriter writer, IEnumerable<MSTest> tests, string assemblyFilePath);

        protected abstract void WriteRunConfig(XmlWriter writer);

        /*
        [DebuggerNonUserCode]
        private static string GuessSearchPathRootFromDeploymentItems(MSTestAssembly assemblyTest)
        {
            string assemblyDirPath = Path.GetFullPath(Path.GetDirectoryName(assemblyTest.AssemblyFilePath));
            HashSet<string> deploymentItemSourcePaths = new HashSet<string>();

            GetAllDeploymentItemSourcePaths(deploymentItemSourcePaths, assemblyTest);
            if (deploymentItemSourcePaths.Count == 0)
                return assemblyDirPath;

            Dictionary<string, int> dirVotes = new Dictionary<string,int>();

            List<string> candidateDirPaths = new List<string>();
            for (string currentDirPath = assemblyDirPath; ! string.IsNullOrEmpty(currentDirPath); currentDirPath = Path.GetDirectoryName(currentDirPath))
                candidateDirPaths.Add(currentDirPath);

            foreach (string deploymentItemSourcePath in deploymentItemSourcePaths)
            {
                string expandedItemSourcePath = Environment.ExpandEnvironmentVariables(deploymentItemSourcePath);

                foreach (string candidateDirPath in candidateDirPaths)
                {
                    try
                    {
                        string candidateItemSourcePath = Path.Combine(candidateDirPath, expandedItemSourcePath);
                        if (File.Exists(candidateItemSourcePath) || Directory.Exists(candidateItemSourcePath))
                        {
                            int currentVote;
                            dirVotes.TryGetValue(candidateDirPath, out currentVote);
                            dirVotes[candidateDirPath] = currentVote + 1;
                        }
                    }
                    catch
                    {
                        // Ignore deployment item that could not be located.
                    }
                }
            }

            int bestVote = 0;
            string bestDir = assemblyDirPath;
            foreach (var vote in dirVotes)
            {
                if (vote.Value > bestVote)
                {
                    bestVote = vote.Value;
                    bestDir = vote.Key;
                }
            }

            return bestDir;
        }

        private static void GetAllDeploymentItemSourcePaths(HashSet<string> deploymentItemSourcePaths, MSTest test)
        {
            foreach (MSTestDeploymentItem deploymentItem in test.DeploymentItems)
            {
                deploymentItemSourcePaths.Add(deploymentItem.SourcePath);
            }

            foreach (MSTest childTest in test.Children)
                GetAllDeploymentItemSourcePaths(deploymentItemSourcePaths, childTest);
        }
         */

        private static IEnumerable<MSTest> GetTestsFromCommands(IEnumerable<ITestCommand> testCommands)
        {
            foreach (ITestCommand testCommand in testCommands)
                yield return (MSTest)testCommand.Test;
        }

        private void CreateTestMetadataFile(string testMetadataFilePath, IEnumerable<MSTest> tests, string assemblyFilePath)
        {
            using (XmlWriter writer = OpenXmlWriter(testMetadataFilePath))
            {
                WriteTestMetadata(writer, tests, assemblyFilePath);
            }
        }

        private void CreateRunConfigFile(string runConfigFilePath)
        {
            using (XmlWriter writer = OpenXmlWriter(runConfigFilePath))
            {
                WriteRunConfig(writer);
            }
        }

        private static XmlWriter OpenXmlWriter(string filePath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;
            return XmlWriter.Create(filePath, settings);
        }

        private sealed class Executor
        {
            private readonly MSTestRunner runner;
            private readonly ITestContext assemblyTestContext;
            private readonly ITestCommand assemblyTestCommand;
            private readonly Dictionary<Guid, ITestCommand> testCommandsByTestId;
            private readonly Dictionary<object, TestStepState> testStepStatesByTestResultId;
            private readonly Dictionary<Test, TestStepState> testStepStatesByTest;

            private TestStepState assemblyTestStepState;
            private object tmi;

            private string originalWorkingDirectory;
#if USE_APPBASE_HACK
            private string originalAppBase;
#endif

            public Executor(MSTestRunner runner, ITestContext assemblyTestContext, ITestCommand assemblyTestCommand)
            {
                this.runner = runner;
                this.assemblyTestContext = assemblyTestContext;
                this.assemblyTestCommand = assemblyTestCommand;

                testCommandsByTestId = new Dictionary<Guid, ITestCommand>();
                testStepStatesByTestResultId = new Dictionary<object, TestStepState>();
                testStepStatesByTest = new Dictionary<Test, TestStepState>();
            }

            public TestOutcome Execute(string testMetadataPath, string testResultsPath,
                string runConfigPath, string searchPathRoot)
            {
                TextWriter writer = assemblyTestContext.LogWriter["MSTest Output"];
                string executablePath = MSTestResolver.FindMSTestPathForVisualStudioVersion(runner.GetVisualStudioVersion());
                if (executablePath == null)
                {
                    assemblyTestContext.LogWriter.Failures.Write(Resources.MSTestController_MSTestExecutableNotFound);
                    return TestOutcome.Error;
                }

                string executableDir = Path.GetDirectoryName(executablePath);
                string privateAssembliesDir = Path.Combine(executableDir, "PrivateAssemblies");
                string publicAssembliesDir = Path.Combine(executableDir, "PublicAssemblies");

                RuntimeAccessor.AssemblyLoader.AddHintDirectory(executableDir);
                RuntimeAccessor.AssemblyLoader.AddHintDirectory(privateAssembliesDir);
                RuntimeAccessor.AssemblyLoader.AddHintDirectory(publicAssembliesDir);

                // Obtain an Executor.
                Assembly commandLineAssembly = Assembly.Load("Microsoft.VisualStudio.QualityTools.CommandLine");
                Type executorType = commandLineAssembly.GetType("Microsoft.VisualStudio.TestTools.CommandLine.Executor");
                object executor = Activator.CreateInstance(executorType);
                try
                {
                    // Configure the Executor's Output to send output to the assembly log writer.
                    PropertyInfo outputProperty = executorType.GetProperty("Output",
                        BindingFlags.Public | BindingFlags.Static);
                    object output = outputProperty.GetValue(executor, null);
                    FieldInfo standardOutputField = output.GetType().GetField("m_standardOutput",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    standardOutputField.SetValue(output, new StreamWriterAdapter(writer));

                    // Register commands with the executor to set command-line arguments.
                    Type commandFactoryType =
                        commandLineAssembly.GetType("Microsoft.VisualStudio.TestTools.CommandLine.CommandFactory");
                    CreateAndAddCommand(executor, commandFactoryType, "/nologo", null);
                    CreateAndAddCommand(executor, commandFactoryType, "/noisolation", null);
                    CreateAndAddCommand(executor, commandFactoryType, "/testmetadata", testMetadataPath);
                    CreateAndAddCommand(executor, commandFactoryType, "/resultsfile", testResultsPath);
                    CreateAndAddCommand(executor, commandFactoryType, "/runconfig", runConfigPath);
                    CreateAndAddCommand(executor, commandFactoryType, "/searchpathroot", searchPathRoot);
                    CreateAndAddCommand(executor, commandFactoryType, "/testlist", SelectedTestListName);

                    // Get the TMI.
                    tmi = commandFactoryType.GetProperty("Tmi", BindingFlags.Public | BindingFlags.Static).GetValue(null, null);

                    // Add event handlers.
                    AddEventHandler(tmi, "TestRunStartedEvent", HandleTestRunStarted);
                    AddEventHandler(tmi, "TestRunFinishedEvent", HandleTestRunFinished);
                    AddEventHandler(tmi, "TestStartedEvent", HandleTestStarted);
                    AddEventHandler(tmi, "TestFinishedEvent", HandleTestFinished);

                    // Execute!
                    InitializeLookupTables();

                    bool success = (bool)executorType.GetMethod("Execute").Invoke(executor, null);

                    FinishTestChildren(assemblyTestCommand);
                    TestOutcome assemblyOutcome = assemblyTestStepState.Outcome;

                    if (!success)
                        assemblyOutcome = TestOutcome.Error;
                    return assemblyOutcome;
                }
                finally
                {
                    // Release state.
                    assemblyTestStepState = null;
                    tmi = null;
                    testCommandsByTestId.Clear();
                    testStepStatesByTestResultId.Clear();
                    testStepStatesByTest.Clear();

                    // Dispose the Executor.  (Also disposes the TMI behind the scenes.)
                    ((IDisposable)executor).Dispose();
                }
            }

            private void InitializeLookupTables()
            {
                foreach (ITestCommand testCommand in assemblyTestCommand.PreOrderTraversal)
                    testCommandsByTestId.Add(((MSTest)testCommand.Test).Guid, testCommand);

                assemblyTestStepState = new TestStepState(null, assemblyTestContext);
                testStepStatesByTest.Add(assemblyTestCommand.Test, assemblyTestStepState);
            }

            private void HandleTestRunStarted(object sender, EventArgs e)
            {
                // Set the current directory because MSTest does not do that itself when the /noisolation
                // switch is specified.  If this is not done, tests will be unable to locate deployment items
                // and other resources they may reference via relative paths.
                Guid testRunId = (Guid) e.GetType().GetProperty("RunId").GetValue(e, null);
                object testRun = tmi.GetType().GetMethod("GetTestRun").Invoke(tmi, new object[] { testRunId });
                object testRunConfiguration = testRun.GetType().GetProperty("RunConfiguration").GetValue(testRun, null);
                string testRunDeploymentOutDirectory = (string) testRunConfiguration.GetType().GetProperty("RunDeploymentOutDirectory").GetValue(testRunConfiguration, null);

                originalWorkingDirectory = Environment.CurrentDirectory;
                Environment.CurrentDirectory = testRunDeploymentOutDirectory;

#if USE_APPBASE_HACK
                originalAppBase = AppDomain.CurrentDomain.BaseDirectory;
                AppDomain.CurrentDomain.SetData("APPBASE", testRunDeploymentOutDirectory);
#pragma warning disable 618,612
                AppDomain.CurrentDomain.ClearPrivatePath();
#pragma warning restore 618,612
#endif
            }

            private void HandleTestRunFinished(object sender, EventArgs e)
            {
#if USE_APPBASE_HACK
                if (originalAppBase != null)
                    AppDomain.CurrentDomain.SetData("APPBASE", originalAppBase);
#endif

                if (originalWorkingDirectory != null)
                    Environment.CurrentDirectory = originalWorkingDirectory;
            }

            private void HandleTestStarted(object sender, EventArgs e)
            {
                foreach (TestStepState testStepState in GetOrCreateTestStepStatesFromTestResultEventArgs(e))
                {
                    testStepState.TestContext.LifecyclePhase = LifecyclePhases.Execute;
                }
            }

            private void HandleTestFinished(object sender, EventArgs e)
            {
                foreach (TestStepState testStepState in GetOrCreateTestStepStatesFromTestResultEventArgs(e))
                {
                    object testResult = GetTestResult(testStepState.TestResultId);
                    RecordTestResult(testStepState, testResult);
                }
            }

            private static void RecordTestResult(TestStepState testStepState, object testResult)
            {
                Array innerResults = GetInnerResults(testResult);
                if (innerResults != null)
                {
                    for (int i = 0; i < innerResults.Length; i++)
                    {
                        object innerResult = innerResults.GetValue(i);

                        TestStep testStep = testStepState.TestContext.TestStep;
                        TestStep innerTestStep = new TestStep(testStep.Test, testStep,
                            testStep.Name, testStep.CodeElement, false);
                        innerTestStep.IsDynamic = true;

                        Array innerInnerResults = GetInnerResults(innerResults);
                        if (innerInnerResults != null && innerInnerResults.Length != 0)
                            innerTestStep.IsTestCase = false;

                        ITestContext innerTestContext = SafeStartChildStep(testStepState.TestContext, innerTestStep);

                        TestStepState innerTestStepState = new TestStepState(testStepState, innerTestContext);
                        RecordTestResult(innerTestStepState, innerResult);
                    }
                }

                Type testResultType = testResult.GetType();
                string stdOut = (string)testResultType.GetProperty("StdOut").GetValue(testResult, null);
                if (!string.IsNullOrEmpty(stdOut))
                    testStepState.TestContext.LogWriter.ConsoleOutput.Write(stdOut);

                string stdErr = (string)testResultType.GetProperty("StdErr").GetValue(testResult, null);
                if (!string.IsNullOrEmpty(stdErr))
                    testStepState.TestContext.LogWriter.ConsoleError.Write(stdErr);

                string debugTrace = (string)testResultType.GetProperty("DebugTrace").GetValue(testResult, null);
                if (!string.IsNullOrEmpty(debugTrace))
                    testStepState.TestContext.LogWriter.DebugTrace.Write(debugTrace);

                string errorMessage = (string)testResultType.GetProperty("ErrorMessage").GetValue(testResult, null);
                if (!string.IsNullOrEmpty(errorMessage))
                    testStepState.TestContext.LogWriter.Failures.Write(errorMessage);

                string errorStackTrace = (string)testResultType.GetProperty("ErrorStackTrace").GetValue(testResult, null);
                if (!string.IsNullOrEmpty(errorStackTrace))
                {
                    if (!string.IsNullOrEmpty(errorMessage))
                        testStepState.TestContext.LogWriter.Failures.WriteLine();
                    testStepState.TestContext.LogWriter.Failures.Write(errorStackTrace);
                }

                string[] textMessages = (string[])testResultType.GetProperty("TextMessages").GetValue(testResult, null);
                foreach (string textMessage in textMessages)
                    testStepState.TestContext.LogWriter.Warnings.WriteLine(textMessage);

                string outcomeString = testResultType.GetProperty("Outcome").GetValue(testResult, null).ToString();
                testStepState.Outcome = GetTestOutcome(outcomeString);

                Array timerResults = (Array)testResultType.GetProperty("TimerResults").GetValue(testResult, null);
                if (timerResults != null)
                {
                    for (int i = 0; i < timerResults.Length; i++)
                    {
                        object timerResult = timerResults.GetValue(i);
                        TimeSpan duration =
                            (TimeSpan)timerResult.GetType().GetProperty("Duration").GetValue(timerResult, null);
                        testStepState.Duration += duration;
                    }
                }

                // Finish the test step unless it is the assembly test which we will finish later.
                if (testStepState.ParentTestStepState != null)
                    testStepState.Finish();
            }

            private IEnumerable<TestStepState> GetOrCreateTestStepStatesFromTestResultEventArgs(EventArgs testResultEventArgs)
            {
                object[] testResultIds = (object[])testResultEventArgs.GetType().GetProperty("ResultIds").GetValue(testResultEventArgs, null);

                foreach (object testResultId in testResultIds)
                {
                    TestStepState testStepState = GetOrCreateTestStepStateByTestResultId(testResultId);
                    if (testStepState != null)
                        yield return testStepState;
                }
            }

            private TestStepState GetOrCreateTestStepStateByTestResultId(object testResultId)
            {
                TestStepState testStepState = GetTestStepStateByTestResultId(testResultId);
                if (testStepState != null)
                    return testStepState;

                object testResult = GetTestResult(testResultId);
                object testObj = testResult.GetType().GetProperty("Test").GetValue(testResult, null);
                object testIdObj = testObj.GetType().GetProperty("Id").GetValue(testObj, null);
                Guid testId = (Guid)testIdObj.GetType().GetProperty("Id").GetValue(testIdObj, null);

                ITestCommand testCommand = GetTestCommandByTestId(testId);
                if (testCommand == null)
                    return null;

                testStepState = GetOrCreateTestStepStateByTest((MSTest) testCommand.Test);
                testStepState.TestResultId = testResultId;
                return testStepState;
            }

            private TestStepState GetOrCreateTestStepStateByTest(MSTest test)
            {
                TestStepState testStepState = GetTestStepStateByTest(test);
                if (testStepState == null)
                {
                    TestStepState parentTestStepState = GetOrCreateTestStepStateByTest((MSTest) test.Parent);
                    Guid testId = test.Guid;
                    ITestCommand testCommand = GetTestCommandByTestId(testId);

                    TestStep testStep = new TestStep(test, parentTestStepState.TestContext.TestStep,
                        test.Name, test.CodeElement, true);
                    if (test.IsDataDriven)
                        testStep.IsTestCase = false;

                    ITestContext testContext = SafeStartCommandStep(parentTestStepState.TestContext, testCommand, testStep);
                    testStepState = new TestStepState(parentTestStepState, testContext);

                    testStepStatesByTest.Add(test, testStepState);
                }

                return testStepState;
            }

            private object GetTestResult(object testResultId)
            {
                return tmi.GetType().GetMethod("GetResult").Invoke(tmi, new object[] { testResultId });
            }

            private ITestCommand GetTestCommandByTestId(Guid testId)
            {
                ITestCommand testCommand;
                testCommandsByTestId.TryGetValue(testId, out testCommand);
                return testCommand;
            }

            private TestStepState GetTestStepStateByTestResultId(object testResultId)
            {
                TestStepState testStepState;
                testStepStatesByTestResultId.TryGetValue(testResultId, out testStepState);
                return testStepState;
            }

            private TestStepState GetTestStepStateByTest(Test test)
            {
                TestStepState testStepState;
                testStepStatesByTest.TryGetValue(test, out testStepState);
                return testStepState;
            }

            private void FinishTest(ITestCommand testCommand)
            {
                FinishTestChildren(testCommand);

                MSTest test = (MSTest) testCommand.Test;
                if (test.IsTestCase)
                {
                    string ignoreReason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
                    if (!String.IsNullOrEmpty(ignoreReason))
                    {
                        TestStepState testStepState = GetOrCreateTestStepStateByTest(test);
                        testStepState.TestContext.LogWriter.Warnings.Write(string.Format("Test was ignored: {0}", ignoreReason));
                        testStepState.Outcome = TestOutcome.Ignored;
                        testStepState.Finish();
                    }
                    else
                    {
                        TestStepState testStepState = GetTestStepStateByTest(test);
                        if (testStepState == null)
                        {
                            testStepState = GetOrCreateTestStepStateByTest(test);
                            testStepState.TestContext.LogWriter.Warnings.Write("No test results available!");
                            testStepState.Outcome = TestOutcome.Skipped;
                            testStepState.Finish();
                        }
                        else
                        {
                            testStepState.Finish();
                        }
                    }
                }
                else
                {
                    TestStepState testStepState = GetTestStepStateByTest(test);
                    if (testStepState != null)
                    {
                        testStepState.Finish();
                    }
                }
            }

            private void FinishTestChildren(ITestCommand testCommand)
            {
                foreach (ITestCommand childTestCommand in testCommand.Children)
                {
                    FinishTest(childTestCommand);
                }
            }

            private static void CreateAndAddCommand(object executor, Type commandFactoryType, string commandName, string commandArg)
            {
                object command = commandFactoryType.GetMethod("CreateCommand").Invoke(null, new object[] { commandName, commandArg });
                executor.GetType().GetMethod("Add").Invoke(executor, new object[] { command });
            }

            private static void AddEventHandler(object obj, string eventName, EventHandler eventHandler)
            {
                EventInfo @event = obj.GetType().GetEvent(eventName);
                Delegate typedEventHandler = Delegate.CreateDelegate(@event.EventHandlerType, eventHandler.Target, eventHandler.Method, true);
                @event.AddEventHandler(obj, typedEventHandler);
            }

            private static Array GetInnerResults(object testResult)
            {
                PropertyInfo innerResultsProperty = testResult.GetType().GetProperty("InnerResults");
                if (innerResultsProperty == null)
                    return null;

                return (Array) innerResultsProperty.GetValue(testResult, null);
            }

            private static TestOutcome GetTestOutcome(string outcome)
            {
                TestOutcome testOutcome;
                // The commented cases are the ones we are not sure how to map yet.
                // By default they'll become TestOutcome.Passed
                switch (outcome)
                {
                    case "Aborted":
                    case "3":
                        testOutcome = TestOutcome.Canceled;
                        break;
                    //case "Completed":
                    //    testOutcome = TestOutcome.Passed;
                    //    break;
                    //case "Disconnected":
                    //    testOutcome = TestOutcome.Passed;
                    //    break;
                    case "Error":
                    case "0":
                        testOutcome = TestOutcome.Error;
                        break;
                    case "Failed":
                    case "1":
                        testOutcome = TestOutcome.Failed;
                        break;
                    case "Inconclusive":
                    case "4":
                        testOutcome = TestOutcome.Inconclusive;
                        break;
                    //case "InProgress":
                    //    testOutcome = TestOutcome.Passed;
                    //    break;
                    //case "Max":
                    //    testOutcome = TestOutcome.Passed;
                    //    break;
                    //case "Min":
                    //    testOutcome = TestOutcome.Passed;
                    //    break;
                    case "NotExecuted":
                    case "7":
                        testOutcome = TestOutcome.Skipped;
                        break;
                    case "NotRunnable":
                    case "6":
                        testOutcome = TestOutcome.Skipped;
                        break;
                    case "Passed":
                    case "10":
                        testOutcome = TestOutcome.Passed;
                        break;
                    //case "PassedButRunAborted":
                    //    testOutcome = TestOutcome.Passed;
                    //    break;
                    case "Pending":
                    case "13":
                        testOutcome = TestOutcome.Pending;
                        break;
                    case "Timeout":
                    case "2":
                        testOutcome = TestOutcome.Timeout;
                        break;
                    //case "Warning":
                    //    testOutcome = TestOutcome.Passed;
                    //    break;
                    default:
                        testOutcome = TestOutcome.Passed;
                        break;
                }

                return testOutcome;
            }
        }

        private static ITestContext SafeStartCommandStep(ITestContext parentTestContext, ITestCommand testCommand, TestStep testStep)
        {
            using (TestContextTrackerAccessor.Instance.EnterContext(parentTestContext))
                return testCommand.StartStep(testStep);
        }

        private static ITestContext SafeStartChildStep(ITestContext parentTestContext, TestStep testStep)
        {
            using (TestContextTrackerAccessor.Instance.EnterContext(parentTestContext))
                return parentTestContext.StartChildStep(testStep);
        }

        private static TestResult SafeFinishStep(ITestContext testContext, TestOutcome outcome, TimeSpan duration)
        {
            testContext.BecomeMultiThreadAware();
            using (TestContextTrackerAccessor.Instance.EnterContext(testContext))
                return testContext.FinishStep(outcome, duration);
        }

        private sealed class TestStepState
        {
            private readonly TestStepState parentTestStepState;
            private readonly ITestContext testContext;

            public TestStepState(TestStepState parentTestStepState, ITestContext testContext)
            {
                this.parentTestStepState = parentTestStepState;
                this.testContext = testContext;
            }

            public TestStepState ParentTestStepState
            {
                get { return parentTestStepState; }
            }

            public ITestContext TestContext
            {
                get { return testContext; }
            }

            public object TestResultId { get; set; }

            public Guid TestExecId { get; set; }

            public TimeSpan Duration { get; set; }

            public TestOutcome Outcome
            {
                get { return testContext.Outcome; }
                set { testContext.SetInterimOutcome(value); }
            }

            public void Finish()
            {
                TestResult testResult = SafeFinishStep(testContext, testContext.Outcome, Duration);

                if (parentTestStepState != null)
                {
                    parentTestStepState.Outcome = parentTestStepState.Outcome.CombineWith(testResult.Outcome.Generalize());
                    parentTestStepState.Duration += testResult.Duration;
                }
            }
        }

        private sealed class StreamWriterAdapter : StreamWriter
        {
            private readonly TextWriter inner;

            public StreamWriterAdapter(TextWriter inner)
                : base(Stream.Null)
            {
                this.inner = inner;
            }

            // These are the only methods used by
            // Microsoft.VisualStudio.TestTools.CommandLine.ConsoleOutput

            public override void WriteLine(string value)
            {
                inner.WriteLine(value);
            }

            public override void Write(string value)
            {
                inner.Write(value);
            }
        }
    }
}
