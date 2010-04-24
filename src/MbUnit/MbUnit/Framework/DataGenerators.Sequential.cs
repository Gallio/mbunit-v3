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
    /// Helper methods to quickly combine and generate random or constrained values for data-driven tests.
    /// </summary>
    public static partial class DataGenerators
    {
        /// <summary>
        /// Generates sequential values.
        /// </summary>
        public static class Sequential
        {
            /// <summary>
            /// Generates a linear sequence of <see cref="Decimal"/> numbers, with a step of 1.
            /// </summary>
            /// <param name="start">The first value of the sequence.</param>
            /// <param name="end">The last value of the sequence.</param>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<decimal> Numbers(decimal start, decimal end)
            {
                return NumbersImpl(start, end, 1, null);
            }

            /// <summary>
            /// Generates a linear sequence of <see cref="double"/> numbers, with a step of 1.
            /// </summary>
            /// <param name="start">The first value of the sequence.</param>
            /// <param name="end">The last value of the sequence.</param>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<double> Numbers(double start, double end)
            {
                return NumbersImpl(start, end, 1, null);
            }

            /// <summary>
            /// Generates a linear sequence of <see cref="int"/> numbers, with a step of 1.
            /// </summary>
            /// <param name="start">The first value of the sequence.</param>
            /// <param name="end">The last value of the sequence.</param>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<int> Numbers(int start, int end)
            {
                return NumbersImpl(start, end, 1, null);
            }

            /// <summary>
            /// Generates a linear sequence of <see cref="Decimal"/> numbers.
            /// </summary>
            /// <param name="start">The first value of the sequence.</param>
            /// <param name="end">The last value of the sequence.</param>
            /// <param name="step">The step between each consecutive value.</param>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<decimal> Numbers(decimal start, decimal end, decimal step)
            {
                return NumbersImpl(start, end, step, null);
            }

            /// <summary>
            /// Generates a linear sequence of <see cref="double"/> numbers.
            /// </summary>
            /// <param name="start">The first value of the sequence.</param>
            /// <param name="end">The last value of the sequence.</param>
            /// <param name="step">The step between each consecutive value.</param>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<double> Numbers(double start, double end, double step)
            {
                return NumbersImpl(start, end, step, null);
            }

            /// <summary>
            /// Generates a linear sequence of <see cref="int"/> numbers.
            /// </summary>
            /// <param name="start">The first value of the sequence.</param>
            /// <param name="end">The last value of the sequence.</param>
            /// <param name="step">The step between each consecutive value.</param>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<int> Numbers(int start, int end, int step)
            {
                return NumbersImpl(start, end, step, null);
            }

            private static IEnumerable<decimal> NumbersImpl(decimal? start, decimal? end, decimal? step, int? count)
            {
                var generator = new SequentialDecimalGenerator
                {
                    Start = start,
                    End = end,
                    Step = step,
                    Count = count,
                };

                foreach (decimal value in generator.Run())
                    yield return value;
            }

            private static IEnumerable<double> NumbersImpl(double? start, double? end, double? step, int? count)
            {
                var generator = new SequentialDoubleGenerator
                {
                    Start = start,
                    End = end,
                    Step = step,
                    Count = count,
                };

                foreach (double value in generator.Run())
                    yield return value;
            }

            private static IEnumerable<int> NumbersImpl(int? start, int? end, int? step, int? count)
            {
                var generator = new SequentialInt32Generator
                {
                    Start = start,
                    End = end,
                    Step = step,
                    Count = count,
                };

                foreach (int value in generator.Run())
                    yield return value;
            }
        }
    }
}
