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
using Gallio.Framework.Pattern;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Tests.Framework
{
    [TestsOn(typeof(CompositePatternAttribute))]
    [TestsOn(typeof(CatchExceptionAttribute))]
    [Author("Justin Webster")]
    [RunSample(typeof(CompositePatternAttributeSample))]
    public class CompositePatternAttributeTest : BaseTestWithSampleRunner
    {

        #region Tests

        [Test]

        [Row("ExpectPassedFromMultipleCompositeCatchAttributesWhenNoExThrown",
            TestStatus.Passed, null,
            MarkupStreamNames.Default, null)]
        [Row("ExpectSkippedFromCompositeCatchArgumentExceptionsAttributeIndex0WhenThrowArgNullEx",
            TestStatus.Skipped, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, Order=1, OutcomeStatus=TestStatus.Skipped).")]
        [Row("ExpectIgnoredFromCompositeCatchArgumentExceptionsAttributeIndex2WhenThrowArgOutOfRangeExWithExpectedMessage",
            TestStatus.Skipped, "ignored",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentOutOfRangeException, Order=2, OutcomeStatus=TestStatus.Skipped, OutcomeCategory=\"ignored\", ExceptionMessage=\"expectedmessage\").")]
        [Row("ExpectInconclusiveFromCompositeCatchArgumentExceptionsAttributeIndex3WhenThrowArgEx",
            TestStatus.Inconclusive, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, Order=3, OutcomeStatus=TestStatus.Inconclusive).")]
        [Row("ExpectInconclusiveFromCompositeCatchArgumentExceptionsAttributeIndex3WhenThrowArgOutOfRangeExWithUnexpectedMessage",
            TestStatus.Inconclusive, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, Order=3, OutcomeStatus=TestStatus.Inconclusive).")]
        [Row("ExpectFailedFromCompositeCatchArgumentExceptionsAttributeWhenThrowInvOpEx",
            TestStatus.Failed, null,
            MarkupStreamNames.Default, null)]

        [Row("ExpectPassedFromCompositeCatchMiscExceptionsAttributeWhenNoExThrown",
            TestStatus.Passed, null,
            MarkupStreamNames.Default, null)]
        [Row("ExpectIgnoredFromCompositeCatchMiscExceptionsAttributeIndex0WhenThrowInvCastEx",
            TestStatus.Skipped, "ignored",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.InvalidCastException, Order=1, OutcomeStatus=TestStatus.Skipped, OutcomeCategory=\"ignored\").")]
        [Row("ExpectPendingFromCompositeCatchMiscExceptionsAttributeIndex1WhenThrowArgExWithExpectedMessage",
            TestStatus.Skipped, "pending",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.Exception, Order=2, OutcomeStatus=TestStatus.Skipped, OutcomeCategory=\"pending\", ExceptionMessage=\"expectedmessage\").")]
        [Row("ExpectFailedFromCompositeCatchMiscExceptionsAttributeWhenThrowArgExWithoutMessage",
            TestStatus.Failed, null,
            MarkupStreamNames.Default, null)]
        [Row("ExpectFailedFromCompositeCatchMiscExceptionsAttributeWhenThrowInvOpEx",
            TestStatus.Failed, null,
            MarkupStreamNames.Default, null)]

        [Row("ExpectPassedFromMultipleCompositeCatchAttributesWhenNoExThrown",
            TestStatus.Passed, null,
            MarkupStreamNames.Default, null)]
        [Row("ExpectSkippedFromFirstOfMultipleCompositeCatchAttributesIndex0WhenThrowArgNullEx",
            TestStatus.Skipped, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentNullException, Order=1, OutcomeStatus=TestStatus.Skipped).")]
        //[Row("ExpectIgnoredFromFirstOfMultipleCompositeCatchAttributesIndex2WhenThrowArgOutOfRangeExWithExpectedMessage",
        //    TestStatus.Skipped, "ignored",
        //    MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentOutOfRangeException, Order=2, OutcomeStatus=TestStatus.Skipped, OutcomeCategory=\"ignored\", ExceptionMessage=\"expectedmessage\").")]
        //[Row("ExpectPendingFromFirstOfReversedMultipleCompositeCatchAttributesIndex1WhenThrowArgOutOfRangeExWithExpectedMessage",
        //    TestStatus.Skipped, "pending",
        //    MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.Exception, Order=2, OutcomeStatus=TestStatus.Skipped, OutcomeCategory=\"pending\", ExceptionMessage=\"expectedmessage\").")]
        [Row("ExpectInconclusiveFromFirstOfMultipleCompositeCatchAttributesIndex3WhenThrowArgEx",
            TestStatus.Inconclusive, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, Order=3, OutcomeStatus=TestStatus.Inconclusive).")]
        [Row("ExpectInconclusiveFromFirstOfMultipleCompositeCatchAttributesIndex3WhenThrowArgOutOfRangeExWithUnexpectedMessage",
            TestStatus.Inconclusive, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, Order=3, OutcomeStatus=TestStatus.Inconclusive).")]
        [Row("ExpectFailedFromMultipleCompositeCatchAttributesWhenThrowInvOpEx",
            TestStatus.Failed, null,
            MarkupStreamNames.Default, null)]
        [Row("ExpectIgnoredFromSecondOfCompositeCatchMiscExceptionsAttributeIndex0WhenThrowInvCastEx",
            TestStatus.Skipped, "ignored",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.InvalidCastException, Order=1, OutcomeStatus=TestStatus.Skipped, OutcomeCategory=\"ignored\").")]
        [Row("ExpectPendingFromSecondOfCompositeCatchMiscExceptionsAttributeIndex1WhenThrowArgExWithExpectedMessage",
            TestStatus.Skipped, "pending",
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.Exception, Order=2, OutcomeStatus=TestStatus.Skipped, OutcomeCategory=\"pending\", ExceptionMessage=\"expectedmessage\").")]
        [Row("ExpectInconclusiveFromFirstOfCompositeCatchMiscExceptionsAttributeIndex3WhenThrowArgOutOfRangeExWithoutMessage",
            TestStatus.Inconclusive, null,
            MarkupStreamNames.Default, "Caught exception. An exception was handled by a CatchExceptionAttribute(ExceptionType=System.ArgumentException, Order=3, OutcomeStatus=TestStatus.Inconclusive).")]

        public void SampleRunnerOutcome(string testMethodName, 
            TestStatus expectedOutcomeStatus, string expectedOutcomeCategory, 
            string logStreamName, string expectedLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(CompositePatternAttributeSample).GetMethod(testMethodName)));

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(run);
                Assert.AreEqual(expectedOutcomeStatus, run.Result.Outcome.Status);
                Assert.AreEqual(expectedOutcomeCategory, run.Result.Outcome.Category);

                if (expectedLogOutput != null)
                    AssertLogContains(run, expectedLogOutput, logStreamName ?? MarkupStreamNames.Failures);
            });
        }

        #endregion

        #region Inner Classes

        public class CompositeCatchArgumentExceptionsAttribute : CompositePatternAttribute
        {
            //NOTE: The Order properties of the attributes are intentionally different than the array sequence.
            private readonly PatternAttribute[] _componentPatterns = new PatternAttribute[]
            {
                new CatchExceptionAttribute(typeof(ArgumentNullException)) { StandardOutcome = "TestOutcome.Skipped", Order = 1},
                new CatchExceptionAttribute(typeof(ArgumentOutOfRangeException)) { StandardOutcome = "TestOutcome.Passed", Order = 4},
                new CatchExceptionAttribute(typeof(ArgumentOutOfRangeException)) { StandardOutcome = "TestOutcome.Ignored", ExceptionMessage = "expectedmessage", Order = 2},
                new CatchExceptionAttribute(typeof(ArgumentException)) { StandardOutcome = "TestOutcome.Inconclusive", Order = 3}
            };

            protected override IEnumerable<IPattern> GetPatterns()
            {
                return _componentPatterns;
            }
        }

        public class CompositeCatchMiscExceptionsAttribute : CompositePatternAttribute
        {
            private readonly PatternAttribute[] _componentPatterns = new PatternAttribute[]
            {
                new CatchExceptionAttribute(typeof(InvalidCastException)) { StandardOutcome = "TestOutcome.Ignored", Order = 1},
                new CatchExceptionAttribute(typeof(Exception)) { StandardOutcome = "TestOutcome.Pending", ExceptionMessage = "expectedmessage", Order = 2}
            };

            protected override IEnumerable<IPattern> GetPatterns()
            {
                return _componentPatterns;
            }
        }

        [Explicit("Sample")]
        public class CompositePatternAttributeSample
        {

            #region CompositeCatchArgumentExceptionsAttribute Tests

            [Test]
            [CompositeCatchArgumentExceptions]
            public void ExpectPassedFromCompositeCatchArgumentExceptionsAttributeWhenNoExThrown()
            {
                Assert.IsTrue(true);
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            public void ExpectSkippedFromCompositeCatchArgumentExceptionsAttributeIndex0WhenThrowArgNullEx()
            {
                throw new ArgumentNullException();
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            public void ExpectIgnoredFromCompositeCatchArgumentExceptionsAttributeIndex2WhenThrowArgOutOfRangeExWithExpectedMessage()
            {
                throw new ArgumentOutOfRangeException("value", "expectedmessage");
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            public void ExpectInconclusiveFromCompositeCatchArgumentExceptionsAttributeIndex3WhenThrowArgEx()
            {
                throw new ArgumentException();
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            public void ExpectInconclusiveFromCompositeCatchArgumentExceptionsAttributeIndex3WhenThrowArgOutOfRangeExWithUnexpectedMessage()
            {
                throw new ArgumentOutOfRangeException("value", "unrecognizedmessage");
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            public void ExpectFailedFromCompositeCatchArgumentExceptionsAttributeWhenThrowInvOpEx()
            {
                throw new InvalidOperationException();
            }

            #endregion

            #region CompositeCatchMiscExceptionsAttribute Tests

            [Test]
            [CompositeCatchMiscExceptions]
            public void ExpectPassedFromCompositeCatchMiscExceptionsAttributeWhenNoExThrown()
            {
                Assert.IsTrue(true);
            }

            [Test]
            [CompositeCatchMiscExceptions]
            public void ExpectIgnoredFromCompositeCatchMiscExceptionsAttributeIndex0WhenThrowInvCastEx()
            {
                throw new InvalidCastException();
            }

            [Test]
            [CompositeCatchMiscExceptions]
            public void ExpectPendingFromCompositeCatchMiscExceptionsAttributeIndex1WhenThrowArgExWithExpectedMessage()
            {
                throw new ArgumentException("expectedmessage", "value");
            }

            [Test]
            [CompositeCatchMiscExceptions]
            public void ExpectFailedFromCompositeCatchMiscExceptionsAttributeWhenThrowArgExWithoutMessage()
            {
                throw new ArgumentException();
            }

            [Test]
            [CompositeCatchMiscExceptions]
            public void ExpectFailedFromCompositeCatchMiscExceptionsAttributeWhenThrowInvOpEx()
            {
                throw new InvalidOperationException();
            }

            #endregion

            #region Multiple CompositeCatch...Attribute Tests

            [Test]
            [CompositeCatchArgumentExceptions]
            [CompositeCatchMiscExceptions]
            public void ExpectPassedFromMultipleCompositeCatchAttributesWhenNoExThrown()
            {
                Assert.IsTrue(true);
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            [CompositeCatchMiscExceptions]
            public void ExpectSkippedFromFirstOfMultipleCompositeCatchAttributesIndex0WhenThrowArgNullEx()
            {
                throw new ArgumentNullException();
            }

            /// <remarks>
            /// <para>
            /// NOTE: It appears that the expected outcome of this test and the related test may be non-deterministic.
            /// This test is therefore being disabled for now.
            /// </para>
            /// NOTE: The expected outcome of this test is different than that of 
            /// <see cref="ExpectPendingFromFirstOfReversedMultipleCompositeCatchAttributesIndex1WhenThrowArgOutOfRangeExWithExpectedMessage"/> 
            /// because of the differing order in which the 2 composite catch attributes are applied to each of those test methods.
            /// Since each composite catch attributes contains a <see cref="CatchExceptionAttribute"/> 
            /// that matches the exception thrown by the test method, 
            /// and each of those have equal <see cref="DecoratorPatternAttribute.Order"/> values, 
            /// the outcome therefore depends on which of the two 
            /// <see cref="CatchExceptionAttribute"/>s intercepts the thrown exception first.
            /// </remarks>
            /// <seealso cref="ExpectPendingFromFirstOfReversedMultipleCompositeCatchAttributesIndex1WhenThrowArgOutOfRangeExWithExpectedMessage"/>
            [Test]
            [CompositeCatchArgumentExceptions]
            [CompositeCatchMiscExceptions]
            public void ExpectIgnoredFromFirstOfMultipleCompositeCatchAttributesIndex2WhenThrowArgOutOfRangeExWithExpectedMessage()
            {
                //TODO: Review: Verify whether this and the related test are non-deterministic, and modify the code accordingly.
                throw new ArgumentOutOfRangeException("value", "expectedmessage");
            }

            /// <remarks>
            /// <para>
            /// NOTE: It appears that the expected outcome of this test and the related test may be non-deterministic.
            /// This test is therefore being disabled for now.
            /// </para>
            /// NOTE: The expected outcome of this test is different than that of 
            /// <see cref="ExpectIgnoredFromFirstOfMultipleCompositeCatchAttributesIndex2WhenThrowArgOutOfRangeExWithExpectedMessage"/> 
            /// because of the differing order in which the 2 composite catch attributes are applied to each of those test methods.
            /// Since each composite catch attributes contains a <see cref="CatchExceptionAttribute"/> 
            /// that matches the exception thrown by the test method, 
            /// and each of those have equal <see cref="DecoratorPatternAttribute.Order"/> values, 
            /// the outcome therefore depends on which of the two 
            /// <see cref="CatchExceptionAttribute"/>s intercepts the thrown exception first.
            /// </remarks>
            /// <seealso cref="ExpectIgnoredFromFirstOfMultipleCompositeCatchAttributesIndex2WhenThrowArgOutOfRangeExWithExpectedMessage"/>
            [Test]
            [CompositeCatchMiscExceptions]
            [CompositeCatchArgumentExceptions]
            public void ExpectPendingFromFirstOfReversedMultipleCompositeCatchAttributesIndex1WhenThrowArgOutOfRangeExWithExpectedMessage()
            {
                //TODO: Review: Verify whether this and the related test are non-deterministic, and modify the code accordingly.
                throw new ArgumentOutOfRangeException("value", "expectedmessage");
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            [CompositeCatchMiscExceptions]
            public void ExpectInconclusiveFromFirstOfMultipleCompositeCatchAttributesIndex3WhenThrowArgEx()
            {
                throw new ArgumentException();
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            [CompositeCatchMiscExceptions]
            public void ExpectInconclusiveFromFirstOfMultipleCompositeCatchAttributesIndex3WhenThrowArgOutOfRangeExWithUnexpectedMessage()
            {
                throw new ArgumentOutOfRangeException("value", "unrecognizedmessage");
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            [CompositeCatchMiscExceptions]
            public void ExpectFailedFromMultipleCompositeCatchAttributesWhenThrowInvOpEx()
            {
                throw new InvalidOperationException();
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            [CompositeCatchMiscExceptions]
            public void ExpectIgnoredFromSecondOfCompositeCatchMiscExceptionsAttributeIndex0WhenThrowInvCastEx()
            {
                throw new InvalidCastException();
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            [CompositeCatchMiscExceptions]
            public void ExpectPendingFromSecondOfCompositeCatchMiscExceptionsAttributeIndex1WhenThrowArgExWithExpectedMessage()
            {
                throw new ArgumentException("expectedmessage", "value");
            }

            [Test]
            [CompositeCatchArgumentExceptions]
            [CompositeCatchMiscExceptions]
            public void ExpectInconclusiveFromFirstOfCompositeCatchMiscExceptionsAttributeIndex3WhenThrowArgOutOfRangeExWithoutMessage()
            {
                throw new ArgumentException();
            }

            #endregion

        }

        #endregion

    }
}
