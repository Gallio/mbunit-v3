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
using Gallio.Common.Text.RegularExpression;
using System.Diagnostics;

namespace Gallio.Tests.Common.Text.RegularExpression
{
    [TestFixture]
    [TestsOn(typeof(RegexLite))]
    public class RegexLiteTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_input_should_throw_exception()
        {
            new RegexLite(null);
        }

        [Test]
        [Row(@"ABCD", @"^ABCD$", Description = "Simple litteral")]
        [Row(@"A(BC)D", @"^ABCD$", Description = "Litteral with basic neutral group")]
        [Row(@"(A)(()B((C())))D", @"^ABCD$", Description = "Literal with crazy neutral groups")]
        [Row(@"AB((CD)", null, Description = "litteral with missing closing parenthesis", ExpectedException = typeof(RegexLiteException))]
        [Row(@"AB(\(CD)", @"^AB\(CD$", Description = "Simple litteral with escaped parenthesis")]
        [Row(@"[ABC]", @"^[ABC]$", Description = "Simple set")]
        [Row(@"[A-Z]", @"^[A-Z]$", Description = "Simple set with range")]
        [Row(@"[A-D0-5]", @"^[A-D0-5]$", Description = "Set with 2 ranges")]
        [Row(@"H[ea]llo", @"^H[ea]llo$", Description = "Hallo/Hello")]
        [Row(@"AB?C", @"^AB?C$", Description = "Optional character")]
        [Row(@"[C?]", @"^C|\?$", Description = "Metacharacters in sets should be ignored")]
        [Row(@"(ABC)?", @"^(ABC)?$", Description = "Optional group")]
        [Row(@"(ABC){2}", @"^ABCABC$", Description = "Quantified group")]
        [Row(@"(ABC){1,2}", @"^(ABC){1,2}$", Description = "Quantified group")]
        [Row(@"[ABC]?", @"^[ABC]?$", Description = "Optional set")]
        [Row(@"[ABC]{2}", @"^[ABC]{2}$", Description = "Quantified set")]
        [Row(@"[ABC]{1,2}", @"^[ABC]{1,2}$", Description = "Quantified set")]
        [Row(@"A{1,3}B{4}C?", @"^A{1,3}B{4}C?$", Description = "Various flat quantifiers")]
        [Row(@"(AB?){2}", @"^(AB?){2}$", Description = "Complex case")]
        [Row(@"([AB]X){2}", @"^([AB]X){2}$", Description = "Complex case")]
        [Row(@"A(B?[0-3]{1,2})?", @"^A(B?[0-3]{1,2})?$", Description = "Complex case")]
        public void Construct_with_pattern(string input, string expected)
        {
            var pattern = new RegexLite(input);
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    string actual = pattern.GetRandomString();
                    TestLog.WriteLine(actual);
                    Assert.FullMatch(actual, expected);
                }
            });
        }
    }
}
