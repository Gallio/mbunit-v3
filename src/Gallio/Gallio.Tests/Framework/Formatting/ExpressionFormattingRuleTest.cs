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
using System.Linq.Expressions;
using Gallio.Framework.Formatting;
using Gallio.Model.Diagnostics;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Formatting
{
    [TestFixture]
    [TestsOn(typeof(ExpressionFormattingRule))]
    public class ExpressionFormattingRuleTest : BaseFormattingRuleTest<ExpressionFormattingRule>
    {
        // Note: The formatting of constants in the expressions depends on other formatters.
        //       Since they are not registered here, they appear as bracketed object values, eg. {5}.
        [Test]
        public void SimpleExpressions()
        {
            NewAssert.Multiple(() =>
            {
                int x = 5, y = 2;
                int[] arr = new int[1];
                UnaryPlusType z = new UnaryPlusType();

                // binary operators
                AssertFormat(() => x + y, "() => x + y");
                AssertFormat(() => checked(x + y), "() => checked(x + y)");
                AssertFormat(() => x & y, "() => x & y");
                AssertFormat(() => true && x == 5, "() => {True} && x == {5}");
                AssertFormat(() => arr[0], "() => arr[{0}]");
                AssertFormat(() => arr ?? arr, "() => arr ?? arr");
                AssertFormat(() => x / y, "() => x / y");
                AssertFormat(() => x == y, "() => x == y");
                AssertFormat(() => x ^ y, "() => x ^ y");
                AssertFormat(() => x > y, "() => x > y");
                AssertFormat(() => x >= y, "() => x >= y");
                AssertFormat(() => ((Func<int>) (() => 5))(), "() => (({Gallio.Func`1[System.Int32]}) (() => {5}))()");
                AssertFormat(() => x << y, "() => x << y");
                AssertFormat(() => x < y, "() => x < y");
                AssertFormat(() => x <= y, "() => x <= y");
                AssertFormat(() => x % y, "() => x % y");
                AssertFormat(() => x * y, "() => x * y");
                AssertFormat(() => checked(x * y), "() => checked(x * y)");
                AssertFormat(() => x != y, "() => x != y");
                AssertFormat(() => x | y, "() => x | y");
                AssertFormat(() => false || x == 5, "() => {False} || x == {5}");
                AssertFormat(Expression.Lambda<System.Func<double>>(Expression.Power(Expression.Constant(3.0),
                        Expression.Constant(4.0))), "() => {3} ** {4}");
                AssertFormat(() => x >> y, "() => x >> y");
                AssertFormat(() => x - y, "() => x - y");
                AssertFormat(() => checked(x - y), "() => checked(x - y)");

                // call
                AssertFormat(() => arr.ToString(), "() => arr.ToString()");

                // conditional
                AssertFormat(() => x == 3 ? 1 : 2, "() => x == {3} ? {1} : {2}");

                // lambda (done elsewhere)

                // list init
                AssertFormat(() => new List<int> {1, 2, 3}, "() => new List`1() { {1}, {2}, {3} }");

                // member init
                AssertFormat(() => new MemberInitType {Bar = 42, List = {1, 2, 3}, Aggregate = {Foo = 42}},
                    "() => new MemberInitType() { Bar = {42}, List = { {1}, {2}, {3} }, Aggregate = { Foo = {42} } }");

                // member access (done elsewhere)

                // new
                AssertFormat(() => new MemberInitType(), "() => new MemberInitType()");

                // new array bounds
                AssertFormat(() => new int[3], "() => new {System.Int32}[{3}]");

                // new array init
                AssertFormat(() => new int[] {1, 2, 3}, "() => new {System.Int32}[] { {1}, {2}, {3} }");

                // parameter
                AssertFormat((int i) => i == 2, "i => i == {2}");

                // quote
                AssertFormat(() => AssertFormat(() => x == 5, "() => x == {5}"),
                    "() => {Gallio.Tests.Framework.Formatting.ExpressionFormattingRuleTest}.AssertFormat((() => x == {5}), {() => x == {5}})");

                // type binary
                AssertFormat(() => (object) x is int, "() => ({System.Object}) x is {System.Int32}");

                // unary
                AssertFormat(() => arr.Length, "() => arr.Length");
                AssertFormat(() => (double) x, "() => ({System.Double}) x");
                AssertFormat(() => checked((double)x), "() => checked(({System.Double}) x)");
                AssertFormat(() => -x, "() => - x");
                AssertFormat(() => checked(-x), "() => checked(- x)");
                AssertFormat(() => ~x, "() => ~ x");
                AssertFormat(() => x as object, "() => x as {System.Object}");
                AssertFormat(() => +z, "() => + z");
            });
        }

        [Test]
        public void Precedence()
        {
            int x = 5, y = 2;
            AssertFormat(() => x + y * x, "() => x + y * x");
            AssertFormat(() => (x + y) * x, "() => (x + y) * x");
            AssertFormat(() => y * x + y, "() => y * x + y");
            AssertFormat(() => y * (x + y), "() => y * (x + y)");
        }

        [Test]
        public void CheckedAndUnchecked()
        {
            int x = 5, y = 2;
            AssertFormat(() => checked(x + y + x), "() => checked(x + y + x)");
            AssertFormat(() => checked(x + unchecked(y + x)), "() => checked(x + unchecked(y + x))");
            AssertFormat(() => checked(x + unchecked(y + x) * x), "() => checked(x + unchecked(y + x) * x)");
            AssertFormat(() => checked(x + unchecked(y + x) * x) + y, "() => checked(x + unchecked(y + x) * x) + y");
            AssertFormat(() => checked(x + unchecked(y + checked(x * x)) * x) + y, "() => checked(x + unchecked(y + checked(x * x)) * x) + y");
        }

        [TestFrameworkInternal]
        private void AssertFormat(Expression<System.Action> expr, string expectedFormat)
        {
            NewAssert.AreEqual(expectedFormat, Formatter.Format(expr));
        }

        [TestFrameworkInternal]
        private void AssertFormat<T>(Expression<System.Func<T>> expr, string expectedFormat)
        {
            NewAssert.AreEqual(expectedFormat, Formatter.Format(expr));
        }

        [TestFrameworkInternal]
        private void AssertFormat<TArg, TResult>(Expression<System.Func<TArg, TResult>> expr, string expectedFormat)
        {
            NewAssert.AreEqual(expectedFormat, Formatter.Format(expr));
        }

        [Test]
        [Row(typeof(Expression), FormattingRulePriority.Best)]
        [Row(typeof(Expression<Func<int>>), FormattingRulePriority.Best)]
        [Row(typeof(string), null)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }

        private struct UnaryPlusType
        {
            public static UnaryPlusType operator +(UnaryPlusType x)
            {
                return x;
            }
        }

        private class MemberInitType
        {
            public int Bar = 0;
            public AggregateType Aggregate = new AggregateType();
            public List<int> List = new List<int>();
        }

        private class AggregateType
        {
            public int Foo = 0;
        }
    }
}