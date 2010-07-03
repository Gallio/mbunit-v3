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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using System.Reflection;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        /// <summary>
        /// Verifies that the specified sequence, collection, or array contains the expected number of elements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion counts the elements according to the underlying type of the sequence.
        /// <list type="bullet">
        /// <item>Uses <see cref="Array.Length"/> if the sequence is an array.</item>
        /// <item>Uses <see cref="ICollection.Count"/> if the sequence is a collection such as <see cref="List{T}"/> or <see cref="Dictionary{K,V}"/>.</item>
        /// <item>Enumerates and counts the elements if the sequence is a simple <see cref="IEnumerable"/>.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="expectedCount">The expected number of elements.</param>
        /// <param name="values">The enumeration of elements to count.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is negative.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is null.</exception>
        public static void Count(int expectedCount, IEnumerable values)
        {
            Count(expectedCount, values, null, null);
        }

        /// <summary>
        /// Verifies that the specified sequence, collection, or array contains the expected number of elements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion counts the elements according to the underlying type of the sequence.
        /// <list type="bullet">
        /// <item>Uses <see cref="Array.Length"/> if the sequence is an array.</item>
        /// <item>Uses <see cref="ICollection.Count"/> if the sequence is a collection such as <see cref="List{T}"/> or <see cref="Dictionary{K,V}"/>.</item>
        /// <item>Enumerates and counts the elements if the sequence is a simple <see cref="IEnumerable"/>.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="expectedCount">The expected number of elements.</param>
        /// <param name="values">The enumeration of elements to count.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is negative.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is null.</exception>
        public static void Count(int expectedCount, IEnumerable values, string messageFormat, params object[] messageArgs)
        {
            if (expectedCount < 0)
                throw new ArgumentOutOfRangeException("expectedCount", "The expected count value must be greater than or equal to 0.");
            if (values == null)
                throw new ArgumentNullException("values");

            AssertionHelper.Verify(() =>
            {
                return ForArray(expectedCount, values, messageFormat, messageArgs)
                    ?? ForNonGenericCollection(expectedCount, values, messageFormat, messageArgs)
                    ?? ForGenericCollection(expectedCount, values, messageFormat, messageArgs)
                    ?? ForSimpleEnumerable(expectedCount, values, messageFormat, messageArgs);
            });
        }

        private static AssertionFailure ForArray(int expectedCount, IEnumerable values, string messageFormat, params object[] messageArgs)
        {
            if (values is Array)
                return CountFromLengthProperty(expectedCount, ((Array)values).Length, messageFormat, messageArgs);

            return null;
        }

        private static AssertionFailure ForNonGenericCollection(int expectedCount, IEnumerable values, string messageFormat, params object[] messageArgs)
        {
            if (values is ICollection)
            {
                return CountFromCountProperty(expectedCount, ((ICollection)values).Count, messageFormat, messageArgs)
                    ?? CountByEnumeratingElements(expectedCount, values, messageFormat, messageArgs);
            }

            return null;
        }

        private static AssertionFailure ForGenericCollection(int expectedCount, IEnumerable values, string messageFormat, params object[] messageArgs)
        {
            foreach (Type type in values.GetType().GetInterfaces())
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    var actualCount = (int)type.GetProperty("Count").GetValue(values, null);
                    return CountFromCountProperty(expectedCount, actualCount, messageFormat, messageArgs)
                        ?? CountByEnumeratingElements(expectedCount, values, messageFormat, messageArgs);
                }
            }

            return null;
        }

        private static AssertionFailure ForSimpleEnumerable(int expectedCount, IEnumerable values, string messageFormat, params object[] messageArgs)
        {
            return CountByEnumeratingElements(expectedCount, values, messageFormat, messageArgs);
        }

        private static AssertionFailure CountFromCountProperty(int expectedCount, int actualCount, string messageFormat, params object[] messageArgs)
        {
            if (actualCount == expectedCount)
                return null;
            
            return new AssertionFailureBuilder("Expected the collection to contain a certain number of elements.")
                .AddRawExpectedValue(expectedCount)
                .AddRawLabeledValue("Actual Value (Count)", actualCount)
                .SetMessage(messageFormat, messageArgs)
                .ToAssertionFailure();
        }

        private static AssertionFailure CountFromLengthProperty(int expectedLength, int actualLength, string messageFormat, params object[] messageArgs)
        {
            if (actualLength == expectedLength)
                return null;

            return new AssertionFailureBuilder("Expected the array to contain a certain number of elements.")
                    .AddRawExpectedValue(expectedLength)
                    .AddRawLabeledValue("Actual Value (Length)", actualLength)
                    .SetMessage(messageFormat, messageArgs)
                    .ToAssertionFailure();
        }

        private static AssertionFailure CountByEnumeratingElements(int expectedCount, IEnumerable values, string messageFormat, params object[] messageArgs)
        {
            int count = 0;
            var enumerator = values.GetEnumerator();

            while (enumerator.MoveNext())
                count++;

            if (count == expectedCount)
                return null;
            
            return new AssertionFailureBuilder("Expected the sequence to contain a certain number of elements.")
                .AddRawExpectedValue(expectedCount)
                .AddRawActualValue(count)
                .SetMessage(messageFormat, messageArgs)
                .ToAssertionFailure();
        }
    }
}
