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
    [TestsOn(typeof(RandomStockStringGenerator))]
    public class RandomStockStringGeneratorTest
    {
        [Test]
        public void Generate_sequence_ok()
        {
            var generator = new RandomStockStringGenerator
            {
                Values = new[] { "A", "B", "C", "D" },
                Count = 8
            };

            var values = generator.Run().Cast<string>().ToArray();
            Assert.Count(8, values);
            Assert.Multiple(() =>
            {
                foreach (string value in values)
                {
                    Assert.Contains<string>(new[] { "A", "B", "C", "D" }, value);
                }
            });
        }

        [Test]
        public void Constructs_with_negative_count_should_throw_exception()
        {
            var generator = new RandomStockStringGenerator
            {
                Values = new[] { "A", "B", "C", "D" },
                Count = -1
            };

            Assert.Throws<GenerationException>(() => generator.Run().Cast<string>().ToArray());
        }
    }
}
