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
using Gallio.Common.Collections;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    [TestsOn(typeof(EquivalenceClassCollection))]
    public class EquivalenceClassCollectionTest
    {
        [Test]
        public void ConstructEmpty()
        {
            var collection = new EquivalenceClassCollection();
            Assert.IsEmpty(collection);
        }

        [Test]
        public void ConstructWithListInitializer()
        {
            var collection = new EquivalenceClassCollection
            {
                {1, "2", 3},
                {4, 5d, "6", 7},
                {8, 9m}
            };

            Assert.Count(3, collection);
            Assert.AreElementsEqual(new object[] { 1, "2", 3 }, collection[0].Cast<object>());
            Assert.AreElementsEqual(new object[] { 4, 5d, "6", 7 }, collection[1].Cast<object>());
            Assert.AreElementsEqual(new object[] { 8, 9m }, collection[2].Cast<object>());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructWithNullParameter()
        {
            new EquivalenceClassCollection(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void ConstructWithParameterHavingNullInstance()
        {
            new EquivalenceClassCollection(new[] { new object(), new object(), null });
        }

        [Test]
        public void ConstructWithParameter()
        {
            var collection = new EquivalenceClassCollection(new object[] { 1, "2", 3d });
            Assert.Count(3, collection);
            Assert.AreElementsEqual(new object[] { 1 }, collection[0].Cast<object>());
            Assert.AreElementsEqual(new object[] { "2" }, collection[1].Cast<object>());
            Assert.AreElementsEqual(new object[] { 3d }, collection[2].Cast<object>());
        }
    }
}
