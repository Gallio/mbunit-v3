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
    [RunSample(typeof(FixtureWithFailingFixtureSetUp))]
    [RunSample(typeof(FixtureWithFailingTest))]
    [RunSample(typeof(FixtureWithFailingFixtureTearDown))]
    public class FixtureSetUpAndFixtureTearDownTest : BaseTestWithSampleRunner
    {
        [Test]
        public void WhenNoFailure_FixturePasses()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(Fixture)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Passed, fixtureRun.Result.Outcome);
                Assert.IsFalse(fixtureRun.Step.IsTestCase);
                AssertLogContains(fixtureRun, "[FixtureSetUp] Outcome: passed\n[FixtureTearDown] Outcome: passed");
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
                AssertLogContains(testRun, "[Test] Outcome: passed");
            });
        }

        [Test]
        public void WhenFixtureSetUpFails_FixtureFailsAndBecomesATestCase()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingFixtureSetUp)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);
                Assert.IsTrue(fixtureRun.Step.IsTestCase);
                AssertLogContains(fixtureRun, "[FixtureSetUp] Outcome: passed\n[FixtureTearDown] Outcome: failed");
            });
        }

        [Test]
        public void WhenFixtureSetUpFails_TestDoesNotRun()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingFixtureSetUp)));
            Assert.AreEqual(0, fixtureRun.Children.Count);
        }

        [Test]
        public void WhenTestFails_FixtureFails()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingTest)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);
                Assert.IsFalse(fixtureRun.Step.IsTestCase);
                AssertLogContains(fixtureRun, "[FixtureSetUp] Outcome: passed\n[FixtureTearDown] Outcome: failed");
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
                AssertLogContains(testRun, "[Test] Outcome: passed");
            });
        }

        [Test]
        public void WhenFixtureTearDownFails_FixtureFails()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingFixtureTearDown)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);
                Assert.IsFalse(fixtureRun.Step.IsTestCase);
                AssertLogContains(fixtureRun, "[FixtureSetUp] Outcome: passed\n[FixtureTearDown] Outcome: passed");
            });
        }

        [Test]
        public void WhenFixtureTearDownFails_TestStillPassesBecauseFixtureTearDownHappensLater()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingFixtureTearDown)));
            var testRun = fixtureRun.Children[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Passed, testRun.Result.Outcome);
                Assert.IsTrue(testRun.Step.IsTestCase);
                AssertLogContains(testRun, "[Test] Outcome: passed");
            });
        }

        [Explicit("Sample")]
        public class Fixture
        {
            [FixtureSetUp]
            public virtual void FixtureSetUp()
            {
                TestLog.WriteLine("[FixtureSetUp] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }

            [Test]
            public virtual void Test()
            {
                TestLog.WriteLine("[Test] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }

            [FixtureTearDown]
            public virtual void FixtureTearDown()
            {
                TestLog.WriteLine("[FixtureTearDown] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }
        }

        [Explicit("Sample")]
        public class FixtureWithFailingFixtureSetUp : Fixture
        {
            public override void FixtureSetUp()
            {
                base.FixtureSetUp();
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
        public class FixtureWithFailingFixtureTearDown : Fixture
        {
            public override void FixtureTearDown()
            {
                base.FixtureTearDown();
                Assert.Fail("Boom");
            }
        }
    }
}
