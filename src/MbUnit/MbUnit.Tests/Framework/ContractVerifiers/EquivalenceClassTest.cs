// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
    public class EquivalenceClassTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructsWithNullInitializerForValueType()
        {
            new EquivalenceClass<int>(null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructsWithNullInitializerForNullableType()
        {
            new EquivalenceClass<int?>(null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructsWithNullInitializerForReferenceType()
        {
            new EquivalenceClass<object>(null);
        }

        [Test, ExpectedArgumentException]
        public void ConstructsWithInitializerContainingNoObjects()
        {
            new EquivalenceClass<object>(EmptyArray<object>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructsWithInitializerContainingNullReference()
        {
            new EquivalenceClass<object>(new Object(), new Object(), null);
        }

        [Test]
        public void ConstructsOk()
        {
            object object1 = new object();
            object object2 = new object();
            object object3 = new object();

            EquivalenceClass<object> target = new EquivalenceClass<object>(object1, object2, object3);
            Assert.AreElementsEqual(new[] { object1, object2, object3 }, target.EquivalentInstances);

            Assert.AreEqual(3, target.Count());
            Assert.IsTrue(target.Contains(object1));
            Assert.IsTrue(target.Contains(object2));
            Assert.IsTrue(target.Contains(object3));
        }
    }
}
