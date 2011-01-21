// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Gallio.Reports.Vtl;
using Gallio.Runner.Reports.Schema;
using Gallio.Model;
using Gallio.Model.Schema;

namespace Gallio.Tests.Reports.Vtl
{
    [TestFixture]
    [TestsOn(typeof(TestStepRunTreeStatistics))]
    public class TestStepRunTreeStatisticsTest
    {
        private TestStepRun CreateFakeRun(string id, TestOutcome outcome, params TestStepRun[] children)
        {
            var run = new TestStepRun(new TestStepData(id, id, id, id));
            run.Result = new TestResult(outcome);
            run.Children.AddRange(children);
            return run;
        }

        [Test]
        public void Count()
        {
            // Root 
            //   +- Child1 (Failed)
            //   +- Child2 (Passed)
            //   +- Child3
            //        +- Child31 (Skipped)
            //        +- Child32
            //             +- Child321 (Passed)
            //             +- Child322 (Failed)
            //             +- Child323 (Pending)
            //             +- Child324 (Ignored)
            //        +- Child33
            //             +- Child331 (Passed)
            //             +- Child332 (Passed)
            //             +- Child333 (Canceled)

            var child1 = CreateFakeRun("1", TestOutcome.Failed);
            var child2 = CreateFakeRun("2", TestOutcome.Passed);
            var child331 = CreateFakeRun("331", TestOutcome.Passed);
            var child332 = CreateFakeRun("332", TestOutcome.Passed);
            var child333 = CreateFakeRun("333", TestOutcome.Canceled);
            var child33 = CreateFakeRun("33", TestOutcome.Passed, child331, child332, child333);
            var child321 = CreateFakeRun("321", TestOutcome.Passed);
            var child322 = CreateFakeRun("322", TestOutcome.Failed);
            var child323 = CreateFakeRun("323", TestOutcome.Pending);
            var child324 = CreateFakeRun("324", TestOutcome.Ignored);
            var child32 = CreateFakeRun("32", TestOutcome.Failed, child321, child322, child323, child324);
            var child31 = CreateFakeRun("31", TestOutcome.Skipped);
            var child3 = CreateFakeRun("3", TestOutcome.Failed, child31, child32, child33);
            var root = CreateFakeRun("Root", TestOutcome.Failed, child1, child2, child3);

            // Root statistics.
            var rootStatistics = new TestStepRunTreeStatistics(root);
            Assert.AreEqual(10, rootStatistics.RunCount);
            Assert.AreEqual(4, rootStatistics.PassedCount);
            Assert.AreEqual(2, rootStatistics.FailedCount);
            Assert.AreEqual(3, rootStatistics.SkippedCount);
            Assert.AreEqual(1, rootStatistics.InconclusiveCount);
            Assert.AreEqual(4, rootStatistics.SkippedOrInconclusiveCount);
            Assert.AreEqual("4 passed", rootStatistics.FormatPassedCountWithCategories());
            Assert.AreEqual("2 failed", rootStatistics.FormatFailedCountWithCategories());
            Assert.AreEqual("3 skipped (1 pending, 1 ignored)", rootStatistics.FormatSkippedCountWithCategories());
            Assert.AreEqual("1 inconclusive", rootStatistics.FormatInconclusiveCountWithCategories());

            // Child node statistics
            var child33Statistics = new TestStepRunTreeStatistics(child33);
            Assert.AreEqual(3, child33Statistics.RunCount);
            Assert.AreEqual(2, child33Statistics.PassedCount);
            Assert.AreEqual(0, child33Statistics.FailedCount);
            Assert.AreEqual(0, child33Statistics.SkippedCount);
            Assert.AreEqual(1, child33Statistics.InconclusiveCount);
            Assert.AreEqual(1, child33Statistics.SkippedOrInconclusiveCount);
            Assert.AreEqual("2 passed", rootStatistics.FormatPassedCountWithCategories());
            Assert.AreEqual("0 failed", rootStatistics.FormatFailedCountWithCategories());
            Assert.AreEqual("0 skipped", rootStatistics.FormatSkippedCountWithCategories());
            Assert.AreEqual("1 inconclusive (1 canceled)", rootStatistics.FormatInconclusiveCountWithCategories());
        }
    }
}
