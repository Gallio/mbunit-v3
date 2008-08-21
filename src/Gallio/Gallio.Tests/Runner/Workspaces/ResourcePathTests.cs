// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio.Runner.Workspaces;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Runner.Workspaces
{
    [TestFixture]
    [TestsOn(typeof(ResourcePath))]
    [VerifyEqualityContract(typeof(ResourcePath))]
    public class ResourcePathTests : IEquivalenceClassProvider<ResourcePath>
    {
        /// <inheritdoc />
        public EquivalenceClassCollection<ResourcePath> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<ResourcePath>.FromDistinctInstances(
                ResourcePath.Empty,
                ResourcePath.Root,
                new ResourcePath(@"\foo"),
                new ResourcePath(@"\bar"),
                new ResourcePath(@"foo"),
                new ResourcePath(@"bar"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorThrowsIfPathIsNull()
        {
            new ResourcePath(null);
        }

        [Test]
        public void EmptyPaths()
        {
            ResourcePath path = new ResourcePath("");
            Assert.AreEqual("", path.Path);
            Assert.AreEqual("", path.ToString());
            Assert.IsTrue(path.IsEmpty);
            Assert.IsFalse(path.IsAbsolute);
            ArrayAssert.AreEqual(new string[] { }, path.Segments);
            Assert.AreEqual(ResourcePath.Empty, path.Container);
            Assert.AreEqual(ResourcePath.Empty, path);
        }

        [Test]
        public void RootPaths()
        {
            ResourcePath path = new ResourcePath("\\");
            Assert.AreEqual("\\", path.Path);
            Assert.AreEqual("\\", path.ToString());
            Assert.IsFalse(path.IsEmpty);
            Assert.IsTrue(path.IsAbsolute);
            ArrayAssert.AreEqual(new string[] { }, path.Segments);
            Assert.AreEqual(ResourcePath.Empty, path.Container);
            Assert.AreEqual(ResourcePath.Root, path);
        }

        [Test]
        public void RelativePaths()
        {
            ResourcePath path = new ResourcePath("foo\\somepath");
            Assert.AreEqual("foo\\somepath", path.Path);
            Assert.AreEqual("foo\\somepath", path.ToString());
            Assert.IsFalse(path.IsEmpty);
            Assert.IsFalse(path.IsAbsolute);
            ArrayAssert.AreEqual(new string[] { "foo", "somepath" }, path.Segments);
            Assert.AreEqual(new ResourcePath("foo"), path.Container);
            Assert.AreEqual(new ResourcePath("foo\\somepath"), path);
        }

        [Test]
        public void AbsolutePaths()
        {
            ResourcePath path = new ResourcePath("\\foo\\somepath");
            Assert.AreEqual("\\foo\\somepath", path.Path);
            Assert.AreEqual("\\foo\\somepath", path.ToString());
            Assert.IsFalse(path.IsEmpty);
            Assert.IsTrue(path.IsAbsolute);
            ArrayAssert.AreEqual(new string[] { "foo", "somepath" }, path.Segments);
            Assert.AreEqual(new ResourcePath("\\foo"), path.Container);
            Assert.AreEqual(new ResourcePath("\\foo\\somepath"), path);
        }

        [Test]
        [Row("", "", "")]
        [Row("\\", "", "\\")]
        [Row("", "\\", "\\")]
        [Row("abc", "def", "abc\\def")]
        [Row("\\abc", "def", "\\abc\\def")]
        [Row("abc", "\\def", "\\def")]
        [Row("\\abc", "\\def", "\\def")]
        public void CombineWithShouldReturnOtherPathIfAbsolute(string a, string b, string combined)
        {
            Assert.AreEqual(new ResourcePath(combined), new ResourcePath(a).CombinedWith(new ResourcePath(b)));
        }
    }
}
