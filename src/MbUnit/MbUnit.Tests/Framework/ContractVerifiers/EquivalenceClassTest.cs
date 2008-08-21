using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Gallio.Collections;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    public class EquivalenceClassTest
    {
        private EquivalenceClass<object> target;

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullInitializer()
        {
            target = new EquivalenceClass<object>(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void ConstructsWithInitializerContainingNoObjects()
        {
            target = new EquivalenceClass<object>(EmptyArray<object>.Instance);
        }

        [Test]
        public void ConstructsOk()
        {
            object object1 = new object();
            object object2 = new object();
            object object3 = new object();
            target = new EquivalenceClass<object>(object1, object2, object3);
            Assert.AreEqual(3, target.Count());
            Assert.IsTrue(target.Contains(object1));
            Assert.IsTrue(target.Contains(object2));
            Assert.IsTrue(target.Contains(object3));
        }
    }
}
