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

using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Models
{
    public class TestStatisticsTests
    {
        [Test]
        public void Passed_should_be_incremented_if_completed_test_step_passed()
        {
            var testStatistics = new TestStatistics();

            testStatistics.Handle(GetTestStepFinished(TestStatus.Passed));

            Assert.AreEqual(1, testStatistics.Passed.Value);
        }

        private static TestStepFinished GetTestStepFinished(TestStatus testStatus)
        {
            var testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testId"))
            {
                Step = new TestStepData("id", "name", "fullName", "testId") { IsTestCase = true },
                Result = new TestResult(new TestOutcome(testStatus))
            };
            return new TestStepFinished(null, testStepRun);
        }

        [Test]
        public void Failed_should_be_incremented_if_completed_test_step_failed()
        {
            var testStatistics = new TestStatistics();

            testStatistics.Handle(GetTestStepFinished(TestStatus.Failed));

            Assert.AreEqual(1, testStatistics.Failed.Value);
        }

        [Test]
        public void Skipped_should_be_incremented_if_completed_test_step_was_skipped()
        {
            var testStatistics = new TestStatistics();

            testStatistics.Handle(GetTestStepFinished(TestStatus.Skipped));

            Assert.AreEqual(1, testStatistics.Skipped.Value);
        }

        [Test]
        public void Inconclusive_should_be_incremented_if_completed_test_step_was_inconclusive()
        {
            var testStatistics = new TestStatistics();

            testStatistics.Handle(GetTestStepFinished(TestStatus.Inconclusive));

            Assert.AreEqual(1, testStatistics.Inconclusive.Value);
        }

        [Test]
        public void Reset_should_set_passed_to_zero()
        {
            var testStatistics = new TestStatistics();
            testStatistics.Handle(GetTestStepFinished(TestStatus.Passed));

            testStatistics.Handle(new TestsReset());

            Assert.AreEqual(0, testStatistics.Passed.Value);
        }

        [Test]
        public void Reset_should_set_failed_to_zero()
        {
            var testStatistics = new TestStatistics();
            testStatistics.Handle(GetTestStepFinished(TestStatus.Failed));

            testStatistics.Handle(new TestsReset());

            Assert.AreEqual(0, testStatistics.Failed.Value);
        }

        [Test]
        public void Reset_should_set_skipped_to_zero()
        {
            var testStatistics = new TestStatistics();
            testStatistics.Handle(GetTestStepFinished(TestStatus.Skipped));

            testStatistics.Handle(new TestsReset());

            Assert.AreEqual(0, testStatistics.Skipped.Value);
        }

        [Test]
        public void Reset_should_set_inconclusive_to_zero()
        {
            var testStatistics = new TestStatistics();
            testStatistics.Handle(GetTestStepFinished(TestStatus.Skipped));

            testStatistics.Handle(new TestsReset());

            Assert.AreEqual(0, testStatistics.Inconclusive.Value);
        }

        [Test]
        public void Count_should_not_be_updated_if_completed_test_step_is_not_test_case()
        {
            var testStatistics = new TestStatistics();
            var testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testId"))
            {
                Result = new TestResult(new TestOutcome(TestStatus.Passed))
            };

            testStatistics.Handle(new TestStepFinished(null, testStepRun));

            Assert.AreEqual(0, testStatistics.Passed.Value);
        }
    }
}
