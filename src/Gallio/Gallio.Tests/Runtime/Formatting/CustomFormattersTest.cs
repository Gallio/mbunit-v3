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
using Gallio.Runtime.Conversions;
using MbUnit.Framework;
using Gallio.Runtime.Formatting;

namespace Gallio.Tests.Runtime.Formatting
{
    [TestFixture]
    [TestsOn(typeof(CustomFormatters))]
    public class CustomFormattersTest
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

        [Test]
        [ExpectedArgumentNullException]
        public void Registers_with_null_type_should_throw_exception()
        {
            var customFormatters = new CustomFormatters();
            customFormatters.Register(null, x => String.Empty);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Registers_with_null_function_should_throw_exception()
        {
            var customFormatters = new CustomFormatters();
            customFormatters.Register(typeof(string), null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Generic_Registers_with_null_formatter_should_throw_exception()
        {
            var customFormatters = new CustomFormatters();
            customFormatters.Register<Foo>(null);
        }

        [Test]
        public void Find_should_return_null_for_non_registered_type()
        {
            var customFormatters = new CustomFormatters();
            var func = customFormatters.Find(typeof(Foo));
            Assert.IsNull(func);
        }

        [Test]
        public void IsRegisteredFor_should_return_true_for_registered_type()
        {
            var customFormatters = new CustomFormatters();
            customFormatters.Register<Foo>(x => String.Empty);
            FormattingFunc func = customFormatters.Find(typeof(Foo));
            Assert.IsNotNull(func);
        }

        [Test]
        public void Formats()
        {
            var customFormatters = new CustomFormatters();
            customFormatters.Register<Foo>(x => String.Format("Foo's value is {0}.", x.Value));
            FormattingFunc func = customFormatters.Find(typeof(Foo));
            string output = func(new Foo(123));
            Assert.AreEqual("Foo's value is 123.", output);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Unregister_with_null_type_should_throw_exception()
        {
            var customFormatters = new CustomFormatters();
            customFormatters.Unregister(null);
        }

        [Test]
        public void Register_and_unregister_ok()
        {
            var customFormatters = new CustomFormatters();
            customFormatters.Register<Foo>(x => String.Empty);
            customFormatters.Unregister<Foo>();
            FormattingFunc func = customFormatters.Find(typeof(Foo));
            Assert.IsNull(func);
        }
    }
}
