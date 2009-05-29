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
using System.Linq;
using System.Text;
using Gallio.Framework.Data.Generation;
using MbUnit.Framework;
using Gallio.Framework;
using System.Collections;

namespace Gallio.Tests.Framework.Data.Generation
{
    [TestFixture]
    [TestsOn(typeof(SequentialNumbersGenerator))]
    public class SequentialNumbersGeneratorTest
    {
        [Test]
        [Row(0, 0, 0, new double[] { })]
        [Row(0, 0, 1, new[] { 0 })]
        [Row(1, 1, 1, new[] { 1 })]
        [Row(1, 1, 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, -1, 10, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 2.26, 6, new[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8 })]
        public void Generate_start_step_count_sequence_ok(decimal start, decimal step, int count, decimal[] expectedOutput)
        {
            var generator = new SequentialNumbersGenerator
            {
                Start = start,
                Step = step,
                Count = count
            };

            var actualOutput = generator.Run().Cast<decimal>();
            Assert.AreElementsEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(0, 0, 0, new double[] { })]
        [Row(0, 0, 1, new[] { 0 })]
        [Row(1, 1, 1, new[] { 1 })]
        [Row(1, 10, 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, 1, 10, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 7.8, 6, new[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8 })]
        public void Generate_start_stop_count_sequence_ok(decimal start, decimal stop, int count, decimal[] expectedOutput)
        {
            var generator = new SequentialNumbersGenerator
            {
                Start = start,
                End = stop,
                Count = count
            };

            var actualOutput = generator.Run().Cast<decimal>();
            Assert.AreElementsEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(0, 0, 1, new[] { 0 })]
        [Row(1, 1, 1, new[] { 1 })]
        [Row(1, 10, 1, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, 1, -1, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 7.8, 2.26, new[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8 })]
        public void Generate_start_stop_step_sequence_ok(decimal start, decimal stop, decimal step, decimal[] expectedOutput)
        {
            var generator = new SequentialNumbersGenerator
            {
                Start = start,
                End = stop,
                Step = step
            };

            var actualOutput = generator.Run().Cast<decimal>();
            Assert.AreElementsEqual(expectedOutput, actualOutput);
        }

        private IEnumerable<object[]> GetInvalidProperyValues()
        {
            yield return new object[] { null, null, null, null };
            yield return new object[] { 1, 10, 1, 10 };
            yield return new object[] { 1, 10, -1, null };
            yield return new object[] { 1, 10, 0, null };
            yield return new object[] { 1, 10, null, -1 };
            yield return new object[] { Decimal.MaxValue, 10, null, 10 };
            yield return new object[] { Decimal.MinValue, 10, null, 10 };
            yield return new object[] { 1, Decimal.MaxValue, null, 10 };
            yield return new object[] { 1, Decimal.MinValue, null, 10 };
            yield return new object[] { 1, 10, Decimal.MaxValue, null };
            yield return new object[] { 1, 10, Decimal.MinValue, null };
            yield return new object[] { 1, 10, null, Int32.MinValue };
            yield return new object[] { 1, 10, null, Int32.MaxValue };
        }

        [Test, Factory("GetInvalidProperyValues")]
        public void Generate_sequence_with_invalid_property_settings_should_throw_exception(decimal? start, decimal? stop, decimal? step, int? count)
        {
            var generator = new SequentialNumbersGenerator
            {
                Start = start,
                End = stop,
                Step = step,
                Count = count
            };

            Assert.Throws<GenerationException>(() => generator.Run().Cast<decimal>().ToArray());
        }

        [Test]
        public void Generate_filtered_sequence()
        {
            var generator = new SequentialNumbersGenerator
            {
                Start = 0,
                End = 100,
                Step = 1,
                Filter = d => IsPrime(d)
            };

            var actualOutput = generator.Run().Cast<decimal>();
            Assert.AreElementsEqual(new decimal[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 }, actualOutput);
        }

        public static bool IsPrime(decimal number)
        {
            // Simple implementation of the "Sieve of Eratosthenes" algorithm, 
            // that checks for the primality of a number 
            // (http://en.wikipedia.org/wiki/Sieve_of_Eratosthenes)
            int n = (int)Convert.ChangeType(number, typeof(int));
            var array = new BitArray(n + 1, true);

            for (int i = 2; i < n + 1; i++)
            {
                if (array[i])
                {
                    for (int j = i * 2; j < n + 1; j += i)
                    {
                        array[j] = false;
                    }

                    if (array[i] && (n == i))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
