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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Gallio;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Model.Diagnostics;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Defines a set of assertions.
    /// </para>
    /// <para>
    /// Each assertion is generally provided in at least 4 flavors distinguished by overloads:
    /// <list type="bullet">
    /// <item>A simple form that takes only the assertion parameters.</item>
    /// <item>A simple form that accepts a custom message format string and arguments in addition to the assertion parameters.</item>
    /// <item>A rich form that takes the assertion parameters and a custom comparer object.</item>
    /// <item>A rich form that accepts a custom message format string and arguments in addition to the assertion parameters and a custom comparer object.</item>
    /// </list>
    /// </para>
    /// <para>
    /// The value upon which the assertion is being evaluated is usually called the "actual value".
    /// Other parameters for the assertion are given names such as the "expected value", "unexpected value",
    /// or other terms as appropriate.  In some cases where the role of a parameter is ambiguous,
    /// we may use designations such as "left" and "right" to distinguish the parameters.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Assert class does not provide direct support for old-style collection types such as <see cref="ICollection" />
    /// and <see cref="IEnumerable" />.  If you are using .Net 3.5 for your test projects, you may find the
    /// "Cast" function helpful.
    /// <example>
    /// <code>
    /// ICollection myOldCollection = subjectUnderTest.DoSomething();
    /// Assert.AreElementsEqual(new[] { "a", "b", "c" }, myOldCollection.Cast&lt;string&gt;());
    /// </code>
    /// </example>
    /// </para>
    /// <para>
    /// Framework authors may choose to extend this class with additional assertions by creating
    /// a subclass.  Alternately, new assertions can be defined in other classes.
    /// </para>
    /// <para>
    /// When formatting values for inclusion in assertion failures, it is recommended to use the
    /// formatter provided by the <see cref="AssertionFailureBuilder.Formatter" /> property instead
    /// of directly calling <see cref="Object.ToString" />.  This enables custom formatting rules to
    /// decide how best to present values of particular types and yields a more consistent user experience.
    /// In particular the <see cref="AssertionFailureBuilder.AddRawLabeledValue" /> method and
    /// its siblings automatically format values in this manner.
    /// </para>
    /// </remarks>
    [TestFrameworkInternal]
    public abstract partial class Assert
    {
        /// <summary>
        /// Prevents instantiation.
        /// Subclasses should likewise define their constructor to be protected.
        /// </summary>
        protected Assert()
        {
        }

        #region Private stuff
        /// <summary>
        /// Always throws an <see cref="InvalidOperationException" />.
        /// Use <see cref="Assert.AreEqual{T}(T, T)" /> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use Assert.AreEquals instead.")]
        public static new void Equals(object a, object b)
        {
            throw new InvalidOperationException("Assert.Equals should not be used for assertions.  Use Assert.AreEqual instead.");
        }

        /// <summary>
        /// Always throws an <see cref="InvalidOperationException" />.
        /// Use <see cref="Assert.AreSame{T}(T, T)" /> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use Assert.AreSame instead.")]
        public static new void ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("Assert.ReferenceEquals should not be used for assertions.  Use Assert.AreSame instead.");
        }
        #endregion

        #region Syntax Extensions
        /// <summary>
        /// Provides methods for composing assertions to map over complex data structures.
        /// </summary>
        public static AssertOverSyntax Over
        {
            get { return AssertOverSyntax.Instance; }
        }
        #endregion

        #region Fail
        /// <summary>
        /// Signals an assertion failure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use <see cref="AssertionHelper.Verify" /> and <see cref="AssertionHelper.Fail" />
        /// instead when implementing custom assertions.
        /// </para>
        /// </remarks>
        /// <exception cref="AssertionException">Thrown unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Fail()
        {
            Fail(string.Empty);
        }

        /// <summary>
        /// Signals an assertion failure with a particular message.
        /// </summary>
        /// <param name="messageFormat">The format of the assertion failure message</param>
        /// <param name="messageArgs">The arguments for the assertion failure message format string</param>
        /// <remarks>
        /// <para>
        /// Use <see cref="AssertionHelper.Verify" /> and <see cref="AssertionHelper.Fail" />
        /// instead when implementing custom assertions.
        /// </para>
        /// </remarks>
        /// <exception cref="AssertionException">Thrown unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Fail(string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Fail(new AssertionFailureBuilder("An assertion failed.")
                .SetMessage(messageFormat, messageArgs)
                .ToAssertionFailure());
        }
        #endregion

        #region Inconclusive
        /// <summary>
        /// Signals that a test has yielded an inconclusive result.
        /// </summary>
        /// <exception cref="TestInconclusiveException">Thrown always</exception>
        public static void Inconclusive()
        {
            Inconclusive(null, null);
        }

        /// <summary>
        /// Signals that a test has yielded an inconclusive result.
        /// </summary>
        /// <param name="messageFormat">The custom message format string, or null if none</param>
        /// <param name="messageArgs">The custom message arguments, or null if none</param>
        /// <exception cref="TestInconclusiveException">Thrown always</exception>
        public static void Inconclusive(string messageFormat, params object[] messageArgs)
        {
            throw new TestInconclusiveException(messageFormat != null && messageArgs != null
                ? String.Format(messageFormat, messageArgs)
                : messageFormat ?? "The test was inconclusive.");
        }
        #endregion

        #region Multiple
        /// <summary>
        /// <para>
        /// Executes an action delegate that contains multiple related assertions.
        /// </para>
        /// <para>
        /// While the delegate runs, the behavior of assertions is change such that
        /// failures are captured but do not cause a <see cref="AssertionFailureException" />
        /// to be throw.  When the delegate returns, the previous assertion failure behavior
        /// is restored and any captured assertion failures are reported.  The net effect
        /// of this change is that the test can continue to run even after an assertion failure
        /// occurs which can help to provide more information about the problem.
        /// </para>
        /// </summary>
        /// <remarks>
        /// A multiple assertion block is useful for verifying the state of a single
        /// component with many parts that require several assertions to check.
        /// This feature can accelerate debugging because more diagnostic information
        /// become available at once.
        /// </remarks>
        /// <param name="action">The action to invoke</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static void Multiple(Action action)
        {
            Multiple(action, null, null);
        }

        /// <summary>
        /// <para>
        /// Executes an action delegate that contains multiple related assertions.
        /// </para>
        /// <para>
        /// While the delegate runs, the behavior of assertions is change such that
        /// failures are captured but do not cause a <see cref="AssertionFailureException" />
        /// to be throw.  When the delegate returns, the previous assertion failure behavior
        /// is restored and any captured assertion failures are reported.  The net effect
        /// of this change is that the test can continue to run even after an assertion failure
        /// occurs which can help to provide more information about the problem.
        /// </para>
        /// <para>
        /// If the block throws an exception other than an assertion failure, then it is
        /// similarly recorded.
        /// </para>
        /// </summary>
        /// <remarks>
        /// A multiple assertion block is useful for verifying the state of a single
        /// component with many parts that require several assertions to check.
        /// This feature can accelerate debugging because more diagnostic information
        /// become available at once.
        /// </remarks>
        /// <param name="action">The action to invoke</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static void Multiple(Action action, string messageFormat, params object[] messageArgs)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            AssertionHelper.Verify(delegate
            {
                AssertionFailure[] failures = AssertionHelper.Eval(action, AssertionFailureBehavior.Log);
                if (failures.Length == 0)
                    return null;

                string description = failures.Length == 1
                    ? "There was 1 failure within the multiple assertion block."
                    : String.Format("There were {0} failures within the multiple assertion block.", failures.Length);

                return new AssertionFailureBuilder(description)
                    .SetMessage(messageFormat, messageArgs)
                    .ToAssertionFailure();
            });
        }
        #endregion
    }
}
