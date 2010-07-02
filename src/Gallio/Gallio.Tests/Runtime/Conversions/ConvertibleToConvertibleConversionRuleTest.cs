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

using Gallio.Runtime.Conversions;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Conversions
{
    [TestFixture]
    [TestsOn(typeof(ConvertibleToConvertibleConversionRule))]
    public class ConvertibleToConvertibleConversionRuleTest : BaseConversionRuleTest<ConvertibleToConvertibleConversionRule>
    {
        [Test]
        public void TransitiveConversion()
        {
            string targetValue = (string)Converter.Convert(42, typeof(string));
            Assert.AreEqual("42", targetValue);
        }

        [Test]
        public void UnsupportedConversion()
        {
            Assert.IsFalse(Converter.CanConvert(typeof(int[]), typeof(int)));
            Assert.IsFalse(Converter.CanConvert(typeof(int), typeof(int[])));
        }

        [Test]
        [Row("42", 42)]
        [Row("", null)]
        [Row(null, null)]
        public void ConvertNullToNullableValue(string sourceValue, int? expectedValue)
        {
            Assert.IsTrue(Converter.CanConvert(typeof(string), typeof(int?)));
            var actualValue = (int?)Converter.Convert(sourceValue, typeof(int?));
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}