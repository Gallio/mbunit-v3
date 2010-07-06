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
using System.Threading;
using Gallio.Common.Collections;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(ThreadedRepeatAttribute))]
    [RunSample(typeof(ThreadedRepeatTestSample))]
    [RunSample(typeof(ThreadedRepeatFixtureSample))]
    public class ThreadedRepeatTest : BaseTestWithSampleRunner
    {
        [Test]
        public void RunTestRepeatedly()
        {
            TestStepRun testRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ThreadedRepeatTestSample).GetMethod("Test")));

            AssertLogContains(testRun, "9 of 10 threaded repetitions passed.");
            Assert.AreEqual(TestOutcome.Failed, testRun.Result.Outcome);

            IList<TestStepRun> testSteps = testRun.Children;
            Assert.Count(10, testSteps, "Expected 10 repetitions represented as steps.");

            for (int i = 0; i < 10; i++)
            {
                string name = "Threaded Repetition #" + (i + 1);
                TestStepRun testStep = GenericCollectionUtils.Find(testSteps, candidate => candidate.Step.Name == name);
                AssertLogContains(testStep, "Run: " + name);

                if (i == 1)
                {
                    Assert.AreEqual(TestOutcome.Failed, testStep.Result.Outcome);
                    AssertLogContains(testStep, "Boom", MarkupStreamNames.Failures);
                }
                else
                {
                    Assert.AreEqual(TestOutcome.Passed, testStep.Result.Outcome);
                }
            }
        }

        [Test]
        public void RunFixtureRepeatedly()
        {
            TestStepRun fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(ThreadedRepeatFixtureSample)));

            AssertLogContains(fixtureRun, "9 of 10 threaded repetitions passed.");
            Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);

            IList<TestStepRun> fixtureSteps = fixtureRun.Children;
            Assert.Count(10, fixtureSteps, "Expected 10 repetitions represented as steps.");

            for (int i = 0; i < 10; i++)
            {
                string name = "Threaded Repetition #" + (i + 1);
                TestStepRun fixtureStep = GenericCollectionUtils.Find(fixtureSteps, candidate => candidate.Step.Name == name);
                Assert.Count(1, fixtureStep.Children);

                TestStepRun testRun = fixtureStep.Children[0];
                AssertLogContains(testRun, "Run: " + name);

                if (i == 1)
                {
                    Assert.AreEqual(TestOutcome.Failed, fixtureStep.Result.Outcome);
                    Assert.AreEqual(TestOutcome.Failed, testRun.Result.Outcome);
                    AssertLogContains(testRun, "Boom", MarkupStreamNames.Failures);
                }
                else
                {
                    Assert.AreEqual(TestOutcome.Passed, fixtureStep.Result.Outcome);
                    Assert.AreEqual(TestOutcome.Passed, testRun.Result.Outcome);
                }
            }
        }

        [Explicit("Sample")]
        internal class ThreadedRepeatTestSample
        {
            [Test, ThreadedRepeat(10)]
            public void Test()
            {
                string name = TestContext.CurrentContext.TestStep.Name;

                TestLog.WriteLine("Run: {0}", name);
                if (name == "Threaded Repetition #2")
                    Assert.Fail("Boom");
            }
        }

        [Explicit("Sample")]
        [ThreadedRepeat(10)]
        internal class ThreadedRepeatFixtureSample
        {
            [Test]
            public void Test()
            {
                string name = TestContext.CurrentContext.Parent.TestStep.Name;

                TestLog.WriteLine("Run: {0}", name);
                if (name == "Threaded Repetition #2")
                    Assert.Fail("Boom");
            }
        }
    }
}
