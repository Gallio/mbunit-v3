// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Collections;
using System.Collections.Generic;

namespace Gallio.Data
{
    /// <summary>
    /// This class provides a set of factory methods for creating enumerable objects
    /// that generate values according to a specified rule.  The generated values may
    /// be used for a variety of purposes including data-driven testing.
    /// </summary>
    public static class Generators
    {
        /// <summary>
        /// Creates an enumeration that yields a linear sequence of <see cref="Int32" /> values
        /// forming an arithmetic progression.
        /// </summary>
        /// <param name="firstValue">The first value to generate</param>
        /// <param name="count">The number of values to generate</param>
        /// <param name="step">The increment for each successive value</param>
        /// <returns>The enumeration</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than 0</exception>
        public static IEnumerable<int> Linear(int firstValue, int count, int step)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", count, "Count must not be less than 0.");

            return InternalLinear(firstValue, count, step);
        }
        private static IEnumerable<int> InternalLinear(int firstValue, int count, int step)
        {
            int remaining = count;
            for (int value = firstValue; remaining-- > 0; value += step)
                yield return value;
        }

        /// <summary>
        /// Creates an enumeration that yields a linear sequence of <see cref="Double" /> values
        /// forming an arithmetic progression.
        /// </summary>
        /// <param name="firstValue">The first value to generate</param>
        /// <param name="count">The number of values to generate</param>
        /// <param name="step">The increment for each successive value</param>
        /// <returns>The enumeration</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than 0</exception>
        public static IEnumerable<double> Linear(double firstValue, int count, double step)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", count, "Count must not be less than 0.");

            return InternalLinear(firstValue, count, step);
        }
        private static IEnumerable<double> InternalLinear(double firstValue, int count, double step)
        {
            int remaining = count;
            for (double value = firstValue; remaining-- > 0; value += step)
                yield return value;
        }

        /// <summary>
        /// Creates an enumeration that yields all values of an <see cref="Enum" /> type.
        /// </summary>
        /// <param name="enumType">The <see cref="Enum" /> type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="enumType"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="enumType"/> is not an <see cref="Enum" /> type</exception>
        public static IEnumerable EnumValues(Type enumType)
        {
            return Enum.GetValues(enumType);
        }
    }
}
