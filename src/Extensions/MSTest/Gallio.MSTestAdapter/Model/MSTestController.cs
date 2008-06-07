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
using System.Diagnostics;
using System.IO;
using System.Xml;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.MSTestAdapter.Properties;
using Gallio.MSTestAdapter.Wrapper;
using Gallio.Runner.Caching;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.MSTestAdapter.Model
{
    internal class MSTestController : BaseTestController
    {
        private static readonly Guid RootTestListGuid = new Guid("8c43106b-9dc1-4907-a29f-aa66a61bf5b6");
        private static readonly Guid SelectedTestListGuid = new Guid("05EF261C-0065-4c5f-9DE3-3D068277A643");
        private const string SelectedTestListName = "SelectedTests";

        private readonly IMSTestCommand mstestCommand;
        private readonly IDiskCache diskCache;

        internal MSTestController(IMSTestCommand mstestCommand, IDiskCache diskCache)
        {
            if (mstestCommand == null)
                throw new ArgumentNullException("mstestCommand");
            if (diskCache == null)
                throw new ArgumentNullException("diskCache");

            this.mstestCommand = mstestCommand;
            this.diskCache = diskCache;
        }

        public static MSTestController CreateController()
        {
            var command = Debugger.IsAttached 
                ? (IMSTestCommand) DebugMSTestCommand.Instance
                : StandaloneMSTestCommand.Instance;

            return new MSTestController(command, new TemporaryDiskCache());
        }

        /// <inheritdoc />
        protected override TestOutcome RunTestsImpl(ITestCommand rootTestCommand, ITestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask(Resources.MSTestController_RunningMSTestTests, rootTestCommand.TestCount);
                if (options.SkipTestExecution)
                {
                    SkipAll(rootTestCommand, parentTestStep);
                    return TestOutcome.Skipped;
                }
                else
                {
                    return RunTest(rootTestCommand, parentTestStep, progressMonitor);
                }
            }
        }

        private TestOutcome RunTest(ITestCommand testCommand, ITestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITest test = testCommand.Test;
            progressMonitor.SetStatus(test.Name);

            // The first test should be an assembly test
            MSTestAssembly assemblyTest = testCommand.Test as MSTestAssembly;
            TestOutcome outcome;
            if (assemblyTest != null)
            {
                ITestContext assemblyContext = testCommand.StartPrimaryChildStep(parentTestStep);
                try
                {
                    outcome = RunSession(assemblyContext, assemblyTest,
                        testCommand, parentTestStep, progressMonitor);
                }
                catch (Exception ex)
                {
                    TestLogWriterUtils.WriteException(assemblyContext.LogWriter, LogStreamNames.Failures, ex, "Internal Error");
                    outcome = TestOutcome.Error;
                }

                assemblyContext.FinishStep(outcome, null);
            }
            else
            {
                outcome = TestOutcome.Skipped;
            }

            progressMonitor.Worked(1);
            return outcome;
        }

        private TestOutcome RunSession(ITestContext assemblyContext, MSTestAssembly assemblyTest,
            ITestCommand assemblyTestCommand, ITestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            IDiskCacheGroup cacheGroup = diskCache.Groups["MSTestAdapter:" + Guid.NewGuid().ToString()];
            try
            {
                cacheGroup.Create();

                string testMetadataPath = cacheGroup.GetFileInfo("tests.vsmdi").FullName;
                string testResultsPath = cacheGroup.GetFileInfo("tests.trx").FullName;
                string workingDirectory = Environment.CurrentDirectory;

                progressMonitor.SetStatus("Generating tests list");
                GenerateTestList(assemblyTestCommand.PreOrderTraversal, assemblyTest, testMetadataPath);

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

        private static void GenerateTestList(IEnumerable<ITestCommand> testCommands,
            MSTestAssembly assemblyTest, string testMetadataFilePath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;
            using (XmlWriter xmlWriter = XmlWriter.Create(testMetadataFilePath, settings))
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("TestLists", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2006");

                xmlWriter.WriteStartElement("TestList");
                xmlWriter.WriteAttributeString("name", "Lists of Tests");
                xmlWriter.WriteAttributeString("id", RootTestListGuid.ToString());

                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("TestList");
                xmlWriter.WriteAttributeString("id", SelectedTestListGuid.ToString());
                xmlWriter.WriteAttributeString("name", SelectedTestListName);
                xmlWriter.WriteAttributeString("parentListId", RootTestListGuid.ToString());
                xmlWriter.WriteStartElement("TestLinks");

                foreach (ITestCommand command in testCommands)
                {
                    if (command.Test.IsTestCase)
                    {
                        xmlWriter.WriteStartElement("TestLink");
                        xmlWriter.WriteAttributeString("id", ((MSTest)command.Test).Guid);
                        xmlWriter.WriteAttributeString("name", ((MSTest)command.Test).TestName);
                        xmlWriter.WriteAttributeString("storage", assemblyTest.AssemblyFilePath);
                        xmlWriter.WriteAttributeString("type",
                            "Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestElement, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.ObjectModel, PublicKeyToken=b03f5f7f11d50a3a");
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
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

            TextWriter writer = new TestLogStreamWriter(context.LogWriter, "MSTest Output");
            int exitCode = mstestCommand.Run(workingDirectory, args, writer, writer);

            if (exitCode == -1)
            {
                context.LogWriter.Write(LogStreamNames.Failures,
                   Resources.MSTestController_MSTestExecutableNotFound);
                return TestOutcome.Error;
            }

            if (exitCode != 0)
            {
                context.LogWriter.Write(LogStreamNames.Warnings,
                    String.Format("MSTest returned an exit code of {0}.", exitCode));
            }

            return TestOutcome.Passed;
        }

        private static bool ProcessTestResults(ITestContext assemblyContext,
            ITestCommand assemblyCommand, string resultsFilePath)
        {
            Dictionary<string, MSTestExecutionInfo> testExecutionInfos = new Dictionary<string,MSTestExecutionInfo>();

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
                    ExtractExecutedTestsInformation(testExecutionInfos, reader);
                }
            }

            // The ignored tests won't be run by MSTest. In the case where all the selected tests
            // have been ignored, we won't even have a results file, so we need to process them
            // here.
            ProcessIgnoredTests(testExecutionInfos, assemblyCommand.PreOrderTraversal);

            bool passed = true;
            foreach (ITestCommand command in assemblyCommand.Children)
            {
                passed &= ProcessTestCommand(command, assemblyContext.TestStep, testExecutionInfos);
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

        private static bool ProcessTestCommand(ITestCommand command, ITestStep parentStep, Dictionary<string, MSTestExecutionInfo> testExecutionInfos)
        {
            ITestContext testContext = command.StartPrimaryChildStep(parentStep);
            MSTest test = (MSTest)command.Test;
            try
            {
                if (test.IsTestCase)
                {
                    if (testExecutionInfos.ContainsKey(test.Guid))
                    {
                        MSTestExecutionInfo testExecutionInfo = testExecutionInfos[test.Guid];

                        if (testExecutionInfo.StdOut != null)
                            LogStdOut(testContext, testExecutionInfo.StdOut);
                        if (testExecutionInfo.Errors != null)
                            LogError(testContext, testExecutionInfo.Errors);

                        testContext.FinishStep(testExecutionInfo.Outcome, testExecutionInfo.Duration);
                        return (testExecutionInfo.Outcome != TestOutcome.Error && testExecutionInfo.Outcome != TestOutcome.Failed);
                    }

                    testContext.LogWriter.Write(LogStreamNames.Warnings, "No test results available!");
                    testContext.FinishStep(TestOutcome.Skipped, null);
                    return true;
                }
                else if (command.Children.Count > 0)
                {
                    bool passed = true;
                    foreach (ITestCommand child in command.Children)
                        passed &= ProcessTestCommand(child, testContext.TestStep, testExecutionInfos);

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
            Dictionary<string, MSTestExecutionInfo> testExecutionInfos,
            XmlReader reader)
        {
            while (reader.ReadToFollowing("UnitTestResult"))
            {
                MSTestExecutionInfo testExecutionInfo = new MSTestExecutionInfo();
                testExecutionInfo.Guid = reader.GetAttribute("testId");
                testExecutionInfo.Duration = GetDuration(reader.GetAttribute("duration"));
                testExecutionInfo.Outcome = GetTestOutcome(reader.GetAttribute("outcome"));
                reader.ReadToFollowing("Output");
                reader.Read();
                if (reader.Name == "StdOut")
                {
                    testExecutionInfo.StdOut = reader.ReadString();
                    reader.Read();
                }
                if (reader.Name == "ErrorInfo")
                {
                    testExecutionInfo.Errors = ReadErrors(reader);
                }

                testExecutionInfos.Add(testExecutionInfo.Guid, testExecutionInfo);
            }
        }

        private static void ProcessIgnoredTests(Dictionary<string, MSTestExecutionInfo> testCommandsByTestGuid, IEnumerable<ITestCommand> allCommands)
        {
            foreach (ITestCommand command in allCommands)
            {
                MSTest test = command.Test as MSTest;
                if (test != null && test.IsTestCase)
                {
                    string ignoreReason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
                    if (!String.IsNullOrEmpty(ignoreReason))
                    {
                        MSTestExecutionInfo testExecutionInfo = new MSTestExecutionInfo();
                        testExecutionInfo.Guid = test.Guid;
                        testExecutionInfo.Outcome = TestOutcome.Ignored;
                        if (!testCommandsByTestGuid.ContainsKey(testExecutionInfo.Guid))
                        {
                            testCommandsByTestGuid.Add(testExecutionInfo.Guid, testExecutionInfo);
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
            context.LogWriter.Write(LogStreamNames.ConsoleOutput, message);
        }

        private static void LogError(ITestContext context, string message)
        {
            context.LogWriter.Write(LogStreamNames.Failures, message);
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

        private sealed class MSTestExecutionInfo
        {
            public string Guid;
            public TimeSpan? Duration;
            public TestOutcome Outcome;
            public string StdOut = null;
            public string Errors = null;
        }
    }
}
