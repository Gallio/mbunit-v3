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
        /// Evaluates an assertion over pairs of values taken from two sequences.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TLeftValue">The left value type</typeparam>
        /// <typeparam name="TRightValue">The right value type</typeparam>
        /// <param name="leftSequence">The left sequence, or null</param>
        /// <param name="rightSequence">The right sequence, or null</param>
        /// <param name="assertion">The assertion to evaluate given a left value and a right value</param>
        public void Pairs<TLeftValue, TRightValue>(IEnumerable<TLeftValue> leftSequence,
            IEnumerable<TRightValue> rightSequence, Action<TLeftValue, TRightValue> assertion)
        {
            Pairs(leftSequence, rightSequence, assertion, null, null);
        }

        /// <summary>
        /// Evaluates an assertion over pairs of values taken from two sequence.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TLeftValue">The left value type</typeparam>
        /// <typeparam name="TRightValue">The right value type</typeparam>
        /// <param name="leftSequence">The left sequence, or null</param>
        /// <param name="rightSequence">The right sequence, or null</param>
        /// <param name="assertion">The assertion to evaluate given a left value and a right value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        public void Pairs<TLeftValue, TRightValue>(IEnumerable<TLeftValue> leftSequence,
            IEnumerable<TRightValue> rightSequence, Action<TLeftValue, TRightValue> assertion,
            string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (leftSequence == null && rightSequence == null)
                    return null;

                if (leftSequence == null || rightSequence == null)
                {
                    return new AssertionFailureBuilder(
                        "Expected the left and right sequences to either both be null or both be non-null.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Left Sequence", leftSequence)
                        .AddRawLabeledValue("Right Sequence", rightSequence)
                        .ToAssertionFailure();
                }

                int index = 0;
                IEnumerator<TLeftValue> leftEnumerator = leftSequence.GetEnumerator();
                IEnumerator<TRightValue> rightEnumerator = rightSequence.GetEnumerator();
                while (leftEnumerator.MoveNext())
                {
                    if (!rightEnumerator.MoveNext())
                    {
                        return new AssertionFailureBuilder("Expected the left and right sequences to have the same number of elements.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddRawLabeledValue("Left Sequence Count", 1 + index + Assert.CountRemainingElements(leftEnumerator))
                            .AddRawLabeledValue("Right Sequence Count", index)
                            .AddRawLabeledValue("Left Sequence", leftSequence)
                            .AddRawLabeledValue("Right Sequence", rightSequence)
                            .ToAssertionFailure();
                    }

                    AssertionFailure[] failures = AssertionHelper.Eval(delegate
                    {
                        assertion(leftEnumerator.Current, rightEnumerator.Current);
                    });

                    if (failures.Length != 0)
                    {
                        return new AssertionFailureBuilder("Assertion failed on two values at a particular index within both sequences.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddLabeledValue("Index", index.ToString())
                            .AddRawLabeledValue("Left Sequence", leftSequence)
                            .AddRawLabeledValue("Right Sequence", rightSequence)
                            .AddInnerFailures(failures)
                            .ToAssertionFailure();
                    }

                    index += 1;
                }

                if (rightEnumerator.MoveNext())
                {
                    return new AssertionFailureBuilder("Expected the left and right sequences to have the same number of elements.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Left Sequence Count", index)
                        .AddRawLabeledValue("Right Sequence Count", index + Assert.CountRemainingElements(rightEnumerator) + 1)
                        .AddRawLabeledValue("Left Sequence", leftSequence)
                        .AddRawLabeledValue("Right Sequence", rightSequence)
                        .ToAssertionFailure();
                }

                return null;
            });
        }

        /// <summary>
        /// Evaluates an assertion over key/value pairs with identical keys drawn from two dictionaries.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TLeftValue">The expected value type</typeparam>
        /// <typeparam name="TRightValue">The expected value type</typeparam>
        /// <param name="leftDictionary">The left dictionary, or null</param>
        /// <param name="rightDictionary">The right dictionary, or null</param>
        /// <param name="assertion">The assertion to evaluate over all pairs, with the left value as first
        /// argument, and right value as second</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public void KeyedPairs<TKey, TLeftValue, TRightValue>(IDictionary<TKey, TLeftValue> leftDictionary,
            IDictionary<TKey, TRightValue> rightDictionary, Action<TLeftValue, TRightValue> assertion)
        {
            KeyedPairs(leftDictionary, rightDictionary, (key, expectedValue, actualValue) => assertion(expectedValue, actualValue));
        }

        /// <summary>
        /// Evaluates an assertion over key/value pairs with identical keys drawn from two dictionaries.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TLeftValue">The expected value type</typeparam>
        /// <typeparam name="TRightValue">The expected value type</typeparam>
        /// <param name="leftDictionary">The left dictionary, or null</param>
        /// <param name="rightDictionary">The right dictionary, or null</param>
        /// <param name="assertion">The assertion to evaluate over all pairs, with the key as first
        /// argument, left value as second, and right value as third</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public void KeyedPairs<TKey, TLeftValue, TRightValue>(IDictionary<TKey, TLeftValue> leftDictionary,
            IDictionary<TKey, TRightValue> rightDictionary, Action<TKey, TLeftValue, TRightValue> assertion)
        {
            KeyedPairs(leftDictionary, rightDictionary, assertion, null, null);
        }

        /// <summary>
        /// Evaluates an assertion over key/value pairs with identical keys drawn from two dictionaries.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TLeftValue">The expected value type</typeparam>
        /// <typeparam name="TRightValue">The expected value type</typeparam>
        /// <param name="leftDictionary">The left dictionary, or null</param>
        /// <param name="rightDictionary">The right dictionary, or null</param>
        /// <param name="assertion">The assertion to evaluate over all pairs, with the left value as first
        /// argument, and right value as second</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public void KeyedPairs<TKey, TLeftValue, TRightValue>(IDictionary<TKey, TLeftValue> leftDictionary,
            IDictionary<TKey, TRightValue> rightDictionary, Action<TLeftValue, TRightValue> assertion,
            string messageFormat, params object[] messageArgs)
        {
            KeyedPairs(leftDictionary, rightDictionary, (key, expectedValue, actualValue) => assertion(expectedValue, actualValue),
                messageFormat, messageArgs);
        }

        /// <summary>
        /// Evaluates an assertion over key/value pairs with identical keys drawn from two dictionaries.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TLeftValue">The expected value type</typeparam>
        /// <typeparam name="TRightValue">The expected value type</typeparam>
        /// <param name="leftDictionary">The left dictionary, or null</param>
        /// <param name="rightDictionary">The right dictionary, or null</param>
        /// <param name="assertion">The assertion to evaluate over all pairs, with the key as first
        /// argument, left value as second, and right value as third</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public void KeyedPairs<TKey, TLeftValue, TRightValue>(IDictionary<TKey, TLeftValue> leftDictionary,
            IDictionary<TKey, TRightValue> rightDictionary, Action<TKey, TLeftValue, TRightValue> assertion,
            string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (leftDictionary == null && rightDictionary == null)
                    return null;

                if (leftDictionary == null || rightDictionary == null)
                {
                    return new AssertionFailureBuilder("Expected the left and right dictionaries to either both be null or both be non-null.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Left Dictionary", leftDictionary)
                        .AddRawLabeledValue("Right Dictionary", rightDictionary)
                        .ToAssertionFailure();
                }

                if (leftDictionary.Count != rightDictionary.Count)
                    return new AssertionFailureBuilder("Expected the left and right dictionaries to have the same number of items.")
                        .AddRawLabeledValue("Left Dictionary Count", leftDictionary.Count)
                        .AddRawLabeledValue("Right Dictionary Count", rightDictionary.Count)
                        .AddRawLabeledValue("Left Dictionary", leftDictionary)
                        .AddRawLabeledValue("Right Dictionary", rightDictionary)
                        .SetMessage(messageFormat, messageArgs)
                        .ToAssertionFailure();

                foreach (KeyValuePair<TKey, TLeftValue> leftPair in leftDictionary)
                {
                    TKey key = leftPair.Key;
                    TRightValue rightValue;
                    if (!rightDictionary.TryGetValue(key, out rightValue))
                        return new AssertionFailureBuilder("Right dictionary does not contain a value for a particular key that is in the left dictionary.")
                            .AddRawLabeledValue("Missing Key", key)
                            .AddRawLabeledValue("Left Dictionary", leftDictionary)
                            .AddRawLabeledValue("Right Dictionary", rightDictionary)
                            .SetMessage(messageFormat, messageFormat)
                            .ToAssertionFailure();

                    AssertionFailure[] failures = AssertionHelper.Eval(delegate
                    {
                        assertion(key, leftPair.Value, rightValue);
                    });

                    if (failures.Length != 0)
                    {
                        return new AssertionFailureBuilder("Assertion failed on two pairs with a particular key in both dictionaries.")
                            .AddRawLabeledValue("Key", key)
                            .AddRawLabeledValue("Left Dictionary", leftDictionary)
                            .AddRawLabeledValue("Right Dictionary", rightDictionary)
                            .SetMessage(messageFormat, messageArgs)
                            .AddInnerFailures(failures)
                            .ToAssertionFailure();
                    }
                }

                return null;
            });
        }
    }
}
