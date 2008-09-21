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
using System.Collections.Generic;
using Gallio;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using System.Collections;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region AreElementsEqual
        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedSequence">The expected sequence</param>
        /// <param name="actualSequence">The actual sequence</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence)
        {
            AreElementsEqual(expectedSequence, actualSequence, null, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedSequence">The expected sequence</param>
        /// <param name="actualSequence">The actual sequence</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, string messageFormat, params object[] messageArgs)
        {
            AreElementsEqual(expectedSequence, actualSequence, null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedSequence">The expected sequence</param>
        /// <param name="actualSequence">The actual sequence</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, Func<T, T, bool> comparer)
        {
            AreElementsEqual(expectedSequence, actualSequence, comparer, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedSequence">The expected sequence</param>
        /// <param name="actualSequence">The actual sequence</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, Func<T, T, bool> comparer, string messageFormat, params object[] messageArgs)
        {
            Over.Pairs(expectedSequence, actualSequence, (expected, actual) => AreEqual(expected, actual, comparer), messageFormat, messageArgs);
        }
        #endregion

        #region AreElementsNotEqual
        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements but that
        /// the elements are not equal.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedSequence">The expected sequence</param>
        /// <param name="actualSequence">The actual sequence</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence)
        {
            AreElementsNotEqual(expectedSequence, actualSequence, null, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements but that
        /// the elements are not equal.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedSequence">The expected sequence</param>
        /// <param name="actualSequence">The actual sequence</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, string messageFormat, params object[] messageArgs)
        {
            AreElementsNotEqual(expectedSequence, actualSequence, null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements but that
        /// the elements are not equal.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedSequence">The expected sequence</param>
        /// <param name="actualSequence">The actual sequence</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, Func<T, T, bool> comparer)
        {
            AreElementsNotEqual(expectedSequence, actualSequence, comparer, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements but that
        /// the elements are not equal.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="expectedSequence">The expected sequence</param>
        /// <param name="actualSequence">The actual sequence</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, Func<T, T, bool> comparer, string messageFormat, params object[] messageArgs)
        {
            Over.Pairs(expectedSequence, actualSequence, (expected, actual) => AreNotEqual(expected, actual, comparer), messageFormat, messageArgs);
        }
        #endregion

        #region Contains
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
                    if (ComparisonSemantics.Equals(expectedValue, item))
                        return null;

                return new AssertionFailureBuilder("Expected the value to appear within the enumeration.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawExpectedValue(expectedValue)
                    .AddRawLabeledValue("Enumeration", enumeration)
                    .ToAssertionFailure();
            });
        }

        /// <summary>
        /// Asserts that <paramref name="expectedKey"/> is in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The dictionary of items</param>
        /// <param name="expectedKey">The key expected to be found in the dictionary</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey expectedKey)
        {
            ContainsKey(dictionary, expectedKey, null);
        }

        /// <summary>
        /// Asserts that <paramref name="expectedKey"/> is in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The dictionary of items</param>
        /// <param name="expectedKey">The key expected to be found in the dictionary</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey expectedKey, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (dictionary.ContainsKey(expectedKey))
                    return null;

                return new AssertionFailureBuilder("Expected the key to appear within the dictionary.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Expected Key", expectedKey)
                    .AddRawLabeledValue("Dictionary", dictionary)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region DoesNotContain
        /// <summary>
        /// Asserts that <paramref name="unexpectedValue"/> is not in the enumeration <paramref name="enumeration"/>.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="enumeration">The enumeration of items</param>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void DoesNotContain<T>(IEnumerable<T> enumeration, T unexpectedValue)
        {
            DoesNotContain(enumeration, unexpectedValue, null);
        }

        /// <summary>
        /// Asserts that <paramref name="unexpectedValue"/> is not in the enumeration <paramref name="enumeration"/>.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="enumeration">The enumeration of items</param>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void DoesNotContain<T>(IEnumerable<T> enumeration, T unexpectedValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                foreach (T item in enumeration)
                {
                    if (ComparisonSemantics.Equals(unexpectedValue, item))
                        return new AssertionFailureBuilder("Expected the value to not appear within the enumeration.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddRawLabeledValue("Unexpected Value", unexpectedValue)
                            .AddRawLabeledValue("Enumeration", enumeration)
                            .ToAssertionFailure();
                }

                return null;
            });
        }

        /// <summary>
        /// Asserts that <paramref name="unexpectedKey"/> is not in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The dictionary of items</param>
        /// <param name="unexpectedKey">The key that should not be found in the dictionary</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void DoesNotContainKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey unexpectedKey)
        {
            DoesNotContainKey(dictionary, unexpectedKey, null);
        }

        /// <summary>
        /// Asserts that <paramref name="unexpectedKey"/> is not in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The dictionary of items</param>
        /// <param name="unexpectedKey">The key expected to be found in the dictionary</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        static public void DoesNotContainKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey unexpectedKey, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (dictionary.ContainsKey(unexpectedKey))
                    return new AssertionFailureBuilder("Expected the key to not appear within the dictionary.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Unexpected Key", unexpectedKey)
                        .AddRawLabeledValue("Dictionary", dictionary)
                        .ToAssertionFailure();

                return null;
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
                if (!actualValue.GetEnumerator().MoveNext())
                    return null;

                return new AssertionFailureBuilder("Expected value to be empty.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
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
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion
    }
}
