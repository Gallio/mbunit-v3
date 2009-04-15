using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Locates resources owned by plugins and other components.
    /// </summary>
    public interface IResourceLocator
    {
        /// <summary>
        /// Resolves a relative path within the resource locator to a full path.
        /// </summary>
        /// <param name="relativePath">The relative path</param>
        /// <returns>The full path</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativePath"/> is null</exception>
        string GetFullPath(string relativePath);
    }
}