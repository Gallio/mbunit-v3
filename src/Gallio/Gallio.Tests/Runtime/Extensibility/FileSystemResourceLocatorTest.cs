using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(FileSystemResourceLocator))]
    public class FileSystemResourceLocatorTest
    {
        [Test]
        public void Constructor_WhenBaseDirectoryIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FileSystemResourceLocator(null));
        }

        [Test]
        public void Constructor_WhenArgumentsValid_InitializesProperties()
        {
            var locator = new FileSystemResourceLocator(new DirectoryInfo(@"C:\SomeFolder"));

            Assert.AreEqual(@"C:\SomeFolder", locator.BaseDirectory.ToString());
        }

        [Test]
        public void GetFullPath_WhenRelativePathNull_Throws()
        {
            var locator = new FileSystemResourceLocator(new DirectoryInfo(@"C:\SomeFolder"));

            Assert.Throws<ArgumentNullException>(() => locator.GetFullPath(null));
        }

        [Test]
        public void GetFullPath_WhenArgumentsValid_ReturnsCombinedFilePathDerivedFromBaseDirectory()
        {
            var locator = new FileSystemResourceLocator(new DirectoryInfo(@"C:\SomeFolder"));

            var path = locator.GetFullPath("somefile.txt");
            Assert.AreEqual(@"C:\SomeFolder\somefile.txt", path);
        }
    }
}
