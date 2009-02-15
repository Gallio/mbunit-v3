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
using System.Collections.Generic;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(RepeatAttribute))]
    [RunSample(typeof(RepeatTestSample))]
    [RunSample(typeof(RepeatFixtureSample))]
    public class RepeatTest : BaseTestWithSampleRunner
    {
        [Test]
        public void RunTestRepeatedly()
        {
            TestStepRun testRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(RepeatTestSample).GetMethod("Test")));

            AssertLogContains(testRun, "3 of 4 repetitions passed.");
            Assert.AreEqual(TestOutcome.Failed, testRun.Result.Outcome);

            IList<TestStepRun> testSteps = testRun.Children;
            Assert.AreEqual(4, testSteps.Count, "Expected 4 repetitions represented as steps.");

            Assert.AreEqual("Repetition #1", testSteps[0].Step.Name);
            AssertLogContains(testSteps[0], "Run: Repetition #1");
            Assert.AreEqual(TestOutcome.Passed, testSteps[0].Result.Outcome);

            Assert.AreEqual("Repetition #2", testSteps[1].Step.Name);
            AssertLogContains(testSteps[1], "Run: Repetition #2");
            AssertLogContains(testSteps[1], "Boom", TestLogStreamNames.Failures);
            Assert.AreEqual(TestOutcome.Failed, testSteps[1].Result.Outcome);

            Assert.AreEqual("Repetition #3", testSteps[2].Step.Name);
            AssertLogContains(testSteps[2], "Run: Repetition #3");
            Assert.AreEqual(TestOutcome.Passed, testSteps[2].Result.Outcome);

            Assert.AreEqual("Repetition #4", testSteps[3].Step.Name);
            AssertLogContains(testSteps[3], "Run: Repetition #4");
            Assert.AreEqual(TestOutcome.Passed, testSteps[3].Result.Outcome);
        }

        [Test]
        public void RunFixtureRepeatedly()
        {
            TestStepRun fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(RepeatFixtureSample)));
            AssertLogContains(fixtureRun, "3 of 4 repetitions passed.");
            Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);

            IList<TestStepRun> fixtureSteps = fixtureRun.Children;
            Assert.AreEqual(4, fixtureSteps.Count, "Expected 4 repetitions represented as steps.");

            Assert.AreEqual("Repetition #1", fixtureSteps[0].Step.Name);
            Assert.AreEqual(TestOutcome.Passed, fixtureSteps[0].Result.Outcome);

            Assert.AreEqual(1, fixtureSteps[0].Children.Count);
            Assert.AreEqual("Test", fixtureSteps[0].Children[0].Step.Name);
            AssertLogContains(fixtureSteps[0].Children[0], "Run: Repetition #1");
            Assert.AreEqual(TestOutcome.Passed, fixtureSteps[0].Children[0].Result.Outcome);

            Assert.AreEqual("Repetition #2", fixtureSteps[1].Step.Name);
            Assert.AreEqual(TestOutcome.Failed, fixtureSteps[1].Result.Outcome);

            Assert.AreEqual(1, fixtureSteps[1].Children.Count);
            Assert.AreEqual("Test", fixtureSteps[1].Children[0].Step.Name);
            AssertLogContains(fixtureSteps[1].Children[0], "Run: Repetition #2");
            AssertLogContains(fixtureSteps[1].Children[0], "Boom", TestLogStreamNames.Failures);
            Assert.AreEqual(TestOutcome.Failed, fixtureSteps[1].Children[0].Result.Outcome);

            Assert.AreEqual("Repetition #3", fixtureSteps[2].Step.Name);
            Assert.AreEqual(TestOutcome.Passed, fixtureSteps[2].Result.Outcome);

            Assert.AreEqual(1, fixtureSteps[2].Children.Count);
            Assert.AreEqual("Test", fixtureSteps[2].Children[0].Step.Name);
            AssertLogContains(fixtureSteps[2].Children[0], "Run: Repetition #3");
            Assert.AreEqual(TestOutcome.Passed, fixtureSteps[2].Children[0].Result.Outcome);

            Assert.AreEqual("Repetition #4", fixtureSteps[3].Step.Name);
            Assert.AreEqual(TestOutcome.Passed, fixtureSteps[3].Result.Outcome);

            Assert.AreEqual(1, fixtureSteps[3].Children.Count);
            Assert.AreEqual("Test", fixtureSteps[3].Children[0].Step.Name);
            AssertLogContains(fixtureSteps[3].Children[0], "Run: Repetition #4");
            Assert.AreEqual(TestOutcome.Passed, fixtureSteps[3].Children[0].Result.Outcome);
        }

        [Explicit("Sample")]
        internal class RepeatTestSample
        {
            [Test, Repeat(4)]
            public void Test()
            {
                string name = TestContext.CurrentContext.TestStep.Name;

                TestLog.WriteLine("Run: {0}", name);
                if (name == "Repetition #2")
                    Assert.Fail("Boom");
            }
        }

        [Explicit("Sample")]
        [Repeat(4)]
        internal class RepeatFixtureSample
        {
            [Test]
            public void Test()
            {
                string name = TestContext.CurrentContext.Parent.TestStep.Name;

                TestLog.WriteLine("Run: {0}", name);
                if (name == "Repetition #2")
                    Assert.Fail("Boom");
            }
        }
    }
}
