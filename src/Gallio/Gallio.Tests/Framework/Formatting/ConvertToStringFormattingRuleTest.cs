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
using Gallio.Framework.Conversions;
using Gallio.Framework.Formatting;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Formatting
{
    [TestFixture]
    [TestsOn(typeof(ConvertToStringFormattingRule))]
    public class ConvertToStringFormattingRuleTest : BaseUnitTest
    {
        [Test]
        [Row("", null)]
        [Row(null, null)]
        [Row("abc", "{abc}")]
        [Row("abc\ndef", "{abc\\ndef}")] // note using literal format
        public void Format(string simulatedConversionResult, string expectedResult)
        {
            object value = new object();
            IConverter converter = Mocks.CreateMock<IConverter>();
            using (Mocks.Record())
            {
                Expect.Call(converter.Convert(value, typeof(string))).Return(simulatedConversionResult);
            }

            ConvertToStringFormattingRule formattingRule = new ConvertToStringFormattingRule(converter);
            Assert.AreEqual(expectedResult, formattingRule.Format(value, Mocks.Stub<IFormatter>()));
        }

        [Test]
        [Row(typeof(object), FormattingRulePriority.Default)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            ConvertToStringFormattingRule formattingRule = new ConvertToStringFormattingRule(Mocks.Stub<IConverter>());
            Assert.AreEqual(expectedPriority, formattingRule.GetPriority(type));
        }
    }
}