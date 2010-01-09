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
using System.IO;

namespace Gallio.Common.Caching
{
    /// <summary>
    /// A disk cache group represents an indexed partition of the disk cache.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is physically manifested as a directory on disk and may contain any
    /// number of related files or directories with the same lifetime.
    /// </para>
    /// </remarks>
    public interface IDiskCacheGroup
    {
        /// <summary>
        /// Gets the disk cache that contains the group.
        /// </summary>
        IDiskCache Cache { get; }

        /// <summary>
        /// Gets the key of the group.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the <see cref="DirectoryInfo" /> that represents the physical
        /// storage location of the disk cache group in the filesystem.
        /// </summary>
        DirectoryInfo Location { get; }

        /// <summary>
        /// Returns true if the group exists on disk.
        /// </summary>
        /// <exception cref="DiskCacheException">Thrown if an error occurs.</exception>
        bool Exists { get; }

        /// <summary>
        /// Creates the group if it does not exist.
        /// </summary>
        /// <exception cref="DiskCacheException">Thrown if an error occurs.</exception>
        void Create();

        /// <summary>
        /// Deletes the group and all of its contents if any.
        /// </summary>
        /// <exception cref="DiskCacheException">Thrown if an error occurs.</exception>
        void Delete();

        /// <summary>
        /// Gets information about a file within the group.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will succeed even if the file or the group does not exist.
        /// </para>
        /// </remarks>
        /// <param name="relativeFilePath">The relative path of the file within the group.</param>
        /// <returns>The file info.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeFilePath"/> is null.</exception>
        /// <exception cref="DiskCacheException">Thrown if an error occurs.</exception>
        FileInfo GetFileInfo(string relativeFilePath);

        /// <summary>
        /// Gets information about a directory within the group.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will succeed even if the directory or the group does not exist.
        /// </para>
        /// </remarks>
        /// <param name="relativeDirectoryPath">The relative path of the directory within the group.</param>
        /// <returns>The directory info.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeDirectoryPath"/> is null.</exception>
        /// <exception cref="DiskCacheException">Thrown if an error occurs.</exception>
        DirectoryInfo GetSubdirectoryInfo(string relativeDirectoryPath);

        /// <summary>
        /// Opens a file within the group.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If a new file is being created, automatically create the group and the
        /// containing directory for the file.
        /// </para>
        /// </remarks>
        /// <param name="relativeFilePath">The relative path of the file to open within the group.</param>
        /// <param name="mode">The file open mode.</param>
        /// <param name="access">The file access mode.</param>
        /// <param name="share">The file sharing mode.</param>
        /// <returns>The file stream.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeFilePath"/> is null.</exception>
        /// <exception cref="DiskCacheException">Thrown if an error occurs.</exception>
        Stream OpenFile(string relativeFilePath, FileMode mode, FileAccess access, FileShare share);

        /// <summary>
        /// Creates a directory within the group.
        /// </summary>
        /// <param name="relativeDirectoryPath">The relative path of the directory to create within the group.</param>
        /// <returns>Directory information for the directory that was created.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeDirectoryPath"/> is null.</exception>
        /// <exception cref="DiskCacheException">Thrown if an error occurs.</exception>
        DirectoryInfo CreateSubdirectory(string relativeDirectoryPath);
    }
}
