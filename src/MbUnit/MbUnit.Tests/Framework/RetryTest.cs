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
using Gallio.Framework;
using MbUnit.Framework;
using Gallio.Framework.Assertions;
using System.Threading;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(Retry))]
    public class RetryTest : BaseAssertTest
    {
        private void DoRetry(bool expectedSucceeded, Gallio.Common.Action action)
        {
            AssertionFailure[] failures = Capture(action);

            if (expectedSucceeded)
            {
                Assert.IsEmpty(failures);
            }
            else
            {
                Assert.AreEqual(1, failures.Length);
                Assert.StartsWith(failures[0].Description, "The 'Retry.Until' operation has failed");
            }
        }

        [Test]
        [Row(true, 3, 0, Description = "Succeeds immediately")]
        [Row(true, 3, 2, Description = "Succeeds after some times")]
        [Row(false, 3, -1, Description = "Fails because conditions is never true.")]
        public void Retry_Repeat_Until_Condition(bool expectedSucceeded, int repeat, int afterCount)
        {
            int count = 0;
            DoRetry(expectedSucceeded, () => Retry.Repeat(repeat).Until(() => afterCount >= 0 && count++ >= afterCount));
        }

        [Test]
        public void Retry_Repeat_Until_Condition_with_Action()
        {
            int count = 0;
            DoRetry(true, () => Retry.Repeat(10).DoBetween(() => count++).Until(() => count >= 8));
        }

        [Test]
        [Row(true, Description = "Succeeds immediately")]
        [Row(false, Description = "Fails because wait handler is never signaled.")]
        public void Retry_Times_Until_WaitHandle(bool signaled)
        {
            var handle = new ManualResetEvent(signaled);
            DoRetry(signaled, () => Retry.Repeat(5).Until(handle));
        }

        [Test]
        [Row(true, 1000, Description = "Succeeds before timeout occured")]
        [Row(false, 250, Description = "Fails because due to timeout error.")]
        [Row(false, 0, Description = "Fails because due to timeout error.")]
        public void Retry_WithTimeout_Until_Condition(bool expectedSucceeded, int timeout)
        {
            var t = DateTime.UtcNow.AddMilliseconds(500);
            DoRetry(expectedSucceeded, () => Retry.WithTimeout(timeout).Until(() => DateTime.UtcNow >= t));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Retry_with_null_DoBetween_action_should_throw_exception()
        {
            Retry.DoBetween(null);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Retry_with_negative_polling_time_should_throw_exception()
        {
            Retry.WithPolling(-100);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Retry_Repeat_twice_should_throw_exception()
        {
            Retry.Repeat(10).Repeat(10);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Retry_WithPolling_twice_should_throw_exception()
        {
            Retry.WithPolling(10).WithPolling(10);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Retry_WithTimeout_twice_should_throw_exception()
        {
            Retry.WithTimeout(10).WithTimeout(10);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Retry_DoBetween_twice_should_throw_exception()
        {
            Retry.DoBetween(() => { }).DoBetween(() => { });
        }
    }
}
