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
    /// Framework authors may choose to extend this class with additional assertions by creating
    /// a subclass.  Alternately, new assertions can be defined in other classes.
    /// </para>
    /// <para>
    /// When formatting values for inclusion in assertion failures, it is recommended to use the
    /// formatter provided by the <see cref="AssertionFailureBuilder.Formatter" /> property instead
    /// of directly calling <see cref="Object.ToString" />.  This enables custom formatting rules to
    /// decide how best to present values of particular types and yields a more consistent user experience.
    /// In particular the <see cref="AssertionFailureBuilder.SetRawLabeledValue" /> method and
    /// its siblings automatically format values in this manner.
    /// </para>
    /// </remarks>
    [TestFrameworkInternal]
    public abstract class Assert
    {
        /// <summary>
        /// Prevents instatiation.
        /// </summary>
        protected Assert()
        {
        }

        #region Private stuff
        /// <summary>
        /// Always throws a <see cref="InvalidOperationException" />.
        /// Use <see cref="Assert.AreEqual{T}(T, T)" /> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static new void Equals(object a, object b)
        {
            throw new InvalidOperationException("Assert.Equals should not be used for assertions.  Use Assert.AreEqual instead.");
        }

        /// <summary>
        /// Always throws a <see cref="InvalidOperationException" />.
        /// Use <see cref="Assert.AreSame{T}(T, T)" /> instead.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static new void ReferenceEquals(object a, object b)
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

        #region AreEqual
        /// <summary>
        /// Verifies that an actual value equals some expected value.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue)
        {
            AreEqual<T>(expectedValue, actualValue, (string)null, null);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, string messageFormat, params object[] messageArgs)
        {
            AreEqual<T>(expectedValue, actualValue, (Func<T, T, bool>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, IEqualityComparer<T> comparer)
        {
            AreEqual<T>(expectedValue, actualValue, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, IEqualityComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AreEqual<T>(expectedValue, actualValue, comparer != null ? comparer.Equals : (Func<T, T, bool>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, Func<T, T, bool> comparer)
        {
            AreEqual<T>(expectedValue, actualValue, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, Func<T, T, bool> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (comparer == null)
                    comparer = DefaultEqualityComparer;

                if (comparer(expectedValue, actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected values to be equal.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawExpectedAndActualValueWithDiffs(expectedValue, actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreNotEqual
        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, (string)null, null);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, string messageFormat, params object[] messageArgs)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, (Func<T, T, bool>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, IEqualityComparer<T> comparer)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, IEqualityComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, comparer != null ? comparer.Equals : (Func<T, T, bool>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, Func<T, T, bool> comparer)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, Func<T, T, bool> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (comparer == null)
                    comparer = DefaultEqualityComparer;

                if (! comparer(unexpectedValue, actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected values to be non-equal.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Unexpected Value", unexpectedValue)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreSame
        /// <summary>
        /// Verifies that an actual value is referentially identical to some expected value.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreSame<T>(T expectedValue, T actualValue)
            where T : class
        {
            AreSame<T>(expectedValue, actualValue, (string)null, null);
        }

        /// <summary>
        /// Verifies that an actual value is referentially identical to some expected value.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreSame<T>(T expectedValue, T actualValue, string messageFormat, params object[] messageArgs)
            where T : class
        {
            AssertionHelper.Verify(delegate
            {
                if (Object.ReferenceEquals(expectedValue, actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected values to be referentially identical.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawExpectedAndActualValueWithDiffs(expectedValue, actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreNotSame
        /// <summary>
        /// Verifies that an actual value is not referentially identical to some unexpected value.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotSame<T>(T unexpectedValue, T actualValue)
            where T : class
        {
            AreNotSame<T>(unexpectedValue, actualValue, (string)null, null);
        }

        /// <summary>
        /// Verifies that an actual value is not referentially identical to some unexpected value.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotSame<T>(T unexpectedValue, T actualValue, string messageFormat, params object[] messageArgs)
            where T : class
        {
            AssertionHelper.Verify(delegate
            {
                if (! Object.ReferenceEquals(unexpectedValue, actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected values to be referentially different.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Unexpected Value", unexpectedValue)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region Contains
        /// <summary>
        /// Verifies that a string contains some expected value.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedSubstring">The expected substring</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Contains(string actualValue, string expectedSubstring)
        {
            Contains(actualValue, expectedSubstring, null);
        }

        /// <summary>
        /// Verifies that a string contains some expected value.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedSubstring">The expected substring</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Contains(string actualValue, string expectedSubstring, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (actualValue.Contains(expectedSubstring))
                    return null;

                return new AssertionFailureBuilder("Expected string to contain a particular substring.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Expected Substring", expectedSubstring)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }

        /// <summary>
        /// Asserts that <paramref name="expectedValue"/> is in the enumeration <paramref name="enumeration"/>.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="enumeration">The enumeration of items</param>
        /// <param name="expectedValue">The expected value expected to be found in the collection</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void Contains<T>(IEnumerable<T> enumeration, T expectedValue)
        {
            Contains(enumeration, expectedValue, null);
        }

        /// <summary>
        /// Asserts that <paramref name="expectedValue"/> is in the enumeration <paramref name="enumeration"/>.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="enumeration">The enumeration of items</param>
        /// <param name="expectedValue">The expected value expected to be found in the collection</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void Contains<T>(IEnumerable<T> enumeration, T expectedValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                foreach (T item in enumeration)
                    if (DefaultEqualityComparer(expectedValue, item))
                        return null;

                return new AssertionFailureBuilder("Expected the value to appear within the enumeration.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawExpectedValue(expectedValue)
                    .SetRawLabeledValue("Enumeration", enumeration)
                    .ToAssertionFailure();
            });
        }

        /// <summary>
        /// Asserts that <paramref name="key"/> is in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The dictionary of items</param>
        /// <param name="key">The key expected to be found in the dictionary</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            ContainsKey(dictionary, key, null);
        }

        /// <summary>
        /// Asserts that <paramref name="key"/> is in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The dictionary of items</param>
        /// <param name="key">The key expected to be found in the dictionary</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (dictionary.ContainsKey(key))
                    return null;

                return new AssertionFailureBuilder("Expected the key to appear within the dictionary.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Key", key)
                    .SetRawLabeledValue("Dictionary", dictionary)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region DoesNotContain
        /// <summary>
        /// Verifies that a string does not contain some unexpected substring.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="unexpectedSubstring">The unexpected substring</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void DoesNotContain(string actualValue, string unexpectedSubstring)
        {
            DoesNotContain(actualValue, unexpectedSubstring, null);
        }

        /// <summary>
        /// Verifies that a string does not contain some unexpected substring.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="unexpectedSubstring">The unexpected substring</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void DoesNotContain(string actualValue, string unexpectedSubstring, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (! actualValue.Contains(unexpectedSubstring))
                    return null;

                return new AssertionFailureBuilder("Expected string to not contain a particular substring.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Unexpected Substring", unexpectedSubstring)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsTrue
        /// <summary>
        /// Verifies that an actual value is true.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsTrue(bool actualValue)
        {
            IsTrue(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is true.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsTrue(bool actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (actualValue)
                    return null;

                return new AssertionFailureBuilder("Expected value to be true.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsFalse
        /// <summary>
        /// Verifies that an actual value is false.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsFalse(bool actualValue)
        {
            IsFalse(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is false.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsFalse(bool actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (!actualValue)
                    return null;

                return new AssertionFailureBuilder("Expected value to be false.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsNull
        /// <summary>
        /// Verifies that an actual value is null.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsNull(object actualValue)
        {
            IsNull(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is null.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsNull(object actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (actualValue == null)
                    return null;

                return new AssertionFailureBuilder("Expected value to be null.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsNotNull
        /// <summary>
        /// Verifies that an actual value is not null.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsNotNull(object actualValue)
        {
            IsNotNull(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is not null.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsNotNull(object actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (actualValue != null)
                    return null;

                return new AssertionFailureBuilder("Expected value to be non-null.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsEmpty
        /// <summary>
        /// Verifies that an actual value contains no elements.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsEmpty(IEnumerable actualValue)
        {
            IsEmpty(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value contains no elements.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsEmpty(IEnumerable actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (! actualValue.GetEnumerator().MoveNext())
                    return null;

                return new AssertionFailureBuilder("Expected value to be empty.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsNotEmpty
        /// <summary>
        /// Verifies that an actual value contains at least one element.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsNotEmpty(IEnumerable actualValue)
        {
            IsNotEmpty(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value contains at least one element.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void IsNotEmpty(IEnumerable actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (actualValue.GetEnumerator().MoveNext())
                    return null;

                return new AssertionFailureBuilder("Expected value to be non-empty.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsAssignableFrom
        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expectedType">The Type to compare with the object's Type</param>
        /// <param name="actualValue">The object under examination</param>
        public static void IsAssignableFrom(Type expectedType, object actualValue)
        {
            IsAssignableFrom(expectedType, actualValue, null, null);
        }

        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expectedType">The Type to compare with the object's Type</param>
        /// <param name="actualValue">The object under examination</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        public static void IsAssignableFrom(Type expectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (expectedType == null)
                throw new ArgumentNullException("expectedType");
            if (actualValue == null)
                throw new ArgumentNullException("actualValue");

            AssertionHelper.Verify(delegate
            {
                if (actualValue.GetType().IsAssignableFrom(expectedType))
                    return null;

                return new AssertionFailureBuilder("Expected the actual type to be assignable to the expected type.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Actual Type", actualValue.GetType())
                    .SetRawLabeledValue("Expected Type", expectedType)
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region IsNotAssignableFrom
        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expectedType">The Type to compare with the object's Type</param>
        /// <param name="actualValue">The object under examination</param>
        public static void IsNotAssignableFrom(Type expectedType, object actualValue)
        {
            IsNotAssignableFrom(expectedType, actualValue, null, null);
        }

        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expectedType">The Type to compare with the object's Type</param>
        /// <param name="actualValue">The object under examination</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        public static void IsNotAssignableFrom(Type expectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (expectedType == null)
                throw new ArgumentNullException("expectedType");
            if (actualValue == null)
                throw new ArgumentNullException("actualValue");

            AssertionHelper.Verify(delegate
            {
                if (!actualValue.GetType().IsAssignableFrom(expectedType))
                    return null;

                return new AssertionFailureBuilder("Expected the actual type not to be assignable to the expected type.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Actual Type", actualValue.GetType())
                    .SetRawLabeledValue("Expected Type", expectedType)
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region IsInstanceOfType
        /// <summary>
        /// Verifies that an actual value is an instance of some expected type.
        /// </summary>
        /// <param name="expectedType">The expected type</param>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedType"/> is null</exception>
        public static void IsInstanceOfType(Type expectedType, object actualValue)
        {
            IsInstanceOfType(expectedType, actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is an instance of some expected type.
        /// </summary>
        /// <param name="expectedType">The expected type</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedType"/> is null</exception>
        public static void IsInstanceOfType(Type expectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (expectedType == null)
                throw new ArgumentNullException("expectedType");

            AssertionHelper.Verify(delegate
            {
                if (expectedType.IsInstanceOfType(actualValue))
                    return null;

                AssertionFailureBuilder builder = new AssertionFailureBuilder("Expected value to be an instance of a particular type.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Expected Type", expectedType);
                if (actualValue != null)
                    builder.SetRawLabeledValue("ActualType", actualValue.GetType());
                return builder
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsNotInstanceOfType
        /// <summary>
        /// Verifies that an actual value is not an instance of some unexpected type.
        /// </summary>
        /// <param name="unexpectedType">The unexpected type</param>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedType"/> is null</exception>
        public static void IsNotInstanceOfType(Type unexpectedType, object actualValue)
        {
            IsNotInstanceOfType(unexpectedType, actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is not an instance of some unexpected type.
        /// </summary>
        /// <param name="unexpectedType">The unexpected type</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedType"/> is null</exception>
        public static void IsNotInstanceOfType(Type unexpectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (unexpectedType == null)
                throw new ArgumentNullException("unexpectedType");

            AssertionHelper.Verify(delegate
            {
                if (! unexpectedType.IsInstanceOfType(actualValue))
                    return null;

                AssertionFailureBuilder builder = new AssertionFailureBuilder("Expected value to not be an instance of a particular type.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Unexpected Type", unexpectedType);
                if (actualValue != null)
                    builder.SetRawLabeledValue("ActualType", actualValue.GetType());
                return builder
                    .SetRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region GreaterThan

        /// <summary>
        /// Verifies that an left value is greater than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        public static void GreaterThan<T>(T left, T right)
        {
            GreaterThan(left, right, (string)null, null);
        }

        /// <summary>
        /// Verifies that an left value is greater than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThan<T>(T left, T right, string messageFormat, params object[] messageArgs)
        {
            GreaterThan(left, right, null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an left value is greater than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThan<T>(T left, T right, Comparison<T> comparer)
        {
            GreaterThan(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an left value is greater than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThan<T>(T left, T right, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertOrder(left, right, comparer
                    , "Expected left to be greater than right."
                    , compareResult => compareResult > 0
                    , messageFormat, messageArgs);
        }
        #endregion

        #region GreaterThanOrEqual
        /// <summary>
        /// Verifies that an left value is greater or equal than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        public static void GreaterThanOrEqual<T>(T left, T right)
        {
            GreaterThanOrEqual(left, right, (string)null, null);
        }

        /// <summary>
        /// Verifies that an left value is greater or equal than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThanOrEqual<T>(T left, T right, string messageFormat, params object[] messageArgs)
        {
            GreaterThanOrEqual(left, right, null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an left value is greater or equal than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThanOrEqual<T>(T left, T right, Comparison<T> comparer)
        {
            GreaterThanOrEqual(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an left value is greater or equal than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThanOrEqual<T>(T left, T right, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertOrder(left, right, comparer
                 , "Expected left to be greater or equal than right."
                 , compareResult => compareResult >= 0
                 , messageFormat, messageArgs);
        }

        #endregion

        #region LessThan

        /// <summary>
        /// Verifies that an left value is less than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        public static void LessThan<T>(T left, T right)
        {
            LessThan(left, right, (string)null, null);
        }

        /// <summary>
        /// Verifies that an left value is less than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThan<T>(T left, T right, string messageFormat, params object[] messageArgs)
        {
            LessThan(left, right, null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an left value is less than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThan<T>(T left, T right, Comparison<T> comparer)
        {
            LessThan(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an left value is less than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThan<T>(T left, T right, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertOrder(left, right, comparer
                    , "Expected left to be less than right."
                    , compareResult => compareResult < 0
                    , messageFormat, messageArgs);
        }
        #endregion

        #region LessThanOrEqual

        /// <summary>
        /// Verifies that an left value is less or equal than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        public static void LessThanOrEqual<T>(T left, T right)
        {
            LessThanOrEqual(left, right, (string)null, null);
        }

        /// <summary>
        /// Verifies that an left value is less or equal than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThanOrEqual<T>(T left, T right, string messageFormat, params object[] messageArgs)
        {
            LessThanOrEqual(left, right, null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an left value is less or equal than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThanOrEqual<T>(T left, T right, Comparison<T> comparer)
        {
            LessThanOrEqual(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an left value is less or equal than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThanOrEqual<T>(T left, T right, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertOrder(left, right, comparer
                    , "Expected left to be less or equal than right."
                    , compareResult => compareResult <= 0
                    , messageFormat, messageArgs);
        }
        #endregion

        #region Between

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        public static void Between<T>(T actualValue, T minimum, T maximum)
        {
            Between(actualValue, minimum, maximum, (string)null, null);
        }

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Between<T>(T actualValue, T minimum, T maximum, string messageFormat, params object[] messageArgs)
        {
            Between(actualValue, minimum, maximum, null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Between<T>(T actualValue, T minimum, T maximum, Comparison<T> comparer)
        {
            Between(actualValue, minimum, maximum, comparer, null, null);
        }

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Between<T>(T actualValue, T minimum, T maximum, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (comparer == null)
                    comparer = DefaultCompare;

                if (comparer(actualValue, minimum) >= 0
                    && comparer(actualValue, maximum) <= 0)
                    return null;

                return new AssertionFailureBuilder("The actual value should be between the minimum and maximum values.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawActualValue(actualValue)
                    .SetRawLabeledValue("Minimum Value", minimum)
                    .SetRawLabeledValue("Maximum Value", maximum)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region NotBetween

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum)
        {
            NotBetween(actualValue, minimum, maximum, (string)null, null);
        }

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum, string messageFormat, params object[] messageArgs)
        {
            NotBetween(actualValue, minimum, maximum, null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum, Comparison<T> comparer)
        {
            NotBetween(actualValue, minimum, maximum, comparer, null, null);
        }

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (comparer == null)
                    comparer = DefaultCompare;

                if (comparer(actualValue, minimum) < 0
                    || comparer(actualValue, maximum) > 0)
                    return null;

                return new AssertionFailureBuilder("The actual value should not be between the minimum and maximum values.")
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawActualValue(actualValue)
                    .SetRawLabeledValue("Minimum Value", minimum)
                    .SetRawLabeledValue("Maximum Value", maximum)
                    .ToAssertionFailure();
            });
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

        #region Throws
        /// <summary>
        /// Evaluates an action delegate and verifies that it throws an exception of a particular type.
        /// </summary>
        /// <typeparam name="TExpectedException">The expected type of exception</typeparam>
        /// <param name="action">The action delegate to evaluate</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static TExpectedException Throws<TExpectedException>(Action action)
            where TExpectedException : Exception
        {
            return Throws<TExpectedException>(action, null, null);
        }

        /// <summary>
        /// Evaluates an action delegate and verifies that it throws an exception of a particular type.
        /// </summary>
        /// <typeparam name="TExpectedException">The expected type of exception</typeparam>
        /// <param name="action">The action delegate to evaluate</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static TExpectedException Throws<TExpectedException>(Action action, string messageFormat, params object[] messageArgs)
            where TExpectedException : Exception
        {
            return (TExpectedException)Throws(typeof(TExpectedException), action, messageFormat, messageArgs);
        }

        /// <summary>
        /// Evaluates an action delegate and verifies that it throws an exception of a particular type.
        /// </summary>
        /// <param name="expectedExceptionType">The expected exception type</param>
        /// <param name="action">The action delegate to evaluate</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static Exception Throws(Type expectedExceptionType, Action action)
        {
            return Throws(expectedExceptionType, action);
        }

        /// <summary>
        /// Evaluates an action delegate and verifies that it throws an exception of a particular type.
        /// </summary>
        /// <param name="expectedExceptionType">The expected exception type</param>
        /// <param name="action">The action delegate to evaluate</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static Exception Throws(Type expectedExceptionType, Action action, string messageFormat, params object[] messageArgs)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Exception result = null;
            AssertionHelper.Verify(delegate
            {
                try
                {
                    action();
                    return new AssertionFailureBuilder("Expected the block to throw an exception.")
                        .SetMessage(messageFormat, messageArgs)
                        .SetRawLabeledValue("Expected Exception Type", expectedExceptionType)
                        .ToAssertionFailure();
                }
                catch (Exception actualException)
                {
                    if (expectedExceptionType.IsInstanceOfType(actualException))
                    {
                        result = actualException;
                        return null;
                    }

                    return new AssertionFailureBuilder("The block threw an exception of a different type than was expected.")
                        .SetMessage(messageFormat, messageArgs)
                        .SetRawLabeledValue("Expected Exception Type", expectedExceptionType)
                        .AddException(actualException)
                        .ToAssertionFailure();
                }
            });

            return result;
        }
        #endregion

        #region DoesNotThrow
        /// <summary>
        /// Evaluates an action delegate and verifies that it does not throw an exception of any type.
        /// </summary>
        /// <param name="action">The action delegate to evaluate</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void DoesNotThrow(Action action)
        {
            DoesNotThrow(action, null, null);
        }


        /// <summary>
        /// Evaluates an action delegate and verifies that it does not throw an exception of any type.
        /// </summary>
        /// <param name="action">The action delegate to evaluate</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void DoesNotThrow(Action action, string messageFormat, params object[] messageArgs)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            AssertionHelper.Verify(delegate
            {
                try
                {
                    action();
                    return null;
                }
                catch (Exception actualException)
                {
                    return new AssertionFailureBuilder("The block threw an exception but none was expected.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddException(actualException)
                        .ToAssertionFailure();
                }
            });
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
                AssertionFailure[] failures = AssertionContext.CurrentContext.CaptureFailures(action,
                    AssertionFailureBehavior.Log, true);
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

        // TODO: Make this extensible.
        private static bool DefaultEqualityComparer<T>(T expectedValue, T actualValue)
        {
            if (Object.ReferenceEquals(expectedValue, actualValue))
                return true;

            if (expectedValue == null || actualValue == null)
                return false;

            var expectedEnumerable = expectedValue as IEnumerable;
            var actualEnumerable = actualValue as IEnumerable;
            if (expectedEnumerable != null && actualEnumerable != null)
                return CompareEnumerables(expectedEnumerable, actualEnumerable);

            return expectedValue.Equals(actualValue);
        }

        private static bool CompareEnumerables(IEnumerable expectedEnumerable, IEnumerable actualEnumerable)
        {
            IEnumerator expectedEnumerator = expectedEnumerable.GetEnumerator();
            IEnumerator actualEnumerator = actualEnumerable.GetEnumerator();
            while (expectedEnumerator.MoveNext())
            {
                if (!actualEnumerator.MoveNext())
                    return false;

                if (!DefaultEqualityComparer(expectedEnumerator.Current, actualEnumerator.Current))
                    return false;
            }

            if (actualEnumerator.MoveNext())
                return false;

            return true;
        }

        private static int DefaultCompare<T>(T left, T right)
        {
            IComparable<T> leftGenericComparable = left as IComparable<T>;
            if (leftGenericComparable != null)
                return leftGenericComparable.CompareTo(right);

            IComparable leftComparable = left as IComparable;
            if (leftComparable != null)
                return leftComparable.CompareTo(right);

            if (right is IComparable<T> || right is IComparable)
                return -1;

            if (Object.Equals(left, default(T)) && Object.Equals(right, default(T)))
                return 0;
            throw new InvalidOperationException(
                String.Format("No ordering comparison defined on type {0}."
                    , typeof(T)));
        }

        private static void AssertOrder<T>(T left, T right, Comparison<T> comparer, string exceptionMessage, Func<int, bool> isCompareSuccesfull, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(() =>
            {
                if (comparer == null)
                    comparer = DefaultCompare;

                if (isCompareSuccesfull(comparer(left, right)))
                    return null;

                return new AssertionFailureBuilder(exceptionMessage)
                    .SetMessage(messageFormat, messageArgs)
                    .SetRawLabeledValue("Left Value", left)
                    .SetRawLabeledValue("Right Value", right)
                    .ToAssertionFailure();
            });
        }
    }
}
