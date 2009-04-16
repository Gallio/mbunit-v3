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
        /// Verifies that the sequence of values contains distinct instances.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Distinct<T>(IEnumerable<T> values)
        {
            Distinct(values, (x, y) => ComparisonSemantics.Equals<T>(x, y), null, null);
        }

        /// <summary>
        /// Verifies that the sequence of values contains distinct instances.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Distinct<T>(IEnumerable<T> values, string messageFormat, params object[] messageArgs)
        {
            Distinct(values, (x, y) => ComparisonSemantics.Equals<T>(x, y), messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the sequence of values contains distinct instances.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="comparer">A comparer instance to be used to determine whether two elements are equal</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Distinct<T>(IEnumerable<T> values, IEqualityComparer<T> comparer)
        {
            Distinct(values, (x, y) => comparer.Equals(x, y), null, null);
        }

        /// <summary>
        /// Verifies that the sequence of values contains distinct instances.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="comparer">A comparer instance to be used to determine whether two elements are equal</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Distinct<T>(IEnumerable<T> values, IEqualityComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            Distinct(values, (x, y) => comparer.Equals(x, y), messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the sequence of values contains distinct instances.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="compare">A delegate used to determine whether two objects are equal.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Distinct<T>(IEnumerable<T> values, EqualityComparison<T> compare)
        {
            Distinct(values, compare, null, null);
        }

        /// <summary>
        /// Verifies that the sequence of values contains distinct instances.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="values">The sequence of values to be tested</param>
        /// <param name="compare">A delegate used to determine whether two objects are equal.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Distinct<T>(IEnumerable<T> values, EqualityComparison<T> compare, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(() =>
            {
                var duplicates = new List<T>();
                int i = 0;

                foreach (var value1 in values)
                {
                    int j = 0;

                    foreach (var value2 in values)
                    {
                        if ((i != j) && compare(value1, value2) && !duplicates.Contains(value1))
                        {
                            duplicates.Add(value1);
                        }

                        j++;
                    }

                    i++;
                }

                if (duplicates.Count > 0)
                {
                    var builder = new AssertionFailureBuilder(
                        "Expected the elements to be distinct but several instances represents the same value.")
                        .SetMessage(messageFormat, messageArgs);

                    foreach (var duplicate in duplicates)
                    {
                        builder.AddRawLabeledValue("Duplicated Value", duplicate);
                    }

                    return builder.ToAssertionFailure();
                }

                return null;
            });
        }
    }
}
