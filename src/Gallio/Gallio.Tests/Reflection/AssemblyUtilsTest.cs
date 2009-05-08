using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection
{
    [TestsOn(typeof(AssemblyUtils))]
    public class AssemblyUtilsTest
    {
        [Test]
        public void IsAssembly_WhenStreamIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssemblyUtils.IsAssembly(null));
        }

        [Test]
        public void IsAssembly_WhenStreamIsEmpty_ReturnsFalse()
        {
            var stream = new MemoryStream();

            Assert.IsFalse(AssemblyUtils.IsAssembly(stream));
        }

        [Test]
        public void IsAssembly_WhenStreamDoesNotContainPEHeaderSignature_ReturnsFalse()
        {
            var stream = new MemoryStream();
            stream.SetLength(1024); // only contains nulls

            Assert.IsFalse(AssemblyUtils.IsAssembly(stream));
        }

        [Test]
        public void IsAssembly_WhenStreamIsACLRAssembly_ReturnsTrue()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Assert.IsTrue(AssemblyUtils.IsAssembly(stream));
            }
        }

        [Test]
        public void IsAssembly_WhenStreamIsAPEFileButNotAnAssembly_ReturnsFalse()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"kernel32.dll");
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Assert.IsFalse(AssemblyUtils.IsAssembly(stream));
            }
        }
    }
}
