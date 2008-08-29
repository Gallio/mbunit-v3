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
using Gallio;
using Gallio.Framework.Assertions;
using Gallio.Model.Diagnostics;

namespace MbUnit.Framework
{
    /// <summary>
    /// Defines methods used with the <see cref="Assert.Over" /> syntax for mapping
    /// assertions over complex data structures.
    /// </summary>
    [TestFrameworkInternal]
    public sealed class AssertOverSyntax
    {
        internal static readonly AssertOverSyntax Instance = new AssertOverSyntax();

        private AssertOverSyntax()
        {
        }

        /// <summary>
        /// Evaluates an assertion over matched pairs of expected and actual values
        /// taken from each sequence.
        /// </summary>
        /// <typeparam name="TExpected">The expected value type</typeparam>
        /// <typeparam name="TActual">The actual value type</typeparam>
        /// <param name="expectedSequence">The sequence of expected values, or null</param>
        /// <param name="actualSequence">The sequence of actual values, or null</param>
        /// <param name="assertion">The assertion to evaluate given an expected value
        /// and an actual value</param>
        public void Pairs<TExpected, TActual>(IEnumerable<TExpected> expectedSequence,
            IEnumerable<TActual> actualSequence, Action<TExpected, TActual> assertion)
        {
            Pairs(expectedSequence, actualSequence, assertion, null, null);
        }

        /// <summary>
        /// Evaluates an assertion over matched pairs of expected and actual values
        /// taken from each sequence.
        /// </summary>
        /// <typeparam name="TExpected">The expected value type</typeparam>
        /// <typeparam name="TActual">The actual value type</typeparam>
        /// <param name="expectedSequence">The sequence of expected values, or null</param>
        /// <param name="actualSequence">The sequence of actual values, or null</param>
        /// <param name="assertion">The assertion to evaluate given an expected value
        /// and an actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        public void Pairs<TExpected, TActual>(IEnumerable<TExpected> expectedSequence,
            IEnumerable<TActual> actualSequence, Action<TExpected, TActual> assertion,
            string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (expectedSequence == null)
                {
                    if (actualSequence == null)
                        return null;

                    return new AssertionFailureBuilder("Expected the expected and actual sequences to either both be null or both be non-null.")
                        .SetRawExpectedValue(expectedSequence)
                        .SetRawActualValue(actualSequence)
                        .SetMessage(messageFormat, messageFormat)
                        .ToAssertionFailure();
                }

                int index = 0;
                IEnumerator<TExpected> expectedEnumerator = expectedSequence.GetEnumerator();
                IEnumerator<TActual> actualEnumerator = actualSequence.GetEnumerator();
                while (expectedEnumerator.MoveNext())
                {
                    if (!actualEnumerator.MoveNext())
                    {
                        return new AssertionFailureBuilder(String.Format("The expected value sequence has {0} elements but the actual value sequence has {1}.",
                            1 + index + CountRemainingElements(expectedEnumerator), index))
                            .SetMessage(messageFormat, messageArgs)
                            .SetRawExpectedAndActualValueWithDiffs(expectedSequence, actualSequence)
                            .ToAssertionFailure();
                    }

                    AssertionFailure[] failures = AssertionHelper.Eval(delegate
                    {
                        assertion(expectedEnumerator.Current, actualEnumerator.Current);
                    });

                    if (failures.Length != 0)
                    {
                        return new AssertionFailureBuilder(String.Format("Assertion failed at index {0}.", index))
                            .SetMessage(messageFormat, messageArgs)
                            .SetRawExpectedAndActualValueWithDiffs(expectedSequence, actualSequence)
                            .ToAssertionFailure();
                    }

                    index += 1;
                }

                if (actualEnumerator.MoveNext())
                {
                    return new AssertionFailureBuilder(String.Format("The expected value sequence has {0} elements but the actual value sequence has {1}.",
                        index, index + CountRemainingElements(actualEnumerator) + 1))
                        .SetMessage(messageFormat, messageArgs)
                        .SetRawExpectedAndActualValueWithDiffs(expectedSequence, actualSequence)
                        .ToAssertionFailure();
                }

