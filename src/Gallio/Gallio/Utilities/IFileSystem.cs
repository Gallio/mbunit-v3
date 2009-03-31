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

namespace Gallio.Utilities
{
    /// <summary>
    /// Wrapper for static System.IO.File operations to 
    /// allow testing.
    /// </summary>
    public interface IFileSystem
    {
        ///<summary>
        /// Checks if a given file exists (File.Exists).
        ///</summary>
        ///<param name="path">The path of the file.</param>
        ///<returns>True if the file exists, otherwise False.</returns>
        bool FileExists(string path);

        ///<summary>
        /// Checks if a path is relative or absolute (Path.IsPathRooted).
        ///</summary>
        ///<param name="path">The path to check.</param>
        ///<returns>True if the path is absolute, otherwise False.</returns>
        bool IsPathRooted(string path);

        ///<summary>
        /// Checks if a directory exists (Directory.Exists)
        ///</summary>
        ///<param name="path">The location of the directory.</param>
        ///<returns>True if the directory exists, otherwise False.</returns>
        bool DirectoryExists(string path);

        ///<summary>
        /// Attempts to create a directory.
        ///</summary>
        ///<param name="path">The location of the directory.</param>
        void CreateDirectory(string path);

        ///<summary>
        /// Attempts to delete a file.
        ///</summary>
        ///<param name="path">The location of the file.</param>
        void DeleteFile(string path);
    }
}
