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

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A resource locator that finds resources relative to a directory in the file system.
    /// </summary>
    public class FileSystemResourceLocator : IResourceLocator
    {
        private readonly DirectoryInfo baseDirectory;

        /// <summary>
        /// Creates a directory resource locator.
        /// </summary>
        /// <param name="baseDirectory">The base directory for relative paths</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="baseDirectory"/> is null</exception>
        public FileSystemResourceLocator(DirectoryInfo baseDirectory)
        {
            if (baseDirectory == null)
                throw new ArgumentNullException("baseDirectory");

            this.baseDirectory = baseDirectory;
        }

        /// <summary>
        /// Gets the base directory.
        /// </summary>
        public DirectoryInfo BaseDirectory
        {
            get { return baseDirectory; }
        }

        /// <inheritdoc />
        public string GetFullPath(string relativePath)
        {
            if (relativePath == null)
                throw new ArgumentNullException("relativePath");

            return Path.Combine(baseDirectory.FullName, relativePath);
        }
    }
}
