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
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Gallio.Collections;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    public class DistinctInstanceCollectionTest
    {
        [Test]
        public void ConstructsDefault()
        {
            var collection = new DistinctInstanceCollection<int>();
            Assert.IsEmpty(collection);
            Assert.IsEmpty(collection.Instances);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void AddsNullReferenceShouldThrowException()
        {
            var collection = new DistinctInstanceCollection<object>();
            collection.Add(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void AddsDoubletReferenceTypeObjectShouldThrowException()
        {
            var collection = new DistinctInstanceCollection<object>();
            var item = new object();
            collection.Add(item);
            collection.Add(item);
        }

        [Test]
        [ExpectedArgumentException]
        public void AddsDoubletValueTypeShouldThrowException()
        {
            var collection = new DistinctInstanceCollection<int>();
            collection.Add(123);
            collection.Add(123);
        }

        [Test]
        public void PopulatesExplicitelyTheCollection()
        {
            var collection = new DistinctInstanceCollection<int>();
            collection.Add(123);
            collection.Add(456);
            collection.Add(789);
            Assert.AreElementsEqualIgnoringOrder(new[] { 123, 456, 789 }, collection);
        }

        [Test]
        public void PopulatesWithListInitializerSyntax()
        {
            var collection = new DistinctInstanceCollection<int> { 123, 456, 789 };
            Assert.AreElementsEqualIgnoringOrder(new[] { 123, 456, 789 }, collection);
        }
    }
}
