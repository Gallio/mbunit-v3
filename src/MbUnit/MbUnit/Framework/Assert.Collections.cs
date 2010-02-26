// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio.Common;
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
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence)
        {
            AreElementsEqual(expectedSequence, actualSequence, (EqualityComparison<T>)null, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, string messageFormat, params object[] messageArgs)
        {
            AreElementsEqual(expectedSequence, actualSequence, (EqualityComparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, IEqualityComparer<T> comparer)
        {
            AreElementsEqual(expectedSequence, actualSequence, comparer != null ? comparer.Equals : (EqualityComparison<T>)null, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, IEqualityComparer<T> comparer, string messageFormat, params object[] messageArgs)
		{
            AreElementsEqual(expectedSequence, actualSequence, comparer != null ? comparer.Equals : (EqualityComparison<T>)null, messageFormat, messageArgs);
		}
		
        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, EqualityComparison<T> comparer)
        {
            AreElementsEqual(expectedSequence, actualSequence, comparer, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, EqualityComparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AreElementsEqualImpl(expectedSequence, actualSequence, comparer, "equal", messageFormat, messageArgs);
        }

        private static void AreElementsEqualImpl<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, EqualityComparison<T> comparer, string equalityMode, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (expectedSequence == null && actualSequence == null)
                    return null;

                if (expectedSequence == null || actualSequence == null)
                {
                    return new AssertionFailureBuilder(
                        String.Format("Expected elements to be {0} but one sequence was null and not the other.", equalityMode))
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Expected Sequence", expectedSequence)
                        .AddRawLabeledValue("Actual Sequence", actualSequence)
                        .ToAssertionFailure();
                }

                if (comparer == null)
                    comparer = ComparisonSemantics.Default.Equals;

                int index = 0;
                IEnumerator<T> expectedEnumerator = expectedSequence.GetEnumerator();
                IEnumerator<T> actualEnumerator = actualSequence.GetEnumerator();
                while (expectedEnumerator.MoveNext())
                {
                    if (!actualEnumerator.MoveNext())
                    {
                        return new AssertionFailureBuilder(
                            String.Format("Expected elements to be {0} but the expected sequence has more elements than the actual sequence.", equalityMode))
                            .SetMessage(messageFormat, messageArgs)
                            .AddRawLabeledValue("Expected Sequence Count", 1 + index + CountRemainingElements(expectedEnumerator))
                            .AddRawLabeledValue("Actual Sequence Count", index)
                            .AddRawLabeledValuesWithDiffs("Expected Sequence", expectedSequence, "Actual Sequence", actualSequence)
                            .ToAssertionFailure();
                    }

                    T expectedValue = expectedEnumerator.Current;
                    T actualValue = actualEnumerator.Current;
                    if (!comparer(expectedValue, actualValue))
                    {
                        return new AssertionFailureBuilder(
                            String.Format("Expected elements to be {0} but they differ in at least one position.", equalityMode))
                            .SetMessage(messageFormat, messageArgs)
                            .AddRawLabeledValuesWithDiffs("Expected Sequence", expectedSequence, "Actual Sequence", actualSequence)
                            .AddLabeledValue("Element Index", index.ToString())
                            .AddRawLabeledValuesWithDiffs("Expected Element", expectedValue, "Actual Element", actualValue)
                            .ToAssertionFailure();
                    }

                    index += 1;
                }

                if (actualEnumerator.MoveNext())
                {
                    return new AssertionFailureBuilder(
                        String.Format("Expected elements to be {0} but the expected sequence has fewer elements than the actual sequence.", equalityMode))
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Expected Sequence Count", index)
                        .AddRawLabeledValue("Actual Sequence Count", index + CountRemainingElements(actualEnumerator) + 1)
                        .AddRawLabeledValuesWithDiffs("Expected Sequence", expectedSequence, "Actual Sequence", actualSequence)
                        .ToAssertionFailure();
                }

                return null;
            });
        }

        #endregion

        #region AreElementsNotEqual
        /// <summary>
        /// Verifies that unexpected and actual sequences differ in at least one element.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedSequence">The unexpected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> unexpectedSequence, IEnumerable<T> actualSequence)
        {
            AreElementsNotEqual(unexpectedSequence, actualSequence, (EqualityComparison<T>)null, null, null);
        }

        /// <summary>
        /// Verifies that unexpected and actual sequences differ in at least one element.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedSequence">The unexpected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> unexpectedSequence, IEnumerable<T> actualSequence, string messageFormat, params object[] messageArgs)
        {
            AreElementsNotEqual(unexpectedSequence, actualSequence, (EqualityComparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that unexpected and actual sequences differ in at least one element.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedSequence">The unexpected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> unexpectedSequence, IEnumerable<T> actualSequence, IEqualityComparer<T> comparer)
        {
            AreElementsNotEqual(unexpectedSequence, actualSequence, comparer != null ? comparer.Equals : (EqualityComparison<T>)null, null, null);
        }

        /// <summary>
        /// Verifies that unexpected and actual sequences differ in at least one element.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedSequence">The unexpected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> unexpectedSequence, IEnumerable<T> actualSequence, IEqualityComparer<T> comparer, string messageFormat, params object[] messageArgs)
		{
            AreElementsNotEqual(unexpectedSequence, actualSequence, comparer != null ? comparer.Equals : (EqualityComparison<T>)null, messageFormat, messageArgs);
		}
		
        /// <summary>
        /// Verifies that unexpected and actual sequences differ in at least one element.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedSequence">The unexpected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> unexpectedSequence, IEnumerable<T> actualSequence, EqualityComparison<T> comparer)
        {
            AreElementsNotEqual(unexpectedSequence, actualSequence, comparer, null, null);
        }

        /// <summary>
        /// Verifies that unexpected and actual sequences differ in at least one element.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedSequence">The unexpected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsNotEqual<T>(IEnumerable<T> unexpectedSequence, IEnumerable<T> actualSequence, EqualityComparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AreElementsNotEqualImpl(unexpectedSequence, actualSequence, comparer, "equal", messageFormat, messageArgs);
        }

        private static void AreElementsNotEqualImpl<T>(IEnumerable<T> unexpectedSequence, IEnumerable<T> actualSequence, EqualityComparison<T> comparer, string equalityMode, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (unexpectedSequence == null)
                {
                    if (actualSequence != null)
                        return null;

                    return new AssertionFailureBuilder("Expected the unexpected and actual sequence to have different elements but both sequences were null.")
                        .SetMessage(messageFormat, messageArgs)
                        .ToAssertionFailure();
                }
                else if (actualSequence == null)
                {
                    return null;
                }

                if (comparer == null)
                    comparer = ComparisonSemantics.Default.Equals;

                IEnumerator<T> unexpectedEnumerator = unexpectedSequence.GetEnumerator();
                IEnumerator<T> actualEnumerator = actualSequence.GetEnumerator();

                while (unexpectedEnumerator.MoveNext())
                {
                    if (!actualEnumerator.MoveNext())
                        return null; // different lengths

                    if (!comparer(unexpectedEnumerator.Current, actualEnumerator.Current))
                        return null; // different contents
                }

                if (actualEnumerator.MoveNext())
                    return null; // different lengths

                return new AssertionFailureBuilder(
                    String.Format("Expected the unexpected and actual sequence to have different elements but all elements were {0}.", equalityMode)) 
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Unexpected Sequence", unexpectedSequence)
                    .AddRawLabeledValue("Actual Sequence", actualSequence)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreElementsEqualIgnoringOrder
        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal but perhaps in a different order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqualIgnoringOrder<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence)
        {
            AreElementsEqualIgnoringOrder(expectedSequence, actualSequence, (EqualityComparison<T>)null, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal but perhaps in a different order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqualIgnoringOrder<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, string messageFormat, params object[] messageArgs)
        {
            AreElementsEqualIgnoringOrder(expectedSequence, actualSequence, (EqualityComparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal but perhaps in a different order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqualIgnoringOrder<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, IEqualityComparer<T> comparer)
        {
            AreElementsEqualIgnoringOrder(expectedSequence, actualSequence, comparer != null ? comparer.Equals : (EqualityComparison<T>)null, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal but perhaps in a different order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqualIgnoringOrder<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, IEqualityComparer<T> comparer, string messageFormat, params object[] messageArgs)
		{
            AreElementsEqualIgnoringOrder(expectedSequence, actualSequence, comparer != null ? comparer.Equals : (EqualityComparison<T>)null, messageFormat, messageArgs);
		}		
		
        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal but perhaps in a different order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqualIgnoringOrder<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, EqualityComparison<T> comparer)
        {
            AreElementsEqualIgnoringOrder(expectedSequence, actualSequence, comparer, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are equal but perhaps in a different order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsEqualIgnoringOrder<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, EqualityComparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AreElementsEqualIgnoringOrderImpl(expectedSequence, actualSequence, comparer, "equal", messageFormat, messageArgs);
        }

        private static void AreElementsEqualIgnoringOrderImpl<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, EqualityComparison<T> comparer, string equalityMode, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (expectedSequence == null && actualSequence == null)
                    return null;

                if (expectedSequence == null || actualSequence == null)
                {
                    return new AssertionFailureBuilder(
                        String.Format("Expected elements to be {0} but one sequence was null and not the other.", equalityMode))
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Expected Sequence", expectedSequence)
                        .AddRawLabeledValue("Actual Sequence", actualSequence)
                        .ToAssertionFailure();
                }

                if (comparer == null)
                    comparer = ComparisonSemantics.Default.Equals;

                // Count the number of matching expected and actual elements.
                var table = new MatchTable<T>(comparer);
                foreach (T expectedElement in expectedSequence)
                    table.AddLeftValue(expectedElement);

                foreach (T actualElement in actualSequence)
                    table.AddRightValue(actualElement);

                // Find out what's different.
                if (table.NonEqualCount == 0)
                    return null;

                var equalElements = new List<T>();
                var excessElements = new List<T>();
                var missingElements = new List<T>();
                foreach (KeyValuePair<T, Pair<int, int>> item in table.Items)
                {
                    T element = item.Key;
                    int expectedCount = item.Value.First;
                    int actualCount = item.Value.Second;

                    AddNTimes(equalElements, element, Math.Min(expectedCount, actualCount));
                    AddNTimes(excessElements, element, actualCount - expectedCount);
                    AddNTimes(missingElements, element, expectedCount - actualCount);
                }

                return new AssertionFailureBuilder(
                    String.Format("Expected elements to be {0} but possibly in a different order.", equalityMode))
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Equal Elements", equalElements)
                    .AddRawLabeledValue("Excess Elements", excessElements)
                    .AddRawLabeledValue("Missing Elements", missingElements)
                    .ToAssertionFailure();
            });
        }

        private static void AddNTimes<T>(List<T> list, T value, int count)
        {
            while (count-- > 0)
                list.Add(value);
        }

        #endregion

        #region AreElementsSame
        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are referentially equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsSame<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence) where T : class
        {
            AreElementsSame(expectedSequence, actualSequence, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are referentially equal and in the same order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsSame<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, string messageFormat, params object[] messageArgs) where T : class
        {
            AreElementsEqualImpl(expectedSequence, actualSequence, (x, y) => Object.ReferenceEquals(x, y), "referentially equal", messageFormat, messageArgs);
        }

        #endregion

        #region AreElementsNotSame
        /// <summary>
        /// Verifies that unexpected and actual sequences differ in at least one element.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedSequence">The unexpected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsNotSame<T>(IEnumerable<T> unexpectedSequence, IEnumerable<T> actualSequence) where T : class
        {
            AreElementsNotSame(unexpectedSequence, actualSequence, null, null);
        }

        /// <summary>
        /// Verifies that unexpected and actual sequences differ in at least one element.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedSequence">The unexpected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsNotSame<T>(IEnumerable<T> unexpectedSequence, IEnumerable<T> actualSequence, string messageFormat, params object[] messageArgs) where T : class
        {
            AreElementsNotEqualImpl(unexpectedSequence, actualSequence, (x, y) => Object.ReferenceEquals(x, y), "referentially equal", messageFormat, messageArgs);
        }

        #endregion

        #region AreElementsSameIgnoringOrder
        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are referentially equal but perhaps in a different order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsSameIgnoringOrder<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence) where T : class
        {
            AreElementsSameIgnoringOrder(expectedSequence, actualSequence, null, null);
        }

        /// <summary>
        /// Verifies that expected and actual sequences have the same number of elements and
        /// that the elements are referentially equal but perhaps in a different order.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedSequence">The expected sequence.</param>
        /// <param name="actualSequence">The actual sequence.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreElementsSameIgnoringOrder<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, string messageFormat, params object[] messageArgs) where T : class
        {
            AreElementsEqualIgnoringOrderImpl(expectedSequence, actualSequence, (x, y) => Object.ReferenceEquals(x, y), "referentially equal", messageFormat, messageArgs);
        }

        #endregion

        #region Contains
        /// <summary>
        /// Verifies that <paramref name="expectedValue"/> is in the enumeration <paramref name="enumeration"/>.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="enumeration">The enumeration of items.</param>
        /// <param name="expectedValue">The expected value expected to be found in the collection.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        static public void Contains<T>(IEnumerable<T> enumeration, T expectedValue)
        {
            Contains(enumeration, expectedValue, null);
        }

        /// <summary>
        /// Verifies that <paramref name="expectedValue"/> is in the enumeration <paramref name="enumeration"/>.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="enumeration">The enumeration of items.</param>
        /// <param name="expectedValue">The expected value expected to be found in the collection.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
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
        /// Verifies that <paramref name="expectedKey"/> is in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="dictionary">The dictionary of items.</param>
        /// <param name="expectedKey">The key expected to be found in the dictionary.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        static public void ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey expectedKey)
        {
            ContainsKey(dictionary, expectedKey, null);
        }

        /// <summary>
        /// Verifies that <paramref name="expectedKey"/> is in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="dictionary">The dictionary of items.</param>
        /// <param name="expectedKey">The key expected to be found in the dictionary.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
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
        /// Verifies that <paramref name="unexpectedValue"/> is not in the enumeration <paramref name="enumeration"/>.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="enumeration">The enumeration of items.</param>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        static public void DoesNotContain<T>(IEnumerable<T> enumeration, T unexpectedValue)
        {
            DoesNotContain(enumeration, unexpectedValue, null);
        }

        /// <summary>
        /// Verifies that <paramref name="unexpectedValue"/> is not in the enumeration <paramref name="enumeration"/>.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="enumeration">The enumeration of items.</param>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
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
        /// Verifies that <paramref name="unexpectedKey"/> is not in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="dictionary">The dictionary of items.</param>
        /// <param name="unexpectedKey">The key that should not be found in the dictionary.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        static public void DoesNotContainKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey unexpectedKey)
        {
            DoesNotContainKey(dictionary, unexpectedKey, null);
        }

        /// <summary>
        /// Verifies that <paramref name="unexpectedKey"/> is not in the dictionary <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="dictionary">The dictionary of items.</param>
        /// <param name="unexpectedKey">The key expected to be found in the dictionary.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
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
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsEmpty(IEnumerable actualValue)
        {
            IsEmpty(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value contains no elements.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
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
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsNotEmpty(IEnumerable actualValue)
        {
            IsNotEmpty(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value contains at least one element.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
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

        #region ForAll

        /// <summary>
        /// Verifies that all the elements of the sequence meet the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of values in the sequence.</typeparam>
        /// <param name="values">The sequence of values to evaluate.</param>
        /// <param name="predicate">The condition that must be fulfilled (returns <c>true</c>) by all the elements of the sequence.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void ForAll<T>(IEnumerable<T> values, Predicate<T> predicate)
        {
            ForAll(values, predicate, null, null);
        }

        /// <summary>
        /// Verifies that all the elements of the sequence meet the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of values in the sequence.</typeparam>
        /// <param name="values">The sequence of values to evaluate.</param>
        /// <param name="predicate">The condition that must be fulfilled (returns <c>true</c>) by all the elements of the sequence.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void ForAll<T>(IEnumerable<T> values, Predicate<T> predicate, string messageFormat, params object[] messageArgs)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            AssertionHelper.Verify(() =>
            {
                var failing = new List<T>();

                foreach (T value in values)
                {
                    if (!predicate(value))
                        failing.Add(value);
                }

                if (failing.Count == 0)
                    return null;

                return new AssertionFailureBuilder("Expected all the elements of the sequence to meet the specified condition, but at least one failed.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Sequence", values)
                    .AddRawLabeledValue("Failing elements", failing)
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region Exists

        /// <summary>
        /// Verifies that at least one element of the sequence meets the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of values in the sequence.</typeparam>
        /// <param name="values">The sequence of values to evaluate.</param>
        /// <param name="predicate">The condition that must be fulfilled (returns <c>true</c>) by at least one element of the sequence.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void Exists<T>(IEnumerable<T> values, Predicate<T> predicate)
        {
            Exists(values, predicate, null, null);
        }

        /// <summary>
        /// Verifies that at least one element of the sequence meets the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of values in the sequence.</typeparam>
        /// <param name="values">The sequence of values to evaluate.</param>
        /// <param name="predicate">The condition that must be fulfilled (returns <c>true</c>) by at least one element of the sequence.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void Exists<T>(IEnumerable<T> values, Predicate<T> predicate, string messageFormat, params object[] messageArgs)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            AssertionHelper.Verify(() =>
            {
                foreach (T value in values)
                {
                    if (predicate(value))
                        return null;
                }

                return new AssertionFailureBuilder("Expected at least one element of the sequence to meet the specified condition, but none passed.")
                    .SetMessage(messageFormat, messageArgs)
                    .ToAssertionFailure();
            });
        }

        #endregion

        internal static int CountRemainingElements(IEnumerator enumerator)
        {
            int count = 0;
            while (enumerator.MoveNext())
                count++;
            return count;
        }
    }
}
