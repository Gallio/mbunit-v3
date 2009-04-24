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
