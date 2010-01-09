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
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest : BaseAssertTest
    {
        [Test]
        public void Fail_without_parameters()
        {
            AssertionFailure[] failures = Capture(Assert.Fail);
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("An assertion failed.", failures[0].Description);
            Assert.IsNull(failures[0].Message);
        }

        [Test]
        public void Fail_with_message_and_arguments()
        {
            AssertionFailure[] failures = Capture(() => Assert.Fail("{0} {1}.", "MbUnit", "message"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("An assertion failed.", failures[0].Description);
            Assert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void Inconclusive()
        {
            var ex = Assert.Throws<TestInconclusiveException>(() => Assert.Inconclusive());
            Assert.AreEqual("The test was inconclusive.", ex.Message);
        }

        [Test]
        public void Inconclusive_with_message()
        {
            var ex = Assert.Throws<TestInconclusiveException>(() => Assert.Inconclusive("{0} {1}.", "MbUnit", "message"));
            Assert.AreEqual("MbUnit message.", ex.Message);
        }

        [Test]
        public void Terminate()
        {
            var ex = Assert.Throws<TestTerminatedException>(() => Assert.Terminate(TestOutcome.Explicit));
            Assert.AreEqual("The test was terminated.", ex.Message);
            Assert.AreEqual(TestOutcome.Explicit, ex.Outcome);
        }

        [Test]
        public void Terminate_with_message()
        {
            var ex = Assert.Throws<TestTerminatedException>(() => Assert.Terminate(TestOutcome.Explicit, "{0} {1}.", "MbUnit", "message"));
            Assert.AreEqual("MbUnit message.", ex.Message);
            Assert.AreEqual(TestOutcome.Explicit, ex.Outcome);
        }

        [Test]
        public void TerminateSilently()
        {
            var ex = Assert.Throws<SilentTestException>(() => Assert.TerminateSilently(TestOutcome.Error));
            Assert.IsFalse(ex.HasNonDefaultMessage);
            Assert.AreEqual(TestOutcome.Error, ex.Outcome);
        }

        [Test]
        public void TerminateSilently_with_message()
        {
            var ex = Assert.Throws<SilentTestException>(() => Assert.TerminateSilently(TestOutcome.Error, "{0} {1}.", "MbUnit", "message"));
            Assert.IsTrue(ex.HasNonDefaultMessage);
            Assert.AreEqual("MbUnit message.", ex.Message);
            Assert.AreEqual(TestOutcome.Error, ex.Outcome);
        }

        [Test]
        public void Multiple_passes_if_no_failures_within_block()
        {
            bool executed = false;
            AssertionFailure[] failures = Capture(() => Assert.Multiple(() =>
            {
                executed = true;
            }));

            Assert.IsTrue(executed);
            Assert.IsEmpty(failures);
        }

        [Test]
        public void Multiple_reports_summary_of_a_single_failure()
        {
            bool executed = false;
            AssertionFailure[] failures = Capture(() => Assert.Multiple(() =>
            {
                Assert.IsFalse(true, "Failed assert");
                executed = true;
            }));

            Assert.IsTrue(executed);
            Assert.AreEqual("There was 1 failure within the multiple assertion block.", failures[0].Description);
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual(null, failures[0].Message);

            Assert.AreEqual(0, failures[0].InnerFailures.Count, "Should not contain inner failures because they are logged immediately.");
        }

        [Test]
        public void Multiple_reports_summary_of_multiple_failures()
        {
            bool executed = false;
            AssertionFailure[] failures = Capture(() => Assert.Multiple(() =>
            {
                Assert.IsFalse(true, "Failed assert #1");
                Assert.IsFalse(false, "Passed assert #1");
                Assert.IsFalse(true, "Failed assert #2");
                executed = true;
            }));

            Assert.IsTrue(executed);
            Assert.AreEqual("There were 2 failures within the multiple assertion block.", failures[0].Description);
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual(null, failures[0].Message);

            Assert.AreEqual(0, failures[0].InnerFailures.Count, "Should not contain inner failures because they are logged immediately.");
        }

        [Test]
        public void Multiple_reports_summary_of_multiple_failures_with_custom_message()
        {
            bool executed = false;
            AssertionFailure[] failures = Capture(() => Assert.Multiple(() =>
            {
                Assert.IsFalse(true, "Failed assert #1");
                Assert.IsFalse(false, "Passed assert #1");
                Assert.IsFalse(true, "Failed assert #2");
                executed = true;
            }, "{0} {1}", "MbUnit", "message"));

            Assert.IsTrue(executed);
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MbUnit message", failures[0].Message);

            Assert.AreEqual(0, failures[0].InnerFailures.Count, "Should not contain inner failures because they are logged immediately.");
        }

        [Test]
        public void Multiple_captures_and_reports_non_silent_AssertionFailureException()
        {
            AssertionFailure[] failures = Capture(() => Assert.Multiple(() =>
            {
                throw new AssertionFailureException(new AssertionFailureBuilder("Boom").ToAssertionFailure(), false);
            }, "{0} {1}", "MbUnit", "message"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MbUnit message", failures[0].Message);

            Assert.AreEqual(0, failures[0].InnerFailures.Count, "Should contain inner failures because it was not logged.");
        }

        [Test]
        public void Multiple_rethrows_test_exceptions_other_than_AssertionFailureException()
        {
            Assert.Throws<TestInconclusiveException>(() =>
            {
                Assert.Multiple(() => { throw new TestInconclusiveException(); });
            });
        }

        [Test]
        public void IsFailurePending_false_when_no_pending_failures()
        {
            bool wasPending = false;
            bool executed = false;
            AssertionFailure[] failures = Capture(() => Assert.Multiple(() =>
            {
                wasPending = Assert.IsFailurePending;
                executed = true;
            }));

            Assert.IsTrue(executed);
            Assert.IsFalse(wasPending);
            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void IsFailurePending_true_when_there_is_a_pending_failures()
        {
            bool wasPending = false;
            bool executed = false;
            AssertionFailure[] failures = Capture(() => Assert.Multiple(() =>
            {
                Assert.Fail("Failure");
                wasPending = Assert.IsFailurePending;
                executed = true;
            }));

            Assert.IsTrue(executed);
            Assert.IsTrue(wasPending);
            Assert.AreEqual(1, failures.Length);
        }
    }
}