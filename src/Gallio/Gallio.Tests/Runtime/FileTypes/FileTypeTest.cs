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

            Assert.IsTrue(type.IsSameOrSubtypeOf(unrelatedType));
        }

        [Test]
        public void ToString_Always_ReturnsIdConcatenatedWithDescription()
        {
            var type = new FileType("id", "description", null);

            Assert.AreEqual("id: description", type.ToString());
        }
    }
}
