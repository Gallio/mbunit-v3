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
using Gallio.Framework.Formatting;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Formatting
{
    [TestFixture]
    [TestsOn(typeof(IntegerFormattingRule))]
    public class IntegerFormattingRuleTest : BaseFormattingRuleTest<IntegerFormattingRule>
    {
        [Test]
        [Row(typeof(short), 42, "42")]
        [Row(typeof(int), 42, "42")]
        [Row(typeof(long), 42, "42")]
        [Row(typeof(ushort), 42, "42")]
        [Row(typeof(uint), 42, "42")]
        [Row(typeof(ulong), 42, "42")]
        [Row(typeof(short), -42, "-42")]
        [Row(typeof(int), -42, "-42")]
        [Row(typeof(long), -42, "-42")]
        public void Format<T>(T value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        [Row(typeof(short), FormattingRulePriority.Best)]
        [Row(typeof(int), FormattingRulePriority.Best)]
        [Row(typeof(long), FormattingRulePriority.Best)]
        [Row(typeof(ushort), FormattingRulePriority.Best)]
        [Row(typeof(uint), FormattingRulePriority.Best)]
        [Row(typeof(ulong), FormattingRulePriority.Best)]
        [Row(typeof(string), null)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }
    }
}