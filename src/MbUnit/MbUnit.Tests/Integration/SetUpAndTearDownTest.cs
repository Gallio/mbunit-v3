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
using System.Linq;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    [RunSample(typeof(Fixture))]
    [RunSample(typeof(FixtureWithFailingSetUp))]
    [RunSample(typeof(FixtureWithFailingTest))]
    [RunSample(typeof(FixtureWithFailingTearDown))]
    public class SetUpAndTearDownTest : BaseTestWithSampleRunner
    {
        [Test]
        public void WhenNoFailure_FixturePasses()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(Fixture)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Passed, fixtureRun.Result.Outcome);
                Assert.IsFalse(fixtureRun.Step.IsTestCase);
            });
        }

        [Test]
        public void WhenNoFailure_TestPasses()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(Fixture)));
            var testRun = fixtureRun.Children[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Passed, testRun.Result.Outcome);
                Assert.IsTrue(testRun.Step.IsTestCase);
                AssertLogContains(testRun, "[SetUp] Outcome: passed\n[Test] Outcome: passed\n[TearDown] Outcome: passed");
            });
        }

        [Test]
        public void WhenSetUpFails_FixtureFails()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingSetUp)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);
                Assert.IsFalse(fixtureRun.Step.IsTestCase);
            });
        }

        [Test]
        public void WhenSetUpFails_TestFailsAndTestMethodDoesNotRun()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingSetUp)));
            var testRun = fixtureRun.Children[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, testRun.Result.Outcome);
                Assert.IsTrue(testRun.Step.IsTestCase);
                AssertLogContains(testRun, "[SetUp] Outcome: passed\n[TearDown] Outcome: failed");
            });
        }

        [Test]
        public void WhenTestFails_FixtureFails()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingTest)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);
                Assert.IsFalse(fixtureRun.Step.IsTestCase);
            });
        }

        [Test]
        public void WhenTestFails_TestFails()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingTest)));
            var testRun = fixtureRun.Children[0];
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(testRun);
                Assert.AreEqual(TestOutcome.Failed, testRun.Result.Outcome);
                Assert.IsTrue(testRun.Step.IsTestCase);
                AssertLogContains(testRun, "[SetUp] Outcome: passed\n[Test] Outcome: passed\n[TearDown] Outcome: failed");
            });
        }

        [Test]
        public void WhenTearDownFails_FixtureFails()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingTearDown)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);
                Assert.IsFalse(fixtureRun.Step.IsTestCase);
            });
        }

        [Test]
        public void WhenTearDownFails_TestFails()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingTearDown)));
            var testRun = fixtureRun.Children[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, testRun.Result.Outcome);
                Assert.IsTrue(testRun.Step.IsTestCase);
                AssertLogContains(testRun, "[SetUp] Outcome: passed\n[Test] Outcome: passed\n[TearDown] Outcome: passed");
            });
        }

        [Explicit("Sample")]
        public class Fixture
        {
            [SetUp]
            public virtual void SetUp()
            {
                TestLog.WriteLine("[SetUp] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }

            [Test]
            public virtual void Test()
            {
                TestLog.WriteLine("[Test] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }

            [TearDown]
            public virtual void TearDown()
            {
                TestLog.WriteLine("[TearDown] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }
        }

        [Explicit("Sample")]
        public class FixtureWithFailingSetUp : Fixture
        {
            public override void SetUp()
            {
                base.SetUp();
                Assert.Fail("Boom");
            }
        }

        [Explicit("Sample")]
        public class FixtureWithFailingTest : Fixture
        {
            public override void Test()
            {
                base.Test();
                Assert.Fail("Boom");
            }
        }

        [Explicit("Sample")]
        public class FixtureWithFailingTearDown : Fixture
        {
            public override void TearDown()
            {
                base.TearDown();
                Assert.Fail("Boom");
            }
        }
    }
}
