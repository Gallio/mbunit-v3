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
using Gallio;
using Gallio.Framework;
using Gallio.Framework.Utilities;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides utilities to assist with the implementation of new asserts.
    /// </summary>
    [TestFrameworkInternal]
    public abstract class AssertHelper
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
        /// This method is useful for composing assertions.
        /// </para>
        /// <para>
        /// The assertion failure behavior while the action runs is <see cref="AssertionFailureBehavior.LogAndThrow" />.
        /// </para>
        /// </summary>
        /// <param name="action">The action to invoke</param>
        /// <returns>The array of failures, may be empty if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static AssertionFailure[] Eval(Action action)
        {
            return Eval(action, AssertionFailureBehavior.LogAndThrow);
        }

        /// <summary>
        /// <para>
        /// Performs an action and returns an array containing the assertion failures
        /// that were observed within the block.  If the block throws an exception, it
        /// is reified as an assertion failure.
        /// </para>
        /// <para>
        /// This method is useful for composing assertions.
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
        /// Append user's message to assert one.
        /// </summary>
        /// <param name="assertMessage">Assert message</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <returns>Assert message with custom message</returns>
        internal static string AppendCustomMessage(string assertMessage, string messageFormat, params object[] messageArgs)
        {
            if (String.IsNullOrEmpty(messageFormat))
                return assertMessage;
            return messageArgs != null 
                ? String.Format("{0}\n{1}", assertMessage, String.Format(messageFormat, messageArgs)) 
                : String.Format("{0}\n{1}", assertMessage, messageFormat);
        }
    }
}