                return null;
            });
        }

        /// <summary>
        /// Evaluates an assertion with matched pairs with identical keys drawn from a
        /// dictionary of expected and actual values that have identical keys.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TExpectedValue">The expected value type</typeparam>
        /// <typeparam name="TActualValue">The expected value type</typeparam>
        /// <param name="expectedDictionary">The dictionary of expected values</param>
        /// <param name="actualDictionary">The dictionary of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs, with the expected value as first
        /// argument, and actual value as second</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public void KeyedPairs<TKey, TExpectedValue, TActualValue>(IDictionary<TKey, TExpectedValue> expectedDictionary,
            IDictionary<TKey, TActualValue> actualDictionary, Action<TExpectedValue, TActualValue> assertion)
        {
            KeyedPairs(expectedDictionary, actualDictionary, (key, expectedValue, actualValue) => assertion(expectedValue, actualValue));
        }

        /// <summary>
        /// Evaluates an assertion with matched pairs with identical keys drawn from a
        /// dictionary of expected and actual values that have identical keys.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TExpectedValue">The expected value type</typeparam>
        /// <typeparam name="TActualValue">The expected value type</typeparam>
        /// <param name="expectedDictionary">The dictionary of expected values</param>
        /// <param name="actualDictionary">The dictionary of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs, with the key as first argument,
        /// expected value as second, and actual value as third</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public void KeyedPairs<TKey, TExpectedValue, TActualValue>(IDictionary<TKey, TExpectedValue> expectedDictionary,
            IDictionary<TKey, TActualValue> actualDictionary, Action<TKey, TExpectedValue, TActualValue> assertion)
        {
            KeyedPairs(expectedDictionary, actualDictionary, assertion, null, null);
        }

        /// <summary>
        /// Evaluates an assertion with matched pairs with identical keys drawn from a
        /// dictionary of expected and actual values that have identical keys.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TExpectedValue">The expected value type</typeparam>
        /// <typeparam name="TActualValue">The expected value type</typeparam>
        /// <param name="expectedDictionary">The dictionary of expected values</param>
        /// <param name="actualDictionary">The dictionary of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs, with the key as first argument,
        /// expected value as second, and actual value as third</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public void KeyedPairs<TKey, TExpectedValue, TActualValue>(IDictionary<TKey, TExpectedValue> expectedDictionary,
            IDictionary<TKey, TActualValue> actualDictionary, Action<TExpectedValue, TActualValue> assertion,
            string messageFormat, params object[] messageArgs)
        {
            KeyedPairs(expectedDictionary, actualDictionary, (key, expectedValue, actualValue) => assertion(expectedValue, actualValue),
                messageFormat, messageArgs);
        }

        /// <summary>
        /// Evaluates an assertion with matched pairs with identical keys drawn from a
        /// dictionary of expected and actual values that have identical keys.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TExpectedValue">The expected value type</typeparam>
        /// <typeparam name="TActualValue">The expected value type</typeparam>
        /// <param name="expectedDictionary">The dictionary of expected values</param>
        /// <param name="actualDictionary">The dictionary of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs, with the key as first argument,
        /// expected value as second, and actual value as third</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public void KeyedPairs<TKey, TExpectedValue, TActualValue>(IDictionary<TKey, TExpectedValue> expectedDictionary,
            IDictionary<TKey, TActualValue> actualDictionary, Action<TKey, TExpectedValue, TActualValue> assertion,
            string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (expectedDictionary == null)
                {
                    if (actualDictionary == null)
                        return null;

                    return new AssertionFailureBuilder("Expected the expected and actual dictionaries to either both be null or both be non-null.")
                        .SetRawLabeledValue("Expected Dictionary", expectedDictionary)
                        .SetRawLabeledValue("Actual Dictionary", actualDictionary)
                        .SetMessage(messageFormat, messageFormat)
                        .ToAssertionFailure();
                }

                if (expectedDictionary.Count != actualDictionary.Count)
                    return new AssertionFailureBuilder(String.Format("The expected and actual dictionaries have a different number of items."))
                        .SetRawLabeledValue("Expected Dictionary Count", expectedDictionary.Count)
                        .SetRawLabeledValue("Actual Dictionary Count", actualDictionary.Count)
                        .SetRawLabeledValue("Expected Dictionary", expectedDictionary)
                        .SetRawLabeledValue("Actual Dictionary", actualDictionary)
                        .SetMessage(messageFormat, messageArgs)
                        .ToAssertionFailure();

                foreach (KeyValuePair<TKey, TExpectedValue> expectedPair in expectedDictionary)
                {
                    TKey key = expectedPair.Key;
                    TActualValue actualValue;
                    if (!actualDictionary.TryGetValue(key, out actualValue))
                        return new AssertionFailureBuilder("Actual collection does not contain a value for a particular key.")
                            .SetRawLabeledValue("Missing Key", key)
                            .SetRawLabeledValue("Expected Dictionary", expectedDictionary)
                            .SetRawLabeledValue("Actual Dictionary", actualDictionary)
                            .SetMessage(messageFormat, messageFormat)
                            .ToAssertionFailure();

                    AssertionFailure[] failures = AssertionHelper.Eval(delegate
                    {
                        assertion(key, expectedPair.Value, actualValue);
                    });

                    if (failures.Length != 0)
                    {
                        return new AssertionFailureBuilder(String.Format("Assertion failed on a particular key."))
                            .SetRawLabeledValue("Failed Key", key)
                            .SetRawLabeledValue("Expected Dictionary", expectedDictionary)
                            .SetRawLabeledValue("Actual Dictionary", actualDictionary)
                            .SetMessage(messageFormat, messageArgs)
                            .ToAssertionFailure();
                    }
                }

                return null;
            });
        }

        private static int CountRemainingElements(IEnumerator enumerator)
        {
            int count = 0;
            while (enumerator.MoveNext())
                count++;
            return count;
        }
    }
}
