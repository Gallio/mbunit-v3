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
using Gallio.Framework;
using MbUnit.Framework;
using Gallio.Runtime.Conversions;

namespace Gallio.Tests.Runtime.Conversions
{
    [TestFixture]
    [TestsOn(typeof(CustomConverters))]
    public class CustomConvertersTest
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
            CustomConverters.UnregisterAll();
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Registers_with_null_source_type_should_throw_exception()
        {
            CustomConverters.Register(null, typeof(Foo), x => null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Registers_with_null_target_type_should_throw_exception()
        {
            CustomConverters.Register(typeof(string), null, x => null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Registers_with_null_conversion_should_throw_exception()
        {
            CustomConverters.Register(typeof(string), typeof(Foo), null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Generic_Registers_with_null_converter_should_throw_exception()
        {
            CustomConverters.Register<string, Foo>(null);
        }

        [Test]
        public void Find_should_return_null_for_non_registered_type()
        {
            var conversion = CustomConverters.Find(new ConversionKey(typeof(string), typeof(Foo)));
            Assert.IsNull(conversion);
        }

        [Test]
        public void IsRegisteredFor_should_return_true_for_registered_type()
        {
            CustomConverters.Register<string, Foo>(x => new Foo(Int32.Parse(x)));
            Conversion conversion = CustomConverters.Find(new ConversionKey(typeof(string), typeof(Foo)));
            Assert.IsNotNull(conversion);
        }

        [Test]
        public void Converts()
        {
            CustomConverters.Register<string, Foo>(x => new Foo(Int32.Parse(x)));
            Conversion conversion = CustomConverters.Find(new ConversionKey(typeof(string), typeof(Foo)));
            var actualFoo = (Foo)conversion("123");
            Assert.AreEqual(123, actualFoo.Value);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Unregister_with_null_source_type_should_throw_exception()
        {
            CustomConverters.Unregister(null, typeof(Foo));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Unregister_with_null_target_type_should_throw_exception()
        {
            CustomConverters.Unregister(typeof(string), null);
        }

        [Test]
        public void Register_and_unregister_ok()
        {
            CustomConverters.Register<string, Foo>(x => new Foo(Int32.Parse(x)));
            CustomConverters.Unregister<string, Foo>();
            var comversion = CustomConverters.Find(new ConversionKey(typeof(string), typeof(Foo)));
            Assert.IsNull(comversion);
        }
    }
}
