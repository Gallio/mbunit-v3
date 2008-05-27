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
using System.IO;
using System.Text;
using Gallio.Runner.Caching;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Caching
{
    [TestsOn(typeof(SimpleDiskCache))]
    public class SimpleDiskCacheTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfPathIsNull()
        {
            new SimpleDiskCache(null);
        }

        [Test]
        public void ShouldUseTheFullPathOfTheCacheDirectory()
        {
            SimpleDiskCache cache = new SimpleDiskCache("foo");
            Assert.AreEqual(cache.CacheDirectoryPath, Path.GetFullPath("foo"));
        }

        [Test]
        public void GroupsThrowIfKeyIsNull()
        {
            SimpleDiskCache cache = new SimpleDiskCache("foo");
            InterimAssert.Throws<ArgumentNullException>(delegate { GC.KeepAlive(cache.Groups[null]); });
        }

        public class WithInitiallyNonExistentDiskCache
        {
            private SimpleDiskCache cache;

            [SetUp]
            public void SetUp()
            {
                cache = new SimpleDiskCache(Path.Combine(Path.GetTempPath(), "SimpleDiskCacheTest"));
                cache.Purge();
            }

            public void TearDown()
            {
                cache.Purge();
            }

            [Test]
            public void DifferentGroupsHaveDifferentLocations()
            {
                Assert.AreNotEqual(cache.Groups["A"].Location.FullName, cache.Groups["B"].Location.FullName);
            }

            [Test]
            public void GroupsDoExistUntilCreated()
            {
                Assert.IsFalse(cache.Groups["A"].Exists);
                cache.Groups["A"].Create();
                Assert.IsTrue(cache.Groups["A"].Exists);
            }

            [Test]
            public void GroupsCeaseToExistOnceDeleted()
            {
                Assert.IsFalse(cache.Groups["A"].Exists);
                cache.Groups["A"].Create();
                Assert.IsTrue(cache.Groups["A"].Exists);
                cache.Groups["A"].Delete();
                Assert.IsFalse(cache.Groups["A"].Exists);
            }

            [Test]
            public void NonExistentGroupsDoNothingWhenDeletedRedundantly()
            {
                cache.Groups["A"].Delete();
                Assert.IsFalse(cache.Groups["A"].Exists);
            }

            [Test]
            public void ExistentGroupsDoNothingWhenCreatedRedundantly()
            {
                cache.Groups["A"].Create();
                Assert.IsTrue(cache.Groups["A"].Exists);
                cache.Groups["A"].Create();
                Assert.IsTrue(cache.Groups["A"].Exists);
            }

            [Test]
            public void GroupsAreCreatedImplicitlyWhenAFileIsOpenedWithOptionToCreate()
            {
                cache.Groups["A"].OpenFile("file", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None).Close();
                Assert.IsTrue(cache.Groups["A"].Exists);
            }

            [Test]
            public void GroupsAreNotCreatedImplicitlyWhenAFileIsOpenedWithoutOptionToCreate()
            {
                InterimAssert.Throws<DiskCacheException>(delegate { cache.Groups["A"].OpenFile("file", FileMode.Open, FileAccess.ReadWrite, FileShare.None); });
                Assert.IsFalse(cache.Groups["A"].Exists);
            }

            [Test]
            public void GroupsAreCreatedImplicitlyWhenASubdirectoryIsCreated()
            {
                cache.Groups["A"].CreateSubdirectory("foo");
                Assert.IsTrue(cache.Groups["A"].Exists);
            }

            [Test, ExpectedArgumentNullException]
            public void OpenFileThrowsIfPathIsNull()
            {
                cache.Groups["A"].OpenFile(null, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }

            [Test, ExpectedArgumentNullException]
            public void CreateSubdirectoryThrowsIfPathIsNull()
            {
                cache.Groups["A"].CreateSubdirectory(null);
            }

            [Test, ExpectedArgumentNullException]
            public void GetFileInfoThrowsIfPathIsNull()
            {
                cache.Groups["A"].GetFileInfo(null);
            }

            [Test, ExpectedArgumentNullException]
            public void GetSubdirectoryInfoThrowsIfPathIsNull()
            {
                cache.Groups["A"].GetSubdirectoryInfo(null);
            }

            [Test]
            public void CachePropertyOfGroupIsSameAsCache()
            {
                Assert.AreSame(cache, cache.Groups["A"].Cache);
            }

            [Test]
            public void LocationOfGroupIsASubdirectoryOfTheCache()
            {
                Assert.AreEqual(cache.CacheDirectoryPath, cache.Groups["A"].Location.Parent.FullName);
            }

            [Test]
            public void GetFileInfoReturnsAFileWithinTheGroup()
            {
                Assert.AreEqual(Path.Combine(cache.Groups["A"].Location.FullName, "Abc\\def.txt"), cache.Groups["A"].GetFileInfo("Abc\\def.txt").FullName);
            }

            [Test]
            public void GetSubdirectoryInfoReturnsADirectoryWithinTheGroup()
            {
                Assert.AreEqual(Path.Combine(cache.Groups["A"].Location.FullName, "Abc\\def"), cache.Groups["A"].GetSubdirectoryInfo("Abc\\def").FullName);
            }

            [Test]
            public void OpenFileReallyOpensAFileAndCreatesItsContainingDirectory()
            {
                using (StreamWriter writer = new StreamWriter(cache.Groups["A"].OpenFile("foo\\bar.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)))
                {
                    writer.Write("foobar");
                }

                Assert.AreEqual("foobar", File.ReadAllText(Path.Combine(cache.Groups["A"].Location.FullName, "foo\\bar.txt")));
            }

            [Test]
            public void CreateSubdirectoryReallyCreatesADirectory()
            {
                cache.Groups["A"].CreateSubdirectory("foo\\bar");
                Assert.IsTrue(Directory.Exists(Path.Combine(cache.Groups["A"].Location.FullName, "foo\\bar")));
            }
        }
    }
}
