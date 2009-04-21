using System;
using System.Collections.Generic;
using System.IO;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Populates a plugin catalog with plugin metadata.
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// Adds a plugin path.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the plugin path refers to a specific plugin file, it will be loaded when
        /// the catalog is populated.  Otherwise if the plugin path refers to a directory,
        /// then the directory will be scanned for *.plugin files when the catalog is
        /// populated.
        /// </para>
        /// </remarks>
        /// <param name="pluginPath">The plugin path to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginPath"/> is null</exception>
        void AddPluginPath(string pluginPath);

        /// <summary>
        /// Adds a plugin definition as XML.
        /// </summary>
        /// <param name="pluginXml">The plugin xml</param>
        /// <param name="baseDirectory">The plugin base directory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginXml"/>
        /// or <paramref name="baseDirectory"/> is null</exception>
        void AddPluginXml(string pluginXml, DirectoryInfo baseDirectory);

        /// <summary>
        /// Defines a constant to be used by the preprocessor.
        /// </summary>
        /// <param name="constant">The constant name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constant"/> is null</exception>
        void DefinePreprocessorConstant(string constant);

        /// <summary>
        /// Populates a catalog with plugin metadata derived from the currently
        /// registered plugin data sources.
        /// </summary>
        /// <param name="catalog">The plugin catalog to populate</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="catalog"/>
        /// is null</exception>
        void PopulateCatalog(IPluginCatalog catalog);
    }
}