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
            var generator = new RandomInt32Generator(minimum, maximum, count);
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

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_count_argument_should_throw_exception()
        {
            new RandomInt32Generator(0, 10, -1);
        }

        [Test]
        [ExpectedArgumentException]
        [Row(Int32.MinValue, 10)]
        [Row(Int32.MaxValue, 10)]
        [Row(10, Int32.MinValue)]
        [Row(10, Int32.MaxValue)]
        [Row(10, 5)]
        public void Constructs_with_invalid_range_should_throw_exception(int minimum, int maximum)
        {
            new RandomInt32Generator(minimum, maximum, 10);
        }
    }
}
