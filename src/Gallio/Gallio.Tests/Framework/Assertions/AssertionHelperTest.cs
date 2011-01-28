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
using System.Threading;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Common.Diagnostics;
using Gallio.Common.Markup;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Assertions
{
    [TestsOn(typeof(AssertionHelper))]
    public class AssertionHelperTest
    {
        [Test]
        public void Verify_WhenAssertionFuncIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssertionHelper.Verify(null));
        }

        [Test]
        public void Verify_WhenCalled_IncrementsAssertionCountEachTime()
        {
            AssertionHelper.Verify(() => null);
            int firstCount = TestContext.CurrentContext.AssertCount;

            try
            {
                AssertionHelper.Verify(() => new AssertionFailureBuilder("Boom").ToAssertionFailure());
            }
            catch (AssertionFailureException)
            {
            }
            int secondCount = TestContext.CurrentContext.AssertCount;

            Assert.AreEqual(1, firstCount);
            Assert.AreEqual(2, secondCount);
        }

        [Test]
        public void Verify_WhenAssertionFuncReturnsNull_DoesNotSubmitAnyFailures()
        {
            var failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Verify(() => null);
            });

            Assert.IsEmpty(failures);
        }

        [Test]
        public void Verify_WhenAssertionFuncReturnsNonNull_SubmitsTheFailure()
        {
            var failure = new AssertionFailureBuilder("Boom").ToAssertionFailure();
            var failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Verify(() => failure);
            });

            Assert.AreElementsEqual(new[] { failure }, failures);
        }

        [Test]
        public void Verify_WhenAssertionFuncThrowsAssertionFailureException_ResubmitsTheFailure()
        {
            var failure = new AssertionFailureBuilder("Boom").ToAssertionFailure();
            var failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Verify(() =>
                {
                    throw new AssertionFailureException(failure, false);
                });
            });

            Assert.AreElementsEqual(new[] { failure }, failures);
        }

        [Test]
        public void Verify_WhenAssertionFuncThrowsTestInconclusiveException_RethrowsTheException()
        {
            Assert.Throws<TestInconclusiveException>(() =>
            {
                throw new TestInconclusiveException();
            });
        }

        [Test]
        public void Verify_WhenAssertionFuncThrowsSomeOtherException_WrapsTheExceptionAsAFailure()
        {
            var failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Verify(() =>
                {
                    throw new InvalidOperationException("Boom");
                });
            });

            Assert.Count(1, failures);
            Assert.AreEqual("An exception occurred while verifying an assertion.", failures[0].Description);
            Assert.Count(1, failures[0].Exceptions);
            Assert.Contains(failures[0].Exceptions[0].ToString(), "Boom");
        }

        [Test]
        public void Fail_WhenFailureIsNull_DoesNothing()
        {
            var failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Fail(null);
            });

            Assert.IsEmpty(failures);
        }

        [Test]
        public void Fail_WhenFailureIsNotNull_SubmitsTheFailure()
        {
            var failure = new AssertionFailureBuilder("Boom").ToAssertionFailure();
            var failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Fail(failure);
            });

            Assert.AreElementsEqual(new[] { failure }, failures);
        }

        [Test]
        public void Eval_WhenActionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssertionHelper.Eval(null));
        }

        [Test]
        public void EvalWithBehavior_WhenActionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssertionHelper.Eval(null, AssertionFailureBehavior.Discard));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Explain_should_throw_exception_when_action_argument_is_null()
        {
            AssertionHelper.Explain(null, 
                innerFailures =>
                    new AssertionFailureBuilder("Boom!")
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Explain_should_throw_exception_when_explanation_argument_is_null()
        {
            AssertionHelper.Explain(() => { }, null);
        }

        [Test]
        public void Explain_should_decorate_an_inner_assertion_failure()
        {
            AssertionFailure[] failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Explain(() =>
                {
                    Assert.Fail("Inner reason");

                }, innerFailures =>
                    new AssertionFailureBuilder("Outer reason")
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
            });

            Assert.Count(1, failures);
            Assert.AreEqual("Outer reason", failures[0].Description);
            Assert.Count(1, failures[0].InnerFailures);
            Assert.AreEqual("Inner reason", failures[0].InnerFailures[0].Message);
        }

        [Test]
        public void Explain_should_decorate_several_inner_assertion_failures()
        {
            AssertionFailure[] failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Explain(() =>
                {
                    Assert.Fail("Inner reason #1");
                    Assert.Fail("Inner reason #2");

                }, AssertionFailureBehavior.CaptureAndContinue, innerFailures =>
                    new AssertionFailureBuilder("Outer reason")
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
            });

            Assert.Count(1, failures);
            Assert.AreEqual("Outer reason", failures[0].Description);
            Assert.Count(2, failures[0].InnerFailures);
            Assert.AreEqual("Inner reason #1", failures[0].InnerFailures[0].Message);
            Assert.AreEqual("Inner reason #2", failures[0].InnerFailures[1].Message);
        }

        [Test]
        public void Explain_should_return_empty_result_when_no_failures_have_occured()
        {
            AssertionFailure[] failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Explain(() =>
                {
                    // No failing assertion.
                
                }, innerFailures =>
                    new AssertionFailureBuilder("Ugh?")
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
            });

            Assert.IsEmpty(failures);
        }
    }
}