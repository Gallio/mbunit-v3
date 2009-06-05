using System;
using System.IO;
using Gallio.Schema.Plugins;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Details of a plugin that has just been loaded.
    /// </summary>
    public class PluginLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// The loaded plugin.
        /// </summary>
        public Plugin Plugin { get; private set; }
        /// <summary>
        /// The base directory for the plugin.
        /// </summary>
        public DirectoryInfo BaseDirectory { get; private set; }
        /// <summary>
        /// The file path of the plugin.
        /// </summary>
        public string PluginFilePath { get; private set; }

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="plugin">The loaded plugin.</param>
        ///<param name="baseDirectory"></param>
        ///<param name="pluginFilePath"></param>
        public PluginLoadedEventArgs(Plugin plugin, DirectoryInfo baseDirectory, string pluginFilePath)
        {
            Plugin = plugin;
            BaseDirectory = baseDirectory;
            PluginFilePath = pluginFilePath;
        }
    }
}
