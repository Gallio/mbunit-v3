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

using System;
using System.Collections.Generic;
using System.Linq;
using Gallio.Framework;
using Gallio.Model.Commands;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(RowAttribute))]
    [TestsOn(typeof(ColumnAttribute))]
    [TestsOn(typeof(BindAttribute))]
    [TestsOn(typeof(CombinatorialJoinAttribute))]
    [TestsOn(typeof(SequentialJoinAttribute))]
    [TestsOn(typeof(PairwiseJoinAttribute))]
    [RunSample(typeof(TypeParameterBindingInsideSample<>))]
    [RunSample(typeof(TypeParameterBindingOutsideSample<>))]
    [RunSample(typeof(ConstructorParameterBindingInsideSample))]
    [RunSample(typeof(ConstructorParameterBindingOutsideSample))]
    [RunSample(typeof(TypeParameterAndConstructorParameterBindingOutsideSample<>))]
    [RunSample(typeof(FieldBindingSample))]
    [RunSample(typeof(PropertyBindingSample))]
    [RunSample(typeof(MethodParameterBindingInsideSample))]
    [RunSample(typeof(MethodParameterBindingOutsideSample))]
    [RunSample(typeof(MethodParameterBindingOutsideSampleInherited))]
    [RunSample(typeof(ExplicitBindingByNameSample<>))]
    [RunSample(typeof(ExplicitBindingByIndexSample<>))]
    [RunSample(typeof(ImplicitBindingByNameSample<>))]
    [RunSample(typeof(ImplicitBindingByIndexOnClassSample<,>))]
    [RunSample(typeof(ImplicitBindingByIndexOnMethodSample))]
    [RunSample(typeof(CombinatorialBindingOfClassAndMethodWithOrderingSample<>))]
    [RunSample(typeof(CombinatorialBindingOfMethodWithMultipleDataSourcesAndOrderingSample))]
    [RunSample(typeof(CombinatorialJoinStrategySample))]
    [RunSample(typeof(SequentialJoinStrategySample))]
    [RunSample(typeof(PairwiseJoinStrategySample))]
    [RunSample(typeof(ConcreteFixtureSample))]
    public class DataBindingTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row(typeof(TypeParameterBindingInsideSample<>), "Test", new[] { "System.Int32" })]
        [Row(typeof(TypeParameterBindingOutsideSample<>), "Test", new[] { "System.String" })]
        [Row(typeof(ConstructorParameterBindingInsideSample), "Test", new[] { "(Apples, 1)" })]
        [Row(typeof(ConstructorParameterBindingOutsideSample), "Test", new[] { "(Apples, 1)" })]
        [Row(typeof(TypeParameterAndConstructorParameterBindingOutsideSample<>), "Test", new[] { "System.String -> (Apples, 1)" })]
        [Row(typeof(FieldBindingSample), "Test", new[] { "(Apples, 1)" })]
        [Row(typeof(PropertyBindingSample), "Test", new[] { "(Apples, 1)" })]
        [Row(typeof(MethodParameterBindingInsideSample), "Test", new[] { "(Apples, 1)" })]
        [Row(typeof(MethodParameterBindingOutsideSample), "Test", new[] { "(Apples, 1)" })]
        [Row(typeof(MethodParameterBindingOutsideSampleInherited), "Test", new[] { "(Apples, 1)", "(Oranges, 2)" }, Description = "Should inherit rows.")]
        [Row(typeof(ExplicitBindingByNameSample<>), "Test", new[] { "System.String -> (Apples, 1) x 10, Empire" })]
        [Row(typeof(ExplicitBindingByIndexSample<>), "Test", new[] { "System.String -> (Apples, 1) x 10, Empire" })]
        [Row(typeof(ImplicitBindingByNameSample<>), "Test", new[] { "System.String -> (Apples, 1) x 10, Empire" })]
        [Row(typeof(ImplicitBindingByIndexOnClassSample<,>), "Test", new[] { "System.String, System.Decimal -> (Apples, 1)" })]
        [Row(typeof(ImplicitBindingByIndexOnMethodSample), "Test", new[] { "System.String, System.Decimal -> (Apples, 1)" })]
        [Row(typeof(CombinatorialBindingOfClassAndMethodWithOrderingSample<>), "Test", new[] {
            "System.String, System.Int32 -> (abc, 456)",
            "System.String, System.String -> (abc, def)",
            "System.Int32, System.Int32 -> (123, 456)",
            "System.Int32, System.String -> (123, def)"
        })]
        [Row(typeof(CombinatorialBindingOfMethodWithMultipleDataSourcesAndOrderingSample), "Test", new[] {
            "System.String, System.Int32 -> (abc, 456)",
            "System.String, System.String -> (abc, def)",
            "System.Int32, System.Int32 -> (123, 456)",
            "System.Int32, System.String -> (123, def)"
        })]
        [Row(typeof(CombinatorialJoinStrategySample), "Test", new[] { "000", "001", "010", "011", "100", "101", "110", "111" })]
        [Row(typeof(SequentialJoinStrategySample), "Test", new[] { "000", "111" })]
        [Row(typeof(PairwiseJoinStrategySample), "Test", new[] { "111", "100", "010", "001" })]
        [Row(typeof(ConcreteFixtureSample), "BaseTest", new[] { "i = 123, s = ABC" })]
        [Row(typeof(ConcreteFixtureSample), "ConcreteTest", new[] { "i = 123, s = ABC" })]
        public void VerifySampleOutput(Type fixtureType, string sampleName, string[] output)
        {
            IList<TestStepRun> runs = Runner
                .GetTestCaseRunsWithin(CodeReference.CreateFromType(fixtureType))
                .Where(run => run.Step.Name.StartsWith(sampleName))
                .ToList();

            Assert.Count(output.Length, runs, "Different number of runs than expected.");

            for (int i = 0; i < output.Length; i++)
                AssertLogContains(runs[i], output[i]);
        }

        [TestFixture, Explicit("Sample")]
        internal class TypeParameterBindingInsideSample<[Column(typeof(int))] T>
        {
            [Test]
            public void Test()
            {
                TestLog.WriteLine(typeof(T));
            }
        }

        [TestFixture, Explicit("Sample")]
        [Row(typeof(string))]
        internal class TypeParameterBindingOutsideSample<T>
        {
            [Test]
            public void Test()
            {
                TestLog.WriteLine(typeof(T));
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class ConstructorParameterBindingInsideSample
        {
            private readonly string item;
            private readonly decimal price;

            public ConstructorParameterBindingInsideSample([Column("Apples")] string item, [Column(1)] decimal price)
            {
                this.item = item;
                this.price = price;
            }

            [Test]
            public void Test()
            {
                TestLog.WriteLine("({0}, {1})", item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        [Row("Apples", 1)]
        internal class ConstructorParameterBindingOutsideSample
        {
            private readonly string item;
            private readonly decimal price;

            public ConstructorParameterBindingOutsideSample(string item, decimal price)
            {
                this.item = item;
                this.price = price;
            }

            [Test]
            public void Test()
            {
                TestLog.WriteLine("({0}, {1})", item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        [Row(typeof(string), "Apples", 1)]
        internal class TypeParameterAndConstructorParameterBindingOutsideSample<T>
        {
            private readonly T item;
            private readonly decimal price;

            public TypeParameterAndConstructorParameterBindingOutsideSample(T item, decimal price)
            {
                this.item = item;
                this.price = price;
            }

            [Test]
            public void Test()
            {
                TestLog.WriteLine("{0} -> ({1}, {2})", typeof(T), item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class FieldBindingSample
        {
            [Column("Apples")]
            public string Item = null;

            [Column(1)]
            public decimal Price = 0.0m;

            [Test]
            public void Test()
            {
                TestLog.WriteLine("({0}, {1})", Item, Price);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class PropertyBindingSample
        {
            private string item;
            private decimal price;

            [Column("Apples")]
            public string Item
            {
                set { item = value; }
            }

            [Column(1)]
            public decimal Price
            {
                get { return price; }
                set { price = value; }
            }

            [Test]
            public void Test()
            {
                TestLog.WriteLine("({0}, {1})", item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class MethodParameterBindingInsideSample
        {
            [Test]
            public void Test([Column("Apples")] string item, [Column(1)] decimal price)
            {
                TestLog.WriteLine("({0}, {1})", item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class MethodParameterBindingOutsideSample
        {
            [Test]
            [Row("Apples", 1)]
            public virtual void Test(string item, decimal price)
            {
                TestLog.WriteLine("({0}, {1})", item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class MethodParameterBindingOutsideSampleInherited : MethodParameterBindingOutsideSample
        {
            [Row("Oranges", 2)]
            public override void Test(string item, decimal price)
            {
                base.Test(item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        [Header("Type", "Item", "Price", "Quantity", "Variety", SourceName = "Data")]
        [Row(typeof(string), "Apples", 1, 10, "Empire", SourceName = "Data")]
        internal class ExplicitBindingByNameSample<[Bind("Type", Source = "Data")] T>
        {
            private readonly T item;
            private decimal price;

            public ExplicitBindingByNameSample([Bind("Item", Source = "Data")] T item)
            {
                this.item = item;
            }

            [Bind("Price", Source = "Data")]
            public decimal Price
            {
                set { price = value; }
            }

            [Bind("Quantity", Source = "Data")]
            public int Quantity = 0;

            [Test]
            public void Test([Bind("Variety", Source = "Data")] string variety)
            {
                TestLog.WriteLine("{0} -> ({1}, {2}) x {3}, {4}", typeof(T), item, price, Quantity, variety);
            }
        }

        [TestFixture, Explicit("Sample")]
        [Row(typeof(string), "Apples", 1, 10, "Empire")]
        internal class ExplicitBindingByIndexSample<[Bind(0)] T>
        {
            private readonly T item;
            private decimal price;

            public ExplicitBindingByIndexSample([Bind(1)] T item)
            {
                this.item = item;
            }

            [Bind(2)]
            public decimal Price
            {
                set { price = value; }
            }

            [Bind(3)]
            public int Quantity = 0;

            [Test]
            public void Test([Bind(4)] string variety)
            {
                TestLog.WriteLine("{0} -> ({1}, {2}) x {3}, {4}", typeof(T), item, price, Quantity, variety);
            }
        }

        [TestFixture, Explicit("Sample")]
        [Header("Type", "Item", "Price", "Quantity", "Variety")]
        [Row(typeof(string), "Apples", 1, 10, "Empire")]
        internal class ImplicitBindingByNameSample<[Name("Type")] T>
        {
            private readonly T item;
            private decimal price;

            public ImplicitBindingByNameSample(T item)
            {
                this.item = item;
            }

            [Bind("Price")]
            public decimal Price
            {
                set { price = value; }
            }

            [Bind("Quantity")]
            public int Quantity = 0;

            [Test]
            public void Test(string variety)
            {
                TestLog.WriteLine("{0} -> ({1}, {2}) x {3}, {4}", typeof(T), item, price, Quantity, variety);
            }
        }

        [TestFixture, Explicit("Sample")]
        [Row(typeof(string), typeof(decimal), "Apples", 1)]
        internal class ImplicitBindingByIndexOnClassSample<T, P>
        {
            private readonly T item;
            private readonly P price;

            public ImplicitBindingByIndexOnClassSample(T item, P price)
            {
                this.item = item;
                this.price = price;
            }

            [Test]
            public void Test()
            {
                TestLog.WriteLine("{0}, {1} -> ({2}, {3})", typeof(T), typeof(P), item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class ImplicitBindingByIndexOnMethodSample
        {
            [Test]
            [Row(typeof(string), typeof(decimal), "Apples", 1)]
            public void Test<T, P>(T item, P price)
            {
                TestLog.WriteLine("{0}, {1} -> ({2}, {3})", typeof(T), typeof(P), item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        [Row(typeof(string), "abc", Order = 1)]
        [Row(typeof(int), 123, Order = 2)]
        internal class CombinatorialBindingOfClassAndMethodWithOrderingSample<TOuter>
        {
            private readonly TOuter outerValue;

            public CombinatorialBindingOfClassAndMethodWithOrderingSample(TOuter outerValue)
            {
                this.outerValue = outerValue;
            }

            [Test]
            [Row(typeof(string), "def", Order = 2)]
            [Row(typeof(int), 456, Order = 1)]
            public void Test<TInner>(TInner innerValue)
            {
                TestLog.WriteLine("{0}, {1} -> ({2}, {3})", typeof(TOuter), typeof(TInner), outerValue, innerValue);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class CombinatorialBindingOfMethodWithMultipleDataSourcesAndOrderingSample
        {
            [Test]
            [Row(typeof(string), "abc", SourceName = "A", Order = 1)]
            [Row(typeof(int), 123, SourceName = "A", Order = 2)]
            [Row(typeof(string), "def", SourceName = "B", Order = 2)]
            [Row(typeof(int), 456, SourceName = "B", Order = 1)]
            public void Test<[Bind(0, Source = "A")] TA, [Bind(0, Source = "B")] TB>(
                [Bind(1, Source = "A")] TA outerValue, [Bind(1, Source = "B")] TB innerValue)
            {
                TestLog.WriteLine("{0}, {1} -> ({2}, {3})", typeof(TA), typeof(TB), outerValue, innerValue);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class CombinatorialJoinStrategySample
        {
            [Test, CombinatorialJoin]
            public void Test([Column(0, 1)] int a, [Column(0, 1)] int b, [Column(0, 1)] int c)
            {
                TestLog.WriteLine("{0}{1}{2}", a, b, c);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class SequentialJoinStrategySample
        {
            [Test, SequentialJoin]
            public void Test([Column(0, 1)] int a, [Column(0, 1)] int b, [Column(0, 1)] int c)
            {
                TestLog.WriteLine("{0}{1}{2}", a, b, c);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class PairwiseJoinStrategySample
        {
            [Test, PairwiseJoin]
            public void Test([Column(0, 1)] int a, [Column(0, 1)] int b, [Column(0, 1)] int c)
            {
                TestLog.WriteLine("{0}{1}{2}", a, b, c);
            }
        }

        [TestFixture, Disable]
        internal abstract class AbstractFixtureSample
        {
            [Bind(0, Source = "DataSource1")]
            public int i = 0;

            [Test]
            public void BaseTest([Bind(0, Source = "DataSource2")] string s)
            {
                TestLog.WriteLine("i = {0}, s = {1}", i, s);
            }
        }

        [TestFixture, Explicit("Sample")]
        [Column(123, SourceName = "DataSource1")]
        [Column("ABC", SourceName = "DataSource2")]
        internal class ConcreteFixtureSample : AbstractFixtureSample
        {
            [Test]
            public void ConcreteTest([Bind(0, Source = "DataSource2")] string s)
            {
                TestLog.WriteLine("i = {0}, s = {1}", i, s);
            }
        }
    }
}
