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
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(AssertExceptionAttribute))]
    [TestsOn(typeof(CatchExceptionAttribute))]
    [TestsOn(typeof(ExpectedExceptionAttribute))]
    [Author("Justin Webster")]
    [RunSample(typeof(AssertExceptionSample))]
    public class AssertExceptionAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("ExpectPassWhenThrowArgNullExAndArgNullExIsAsserted", 
            TestStatus.Passed, null, null)]
        [Row("ExpectFailedWhenExceptionNotThrownAndArgNullExIsAsserted", 
            TestStatus.Failed, null, "Expected an exception of type 'System.ArgumentNullException' but none was thrown.")]
        [Row("ExpectPassWhenThrowArgNullExAndArgExIsAsserted", 
            TestStatus.Passed, null, null)]
        [Row("ExpectFailWhenThrowInvalidOpExAndArgNullExIsAsserted", 
            TestStatus.Failed, null, "Expected an exception of type 'System.ArgumentNullException' but a different exception was thrown.")]
        [Row("ExpectPassWhenThrowMatchingArgNullExAndArgNullExWithMessageSubstringIsAsserted", 
            TestStatus.Passed, null, null)]
        [Row("ExpectFailWhenThrowInvOpExWithUnmatchedMessageAndInvOpExWithMessageSubstringIsAsserted", 
            TestStatus.Failed, null, "Expected an exception of type 'System.InvalidOperationException' with message substring 'expectedmessage' but a different exception was thrown.")]

        [Row("ExpectPassWhenThrowArgNullExAndArgExIsAssertedAndArgNullExIsSkipped", 
            TestStatus.Passed, null, null)]
        [Row("ExpectFailWhenThrowArgExAndArgNullExIsAssertedAndArgExIsSkipped", 
            TestStatus.Failed, null, "Expected an exception of type 'System.ArgumentNullException' but a different exception was thrown.")]
        [Row("ExpectPassFromExpExcOrder5WhenThrowInvOpExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsAsserted", 
            TestStatus.Passed, null, null)]
        [Row("ExpectPassFromOrder5WhenThrowInvOpExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsAsserted", 
            TestStatus.Passed, null, null)]
        [Row("ExpectFailWhenThrowOutOfMemExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsAsserted", 
            TestStatus.Failed, null, "Expected an exception of type 'System.InvalidOperationException' but a different exception was thrown.")]
        [Row("ExpectSkippedWhenThrowArgNullExAndArgNullExIsSkippedAndArgExIsAsserted", 
            TestStatus.Skipped, null, null)]
        [Row("ExpectSkippedWhenThrowArgExAndArgExIsSkippedAndArgNullExIsAsserted", 
            TestStatus.Skipped, null, null)]
        [Row("ExpectInconclusiveFromOrder2WhenThrowArgExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsAsserted", 
            TestStatus.Inconclusive, null, null)]

        public void AssertExceptionOutcome(string testMethodName, 
            TestStatus expectedOutcomeStatus, string expectedOutcomeCategory, 
            string expectedLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(AssertExceptionSample).GetMethod(testMethodName)));

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(run);
                Assert.AreEqual(expectedOutcomeStatus, run.Result.Outcome.Status);
                Assert.AreEqual(expectedOutcomeCategory, run.Result.Outcome.Category);

                if (expectedLogOutput != null)
                    AssertLogContains(run, expectedLogOutput, MarkupStreamNames.Failures);
            });
        }

        [Explicit("Sample")]
        public class AssertExceptionSample
        {

            #region AssertException Basic Tests

            /// <summary>
            /// Verifies simplest happy path scenario.
            /// </summary>
            [Test]
            [AssertException(typeof(ArgumentNullException))]
            public void ExpectPassWhenThrowArgNullExAndArgNullExIsAsserted()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies sad path scenario where no exception is thrown by the test.
            /// </summary>
            [Test]
            [AssertException(typeof(ArgumentNullException))]
            public void ExpectFailedWhenExceptionNotThrownAndArgNullExIsAsserted()
            {
            }

            /// <summary>
            /// Verifies sad path scenario where an exception of the wrong type is thrown by the test.
            /// </summary>
            [Test]
            [AssertException(typeof(ArgumentNullException))]
            public void ExpectFailWhenThrowInvalidOpExAndArgNullExIsAsserted()
            {
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Verifies that AssertException can handle an Exception that inherits from the AssertExceptionAttribute.ExceptionType.
            /// </summary>
            [Test]
            [AssertException(typeof(ArgumentException))]
            public void ExpectPassWhenThrowArgNullExAndArgExIsAsserted()
            {
                throw new ArgumentNullException();
            }

            [Test]
            [AssertException(typeof(ArgumentNullException), ExceptionMessage = "expectedmessage")]
            public void ExpectPassWhenThrowMatchingArgNullExAndArgNullExWithMessageSubstringIsAsserted()
            {
                throw new ArgumentNullException("paramname", "the expectedmessage is part of the message");
            }

            [Test]
            [AssertException(typeof(InvalidOperationException), ExceptionMessage = "expectedmessage")]
            public void ExpectFailWhenThrowInvOpExWithUnmatchedMessageAndInvOpExWithMessageSubstringIsAsserted()
            {
                throw new InvalidOperationException("a different message than expected");
            }

            #endregion

            #region CatchException+AssertException Combo Tests

            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 2)]
            [AssertException(typeof(ArgumentException), Order = 1)]
            public void ExpectPassWhenThrowArgNullExAndArgExIsAssertedAndArgNullExIsSkipped()
            {
                throw new ArgumentNullException();
            }

            [Test]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Skipped", Order = 2)]
            [AssertException(typeof(ArgumentNullException), Order = 1)]
            public void ExpectFailWhenThrowArgExAndArgNullExIsAssertedAndArgExIsSkipped()
            {
                throw new ArgumentException();
            }

            /// <summary>
            /// Verifies that multiple CatchExceptions can transparently detect and rethrow an Exception before an AssertException gets it 
            /// (without changing the behavior of the AssertException).
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending", Order = 3)]
            [CatchException(typeof(NotSupportedException), StandardOutcome = "TestOutcome.Ignored", Order = 4)]
            [AssertException(typeof(InvalidOperationException), Order = 5)]
            public void ExpectPassFromExpExcOrder5WhenThrowInvOpExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsAsserted()
            {
                throw new InvalidOperationException();
            }

            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending", Order = 3)]
            [CatchException(typeof(NotSupportedException), StandardOutcome = "TestOutcome.Ignored", Order = 4)]
            [CatchException(typeof(InvalidOperationException), StandardOutcome = "TestOutcome.Passed", Order = 5)]
            public void ExpectPassFromOrder5WhenThrowInvOpExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsAsserted()
            {
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Verifies that multiple CatchExceptions can transparently detect and rethrow an Exception before an AssertException gets it 
            /// (without changing the behavior of the AssertException).
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending", Order = 3)]
            [CatchException(typeof(NotSupportedException), StandardOutcome = "TestOutcome.Ignored", Order = 4)]
            [AssertException(typeof(InvalidOperationException), Order = 5)]
            public void ExpectFailWhenThrowOutOfMemExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsAsserted()
            {
                throw new OutOfMemoryException();
            }

            /// <summary>
            /// Verifies that a CatchException can detect an Exception before an AssertException sees it 
            /// (effectively overriding the behavior of the AssertException).
            /// </summary>
            //[Disable("Disabled because the AssertExceptionAttribute seems to execute earlier than indicated by it's Order, and causes a Failed outcome before the CatchExceptionAttribute handles the thrown Exception.")]
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [AssertException(typeof(ArgumentException), Order = 2)]
            public void ExpectSkippedWhenThrowArgNullExAndArgNullExIsSkippedAndArgExIsAsserted()
            {
                throw new ArgumentNullException();
            }

            //[Disable("Disabled because the AssertExceptionAttribute seems to execute earlier than indicated by it's Order, and causes a Failed outcome before the CatchExceptionAttribute handles the thrown Exception.")]
            [Test]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [AssertException(typeof(ArgumentNullException), Order = 2)]
            public void ExpectSkippedWhenThrowArgExAndArgExIsSkippedAndArgNullExIsAsserted()
            {
                throw new ArgumentException();
            }

            /// <summary>
            /// Verifies that a CatchException can detect an Exception before an AssertException sees it 
            /// (effectively overriding the behavior of the AssertException),
            /// even when there are other CatchExceptions that detect and rethrow the Exception 
            /// (before and after the handling CatchException, and before the AssertException).
            /// </summary>
            //[Disable("Disabled because the AssertExceptionAttribute seems to execute earlier than indicated by it's Order, and causes a Failed outcome before the CatchExceptionAttribute handles the thrown Exception.")]
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending", Order = 3)]
            [CatchException(typeof(NotSupportedException), StandardOutcome = "TestOutcome.Ignored", Order = 4)]
            [AssertException(typeof(InvalidOperationException), Order = 5)]
            public void ExpectInconclusiveFromOrder2WhenThrowArgExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsAsserted()
            {
                throw new ArgumentException();
            }

            #endregion

            #region Example Tests

            /// <summary>
            /// This test will have an outcome of <see cref="TestOutcome.Passed"/>.
            /// </summary>
            [Test]
            [AssertException(typeof(NotSupportedException))]
            public void AssertExceptionExample1()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// This test will have an outcome of <see cref="TestOutcome.Failed"/> 
            /// whether the test throws a <see cref="NotImplementedException"/> or throws no exception.
            /// </summary>
            [Test]
            [AssertException(typeof(NotSupportedException))]
            public void AssertExceptionExample2()
            {
                bool randomBool = new Random().Next() % 2 == 0;
                if (randomBool)
                    throw new NotImplementedException();
            }

            /// <summary>
            /// This test will have an outcome of <see cref="TestOutcome.Pending"/>.
            /// </summary>
            /// <remarks>
            /// This test can have any of several outcomes depending on what type of exception is thrown (if any) during test execution.
            /// </remarks>
            [Test]
            [CatchException(typeof(NotSupportedException), OutcomeStatus = TestStatus.Skipped, OutcomeCategory = "notsupported")]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending")]
            [CatchException(typeof(TimeoutException), ExceptionMessage = "deadlock", OutcomeStatus = TestStatus.Inconclusive, OutcomeCategory = "deadlock", Order = 2)]
            [CatchException(typeof(TimeoutException), ExceptionMessage = "server not responding", OutcomeStatus = TestStatus.Inconclusive, OutcomeCategory = "timeout", Order = 3)]
            [AssertException(typeof(ArithmeticException), Order = 4)]
            public void AssertExceptionExample3()
            {
                //throw new NotSupportedException(); //outcome would be: new TestOutcome(TestStatus.Skipped, "notsupported")
                throw new NotImplementedException(); //outcome would be: TestOutcome.Pending
                //throw new TimeoutException("A deadlock occurred."); //outcome would be: new TestOutcome(TestStatus.Inconclusive, "deadlock")
                //throw new TimeoutException("The server is not responding."); //outcome would be: new TestOutcome(TestStatus.Inconclusive, "timeout")
                //throw new TimeoutException("Transaction timeout."); //outcome would be: TestOutcome.Failed
                //throw new TimeoutException(); //outcome would be: TestOutcome.Failed
                //throw new ArithmeticException(); //outcome would be: TestOutcome.Passed
                //return; //outcome would be: TestOutcome.Failed
            }

            #endregion

        }
    }
}
