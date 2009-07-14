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
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Gallio.Common.Collections;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.FileTypes
{
    /// <summary>
    /// A built-in file type manager that identified file types based on a
    /// set of registered <see cref="IFileTypeRecognizer" /> components.
    /// </summary>
    public class FileTypeManager : IFileTypeManager
    {
        private readonly FileType unknownFileType;
        private readonly List<FileType> fileTypes;
        private readonly Dictionary<string, FileTypeInfo> fileTypeInfos;
        private readonly List<FileTypeInfo> rootFileTypeInfos;

        /// <summary>
        /// Creates a file type manager.
        /// </summary>
        /// <param name="fileTypeRecognizerHandles">The file type recognizer component handles.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileTypeRecognizerHandles"/> is null.</exception>
        public FileTypeManager(ComponentHandle<IFileTypeRecognizer, FileTypeRecognizerTraits>[] fileTypeRecognizerHandles)
        {
            if (fileTypeRecognizerHandles == null || Array.IndexOf(fileTypeRecognizerHandles, null) >= 0)
                throw new ArgumentNullException("fileTypeRecognizerHandles");

            unknownFileType = new FileType("Unknown", "File of unknown type.", null);
            fileTypes = new List<FileType>();
            fileTypeInfos = new Dictionary<string, FileTypeInfo>();
            rootFileTypeInfos = new List<FileTypeInfo>();

            Initialize(fileTypeRecognizerHandles);

            // Note: The unknown type is not reachable from the roots so we pay no cost
            //       trying to apply a recognizer to it.  We add it to the other tables for
            //       lookup and enumeration only.
            fileTypes.Add(unknownFileType);
            fileTypeInfos.Add(unknownFileType.Id, new FileTypeInfo()
            {
                FileType = unknownFileType
            });
        }

        /// <inheritdoc />
        public FileType UnknownFileType
        {
            get { return unknownFileType; }
        }

        /// <inheritdoc />
        public FileType GetFileTypeById(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            FileTypeInfo fileTypeInfo;
            if (fileTypeInfos.TryGetValue(id, out fileTypeInfo))
                return fileTypeInfo.FileType;
            return null;
        }

        /// <inheritdoc />
        public IList<FileType> GetFileTypes()
        {
            return new ReadOnlyCollection<FileType>(fileTypes);
        }

        /// <inheritdoc />
        public FileType IdentifyFileType(FileInfo fileInfo)
        {
             if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            using (var fileInspector = new LazyFileInspector(fileInfo))
                return IdentifyFileTypeImpl(fileInspector, null);
        }

        /// <inheritdoc />
        public FileType IdentifyFileType(IFileInspector fileInspector)
        {
            if (fileInspector == null)
                throw new ArgumentNullException("fileInspector");

            return IdentifyFileTypeImpl(fileInspector, null);
        }

        /// <inheritdoc />
        public FileType IdentifyFileType(FileInfo fileInfo, IEnumerable<FileType> candidates)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");
            if (candidates == null)
                throw new ArgumentNullException("candidates");

            using (var fileInspector = new LazyFileInspector(fileInfo))
                return IdentifyFileTypeImpl(fileInspector, candidates);
        }

        /// <inheritdoc />
        public FileType IdentifyFileType(IFileInspector fileInspector, IEnumerable<FileType> candidates)
        {
            if (fileInspector == null)
                throw new ArgumentNullException("fileInspector");
            if (candidates == null)
                throw new ArgumentNullException("candidates");

            return IdentifyFileTypeImpl(fileInspector, candidates);
        }

        private FileType IdentifyFileTypeImpl(IFileInspector fileInspector, IEnumerable<FileType> candidates)
        {
            Dictionary<FileType, bool> fileTypeFilter = CreateFileTypeFilter(candidates);

            FileType fileType = IdentifyFileTypeRecursive(fileInspector, fileTypeFilter, rootFileTypeInfos);
            if (fileType == null)
                fileType = unknownFileType;
            return fileType;
        }

        private static Dictionary<FileType, bool> CreateFileTypeFilter(IEnumerable<FileType> candidates)
        {
            if (candidates == null)
                return null;

            Dictionary<FileType, bool> fileTypeFilter = new Dictionary<FileType, bool>();
            foreach (FileType fileType in candidates)
                AddFileTypeToFilter(fileTypeFilter, fileType, true);

            return fileTypeFilter;
        }

        private static void AddFileTypeToFilter(Dictionary<FileType, bool> fileTypeFilter, FileType fileType, bool isExplicitCandidate)
        {
            if (!isExplicitCandidate && fileTypeFilter.ContainsKey(fileType))
                return;

            fileTypeFilter[fileType] = isExplicitCandidate;

            if (fileType.SuperType != null)
                AddFileTypeToFilter(fileTypeFilter, fileType.SuperType, false);
        }

        private static FileType IdentifyFileTypeRecursive(IFileInspector fileInspector, Dictionary<FileType, bool> fileTypeFilter,
            IEnumerable<FileTypeInfo> fileTypeInfos)
        {
            foreach (var fileTypeInfo in fileTypeInfos)
            {
                bool isExplicitCandidate;
                if (fileTypeFilter != null)
                {
                    if (! fileTypeFilter.TryGetValue(fileTypeInfo.FileType, out isExplicitCandidate))
                        continue; // ignore if not in filter
                }
                else
                {
                    isExplicitCandidate = true;
                }

                if (fileTypeInfo.IsRecognizedFile(fileInspector))
                {
                    FileType subtype = IdentifyFileTypeRecursive(fileInspector, fileTypeFilter, fileTypeInfo.Subtypes);
                    if (subtype != null)
                        return subtype;

                    if (isExplicitCandidate)
                        return fileTypeInfo.FileType;
                }
            }

            return null;
        }

        private void Initialize(IEnumerable<ComponentHandle<IFileTypeRecognizer, FileTypeRecognizerTraits>> fileTypeRecognizerHandles)
        {
            foreach (var handle in fileTypeRecognizerHandles)
            {
                string id = handle.GetTraits().Id;
                if (fileTypeInfos.ContainsKey(id))
                    throw new InvalidOperationException(string.Format("There appear to be multiple file types registered with id '{0}'.", id));

                fileTypeInfos.Add(id, new FileTypeInfo() { RecognizerHandle = handle });
            }

            foreach (var fileTypeInfo in fileTypeInfos.Values)
            {
                FileTypeRecognizerTraits recognizerTraits = fileTypeInfo.RecognizerHandle.GetTraits();

                if (recognizerTraits.FileNameRegex != null)
                    fileTypeInfo.FileNameRegex = new Regex(recognizerTraits.FileNameRegex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                if (recognizerTraits.ContentsRegex != null)
                    fileTypeInfo.ContentsRegex = new Regex(recognizerTraits.ContentsRegex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                string superTypeId = recognizerTraits.SuperTypeId;
                if (superTypeId != null)
                {
                    FileTypeInfo superTypeInfo;
                    if (!fileTypeInfos.TryGetValue(superTypeId, out superTypeInfo))
                        throw new InvalidOperationException(string.Format("File type '{0}' refers to super type '{1}' but the super type is not registered.", recognizerTraits.Id, superTypeId));

                    if (fileTypeInfo.IsSameOrSubtype(superTypeInfo))
                        throw new InvalidOperationException(string.Format("File type '{0}' contains a circular reference to super type '{1}'.", recognizerTraits.Id, superTypeId));

                    superTypeInfo.AddSubtype(fileTypeInfo);
                }
                else
                {
                    rootFileTypeInfos.Add(fileTypeInfo);
                }
            }

            GenerateFileTypes(rootFileTypeInfos, null);
        }

        private void GenerateFileTypes(IEnumerable<FileTypeInfo> fileTypeInfos, FileType superType)
        {
            foreach (var fileTypeInfo in fileTypeInfos)
            {
                FileTypeRecognizerTraits recognizerTraits = fileTypeInfo.RecognizerHandle.GetTraits();
                FileType fileType = new FileType(recognizerTraits.Id, recognizerTraits.Description, superType);
                fileTypes.Add(fileType);

                fileTypeInfo.FileType = fileType;

                GenerateFileTypes(fileTypeInfo.Subtypes, fileType);
            }
        }

        private sealed class FileTypeInfo
        {
            private List<FileTypeInfo> subtypes;

            public ComponentHandle<IFileTypeRecognizer, FileTypeRecognizerTraits> RecognizerHandle { get; set; }

            public FileType FileType { get; set; }

            public IList<FileTypeInfo> Subtypes
            {
                get { return subtypes ?? (IList<FileTypeInfo>) EmptyArray<FileTypeInfo>.Instance; } 
            }

            public Regex FileNameRegex { get; set; }

            public Regex ContentsRegex { get; set; }

            public void AddSubtype(FileTypeInfo subtype)
            {
                if (subtypes == null)
                    subtypes = new List<FileTypeInfo>();
                subtypes.Add(subtype);
            }

            public bool IsRecognizedFile(IFileInspector fileInspector)
            {
                if (FileNameRegex != null)
                {
                    FileInfo fileInfo;
                    if (!fileInspector.TryGetFileInfo(out fileInfo))
                        return false;

                    if (! FileNameRegex.IsMatch(fileInfo.Name))
                        return false;
                }

                if (ContentsRegex != null)
                {
                    string contents;
                    if (!fileInspector.TryGetContents(out contents))
                        return false;

                    if (! ContentsRegex.IsMatch(contents))
                        return false;
                }

                IFileTypeRecognizer fileTypeRecognizer = RecognizerHandle.GetComponent();
                return fileTypeRecognizer.IsRecognizedFile(fileInspector);
            }

            public bool IsSameOrSubtype(FileTypeInfo possibleSubtype)
            {
                if (this == possibleSubtype)
                    return true;

                if (subtypes != null)
                {
                    foreach (FileTypeInfo subtype in Subtypes)
                    {
                        if (subtype.IsSameOrSubtype(possibleSubtype))
                            return true;
                    }
                }

                return false;
            }
        }
    }
}
