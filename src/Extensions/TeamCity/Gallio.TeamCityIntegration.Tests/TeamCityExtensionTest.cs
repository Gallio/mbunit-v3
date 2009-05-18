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
using System.Text;
using Gallio.Common.Concurrency;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using Gallio.Model.Execution;
using Gallio.Common.Markup;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime.Logging;
using MbUnit.Framework;

namespace Gallio.TeamCityIntegration.Tests
{
    [TestsOn(typeof(TeamCityExtension))]
    public class TeamCityExtensionTest
    {
        private TestRunnerEventDispatcher dispatcher;
        private Log log;

        private static readonly StructuredDocument ComprehensiveDocument;

        static TeamCityExtensionTest()
        {
            StructuredDocumentWriter logWriter = new StructuredDocumentWriter();
            logWriter.ConsoleOutput.WriteLine("output");
            logWriter.ConsoleInput.WriteLine("input");
            logWriter.DebugTrace.WriteLine("trace");
            logWriter.Default.WriteLine("log");
            logWriter.ConsoleError.WriteLine("error");
            logWriter.Failures.WriteLine("failure");
            logWriter.Warnings.WriteLine("warning");
            logWriter.Close();

            ComprehensiveDocument = logWriter.Document;
        }

        [SetUp]
        public void SetUp()
        {
            dispatcher = new TestRunnerEventDispatcher();
            log = new Log();

            var ext = new TeamCityExtension("flow");
            ext.Install(dispatcher, log);
        }

        [Test]
        public void InitializeStarted()
        {
            dispatcher.NotifyInitializeStarted(new InitializeStartedEventArgs(new TestRunnerOptions()));

            Assert.AreEqual("##teamcity[progressMessage 'Initializing test runner.' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void ExploreStarted()
        {
            dispatcher.NotifyExploreStarted(new ExploreStartedEventArgs(new TestPackageConfig(), new TestExplorationOptions(),
                new LockBox<Report>(new Report())));

            Assert.AreEqual("##teamcity[progressStart 'Exploring tests.' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void ExploreFinished()
        {
            dispatcher.NotifyExploreFinished(new ExploreFinishedEventArgs(true, new Report()));

            Assert.AreEqual("##teamcity[progressFinish 'Exploring tests.' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void RunStarted()
        {
            dispatcher.NotifyRunStarted(new RunStartedEventArgs(new TestPackageConfig(), new TestExplorationOptions(), new TestExecutionOptions(),
                new LockBox<Report>(new Report())));

            Assert.AreEqual("##teamcity[progressStart 'Running tests.' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void RunFinished()
        {
            dispatcher.NotifyRunFinished(new RunFinishedEventArgs(true, new Report()));

            Assert.AreEqual("##teamcity[progressFinish 'Running tests.' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void DisposeFinished()
        {
            dispatcher.NotifyDisposeFinished(new DisposeFinishedEventArgs(true));

            Assert.AreEqual("##teamcity[progressMessage 'Disposed test runner.' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void TestStepStarted_Root()
        {
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(
                new Report(),
                new TestData("id", "root", "testFullName"),
                new TestStepRun(new TestStepData("stepId", "root", "", "id") { IsPrimary = true })));

            Assert.AreEqual("", log.ToString());
        }

        [Test]
        public void TestStepStarted_NonPrimaryNonTestCase()
        {
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "id") { IsPrimary = false, IsTestCase = false })));

            Assert.AreEqual("", log.ToString());
        }

        [Test]
        public void TestStepStarted_NonPrimaryTestCase()
        {
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "id") { IsPrimary = false, IsTestCase = true })));

            Assert.AreEqual("##teamcity[testStarted name='stepFullName' captureStandardOutput=\'false\' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void TestStepStarted_PrimaryNonTestCase()
        {
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "id") { IsPrimary = true, IsTestCase = false })));

            Assert.AreEqual("##teamcity[testSuiteStarted name='stepFullName' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void TestStepStarted_PrimaryTestCase()
        {
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "id") { IsPrimary = true, IsTestCase = true })));

