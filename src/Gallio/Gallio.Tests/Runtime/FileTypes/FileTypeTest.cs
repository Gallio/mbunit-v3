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
using System.Text;
using Gallio.Runtime.FileTypes;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.FileTypes
{
    [TestsOn(typeof(FileType))]
    public class FileTypeTest
    {
        [Test]
        public void Constructor_WhenIdIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FileType(null, "description", null));
        }

        [Test]
        public void Constructor_WhenIdIsEmpty_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FileType("", "description", null));
            Assert.Contains(ex.Message, "The file type id must not be empty.");
        }

        [Test]
        public void Constructor_WhenDescriptionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FileType("id", null, null));
        }

        [Test]
        public void Constructor_WhenArgumentsValid_SetsProperties()
        {
            var superType = new FileType("superId", "superDescription", null);
            var type = new FileType("id", "description", superType);

            Assert.AreEqual("id", type.Id);
            Assert.AreEqual("description", type.Description);
            Assert.AreEqual(superType, type.SuperType);
        }

        [Test]
        public void IsSameOrSubtypeOf_WhenOtherTypeIsNull_Throws()
        {
            var type = new FileType("id", "description", null);

            Assert.Throws<ArgumentNullException>(() => type.IsSameOrSubtypeOf(null));           
        }

        [Test]
        public void IsSameOrSubtypeOf_WhenOtherTypeIsSame_ReturnsTrue()
        {
            var type = new FileType("id", "description", null);

            Assert.IsTrue(type.IsSameOrSubtypeOf(type));
        }

        [Test]
        public void IsSameOrSubtypeOf_WhenOtherTypeIsASuperType_ReturnsTrue()
        {
            var superSuperType = new FileType("superSuperId", "superSuperDescription", null);
            var superType = new FileType("superId", "superDescription", superSuperType);
            var type = new FileType("id", "description", superType);

            Assert.IsTrue(type.IsSameOrSubtypeOf(superType));
            Assert.IsTrue(type.IsSameOrSubtypeOf(superSuperType));
        }

        [Test]
        public void IsSameOrSubtypeOf_WhenOtherTypeIsUnrelated_ReturnsFalse()
        {
            var superType = new FileType("superId", "superDescription", null);
            var type = new FileType("id", "description", superType);
            var unrelatedType = new FileType("otherId", "otherDescription", superType);

            Assert.IsFalse(type.IsSameOrSubtypeOf(unrelatedType));
        }

        [Test]
        public void IsSameOrSubtypeOfAny_WhenOtherTypesIsNull_Throws()
        {
            var type = new FileType("id", "description", null);

            Assert.Throws<ArgumentNullException>(() => type.IsSameOrSubtypeOfAny(null));
        }

        [Test]
        public void IsSameOrSubtypeOfAny_WhenOtherTypesContainsNull_Throws()
        {
            var type = new FileType("id", "description", null);

            Assert.Throws<ArgumentNullException>(() => type.IsSameOrSubtypeOfAny(new FileType[] { null }));
        }

        [Test]
        public void IsSameOrSubtypeOfAny_WhenOtherTypesContainsSame_ReturnsTrue()
        {
            var type = new FileType("id", "description", null);

            Assert.IsTrue(type.IsSameOrSubtypeOfAny(type));
        }

        [Test]
        public void IsSameOrSubtypeOfAny_WhenOtherTypesIsEmpty_ReturnsFalse()
        {
            var type = new FileType("id", "description", null);

            Assert.IsFalse(type.IsSameOrSubtypeOfAny());
        }

        [Test]
        public void IsSameOrSubtypeOfAny_WhenOtherTypesIsUnrelated_ReturnsFalse()
        {
            var superType = new FileType("superId", "superDescription", null);
            var type = new FileType("id", "description", superType);
            var unrelatedType = new FileType("otherId", "otherDescription", superType);

            Assert.IsFalse(type.IsSameOrSubtypeOfAny(unrelatedType));
        }

        [Test]
        public void IsSameOrSubtypeOfAny_WhenOtherTypesContainsUnrelatedAndSame_ReturnsTrue()
        {
            var superType = new FileType("superId", "superDescription", null);
            var type = new FileType("id", "description", superType);
            var unrelatedType = new FileType("otherId", "otherDescription", superType);

            Assert.IsTrue(type.IsSameOrSubtypeOfAny(unrelatedType, type));
        }

        [Test]
        public void ToString_Always_ReturnsIdConcatenatedWithDescription()
        {
            var type = new FileType("id", "description", null);

            Assert.AreEqual("id: description", type.ToString());
        }
    }
}
