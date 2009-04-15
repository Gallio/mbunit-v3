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
