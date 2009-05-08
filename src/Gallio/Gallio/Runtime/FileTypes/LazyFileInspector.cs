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

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public bool TryGetFileInfo(out FileInfo fileInfo)
        {
            ThrowIfDisposed();

            fileInfo = this.fileInfo;
            return true;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
