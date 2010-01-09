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
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using MbUnit.TestResources.Reflection;

namespace Gallio.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(TestContext))]
    [RunSample(typeof(AutoExecuteSamples))]
    public class TestContextTest : BaseTestWithSampleRunner
    {
        [Test]
        public void CurrentTestHasCorrectTestName()
        {
            Assert.AreEqual("CurrentTestHasCorrectTestName", TestContext.CurrentContext.Test.Name);
        }

        [Test]
        public void CanStoreDataInTestContext()
        {
            Key<string> key = new Key<string>("key");
            TestContext.CurrentContext.Data.SetValue(key, "value");
            Assert.AreEqual("value", TestContext.CurrentContext.Data.GetValue(key));
        }

        [Test]
        public void AutoExecute_WhenActionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => TestContext.CurrentContext.AutoExecute(TriggerEvent.TestFailed, null));
            Assert.Throws<ArgumentNullException>(() => TestContext.CurrentContext.AutoExecute(TriggerEvent.TestFailed, null, () => { }));
        }

        [Test]
        [Row("OnTestFinished_Triggered", true)]
        [Row("OnTestPassed_Triggered", true)]
        [Row("OnTestPassed_NotTriggered", false)]
        [Row("OnTestPassedOrInconclusive_TriggeredOnPassed", true)]
        [Row("OnTestPassedOrInconclusive_TriggeredOnInconclusive", true)]
        [Row("OnTestPassedOrInconclusive_NotTriggered", false)]
        [Row("OnTestInconclusive_Triggered", true)]
        [Row("OnTestInconclusive_NotTriggered", false)]
        [Row("OnTestFailed_Triggered", true)]
        [Row("OnTestFailed_NotTriggered", false)]
        [Row("OnTestFailedOrInconclusive_TriggeredOnFailed", true)]
        [Row("OnTestFailedOrInconclusive_TriggeredOnInconclusive", true)]
        [Row("OnTestFailedOrInconclusive_NotTriggered", false)]
        public void AutoExecute_ExecutesActionWhenTriggeredAndCleanupAlways(string testName, bool triggered)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(AutoExecuteSamples).GetMethod(testName)));

            if (triggered)
            {
                Assert.Contains(run.TestLog.ToString(), "Triggered 1");
                Assert.Contains(run.TestLog.ToString(), "Triggered 2");
            }
            else
            {
                Assert.DoesNotContain(run.TestLog.ToString(), "Triggered 1");
                Assert.DoesNotContain(run.TestLog.ToString(), "Triggered 2");
            }

            Assert.Contains(run.TestLog.ToString(), "Cleanup");
        }

        [Explicit("Sample")]
        public class AutoExecuteSamples
        {
            [Test]
            public void OnTestFinished_Triggered()
            {
                Register(TriggerEvent.TestFinished);
            }

            [Test]
            public void OnTestPassed_Triggered()
            {
                Register(TriggerEvent.TestPassed);
            }

            [Test]
            public void OnTestPassed_NotTriggered()
            {
                Register(TriggerEvent.TestPassed);
                Assert.Fail();
            }

            [Test]
            public void OnTestPassedOrInconclusive_TriggeredOnPassed()
            {
                Register(TriggerEvent.TestPassedOrInconclusive);
            }

            [Test]
            public void OnTestPassedOrInconclusive_TriggeredOnInconclusive()
            {
                Register(TriggerEvent.TestPassedOrInconclusive);
                Assert.Inconclusive();
            }

            [Test]
            public void OnTestPassedOrInconclusive_NotTriggered()
            {
                Register(TriggerEvent.TestPassedOrInconclusive);
                Assert.Fail();
            }

            [Test]
            public void OnTestInconclusive_Triggered()
            {
                Register(TriggerEvent.TestInconclusive);
                Assert.Inconclusive();
            }

            [Test]
            public void OnTestInconclusive_NotTriggered()
            {
                Register(TriggerEvent.TestInconclusive);
            }

            [Test]
            public void OnTestFailed_Triggered()
            {
                Register(TriggerEvent.TestFailed);
                Assert.Fail();
            }

            [Test]
            public void OnTestFailed_NotTriggered()
            {
                Register(TriggerEvent.TestFailed);
            }

            [Test]
            public void OnTestFailedOrInconclusive_TriggeredOnFailed()
            {
                Register(TriggerEvent.TestFailedOrInconclusive);
                Assert.Fail();
            }

            [Test]
            public void OnTestFailedOrInconclusive_TriggeredOnInconclusive()
            {
                Register(TriggerEvent.TestFailedOrInconclusive);
                Assert.Inconclusive();
            }

            [Test]
            public void OnTestFailedOrInconclusive_NotTriggered()
            {
                Register(TriggerEvent.TestFailedOrInconclusive);
            }

            private static void Register(TriggerEvent triggerEvent)
            {
                TestContext.CurrentContext.AutoExecute(triggerEvent,
                    () => TestLog.WriteLine("Triggered 1"));
                TestContext.CurrentContext.AutoExecute(triggerEvent,
                    () => TestLog.WriteLine("Triggered 2"),
                    () => TestLog.WriteLine("Cleanup"));
            }
        }
    }
}