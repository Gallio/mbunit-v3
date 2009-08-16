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
using System.Xml;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Model;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Tree;
using Gallio.MSTestAdapter.Model;
using Gallio.MSTestAdapter.Properties;
using Gallio.Runtime.ProgressMonitoring;

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

        public TestOutcome RunSession(ITestContext assemblyContext, MSTestAssembly assemblyTest,
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

                // Set the working directory for the test runner based on the current
                // directory that was set in our current test isolation context.
                string workingDirectory = Environment.CurrentDirectory;

                progressMonitor.SetStatus("Generating test metadata file.");
                CreateTestMetadataFile(testMetadataPath,
                    GetTestsFromCommands(assemblyTestCommand.PreOrderTraversal), assemblyTest.AssemblyFilePath);

                progressMonitor.SetStatus("Generating run config file.");
                CreateRunConfigFile(runConfigPath);

                progressMonitor.SetStatus("Executing tests.");
                TestOutcome outcome = ExecuteTests(assemblyContext, workingDirectory,
                    testMetadataPath, testResultsPath, runConfigPath, searchPathRoot);

                progressMonitor.SetStatus("Processing results.");
                outcome = outcome.CombineWith(ProcessTestResults(assemblyContext, assemblyTestCommand, testResultsPath));

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

        private static MSTestCommand GetMSTestCommand()
        {
            /*
            return Debugger.IsAttached
                ? (MSTestCommand) EmbeddedMSTestCommand.Instance
                : StandaloneMSTestCommand.Instance;
             */

            // Always use the embedded MSTest command since this also provides support
            // for code coverage, performs better and ensures more consistent results
            // than if we were to run tests in two different ways.
            return EmbeddedMSTestCommand.Instance;
        }

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

        private TestOutcome ExecuteTests(ITestContext context, string workingDirectory,
            string testMetadataPath, string testResultsPath, string runConfigPath, string searchPathRoot)
        {
            MSTestCommandArguments args = new MSTestCommandArguments();
            args.NoLogo = true;
            args.TestMetadata = testMetadataPath;
            args.ResultsFile = testResultsPath;
            args.RunConfig = runConfigPath;
            args.TestList = SelectedTestListName;
            args.SearchPathRoot = searchPathRoot;

            string executablePath = MSTestResolver.FindMSTestPathForVisualStudioVersion(GetVisualStudioVersion());
            if (executablePath == null)
            {
                context.LogWriter.Failures.Write(Resources.MSTestController_MSTestExecutableNotFound);
                return TestOutcome.Error;
            }

            TextWriter writer = context.LogWriter["MSTest Output"];
            int exitCode = GetMSTestCommand().Run(executablePath, workingDirectory, args, writer);

            if (exitCode != 0)
            {
                context.LogWriter.Failures.Write("MSTest returned an exit code of {0}.", exitCode);
            }

            return TestOutcome.Passed;
        }

        private TestOutcome ProcessTestResults(ITestContext assemblyContext,
            ITestCommand assemblyCommand, string resultsFilePath)
        {
            MultiMap<string, MSTestResult> testResults = new MultiMap<string, MSTestResult>();

            if (File.Exists(resultsFilePath))
            {
                using (XmlReader reader = OpenTestResultsFile(resultsFilePath))
                {
                    // Errors in the class or assembly setup/teardown methods are put in a general error
                    // section by MSTest, so we log them at the assembly level.
                    ProcessGeneralErrorMessages(assemblyContext, reader);
                }

                using (XmlReader reader = OpenTestResultsFile(resultsFilePath))
                {
                    ExtractExecutedTestsInformation(testResults, reader);
                }
            }

            // The ignored tests won't be run by MSTest. In the case where all the selected tests
            // have been ignored, we won't even have a results file, so we need to process them
            // here.
            GenerateFakeTestResultsForIgnoredTests(testResults, assemblyCommand.PreOrderTraversal);

            TestOutcome combinedOutcome = TestOutcome.Passed;
            foreach (ITestCommand command in assemblyCommand.Children)
            {
                TestResult commandResult = ProcessTestCommand(command, assemblyContext.TestStep, testResults);
                combinedOutcome = combinedOutcome.CombineWith(commandResult.Outcome);
            }

            return combinedOutcome.Generalize();
        }

        private static XmlReader OpenTestResultsFile(string path)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.CloseInput = true;
            return XmlReader.Create(path, settings);
        }

        private static void ProcessGeneralErrorMessages(ITestContext assemblyContext,
            XmlReader reader)
        {
            while (reader.ReadToFollowing("RunInfo"))
            {
                reader.ReadToFollowing("Text");
                LogError(assemblyContext, reader.ReadString());
            }
        }

        private static TestResult ProcessTestCommand(ITestCommand command, TestStep parentStep, MultiMap<string, MSTestResult> testResults)
        {
            MSTest test = (MSTest)command.Test;
            IList<MSTestResult> testResultList = testResults[test.Guid];

            if (testResultList.Count == 0)
            {
                ITestContext testContext = command.StartStep(new TestStep(test, parentStep));

                TestOutcome combinedOutcome = TestOutcome.Passed;
                if (test.IsTestCase)
                {
                    testContext.LogWriter.Warnings.Write("No test results available!");
                    combinedOutcome = TestOutcome.Skipped;
                }

                TestResult childrenResult = ProcessTestCommandChildren(command, testContext.TestStep, testResults);
                combinedOutcome = combinedOutcome.CombineWith(childrenResult.Outcome.Generalize());

                return testContext.FinishStep(combinedOutcome, childrenResult.Duration);
            }
            else if (testResultList.Count == 1)
            {
                return ProcessTestCommandResultTree(test, command, parentStep, testResults, testResultList[0], true, false);
            }
            else
            {
                ITestContext testContext = command.StartStep(new TestStep(test, parentStep)
                {
                    IsTestCase = false
                });

                TestOutcome combinedOutcome = TestOutcome.Passed;
                TimeSpan combinedDuration = TimeSpan.Zero;

                foreach (MSTestResult testResult in testResultList)
                {
                    TestResult individualResult = ProcessTestCommandResultTree(test, command, parentStep, testResults, testResult, false, false);
                    combinedOutcome = combinedOutcome.CombineWith(individualResult.Outcome);
                    combinedDuration += individualResult.Duration;
                }

                TestResult childrenResult = ProcessTestCommandChildren(command, testContext.TestStep, testResults);
                combinedOutcome = combinedOutcome.CombineWith(childrenResult.Outcome.Generalize());
                combinedDuration += childrenResult.Duration;

                return testContext.FinishStep(combinedOutcome, combinedDuration);
            }
        }

        private static TestResult ProcessTestCommandResultTree(MSTest test, ITestCommand command, TestStep parentStep, MultiMap<string, MSTestResult> testResults, MSTestResult testResult, bool isPrimary, bool isDynamic)
        {
            TestStep testStep = new TestStep(test, parentStep, test.Name, test.CodeElement, isPrimary);
            if (testResult.Children.Count > 0)
                testStep.IsTestCase = false;
            testStep.IsDynamic = isDynamic;

            ITestContext testContext = command.StartStep(testStep);

            if (testResult.StdOut != null)
                LogStdOut(testContext, testResult.StdOut);
            if (testResult.Errors != null)
                LogError(testContext, testResult.Errors);

            TestOutcome combinedOutcome = testResult.Outcome;
            TimeSpan combinedDuration = testResult.Duration;

            foreach (MSTestResult childResultData in testResult.Children)
            {
                // Note: Don't need to sum over outcome and duration because it should already be included in parent information.
                ProcessTestCommandResultTree(test, command, testContext.TestStep, testResults, childResultData, false, true);
            }

            if (isPrimary)
            {
                TestResult childrenResult = ProcessTestCommandChildren(command, testContext.TestStep, testResults);
                combinedOutcome = combinedOutcome.CombineWith(childrenResult.Outcome.Generalize());
                combinedDuration += childrenResult.Duration;
            }

            return testContext.FinishStep(combinedOutcome, combinedDuration);
        }

        private static TestResult ProcessTestCommandChildren(ITestCommand command, TestStep parentStep, MultiMap<string, MSTestResult> testResults)
        {
            TestOutcome combinedOutcome = TestOutcome.Passed;
            TimeSpan combinedDuration = TimeSpan.Zero;

            foreach (ITestCommand child in command.Children)
            {
                TestResult childResult = ProcessTestCommand(child, parentStep, testResults);
                combinedOutcome = combinedOutcome.CombineWith(childResult.Outcome);
                combinedDuration += childResult.Duration;
            }

            return new TestResult(combinedOutcome) { Duration = combinedDuration };
        }

        protected abstract void ExtractExecutedTestsInformation(
            MultiMap<string, MSTestResult> testResults,
            XmlReader reader);

        private static void GenerateFakeTestResultsForIgnoredTests(MultiMap<string, MSTestResult> testResults, IEnumerable<ITestCommand> allCommands)
        {
            foreach (ITestCommand command in allCommands)
            {
                MSTest test = command.Test as MSTest;
                if (test != null && test.IsTestCase)
                {
                    string ignoreReason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
                    if (!String.IsNullOrEmpty(ignoreReason))
                    {
                        MSTestResult testResult = new MSTestResult();
                        testResult.Guid = test.Guid;
                        testResult.Outcome = TestOutcome.Ignored;
                        if (!testResults.ContainsKey(testResult.Guid))
                        {
                            testResults.Add(testResult.Guid, testResult);
                        }
                    }
                }
            }
        }

        private static void LogStdOut(ITestContext context, string message)
        {
            context.LogWriter.ConsoleOutput.Write(message);
        }

        private static void LogError(ITestContext context, string message)
        {
            context.LogWriter.Failures.Write(message);
        }

        protected static TestOutcome GetTestOutcome(string outcome)
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

        protected static TimeSpan GetDuration(string duration)
        {
            return TimeSpan.Parse(duration);
        }
    }
}
