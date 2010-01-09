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
    [TestsOn(typeof(ElementLiteral))]
    public class ElementLiteralTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_quantifier_should_throw_exception()
        {
            new ElementLiteral(null, "Hello");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_literal_should_throw_exception()
        {
            new ElementLiteral(Quantifier.One, null);
        }

        [Test]
        [Row(1, 3, "^(Hello){1,3}$")]
        [Row(1, 1, "^Hello$")]
        [Row(0, 0, "^$")]
        public void GetRandomString(int minimum, int maximum, string expected)
        {
            var element = new ElementLiteral(new Quantifier(minimum, maximum), "Hello");

            for (int i = 0; i < 10; i++)
            {
                string actual = element.GetRandomString();
                Assert.FullMatch(actual, expected);
            }
        }
    }
}
