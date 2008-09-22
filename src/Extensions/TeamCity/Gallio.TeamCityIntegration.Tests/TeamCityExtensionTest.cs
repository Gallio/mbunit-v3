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
using System.Text;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Logging;
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

        private static readonly StructuredTestLog ComprehensiveTestLog;

        static TeamCityExtensionTest()
        {
            StructuredTestLogWriter logWriter = new StructuredTestLogWriter();
            logWriter.ConsoleOutput.WriteLine("output");
            logWriter.ConsoleInput.WriteLine("input");
            logWriter.DebugTrace.WriteLine("trace");
            logWriter.Default.WriteLine("log");
            logWriter.ConsoleError.WriteLine("error");
            logWriter.Failures.WriteLine("failure");
            logWriter.Warnings.WriteLine("warning");
            logWriter.Close();

            ComprehensiveTestLog = logWriter.TestLog;
        }

        [SetUp]
        public void SetUp()
        {
            dispatcher = new TestRunnerEventDispatcher();
            log = new Log();

            var ext = new TeamCityExtension();
            ext.Install(dispatcher, log);
        }

        [Test]
        public void InitializeStarted()
        {
            dispatcher.NotifyInitializeStarted(new InitializeStartedEventArgs(new TestRunnerOptions()));

            Assert.AreEqual("##teamcity[progressMessage 'Initializing test runner.']\n", log.ToString());
        }

        [Test]
        public void LoadStarted()
        {
            dispatcher.NotifyLoadStarted(new LoadStartedEventArgs(new TestPackageConfig()));

            Assert.AreEqual("##teamcity[progressMessage 'Loading tests.']\n", log.ToString());
        }

        [Test]
        public void ExploreStarted()
        {
            dispatcher.NotifyExploreStarted(new ExploreStartedEventArgs(new TestExplorationOptions()));

            Assert.AreEqual("##teamcity[progressMessage 'Exploring tests.']\n", log.ToString());
        }

        [Test]
        public void RunStarted()
        {
            dispatcher.NotifyRunStarted(new RunStartedEventArgs(new TestExecutionOptions()));

            Assert.AreEqual("##teamcity[progressStart 'Running tests.']\n", log.ToString());
        }

        [Test]
        public void RunFinished()
        {
            dispatcher.NotifyRunFinished(new RunFinishedEventArgs(true, new Report()));

            Assert.AreEqual("##teamcity[progressFinish 'Running tests.']\n", log.ToString());
        }

        [Test]
        public void UnloadFinished()
        {
            dispatcher.NotifyUnloadFinished(new UnloadFinishedEventArgs(true));

            Assert.AreEqual("##teamcity[progressMessage 'Unloaded tests.']\n", log.ToString());
        }

        [Test]
        public void DisposeFinished()
        {
            dispatcher.NotifyDisposeFinished(new DisposeFinishedEventArgs(true));

            Assert.AreEqual("##teamcity[progressMessage 'Disposed test runner.']\n", log.ToString());
        }

        [Test]
        public void TestStepStarted_NonPrimary()
        {
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("id", "stepName", "stepFullName", "id") { IsPrimary = false })));

            Assert.AreEqual("", log.ToString());
        }

        [Test]
        public void TestStepStarted_Root()
        {
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(
                new Report(),
                new TestData("id", "root", "testFullName"),
                new TestStepRun(new TestStepData("id", "root", "", "id") { IsPrimary = true })));

            Assert.AreEqual("", log.ToString());
        }

        [Test]
        public void TestStepStarted_PrimaryNonTestCase()
        {
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("id", "stepName", "stepFullName", "id") { IsPrimary = true, IsTestCase = false })));

            Assert.AreEqual("##teamcity[testSuiteStarted name='stepFullName']\n", log.ToString());
        }

        [Test]
        public void TestStepStarted_PrimaryTestCase()
        {
            dispatcher.NotifyTestStepStarted(new TestStepStartedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("id", "stepName", "stepFullName", "id") { IsPrimary = true, IsTestCase = true })));

            Assert.AreEqual("##teamcity[testStarted name='stepFullName' captureStandardOutput=\'false\']\n", log.ToString());
        }

        [Test]
        public void TestStepFinished_NonPrimary()
        {
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("id", "stepName", "stepFullName", "id") { IsPrimary = false })));

            Assert.AreEqual("", log.ToString());
        }

        [Test]
        public void TestStepFinished_Root()
        {
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(
                new Report(),
                new TestData("id", "root", "testFullName"),
                new TestStepRun(new TestStepData("id", "root", "", "id") { IsPrimary = true })));

            Assert.AreEqual("", log.ToString());
        }

        [Test]
        public void TestStepFinished_PrimaryNonTestCase()
        {
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("id", "stepName", "stepFullName", "id") { IsPrimary = true, IsTestCase = false })));

            Assert.AreEqual("##teamcity[testSuiteFinished name='stepFullName']\n", log.ToString());
        }

        [Test]
        public void TestStepFinished_PrimaryTestCase_Passed()
        {
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("id", "stepName", "stepFullName", "id") { IsPrimary = true, IsTestCase = true }) {
                    Result = new TestResult() { Outcome = TestOutcome.Passed, Duration = 0.3 },
                    TestLog = ComprehensiveTestLog
                }));

            Assert.AreEqual("##teamcity[testStdOut name='stepFullName' out='output|n|ninput|n|ntrace|n|nlog']\n"
                + "##teamcity[testStdErr name='stepFullName' out='error|n|nwarning|n|nfailure']\n"
                + "##teamcity[testFinished name='stepFullName' duration='300']\n", log.ToString());
        }

        [Test]
        public void TestStepFinished_PrimaryTestCase_Failed()
        {
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("id", "stepName", "stepFullName", "id") { IsPrimary = true, IsTestCase = true })
                {
                    Result = new TestResult() { Outcome = new TestOutcome(TestStatus.Failed, "myError"), Duration = 0.3 },
                    TestLog = ComprehensiveTestLog
                }));

            Assert.AreEqual("##teamcity[testStdOut name='stepFullName' out='output|n|ninput|n|ntrace|n|nlog']\n"
                + "##teamcity[testStdErr name='stepFullName' out='error|n|nwarning']\n"
                + "##teamcity[testFailed name='stepFullName' message='myError' details='failure']\n"
                + "##teamcity[testFinished name='stepFullName' duration='300']\n", log.ToString());
        }

        [Test]
        public void TestStepFinished_PrimaryTestCase_Ignored()
        {
            dispatcher.NotifyTestStepFinished(new TestStepFinishedEventArgs(
                new Report(),
                new TestData("id", "testName", "testFullName"),
                new TestStepRun(new TestStepData("id", "stepName", "stepFullName", "id") { IsPrimary = true, IsTestCase = true })
                {
                    Result = new TestResult() { Outcome = TestOutcome.Ignored, Duration = 0.3 },
                    TestLog = ComprehensiveTestLog
                }));

            Assert.AreEqual("##teamcity[testStdOut name='stepFullName' out='output|n|ninput|n|ntrace|n|nlog']\n"
                + "##teamcity[testStdErr name='stepFullName' out='error|n|nfailure']\n"
                + "##teamcity[testIgnored name='stepFullName' message='warning']\n"
                + "##teamcity[testFinished name='stepFullName' duration='300']\n", log.ToString());
        }

        private sealed class Log : BaseLogger
        {
            private readonly StringBuilder output = new StringBuilder();

            public override string ToString()
            {
                return output.ToString();
            }

            protected override void LogImpl(LogSeverity severity, string message, Exception exception)
            {
                Assert.AreEqual(LogSeverity.Important, severity);
                Assert.IsNull(exception);

                output.Append(message).Append('\n');
            }
        }
    }
}
