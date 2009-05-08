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
using Gallio.Runtime.Formatting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Formatting
{
    [TestFixture]
    [TestsOn(typeof(SingleFormattingRule))]
    public class SingleFormattingRuleTest : BaseFormattingRuleTest<SingleFormattingRule>
    {
        [Test]
        [Row(5.5f, "5.5f")]
        [Row(0f, "0f")]
        [Row(-1.2f, "-1.2f")]
        public void Format(float value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        [Row(typeof(float), FormattingRulePriority.Best)]
        [Row(typeof(string), null)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }
    }
}