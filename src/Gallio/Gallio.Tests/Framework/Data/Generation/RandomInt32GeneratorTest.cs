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
    [TestsOn(typeof(RandomInt32Generator))]
    public class RandomInt32GeneratorTest
    {
        [Test]
        [Row(0, 1, 0)]
        [Row(-1, 1, 1)]
        [Row(-10, 10, 123)]
        [Row(0, 100000, 3)]
        public void Generate_sequence_ok(int minimum, int maximum, int count)
        {
            var generator = new RandomInt32Generator
            {
                Minimum = minimum,
                Maximum = maximum,
                Count = count
            };

            var values = generator.Run().Cast<int>().ToArray();
            Assert.AreEqual(count, values.Length);
            Assert.Multiple(() =>
            {
                foreach (int value in values)
                {
                    Assert.Between(value, minimum, maximum);
                }
            });
        }

        private IEnumerable<object[]> GetInvalidProperyValues()
        {
            yield return new object[] { Int32.MinValue, 10, 1 };
            yield return new object[] { Int32.MaxValue, 10, 1 };
            yield return new object[] { 10, Int32.MinValue, 1 };
            yield return new object[] { 10, Int32.MaxValue, 1 };
            yield return new object[] { 10, 5, 1, }; // Minimum greater than maximum!
            yield return new object[] { 10, 20, -1 }; // Negative count!
        }

        [Test, Factory("GetInvalidProperyValues")]
        public void Constructs_with_invalid_property_should_throw_exception(int minimum, int maximum, int count)
        {
            var generator = new RandomInt32Generator
            {
                Minimum = minimum,
                Maximum = maximum,
                Count = count
            };

            Assert.Throws<GenerationException>(() => generator.Run().Cast<int>().ToArray());
        }

        [Test]
        public void Generate_filtered_sequence()
        {
            var generator = new RandomInt32Generator
            {
                Minimum = 0,
                Maximum = 100,
                Count = 50,
                Filter = d => (d % 2) == 0
            };

            var values = generator.Run().Cast<int>().ToArray();
            Assert.AreEqual(50, values.Length);
            Assert.Multiple(() =>
            {
                foreach (int value in values)
                {
                    Assert.AreEqual(0, value % 2);
                }
            });
        }
    }
}
