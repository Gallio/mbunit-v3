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

#region Using Directives

using System;
using System.Threading;
using MbUnit.Framework;

#endregion

namespace MbUnit.Tests.Framework
{
    /// <summary>
    /// Test fixture of the <see cref="ThreadedRepeatAttribute" />
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(ThreadedRepeatAttribute))]
    public class ThreadedRepeatAttributeTest
    {
        private volatile int _entranceCount;
        private readonly Random _random = new Random();
        private readonly object _lock = new object();
        private const int _slowRunningMaxTime = 2000;   // in milliseconds

        [SetUp]
        public void Setup()
        {
            _entranceCount = 0;
        }

        [TearDown]
        public void TearDown()
        {
            Assert.AreEqual(10, _entranceCount, "The tests did not run in the expected number of threads.");
        }

        /// <summary>
        /// This test ensures that tests that fail immediately work
        /// </summary>
        [Test]
        [ThreadedRepeat(10)]
        [ExpectedException(typeof(AssertionException), "This test failed.")]
        public void FastRunningThreadWithExpectedExceptionTest()
        {
            lock (_lock)
            {
                _entranceCount++;
            }
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
            lock (_lock)
            {
                _entranceCount++;
            }
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
            lock (_lock)
            {
                _entranceCount++;
            }
        }

        /// <summary>
        /// This test ensures that tests that take a little while to pass work
        /// </summary>
        [Test]
        [ThreadedRepeat(10)]
        public void SlowRunningThreadTest()
        {
            lock (_lock)
            {
                _entranceCount++;
            }
            Thread.Sleep(_random.Next(_slowRunningMaxTime));
        }

        /// <summary>
        /// This test checks that ThreadedRepeat works with RowTest
        /// </summary>
        [Test]
        [Row(5, 6, 11, Description = "Valid Case")]
        [Row(2, 2, 5, Description = "Fails, Numbers do not add up", ExpectedException = typeof(AssertionException))]
        [Row(3, 6, 9, Description = "Valid Case")]
        [ThreadedRepeat(10)]
        public void ThreadedRepeatRowTest(int x, int y, int expected)
        {
            lock (_lock)
            {
                _entranceCount++;
            }
            Assert.AreEqual(expected, x + y);
        }

        /// <summary>
        /// This test checks that ThreadedRepeat works with RowTest and ExpectedException
        /// </summary>
        [Test]
        [Row(5, 6, 10)]
        [Row(2, 2, 5)]
        [Row(3, 6, 56)]
        [ThreadedRepeat(10)]
        [ExpectedException(typeof(AssertionException))]
        public void ThreadedRepeatRowWithExpectedExceptionTest(int x, int y, int expected)
        {
            lock (_lock)
            {
                _entranceCount++;
            }
            Assert.AreEqual(expected, x + y);
        }
    }
}
