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
using System.Collections;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Static helpers to quickly generate random constrained values.
    /// </summary>
    public static class RandomGenerator
    {
        /// <summary>
        /// A random number generator within a specified range.
        /// </summary>
        public static class Number
        {
            /// <summary>
            /// Returns the specified number of random numbers.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="minimum">The lower bound of the range.</param>
            /// <param name="maximum">The upper bound of the range.</param>
            /// <returns>An enumeration of random number values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<decimal> Run(int count, decimal minimum, decimal maximum)
            {
                var generator = new RandomNumbersGenerator
                {
                    Count = count,
                    Minimum = minimum,
                    Maximum = maximum,
                };

                foreach (decimal value in generator.Run())
                    yield return value;
            }
        }

        /// <summary>
        /// A random string generator based on a regular expression filter.
        /// </summary>
        public static class Regex
        {
            /// <summary>
            /// Returns the specified number of random strings.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="regularExpressionPattern">The regular expression filter.</param>
            /// <returns>An enumeration of random string values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<string> Run(int count, string regularExpressionPattern)
            {
                var generator = new RandomRegexLiteStringsGenerator
                {
                    Count = count,
                    RegularExpressionPattern = regularExpressionPattern,
                };

                foreach (string value in generator.Run())
                    yield return value;
            }
        }
    }
}
