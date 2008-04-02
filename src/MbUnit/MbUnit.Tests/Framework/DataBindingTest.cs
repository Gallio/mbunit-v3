// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Framework;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests.Integration;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(RowAttribute))]
    [TestsOn(typeof(ColumnAttribute))]
    [TestsOn(typeof(BindAttribute))]
    public class DataBindingTest : BaseSampleTest
    {
        [Test]
        [Row(typeof(TypeParameterBindingInsideSample<>), "Test", new string[] { "System.Int32" })]
        [Row(typeof(TypeParameterBindingOutsideSample<>), "Test", new string[] { "System.String" })]
        [Row(typeof(ConstructorParameterBindingInsideSample), "Test", new string[] { "(Apples, 1)" })]
        [Row(typeof(ConstructorParameterBindingOutsideSample), "Test", new string[] { "(Apples, 1)" })]
        [Row(typeof(TypeParameterAndConstructorParameterBindingOutsideSample<>), "Test", new string[] { "System.String -> (Apples, 1)" })]
        [Row(typeof(FieldBindingSample), "Test", new string[] { "(Apples, 1)" })]
        [Row(typeof(PropertyBindingSample), "Test", new string[] { "(Apples, 1)" })]
        [Row(typeof(MethodParameterBindingInsideSample), "Test", new string[] { "(Apples, 1)" })]
        [Row(typeof(MethodParameterBindingOutsideSample), "Test", new string[] { "(Apples, 1)" })]
        [Row(typeof(ExplicitBindingByNameSample<>), "Test", new string[] { "System.String -> (Apples, 1) x 10, Empire" })]
        [Row(typeof(ExplicitBindingByIndexSample<>), "Test", new string[] { "System.String -> (Apples, 1) x 10, Empire" })]
        [Row(typeof(ImplicitBindingByNameSample<>), "Test", new string[] { "System.String -> (Apples, 1) x 10, Empire" })]
        [Row(typeof(ImplicitBindingByIndexOnClassSample<,>), "Test", new string[] { "System.String, System.Decimal -> (Apples, 1)" })]
        [Row(typeof(ImplicitBindingByIndexOnMethodSample), "Test", new string[] { "System.String, System.Decimal -> (Apples, 1)" })]
        [Row(typeof(CombinatorialBindingOfClassAndMethodWithOrderingSample<>), "Test", new string[] {
            "System.String, System.Int32 -> (abc, 456)",
            "System.String, System.String -> (abc, def)",
            "System.Int32, System.Int32 -> (123, 456)",
            "System.Int32, System.String -> (123, def)"
        })]
        [Row(typeof(CombinatorialBindingOfMethodWithMultipleDataSourcesAndOrderingSample), "Test", new string[] {
            "System.String, System.Int32 -> (abc, 456)",
            "System.String, System.String -> (abc, def)",
            "System.Int32, System.Int32 -> (123, 456)",
            "System.Int32, System.String -> (123, def)"
        })]
        [Row(typeof(CombinatorialJoinStrategySample), "Test", new string[] { "000", "001", "010", "011", "100", "101", "110", "111" })]
        [Row(typeof(SequentialJoinStrategySample), "Test", new string[] { "000", "111" })]
        // pending implementation: [Row(typeof(PairwiseJoinStrategySample), "Test", new string[] { "000", "011", "110" })]
        public void VerifySampleOutput(Type fixtureType, string sampleName, string[] output)
        {
            RunFixtures(fixtureType);

            IList<TestStepRun> runs = Runner.GetTestCaseRunsWithin(CodeReference.CreateFromType(fixtureType));

            Assert.AreEqual(output.Length, runs.Count, "Different number of runs than expected.");

            for (int i = 0; i < output.Length; i++)
                AssertLogOutput(runs[i], output[i]);
        }

        private static void AssertLogOutput(TestStepRun run, string expectedOutput)
        {
            Assert.Contains(run.ExecutionLog.GetStream(LogStreamNames.Default).ToString(), expectedOutput);
        }


        [TestFixture, Explicit("Sample")]
        internal class TypeParameterBindingInsideSample<[Column(typeof(int))] T>
        {
            [Test]
            public void Test()
            {
                Log.WriteLine(typeof(T));
            }
        }

        [TestFixture, Explicit("Sample")]
        [Row(typeof(string))]
        internal class TypeParameterBindingOutsideSample<T>
        {
            [Test]
            public void Test()
            {
                Log.WriteLine(typeof(T));
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
                Log.WriteLine("({0}, {1})", item, price);
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
                Log.WriteLine("({0}, {1})", item, price);
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
                Log.WriteLine("{0} -> ({1}, {2})", typeof(T), item, price);
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
                Log.WriteLine("({0}, {1})", Item, Price);
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
                Log.WriteLine("({0}, {1})", item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class MethodParameterBindingInsideSample
        {
            [Test]
            public void Test([Column("Apples")] string item, [Column(1)] decimal price)
            {
                Log.WriteLine("({0}, {1})", item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class MethodParameterBindingOutsideSample
        {
            [Test]
            [Row("Apples", 1)]
            public void Test(string item, decimal price)
            {
                Log.WriteLine("({0}, {1})", item, price);
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
                Log.WriteLine("{0} -> ({1}, {2}) x {3}, {4}", typeof(T), item, price, Quantity, variety);
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
                Log.WriteLine("{0} -> ({1}, {2}) x {3}, {4}", typeof(T), item, price, Quantity, variety);
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
                Log.WriteLine("{0} -> ({1}, {2}) x {3}, {4}", typeof(T), item, price, Quantity, variety);
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
                Log.WriteLine("{0}, {1} -> ({2}, {3})", typeof(T), typeof(P), item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class ImplicitBindingByIndexOnMethodSample
        {
            [Test]
            [Row(typeof(string), typeof(decimal), "Apples", 1)]
            public void Test<T, P>(T item, P price)
            {
                Log.WriteLine("{0}, {1} -> ({2}, {3})", typeof(T), typeof(P), item, price);
            }
        }

        [TestFixture, Explicit("Sample")]
        [Row(typeof(string), "abc", Order=1)]
        [Row(typeof(int), 123, Order=2)]
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
                Log.WriteLine("{0}, {1} -> ({2}, {3})", typeof(TOuter), typeof(TInner), outerValue, innerValue);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class CombinatorialBindingOfMethodWithMultipleDataSourcesAndOrderingSample
        {
            [Test]
            [Row(typeof(string), "abc", SourceName="A", Order = 1)]
            [Row(typeof(int), 123, SourceName = "A", Order = 2)]
            [Row(typeof(string), "def", SourceName = "B", Order = 2)]
            [Row(typeof(int), 456, SourceName = "B", Order = 1)]
            public void Test<[Bind(0, Source = "A")] TA, [Bind(0, Source = "B")] TB>(
                [Bind(1, Source = "A")] TA outerValue, [Bind(1, Source = "B")] TB innerValue)
            {
                Log.WriteLine("{0}, {1} -> ({2}, {3})", typeof(TA), typeof(TB), outerValue, innerValue);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class CombinatorialJoinStrategySample
        {
            [Test, CombinatorialJoin]
            public void Test([Column(0, 1)] int a, [Column(0, 1)] int b, [Column(0, 1)] int c)
            {
                Log.WriteLine("{0}{1}{2}", a, b, c);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class SequentialJoinStrategySample
        {
            [Test, SequentialJoin]
            public void Test([Column(0, 1)] int a, [Column(0, 1)] int b, [Column(0, 1)] int c)
            {
                Log.WriteLine("{0}{1}{2}", a, b, c);
            }
        }

        [TestFixture, Explicit("Sample")]
        internal class PairwiseJoinStrategySample
        {
            [Test, PairwiseJoin]
            public void Test([Column(0, 1)] int a, [Column(0, 1)] int b, [Column(0, 1)] int c)
            {
                Log.WriteLine("{0}{1}{2}", a, b, c);
            }
        }
    }
}
