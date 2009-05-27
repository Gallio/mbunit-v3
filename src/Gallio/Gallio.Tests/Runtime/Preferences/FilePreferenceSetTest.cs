using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Runtime.Preferences;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Preferences
{
    [TestsOn(typeof(FilePreferenceSet))]
    public class FilePreferenceSetTest
    {
        public class TopLevelOperations
        {
            [Test]
            public void Constructor_WhenFileIsNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new FilePreferenceSet(null));
            }

            [Test]
            public void Constructor_WhenFileIsValid_InitializesPreferenceSetFile()
            {
                var file = new FileInfo(@"C:\Foo\Prefs.gallioprefs");

                var preferenceSet = new FilePreferenceSet(file);

                Assert.AreEqual(file, preferenceSet.PreferenceSetFile);
            }

            [Test]
            public void Read_WhenActionIsNull_Throws()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();

                Assert.Throws<ArgumentNullException>(() => preferenceSet.Read(null));
            }

            [Test]
            public void Read_WhenFuncIsNull_Throws()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();

                Assert.Throws<ArgumentNullException>(() => preferenceSet.Read<object>(null));
            }

            [Test]
            public void Write_WhenActionIsNull_Throws()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();

                Assert.Throws<ArgumentNullException>(() => preferenceSet.Write(null));
            }

            [Test]
            public void Write_WhenFuncIsNull_Throws()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();

                Assert.Throws<ArgumentNullException>(() => preferenceSet.Write<object>(null));
            }

            [Test]
            public void Read_WithFunc_ReturnsFuncResult()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();

                int result = preferenceSet.Read(reader => 42);

                Assert.AreEqual(42, result);
            }

            [Test]
            public void Write_WithFunc_ReturnsFuncResult()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();

                int result = preferenceSet.Write(writer => 42);

                Assert.AreEqual(42, result);
            }
        }

        public class ReaderOperations
        {
            [Test]
            public void GetSetting_WhenFileDoesNotExist_ReturnsNull()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();

                string result = preferenceSet.Read(reader => reader.GetSetting(new Key<string>("name")));

                Assert.IsNull(result);
            }

            [Test]
            public void GetSettingWithDefaultValue_WhenFileDoesNotExist_ReturnsDefaultValue()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();

                string result = preferenceSet.Read(reader => reader.GetSetting(new Key<string>("name"), "defaultValue"));

                Assert.AreEqual("defaultValue", result);
            }

            [Test]
            public void HasSetting_WhenFileDoesNotExist_ReturnsFalse()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();

                bool result = preferenceSet.Read(reader => reader.HasSetting(new Key<string>("name")));

                Assert.IsFalse(result);
            }

            [Test]
            public void GetSetting_WhenFileExistsButNameDoesNot_ReturnsNull()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));

                string result = preferenceSet.Read(reader => reader.GetSetting(new Key<string>("otherName")));

                Assert.IsNull(result);
            }

            [Test]
            public void GetSettingWithDefaultValue_WhenFileExistsButNameDoesNot_ReturnsDefaultValue()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));

                string result = preferenceSet.Read(reader => reader.GetSetting(new Key<string>("otherName"), "defaultValue"));

                Assert.AreEqual("defaultValue", result);
            }

            [Test]
            public void HasSetting_WhenFileExistsButNameDoesNot_ReturnsFalse()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));

                bool result = preferenceSet.Read(reader => reader.HasSetting(new Key<string>("otherName")));

                Assert.IsFalse(result);
            }

            [Test]
            public void GetSetting_WhenNameDefined_ReturnsValue()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));

                string result = preferenceSet.Read(reader => reader.GetSetting(new Key<string>("name")));

                Assert.AreEqual("value", result);
            }

            [Test]
            public void GetSettingWithDefaultValue_WhenNameDefined_ReturnsDefaultValue()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));

                string result = preferenceSet.Read(reader => reader.GetSetting(new Key<string>("name"), "defaultValue"));

                Assert.AreEqual("value", result);
            }

            [Test]
            public void HasSetting_WhenNameDefined_ReturnsTrue()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));

                bool result = preferenceSet.Read(reader => reader.HasSetting(new Key<string>("name")));

                Assert.IsTrue(result);
            }
        }

        public class WriterOperations
        {
            [Test]
            public void SetSetting_WhenNameAlreadyExistsAndValueIsNonNull_ReplacesIt()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "newValue"));

                string result = preferenceSet.Read(reader => reader.GetSetting(new Key<string>("name")));

                Assert.AreEqual("newValue", result);
            }

            [Test]
            public void SetSetting_WhenNameAlreadyExistsAndValueIsNull_RemovesIt()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), null));

                bool result = preferenceSet.Read(reader => reader.HasSetting(new Key<string>("name")));

                Assert.IsFalse(result);
            }

            [Test]
            public void RemoveSetting_WhenNameDoesNotExist_DoesNothing()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.RemoveSetting(new Key<string>("name")));

                bool result = preferenceSet.Read(reader => reader.HasSetting(new Key<string>("name")));

                Assert.IsFalse(result);
            }

            [Test]
            public void RemoveSetting_WhenNameExists_RemovesIt()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));
                preferenceSet.Write(writer => writer.RemoveSetting(new Key<string>("name")));

                bool result = preferenceSet.Read(reader => reader.HasSetting(new Key<string>("name")));

                Assert.IsFalse(result);
            }

            [Test]
            public void SetSetting_WhenDataTypeIsNotString_ConvertsItForRoundTrip()
            {
                var preferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                preferenceSet.Write(writer => writer.SetSetting(new Key<int>("name"), 42));

                int result = preferenceSet.Read(reader => reader.GetSetting<int>(new Key<int>("name")));

                Assert.AreEqual(42, result);
            }
        }

        public class Concurrency
        {
            [Test]
            public void ShouldMigrateSettingsAcrossInstancesWhenFileModifiedExternally()
            {
                var firstPreferenceSet = CreateFilePreferenceSetWithNonExistantTempFile();
                var secondPreferenceSet = new FilePreferenceSet(firstPreferenceSet.PreferenceSetFile);

                firstPreferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "value"));
                Assert.AreEqual("value", secondPreferenceSet.Read(reader => reader.GetSetting(new Key<string>("name"))),
                    "Setting written to first set should be reflected in second set.");

                secondPreferenceSet.Write(writer => writer.SetSetting(new Key<string>("name"), "newValue"));
                Assert.AreEqual("newValue", firstPreferenceSet.Read(reader => reader.GetSetting(new Key<string>("name"))),
                    "Setting written to second set should be reflected in first set.");
            }
        }

        private static FilePreferenceSet CreateFilePreferenceSetWithNonExistantTempFile()
        {
            DirectoryInfo tempDir = SpecialPathPolicy.For<FilePreferenceSetTest>().GetTempDirectory();
            FileInfo tempFile = new FileInfo(Path.Combine(tempDir.FullName, "temp.gallioprefs"));

            if (tempFile.Exists)
                tempFile.Delete();

            return new FilePreferenceSet(tempFile);
        }
    }
}
