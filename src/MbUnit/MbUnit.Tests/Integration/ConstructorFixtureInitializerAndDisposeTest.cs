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
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    [RunSample(typeof(Fixture))]
    [RunSample(typeof(FixtureWithFailingConstructor))]
    [RunSample(typeof(FixtureWithFailingFixtureInitializer))]
    [RunSample(typeof(FixtureWithFailingTest))]
    [RunSample(typeof(FixtureWithFailingDispose))]
    public class ConstructorFixtureInitializerAndDisposeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void WhenNoFailure_FixturePasses()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(Fixture)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Passed, fixtureRun.Result.Outcome);
                Assert.IsFalse(fixtureRun.Step.IsTestCase);
                AssertLogContains(fixtureRun, "[Constructor] Outcome: passed\n[FixtureInitializer] Outcome: passed\n[Dispose] Outcome: passed");
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
        public void WhenConstructorFails_FixtureFailsAndBecomesATestCase()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingConstructor)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);
                Assert.IsTrue(fixtureRun.Step.IsTestCase);
                AssertLogContains(fixtureRun, "[Constructor] Outcome: passed");
            });
        }

        [Test]
        public void WhenConstructorFails_TestDoesNotRun()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingConstructor)));
            Assert.AreEqual(0, fixtureRun.Children.Count);
        }

        [Test]
        public void WhenFixtureInitializerFails_FixtureFailsAndBecomesATestCase()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingFixtureInitializer)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);
                Assert.IsTrue(fixtureRun.Step.IsTestCase);
                AssertLogContains(fixtureRun, "[Constructor] Outcome: passed\n[FixtureInitializer] Outcome: passed\n[Dispose] Outcome: failed");
            });
        }

        [Test]
        public void WhenFixtureInitializerFails_TestDoesNotRun()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingFixtureInitializer)));
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
                AssertLogContains(fixtureRun, "[Constructor] Outcome: passed\n[FixtureInitializer] Outcome: passed\n[Dispose] Outcome: failed");
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
        public void WhenDisposeFails_FixtureFails()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingDispose)));
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Failed, fixtureRun.Result.Outcome);
                Assert.IsFalse(fixtureRun.Step.IsTestCase);
                AssertLogContains(fixtureRun, "[Constructor] Outcome: passed\n[FixtureInitializer] Outcome: passed\n[Dispose] Outcome: passed");
            });
        }

        [Test]
        public void WhenDisposeFails_TestStillPassesBecauseDisposeHappensLater()
        {
            var fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(FixtureWithFailingDispose)));
            var testRun = fixtureRun.Children[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestOutcome.Passed, testRun.Result.Outcome);
                Assert.IsTrue(testRun.Step.IsTestCase);
                AssertLogContains(testRun, "[Test] Outcome: passed");
            });
        }

        [Explicit("Sample")]
        public class Fixture : IDisposable
        {
            public Fixture()
            {
                TestLog.WriteLine("[Constructor] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }

            [FixtureInitializer]
            public virtual void FixtureInitializer()
            {
                TestLog.WriteLine("[FixtureInitializer] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }

            [Test]
            public virtual void Test()
            {
                TestLog.WriteLine("[Test] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }

            public virtual void Dispose()
            {
                TestLog.WriteLine("[Dispose] Outcome: {0}", TestContext.CurrentContext.Outcome);
            }
        }

        [Explicit("Sample")]
        public class FixtureWithFailingConstructor : Fixture
        {
            public FixtureWithFailingConstructor()
            {
                Assert.Fail("Boom");
            }
        }

        [Explicit("Sample")]
        public class FixtureWithFailingFixtureInitializer : Fixture
        {
            public override void FixtureInitializer()
            {
                base.FixtureInitializer();
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
        public class FixtureWithFailingDispose : Fixture
        {
            public override void Dispose()
            {
                base.Dispose();
                Assert.Fail("Boom");
            }
        }
    }
}
