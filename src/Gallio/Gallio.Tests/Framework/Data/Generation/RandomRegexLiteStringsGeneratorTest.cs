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

namespace Gallio.Tests.Framework.Data.Generation
{
    [TestFixture]
    [TestsOn(typeof(RandomRegexLiteStringsGenerator))]
    public class RandomRegexLiteStringsGeneratorTest
    {
        [Test]
        [Row(@"[0-9]{4,10}", @"^[0-9]{4,10}$", 100)]
        public void Generate_sequence_ok(string pattern, string expected, int count)
        {
            var generator = new RandomRegexLiteStringsGenerator
            {
                RegularExpressionPattern = pattern,
                Count = count
            };

            var values = generator.Run().Cast<string>().ToArray();
            Assert.AreEqual(count, values.Length);
            Assert.Multiple(() =>
            {
                foreach (string value in values)
                {
                    Assert.FullMatch(value, expected);
                }
            });
        }

        private IEnumerable<object[]> GetInvalidProperyValues()
        {
            yield return new object[] { null, 1 };
            yield return new object[] { "[A-Z", 1, }; // Invalid regular expression!
            yield return new object[] { "ABC", -1 }; // Negative count!
        }

        [Test, Factory("GetInvalidProperyValues")]
        public void Constructs_with_invalid_property_should_throw_exception(string pattern, int count)
        {
            var generator = new RandomRegexLiteStringsGenerator
            {
                RegularExpressionPattern = pattern,
                Count = count
            };

            Assert.Throws<GenerationException>(() => generator.Run().Cast<string>().ToArray());
        }
    }
}
