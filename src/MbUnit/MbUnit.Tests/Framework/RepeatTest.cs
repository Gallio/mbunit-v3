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
    [RunSample(typeof(RepeatSample))]
    public class RepeatTest : BaseTestWithSampleRunner
    {
        [Test]
        public void CheckSampleOutput()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(RepeatSample).GetMethod("Test")));

            AssertLogContains(run, "3 of 4 repetitions passed.");
            Assert.AreEqual(TestOutcome.Failed, run.Result.Outcome);

            IList<TestStepRun> steps = run.Children;
            Assert.AreEqual(4, steps.Count, "Expected 4 repetitions represented as steps.");

            Assert.AreEqual("Repetition #1", steps[0].Step.Name);
            AssertLogContains(steps[0], "Run #1");
            Assert.AreEqual(TestOutcome.Passed, steps[0].Result.Outcome);

            Assert.AreEqual("Repetition #2", steps[1].Step.Name);
            AssertLogContains(steps[1], "Run #2");
            AssertLogContains(steps[1], "Boom", TestLogStreamNames.Failures);
            Assert.AreEqual(TestOutcome.Failed, steps[1].Result.Outcome);

            Assert.AreEqual("Repetition #3", steps[2].Step.Name);
            AssertLogContains(steps[2], "Run #3");
            Assert.AreEqual(TestOutcome.Passed, steps[2].Result.Outcome);

            Assert.AreEqual("Repetition #4", steps[3].Step.Name);
            AssertLogContains(steps[3], "Run #4");
            Assert.AreEqual(TestOutcome.Passed, steps[3].Result.Outcome);
        }

        [Explicit("Sample")]
        internal class RepeatSample
        {
            private int index;

            [Test, Repeat(4)]
            public void Test()
            {
                TestLog.WriteLine("Run #{0}", ++index);
                if (index == 2)
                    Assert.Fail("Boom");
            }
        }
    }
}
