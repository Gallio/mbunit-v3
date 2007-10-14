// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace MbUnit.Core.IO
{
    ///<summary>
    /// Provides an abstraction of a file system.
    ///</summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Replaces invalid characters in a file or directory name with underscores
        /// and trims it if it is too long.
        /// </summary>
        /// <param name="fileName">The file or directory name</param>
        /// <returns>The encoded file or directory name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileName"/> is null</exception>
        string EncodeFileName(string fileName);

        /// <summary>
        /// Gets the full directory name of the specified path.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The full directory name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        string GetFullDirectoryName(string path);

        /// <summary>
        /// Creates the directory if it does not exist.
        /// </summary>
        /// <param name="path">The path of the directory to create</param>
        void CreateDirectory(string path);

        ///<summary>
        /// Returns the contents of the file as a text string.
        ///</summary>
        ///<param name="path">The file path</param>
        ///<returns>Contents of the file as a string</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        string ReadAllText(string path);

        /// <summary>
        /// Returns the contents of the file as a byte array.
        /// </summary>
        ///<param name="path">The file path</param>
        ///<returns>Contents of the file as a byte array</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        byte[] ReadAllBytes(string path);

        /// <summary>
        /// Opens a file.
        /// </summary>
        /// <param name="path">The file path</param>
        /// <param name="mode">The file create/append mode</param>
        /// <param name="access">The file read/write access</param>
        /// <returns>The stream</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        Stream OpenFile(string path, FileMode mode, FileAccess access);

        /// <summary>
        /// Recursively copies files and folders from the source directory to the destination.
        /// </summary>
        /// <param name="sourcePath">The source file or directory path</param>
        /// <param name="destPath">The destination file or directory path</param>
        /// <param name="overwrite">If true, overwrites existing files in the destination</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourcePath"/> or <paramref name="destPath"/> is null</exception>
        void Copy(string sourcePath, string destPath, bool overwrite);

        /// <summary>
        /// Recursively deletes a file or directory.
        /// Does nothing if the file or directory does not exist.
        /// </summary>
        /// <param name="path">The path</param>
        void Delete(string path);
    }
}