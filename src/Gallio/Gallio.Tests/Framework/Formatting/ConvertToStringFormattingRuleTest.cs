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
using Gallio.Framework.Conversions;
using Gallio.Framework.Formatting;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Formatting
{
    [TestFixture]
    [TestsOn(typeof(ConvertToStringFormattingRule))]
    public class ConvertToStringFormattingRuleTest : BaseTestWithMocks
    {
        [Test]
        [Row(null, null)]
        [Row("", "{}")]
        [Row("abc", "{abc}")]
        [Row("abc\ndef", "{abc\\ndef}")] // note using literal format
        public void Format(string simulatedConversionResult, string expectedResult)
        {
            object value = new object();
            IConverter converter = Mocks.StrictMock<IConverter>();
            using (Mocks.Record())
            {
                Expect.Call(converter.Convert(value, typeof(string))).Return(simulatedConversionResult);
            }

            ConvertToStringFormattingRule formattingRule = new ConvertToStringFormattingRule(converter);
            Assert.AreEqual(expectedResult, formattingRule.Format(value, Mocks.Stub<IFormatter>()));
        }

        [Test]
        [Row(typeof(object), 10000, FormattingRulePriority.Fallback, Description = "Object conversion with typical cost, default ToString.")]
        [Row(typeof(object), 1000000, FormattingRulePriority.Default, Description = "Object conversion with default cost, default ToString.")]
        [Row(typeof(object), 100000000, FormattingRulePriority.Default, Description = "Object conversion with maximum cost, default ToString.")]
        [Row(typeof(ClassWithDefaultToString), 10000, FormattingRulePriority.Fallback, Description = "Object conversion with typical cost, default ToString.")]
        [Row(typeof(ClassWithDefaultToString), 1000000, FormattingRulePriority.Default, Description = "Object conversion with default cost, default ToString.")]
        [Row(typeof(ClassWithDefaultToString), 100000000, FormattingRulePriority.Default, Description = "Object conversion with maximum cost, default ToString.")]
        [Row(typeof(ValueType), 10000, FormattingRulePriority.Fallback, Description = "Object conversion with typical cost, default ToString.")]
        [Row(typeof(ValueType), 1000000, FormattingRulePriority.Default, Description = "Object conversion with default cost, default ToString.")]
        [Row(typeof(ValueType), 100000000, FormattingRulePriority.Default, Description = "Object conversion with maximum cost, default ToString.")]
        [Row(typeof(StructWithDefaultToString), 10000, FormattingRulePriority.Fallback, Description = "Object conversion with typical cost, default ToString.")]
        [Row(typeof(StructWithDefaultToString), 1000000, FormattingRulePriority.Default, Description = "Object conversion with default cost, default ToString.")]
        [Row(typeof(StructWithDefaultToString), 100000000, FormattingRulePriority.Default, Description = "Object conversion with maximum cost, default ToString.")]
        [Row(typeof(string), 0, FormattingRulePriority.Fallback, Description = "Object conversion with zero cost, non-default ToString.")]
        [Row(typeof(DateTime), 10000, FormattingRulePriority.Fallback, Description = "Object conversion with typical cost, non-default ToString.")]
        [Row(typeof(DateTime), 1000000, FormattingRulePriority.Fallback, Description = "Object conversion with default cost, non-default ToString.")]
        [Row(typeof(DateTime), 100000000, FormattingRulePriority.Fallback, Description = "Object conversion with maximum cost, non-default ToString.")]
        public void GetPriority(Type type, int simulatedConversionCost, int? expectedPriority)
        {
            IConverter converter = Mocks.StrictMock<IConverter>();
            using (Mocks.Record())
            {
                Expect.Call(converter.GetConversionCost(type, typeof(string))).Return(new ConversionCost(simulatedConversionCost));
            }

            ConvertToStringFormattingRule formattingRule = new ConvertToStringFormattingRule(converter);
            Assert.AreEqual(expectedPriority, formattingRule.GetPriority(type));
        }

        private class ClassWithDefaultToString
        {
        }

        private struct StructWithDefaultToString
        {
        }
    }
}