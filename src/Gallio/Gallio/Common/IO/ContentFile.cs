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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Common.IO
{
    /// <summary>
    /// Representation of a file resource.
    /// </summary>
    public class ContentFile : Content
    {
        private readonly string filePath;
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Constructs the representation of a file resource.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        public ContentFile(string filePath)
            : this(filePath, new FileSystem())
        {
        }

        /// <summary>
        /// Constructs the representation of a file resource.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <param name="fileSystem">A file system wrapper</param>
        public ContentFile(string filePath, IFileSystem fileSystem)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            this.filePath = filePath;
            this.fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public override bool IsDynamic
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public override Stream OpenStream()
        {
            return fileSystem.OpenRead(filePath);
        }

        /// <inheritdoc />
        public override TextReader OpenTextReader()
        {
            return new StreamReader(OpenStream());
        }
    }
}