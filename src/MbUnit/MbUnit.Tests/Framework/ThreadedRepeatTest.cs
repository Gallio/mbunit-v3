// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Collections;
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
    [TestsOn(typeof(ThreadedRepeatAttribute))]
    [RunSample(typeof(ThreadedRepeatSample))]
    public class ThreadedRepeatTest : BaseTestWithSampleRunner
    {
        [Test]
        public void CheckSampleOutput()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ThreadedRepeatSample).GetMethod("Test")));

            AssertLogContains(run, "9 of 10 threaded repetitions passed.");
            Assert.AreEqual(TestOutcome.Failed, run.Result.Outcome);

            IList<TestStepRun> steps = run.Children;
            Assert.AreEqual(10, steps.Count, "Expected 10 repetitions represented as steps.");

            for (int i = 0; i < 10; i++)
            {
                string name = "Threaded Repetition #" + (i + 1);
                TestStepRun step = GenericUtils.Find(steps, candidate => candidate.Step.Name == name);
                AssertLogContains(step, "Run: " + name);

                if (i == 1)
                {
                    Assert.AreEqual(TestOutcome.Failed, step.Result.Outcome);
                    AssertLogContains(step, "Boom", TestLogStreamNames.Failures);
                }
                else
                {
                    Assert.AreEqual(TestOutcome.Passed, step.Result.Outcome);
                }
            }
        }

        [Explicit("Sample")]
        internal class ThreadedRepeatSample
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
    }
}
