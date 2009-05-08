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
using System.IO;
using System.Reflection;
using Gallio.Common.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Reflection
{
    [TestsOn(typeof(AssemblyUtils))]
    public class AssemblyUtilsTest
    {
        [Test]
        public void IsAssembly_WhenStreamIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssemblyUtils.IsAssembly((string) null));
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