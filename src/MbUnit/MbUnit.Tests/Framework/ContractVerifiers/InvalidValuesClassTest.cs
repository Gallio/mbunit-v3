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
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Gallio.Common.Collections;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    public class InvalidValuesClassTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_without_initializer_and_null_exception_type_should_throw_exception()
        {
            new InvalidValuesClass<int>(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_initializer_and_null_exception_type_should_throw_exception()
        {
            new InvalidValuesClass<int>(null, new[] { 123, 456 });
        }

        internal class MyErroneousException // Does not derive from System.Exception!
        {
        }

        [Test]
        [ExpectedArgumentException]
        public void Constructs_without_initializer_and_invalid_exception_type_should_throw_exception()
        {
            new InvalidValuesClass<int>(typeof(MyErroneousException));
        }

        [Test]
        [ExpectedArgumentException]
        public void Constructs_with_initializer_and_invalid_exception_type_should_throw_exception()
        {
            new InvalidValuesClass<int>(typeof(MyErroneousException), new[] { 123, 456 });
        }

        [Test]
        public void Constructs_empty_ok()
        {
            var collection = new InvalidValuesClass<int>(typeof(ArgumentOutOfRangeException));
            Assert.AreEqual(typeof(ArgumentOutOfRangeException), collection.ExpectedExceptionType);
            Assert.IsEmpty(collection);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Adds_null_reference_should_throw_exception()
        {
            var collection = new InvalidValuesClass<object>(typeof(ArgumentException));
            collection.Add(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void Adds_doublet_reference_type_object_should_throw_exception()
        {
            var collection = new InvalidValuesClass<object>(typeof(ArgumentException));
            var item = new object();
            collection.Add(item);
            collection.Add(item);
        }

        [Test]
        [ExpectedArgumentException]
        public void Adds_doublet_value_type_should_throw_exception()
        {
            var collection = new InvalidValuesClass<int>(typeof(ArgumentException));
            collection.Add(123);
            collection.Add(123);
        }

        [Test]
        public void Populates_explicitely_the_collection_ok()
        {
            var collection = new InvalidValuesClass<int>(typeof(ArgumentException));
            collection.Add(123);
            collection.Add(456);
            collection.Add(789);
            Assert.AreEqual(typeof(ArgumentException), collection.ExpectedExceptionType);
            Assert.AreElementsEqualIgnoringOrder(new[] { 123, 456, 789 }, collection);
        }

        [Test]
        public void Populates_within_constructor_ok()
        {
            var collection = new InvalidValuesClass<int>(typeof(ArgumentException), new[] { 123, 456, 789 });
            Assert.AreEqual(typeof(ArgumentException), collection.ExpectedExceptionType);
            Assert.AreElementsEqualIgnoringOrder(new[] { 123, 456, 789 }, collection);
        }
    }
}
