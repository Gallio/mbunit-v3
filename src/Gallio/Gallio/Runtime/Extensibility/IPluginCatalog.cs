using System;
using System.IO;
using Gallio.Schema.Plugins;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A plugin catalog describes the configuration of a collection of plugins that
    /// are to be installed in a registry.
    /// </summary>
    public interface IPluginCatalog
    {
        /// <summary>
        /// Adds a plugin to the catalog.
        /// </summary>
        /// <param name="plugin">The plugin to add</param>
        /// <param name="baseDirectory">The plugin base directory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="plugin"/>
        /// or <paramref name="baseDirectory"/> is null</exception>
        void AddPlugin(Plugin plugin, DirectoryInfo baseDirectory);

        /// <summary>
        /// Registers the contents of the catalog into the specified registry.
        /// </summary>
        /// <param name="registry">The registry</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registry"/> is null</exception>
        void ApplyTo(IRegistry registry);
    }
}