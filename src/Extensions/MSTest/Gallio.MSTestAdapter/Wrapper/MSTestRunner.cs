using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.MSTestAdapter.Model;
using Gallio.MSTestAdapter.Properties;
using Gallio.Runner.Caching;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.MSTestAdapter.Wrapper
{
    internal abstract class MSTestRunner
    {
        protected static readonly Guid RootTestListGuid = new Guid("8c43106b-9dc1-4907-a29f-aa66a61bf5b6");
        protected static readonly Guid SelectedTestListGuid = new Guid("05EF261C-0065-4c5f-9DE3-3D068277A643");
        protected const string SelectedTestListName = "SelectedTests";

        private readonly IDiskCache diskCache;

        public MSTestRunner(IDiskCache diskCache)
        {
            if (diskCache == null)
                throw new ArgumentNullException("diskCache");

            this.diskCache = diskCache;
        }

        public static MSTestRunner GetRunnerForFrameworkVersion(Version frameworkVersion, IDiskCache diskCache)
        {
            //if (frameworkVersion.Major == 8 && frameworkVersion.Minor == 0)
            //    return new MSTestRunner2005(diskCache);
            if (frameworkVersion.Major == 9 && frameworkVersion.Minor == 0)
                return new MSTestRunner2008(diskCache);
            if (frameworkVersion.Major == 10 && frameworkVersion.Minor == 0)
                return new MSTestRunner2010(diskCache);

            throw new NotSupportedException(string.Format("MSTest v{0}.{1} is not supported at this time.", frameworkVersion.Major, frameworkVersion.Minor));
        }

        public TestOutcome RunSession(ITestContext assemblyContext, MSTestAssembly assemblyTest,
            ITestCommand assemblyTestCommand, ITestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            IDiskCacheGroup cacheGroup = diskCache.Groups["MSTestAdapter:" + Guid.NewGuid()];
            try
            {
                cacheGroup.Create();

                string testMetadataPath = cacheGroup.GetFileInfo("tests.vsmdi").FullName;
                string testResultsPath = cacheGroup.GetFileInfo("tests.trx").FullName;
                string workingDirectory = Environment.CurrentDirectory;

                progressMonitor.SetStatus("Generating tests list");
                CreateTestListFile(testMetadataPath,
                    GetTestsFromCommands(assemblyTestCommand.PreOrderTraversal), assemblyTest.AssemblyFilePath);

                progressMonitor.SetStatus("Executing tests");
                TestOutcome outcome = ExecuteTests(assemblyContext, workingDirectory,
                    testMetadataPath, testResultsPath);

                progressMonitor.SetStatus("Processing results");
                if (!ProcessTestResults(assemblyContext, assemblyTestCommand, testResultsPath))
                    outcome = outcome.CombineWith(TestOutcome.Failed);

                return outcome;
            }
            finally
            {
                cacheGroup.Delete();
            }
        }

        protected abstract string GetVisualStudioVersion();

        protected abstract void WriteTestList(XmlWriter writer, IEnumerable<MSTest> tests, string assemblyFilePath);

        private static MSTestCommand GetMSTestCommand()
        {
            return Debugger.IsAttached
                ? (MSTestCommand) DebugMSTestCommand.Instance
                : StandaloneMSTestCommand.Instance;
        }

        private static IEnumerable<MSTest> GetTestsFromCommands(IEnumerable<ITestCommand> testCommands)
        {
            foreach (ITestCommand testCommand in testCommands)
                yield return (MSTest)testCommand.Test;
        }

        private void CreateTestListFile(string testMetadataFilePath, IEnumerable<MSTest> tests, string assemblyFilePath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;
            using (XmlWriter writer = XmlWriter.Create(testMetadataFilePath, settings))
            {
                WriteTestList(writer, tests, assemblyFilePath);
            }
        }

        private TestOutcome ExecuteTests(ITestContext context, string workingDirectory,
            string testMetadataPath, string testResultsPath)
        {
            MSTestCommandArguments args = new MSTestCommandArguments();
            args.NoLogo = true;
            args.TestMetadata = testMetadataPath;
            args.ResultsFile = testResultsPath;
            args.TestList = SelectedTestListName;

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

        private static bool ProcessTestResults(ITestContext assemblyContext,
            ITestCommand assemblyCommand, string resultsFilePath)
        {
            Dictionary<string, MSTestResult> testResults = new Dictionary<string, MSTestResult>();

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
            ProcessIgnoredTests(testResults, assemblyCommand.PreOrderTraversal);

            bool passed = true;
            foreach (ITestCommand command in assemblyCommand.Children)
            {
                passed &= ProcessTestCommand(command, assemblyContext.TestStep, testResults);
            }

            return passed;
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

        private static bool ProcessTestCommand(ITestCommand command, ITestStep parentStep, Dictionary<string, MSTestResult> testResults)
        {
            ITestContext testContext = command.StartPrimaryChildStep(parentStep);
            MSTest test = (MSTest)command.Test;
            try
            {
                if (test.IsTestCase)
                {
                    if (testResults.ContainsKey(test.Guid))
                    {
                        MSTestResult testResult = testResults[test.Guid];

                        if (testResult.StdOut != null)
                            LogStdOut(testContext, testResult.StdOut);
                        if (testResult.Errors != null)
                            LogError(testContext, testResult.Errors);

                        testContext.FinishStep(testResult.Outcome, testResult.Duration);
                        return (testResult.Outcome != TestOutcome.Error && testResult.Outcome != TestOutcome.Failed);
                    }

                    testContext.LogWriter.Warnings.Write("No test results available!");
                    testContext.FinishStep(TestOutcome.Skipped, null);
                    return true;
                }
                else if (command.Children.Count > 0)
                {
                    bool passed = true;
                    foreach (ITestCommand child in command.Children)
                        passed &= ProcessTestCommand(child, testContext.TestStep, testResults);

                    testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
                    return passed;
                }
                else
                {
                    testContext.FinishStep(TestOutcome.Passed, null);
                    return true;
                }
            }
            catch
            {
                testContext.FinishStep(TestOutcome.Error, null);
                throw;
            }
        }

        private static void ExtractExecutedTestsInformation(
            Dictionary<string, MSTestResult> testResults,
            XmlReader reader)
        {
            while (reader.ReadToFollowing("UnitTestResult"))
            {
                MSTestResult testResult = new MSTestResult();
                testResult.Guid = reader.GetAttribute("testId");
                testResult.Duration = GetDuration(reader.GetAttribute("duration"));
                testResult.Outcome = GetTestOutcome(reader.GetAttribute("outcome"));
                reader.ReadToFollowing("Output");
                reader.Read();
                if (reader.Name == "StdOut")
                {
                    testResult.StdOut = reader.ReadString();
                    reader.Read();
                }
                if (reader.Name == "ErrorInfo")
                {
                    testResult.Errors = ReadErrors(reader);
                }

                testResults.Add(testResult.Guid, testResult);
            }
        }

        private static void ProcessIgnoredTests(Dictionary<string, MSTestResult> testCommandsByTestGuid, IEnumerable<ITestCommand> allCommands)
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
                        if (!testCommandsByTestGuid.ContainsKey(testResult.Guid))
                        {
                            testCommandsByTestGuid.Add(testResult.Guid, testResult);
                        }
                        //testCommandsByTestGuid.Add(testExecutionInfo.Guid, testExecutionInfo);
                    }
                }
            }
        }

        private static string ReadErrors(XmlReader reader)
        {
            reader.ReadToFollowing("Message");
            string message = reader.ReadString();
            reader.ReadToFollowing("StackTrace");
            message += "\n" + reader.ReadString();
            return message;
        }

        private static void LogStdOut(ITestContext context, string message)
        {
            context.LogWriter.ConsoleOutput.Write(message);
        }

        private static void LogError(ITestContext context, string message)
        {
            context.LogWriter.Failures.Write(message);
        }

        private static TestOutcome GetTestOutcome(string outcome)
        {
            TestOutcome testOutcome;
            // The commented cases are the ones we are not sure how to map yet.
            // By default they'll become TestOutcome.Passed
            switch (outcome)
            {
                case "Aborted":
                    testOutcome = TestOutcome.Canceled;
                    break;
                //case "Completed":
                //    testOutcome = TestOutcome.Passed;
                //    break;
                //case "Disconnected":
                //    testOutcome = TestOutcome.Passed;
                //    break;
                case "Error":
                    testOutcome = TestOutcome.Error;
                    break;
                case "Failed":
                    testOutcome = TestOutcome.Failed;
                    break;
                case "Inconclusive":
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
                    testOutcome = TestOutcome.Skipped;
                    break;
                case "NotRunnable":
                    testOutcome = TestOutcome.Skipped;
                    break;
                case "Passed":
                    testOutcome = TestOutcome.Passed;
                    break;
                //case "PassedButRunAborted":
                //    testOutcome = TestOutcome.Passed;
                //    break;
                case "Pending":
                    testOutcome = TestOutcome.Pending;
                    break;
                case "Timeout":
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

        private static TimeSpan GetDuration(string duration)
        {
            return TimeSpan.Parse(duration);
        }
    }
}
