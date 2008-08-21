using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Collections;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    public class EquivalenceClassCollectionTest
    {
        private EquivalenceClassCollection<object> collection;

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullInitializer()
        {
            collection = new EquivalenceClassCollection<object>(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void ConstructsWithInitializerHavingNullElement()
        {
            EquivalenceClass<object> class1 = new EquivalenceClass<object>(new object());
            collection = new EquivalenceClassCollection<object>(class1, null);
        }

        [Test]
        public void ConstructWithEmptyInitializer()
        {
            collection = new EquivalenceClassCollection<object>(EmptyArray<EquivalenceClass<object>>.Instance);
            Assert.AreEqual(0, collection.Count());
        }

        [Test]
        public void ConstructsOk()
        {
            EquivalenceClass<object> class1 = new EquivalenceClass<object>(new object());
            EquivalenceClass<object> class2 = new EquivalenceClass<object>(new object());
            collection = new EquivalenceClassCollection<object>(class1, class2);
            Assert.AreEqual(2, collection.Count());
            Assert.IsTrue(collection.Contains(class1));
            Assert.IsTrue(collection.Contains(class2));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsFromDistinctInstancesWithNullInitializer()
        {
            collection = EquivalenceClassCollection<object>.FromDistinctInstances(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void ConstructsFromDistinctInstancesWithInitializerHavinhNullElement()
        {
            collection = EquivalenceClassCollection<object>.FromDistinctInstances(new object(), null);
        }

        [Test]
        public void ConstructsFromDistinctInstancesOk()
        {
            object instance1 = new object();
            object instance2 = new object();
            object instance3 = new object();
            collection = EquivalenceClassCollection<object>.FromDistinctInstances(instance1, instance2, instance3);
            Assert.AreEqual(3, collection.Count());
            Assert.AreEqual(1, collection.ElementAt(0).Count());
            Assert.AreEqual(1, collection.ElementAt(1).Count());
            Assert.AreEqual(1, collection.ElementAt(2).Count());
        }
    }
}
