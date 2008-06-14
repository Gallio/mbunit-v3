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
using System.Threading;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(ThreadedRepeatAttribute))]
    public class ThreadedRepeatAdditionalUseCasesTest
    {
        private int entranceCount;
        private readonly Random _random = new Random();
        private const int _slowRunningMaxTime = 2000;   // in milliseconds

        [SetUp]
        public void Setup()
        {
            entranceCount = 0;
        }

        [TearDown]
        public void TearDown()
        {
            Assert.AreEqual(10, entranceCount, "The tests did not run in the expected number of threads.");
        }

        /// <summary>
        /// This test ensures that tests that fail immediately work
        /// </summary>
        [Test]
        [ThreadedRepeat(10)]
        [ExpectedException(typeof(AssertionException), "This test failed.")]
        public void FastRunningThreadWithExpectedExceptionTest()
        {
            Interlocked.Increment(ref entranceCount);

            Assert.Fail("This test failed.");
        }

        /// <summary>
        /// This test ensures that tests that take a little while to fail work
        /// </summary>
        [Test]
        [ThreadedRepeat(10)]
        [ExpectedException(typeof(AssertionException), "This test failed.")]
        public void SlowRunningThreadWithExpectedExceptionTest()
        {
            Interlocked.Increment(ref entranceCount);

            Thread.Sleep(_random.Next(_slowRunningMaxTime));
            Assert.Fail("This test failed.");
        }

        /// <summary>
        /// This test ensures that tests that pass immediately work
        /// </summary>
        [Test]
        [ThreadedRepeat(10)]
        public void FastRunningThreadTest()
        {
            Interlocked.Increment(ref entranceCount);
        }

        /// <summary>
        /// This test ensures that tests that take a little while to pass work
        /// </summary>
        [Test]
        [ThreadedRepeat(10)]
        public void SlowRunningThreadTest()
        {
            Interlocked.Increment(ref entranceCount);

            Thread.Sleep(_random.Next(_slowRunningMaxTime));
        }

        /// <summary>
        /// This test checks that ThreadedRepeat works with RowTest
        /// </summary>
        [Test]
        [Row(5, 6, 11, Description = "Valid Case")]
        [Row(2, 2, 5, Description = "Fails, Numbers do not add up", ExpectedException = typeof(AssertionException))]
        [ThreadedRepeat(10)]
        public void ThreadedRepeatRowTest(int x, int y, int expected)
        {
            Interlocked.Increment(ref entranceCount);

            Assert.AreEqual(expected, x + y);
        }

        /// <summary>
        /// This test checks that ThreadedRepeat works with RowTest and ExpectedException
        /// </summary>
        [Test]
        [Row(5, 6, 10)]
        [Row(2, 2, 5)]
        [ThreadedRepeat(10)]
        [ExpectedException(typeof(AssertionException))]
        public void ThreadedRepeatRowWithExpectedExceptionTest(int x, int y, int expected)
        {
            Interlocked.Increment(ref entranceCount);

            Assert.AreEqual(expected, x + y);
        }
    }
}
