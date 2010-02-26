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
using System.Text;
using Gallio.Common;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(CustomComparers))]
    public class CustomComparersTest
    {
        internal class Foo
        {
            private readonly int value;

            public int Value
            {
                get { return value; }
            }

            public Foo(int value)
            {
                this.value = value;
            }
        }

        [SetUp]
        public void Setup()
        {
            var customComparers = new CustomComparers();
            customComparers.UnregisterAll();
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Registers_with_null_type_should_throw_exception()
        {
            var customComparers = new CustomComparers();
            customComparers.Register(null, (x, y) => 0);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Registers_with_null_comparer_should_throw_exception()
        {
            var customComparers = new CustomComparers();
            customComparers.Register(typeof(Foo), null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Generic_Registers_with_null_comparer_should_throw_exception()
        {
            var customComparers = new CustomComparers();
            customComparers.Register<Foo>(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Find_with_null_type_should_throw_exception()
        {
            var customComparers = new CustomComparers();
            customComparers.Find(null);
        }

        [Test]
        public void Find_should_return_null_for_non_registered_type()
        {
            var customComparers = new CustomComparers();
            var comparer = customComparers.Find(typeof(Foo));
            Assert.IsNull(comparer);
        }

        [Test]
        public void IsRegisteredFor_should_return_true_for_registered_type()
        {
            var customComparers = new CustomComparers();
            customComparers.Register<Foo>((x, y) => x.Value.CompareTo(y.Value));
            Comparison comparer = customComparers.Find(typeof(Foo));
            Assert.IsNotNull(comparer);
        }

        [Test]
        [Row(123, 456, -1)]
        [Row(123, 123, 0)]
        [Row(456, 123, 1)]
        public void Equals_ok(int foo1, int foo2, int expectedSign)
        {
            var customComparers = new CustomComparers();
            customComparers.Register<Foo>((x, y) => x.Value.CompareTo(y.Value));
            Comparison comparer = customComparers.Find(typeof(Foo));
            int actualSign = comparer(new Foo(foo1), new Foo(foo2));
            Assert.AreEqual(expectedSign, actualSign, (x, y) => Math.Sign(x) == Math.Sign(y));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Unregister_with_null_type_should_throw_exception()
        {
            var customComparers = new CustomComparers();
            customComparers.Unregister(null);
        }

        [Test]
        public void Register_and_unregister_ok()
        {
            var customComparers = new CustomComparers();
            customComparers.Register<Foo>((x, y) => x.Value.CompareTo(y.Value));
            customComparers.Unregister<Foo>();
            var comparer = customComparers.Find(typeof(Foo));
            Assert.IsNull(comparer);
        }
    }
}
