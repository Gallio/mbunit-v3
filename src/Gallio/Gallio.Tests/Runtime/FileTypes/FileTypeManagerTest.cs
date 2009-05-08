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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.FileTypes;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.FileTypes
{
    [TestsOn(typeof(FileTypeManager))]
    public class FileTypeManagerTest
    {
        [Test]
        public void Constructor_WhenHandlesArrayIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FileTypeManager(null));
        }

        [Test]
        public void Constructor_WhenHandlesArrayContainsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FileTypeManager(
                new ComponentHandle<IFileTypeRecognizer, FileTypeRecognizerTraits>[] { null }));
        }

        [Test]
        public void Constructor_WhenHandlesArrayContainsDoublyDefinedType_Throws()
        {
            var handles = CreateRecognizerHandles(
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("TypeA", "A") },
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("TypeA", "Duplicate") });

            var ex = Assert.Throws<InvalidOperationException>(() => new FileTypeManager(handles));
            Assert.AreEqual("There appear to be multiple file types registered with id 'TypeA'.", ex.Message);
        }

        [Test]
        public void Constructor_WhenHandlesArrayContainsReferenceToUnknownSuperType_Throws()
        {
            var handles = CreateRecognizerHandles(
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("TypeA", "A") { SuperTypeId = "TypeB" } });

            var ex = Assert.Throws<InvalidOperationException>(() => new FileTypeManager(handles));
            Assert.AreEqual("File type 'TypeA' refers to super type 'TypeB' but the super type is not registered.", ex.Message);
        }

        [Test]
        public void Constructor_WhenHandlesArrayContainsCircularReferenceToSuperType_Throws()
        {
            var handles = CreateRecognizerHandles(
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("TypeA", "A") { SuperTypeId = "TypeB" } },
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("TypeB", "B") { SuperTypeId = "TypeA" } });

            var ex = Assert.Throws<InvalidOperationException>(() => new FileTypeManager(handles));
            Assert.AreEqual("File type 'TypeB' contains a circular reference to super type 'TypeA'.", ex.Message);
        }

        [Test]
        public void UnknownFileType_ReturnsAFileTypeCalledUnknown()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            FileType fileType = fileTypeManager.UnknownFileType;

            Assert.Multiple(() =>
            {
                Assert.AreEqual("Unknown", fileType.Id);
                Assert.AreEqual("File of unknown type.", fileType.Description);
                Assert.IsNull(fileType.SuperType);
            });
        }

        [Test]
        public void GetFileTypeById_WhenIdIsNull_Throws()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            Assert.Throws<ArgumentNullException>(() => fileTypeManager.GetFileTypeById(null));
        }

        [Test]
        public void GetFileTypeById_WhenIdIsTheUnknownFileTypeId_ReturnsUnknownFileType()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            FileType fileType = fileTypeManager.GetFileTypeById("Unknown");
            Assert.AreSame(fileTypeManager.UnknownFileType, fileType);
        }

        [Test]
        public void GetFileTypeById_WhenIdIsKnown_ReturnsFileType()
        {
            var handles = CreateRecognizerHandles(
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("Type", "A file type.") });
            var fileTypeManager = new FileTypeManager(handles);

            FileType fileType = fileTypeManager.GetFileTypeById("Type");
            Assert.AreEqual("Type", fileType.Id);
            Assert.AreEqual("A file type.", fileType.Description);
        }

        [Test]
        public void GetFileTypeById_WhenIdIsNotKnown_ReturnsNull()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            Assert.IsNull(fileTypeManager.GetFileTypeById("Type"));
        }

        [Test]
        public void GetFileTypes_ReturnsListOfAllFileTypesIncludingUnknown()
        {
            var handles = CreateRecognizerHandles(
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("TypeA", "A") },
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("TypeB", "B") { SuperTypeId = "TypeC" } },
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("TypeC", "C") { SuperTypeId = "TypeD" } },
                new RecognizerInfo { Traits = new FileTypeRecognizerTraits("TypeD", "D") });
            var fileTypeManager = new FileTypeManager(handles);

            var expectedFileTypes = new FileType[5];
            expectedFileTypes[0] = new FileType("TypeA", "A", null);
            expectedFileTypes[3] = new FileType("TypeD", "D", null);
            expectedFileTypes[2] = new FileType("TypeC", "C", expectedFileTypes[3]);
            expectedFileTypes[1] = new FileType("TypeB", "B", expectedFileTypes[2]);
            expectedFileTypes[4] = fileTypeManager.UnknownFileType;

            IList<FileType> actualFileTypes = fileTypeManager.GetFileTypes();
            Assert.Over.KeyedPairs(
                expectedFileTypes.ToDictionary(x => x.Id),
                actualFileTypes.ToDictionary(x => x.Id),
                (x, y) => AssertEx.That(() => AreEquivalent(x, y)));
        }

        [Test]
        public void IdentifyFileType_FileInfo_WhenFileInfoIsNull_Throws()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            Assert.Throws<ArgumentNullException>(() => fileTypeManager.IdentifyFileType((FileInfo)null));
        }

        [Test]
        public void IdentifyFileType_FileInspector_WhenFileInspectorIsNull_Throws()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            Assert.Throws<ArgumentNullException>(() => fileTypeManager.IdentifyFileType((IFileInspector)null));
        }

        [Test]
        public void IdentifyFileType_FileInfoWithCandidates_WhenFileInfoIsNull_Throws()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            Assert.Throws<ArgumentNullException>(() => fileTypeManager.IdentifyFileType((FileInfo)null, new FileType[0]));
        }

        [Test]
        public void IdentifyFileType_FileInspectorWithCandidates_WhenFileInspectorIsNull_Throws()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            Assert.Throws<ArgumentNullException>(() => fileTypeManager.IdentifyFileType((IFileInspector)null, new FileType[0]));
        }

        [Test]
        public void IdentifyFileType_FileInfoWithCandidates_WhenCandidatesIsNull_Throws()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            Assert.Throws<ArgumentNullException>(() => fileTypeManager.IdentifyFileType(new FileInfo(@"C:\"), null));
        }

        [Test]
        public void IdentifyFileType_FileInspectorWithCandidates_WhenCandidatesIsNull_Throws()
        {
            var handles = CreateRecognizerHandles();
            var fileTypeManager = new FileTypeManager(handles);

            Assert.Throws<ArgumentNullException>(() => fileTypeManager.IdentifyFileType(MockRepository.GenerateStub<IFileInspector>(), null));
        }

        [Test]
        [Row(null, null, true, true, Description = "No filename regex, no contents regex, recognizer returns true.")]
        [Row(null, null, false, false, Description = "No filename regex, no contents regex, recognizer returns false.")]
        [Row(".*.txt", null, true, true, Description = "Matched filename regex, no contents regex, recognizer returns true.")]
        [Row(".*.txt", null, false, false, Description = "Matched filename regex, no contents regex, recognizer returns false.")]
        [Row(".*.dll", null, true, false, Description = "Unmatched filename regex, no contents regex, recognizer returns true.")]
        [Row(".*.dll", null, false, false, Description = "Unmatched filename regex, no contents regex, recognizer returns false.")]
        [Row(null, "contents", true, true, Description = "No filename regex, matched contents regex, recognizer returns true.")]
        [Row(null, "contents", false, false, Description = "No filename regex, matched contents regex, recognizer returns false.")]
        [Row(null, "different", true, false, Description = "No filename regex, unmatched contents regex, recognizer returns true.")]
        [Row(null, "different", false, false, Description = "No filename regex, unmatched contents regex, recognizer returns false.")]
        public void IdentifyFileType_FileInspector_WhenMatchCriteriaAreVaried_ReturnsTypeIfMatchedOrUnknownOtherwise(string fileNameRegex, string contentsRegex, bool recognizerResult,
            bool expectedMatch)
        {
            var inspector = new FakeFileInspector()
            {
                FileInfo = new FileInfo(@"C:\file.txt"),
                Contents = "contents"
            };                
            var recognizer = MockRepository.GenerateStub<IFileTypeRecognizer>();
            recognizer.Stub(x => x.IsRecognizedFile(inspector)).Return(recognizerResult);
            var handles = CreateRecognizerHandles(
                new RecognizerInfo
                {
                    Traits = new FileTypeRecognizerTraits("TypeA", "A")
                        {
                            FileNameRegex = fileNameRegex,
                            ContentsRegex = contentsRegex
                        },
                    Recognizer = recognizer
                });
            var fileTypeManager = new FileTypeManager(handles);

            FileType fileType = fileTypeManager.IdentifyFileType(inspector);

            Assert.AreEqual(expectedMatch ? "TypeA" : "Unknown", fileType.Id);
        }

        [Test]
        [Row(true, true, false)]
        [Row(true, false, false)]
        [Row(false, true, true)]
        [Row(false, false, false)]
        public void IdentifyFileType_FileInspector_WhenFileInfoNotAvailable_ReturnsTypeOnlyIfNoFilenameCriteriaAndRecognizerMatches(
            bool filenameCriteria, bool recognizerResult, bool expectedMatch)
        {
            var inspector = new FakeFileInspector();
            var recognizer = MockRepository.GenerateStub<IFileTypeRecognizer>();
            recognizer.Stub(x => x.IsRecognizedFile(inspector)).Return(recognizerResult);
            var handles = CreateRecognizerHandles(
                new RecognizerInfo
                {
                    Traits = new FileTypeRecognizerTraits("TypeA", "A")
                    {
                        FileNameRegex = filenameCriteria ? ".*" : null
                    },
                    Recognizer = recognizer
                });
            var fileTypeManager = new FileTypeManager(handles);

            FileType fileType = fileTypeManager.IdentifyFileType(inspector);

            Assert.AreEqual(expectedMatch ? "TypeA" : "Unknown", fileType.Id);
        }

        [Test]
        [Row(true, true, false)]
        [Row(true, false, false)]
        [Row(false, true, true)]
        [Row(false, false, false)]
        public void IdentifyFileType_FileInspector_WhenContentsNotAvailable_ReturnsTypeOnlyIfNoContentCriteriaAndRecognizerMatches(
            bool contentCriteria, bool recognizerResult, bool expectedMatch)
        {
            var inspector = new FakeFileInspector();
            var recognizer = MockRepository.GenerateStub<IFileTypeRecognizer>();
            recognizer.Stub(x => x.IsRecognizedFile(inspector)).Return(recognizerResult);
            var handles = CreateRecognizerHandles(
                new RecognizerInfo
                {
                    Traits = new FileTypeRecognizerTraits("TypeA", "A")
                    {
                        ContentsRegex = contentCriteria ? ".*" : null
                    },
                    Recognizer = recognizer
                });
            var fileTypeManager = new FileTypeManager(handles);

            FileType fileType = fileTypeManager.IdentifyFileType(inspector);

            Assert.AreEqual(expectedMatch ? "TypeA" : "Unknown", fileType.Id);
        }

        [Test]
        [Row(true, true, true, true, "SubType", Description = "Supertype included, subtype included, supertype matched, subtype matched, should match subtype.")]
        [Row(true, true, true, false, "SuperType", Description = "Supertype included, subtype included, supertype matched, subtype unmatched, should match supertype.")]
        [Row(true, true, false, true, "Unknown", Description = "Supertype included, subtype included, supertype unmatched, subtype matched, should match unknown.")]
        [Row(false, true, true, true, "SubType", Description = "Supertype excluded, subtype included, supertype matched, subtype matched, should match subtype.")]
        [Row(true, false, true, true, "SuperType", Description = "Supertype included, subtype excluded, supertype matched, subtype matched, should match supertype.")]
        [Row(false, true, true, false, "Unknown", Description = "Supertype excluded, subtype included, supertype matched, subtype unmatched, should match unknown.")]
        [Row(false, false, true, true, "Unknown", Description = "Supertype excluded, subtype excluded, supertype matched, subtype matched, should match unknown.")]
        public void IdentifyFileType_FileInspectorWithCandidates_WhenMatchCriteriaAndCandidatesAreVaried_ReturnsTypeIfMatchedOrUnknownOtherwise(
            bool includeSupertypeAsCandidate, bool includeSubtypeAsCandidate,
            bool superRecognizerResult, bool subRecognizerResult,
            string expectedMatchType)
        {
            var inspector = new FakeFileInspector()
            {
                FileInfo = new FileInfo(@"C:\file.txt"),
                Contents = "contents"
            };
            var superRecognizer = MockRepository.GenerateStub<IFileTypeRecognizer>();
            superRecognizer.Stub(x => x.IsRecognizedFile(inspector)).Return(superRecognizerResult);
            var subRecognizer = MockRepository.GenerateStub<IFileTypeRecognizer>();
            subRecognizer.Stub(x => x.IsRecognizedFile(inspector)).Return(subRecognizerResult);
            var handles = CreateRecognizerHandles(
                new RecognizerInfo
                {
                    Traits = new FileTypeRecognizerTraits("SuperType", "Super"),
                    Recognizer = superRecognizer
                },
                new RecognizerInfo
                {
                    Traits = new FileTypeRecognizerTraits("SubType", "Sub") { SuperTypeId = "SuperType" },
                    Recognizer = subRecognizer
                });
            var fileTypeManager = new FileTypeManager(handles);
            var candidates = new List<FileType>();
            if (includeSupertypeAsCandidate)
                candidates.Add(fileTypeManager.GetFileTypeById("SuperType"));
            if (includeSubtypeAsCandidate)
                candidates.Add(fileTypeManager.GetFileTypeById("SubType"));

            FileType fileType = fileTypeManager.IdentifyFileType(inspector, candidates);

            Assert.AreEqual(expectedMatchType, fileType.Id);
        }

        private static bool AreEquivalent(FileType left, FileType right)
        {
            if (left == null && right == null)
                return true;
            if (left == null || right == null)
                return false;

            return left.Id == right.Id
                && left.Description == right.Description
                && AreEquivalent(left.SuperType, right.SuperType);
        }

        private sealed class RecognizerInfo
        {
            public IFileTypeRecognizer Recognizer;
            public FileTypeRecognizerTraits Traits;
        }

        private static ComponentHandle<IFileTypeRecognizer, FileTypeRecognizerTraits>[] CreateRecognizerHandles(
            params RecognizerInfo[] recognizerInfos)
        {
            return GenericCollectionUtils.ConvertAllToArray(recognizerInfos, recognizerInfo =>
                ComponentHandle.CreateStub("component",
                    recognizerInfo.Recognizer ?? MockRepository.GenerateStub<IFileTypeRecognizer>(),
                    recognizerInfo.Traits ?? new FileTypeRecognizerTraits("Dummy", "Dummy")));
        }

        private class FakeFileInspector : IFileInspector
        {
            public FileInfo FileInfo { get; set; }

            public Stream Stream { get; set; }

            public string Contents { get; set; }

            public bool TryGetFileInfo(out FileInfo fileInfo)
            {
                fileInfo = FileInfo;
                return fileInfo != null;
            }

            public bool TryGetContents(out string contents)
            {
                contents = Contents;
                return contents != null;
            }

            public bool TryGetStream(out Stream stream)
            {
                stream = Stream;
                return stream != null;
            }
        }
    }
}
