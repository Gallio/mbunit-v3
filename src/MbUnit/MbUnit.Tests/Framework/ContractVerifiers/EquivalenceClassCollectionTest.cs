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
        [ExpectedArgumentNullException]
        public void ConstructsWithNullInitializer()
        {
            new EquivalenceClassCollection<object>(null);
        }

        [Test]
        public void ConstructsWithInitializerHavingNullElement()
        {
            EquivalenceClass<object> class1 = new EquivalenceClass<object>(new object());
            NewAssert.Throws<ArgumentException>(() => new EquivalenceClassCollection<object>(class1, null));
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
            NewAssert.AreEqual(new[] { class1, class2 }, collection.EquivalenceClasses);
            NewAssert.AreEqual(new[] { class1, class2 }, collection.ToArray());
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructsFromDistinctInstancesWithNullInitializerForValueType()
        {
            EquivalenceClassCollection<int>.FromDistinctInstances(null);
        }

        [Test]
        public void ConstructsFromDistinctInstancesWithNullInitializerForNullableType()
        {
            EquivalenceClassCollection<int?> @class = EquivalenceClassCollection<int?>.FromDistinctInstances(null);
            Assert.AreEqual(1, @class.Count());
            NewAssert.AreEqual(new int?[] { null }, @class.EquivalenceClasses[0].EquivalentInstances);
        }

        [Test]
        public void ConstructsFromDistinctInstancesWithNullInitializerForReferenceType()
        {
            EquivalenceClassCollection<object> @class = EquivalenceClassCollection<object>.FromDistinctInstances(null);
            Assert.AreEqual(1, @class.Count());
            NewAssert.AreEqual(new object[] { null }, @class.EquivalenceClasses[0].EquivalentInstances);
        }

        [Test]
        public void ConstructsFromDistinctInstancesOk()
        {
            object instance1 = new object();
            object instance2 = new object();
            object instance3 = new object();

            EquivalenceClassCollection<object> collection = EquivalenceClassCollection<object>.FromDistinctInstances(instance1, instance2, instance3);
            NewAssert.Over.Sequence(new[] { instance1, instance2, instance3 }, collection.EquivalenceClasses,
                (instance, @class) => NewAssert.AreEqual(new[] { instance }, @class.EquivalentInstances));
            NewAssert.Over.Sequence(new[] { instance1, instance2, instance3 }, collection.ToArray(),
                (instance, @class) => NewAssert.AreEqual(new[] { instance }, @class.EquivalentInstances));
        }
    }
}
