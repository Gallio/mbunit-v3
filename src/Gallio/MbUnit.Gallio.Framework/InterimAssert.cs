using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MbUnit.Framework.Exceptions;

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
    }
}
