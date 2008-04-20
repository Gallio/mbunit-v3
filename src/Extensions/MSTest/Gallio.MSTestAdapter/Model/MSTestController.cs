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

// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.IO;
using System.Xml;
using Gallio.MSTestAdapter.Properties;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Concurrency;
using Microsoft.Win32;

namespace Gallio.MSTestAdapter
{
    internal class MSTestController : BaseTestController
    {
        private readonly static string msTestPath;

        static MSTestController()
        {
            msTestPath = FindMSTestPath("9.0");
            if (String.IsNullOrEmpty(msTestPath))
            {
                msTestPath = FindMSTestPath("8.0");
            }
        }

        private static string FindMSTestPath(string visualStudioVersion)
        {
            using (RegistryKey key =
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\" + visualStudioVersion))
            {
                if (key != null)
                {
                    string visualStudioInstallDir = (string)key.GetValue("InstallDir");
                    if (visualStudioInstallDir != null)
                    {
                        string msTestExecutablePath = Path.Combine(visualStudioInstallDir, "MSTest.exe");
                        if (File.Exists(msTestExecutablePath))
                        {
                            return msTestExecutablePath;
                        }
                    }
                }
            }

            return null;
        }

        /// <inheritdoc />
        protected override void RunTestsInternal(ITestCommand rootTestCommand, ITestStep parentTestStep,
            TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask(Resources.MSTestController_RunningMSTestTests, rootTestCommand.TestCount);
                if (options.SkipTestExecution)
                {
                    SkipAll(rootTestCommand, parentTestStep);
                }
                else
                {
                    RunTest(rootTestCommand, parentTestStep, progressMonitor);
                }
            }
        }

        private static void RunTest(ITestCommand testCommand, ITestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITest test = testCommand.Test;
            progressMonitor.SetStatus(test.Name);

            // The first test should be an assembly test
            MSTestAssembly assemblyTest = testCommand.Test as MSTestAssembly;
            if (assemblyTest != null)
            {
                ITestContext context = testCommand.StartPrimaryChildStep(parentTestStep);
                try
                {
                    IList<ITestCommand> allCommands = testCommand.GetAllCommands();
                    DeleteOutputFilesIfExist(assemblyTest);
                    GenerateTestList(assemblyTest, allCommands);
                    ExecuteTests(context, assemblyTest);
                    bool passed = ProcessTestResults(context, testCommand, allCommands);
                    context.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
                }
                catch (Exception ex)
                {
                    TestLogWriterUtils.WriteException(context.LogWriter, LogStreamNames.Failures, ex, "Internal Error");
                    context.FinishStep(TestOutcome.Failed, null);
                }
            }

            progressMonitor.Worked(1);
        }

        private static void DeleteOutputFilesIfExist(MSTestAssembly assemblyTest)
        {
            //TODO: Should we wrap the potential exceptions that can occur here?
            if (File.Exists(assemblyTest.FullResultsFileName))
            {
                File.Delete(assemblyTest.FullResultsFileName);
            }
            if (File.Exists(assemblyTest.FullTestMetadataFileName))
            {
                File.Delete(assemblyTest.FullTestMetadataFileName);
            }
        }

