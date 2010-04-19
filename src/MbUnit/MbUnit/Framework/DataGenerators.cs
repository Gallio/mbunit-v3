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
using Gallio.Framework.Data.Generation;

namespace MbUnit.Framework
{
    /// <summary>
    /// Static helpers to quickly generate random constrained values.
    /// </summary>
    public static class DataGenerators
    {
        /// <summary>
        /// Generation of random or constrained numbers.
        /// </summary>
        public static class Numbers
        {
            /// <summary>
            /// Returns the specified number of random numbers.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="minimum">The lower bound of the range.</param>
            /// <param name="maximum">The upper bound of the range.</param>
            /// <returns>An enumeration of random number values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<decimal> Random(int count, decimal minimum, decimal maximum)
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

            /// <summary>
            /// sequential a linear sequence of <see cref="Decimal"/> numbers, with a step of 1.
            /// </summary>
            /// <param name="start">The first value of the sequence.</param>
            /// <param name="end">The last value of the sequence.</param>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<decimal> Sequence(decimal start, decimal end)
            {
                return SequenceImpl(start, end, 1, null);
            }

            /// <summary>
            /// sequential a linear sequence of <see cref="Decimal"/> numbers.
            /// </summary>
            /// <param name="start">The first value of the sequence.</param>
            /// <param name="end">The last value of the sequence.</param>
            /// <param name="step">The step between each consecutive value.</param>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<decimal> Sequence(decimal start, decimal end, decimal step)
            {
                return SequenceImpl(start, end, step, null);
            }

            private static IEnumerable<decimal> SequenceImpl(decimal? start, decimal? end, decimal? step, int? count)
            {
                var generator = new SequentialNumbersGenerator
                {
                    Start = start,
                    End = end,
                    Step = step,
                    Count = count,
                };

                foreach (decimal value in generator.Run())
                    yield return value;
            }
        }

        /// <summary>
        /// Generation of random or constrained strings.
        /// </summary>
        public static class Strings
        {
            /// <summary>
            /// Returns the specified number of random strings based on a regular expression filter.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="regularExpressionPattern">The regular expression filter.</param>
            /// <returns>An enumeration of random string values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<string> Random(int count, string regularExpressionPattern)
            {
                var generator = new RandomRegexLiteStringsGenerator
                {
                    Count = count,
                    RegularExpressionPattern = regularExpressionPattern,
                };

                foreach (string value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns the specified number of random strings from a pre-existing stock of values.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="stock">A stock of preset values.</param>
            /// <returns>An enumeration of random string values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<string> Random(int count, RandomStringStock stock)
            {
                var generator = new RandomStockStringsGenerator
                {
                    Count = count,
                    Values = RandomStringStockInfo.FromStock(stock).GetItems(),
                };

                foreach (string value in generator.Run())
                    yield return value;
            }
        }
    }
}
