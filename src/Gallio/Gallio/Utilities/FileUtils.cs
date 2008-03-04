// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.IO;

namespace Gallio.Utilities
{
    /// <summary>
    /// Utilities for manipulating files.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Replaces invalid characters in a file or directory name with underscores
        /// and trims it if it is too long.
        /// </summary>
        /// <param name="fileName">The file or directory name</param>
        /// <returns>The encoded file or directory name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileName"/> is null</exception>
        public static string EncodeFileName(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(@"fileName");

            fileName = fileName.Trim();
            foreach (char c in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(c, '_');

            // Note: Windows file system has 255 char max filename length restriction.
            fileName = StringUtils.Truncate(fileName, 255);
            return fileName;
        }

        /// <summary>
        /// Gets the full path of the containing directory.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The full path of the parent directory or null if it is at the root</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        public static string GetFullPathOfParentDirectory(string path)
        {
            return Path.GetDirectoryName(Path.GetFullPath(path));
        }

        /// <summary>
        /// Recursively copies files and folders from the source path to the destination.
        /// </summary>
        /// <param name="sourcePath">The source file or directory path</param>
        /// <param name="destPath">The destination file or directory path</param>
        /// <param name="overwrite">If true, overwrites existing files in the destination</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourcePath"/> or <paramref name="destPath"/> is null</exception>
        public static void CopyAll(string sourcePath, string destPath, bool overwrite)
        {
            if (sourcePath == null)
                throw new ArgumentNullException(@"sourcePath");
            if (destPath == null)
                throw new ArgumentNullException(@"destPath");

            if (Directory.Exists(sourcePath))
            {
                Directory.CreateDirectory(destPath);

                foreach (FileSystemInfo entry in new DirectoryInfo(sourcePath).GetFileSystemInfos())
                {
                    if (CanCopy(entry))
                        CopyAll(entry.FullName, Path.Combine(destPath, entry.Name), overwrite);
                }
            }
            else if (File.Exists(sourcePath))
            {
                if (CanCopy(new FileInfo(sourcePath)))
                    File.Copy(sourcePath, destPath, overwrite);
            }
            else
            {
                throw new FileNotFoundException("Source file or directory does not exist.", sourcePath);
            }
        }

        /// <summary>
        /// Recursively deletes a file or directory.
        /// Does nothing if the file or directory does not exist.
        /// </summary>
        /// <param name="path">The path</param>
        public static void DeleteAll(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            else if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        /// Makes all paths in the list absolute.
        /// </summary>
        /// <param name="baseDirectory">The base directory for resolving relative paths,
        /// or null to use the current directory</param>
        /// <param name="paths">The list of paths to canonicalize in place</param>
        public static void CanonicalizePaths(string baseDirectory, IList<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
                paths[i] = CanonicalizePath(baseDirectory, paths[i]);
        }

        /// <summary>
        /// Makes an absolute path.
        /// </summary>
        /// <param name="baseDirectory">The base directory for resolving relative paths,
        /// or null to use the current directory</param>
        /// <param name="path">The path to canonicalize, or null if none</param>
        /// <returns>The absolute path, or null if none</returns>
        public static string CanonicalizePath(string baseDirectory, string path)
        {
            if (path == null)
                return null;
            if (Path.IsPathRooted(path))
                return path;
            if (baseDirectory == null)
                return path.Length == 0 ? Environment.CurrentDirectory : Path.GetFullPath(path);
            return Path.Combine(Path.GetFullPath(baseDirectory), path);
        }

        private static bool CanCopy(FileSystemInfo entry)
        {
            // Omit reparse points to prevent indefinite recursion.
            // Omit hidden and system files to prevent copying of SVN metadata during debugging
            // and other similar resources.
            return (entry.Attributes & (FileAttributes.ReparsePoint | FileAttributes.Hidden | FileAttributes.System)) == 0;
        }
    }
}
