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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MbUnit.Core.Utilities
{
    /// <summary>
    /// File manipulation utilities.
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
                throw new ArgumentNullException("fileName");

            fileName = fileName.Trim();
            foreach (char c in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(c, '_');

            // Note: Windows file system has 255 char max filename length restriction.
            fileName = StringUtils.Truncate(fileName, 255);
            return fileName;
        }

        /// <summary>
        /// Copies the contents of a stream to a file.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="path">The path of the file to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/>
        /// or <paramref name="path"/> is null</exception>
        public static void CopyStreamToFile(Stream stream, string path)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (path == null)
                throw new ArgumentNullException("path");

            using (stream)
            {
                using (Stream outputStream = File.OpenWrite(path))
                {
                    byte[] buffer = new byte[Math.Min(4096, stream.Length)];
                    for (; ; )
                    {
                        int count = stream.Read(buffer, 0, buffer.Length);
                        if (count == 0)
                            return;

                        outputStream.Write(buffer, 0, count);
                    }
                }
            }
        }
    }
}
