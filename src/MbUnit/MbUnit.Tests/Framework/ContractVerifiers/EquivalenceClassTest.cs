using System;
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Gallio.Collections;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    public class EquivalenceClassTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructsWithNullInitializerForValueType()
        {
            new EquivalenceClass<int>(null);
        }

        [Test]
        public void ConstructsWithNullInitializerForNullableType()
        {
            EquivalenceClass<int?> target = new EquivalenceClass<int?>(null);
            NewAssert.AreEqual(new int?[] { null }, target.EquivalentInstances);
        }

        [Test]
        public void ConstructsWithNullInitializerForReferenceType()
        {
            EquivalenceClass<object> target = new EquivalenceClass<object>(null);
            NewAssert.AreEqual(new object[] { null }, target.EquivalentInstances);
        }

        [Test]
        [ExpectedArgumentException]
        public void ConstructsWithInitializerContainingNoObjects()
        {
            new EquivalenceClass<object>(EmptyArray<object>.Instance);
        }

        [Test]
        public void ConstructsOk()
        {
            object object1 = new object();
            object object2 = new object();
            object object3 = new object();

            EquivalenceClass<object> target = new EquivalenceClass<object>(object1, object2, object3);
            NewAssert.AreEqual(new[] { object1, object2, object3 }, target.EquivalentInstances);

            Assert.AreEqual(3, target.Count());
            Assert.IsTrue(target.Contains(object1));
            Assert.IsTrue(target.Contains(object2));
            Assert.IsTrue(target.Contains(object3));
        }
    }
}
