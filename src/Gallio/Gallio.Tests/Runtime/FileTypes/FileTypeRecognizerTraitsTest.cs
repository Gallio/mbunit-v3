using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Runtime.FileTypes;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.FileTypes
{
    [TestsOn(typeof(FileTypeRecognizerTraits))]
    public class FileTypeRecognizerTraitsTest
    {
        [Test]
        public void Constructor_WhenIdIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FileTypeRecognizerTraits(null, "description"));
        }

        [Test]
        public void Constructor_WhenIdIsEmpty_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FileTypeRecognizerTraits("", "description"));
            Assert.Contains(ex.Message, "The file type id must not be empty.");
        }

        [Test]
        public void Constructor_WhenDescriptionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FileTypeRecognizerTraits("id", null));
        }

        [Test]
        public void Constructor_WhenArgumentsValid_SetsProperties()
        {
            var traits = new FileTypeRecognizerTraits("id", "description");

            Assert.AreEqual("id", traits.Id);
            Assert.AreEqual("description", traits.Description);
        }
    }
}
