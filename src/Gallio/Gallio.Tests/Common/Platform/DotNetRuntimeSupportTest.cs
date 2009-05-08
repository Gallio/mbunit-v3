using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Common.Platform;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Platform
{
    [TestsOn(typeof(DotNetFrameworkSupport))]
    public class DotNetFrameworkSupportTest
    {
        [Test]
        public void IsAssembly_WhenStreamIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => DotNetRuntimeSupport.IsAssembly((string) null));
        }

        [Test]
        public void IsAssembly_WhenStreamIsEmpty_ReturnsFalse()
        {
            var stream = new MemoryStream();

            Assert.IsFalse(DotNetRuntimeSupport.IsAssembly(stream));
        }

        [Test]
        public void IsAssembly_WhenStreamDoesNotContainPEHeaderSignature_ReturnsFalse()
        {
            var stream = new MemoryStream();
            stream.SetLength(1024); // only contains nulls

            Assert.IsFalse(DotNetRuntimeSupport.IsAssembly(stream));
        }

        [Test]
        public void IsAssembly_WhenStreamIsACLRAssembly_ReturnsTrue()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Assert.IsTrue(DotNetRuntimeSupport.IsAssembly(stream));
            }
        }

        [Test]
        public void IsAssembly_WhenStreamIsAPEFileButNotAnAssembly_ReturnsFalse()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"kernel32.dll");
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Assert.IsFalse(DotNetRuntimeSupport.IsAssembly(stream));
            }
        }
    }
}