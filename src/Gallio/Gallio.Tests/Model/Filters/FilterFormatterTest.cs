extern alias MbUnit2;

using Gallio.Model;
using Gallio.Model.Filters;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(FilterFormatter))]
    public class FilterFormatterTest
    {
        [RowTest]
        [Row("*")]
        [Row("Key: /reg ex/")]
        [Row("Key: /reg ex/i")]
        [Row("Key: 'value 1', value2, /value3/, /value\\/4/i, 'value \"5', 'value \\'6'")]
        [Row("(Assembly: 'foo, bar' and Id: SomeId and Member: Member and Key: 'This Value' and Namespace: Foo.Bar and Type: This.Type)")]
        [Row("not (Abc: 123 and not (Def: 456 or not Ghi: 789)")]
        [Row("not (not not (((Abc: def))))")]
        public void RoundTripFormatting(string filterExpr)
        {
            Filter<ITest> filter = FilterUtils.ParseTestFilter(filterExpr);
            Assert.AreEqual(filterExpr, filter.ToFilterExpr());
        }
    }
}
