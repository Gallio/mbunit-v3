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
        [Test, ExpectedArgumentNullException]
        public void ConstructsWithNullInitializer()
        {
            new EquivalenceClassCollection<object>(null);
        }

        [Test]
        public void ConstructsWithInitializerHavingNullElement()
        {
            EquivalenceClass<object> class1 = new EquivalenceClass<object>(new object());
            Assert.Throws<ArgumentException>(() => new EquivalenceClassCollection<object>(class1, null));
        }

        [Test]
        public void ConstructWithEmptyInitializer()
        {
            EquivalenceClassCollection<object> collection = new EquivalenceClassCollection<object>(EmptyArray<EquivalenceClass<object>>.Instance);
            Assert.AreEqual(0, collection.EquivalenceClasses.Count);
            Assert.AreEqual(0, collection.Count());
        }

        [Test]
        public void ConstructsOk()
        {
            EquivalenceClass<object> class1 = new EquivalenceClass<object>(new object());
            EquivalenceClass<object> class2 = new EquivalenceClass<object>(new object());

            EquivalenceClassCollection<object> collection = new EquivalenceClassCollection<object>(class1, class2);
            Assert.AreEqual(new[] { class1, class2 }, collection.EquivalenceClasses);
            Assert.AreElementsEqual(new[] { class1, class2 }, collection.ToArray());
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructsFromDistinctInstancesWithNullInitializerForValueType()
        {
            EquivalenceClassCollection<int>.FromDistinctInstances(null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructsFromDistinctInstancesWithNullInitializerForNullableType()
        {
            EquivalenceClassCollection<int?>.FromDistinctInstances(null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructsFromDistinctInstancesWithNullInitializerForReferenceType()
        {
            EquivalenceClassCollection<object>.FromDistinctInstances(null);
        }

        [Test]
        public void ConstructsFromDistinctInstancesOk()
        {
            object instance1 = new object();
            object instance2 = new object();
            object instance3 = new object();

            EquivalenceClassCollection<object> collection = EquivalenceClassCollection<object>.FromDistinctInstances(instance1, instance2, instance3);
            Assert.Over.Pairs(new[] { instance1, instance2, instance3 }, collection.EquivalenceClasses,
                (instance, @class) => Assert.AreElementsEqual(new[] { instance }, @class.EquivalentInstances));
            Assert.Over.Pairs(new[] { instance1, instance2, instance3 }, collection.ToArray(),
                (instance, @class) => Assert.AreElementsEqual(new[] { instance }, @class.EquivalentInstances));
        }
    }
}
