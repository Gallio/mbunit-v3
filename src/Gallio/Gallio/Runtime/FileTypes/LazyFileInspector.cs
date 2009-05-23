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
    /// A file inspector that opens a file only on demand as needed.
    /// </summary>
    internal class LazyFileInspector : IFileInspector, IDisposable
    {
        private FileInfo fileInfo;
        private string contents;
        private FileStream stream;

        /// <summary>
        /// Creates a file inspector for the file describe the specified file info object.
        /// </summary>
        /// <param name="fileInfo">The file to inspect</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileInfo"/> is null</exception>
        public LazyFileInspector(FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            this.fileInfo = fileInfo;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool TryGetFileInfo(out FileInfo fileInfo)
        {
            ThrowIfDisposed();

            fileInfo = this.fileInfo;
            return true;
        }

        public bool TryGetContents(out string contents)
        {
            ThrowIfDisposed();

            if (this.contents == null)
            {
                var stream = OpenOrReuseStream();
                if (stream == null)
                {
                    contents = null;
                    return false;
                }

                var reader = new StreamReader(stream);
                this.contents = reader.ReadToEnd();
            }

            contents = this.contents;
            return true;
        }

        public bool TryGetStream(out Stream stream)
        {
            ThrowIfDisposed();

            stream = OpenOrReuseStream();
            return stream != null;
        }

        /// <summary>
        /// Disposes the inspector.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()" /> was called directly</param>
        protected virtual void Dispose(bool disposing)
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }

            contents = null;
            fileInfo = null;
        }

        private FileStream OpenOrReuseStream()
        {
            if (stream != null)
            {
                if (stream.CanRead)
                {
                    stream.Position = 0;
                    return stream;
                }

                stream = null;
            }

            stream = OpenNewStream();
            return stream;
        }

        private FileStream OpenNewStream()
        {
            if (fileInfo.Exists)
            {
                try
                {
                    return fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (IOException)
                {
                    // Consume I/O errors.
                }
            }

            return null;
        }

        private void ThrowIfDisposed()
        {
            if (fileInfo == null)
                throw new ObjectDisposedException("The file inspector has been disposed.");
        }
    }
}
