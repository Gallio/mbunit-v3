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
using Gallio.Hosting.ProgressMonitoring;
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
                    DeleteResultFileIfExists(assemblyTest);
                    GenerateTestList(assemblyTest, allCommands);
                    ExecuteTests(context, assemblyTest);
                    ProcessTestResults(context, assemblyTest, allCommands);
                    context.FinishStep(TestOutcome.Passed, null);
                }
                catch (Exception ex)
                {
                    TestLogWriterUtils.WriteException(context.LogWriter, LogStreamNames.Failures, ex, "Internal Error");
                    context.FinishStep(TestOutcome.Failed, null);
                }
            }

            progressMonitor.Worked(1);
        }

        private static void DeleteResultFileIfExists(MSTestAssembly assemblyTest)
        {
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

        private static void ProcessTestResults(ITestContext assemblyContext, MSTestAssembly assemblyTest, IEnumerable<ITestCommand> allCommands)
        {
            Dictionary<string, ITestCommand> testCommandsByTestGuid = GroupCommandsByTestGuid(allCommands);
            Dictionary<ITestCommand, ITestContext> startedCommands = new Dictionary<ITestCommand, ITestContext>();
            Dictionary<ITestCommand, bool> outcomes = new Dictionary<ITestCommand, bool>();

            ProcessGeneralErrorMessages(assemblyContext, assemblyTest);
            ProcessExecutedTests(assemblyTest, testCommandsByTestGuid, startedCommands, assemblyContext.TestStep, outcomes);

            // The ignored tests won't be run by MSTest. In the case where all the selected tests
            // have been ignored, we won't even have a results file, so we need to process them
            // here.
            ProcessIgnoredTests(testCommandsByTestGuid, startedCommands, assemblyContext.TestStep);

            foreach (ITestCommand command in startedCommands.Keys)
            {
                if (outcomes.ContainsKey(command))
                {
                    TestOutcome outcome = (outcomes[command]) ? TestOutcome.Passed : TestOutcome.Failed;
                    startedCommands[command].FinishStep(outcome, null);
                }
                else
                {
                    startedCommands[command].FinishStep(TestOutcome.Passed, null);
                }
            }
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

        private static void ProcessExecutedTests(MSTestAssembly assemblyTest, IDictionary<string, ITestCommand> testCommandsByTestGuid, IDictionary<ITestCommand, ITestContext> startedCommands, ITestStep assemblyTestStep, IDictionary<ITestCommand, bool> outcomes)
        {
            if (File.Exists(assemblyTest.FullResultsFileName))
            {
                using (XmlReader reader = XmlReader.Create(assemblyTest.FullResultsFileName))
                {
                    while (reader.ReadToFollowing("UnitTestResult"))
                    {
                        string id = reader.GetAttribute("testId");
                        string duration = reader.GetAttribute("duration");
                        string outcome = reader.GetAttribute("outcome");
                        ITestCommand currentCommand = testCommandsByTestGuid[id];
                        ITestStep parentStep =
                            ProcessParentTestCommand(startedCommands, currentCommand.Parent, assemblyTestStep);
                        bool passed = ProcessTestMethod(reader, currentCommand, parentStep, outcome, duration);
                        ProcessOutcome(currentCommand, outcomes, passed);
                    }
                }
            }
        }

        private static void ProcessIgnoredTests(IDictionary<string, ITestCommand> testCommandsByTestGuid, IDictionary<ITestCommand, ITestContext> startedCommands, ITestStep assemblyTestStep)
        {
            foreach (string guid in testCommandsByTestGuid.Keys)
            {
                ITestCommand currentCommand = testCommandsByTestGuid[guid];
                string ignoreReason = currentCommand.Test.Metadata.GetValue(MetadataKeys.IgnoreReason);
                if (!String.IsNullOrEmpty(ignoreReason))
                {
                    ITestStep parentStep =
                        ProcessParentTestCommand(startedCommands, currentCommand.Parent, assemblyTestStep);
                    ITestContext context = currentCommand.StartPrimaryChildStep(parentStep);
                    context.FinishStep(TestOutcome.Ignored, null);
                }
            }
        }

        private static void ProcessOutcome(ITestCommand currentCommand, IDictionary<ITestCommand, bool> outcomes, bool passed)
        {
            if (outcomes.ContainsKey(currentCommand.Parent))
            {
                outcomes[currentCommand] &= passed;
            }
            else
            {
                outcomes.Add(currentCommand, passed);
            }
        }

        private static ITestStep ProcessParentTestCommand(IDictionary<ITestCommand, ITestContext> startedCommands, ITestCommand currentCommand, ITestStep assemblyTestStep)
        {
            ITestStep parentStep;
            if (!startedCommands.ContainsKey(currentCommand))
            {
                ITestContext parentContext = currentCommand.StartPrimaryChildStep(assemblyTestStep);
                startedCommands.Add(currentCommand, parentContext);
                parentStep = parentContext.TestStep;
            }
            else
            {
                parentStep = startedCommands[currentCommand].TestStep;
            }
            return parentStep;
        }

        private static bool ProcessTestMethod(XmlReader reader, ITestCommand currentCommand, ITestStep parentStep, string outcome, string duration)
        {
            ITestContext context = currentCommand.StartPrimaryChildStep(parentStep);
            TestOutcome testOutcome = GetTestOutcome(outcome);
            reader.ReadToFollowing("Output");
            ReadStdOut(reader, context);
            if (testOutcome == TestOutcome.Failed)
            {
                ReadErrors(reader, context);
            }
            context.FinishStep(testOutcome, GetDuration(duration));

            return (testOutcome == TestOutcome.Passed) ? true : false;
        }

        private static void ReadErrors(XmlReader reader, ITestContext context)
        {
            if (reader.ReadToFollowing("ErrorInfo"))
            {
                reader.ReadToFollowing("Message");
                string message = reader.ReadString();
                reader.ReadToFollowing("StackTrace");
                message += "\n" + reader.ReadString();
                LogError(context, message);
            }
        }

        private static void ReadStdOut(XmlReader reader, ITestContext context)
        {
            if (reader.ReadToFollowing("StdOut"))
            {
                LogStdOut(context, reader.ReadString());
            }
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

        private static Dictionary<string, ITestCommand> GroupCommandsByTestGuid(IEnumerable<ITestCommand> allCommands)
        {
            Dictionary<string, ITestCommand> testCommandsByTestGuid = new Dictionary<string, ITestCommand>();
            foreach (ITestCommand command in allCommands)
            {
                MSTest test = command.Test as MSTest;
                if (test != null && test.IsTestCase)
                {
                    testCommandsByTestGuid.Add(test.Guid, command);
                }
            }

            return testCommandsByTestGuid;
        }

        private static string QuoteFilename(string filename)
        {
            return "\"" + filename + "\"";
        }
    }
}
