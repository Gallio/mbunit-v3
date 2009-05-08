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
using System.Linq.Expressions;
using System.Text;
using Gallio.Framework;
using Gallio.Runtime.Formatting;
using Gallio.Common.Linq;
using Gallio.Runtime.Diagnostics;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Linq
{
    [TestsOn(typeof(ExpressionInstrumentor))]
    public class ExpressionInstrumentorTest
    {
        [Test, ExpectedArgumentNullException]
        public void RewriteThrowsIfArgumentIsNull()
        {
            new ExpressionTracer().Rewrite<Expression<Gallio.Common.Func<int>>>(null);
        }

        [Test]
        public void AllExpressionTypes()
        {
            Assert.Multiple(() =>
            {
                int x = 5, y = 2;
                int[] arr = new int[1];
                UnaryPlusType z = new UnaryPlusType();

                // binary operators
                AssertTrace(() => x + y, 7,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "Add" });
                AssertTrace(() => checked(x + y), 7,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "AddChecked" });
                AssertTrace(() => x & y, 0,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "And" });
                AssertTrace(() => true && x == 5, true,
                    new[] { "Constant", "Constant", "MemberAccess", "Constant", "Equal", "AndAlso" });
                AssertTrace(() => arr[0], 0,
                    new[] { "Constant", "MemberAccess", "Constant", "ArrayIndex" });
                AssertTrace(() => arr ?? arr, arr,
                    new[] { "Constant", "MemberAccess", "Coalesce" });
                AssertTrace(() => x / y, 2,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "Divide" });
                AssertTrace(() => x == y, false,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "Equal" });
                AssertTrace(() => x ^ y, 7,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "ExclusiveOr" });
                AssertTrace(() => x > y, true,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "GreaterThan" });
                AssertTrace(() => x >= y, true,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "GreaterThanOrEqual" });
                AssertTrace(() => ((Gallio.Common.Func<int>)(() => 5))(), 5,
                    new[] { "Lambda", "Convert", "Invoke" });
                AssertTrace(() => x << y, 20,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "LeftShift" });
                AssertTrace(() => x < y, false,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "LessThan" });
                AssertTrace(() => x <= y, false,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "LessThanOrEqual" });
                AssertTrace(() => x % y, 1,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "Modulo" });
                AssertTrace(() => x * y, 10,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "Multiply" });
                AssertTrace(() => checked(x * y), 10,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "MultiplyChecked" });
                AssertTrace(() => x != y, true,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "NotEqual" });
                AssertTrace(() => x | y, 7,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "Or" });
                AssertTrace(() => false || x == 5, true,
                    new[] { "Constant", "Constant", "MemberAccess", "Constant", "Equal", "OrElse" });
                AssertTrace(Expression.Lambda<System.Func<double>>(Expression.Power(Expression.Constant(3.0), Expression.Constant(4.0))), 81.0,
                    new[] { "Constant", "Constant", "Power" });
                AssertTrace(() => x >> y, 1,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "RightShift" });
                AssertTrace(() => x - y, 3,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "Subtract" });
                AssertTrace(() => checked(x - y), 3,
                    new[] { "Constant", "MemberAccess", "Constant", "MemberAccess", "SubtractChecked" });

                // call
                AssertTrace(() => arr.ToString(), arr.ToString(),
                    new[] { "Constant", "MemberAccess", "Call" });

                // conditional
                AssertTrace(() => x == 3 ? 1 : 2, 2,
                    new[] { "Constant", "MemberAccess", "Constant", "Equal", "Constant", "Conditional" });

                // lambda (done elsewhere)

                // list init
                AssertTrace(() => new List<int> { 1, 2, 3 }.Count, 3,
                    new[] { "Constant", "Constant", "Constant", "ListInit", "MemberAccess" });

                // member init
                AssertTrace(() => new MemberInitType { Bar = 42, List = { 1, 2, 3 }, Aggregate = { Foo = 42 } }.Bar, 42,
                    new[] { "Constant", "Constant", "Constant", "Constant", "Constant", "MemberInit", "MemberAccess" });
                AssertTrace(() => new MemberInitType { Bar = 42, List = { 1, 2, 3 }, Aggregate = { Foo = 42 } }.List.Count, 3,
                    new[] { "Constant", "Constant", "Constant", "Constant", "Constant", "MemberInit", "MemberAccess", "MemberAccess" });
                AssertTrace(() => new MemberInitType { Bar = 42, List = { 1, 2, 3 }, Aggregate = { Foo = 42 } }.Aggregate.Foo, 42,
                    new[] { "Constant", "Constant", "Constant", "Constant", "Constant", "MemberInit", "MemberAccess", "MemberAccess" });
                
                // member access (done elsewhere)

                // new
                AssertTrace(() => new MemberInitType().Bar, 0,
                    new[] { "New", "MemberAccess" });

                // new array bounds
                AssertTrace(() => new int[3].Length, 3,
                    new[] { "Constant", "NewArrayBounds", "ArrayLength" });

                // new array init
                AssertTrace(() => new int[] { 1, 2, 3 }.Length, 3,
                    new[] { "Constant", "Constant", "Constant", "NewArrayInit", "ArrayLength" });

                // parameter
                AssertTrace(i => i == 2, 2, true,
                    new[] { "Parameter", "Constant", "Equal" });

                // quote
                AssertTrace(() => AssertTrace(() => x == 5, true, new[] { "Constant", "MemberAccess", "Constant", "Equal" }),
                    new[] { "Quote", "Constant", "Constant", "Constant", "Constant", "Constant", "NewArrayInit", "Call" });

                // type binary
                AssertTrace(() => ((object)x) is int, true,
                    new[] { "Constant", "MemberAccess", "Convert", "TypeIs" });

                // unary
                AssertTrace(() => arr.Length, 1,
                    new[] { "Constant", "MemberAccess", "ArrayLength" });
                AssertTrace(() => (double)x, x,
                    new[] { "Constant", "MemberAccess", "Convert" });
                AssertTrace(() => checked((double)x), x,
                    new[] { "Constant", "MemberAccess", "ConvertChecked" });
                AssertTrace(() => -x, -x,
                    new[] { "Constant", "MemberAccess", "Negate" });
                AssertTrace(() => checked(-x), -x,
                    new[] { "Constant", "MemberAccess", "NegateChecked" });
                AssertTrace(() => ~x, ~x,
                    new[] { "Constant", "MemberAccess", "Not" });
                AssertTrace(() => x as object, x,
                    new[] { "Constant", "MemberAccess", "TypeAs" });
                AssertTrace(() => +z, z,
                    new[] { "Constant", "MemberAccess", "UnaryPlus" });
            });
        }

        [Test]
        public void VoidReturnType()
        {
            AssertTrace(() => TestLog.WriteLine("Foo"), new[] { "Constant", "Call" });
        }

        [SystemInternal]
        private static void AssertTrace<TArg, T>(Expression<System.Func<TArg, T>> expr, TArg arg,
            T expectedValue, string[] expectedTrace)
        {
            var tracer = new ExpressionTracer();
            T actualValue = tracer.Compile(expr)(arg);

            Assert.AreEqual(expectedValue, actualValue, "Expression result should be equal.");
            Assert.Over.Pairs(expectedTrace, tracer.Trace, Assert.AreEqual,
                "Expression trace should be equal.");
        }

        [SystemInternal]
        private static void AssertTrace<T>(Expression<System.Func<T>> expr,
            T expectedValue, string[] expectedTrace)
        {
            using (TestLog.BeginSection(expr.Format()))
            {
                var tracer = new ExpressionTracer();

                TestLog.WriteLine("Rewritten expression: {0}", tracer.Rewrite(expr).Format());
                T actualValue = tracer.Compile(expr)();

                Assert.AreEqual(expectedValue, actualValue, "Expression result should be equal.");
                Assert.Over.Pairs(expectedTrace, tracer.Trace, Assert.AreEqual,
                    "Expression trace should be equal.");
            }
        }

        [SystemInternal]
        private static void AssertTrace(Expression<System.Action> expr, string[] expectedTrace)
        {
            var tracer = new ExpressionTracer();
            tracer.Compile(expr)();

            Assert.Over.Pairs(expectedTrace, tracer.Trace, Assert.AreEqual,
                "Expression trace should be equal.");
        }

        private class ExpressionTracer : ExpressionInstrumentor
        {
            private readonly List<string> trace = new List<string>();

            public IList<string> Trace
            {
                get { return trace; }
            }

            protected override T Intercept<T>(Expression expr, System.Func<T> continuation)
            {
                T value = base.Intercept<T>(expr, continuation);
                TestLog.WriteLine("{0}: {1}", Formatter.Instance.Format(expr), Formatter.Instance.Format(value));

                trace.Add(expr.NodeType.ToString());
                return value;
            }
        }

        private struct UnaryPlusType
        {
            public static UnaryPlusType operator+(UnaryPlusType x)
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