// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Globalization;
using System.Text;
using Gallio;
using Gallio.Framework;
using Gallio.Logging;

namespace MbUnit.Framework
{
    /// <summary>
    /// This is an interim assertion class intended to be used within
    /// MbUnit v2 tests.  We'll refactor these assertions when we move
    /// to Gallio.
    /// </summary>
    /// <remarks>
    /// DO NOT USE THIS AS THE MASTER PATTERN FOR GALLIO ASSERTIONS!
    /// The real asserts will have much more diagnostic output and will
    /// be integrated more tightly with framework services for
    /// formatting and logging.
    /// </remarks>
    public static class InterimAssert
    {
        /// <summary>
        /// Asserts that the specified block of code does not throw an exception.
        /// </summary>
        /// <param name="block">The block of code to run</param>
        public static void DoesNotThrow(Block block)
        {
            DoesNotThrow(block, "");
        }

        /// <summary>
        /// Asserts that the specified block of code does not throw an exception.
        /// </summary>
        /// <param name="block">The block of code to run</param>
        public static void DoesNotThrow(Block block, string message)
        {
            try
            {
                block();
            }
            catch (Exception ex)
            {
                Assert.Fail("The block threw an exception: " + ex + "\n" + message);
            }
        }

        /// <summary>
        /// Asserts that the specified block of code does not throw an exception.
        /// </summary>
        /// <param name="block">The block of code to run</param>
        public static void DoesNotThrow(Block block, string messageFormat, params object[] messageArgs)
        {
            DoesNotThrow(block, String.Format(messageFormat, messageArgs));
        }

        public static void Throws<T>(Block block)
            where T : Exception
        {
            Throws(typeof(T), block);
        }

        public static void Throws(Type exceptionType, Block block)
        {
            try
            {
                block();
            }
            catch (Exception ex)
            {
                if (ex is ClientException && ex.InnerException != null)
                    ex = ex.InnerException;

                if (exceptionType.IsInstanceOfType(ex))
                    return;

                Assert.Fail("Expected the block to throw an exception of type '{0}' but it actually threw:\n{1}", exceptionType.FullName, ex);
            }

            Assert.Fail("Expected the block to throw an exception of type '{0}'.", exceptionType.FullName);
        }

        public static void AreElementsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual,
            Relation<T> equivalenceRelation)
        {
            WithPairs(expected, actual, delegate(T expectedValue, T actualValue)
            {
                if (!equivalenceRelation(expectedValue, actualValue))
                    throw new AssertionException("Elements differ.");
            });
        }

        public static void AreElementsEqual<TKey, TValue>(IDictionary<TKey, TValue> expected,
            IDictionary<TKey, TValue> actual, Relation<TValue> equivalenceRelation)
        {
            WithKeyedPairs(expected, actual, delegate(TKey key, TValue expectedValue, TValue actualValue)
            {
                if (!equivalenceRelation(expectedValue, actualValue))
                    throw new AssertionException("Elements differ.");
            });
        }

        /// <summary>
        /// Evaluates an assertion with matched pairs drawn from each collection.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="expectedValues">The enumeration of expected values</param>
        /// <param name="actualValues">The enumeration of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs</param>
        public static void WithPairs<T>(IEnumerable<T> expectedValues, IEnumerable<T> actualValues,
            PairwiseAssertion<T> assertion)
        {
            if (expectedValues == null)
            {
                if (actualValues != null)
                    throw new AssertionException();
                return;
            }

            int index = 0;
            IEnumerator<T> expectedEnumerator = expectedValues.GetEnumerator();
            IEnumerator<T> actualEnumerator = actualValues.GetEnumerator();
            while (expectedEnumerator.MoveNext())
            {
                if (! actualEnumerator.MoveNext())
                    throw new AssertionException("Actual collection has fewer elements than expected collection.");

                try
                {
                    assertion(expectedEnumerator.Current, actualEnumerator.Current);
                }
                catch (Exception ex)
                {
                    throw new AssertionException("Failure occurred at index: " + index, ex);
                }

                index += 1;
            }

            if (actualEnumerator.MoveNext())
                throw new AssertionException("Actual collection has more elements than expected collection.");
        }

        /// <summary>
        /// Evaluates an assertion with matched pairs drawn from each dictionary
        /// that have identical keys.  Fails if the collections have different sizes
        /// or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="expectedValues">The enumeration of expected values</param>
        /// <param name="actualValues">The enumeration of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs</param>
        public static void WithKeyedPairs<TKey, TValue>(IDictionary<TKey, TValue> expectedValues,
            IDictionary<TKey, TValue> actualValues, KeyedPairwiseAssertion<TKey, TValue> assertion)
        {
            if (expectedValues == null)
            {
                if (actualValues != null)
                    throw new AssertionException();
                return;
            }

            if (expectedValues.Count != actualValues.Count)
                throw new AssertionException(String.Format(CultureInfo.CurrentCulture,
                    "Expected collection has {0} values but actual collection has {1} values.",
                    expectedValues.Count, actualValues.Count));

            foreach (KeyValuePair<TKey, TValue> expectedPair in expectedValues)
            {
                TKey key = expectedPair.Key;
                TValue actualValue;
                if (!actualValues.TryGetValue(key, out actualValue))
                    throw new AssertionException("Actual collection missing value for key: " + key);

                try
                {
                    assertion(key, expectedPair.Value, actualValue);
                }
                catch (Exception ex)
                {
                    throw new AssertionException("Failure occurred with key: " + key, ex);
                }
            }
        }

        /// <summary>
        /// Evaluates an assertion with matched pairs drawn from each dictionary
        /// that have identical keys.  Fails if the collections have different sizes
        /// or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="expectedValues">The enumeration of expected values</param>
        /// <param name="actualValues">The enumeration of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs</param>
        public static void WithKeyedPairs<TKey, TValue>(IDictionary<TKey, TValue> expectedValues,
            IDictionary<TKey, TValue> actualValues, PairwiseAssertion<TValue> assertion)
        {
            WithKeyedPairs(expectedValues, actualValues,
                delegate(TKey key, TValue expectedValue, TValue actualValue)
                {
                    assertion(expectedValue, actualValue);
                });
        }

        /// <summary>
        /// Asserts that all of the values in the objects array are distinct by
        /// equality and hashcode.
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="items">The objects</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="items"/> is null</exception>
        public static void AreDistinct<T>(params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            for (int i = 0; i < items.Length; i++)
            {
                for (int j = i + 1; j < items.Length; j++)
                {
                    Assert.AreNotEqual(items[i], items[j], "Item {0} should not equal item {1}.", i, j);
                    Assert.AreNotEqual(items[j], items[i], "Item {0} should not equal item {1}.", j, i);

                    if (items[i] != null && items[j] != null)
                        Assert.AreNotEqual(items[i].GetHashCode(), items[j].GetHashCode(), "Objects {0} and {1} should not have the same hashcode.", i, j);
                }
            }
        }

        public static void Inconclusive(string messageFormat, params object[] messageArgs)
        {
            throw new TestInconclusiveException(String.Format(messageFormat, messageArgs));
        }
    }
}
