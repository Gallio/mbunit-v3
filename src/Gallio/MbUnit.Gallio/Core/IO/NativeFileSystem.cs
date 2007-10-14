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
using MbUnit.Utilities;

namespace MbUnit.Core.IO
{
    ///<summary>
    /// Performs IO related operations on the native file system.
    ///</summary>
    public class NativeFileSystem : IFileSystem
    {
        private static readonly NativeFileSystem instance = new NativeFileSystem();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NativeFileSystem Instance
        {
            get { return instance; }
        }

        /// <inheritdoc />
        public string EncodeFileName(string fileName)
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

        /// <inheritdoc />
        public string GetFullDirectoryName(string path)
        {
            return Path.GetDirectoryName(Path.GetFullPath(path));
        }

        /// <inheritdoc />
        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <inheritdoc />
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <inheritdoc />
        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        /// <inheritdoc />
        public Stream OpenFile(string path, FileMode mode, FileAccess access)
        {
            return File.Open(path, mode, access);
        }

        /// <inheritdoc />
        public void Copy(string sourcePath, string destPath, bool overwrite)
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
                        Copy(entry.FullName, Path.Combine(destPath, entry.Name), overwrite);
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

        /// <inheritdoc />
        public void Delete(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            else if (File.Exists(path))
                File.Delete(path);
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