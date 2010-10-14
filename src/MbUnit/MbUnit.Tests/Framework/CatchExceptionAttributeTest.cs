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
    [TestsOn(typeof(CatchExceptionAttribute))]
    [TestsOn(typeof(ExpectedExceptionAttribute))]
    [Author("Justin Webster")]
    [RunSample(typeof(CatchExceptionSample))]
    public class CatchExceptionAttributeTest : BaseTestWithSampleRunner
    {

        [Test]
        [Row("ExpectErrorWhenThrowArgNullExAndArgNullExIsError",
            TestStatus.Failed, "error",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Failed, OutcomeCategory=\"error\").")]
        [Row("ExpectErrorWhenThrowArgNullExAndArgExIsError", 
            TestStatus.Failed, "error",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, OutcomeStatus=TestStatus.Failed, OutcomeCategory=\"error\").")]
        [Row("ExpectFailWhenThrowInvalidOpExAndArgNullExIsError", 
            TestStatus.Failed, null,
            MarkupStreamNames.Failures, "Execute\nSystem.InvalidOperationException: Operation is not valid due to the current state of the object.")]
        [Row("ExpectPassWhenExceptionNotThrownAndArgNullExIsError", 
            TestStatus.Passed, null, 
            null, null)]

        [Row("ExpectErrorWhenThrowMatchingArgNullExAndArgNullExWithMessageSubstringIsError", 
            TestStatus.Failed, "error",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Failed, OutcomeCategory=\"error\", ExceptionMessage=\"expectedmessage\").")]
        [Row("ExpectFailWhenThrowInvOpExWithUnmatchedMessageAndInvOpExWithMessageSubstringIsSkipped", 
            TestStatus.Failed, null,
            MarkupStreamNames.Failures, "Execute\nSystem.InvalidOperationException: a different message than expected\n")]

        [Row("ExpectSkippedErrorFromOutcomeStatusPropertyAndUnspecifiedOutcomeCategoryProperty",
            TestStatus.Skipped, "error",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Skipped, OutcomeCategory=\"error\").")]
        [Row("ExpectFailedCustomFromOutcomeCategoryPropertyAndUnspecifiedOutcomeStatusProperty",
            TestStatus.Failed, "custom",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Failed, OutcomeCategory=\"custom\").")]
        [Row("ExpectFailedFromOutcomeCategoryPropertyEqualsNullAndUnspecifiedOutcomeStatusProperty",
            TestStatus.Failed, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Failed).")]
        [Row("ExpectPassedCustomFromOutcomeStatusPropertyAndOutcomeCategoryProperty",
            TestStatus.Passed, "custom",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Passed, OutcomeCategory=\"custom\").")]
        [Row("ExpectPassedNullFromOutcomeStatusPropertyEqualsPassedAndOutcomeCategoryPropertyEqualsNull",
            TestStatus.Passed, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Passed).")]
        [Row("ExpectPassedFailedFromOutcomeStatusPropertyAndOutcomeCategoryProperty",
            TestStatus.Passed, "failed",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Passed, OutcomeCategory=\"failed\").")]
        [Row("ExpectFailedPassedFromOutcomeStatusPropertyAndOutcomeCategoryProperty",
            TestStatus.Failed, "passed",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Failed, OutcomeCategory=\"passed\").")]
        [Row("ExpectInconclusiveConclusiveFromOutcomeStatusPropertyAndOutcomeCategoryProperty",
            TestStatus.Inconclusive, "conclusive",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Inconclusive, OutcomeCategory=\"conclusive\").")]

        [Row("ExpectSkippedWhenThrowArgNullExAndArgNullExIsSkipped", 
            TestStatus.Skipped, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, OutcomeStatus=TestStatus.Skipped).")]
        [Row("ExpectIgnoredWhenThrowArgNullExAndArgExIsIgnored", 
            TestStatus.Skipped, "ignored",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, OutcomeStatus=TestStatus.Skipped, OutcomeCategory=\"ignored\").")]

        [Row("ExpectSkippedFromOrder1WhenThrowArgNullExAndArgNullExIsSkippedAndArgExIsInconclusiveAndExIsError", 
            TestStatus.Skipped, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, Order=1, OutcomeStatus=TestStatus.Skipped).")]
        [Row("ExpectInconclusiveFromOrder2WhenThrowArgExAndArgNullExIsSkippedAndArgExIsInconclusiveAndExIsError", 
            TestStatus.Inconclusive, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, Order=2, OutcomeStatus=TestStatus.Inconclusive).")]
        [Row("ExpectErrorFromOrder3WhenThrowNotImplExAndArgNullExIsSkippedAndArgExIsInconclusiveAndExIsError", 
            TestStatus.Failed, "error",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.Exception, Order=3, OutcomeStatus=TestStatus.Failed, OutcomeCategory=\"error\").")]
        [Row("ExpectInconclusiveFromOrder1WhenThrowArgNullExAndArgExIsInconclusiveAndArgNullExIsSkippedAndExIsError", 
            TestStatus.Inconclusive, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, Order=1, OutcomeStatus=TestStatus.Inconclusive).")]
        [Row("ExpectInconclusiveFromOrder1WhenThrowArgExAndArgExIsInconclusiveAndArgNullExIsSkippedAndExIsError", 
            TestStatus.Inconclusive, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, Order=1, OutcomeStatus=TestStatus.Inconclusive).")]
        [Row("ExpectErrorFromOrder3WhenThrowNotImplExAndArgExIsInconclusiveAndArgNullExIsSkippedAndExIsError", 
            TestStatus.Failed, "error",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.Exception, Order=3, OutcomeStatus=TestStatus.Failed, OutcomeCategory=\"error\").")]
        [Row("ExpectPassFromOrder5WhenThrowInvOpExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsPassed", 
            TestStatus.Passed, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.InvalidOperationException, Order=5, OutcomeStatus=TestStatus.Passed).")]

        [Row("ExpectPassWhenThrowArgNullExAndArgExIsExpectedAndArgNullExIsSkipped", 
            TestStatus.Passed, null, 
            null, null)]
        [Row("ExpectFailWhenThrowArgExAndArgNullExIsExpectedAndArgExIsSkipped", 
            TestStatus.Failed, null,
            MarkupStreamNames.Failures, "Expected an exception of type 'System.ArgumentNullException' but a different exception was thrown.")]
        [Row("ExpectPassFromExpExcOrder5WhenThrowInvOpExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsExpected", 
            TestStatus.Passed, null, 
            null, null)]
        [Row("ExpectFailWhenThrowOutOfMemExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsExpected", 
            TestStatus.Failed, null,
            MarkupStreamNames.Failures, "Expected an exception of type 'System.InvalidOperationException' but a different exception was thrown.")]

        //Disabled//[Row("ExpectSkippedWhenThrowArgNullExAndArgNullExIsSkippedAndArgExIsExpected", TestStatus.Skipped, null, null, null)]
        //Disabled//[Row("ExpectSkippedWhenThrowArgExAndArgExIsSkippedAndArgNullExIsExpected", TestStatus.Skipped, null, null, null)]
        //Disabled//[Row("ExpectInconclusiveFromOrder2WhenThrowArgExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsExpected", TestStatus.Inconclusive, null, null)]
        public void CatchExceptionOutcome(string testMethodName, 
            TestStatus expectedOutcomeStatus, string expectedOutcomeCategory, 
            string logStreamName, string expectedLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(CatchExceptionSample).GetMethod(testMethodName)));

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(run);
                Assert.AreEqual(expectedOutcomeStatus, run.Result.Outcome.Status);
                Assert.AreEqual(expectedOutcomeCategory, run.Result.Outcome.Category);

                if (expectedLogOutput != null)
                    AssertLogContains(run, expectedLogOutput, logStreamName ?? MarkupStreamNames.Failures);
            });
        }

        [Explicit("Sample")]
        public class CatchExceptionSample
        {
            #region CatchException Basic Tests

            /// <summary>
            /// Verifies simplest catch scenario.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException))]
            public void ExpectErrorWhenThrowArgNullExAndArgNullExIsError()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies that a CatchException can detect and handle an Exception 
            /// that inherits from the CatchExceptionAttribute.ExceptionType.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentException))]
            public void ExpectErrorWhenThrowArgNullExAndArgExIsError()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies scenario where an unhandled exception Type is thrown by the test.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException))]
            public void ExpectFailWhenThrowInvalidOpExAndArgNullExIsError()
            {
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Verifies scenario where no exception is thrown by the test.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException))]
            public void ExpectPassWhenExceptionNotThrownAndArgNullExIsError()
            {
            }

            #endregion

            #region ExceptionMessage Property Tests

            /// <summary>
            /// Verifies scenario where a handled exception Type with a handled Exception.Message is thrown by the test.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), ExceptionMessage = "expectedmessage")]
            public void ExpectErrorWhenThrowMatchingArgNullExAndArgNullExWithMessageSubstringIsError()
            {
                throw new ArgumentNullException("paramname", "the expectedmessage is part of the message");
            }

            /// <summary>
            /// Verifies scenario where a handled exception Type with an unhandled Exception.Message is thrown by the test.
            /// </summary>
            [Test]
            [CatchException(typeof(InvalidOperationException), ExceptionMessage = "expectedmessage", StandardOutcome = "TestOutcome.Skipped")]
            public void ExpectFailWhenThrowInvOpExWithUnmatchedMessageAndInvOpExWithMessageSubstringIsSkipped()
            {
                throw new InvalidOperationException("a different message than expected");
            }

            #endregion

            #region OutcomeStatus and OutcomeCategory Property Tests

            /// <summary>
            /// Verifies that a custom <see cref="TestOutcome.Status"/> can be specified using 
            /// <see cref="CatchExceptionAttribute.OutcomeStatus"/>.
            /// Also verifies that <see cref="CatchExceptionAttribute.OutcomeStatus"/> 
            /// can be specified without specifying <see cref="CatchExceptionAttribute.OutcomeCategory"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), OutcomeStatus = TestStatus.Skipped)]
            public void ExpectSkippedErrorFromOutcomeStatusPropertyAndUnspecifiedOutcomeCategoryProperty()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies that a custom <see cref="TestOutcome.Category"/> can be specified using 
            /// <see cref="CatchExceptionAttribute.OutcomeCategory"/>.
            /// Also verifies that <see cref="CatchExceptionAttribute.OutcomeCategory"/> 
            /// can be specified without specifying <see cref="CatchExceptionAttribute.OutcomeStatus"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), OutcomeCategory = "custom")]
            public void ExpectFailedCustomFromOutcomeCategoryPropertyAndUnspecifiedOutcomeStatusProperty()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies that a custom <see cref="TestOutcome.Category"/> can be specified using 
            /// <see cref="CatchExceptionAttribute.OutcomeCategory"/>.
            /// Also verifies that <see cref="CatchExceptionAttribute.OutcomeCategory"/> 
            /// can be specified without specifying <see cref="CatchExceptionAttribute.OutcomeStatus"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), OutcomeCategory = null)]
            public void ExpectFailedFromOutcomeCategoryPropertyEqualsNullAndUnspecifiedOutcomeStatusProperty()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies that a custom <see cref="CatchExceptionAttribute.OutcomeCategory"/> can be specified 
            /// even when the value of <see cref="CatchExceptionAttribute.OutcomeStatus"/> 
            /// is <see cref="TestStatus.Passed"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), OutcomeStatus = TestStatus.Passed, OutcomeCategory = "custom")]
            public void ExpectPassedCustomFromOutcomeStatusPropertyAndOutcomeCategoryProperty()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies that a custom <see cref="CatchExceptionAttribute.OutcomeCategory"/> can be specified 
            /// even when the value of <see cref="CatchExceptionAttribute.OutcomeStatus"/> 
            /// is <see cref="TestStatus.Passed"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), OutcomeStatus = TestStatus.Passed, OutcomeCategory = null)]
            public void ExpectPassedNullFromOutcomeStatusPropertyEqualsPassedAndOutcomeCategoryPropertyEqualsNull()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies that an <see cref="CatchExceptionAttribute.OutcomeCategory"/> can be specified 
            /// even when the value of <see cref="CatchExceptionAttribute.OutcomeStatus"/> 
            /// is <see cref="TestStatus.Passed"/>.
            /// Also verifies that unusual value pairs can be specified (using a standard category) for 
            /// <see cref="TestOutcome.Status"/> and <see cref="TestOutcome.Category"/> 
            /// using <see cref="CatchExceptionAttribute.OutcomeStatus"/> and 
            /// <see cref="CatchExceptionAttribute.OutcomeCategory"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), OutcomeStatus = TestStatus.Passed, OutcomeCategory = "failed")]
            public void ExpectPassedFailedFromOutcomeStatusPropertyAndOutcomeCategoryProperty()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies that unusual value pairs can be specified (using a standard category) for 
            /// <see cref="TestOutcome.Status"/> and <see cref="TestOutcome.Category"/> 
            /// using <see cref="CatchExceptionAttribute.OutcomeStatus"/> and 
            /// <see cref="CatchExceptionAttribute.OutcomeCategory"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), OutcomeStatus = TestStatus.Failed, OutcomeCategory = "passed")]
            public void ExpectFailedPassedFromOutcomeStatusPropertyAndOutcomeCategoryProperty()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies that unusual value pairs can be specified (using a custom category) for 
            /// <see cref="TestOutcome.Status"/> and <see cref="TestOutcome.Category"/> 
            /// using <see cref="CatchExceptionAttribute.OutcomeStatus"/> and 
            /// <see cref="CatchExceptionAttribute.OutcomeCategory"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), OutcomeStatus = TestStatus.Inconclusive, OutcomeCategory = "conclusive")]
            public void ExpectInconclusiveConclusiveFromOutcomeStatusPropertyAndOutcomeCategoryProperty()
            {
                throw new ArgumentNullException();
            }

            #endregion

            #region StandardOutcome Property Tests

            /// <summary>
            /// Verifies that a non-Error TestOutcome can be specified when handling an Exception.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped")]
            public void ExpectSkippedWhenThrowArgNullExAndArgNullExIsSkipped()
            {
                throw new ArgumentNullException();
            }

            /// <summary>
            /// Verifies that a non-Error TestOutcome can be specified when handling an Exception 
            /// that inherits from the CatchExceptionAttribute.ExceptionType.
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Ignored")]
            public void ExpectIgnoredWhenThrowArgNullExAndArgExIsIgnored()
            {
                throw new ArgumentNullException();
            }

            #endregion

            #region Multiple CatchException Tests

            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(Exception), Order = 3)]
            public void ExpectSkippedFromOrder1WhenThrowArgNullExAndArgNullExIsSkippedAndArgExIsInconclusiveAndExIsError()
            {
                throw new ArgumentNullException();
            }

            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(Exception), Order = 3)]
            public void ExpectInconclusiveFromOrder2WhenThrowArgExAndArgNullExIsSkippedAndArgExIsInconclusiveAndExIsError()
            {
                throw new ArgumentException();
            }

            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(Exception), Order = 3)]
            public void ExpectErrorFromOrder3WhenThrowNotImplExAndArgNullExIsSkippedAndArgExIsInconclusiveAndExIsError()
            {
                throw new NotImplementedException();
            }

            [Test]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 1)]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 2)]
            [CatchException(typeof(Exception), Order = 3)]
            public void ExpectInconclusiveFromOrder1WhenThrowArgNullExAndArgExIsInconclusiveAndArgNullExIsSkippedAndExIsError()
            {
                throw new ArgumentNullException();
            }

            [Test]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 1)]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 2)]
            [CatchException(typeof(Exception), Order = 3)]
            public void ExpectInconclusiveFromOrder1WhenThrowArgExAndArgExIsInconclusiveAndArgNullExIsSkippedAndExIsError()
            {
                throw new ArgumentException();
            }

            [Test]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 1)]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 2)]
            [CatchException(typeof(Exception), Order = 3)]
            public void ExpectErrorFromOrder3WhenThrowNotImplExAndArgExIsInconclusiveAndArgNullExIsSkippedAndExIsError()
            {
                throw new NotImplementedException();
            }

            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending", Order = 3)]
            [CatchException(typeof(NotSupportedException), StandardOutcome = "TestOutcome.Ignored", Order = 4)]
            [CatchException(typeof(InvalidOperationException), StandardOutcome = "TestOutcome.Passed", Order = 5)]
            public void ExpectPassFromOrder5WhenThrowInvOpExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsPassed()
            {
                throw new InvalidOperationException();
            }

            #endregion

            #region CatchException+ExpectedException Combo Tests

            /// <summary>
            /// Verifies that an ExpectedException can detect an Exception before a CatchException sees it 
            /// (effectively overriding the behavior of the CatchException).
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 2)]
            [ExpectedException(typeof(ArgumentException), Order = 1)]
            public void ExpectPassWhenThrowArgNullExAndArgExIsExpectedAndArgNullExIsSkipped()
            {
                throw new ArgumentNullException();
            }

            [Test]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Skipped", Order = 2)]
            [ExpectedException(typeof(ArgumentNullException), Order = 1)]
            public void ExpectFailWhenThrowArgExAndArgNullExIsExpectedAndArgExIsSkipped()
            {
                throw new ArgumentException();
            }

            /// <summary>
            /// Verifies that multiple CatchExceptions can transparently detect and rethrow an Exception before an ExpectedException gets it 
            /// (without changing the behavior of the ExpectedException).
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending", Order = 3)]
            [CatchException(typeof(NotSupportedException), StandardOutcome = "TestOutcome.Ignored", Order = 4)]
            [ExpectedException(typeof(InvalidOperationException), Order = 5)]
            public void ExpectPassFromExpExcOrder5WhenThrowInvOpExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsExpected()
            {
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Verifies that multiple CatchExceptions can transparently detect and rethrow an Exception before an ExpectedException gets it 
            /// (without changing the behavior of the ExpectedException).
            /// </summary>
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending", Order = 3)]
            [CatchException(typeof(NotSupportedException), StandardOutcome = "TestOutcome.Ignored", Order = 4)]
            [ExpectedException(typeof(InvalidOperationException), Order = 5)]
            public void ExpectFailWhenThrowOutOfMemExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsExpected()
            {
                throw new OutOfMemoryException();
            }

            #region Disabled CatchException+ExpectedException Combo Tests

            /// <summary>
            /// Verifies that a CatchException can detect an Exception before an ExpectedException sees it 
            /// (effectively overriding the behavior of the ExpectedException).
            /// </summary>
            [Disable("Disabled because the ExpectedExceptionAttribute seems to execute earlier than indicated by it's Order, and causes a Failed outcome before the CatchExceptionAttribute handles the thrown Exception.")]
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [ExpectedException(typeof(ArgumentException), Order = 2)]
            public void ExpectSkippedWhenThrowArgNullExAndArgNullExIsSkippedAndArgExIsExpected()
            {
                throw new ArgumentNullException();
            }

            [Disable("Disabled because the ExpectedExceptionAttribute seems to execute earlier than indicated by it's Order, and causes a Failed outcome before the CatchExceptionAttribute handles the thrown Exception.")]
            [Test]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [ExpectedException(typeof(ArgumentNullException), Order = 2)]
            public void ExpectSkippedWhenThrowArgExAndArgExIsSkippedAndArgNullExIsExpected()
            {
                throw new ArgumentException();
            }

            /// <summary>
            /// Verifies that a CatchException can detect an Exception before an ExpectedException sees it 
            /// (effectively overriding the behavior of the ExpectedException),
            /// even when there are other CatchExceptions that detect and rethrow the Exception 
            /// (before and after the handling CatchException, and before the ExpectedException).
            /// </summary>
            [Disable("Disabled because the ExpectedExceptionAttribute seems to execute earlier than indicated by it's Order, and causes a Failed outcome before the CatchExceptionAttribute handles the thrown Exception.")]
            [Test]
            [CatchException(typeof(ArgumentNullException), StandardOutcome = "TestOutcome.Skipped", Order = 1)]
            [CatchException(typeof(ArgumentException), StandardOutcome = "TestOutcome.Inconclusive", Order = 2)]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending", Order = 3)]
            [CatchException(typeof(NotSupportedException), StandardOutcome = "TestOutcome.Ignored", Order = 4)]
            [ExpectedException(typeof(InvalidOperationException), Order = 5)]
            public void ExpectInconclusiveFromOrder2WhenThrowArgExAndArgNullExIsSkippedAndArgExIsInconclusiveAndNotImplExIsPendingAndNotSuppExIsIgnoredAndInvOpExIsExpected()
            {
                throw new ArgumentException();
            }

            #endregion

            #endregion

            #region Example Tests

            /// <summary>
            /// This test will have an outcome of <see cref="TestOutcome.Error"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(NotSupportedException))]
            public void CatchExceptionExample1()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// This test will have an outcome of <see cref="TestOutcome.Pending"/>.
            /// </summary>
            [Test]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending")]
            public void CatchExceptionExample2()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// This test will have an outcome of <see cref="TestOutcome.Passed"/> 
            /// whether the test throws a <see cref="NotImplementedException"/> or throws no exception.
            /// </summary>
            [Test]
            [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Passed")]
            public void CatchExceptionExample3()
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
            [CatchException(typeof(TimeoutException), ExceptionMessage = "server is not responding", OutcomeStatus = TestStatus.Inconclusive, OutcomeCategory = "timeout", Order = 3)]
            public void CatchExceptionExample4()
            {
                //throw new NotSupportedException(); //outcome would be: new TestOutcome(TestStatus.Skipped, "notsupported")
                throw new NotImplementedException(); //outcome would be: TestOutcome.Pending
                //throw new TimeoutException("A deadlock occurred."); //outcome would be: new TestOutcome(TestStatus.Inconclusive, "deadlock")
                //throw new TimeoutException("The server is not responding."); //outcome would be: new TestOutcome(TestStatus.Inconclusive, "timeout")
                //throw new TimeoutException("Transaction timeout."); //outcome would be: TestOutcome.Failed
                //throw new TimeoutException(); //outcome would be: TestOutcome.Failed
                //throw new ArithmeticException(); //outcome would be: TestOutcome.Failed
                //return; //outcome would be: TestOutcome.Passed
            }

            #endregion
        }
    }
}
