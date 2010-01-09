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


using Gallio.Model;
using Gallio.Model.Filters;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(FilterFormatter))]
    public class FilterFormatterTest
    {
        [Test]
        [Row("*")]
        [Row("Key: /reg ex/")]
        [Row("Key: /reg ex/i")]
        [Row("Key: 'value 1', value2, /value3/, /value\\/4/i, 'value \"5', 'value \\'6'")]
        [Row("(Assembly: 'foo, bar' and Id: SomeId and Member: Member and Key: 'This Value' and Namespace: Foo.Bar and Type: This.Type)")]
        [Row("not (Abc: 123 and not (Def: 456 or not Ghi: 789))")]
        [Row("not (not not (((Abc: def))))")]
        [Row(@"Abc: /123 456 \/ 789/i")]        
        public void RoundTripFormatting(string filterExpr)
        {
            Filter<ITestDescriptor> filter = FilterUtils.ParseTestFilter(filterExpr);
                        
            string formattedFilterExpression = filter.ToFilterExpr();
            Filter<ITestDescriptor> filterFromFormattedFilterExpression = FilterUtils.ParseTestFilter(formattedFilterExpression);

            // The exact filter expression may be different (redundant parentheses are lost for
            // example), so we compare the final structure of the filters created by parsing the
            // original expression and the formatted filter expression
            Assert.AreEqual(filterFromFormattedFilterExpression.ToString(), filter.ToString());
        }
    }
}
