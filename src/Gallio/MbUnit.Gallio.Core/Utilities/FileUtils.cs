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
            if (fileName.Length > 255)
                fileName = fileName.Substring(0, 255);

            return fileName;
        }
    }
}
