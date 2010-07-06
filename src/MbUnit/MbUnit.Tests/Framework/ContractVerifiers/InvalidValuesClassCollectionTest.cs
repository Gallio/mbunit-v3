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
    public class InvalidValuesClassCollectionTest
    {
        [Test]
        public void Constructs_empty()
        {
            var collection = new InvalidValuesClassCollection<int>();
            Assert.IsEmpty(collection);
            Assert.Count(0, collection);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Adds_null_invalid_class_should_throw_exception()
        {
            var collection = new InvalidValuesClassCollection<int>();
            collection.Add((InvalidValuesClass<int>)null);
        }

        [Test]
        [ExpectedArgumentException]
        public void Adds_several_times_classes_associated_with_same_exception_type_should_throw_exception()
        {
            var collection = new InvalidValuesClassCollection<int>();
            collection.Add(new InvalidValuesClass<int>(typeof(ArgumentOutOfRangeException)));
            collection.Add(new InvalidValuesClass<int>(typeof(ArgumentOutOfRangeException)));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Adds_null_exception_type_should_throw_exception()
        {
            var collection = new InvalidValuesClassCollection<int>();
            collection.Add(null, 1, 2, 3);
        }

        [Test]
        [ExpectedArgumentException]
        public void Adds_several_times_classes_arguments_associated_with_same_exception_type_should_throw_exception()
        {
            var collection = new InvalidValuesClassCollection<int>();
            collection.Add(typeof(ArgumentOutOfRangeException), 1, 2, 3);
            collection.Add(typeof(ArgumentOutOfRangeException), 4, 5, 6);
        }

        [Test]
        public void Adds_classes_Ok()
        {
            var collection = new InvalidValuesClassCollection<int>();
            collection.Add(typeof(ArgumentOutOfRangeException), 1, 2, 3);
            collection.Add(new InvalidValuesClass<int>(typeof(ArgumentException)) { 4, 5, 6 });
            collection.Add(typeof(InvalidOperationException), 7, 8, 9);
            Assert.Count(3, collection);
            Assert.AreElementsEqualIgnoringOrder(new[] { typeof(ArgumentOutOfRangeException), typeof(ArgumentException), typeof(InvalidOperationException) }, collection.Select(x => x.ExpectedExceptionType));        
        }
    }
}
