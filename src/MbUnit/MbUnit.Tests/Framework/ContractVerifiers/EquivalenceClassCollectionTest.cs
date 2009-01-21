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
using System.Collections.Generic;
using System.Linq;
using Gallio.Collections;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    public class EquivalenceClassCollectionTest
    {
        [Test]
        public void ConstructEmpty()
        {
            var collection = new EquivalenceClassCollection<object>();
            Assert.IsEmpty(collection.EquivalenceClasses);
        }

        [Test]
        public void ConstructWithListInitializer()
        {
            var collection = new EquivalenceClassCollection<int>
            {
                {1, 2, 3},
                {4, 5, 6, 7},
                {8, 9}
            };

            Assert.AreEqual(3, collection.Count());
            Assert.AreElementsEqual(new[] { 1, 2, 3 }, collection.ElementAt(0));
            Assert.AreElementsEqual(new[] { 4, 5, 6, 7 }, collection.ElementAt(1));
            Assert.AreElementsEqual(new[] { 8, 9 }, collection.ElementAt(2));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructWithNullParameter()
        {
            new EquivalenceClassCollection<object>(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void ConstructWithParameterHavingNullInstance()
        {
            new EquivalenceClassCollection<object>(new object[] { new object(), new object(), null });
        }

        [Test]
        public void ConstructWithParameter()
        {
            var collection = new EquivalenceClassCollection<int>(new[] { 1, 2, 3 });
            Assert.AreEqual(3, collection.Count());
            Assert.AreElementsEqual(new[] { 1 }, collection.ElementAt(0));
            Assert.AreElementsEqual(new[] { 2 }, collection.ElementAt(1));
            Assert.AreElementsEqual(new[] { 3 }, collection.ElementAt(2));
        }
    }
}
