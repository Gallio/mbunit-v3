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
using System.Threading;
using Gallio.Common;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(ParallelizableAttribute))]
    [RunSample(typeof(Tests))]
    [Explicit("Tests are timing sensitive and fail periodically on the build server.")]
    public class ParallelizableTest : BaseTestWithSampleRunner
    {
        private int originalDegreeOfParalleism;

        [FixtureSetUp]
        public void IncreaseDegreeOfParallelism()
        {
            originalDegreeOfParalleism = TestAssemblyExecutionParameters.DegreeOfParallelism;

            TestAssemblyExecutionParameters.DegreeOfParallelism = 16;
        }

        [FixtureTearDown]
        public void ResetDegreeOfParallelism()
        {
            TestAssemblyExecutionParameters.DegreeOfParallelism = originalDegreeOfParalleism;
        }

        [Test]
        public void ParallelizedTests()
        {
            Assert.IsTrue(
                WasParallel("One") || WasParallel("Two") || WasParallel("Three") || WasParallel("Four"),
                "Expected at least one of the set of parallelizable tests to run in parallel with another one.");

            Assert.IsTrue(
                WasParallel("Seven") || WasParallel("Eight") || WasParallel("Nine") || WasParallel("Ten"),
                "Expected at least one of the set of parallelizable tests to run in parallel with another one.");

            Assert.IsFalse(
                WasParallel("Five") || WasParallel("Six") || WasParallel("Eleven"),
                "Expected none of the non-parallelizable tests to run in parallel with any other ones.");

            Assert.IsFalse(
                WasParallel("Twelve"),
                "Expected the parallelizable but standalone to run on its own since there are no other tests of the same order.");
        }

        [Test]
        public void ExplicitOrderingShouldHaveBeenPreserved()
        {
            Pair<DateTime, DateTime> batch1 = GetEarliestAndLatestStartAndEndTimes("One", "Two", "Three", "Four");
            Pair<DateTime, DateTime> batch2 = GetEarliestAndLatestStartAndEndTimes("Five");
            Pair<DateTime, DateTime> batch3 = GetEarliestAndLatestStartAndEndTimes("Six");
            Pair<DateTime, DateTime> batch4 = GetEarliestAndLatestStartAndEndTimes("Seven", "Eight", "Nine", "Ten");
            Pair<DateTime, DateTime> batch5 = GetEarliestAndLatestStartAndEndTimes("Eleven");
            Pair<DateTime, DateTime> batch6 = GetEarliestAndLatestStartAndEndTimes("Twelve");

            Assert.LessThanOrEqualTo(batch1.Second, batch2.First);
            Assert.LessThanOrEqualTo(batch2.Second, batch3.First);
            Assert.LessThanOrEqualTo(batch3.Second, batch4.First);
            Assert.LessThanOrEqualTo(batch4.Second, batch5.First);
            Assert.LessThanOrEqualTo(batch5.Second, batch6.First);
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

        private bool WasParallel(string testName)
        {
            return GetTestStepRun(testName).TestLog.ToString().Contains("Detected a parallel");
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
                int newCount = Interlocked.Increment(ref counter);
                TestLog.WriteLine(newCount);
                if (newCount != 1)
                    TestLog.WriteLine("Detected a parallel test during SetUp.");
            }

            [TearDown]
            public void DecrementAndWriteCounterOnExit()
            {
                Thread.Sleep(500);

                int newCount = Interlocked.Decrement(ref counter);
                TestLog.WriteLine(newCount);
                if (newCount != 0)
                    TestLog.WriteLine("Detected a parallel test during TearDown.");
            }

            [Test, Parallelizable]
            public void One()
            {
                TestLog.WriteLine("One");
            }

            [Test, Parallelizable]
            public void Two()
            {
                TestLog.WriteLine("Two");
            }

            [Test, Parallelizable]
            public void Three()
            {
                TestLog.WriteLine("Three");
            }

            [Test, Parallelizable]
            public void Four()
            {
                TestLog.WriteLine("Four");
            }

            [Test]
            public void Five()
            {
                TestLog.WriteLine("Five");
            }

            [Test, DependsOn("Five")]
            public void Six()
            {
                TestLog.WriteLine("Six");
            }

            [Test(Order=1), Parallelizable]
            public void Seven()
            {
                TestLog.WriteLine("Seven");
            }

            [Test(Order=1), Parallelizable]
            public void Eight()
            {
                TestLog.WriteLine("Eight");
            }

            [Test(Order = 1), Parallelizable]
            public void Nine()
            {
                TestLog.WriteLine("Nine");
            }

            [Test(Order = 1), Parallelizable]
            public void Ten()
            {
                TestLog.WriteLine("Ten");
            }

            [Test(Order = 1)]
            public void Eleven()
            {
                TestLog.WriteLine("Eleven");
            }

            [Test(Order = 2), Parallelizable]
            public void Twelve()
            {
                TestLog.WriteLine("Twelve");
            }
        }

        [Explicit("Sample")]
        public class NestedParallelizableTests
        {
            [Test]
            public void Test()
            {
            }

            [Parallelizable(TestScope.All)]
            public class Fixture1
            {
                [Test]
                public void Test1()
                {
                }

                [Test]
                public void Test2()
                {
                }

                [Test]
                [Column(1, 2, 3, 4, 5)]
                public void DataDrivenTest(int x)
                {
                }
            }

            [Parallelizable(TestScope.All)]
            public class Fixture2
            {
                [Test]
                public void Test1()
                {
                }

                [Test]
                public void Test2()
                {
                }

                [Test]
                [Column(1, 2, 3, 4, 5)]
                public void DataDrivenTest(int x)
                {
                }
            }
        }
    }
}
