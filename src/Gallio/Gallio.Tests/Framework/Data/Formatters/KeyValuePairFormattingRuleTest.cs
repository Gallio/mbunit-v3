// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Data.Formatters;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data.Formatters
{
    [TestFixture]
    [TestsOn(typeof(KeyValuePairFormattingRule))]
    public class KeyValuePairFormattingRuleTest : BaseFormattingRuleTest<KeyValuePairFormattingRule>
    {
        [Test]
        [Row("abc", 123, "{abc}: {123}")]
        public void Format(object key, object value, string expectedResult)
        {
            KeyValuePair<object, object> entry = new KeyValuePair<object, object>(key, value);
            Assert.AreEqual(expectedResult, Formatter.Format(entry));
        }

        [Test]
        [Row(typeof(KeyValuePair<object, object>), FormattingRulePriority.Best)]
        [Row(typeof(KeyValuePair<int, string>), FormattingRulePriority.Best)]
        [Row(typeof(string), null)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }
    }
}