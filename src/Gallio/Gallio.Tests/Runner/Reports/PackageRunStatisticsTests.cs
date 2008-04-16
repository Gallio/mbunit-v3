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

using Gallio.Runner.Reports;
using Gallio.Model;
using Gallio.Model.Serialization;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(Statistics))]
    public class PackageRunStatisticsTests
    {
        private Statistics stats;

        [SetUp]
        public void TestStart()
        {
            stats = new Statistics();
        }

        [Test]
        public void FormatTestCaseResultSummaryNothingSet()
        {
            Assert.AreEqual("0 run, 0 passed, 0 failed, 0 inconclusive, 0 skipped", stats.FormatTestCaseResultSummary());
        }

        [Test]
        public void FormatTestCaseResultSummaryAllValuesSet()
        {
            SetPackageRunStatisticsProperties();
            Assert.AreEqual("3 run, 20 passed, 123 failed (41 error, 42 timeout), 61 inconclusive (31 canceled), 33 skipped (11 ignored, 12 pending)", stats.FormatTestCaseResultSummary());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void MergeStepStatisticsWithNullTestStepRun()
        {
            stats.MergeStepStatistics(null);
        }

        [Test]
        [Row(TestStatus.Skipped, true)]
        [Row(TestStatus.Skipped, false)]
        [Row(TestStatus.Passed, true)]
        [Row(TestStatus.Passed, false)]
        [Row(TestStatus.Inconclusive, true)]
        [Row(TestStatus.Inconclusive, false)]
        [Row(TestStatus.Failed, true)]
        [Row(TestStatus.Failed, false)]
        public void MergeStepStatistics(TestStatus status, bool isTestCase)
        {
            TestStepRun testStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "fullName", "testId"));
            testStepRun.Result.Outcome = new TestOutcome(status);
            testStepRun.Result.AssertCount = 3;
            testStepRun.Step.IsTestCase = isTestCase;

            stats.MergeStepStatistics(testStepRun);
            Assert.AreEqual(3, stats.AssertCount);
            Assert.AreEqual(1, stats.StepCount);
            Assert.AreEqual(isTestCase ? 1 : 0, stats.TestCount);
            Assert.AreEqual(isTestCase && status != TestStatus.Skipped ? 1 : 0, stats.RunCount);

            Assert.AreEqual(isTestCase && status == TestStatus.Skipped ? 1 : 0, stats.SkippedCount);
            Assert.AreEqual(isTestCase && status == TestStatus.Passed ? 1 : 0, stats.PassedCount);
            Assert.AreEqual(isTestCase && status == TestStatus.Inconclusive ? 1 : 0, stats.InconclusiveCount);
            Assert.AreEqual(isTestCase && status == TestStatus.Failed ? 1 : 0, stats.FailedCount);

            Assert.AreEqual(isTestCase ? 1 : 0, stats.GetOutcomeCount(new TestOutcome(status)));
        }

        [Test]
        public void SetAndGetProperties()
        {
            SetPackageRunStatisticsProperties();

            Assert.AreEqual(1, stats.AssertCount);
            Assert.AreEqual(2, stats.TestCount);
            Assert.AreEqual(3, stats.RunCount);
            Assert.AreEqual(4, stats.StepCount);
            Assert.AreEqual(1.3, stats.Duration);

            Assert.AreEqual(33, stats.SkippedCount);
            Assert.AreEqual(10, stats.GetOutcomeCount(TestOutcome.Skipped));
            Assert.AreEqual(11, stats.GetOutcomeCount(TestOutcome.Ignored));
            Assert.AreEqual(12, stats.GetOutcomeCount(TestOutcome.Pending));

            Assert.AreEqual(20, stats.PassedCount);
            Assert.AreEqual(20, stats.GetOutcomeCount(TestOutcome.Passed));

            Assert.AreEqual(61, stats.InconclusiveCount);
            Assert.AreEqual(30, stats.GetOutcomeCount(TestOutcome.Inconclusive));
            Assert.AreEqual(31, stats.GetOutcomeCount(TestOutcome.Canceled));

            Assert.AreEqual(123, stats.FailedCount);
            Assert.AreEqual(40, stats.GetOutcomeCount(TestOutcome.Failed));
            Assert.AreEqual(41, stats.GetOutcomeCount(TestOutcome.Error));
            Assert.AreEqual(42, stats.GetOutcomeCount(TestOutcome.Timeout));
        }

        private void SetPackageRunStatisticsProperties()
        {
            stats.AssertCount = 1;
            stats.TestCount = 2;
            stats.RunCount = 3;
            stats.StepCount = 4;
            stats.Duration = 1.3;

            stats.SkippedCount = 33;
            stats.SetOutcomeCount(TestOutcome.Skipped, 10);
            stats.SetOutcomeCount(TestOutcome.Ignored, 11);
            stats.SetOutcomeCount(TestOutcome.Pending, 12);

            stats.PassedCount = 20;
            stats.SetOutcomeCount(TestOutcome.Passed, 20);

            stats.InconclusiveCount = 61;
            stats.SetOutcomeCount(TestOutcome.Inconclusive, 30);
            stats.SetOutcomeCount(TestOutcome.Canceled, 31);

            stats.FailedCount = 123;
            stats.SetOutcomeCount(TestOutcome.Failed, 40);
            stats.SetOutcomeCount(TestOutcome.Error, 41);
            stats.SetOutcomeCount(TestOutcome.Timeout, 42);
        }
    }
}