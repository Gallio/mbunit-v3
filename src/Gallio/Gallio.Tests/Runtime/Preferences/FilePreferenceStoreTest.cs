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
        public void Indexer_WhenNameIsValid_ReturnsDifferentInstanceForDifferentName()
        {
            var directory = new DirectoryInfo(@"C:\Foo");
            var preferenceStore = new FilePreferenceStore(directory);

            Assert.AreNotSame(preferenceStore["name"], preferenceStore["different"]);
        }
    }
}
