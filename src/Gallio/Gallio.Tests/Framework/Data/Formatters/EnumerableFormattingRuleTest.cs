// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections;
using System.Collections.Generic;
using Gallio.Framework.Data.Formatters;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data.Formatters
{
    [TestFixture]
    [TestsOn(typeof(EnumerableFormattingRule))]
    public class EnumerableFormattingRuleTest : BaseFormattingRuleTest<EnumerableFormattingRule>
    {
        [Test]
        [Row(new int[] { 1, 2, 3 }, "[{1}, {2}, {3}]")]
        [Row(new object[] { "a", new int[] { 2, 3 }, "bc" }, "[[{a}], [{2}, {3}], [{b}, {c}]]",
            Description="A compound array.  Note that the strings are considered enumerable as character strings and there is no string formatting override in the context of this test so they are represented as char enumerations here.")]
        public void Format(object value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        [Row(typeof(IEnumerable), FormattingRulePriority.Typical)]
        [Row(typeof(IEnumerable<int>), FormattingRulePriority.Typical)]
        [Row(typeof(int[]), FormattingRulePriority.Typical)]
        [Row(typeof(ArrayList), FormattingRulePriority.Typical)]
        [Row(typeof(List<int>), FormattingRulePriority.Typical)]
        [Row(typeof(int), null)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }
    }
}