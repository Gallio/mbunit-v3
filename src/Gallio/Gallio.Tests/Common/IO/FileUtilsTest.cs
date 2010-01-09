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
using Gallio.Common.IO;
using MbUnit.Framework;

namespace Gallio.Tests.Common.IO
{
    [TestsOn(typeof(FileUtils))]
    public class FileUtilsTest
    {
        [Test]
        public void MakeAbsolutePath_WhenRelativePathIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => FileUtils.MakeAbsolutePath(null, "abc"));
        }

        [Test]
        public void MakeAbsolutePath_WhenBasePathPathIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => FileUtils.MakeAbsolutePath(@"C:\abc", null));
        }

        [Test]
        [Row(@"C.txt", @"C:\A\B", @"C:\A\B\C.txt")]
        [Row(@"C.txt", @"C:\A\B\", @"C:\A\B\C.txt")]
        [Row(@"..\..\C.txt", @"C:\A\B\", @"C:\C.txt")]
        [Row(@"D:\X\Y\Z.txt", @"C:\A\B\", @"D:\X\Y\Z.txt")]
        [Row(@"\\Machine\X\Y\Z.txt", @"\\Machine\X\W", @"\\Machine\X\Y\Z.txt")]
        [Row(@"..\Y\Z.txt", @"\\Machine\X\W", @"\\Machine\X\Y\Z.txt")]
        public void MakeAbsolutePath(string relativePath, string basePath, string expectedAbsolutePath)
        {
            string absolutePath = FileUtils.MakeAbsolutePath(relativePath, basePath);
            Assert.AreEqual(expectedAbsolutePath, absolutePath);
        }

        [Test]
        public void MakeRelativePath_WhenAbsolutePathIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => FileUtils.MakeRelativePath(null, "abc"));
        }

        [Test]
        public void MakeRelativePath_WhenBasePathPathIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => FileUtils.MakeRelativePath(@"C:\abc", null));
        }

        [Test]
        [Row(@"C:\A\B\C.txt", @"C:\", @"A\B\C.txt")]
        [Row(@"C:\A\B\C.txt", @"C:\A\B\X", @"..\C.txt")]
        [Row(@"C:\A\B\C.txt", @"C:\A\X\Y", @"..\..\B\C.txt")]
        [Row(@"C:\A\B\C.txt", @"C:\A\B", @"C.txt")]
        [Row(@"C:\A\B\C.txt", @"C:\A\B\", @"C.txt")]
        [Row(@"C:\A\B\C.txt", @"C:\A", @"B\C.txt")]
        [Row(@"C:\A\B\C.txt", @"C:\X", @"..\A\B\C.txt")]
        [Row(@"C:\A\B\C.txt", @"C:\X\Y", @"..\..\A\B\C.txt")]
        [Row(@"C:\A\B\C.txt", @"C:\X\Y\Z", @"..\..\..\A\B\C.txt")]
        [Row(@"\\Machine\X\Y\Z.txt", @"C:\A\B\", @"\\Machine\X\Y\Z.txt")]
        [Row(@"\\Machine\X\Y\Z.txt", @"\\Machine\X\W", @"..\Y\Z.txt")]
        [Row(@"C:\A\B", @"C:\A\B", @".")]
        [Row(@"C:\A\B\", @"C:\A\B\", @".")]
        [Row(@"C:\A\B\", @"C:\A\B", @".")]
        [Row(@"C:\A\B", @"C:\A\B", @".")]
        public void MakeRelativePath(string absolutePath, string basePath, string expectedRelativePath)
        {
            string relativePath = FileUtils.MakeRelativePath(absolutePath, basePath);
            Assert.AreEqual(expectedRelativePath, relativePath);
        }
    }
}
