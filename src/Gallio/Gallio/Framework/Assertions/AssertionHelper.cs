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
using System.Threading;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Diagnostics;

namespace Gallio.Framework.Assertions
{
    /// <summary>
    /// Provides utilities to assist with the implementation of new asserts.
    /// </summary>
    [SystemInternal]
    public abstract class AssertionHelper
    {
        /// <summary>
        /// Verifies that an assertion succeeded.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion function should return null to indicate that the assertion has passed
        /// or <see cref="AssertionFailure" /> to indicate that it has failed.
        /// </para>
        /// <para>
        /// If the assertion function throws an exception it is reported as an assertion failure
        /// unless the exception is a <see cref="TestException"/> (except <see cref="AssertionFailureException"/>)
        /// in which case the exception is rethrown by this method.  This behavior enables special
        /// test exceptions such as <see cref="TestTerminatedException" /> to be used to terminate
        /// the test at any point instead of being reported as assertion failures.
        /// </para>
        /// <para>
        /// When an assertion failure is detected, it is submitted to <see cref="AssertionContext.SubmitFailure"/>
        /// which may choose to throw a <see cref="AssertionFailureException" /> or do something else.
        /// </para>
        /// <para>
        /// Using this method enables the system to track statistics about assertions
        /// and to ensure that assertion failures are reported uniformly.
        /// </para>
        /// <para>
        /// It is important to note that not all failures will result in a <see cref="AssertionFailureException"/>
        /// being thrown.  Refer to <see cref="AssertionContext.SubmitFailure"/> for details.
        /// </para>
        /// </remarks>
        /// <param name="assertionFunc">The assertion function to evaluate.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assertionFunc"/> is null.</exception>
        public static void Verify(Func<AssertionFailure> assertionFunc)
        {
            if (assertionFunc == null)
                throw new ArgumentNullException("assertionFunc");

            TestContext context = TestContext.CurrentContext;
            if (context != null)
                context.IncrementAssertCount();

            AssertionFailure failure;
            try
            {
                failure = assertionFunc();
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (AssertionFailureException ex)
            {
                failure = ex.Failure;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                failure = new AssertionFailureBuilder("An exception occurred while verifying an assertion.")
                    .AddException(ex)
                    .ToAssertionFailure();
            }

            Fail(failure);
        }

        /// <summary>
        /// Submits a failure if the assertion failure object is non-null.
        /// </summary>
        /// <param name="failure">Failure to be submited, or null if none.</param>
        public static void Fail(AssertionFailure failure)
        {
            if (failure != null)
            {
                AssertionContext context = AssertionContext.CurrentContext;
                if (context != null)
                {
                    context.SubmitFailure(failure);
                }
                else
                {
                    throw new AssertionFailureException(failure, false);
                }
            }
        }

        /// <summary>
        /// Performs an action and returns an array containing the assertion failures
        /// that were observed within the block.  If the block throws an exception, it
        /// is reified as an assertion failure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the action throws an exception it is reported as an assertion failure
        /// unless the exception is a <see cref="TestException"/> (except <see cref="AssertionFailureException"/>)
        /// in which case the exception is rethrown by this method.  This behavior enables special
        /// test exceptions such as <see cref="TestTerminatedException" /> to be used to terminate
        /// the test at any point instead of being reported as assertion failures.
        /// </para>
        /// <para>
        /// The assertion failure behavior while the action runs is <see cref="AssertionFailureBehavior.Throw" />
        /// so the action terminates on the first failure.  The assertion failure itself is returned
        /// but it is not logged.
        /// </para>
        /// <para>
        /// This method is very useful for composing assertions because it enables assertions to be
        /// evaluated, and, when they fail, the failure can be recorded as an inner assertion failure
        /// of some larger composite assertion.  For example, this makes it possible to create an assertion
        /// over a collection of items by composing an assertion over a single item.
        /// </para>
        /// </remarks>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The array of failures, may be empty if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        public static AssertionFailure[] Eval(Action action)
        {
            return Eval(action, AssertionFailureBehavior.Throw);
        }

        /// <summary>
        /// Performs an action and returns an array containing the assertion failures
        /// that were observed within the block.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the action throws an exception it is reported as an assertion failure
        /// unless the exception is a <see cref="TestException"/> (except <see cref="AssertionFailureException"/>)
        /// in which case the exception is rethrown by this method.  This behavior enables special
        /// test exceptions such as <see cref="TestTerminatedException" /> to be used to terminate
        /// the test at any point instead of being reported as assertion failures.
        /// </para>
        /// </remarks>
        /// <param name="action">The action to invoke.</param>
        /// <param name="assertionFailureBehavior">The assertion failure behavior to use while the action runs.</param>
        /// <returns>The array of failures, may be empty if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is no current assertion context.</exception>
        public static AssertionFailure[] Eval(Action action, AssertionFailureBehavior assertionFailureBehavior)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            AssertionContext context = AssertionContext.CurrentContext;
            if (context != null)
            {
                return context.CaptureFailures(action, assertionFailureBehavior, true);
            }
            else
            {
                try
                {
                    action();
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (AssertionFailureException ex)
                {
                    return new[] { ex.Failure };
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    return new[] { AssertionFailureBuilder.WrapExceptionAsAssertionFailure(ex) };
                }

                return EmptyArray<AssertionFailure>.Instance;
            }
        }

        /// <summary>
        /// Performs an action and combines the possible assertion failures
        /// that were observed within the block, into a single outer failure with
        /// a common explanation.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="explanation">A function that takes an array of inner failures and
        /// returns a single outer failure with a common explanation.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="explanation"/> is null.</exception>
        public static void Explain(Action action, AssertionFailureExplanation explanation)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (explanation == null)
                throw new ArgumentNullException("explanation");

            Verify(() =>
            {
                AssertionFailure[] failures = Eval(action);
                return failures.Length == 0 ? null : explanation(failures);
            });
        }
    }
}