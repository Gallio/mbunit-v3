using System;
using Gallio.Framework.Data.Conversions;
using Gallio.Framework.Data.Formatters;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data.Formatters
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