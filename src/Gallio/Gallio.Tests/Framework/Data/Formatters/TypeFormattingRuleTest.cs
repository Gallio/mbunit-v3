using System;
using System.Collections.Generic;
using Gallio.Framework.Data.Formatters;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data.Formatters
{
    [TestFixture]
    [TestsOn(typeof(TypeFormattingRule))]
    public class TypeFormattingRuleTest : BaseFormattingRuleTest<TypeFormattingRule>
    {
        [Test]
        [Row(typeof(string), "System.String")]
        [Row(typeof(int[]), "System.Int32[]")]
        [Row(typeof(Dictionary<,>.Enumerator), "System.Collections.Generic.Dictionary`2+Enumerator")]
        public void Format(string value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        [Row(typeof(Type), FormattingRulePriority.Best)]
        [Row(typeof(int), null)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }
    }
}