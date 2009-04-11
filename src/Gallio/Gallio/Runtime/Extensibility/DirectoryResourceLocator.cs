using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A resource locator that finds resources relative to a directory.
    /// </summary>
    public class DirectoryResourceLocator : IResourceLocator
    {
        private readonly DirectoryInfo baseDirectory;

        /// <summary>
        /// Creates a directory resource locator.
        /// </summary>
        /// <param name="baseDirectory">The base directory for relative paths</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="baseDirectory"/> is null</exception>
        public DirectoryResourceLocator(DirectoryInfo baseDirectory)
        {
            if (baseDirectory == null)
                throw new ArgumentNullException("baseDirectory");

            this.baseDirectory = baseDirectory;
        }
    }
}