        private static void GenerateTestList(MSTestAssembly assemblyTest, IEnumerable<ITestCommand> testCommands)
        {
            string parentListId = "8c43106b-9dc1-4907-a29f-aa66a61bf5b6";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter xmlWriter = XmlWriter.Create(assemblyTest.FullTestMetadataFileName, settings))
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("TestLists", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2006");

                xmlWriter.WriteStartElement("TestList");
                xmlWriter.WriteAttributeString("name", "Lists of Tests");
                xmlWriter.WriteAttributeString("id", parentListId);

                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("TestList");
                xmlWriter.WriteAttributeString("id", assemblyTest.Guid);
                xmlWriter.WriteAttributeString("name", assemblyTest.TestListName);
                xmlWriter.WriteAttributeString("parentListId", parentListId);
                xmlWriter.WriteStartElement("TestLinks");

                foreach (ITestCommand command in testCommands)
                {
                    if (command.Test.IsTestCase)
                    {
                        xmlWriter.WriteStartElement("TestLink");
                        xmlWriter.WriteAttributeString("id", ((MSTest)command.Test).Guid);
                        xmlWriter.WriteAttributeString("name", ((MSTest)command.Test).TestName);
                        xmlWriter.WriteAttributeString("storage", assemblyTest.FullPath);
                        xmlWriter.WriteAttributeString("type",
                            "Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestElement, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.ObjectModel,   PublicKeyToken=b03f5f7f11d50a3a");
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }
        }

        private static void ExecuteTests(ITestContext assemblyContext, MSTestAssembly assemblyTest)
        {
            if (String.IsNullOrEmpty(msTestPath))
            {
                assemblyContext.LogWriter.Write(LogStreamNames.Warnings,
                    Resources.MSTestController_MSTestExecutableNotFound);
                return;
            }

            ProcessTask MSTestProcess = new ProcessTask(
                msTestPath,
                @" /nologo"
                + " /resultsfile:"
                + QuoteFilename(assemblyTest.ResultsFileName)
                + " /testlist:"
                + assemblyTest.TestListName
                + " /testmetadata:"
                + QuoteFilename(assemblyTest.TestMetadataFileName),
                assemblyTest.DirectoryName);
            MSTestProcess.Run(null);
        }

        private static bool ProcessTestResults(ITestContext assemblyContext, ITestCommand assemblyCommand, IEnumerable<ITestCommand> allCommands)
        {
            MSTestAssembly assemblyTest = (MSTestAssembly)assemblyCommand.Test;

            // Erros in the class or assembly setup/teardown methods are put in a general error
            // section by MSTest, so we log them at the assembly level.
            ProcessGeneralErrorMessages(assemblyContext, assemblyTest);

            Dictionary<string, MSTestExecutionInfo> testExecutionInfos = ExtractExecutedTestsInformation(assemblyTest);

            // The ignored tests won't be run by MSTest. In the case where all the selected tests
            // have been ignored, we won't even have a results file, so we need to process them
            // here.
            ProcessIgnoredTests(testExecutionInfos, allCommands);

            bool passed = true;
            foreach (ITestCommand command in assemblyCommand.Children)
            {
                passed &= ProcessTestCommand(command, assemblyContext.TestStep, testExecutionInfos);
            }

            return passed;
        }

        private static void ProcessGeneralErrorMessages(ITestContext assemblyContext, MSTestAssembly assemblyTest)
        {
            if (File.Exists(assemblyTest.FullResultsFileName))
            {
                using (XmlReader reader = XmlReader.Create(assemblyTest.FullResultsFileName))
                {
                    while (reader.ReadToFollowing("RunInfo"))
                    {
                        reader.ReadToFollowing("Text");
                        LogError(assemblyContext, reader.ReadString());
                    }
                }
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
                        if (testExecutionInfo.StdOut != null) LogStdOut(testContext, testExecutionInfo.StdOut);
                        if (testExecutionInfo.Errors != null) LogError(testContext, testExecutionInfo.Errors);
                        testContext.FinishStep(testExecutionInfo.Outcome, testExecutionInfo.Duration);
                        return (testExecutionInfo.Outcome != TestOutcome.Error && testExecutionInfo.Outcome != TestOutcome.Failed);
                    }
                    testContext.FinishStep(TestOutcome.Passed, null);
                    return true;
                }
                else if (command.Children.Count > 0)
                {
                    bool passed = true;
                    foreach (ITestCommand child in command.Children)
                    {
                        passed &= ProcessTestCommand(child, testContext.TestStep, testExecutionInfos);
                    }
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

        private static Dictionary<string, MSTestExecutionInfo> ExtractExecutedTestsInformation(MSTestAssembly assemblyTest)
        {
            Dictionary<string, MSTestExecutionInfo> testsExecutionInfo = new Dictionary<string, MSTestExecutionInfo>();
            if (File.Exists(assemblyTest.FullResultsFileName))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreWhitespace = true;
                using (XmlReader reader = XmlReader.Create(assemblyTest.FullResultsFileName, settings))
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
                        testsExecutionInfo.Add(testExecutionInfo.Guid, testExecutionInfo);
                    }
                }
            }

            return testsExecutionInfo;
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
                        testCommandsByTestGuid.Add(testExecutionInfo.Guid, testExecutionInfo);
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

        private static string ReadStdOut(XmlReader reader)
        {
            if (reader.ReadToFollowing("StdOut"))
            {
                return reader.ReadString();
            }

            return null;
        }

        private static void LogStdOut(ITestContext context, string message)
        {
            context.LogWriter.Write(LogStreamNames.ConsoleOutput, message);
        }

        private static void LogError(ITestContext context, string message)
        {
            ITestLogWriter writer = context.LogWriter;
            string streamName = LogStreamNames.Failures;
            writer.BeginSection(streamName, "Internal Error");
            writer.Write(streamName, message);
            writer.EndSection(streamName);
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

        private static string QuoteFilename(string filename)
        {
            return "\"" + filename + "\"";
        }

        private class MSTestExecutionInfo
        {
            public string Guid;
            public TimeSpan? Duration;
            public TestOutcome Outcome;
            public string StdOut = null;
            public string Errors = null;
        }
    }
}
