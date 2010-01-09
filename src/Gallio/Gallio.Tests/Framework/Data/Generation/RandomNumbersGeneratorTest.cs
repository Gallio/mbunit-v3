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
using System.Linq;
using System.Text;
using Gallio.Framework.Data.Generation;
using MbUnit.Framework;
using Gallio.Framework;

namespace Gallio.Tests.Framework.Data.Generation
{
    [TestFixture]
    [TestsOn(typeof(RandomNumbersGenerator))]
    public class RandomNumbersGeneratorTest
    {
        [Test]
        [Row(0, 1, 0)]
        [Row(-1, 1, 1)]
        [Row(-10, 10, 123)]
        [Row(0, 100000, 3)]
        public void Generate_sequence_ok(decimal minimum, decimal maximum, int count)
        {
            var generator = new RandomNumbersGenerator
            {
                Minimum = minimum,
                Maximum = maximum,
                Count = count
            };

            var values = generator.Run().Cast<decimal>().ToArray();
            Assert.AreEqual(count, values.Length);
            Assert.Multiple(() =>
            {
                foreach (decimal value in values)
                {
                    Assert.Between(value, minimum, maximum);
                }
            });
        }

        private IEnumerable<object[]> GetInvalidProperyValues()
        {
            yield return new object[] { Decimal.MinValue, 10, 1 };
            yield return new object[] { Decimal.MaxValue, 10, 1 };
            yield return new object[] { 10, Decimal.MinValue, 1 };
            yield return new object[] { 10, Decimal.MaxValue, 1 };
            yield return new object[] { 10, 5, 1, }; // Minimum greater than maximum!
            yield return new object[] { 10, 20, -1 }; // Negative count!
        }

        [Test, Factory("GetInvalidProperyValues")]
        public void Constructs_with_invalid_property_should_throw_exception(decimal minimum, decimal maximum, int count)
        {
            var generator = new RandomNumbersGenerator
            {
                Minimum = minimum,
                Maximum = maximum,
                Count = count
            };

            Assert.Throws<GenerationException>(() => generator.Run().Cast<decimal>().ToArray());
        }

        [Test]
        public void Generate_filtered_sequence()
        {
            var generator = new RandomNumbersGenerator
            {
                Minimum = 0,
                Maximum = 100,
                Count = 50,
                Filter = d => ((int)d % 2) == 0
            };

            var values = generator.Run().Cast<decimal>().ToArray();
            Assert.AreEqual(50, values.Length);
            Assert.Multiple(() =>
            {
                foreach (decimal value in values)
                {
                    Assert.AreEqual(0, (int)value % 2);
                }
            });
        }
    }
}
