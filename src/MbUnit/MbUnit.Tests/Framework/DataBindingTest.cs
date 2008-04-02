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
        [Row(typeof(ExplicitBindingSample<>), "Test", new string[] { "System.String -> (Apples, 1) x 10, Empire" })]
        [Row(typeof(ImplicitBindingByNameSample<>), "Test", new string[] { "System.String -> (Apples, 1) x 10, Empire" })]
        [Row(typeof(ImplicitBindingByIndexSample<>), "Test", new string[] { "System.String -> (Apples, 1) x 10, Empire" })]
        public void VerifySampleOutput(Type fixtureType, string sampleName, string[] output)
        {
            RunFixtures(fixtureType);
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(fixtureType.GetMethod(sampleName)));

            Assert.AreEqual(output.Length, run.Children.Count, "Different number of runs than expected.");

            for (int i = 0; i < output.Length; i++)
                AssertLogOutput(run.Children[i], output[i]);
        }

        private static void AssertLogOutput(TestStepRun run, string expectedOutput)
        {
            Assert.Contains(run.ExecutionLog.GetStream(LogStreamNames.Default).ToString(), expectedOutput);
        }
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
    [Header("Type", "Item", "Price", "Quantity", "Variety", SourceName="Data")]
    [Row(typeof(string), "Apples", 1, 10, "Empire", SourceName="Data")]
    internal class ExplicitBindingSample<[Bind("Type", Source="Data")] T>
    {
        private readonly T item;
        private decimal price;

        public ExplicitBindingSample([Bind("Item")] T item)
        {
            this.item = item;
        }

        [Bind("Price", Source="Data")]
        public decimal Price
        {
            set { price = value; }
        }

        [Bind("Quantity", Source="Data")]
        public int Quantity = 0;

        [Test]
        public void Test([Bind("Variety", Source="Data")] string variety)
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
    [Row(typeof(string), "Apples", 1, 10, "Empire")]
    internal class ImplicitBindingByIndexSample<T>
    {
        private readonly T item;
        private decimal price;

        public ImplicitBindingByIndexSample(T item)
        {
            this.item = item;
        }

        [Parameter]
        public decimal Price
        {
            set { price = value; }
        }

        [Bind(3)] // can't implicitly bind here because the implicit index will be 2, which is already assigned to price
        public int Quantity = 0;

        [Test]
        public void Test(string variety)
        {
            Log.WriteLine("{0} -> ({1}, {2}) x {3}, {4}", typeof(T), item, price, Quantity, variety);
        }
    }
}
