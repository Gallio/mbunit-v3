// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using System.Text;
using Gallio;
using Gallio.Framework;

#pragma warning disable 1591

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
        /// <param name="action">The block of code to run</param>
        public static void DoesNotThrow(Action action)
        {
            DoesNotThrow(action, "");
        }

        /// <summary>
        /// Asserts that the specified block of code does not throw an exception.
        /// </summary>
        /// <param name="action">The block of code to run</param>
        /// <param name="message">The failure message</param>
        public static void DoesNotThrow(Action action, string message)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.Fail("The block threw an exception: " + ex + "\n" + message);
            }
        }

        /// <summary>
        /// Asserts that the specified block of code does not throw an exception.
        /// </summary>
        /// <param name="action">The block of code to run</param>
        /// <param name="messageFormat">The failure message format string</param>
        /// <param name="messageArgs">The failure message arguments</param>
        public static void DoesNotThrow(Action action, string messageFormat, params object[] messageArgs)
        {
            DoesNotThrow(action, String.Format(messageFormat, messageArgs));
        }

        public static void Throws<T>(Action action)
            where T : Exception
        {
            Throws(typeof(T), action);
        }

        public static void Throws(Type exceptionType, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (exceptionType.IsInstanceOfType(ex)
                    || (ex is TargetInvocationException && ex.InnerException != null
                        && exceptionType.IsInstanceOfType(ex.InnerException)))
                    return;

                Assert.Fail("Expected the block to throw an exception of type '{0}' but it actually threw:\n{1}", exceptionType, ex);
            }

            Assert.Fail("Expected the block to throw an exception of type '{0}'.", exceptionType);
        }

        public static void AreElementsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual,
            Func<T, T, bool> equivalenceRelation)
        {
            WithPairs(expected, actual, delegate(T expectedValue, T actualValue)
            {
                if (!equivalenceRelation(expectedValue, actualValue))
                    throw new AssertionException("Elements differ.");
            });
        }

        public static void AreElementsEqual<TKey, TValue>(IDictionary<TKey, TValue> expected,
            IDictionary<TKey, TValue> actual, Func<TValue, TValue, bool> equivalenceRelation)
        {
            WithKeyedPairs(expected, actual, delegate(TKey key, TValue expectedValue, TValue actualValue)
            {
                if (!equivalenceRelation(expectedValue, actualValue))
                    throw new AssertionException("Elements differ.");
            });
        }

        public static void AreElementsEqualIgnoringOrder<TValue>(IEnumerable<TValue> expected, IEnumerable<TValue> actual,
            Func<TValue, TValue, bool> equivalenceRelation)
        {
            LinkedList<TValue> expectedElements = new LinkedList<TValue>(expected);
            LinkedList<TValue> actualElements = new LinkedList<TValue>(actual);

            for (LinkedListNode<TValue> expectedNode = expectedElements.First; expectedNode != null; )
            {
                LinkedListNode<TValue> nextExpectedNode = expectedNode.Next;

                for (LinkedListNode<TValue> actualNode = actualElements.First; actualNode != null; actualNode = actualNode.Next)
                {
                    if (equivalenceRelation(expectedNode.Value, actualNode.Value))
                    {
                        expectedElements.Remove(expectedNode);
                        actualElements.Remove(actualNode);
                        break;
                    }
                }

                expectedNode = nextExpectedNode;
            }

            StringBuilder builder = new StringBuilder();
            if (expectedElements.Count != 0)
            {
                builder.AppendFormat("The following {0} expected element(s) were not found:\n", expectedElements.Count);

                foreach (TValue value in expectedElements)
                    builder.Append("[[").Append(value).AppendLine("]]");
            }

            if (actualElements.Count != 0)
            {
                if (builder.Length != 0)
                    builder.AppendLine();

                builder.AppendFormat("The following {0} actual element(s) were not expected:\n", actualElements.Count);

                foreach (TValue value in actualElements)
                    builder.Append("[[").Append(value).AppendLine("]]");
            }

            if (builder.Length != 0)
                Assert.Fail(builder.ToString());
        }

        /// <summary>
        /// Evaluates an assertion with matched pairs drawn from each collection.
        /// Fails if the collections have different sizes or if one is null but not the other.
        /// </summary>
        /// <typeparam name="TExpected">The expected value type</typeparam>
        /// <typeparam name="TActual">The actual value type</typeparam>
        /// <param name="expectedValues">The enumeration of expected values</param>
        /// <param name="actualValues">The enumeration of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs</param>
        public static void WithPairs<TExpected, TActual>(IEnumerable<TExpected> expectedValues, IEnumerable<TActual> actualValues,
            Action<TExpected, TActual> assertion)
        {
            if (expectedValues == null)
            {
                if (actualValues != null)
                    throw new AssertionException();
                return;
            }

            int index = 0;
            IEnumerator<TExpected> expectedEnumerator = expectedValues.GetEnumerator();
            IEnumerator<TActual> actualEnumerator = actualValues.GetEnumerator();
            while (expectedEnumerator.MoveNext())
            {
                if (!actualEnumerator.MoveNext())
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
        /// <typeparam name="TExpectedValue">The expected value type</typeparam>
        /// <typeparam name="TActualValue">The expected value type</typeparam>
        /// <param name="expectedValues">The enumeration of expected values</param>
        /// <param name="actualValues">The enumeration of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs</param>
        public static void WithKeyedPairs<TKey, TExpectedValue, TActualValue>(IDictionary<TKey, TExpectedValue> expectedValues,
            IDictionary<TKey, TActualValue> actualValues, Action<TKey, TExpectedValue, TActualValue> assertion)
        {
            if (expectedValues == null)
            {
                if (actualValues != null)
                    throw new AssertionException();
                return;
            }

            AreElementsEqualIgnoringOrder(expectedValues.Keys, actualValues.Keys, delegate(TKey a, TKey b) { return Equals(a, b); });

            foreach (KeyValuePair<TKey, TExpectedValue> expectedPair in expectedValues)
            {
                TKey key = expectedPair.Key;
                TActualValue actualValue;
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
        /// <typeparam name="TExpectedValue">The expected value type</typeparam>
        /// <typeparam name="TActualValue">The actual value type</typeparam>
        /// <param name="expectedValues">The enumeration of expected values</param>
        /// <param name="actualValues">The enumeration of actual values</param>
        /// <param name="assertion">The assertion to evaluate over all pairs</param>
        public static void WithKeyedPairs<TKey, TExpectedValue, TActualValue>(IDictionary<TKey, TExpectedValue> expectedValues,
            IDictionary<TKey, TActualValue> actualValues, Action<TExpectedValue, TActualValue> assertion)
        {
            WithKeyedPairs(expectedValues, actualValues,
                delegate(TKey key, TExpectedValue expectedValue, TActualValue actualValue)
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
