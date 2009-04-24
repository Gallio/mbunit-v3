// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using System.Collections;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        /// <summary>
        /// Verifies that the sequence of values are sorted in the specified order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="sortOrder">The expected sort order</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Sorted<T>(IEnumerable<T> values, SortOrder sortOrder)
        {
            Sorted(values, sortOrder, (Comparison<T>)null, null, null);
        }

        /// <summary>
        /// Verifies that the sequence of values are sorted in the specified order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="sortOrder">The expected sort order</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Sorted<T>(IEnumerable<T> values, SortOrder sortOrder, string messageFormat, params object[] messageArgs)
        {
            Sorted(values, sortOrder, (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the sequence of values are sorted in the specified order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="sortOrder">The expected sort order</param>
        /// <param name="comparer">A comparer instance to be used to compare two elements of the sequence, or null to use a default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Sorted<T>(IEnumerable<T> values, SortOrder sortOrder, IComparer<T> comparer)
        {
            Sorted(values, sortOrder, comparer != null ? comparer.Compare : (Comparison<T>)null, null, null);
        }

        /// <summary>
        /// Verifies that the sequence of values are sorted in the specified order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="sortOrder">The expected sort order</param>
        /// <param name="comparer">A comparer instance to be used to compare two elements of the sequence, or null to use a default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Sorted<T>(IEnumerable<T> values, SortOrder sortOrder, IComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            Sorted(values, sortOrder, comparer != null ? comparer.Compare : (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the sequence of values are sorted in the specified order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="sortOrder">The expected sort order</param>
        /// <param name="compare">A comparison function to be used to compare two elements of the sequence, or null to use a default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Sorted<T>(IEnumerable<T> values, SortOrder sortOrder, Comparison<T> compare)
        {
            Sorted(values, sortOrder, compare, null, null);
        }

        /// <summary>
        /// Verifies that the sequence of values are sorted in the specified order.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="sortOrder">The expected sort order</param>
        /// <param name="comparer">A comparison function to be used to compare two elements of the sequence, or null to use a default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Sorted<T>(IEnumerable<T> values, SortOrder sortOrder, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(() =>
            {
                bool first = true;
                var previous = default(T);
                var sortInfo = SortOrderInfo.FromSortOrder(sortOrder);
                int delta;
                int index = 0;

                if (comparer == null)
                    comparer = ComparisonSemantics.Compare;

                foreach (T value in values)
                {
                    if (!first)
                    {
                        try
                        {
                            delta = comparer(value, previous);
                        }
                        catch (InvalidOperationException exception)
                        {
                            return new AssertionFailureBuilder(
                                "Expected the elements to be sorted in a specific order but no implicit ordering comparison can be found for the subject type.")
                                .SetMessage(messageFormat, messageArgs)
                                .AddRawLabeledValue("Type", typeof(T))
                                .AddException(exception)
                                .ToAssertionFailure();
                        }

                        if (!sortInfo.VerifyDeltaSign(delta))
                        {
                            return new AssertionFailureBuilder(
                                "Expected the elements to be sorted in a specific order but the sequence of values mismatches at one position at least.")
                                .SetMessage(messageFormat, messageArgs)
                                .AddRawLabeledValue("Expected Sort Order", sortInfo.Description)
                                .AddRawLabeledValue("Sequence", values)
                                .AddRawLabeledValue("Failing Position", index)
                                .ToAssertionFailure();
                        }
                    }

                    previous = value;
                    first = false;
                    index++;
                }

                return null;
            });
        }

        private class SortOrderInfo
        {
            private readonly string description;
            private readonly Func<int, bool> verifyDeltaSign;

            private SortOrderInfo(string description, Func<int, bool> verifyDeltaSign)
            {
                this.description = description;
                this.verifyDeltaSign = verifyDeltaSign;
            }

            public string Description
            {
                get
                {
                    return description;
                }
            }

            public bool VerifyDeltaSign(int delta)
            {
                return verifyDeltaSign(delta);
            }

            public static SortOrderInfo FromSortOrder(SortOrder sortOrder)
            {
                return all[sortOrder];
            }

            private static readonly Dictionary<SortOrder, SortOrderInfo> all = new Dictionary<SortOrder, SortOrderInfo>
            {
                {   SortOrder.Increasing, 
                    new SortOrderInfo(
                        "Increasing (the next element is greater than or equal to the previous element)", 
                        delta => delta >= 0)  
                },
                {   SortOrder.StrictlyIncreasing, 
                    new SortOrderInfo(
                        "Strictly Increasing (the next element is strictly greater than the previous element)", 
                        delta => delta > 0) 
                },
                {   SortOrder.Decreasing, 
                    new SortOrderInfo(
                        "Decreasing (the next element is less than or equal to the previous element)", 
                        delta => delta <= 0) 
                },
                {   SortOrder.StrictlyDecreasing, 
                    new SortOrderInfo(
                        "Strictly Decreasing (the next element is strictly less than the previous element)", 
                        delta => delta < 0) 
                }
            };
        }
    }
}
