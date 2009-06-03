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
using Gallio.Common.Policies;
using Gallio.Runtime.Preferences;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Preferences
{
    [TestsOn(typeof(FilePreferenceStore))]
    public class FilePreferenceStoreTest
    {
        [Test]
        public void Constructor_WhenDirectoryIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FilePreferenceStore(null));
        }

        [Test]
        public void Constructor_WhenDirectoryIsValid_InitializesPreferenceStoreDir()
        {
            var directory = new DirectoryInfo(@"C:\Foo");

            var preferenceStore = new FilePreferenceStore(directory);

            Assert.AreEqual(directory, preferenceStore.PreferenceStoreDir);
        }

        [Test]
        public void Indexer_WhenNameIsNull_Throws()
        {
            var directory = new DirectoryInfo(@"C:\Foo");
            var preferenceStore = new FilePreferenceStore(directory);

            object x;
            Assert.Throws<ArgumentNullException>(() => { x = preferenceStore[null]; });
        }

        [Test]
        public void Indexer_WhenNameIsValid_ReturnsFilePreferenceSet()
        {
            var directory = new DirectoryInfo(@"C:\Foo");
            var preferenceStore = new FilePreferenceStore(directory);

            var preferenceSet = (FilePreferenceSet)preferenceStore["Prefs"];

            Assert.AreEqual(new FileInfo(@"C:\Foo\Prefs.gallioprefs"), preferenceSet.PreferenceSetFile);
        }

        [Test]
        public void Indexer_WhenNameIsValid_ReturnsSameInstanceForSameName()
        {
            var directory = new DirectoryInfo(@"C:\Foo");
            var preferenceStore = new FilePreferenceStore(directory);

            Assert.AreSame(preferenceStore["name"], preferenceStore["name"]);
        }

        [Test]
        public void Indexer_WhenNameIsValidButContainsInvalidCharsForAFileName_ReturnsFilePreferenceSetInstanceWithEncodedFileName()
        {
            var directory = new DirectoryInfo(@"C:\Foo");
            var preferenceStore = new FilePreferenceStore(directory);

            var preferenceSet = (FilePreferenceSet)preferenceStore["Prefs?\\Foo"];

            Assert.AreEqual(new FileInfo(@"C:\Foo\Prefs__Foo.gallioprefs"), preferenceSet.PreferenceSetFile);
        }

        [Test]
        public void Indexer_WhenNameIsValid_ReturnsDifferentInstanceForDifferentName()
        {
            var directory = new DirectoryInfo(@"C:\Foo");
            var preferenceStore = new FilePreferenceStore(directory);

            Assert.AreNotSame(preferenceStore["name"], preferenceStore["different"]);
        }
    }
}
