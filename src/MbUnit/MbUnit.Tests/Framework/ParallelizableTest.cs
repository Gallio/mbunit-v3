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
using System.Linq;
using System.Text;
using System.Threading;
using Gallio;
using Gallio.Framework;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(ParallelizableAttribute))]
    [RunSample(typeof(Tests))]
    public class ParallelizableTest : BaseTestWithSampleRunner
    {
        [Test]
        public void NonParallizableTestsShouldRunIndependently()
        {
            AssertLogContains(GetTestStepRun("Five"), "1\nFive\n0");
            AssertLogContains(GetTestStepRun("Six"), "1\nSix\n0");
            AssertLogContains(GetTestStepRun("Nine"), "1\nNine\n0");
        }

        [Test]
        public void ParallelizableTestsWithNoSiblingsShouldRunIndependently()
        {
            AssertLogContains(GetTestStepRun("Ten"), "1\nTen\n0");
        }

        [Test]
        public void ExplicitOrderingShouldHaveBeenPreserved()
        {
            Pair<DateTime, DateTime> batch1 = GetEarliestAndLatestStartAndEndTimes("One", "Two", "Three", "Four");
            Pair<DateTime, DateTime> batch2 = GetEarliestAndLatestStartAndEndTimes("Five");
            Pair<DateTime, DateTime> batch3 = GetEarliestAndLatestStartAndEndTimes("Six");
            Pair<DateTime, DateTime> batch4 = GetEarliestAndLatestStartAndEndTimes("Seven", "Eight");
            Pair<DateTime, DateTime> batch5 = GetEarliestAndLatestStartAndEndTimes("Nine");
            Pair<DateTime, DateTime> batch6 = GetEarliestAndLatestStartAndEndTimes("Ten");

            Assert.LessThanOrEqualTo(batch1.Second, batch2.First);
            Assert.LessThanOrEqualTo(batch2.Second, batch3.First);
            Assert.LessThanOrEqualTo(batch3.Second, batch4.First);
            Assert.LessThanOrEqualTo(batch4.Second, batch5.First);
            Assert.LessThanOrEqualTo(batch5.Second, batch6.First);
        }

        [Test]
        public void AtLeastOnePairOfParallelizableTestsShouldHaveRunInParallel()
        {
            string[] names = new[] { "One", "Two", "Three", "Four" };

            foreach (string outerName in names)
            {
                TestStepRun outerRun = GetTestStepRun(outerName);

                foreach (string innerName in names)
                {
                    TestStepRun innerRun = GetTestStepRun(innerName);

                    if (innerRun.StartTime > outerRun.StartTime && innerRun.StartTime < outerRun.EndTime)
                        return; // found an example of a test that ran in parallel
                }
            }

            Assert.Fail("Expected at least one pair of parallelizable tests to run concurrently during the same period of time but there were no such examples were found.");
        }

        private Pair<DateTime, DateTime> GetEarliestAndLatestStartAndEndTimes(params string[] testNames)
        {
            DateTime startTime = DateTime.MaxValue;
            DateTime endTime = DateTime.MinValue;

            foreach (string testName in testNames)
            {
                TestStepRun run = GetTestStepRun(testName);
                if (run.StartTime < startTime)
                    startTime = run.StartTime;
                if (run.EndTime > endTime)
                    endTime = run.EndTime;
            }

            return new Pair<DateTime, DateTime>(startTime, endTime);
        }

        private TestStepRun GetTestStepRun(string testName)
        {
            IList<TestStepRun> runs = Runner.GetTestCaseRunsWithin(
                CodeReference.CreateFromMember(typeof(Tests).GetMethod(testName)));
            Assert.AreEqual(1, runs.Count, "Different number of runs than expected.");
            return runs[0];
        }

        [Explicit("Sample")]
        public class Tests
        {
            private int counter;

            [SetUp]
            public void IncrementAndWriteCounterOnEntry()
            {
                TestLog.WriteLine(Interlocked.Increment(ref counter));
            }

            [TearDown]
            public void DecrementAndWriteCounterOnExit()
            {
                TestLog.WriteLine(Interlocked.Decrement(ref counter));
            }

            [Test, Parallelizable]
            public void One()
            {
                TestLog.WriteLine("One");
                Thread.Sleep(100);
            }

            [Test, Parallelizable]
            public void Two()
            {
                TestLog.WriteLine("Two");
                Thread.Sleep(100);
            }

            [Test, Parallelizable]
            public void Three()
            {
                TestLog.WriteLine("Three");
                Thread.Sleep(100);
            }

            [Test, Parallelizable]
            public void Four()
            {
                TestLog.WriteLine("Four");
                Thread.Sleep(100);
            }

            [Test]
            public void Five()
            {
                TestLog.WriteLine("Five");
                Thread.Sleep(100);
            }

            [Test, DependsOn("Five")]
            public void Six()
            {
                TestLog.WriteLine("Six");
                Thread.Sleep(100);
            }

            [Test(Order=1), Parallelizable]
            public void Seven()
            {
                TestLog.WriteLine("Seven");
                Thread.Sleep(100);
            }

            [Test(Order=1), Parallelizable]
            public void Eight()
            {
                TestLog.WriteLine("Eight");
                Thread.Sleep(100);
            }

            [Test(Order = 1)]
            public void Nine()
            {
                TestLog.WriteLine("Nine");
                Thread.Sleep(100);
            }

            [Test(Order = 2), Parallelizable]
            public void Ten()
            {
                TestLog.WriteLine("Ten");
                Thread.Sleep(100);
            }
        }
    }
}
