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
        public void Generate_sequence_ok(double minimum, double maximum, int count)
        {
            var generator = new RandomNumbersGenerator(minimum, maximum, count);
            var values = generator.Run().Cast<double>().ToArray();
            Assert.AreEqual(count, values.Length);
            Assert.Multiple(() =>
            {
                foreach (double value in values)
                {
                    Assert.Between(value, minimum, maximum);
                }
            });
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_count_argument_should_throw_exception()
        {
            new RandomNumbersGenerator(0, 10, -1);
        }

        [Test]
        [ExpectedArgumentException]
        [Row(Double.MinValue, 10)]
        [Row(Double.MaxValue, 10)]
        [Row(Double.PositiveInfinity, 10)]
        [Row(Double.NegativeInfinity, 10)]
        [Row(Double.NaN, 10)]
        [Row(10, Double.MinValue)]
        [Row(10, Double.MaxValue)]
        [Row(10, Double.PositiveInfinity)]
        [Row(10, Double.NegativeInfinity)]
        [Row(10, Double.NaN)]
        [Row(10, 5)]
        public void Constructs_with_invalid_range_should_throw_exception(double minimum, double maximum)
        {
            new RandomNumbersGenerator(minimum, maximum, 10);
        }
    }
}
