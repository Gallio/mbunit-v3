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
using System.Text;

namespace Gallio.Runtime.FileTypes
{
    /// <summary>
    /// Provides services related to determining the type of a file.
    /// </summary>
    public interface IFileTypeManager
    {
        /// <summary>
        /// Gets a file type used to identify a file of unknown type.
        /// </summary>
        FileType UnknownFileType { get; }

        /// <summary>
        /// Gets information about a registered file type by its id.
        /// </summary>
        /// <param name="id">The file type id.</param>
        /// <returns>The associated file type information, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is null.</exception>
        FileType GetFileTypeById(string id);

        /// <summary>
        /// Gets information about all registered file types.
        /// </summary>
        /// <returns>The list of all registered file types.</returns>
        IList<FileType> GetFileTypes();

        /// <summary>
        /// Identifies the type of a file by consulting the set of all registered file types.
        /// </summary>
        /// <param name="fileInfo">The file to identify.</param>
        /// <returns>The file type of the file, or <see cref="UnknownFileType" /> type if unidentified.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileInfo"/> is null.</exception>
        /// <exception cref="IOException">Thrown if the file cannot be accessed.</exception>
        FileType IdentifyFileType(FileInfo fileInfo);

        /// <summary>
        /// Identifies the type of a file by consulting the set of all registered file types.
        /// </summary>
        /// <param name="fileInspector">The file inspector for the file to identify.</param>
        /// <returns>The file type of the file, or <see cref="UnknownFileType" /> type if unidentified.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileInspector"/> is null.</exception>
        /// <exception cref="IOException">Thrown if the file cannot be accessed.</exception>
        FileType IdentifyFileType(IFileInspector fileInspector);

        /// <summary>
        /// Identifies the type of a file by consulting only the specified list of file type candidates.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is more efficient when we are interested in only a few possible results.
        /// </para>
        /// </remarks>
        /// <param name="fileInfo">The file to identify.</param>
        /// <param name="candidates">The collection of specific file type candidates to consider.</param>
        /// <returns>The file type of the file, or <see cref="UnknownFileType" /> type if unidentified
        /// among the candidates considered.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileInfo"/> or
        /// <paramref name="candidates"/> is null.</exception>
        /// <exception cref="IOException">Thrown if the file cannot be accessed.</exception>
        FileType IdentifyFileType(FileInfo fileInfo, IEnumerable<FileType> candidates);

        /// <summary>
        /// Identifies the type of a file by consulting only the specified list of file type candidates.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is more efficient when we are interested in only a few possible results.
        /// </para>
        /// </remarks>
        /// <param name="fileInspector">The file inspector for the file to identify.</param>
        /// <param name="candidates">The collection of specific file type candidates to consider.</param>
        /// <returns>The file type of the file, or <see cref="UnknownFileType" /> type if unidentified
        /// among the candidates considered.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileInspector"/> or
        /// <paramref name="candidates"/> is null.</exception>
        /// <exception cref="IOException">Thrown if the file cannot be accessed.</exception>
        FileType IdentifyFileType(IFileInspector fileInspector, IEnumerable<FileType> candidates);
    }
}
