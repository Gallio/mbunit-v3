using System;
using System.Collections.Generic;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Populates a plugin catalog with plugin metadata.
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// Populates a catalog with plugin metadata from the specified plugin paths.
        /// </summary>
        /// <param name="catalog">The plugin catalog to populate</param>
        /// <param name="pluginPaths">The list of plugin paths to search</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="catalog"/>
        /// or <paramref name="pluginPaths"/> is null or contains a null</exception>
        void PopulateCatalog(IPluginCatalog catalog, IList<string> pluginPaths);
    }
}