            Assert.AreEqual("##teamcity[testStarted name='stepFullName' captureStandardOutput=\'false\' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void TestStepFinished_Root()
        {
            var report = new Report();
            var testData = new TestData("id", "root", "testFullName");
            var testStepRun = new TestStepRun(new TestStepData("stepId", "root", "", "id") { IsPrimary = true });
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRun));
            log.Clear();

            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRun));

            Assert.AreEqual("", log.ToString());
        }

        [Test]
        public void TestStepFinished_NonPrimaryNonTestCase()
        {
            var report = new Report();
            var testData = new TestData("id", "testName", "testFullName");
            var testStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "id") { IsPrimary = false, IsTestCase = false });
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRun));
            log.Clear();

            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRun));

            Assert.AreEqual("", log.ToString());
        }

        [Test]
        public void TestStepFinished_PrimaryNonTestCase()
        {
            var report = new Report();
            var testData = new TestData("id", "testName", "testFullName");
            var testStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "id") { IsPrimary = true, IsTestCase = false });
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRun));
            log.Clear();

            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRun));

            Assert.AreEqual("##teamcity[testSuiteFinished name='stepFullName' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void TestStepFinished_TestCase_Passed([Column(true, false)] bool primary)
        {
            var report = new Report();
            var testData = new TestData("id", "testName", "testFullName");
            var testStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "id") { IsPrimary = primary, IsTestCase = true })
            {
                Result = new TestResult() { Outcome = TestOutcome.Passed, Duration = 0.3 },
                TestLog = ComprehensiveDocument
            };
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRun));
            log.Clear();

            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRun));

            Assert.AreEqual("##teamcity[testStdOut name='stepFullName' out='output|n|ninput|n|ntrace|n|nlog' flowId='flow']\n"
                + "##teamcity[testStdErr name='stepFullName' out='error|n|nwarning|n|nfailure' flowId='flow']\n"
                + "##teamcity[testFinished name='stepFullName' duration='300' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void TestStepFinished_TestCase_Failed([Column(true, false)] bool primary)
        {
            var report = new Report();
            var testData = new TestData("id", "testName", "testFullName");
            var testStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "id") { IsPrimary = primary, IsTestCase = true })
            {
                Result = new TestResult() { Outcome = new TestOutcome(TestStatus.Failed, "myError"), Duration = 0.3 },
                TestLog = ComprehensiveDocument
            };
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRun));
            log.Clear();

            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRun));

            Assert.AreEqual("##teamcity[testStdOut name='stepFullName' out='output|n|ninput|n|ntrace|n|nlog' flowId='flow']\n"
                + "##teamcity[testStdErr name='stepFullName' out='error|n|nwarning' flowId='flow']\n"
                + "##teamcity[testFailed name='stepFullName' message='myError' details='failure' flowId='flow']\n"
                + "##teamcity[testFinished name='stepFullName' duration='300' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void TestStepFinished_TestCase_Ignored([Column(true, false)] bool primary)
        {
            var report = new Report();
            var testData = new TestData("id", "testName", "testFullName");
            var testStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "id") { IsPrimary = primary, IsTestCase = true })
            {
                Result = new TestResult() { Outcome = TestOutcome.Ignored, Duration = 0.3 },
                TestLog = ComprehensiveDocument
            };
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRun));
            log.Clear();

            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRun));

            Assert.AreEqual("##teamcity[testStdOut name='stepFullName' out='output|n|ninput|n|ntrace|n|nlog' flowId='flow']\n"
                + "##teamcity[testStdErr name='stepFullName' out='error|n|nfailure' flowId='flow']\n"
                + "##teamcity[testIgnored name='stepFullName' message='warning' flowId='flow']\n"
                + "##teamcity[testFinished name='stepFullName' duration='300' flowId='flow']\n", log.ToString());
        }

        [Test]
        public void TestStepStartedAndFinished_ParallelExecution()
        {
            var report = new Report();
            var testData = new TestData("id", "testName", "testFullName");
            var testStepRunRoot = new TestStepRun(new TestStepData("root", "root", "root", "id") { IsPrimary = true });
            var testStepRunChildA = new TestStepRun(new TestStepData("childA", "childA", "childA", "id") { IsPrimary = true, ParentId = "root" });
            var testStepRunChildB = new TestStepRun(new TestStepData("childB", "childB", "childB", "id") { IsPrimary = true, ParentId = "root" });
            var testStepRunGrandChildA1 = new TestStepRun(new TestStepData("grandChildA1", "grandChildA1", "childA/grandChildA1", "id") { IsPrimary = true, ParentId = "childA" });
            var testStepRunGrandChildA2 = new TestStepRun(new TestStepData("grandChildA2", "grandChildA2", "childA/grandChildA2", "id") { IsPrimary = true, ParentId = "childA" });
            var testStepRunGrandChildB1 = new TestStepRun(new TestStepData("grandChildB1", "grandChildB1", "childB/grandChildB1", "id") { IsPrimary = true, ParentId = "childB" });
            var testStepRunGrandChildB2 = new TestStepRun(new TestStepData("grandChildB2", "grandChildB2", "childB/grandChildB2", "id") { IsPrimary = true, ParentId = "childB" });
            var testStepRunGrandChildB3 = new TestStepRun(new TestStepData("grandChildB3", "grandChildB3", "childB/grandChildB3", "id") { IsPrimary = true, ParentId = "childB" });

            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRunRoot));
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRunChildA));
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRunGrandChildA1));
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRunChildB));
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRunGrandChildA1));
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRunGrandChildA2));
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRunGrandChildB1));
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRunGrandChildB2));
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRunGrandChildB2));
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRunGrandChildB1));
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(report, testData, testStepRunGrandChildB3));
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRunGrandChildB3));
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRunChildB));
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRunGrandChildA2));
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRunChildA));
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(report, testData, testStepRunRoot));

            Assert.AreEqual(
                "##teamcity[testSuiteStarted name='root' flowId='flow']\n" +
                "##teamcity[testSuiteStarted name='childA' flowId='flow']\n" +
                "##teamcity[testSuiteStarted name='childA/grandChildA1' flowId='flow']\n" +
                "##teamcity[testSuiteFinished name='childA/grandChildA1' flowId='flow']\n" +
                "##teamcity[testSuiteStarted name='childA/grandChildA2' flowId='flow']\n" +
                "##teamcity[testSuiteFinished name='childA/grandChildA2' flowId='flow']\n" +
                "##teamcity[testSuiteFinished name='childA' flowId='flow']\n" +
                "##teamcity[testSuiteStarted name='childB' flowId='flow']\n" +
                "##teamcity[testSuiteStarted name='childB/grandChildB1' flowId='flow']\n" +
                "##teamcity[testSuiteFinished name='childB/grandChildB1' flowId='flow']\n" +
                "##teamcity[testSuiteStarted name='childB/grandChildB2' flowId='flow']\n" +
                "##teamcity[testSuiteFinished name='childB/grandChildB2' flowId='flow']\n" +
                "##teamcity[testSuiteStarted name='childB/grandChildB3' flowId='flow']\n" +
                "##teamcity[testSuiteFinished name='childB/grandChildB3' flowId='flow']\n" +
                "##teamcity[testSuiteFinished name='childB' flowId='flow']\n" +
                "##teamcity[testSuiteFinished name='root' flowId='flow']\n",
                log.ToString());
        }

        private sealed class Log : BaseLogger
        {
            private readonly StringBuilder output = new StringBuilder();

            public void Clear()
            {
                output.Length = 0;
            }

            public override string ToString()
            {
                return output.ToString();
            }

            protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
            {
                Assert.AreEqual(LogSeverity.Important, severity);
                Assert.IsNull(exceptionData);

                output.Append(message).Append('\n');
            }
        }
    }
}
