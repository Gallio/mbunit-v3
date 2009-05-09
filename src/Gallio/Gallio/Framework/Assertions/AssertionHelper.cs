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
        /// <para>
        /// Verifies that an assertion succeeded.
        /// </para>
        /// <para>
        /// If the assertion function returns null then the assertion is deemed to have passed.
        /// If it returns an <see cref="AssertionFailure" /> or throws an exception,
        /// then is is deemed to have failed.
        /// </para>
        /// <para>
        /// When an assertion failure is detected, it is submitted to <see cref="AssertionContext.SubmitFailure"/>
        /// which may choose to throw a <see cref="AssertionFailureException" /> or do something else.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Using this method enables the system to track statistics about assertions
        /// and to ensure that assertion failures are reported uniformly.
        /// </para>
        /// <para>
        /// It is important to note that not all failures will result in a <see cref="AssertionFailureException"/>
        /// being thrown.  Refer to <see cref="AssertionContext.SubmitFailure"/> for details.
        /// </para>
        /// </remarks>
        /// <param name="assertionFunc">The assertion function to evaluate</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assertionFunc"/> is null</exception>
        public static void Verify(Func<AssertionFailure> assertionFunc)
        {
            if (assertionFunc == null)
                throw new ArgumentNullException("assertionFunc");

            TestContext.CurrentContext.IncrementAssertCount();

            AssertionFailure failure;
            try
            {
                failure = assertionFunc();
            }
            catch (ThreadAbortException)
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
        /// <param name="failure">Failure to be submited, or null if none</param>
        public static void Fail(AssertionFailure failure)
        {
            if (failure != null)
                AssertionContext.CurrentContext.SubmitFailure(failure);
        }

        /// <summary>
        /// <para>
        /// Performs an action and returns an array containing the assertion failures
        /// that were observed within the block.  If the block throws an exception, it
        /// is reified as an assertion failure.
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
        /// </summary>
        /// <param name="action">The action to invoke</param>
        /// <returns>The array of failures, may be empty if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static AssertionFailure[] Eval(Action action)
        {
            return Eval(action, AssertionFailureBehavior.Throw);
        }

        /// <summary>
        /// <para>
        /// Performs an action and returns an array containing the assertion failures
        /// that were observed within the block.  If the block throws an exception, it
        /// is reified as an assertion failure.
        /// </para>
        /// </summary>
        /// <param name="action">The action to invoke</param>
        /// <param name="assertionFailureBehavior">The assertion failure behavior to use while the action runs</param>
        /// <returns>The array of failures, may be empty if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static AssertionFailure[] Eval(Action action, AssertionFailureBehavior assertionFailureBehavior)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            return AssertionContext.CurrentContext.CaptureFailures(action, assertionFailureBehavior, true);
        }

        /// <summary>
        /// A delegate for the <see cref="Explain" /> decorator method which combines the specified
        /// inner failures into a single outer failure with a common explanation.
        /// </summary>
        /// <param name="innerFailures">The inner failures to combine together.</param>
        /// <returns></returns>
        public delegate AssertionFailure Explanation(AssertionFailure[] innerFailures);

        /// <summary>
        /// Performs an action and combines the possible assertion failures
        /// that were observed within the block, into a single outer failure with
        /// a common explanation.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="explanation">A function that takes an array of inner failures and
        /// returns a single outer failure with a common explanation.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="explanation"/> is null</exception>
        public static void Explain(Action action, Explanation explanation)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (explanation == null)
                throw new ArgumentNullException("explanation");

            AssertionHelper.Verify(() =>
            {
                AssertionFailure[] failures = AssertionHelper.Eval(() =>
                {
                    action();
                });

                return failures.Length == 0 ? null : explanation(failures);
            });
        }
    }
}