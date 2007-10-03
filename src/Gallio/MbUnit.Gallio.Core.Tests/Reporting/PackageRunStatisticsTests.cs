// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

extern alias MbUnit2;
using MbUnit.Core.Model;
using MbUnit.Core.Reporting;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Reporting
{
    [TestFixture]
    [TestsOn(typeof(PackageRunStatistics))]
    [Author("Vadim")]
    public class PackageRunStatisticsTests
    {
        private PackageRunStatistics _prStat;

        [SetUp]
        public void TestStart()
        {
            _prStat = new PackageRunStatistics();
        }

        [Test]
        public void FormatTestCaseResultSummaryNothingSet()
        {
            Assert.AreEqual("Run: 0, Passed: 0, Failed: 0, Inconclusive: 0, Ignored: 0, Skipped: 0.", _prStat.FormatTestCaseResultSummary());
        }

        [Test]
        public void FormatTestCaseResultSummaryAllValuesSet()
        {
            SetPackageRunStaticsProperties();
            Assert.AreEqual("Run: 2, Passed: 3, Failed: 4, Inconclusive: 5, Ignored: 6, Skipped: 7.", _prStat.FormatTestCaseResultSummary());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void MergeStepStatisticsWithNullStepRun()
        {
            _prStat.MergeStepStatistics(null, true);
        }

        [RowTest]
        [Row(TestStatus.NotRun, TestOutcome.Failed, false, 3, 8, 4, 6, 7, 2, 3, 4, 5)]
        [Row(TestStatus.NotRun, TestOutcome.Passed, true, 3, 8, 5, 6, 7, 2, 3, 4, 5)]
        [Row(TestStatus.Ignored, TestOutcome.Passed, true, 3, 8, 5, 7, 7, 2, 3, 4, 5)]
        [Row(TestStatus.Skipped, TestOutcome.Passed, true, 3, 8, 5, 6, 8, 2, 3, 4, 5)]
        [Row(TestStatus.Executed, TestOutcome.Passed, true, 3, 8, 5, 6, 7, 3, 4, 4, 5)]
        [Row(TestStatus.Canceled, TestOutcome.Failed, true, 3, 8, 5, 6, 7, 3, 3, 5, 5)]
        [Row(TestStatus.Canceled, TestOutcome.Inconclusive, true, 3, 8, 5, 6, 7, 3, 3, 4, 6)]
        public void MergeStepStatisticsTest(TestStatus status, TestOutcome outcome, bool isTestCase, int stepRunAsrtCnt
            , int asrtCnt, int tstCnt, int ignrCnt, int skipCnt, int runCnt, int passCnt, int failCnt, int inclsvCnt)
        {
            SetPackageRunStaticsProperties();
            StepRun stepRun = new StepRun(new StepData("stepId", "stepName", "fullName", "testId"));
            stepRun.Result.Status = status;
            stepRun.Result.Outcome = outcome;
            stepRun.Result.AssertCount = stepRunAsrtCnt;
            _prStat.MergeStepStatistics(stepRun, isTestCase);
            AssertProperties(asrtCnt, tstCnt, ignrCnt, skipCnt, runCnt, passCnt, failCnt, inclsvCnt);
        }

        private void SetPackageRunStaticsProperties()
        {
            _prStat.AssertCount = 5;
            _prStat.RunCount = 2;
            _prStat.PassCount = 3;
            _prStat.FailureCount = 4;
            _prStat.InconclusiveCount = 5;
            _prStat.IgnoreCount = 6;
            _prStat.SkipCount = 7;
            _prStat.TestCount = 4;
        }

        private void AssertProperties(int asrtCnt, int tstCnt, int ignrCnt, int skipCnt, int runCnt, int passCnt, int failCnt, int inclsvCnt)
        {
            Assert.AreEqual(asrtCnt, _prStat.AssertCount);
            Assert.AreEqual(tstCnt, _prStat.TestCount);
            Assert.AreEqual(ignrCnt, _prStat.IgnoreCount);
            Assert.AreEqual(skipCnt, _prStat.SkipCount);
            Assert.AreEqual(runCnt, _prStat.RunCount);
            Assert.AreEqual(passCnt, _prStat.PassCount);
            Assert.AreEqual(failCnt, _prStat.FailureCount);
            Assert.AreEqual(inclsvCnt, _prStat.InconclusiveCount);
        }
    }
}