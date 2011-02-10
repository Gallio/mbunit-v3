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
using Gallio.Common.Text.RegularExpression;

namespace Gallio.Tests.Common.Text.RegularExpression
{
    [TestFixture]
    [TestsOn(typeof(ElementSet))]
    public class ElementSetTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_quantifier_should_throw_exception()
        {
            new ElementSet(null, "ABC");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_raw_string_should_throw_exception()
        {
            new ElementSet(Quantifier.One, (string)null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_characters_should_throw_exception()
        {
            new ElementSet(Quantifier.One, (char[])null);
        }

        [Test]
        [Row(1, 3, "ABC", "^[ABC]{1,3}$")]
        [Row(1, 1, "ABC", "^[ABC]$")]
        [Row(0, 0, "ABC", "^$")]
        [Row(1, 3, "A-Z", "^[A-Z]{1,3}$")]
        [Row(1, 1, "A-Z", "^[A-Z]$")]
        [Row(1, 1, "A-D0-5", "^[A-D0-5]$")]
        [Row(1, 1, "ABCM-P0-39", "^[ABCM-P0-39]$")]
        public void GetRandomString(int minimum, int maximum, string raw, string expected)
        {
            var element = new ElementSet(new Quantifier(minimum, maximum), raw);
            var random = new Random();

            for (int i = 0; i < 10; i++)
            {
                string actual = element.GetRandomString(random);
                TestLog.WriteLine(actual);
                Assert.FullMatch(actual, expected);
            }
        }

        [Test]
        [ExpectedException(typeof(RegexLiteException))]
        public void Initialize_with_invalid_content_should_throw_exception()
        {
            new ElementSet(Quantifier.One, "Z-A");
        }

    }
}
