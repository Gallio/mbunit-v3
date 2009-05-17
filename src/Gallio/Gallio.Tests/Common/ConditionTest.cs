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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Common
{
    [TestsOn(typeof(Condition))]
    public class ConditionTest
    {
        [Test]
        public void Parse_WhenExpressionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Condition.Parse(null));
        }

        [Test]
        public void Evaluate_WhenContextIsNull_Throws()
        {
            var condition = Condition.Parse("${namespace:identifier}");

            Assert.Throws<ArgumentNullException>(() => condition.Evaluate(null));
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void Parse_WhenPropertyExistenceConditionProvided_ReturnsAPropertyCondition(bool satisfied)
        {
            var context = MockRepository.GenerateMock<MockableConditionContext>();
            context.Stub(x => x.HasPropertyImplMock("namespace", "identifier")).Return(satisfied);

            var condition = Condition.Parse("${namespace:identifier}");

            Assert.Multiple(() =>
            {
                Assert.AreEqual("${namespace:identifier}", condition.ToString());
                Assert.AreEqual(satisfied, condition.Evaluate(context));
            });
        }

        [Test]
        public void Parse_WhenOrConditionProvided_ReturnsAnOrCondition(
            [Column(false, true)] bool x,
            [Column(false, true)] bool y,
            [Column(false, true)] bool z)
        {
            var context = MockRepository.GenerateMock<MockableConditionContext>();
            context.Stub(c => c.HasPropertyImplMock("x", "long")).Return(x);
            context.Stub(c => c.HasPropertyImplMock("long", "y")).Return(y);
            context.Stub(c => c.HasPropertyImplMock("z", "q")).Return(z);

            var condition = Condition.Parse("${x:long} or ${long:y} or ${z:q}");

            Assert.Multiple(() =>
            {
                Assert.AreEqual("${x:long} or ${long:y} or ${z:q}", condition.ToString());
                Assert.AreEqual(x || y || z, condition.Evaluate(context));
            });
        }

        [Test]
        [Row("", "Incomplete conditional expression.")]
        [Row("or", "Did not expect 'or' in this context.")]
        [Row("${}", "Expected property expression of form '${namespace:identifier}' got '${}' instead.")]
        [Row("${:}", "Expected property expression of form '${namespace:identifier}' got '${:}' instead.")]
        [Row("${x:}", "Expected property expression of form '${namespace:identifier}' got '${x:}' instead.")]
        [Row("${:y}", "Expected property expression of form '${namespace:identifier}' got '${:y}' instead.")]
        [Row("${x:y} invalid combinator", "Did not expect 'invalid' in this context.")]
        public void Parse_WhenConditionMalformed_Throws(string expression, string message)
        {
            var ex = Assert.Throws<FormatException>(() => Condition.Parse(expression));
            Assert.AreEqual(message, ex.Message);
        }
    }
}
