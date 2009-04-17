using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Gallio.Collections;
using Gallio.Schema.Plugins;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Loads plugins by recursively scanning plugin paths in the file system
    /// for *.plugin files that contain plugin metadata.
    /// </summary>
    public class FileSystemPluginLoader : IPluginLoader
    {
        private readonly XmlSerializer pluginXmlSerializer;

        /// <summary>
        /// Creates a file system plugin loader.
        /// </summary>
        public FileSystemPluginLoader()
        {
            pluginXmlSerializer = new XmlSerializer(typeof(Plugin));
        }

        /// <inheritdoc />
        public void PopulateCatalog(IPluginCatalog catalog, IList<string> pluginPaths)
        {
            if (catalog == null)
                throw new ArgumentNullException("catalog");
            if (pluginPaths == null || pluginPaths.Contains(null))
                throw new ArgumentNullException("pluginPaths");

            HashSet<string> uniquePluginFilePaths = new HashSet<string>();

            foreach (string pluginPath in pluginPaths)
            {
                DirectoryInfo pluginDirectory = new DirectoryInfo(pluginPath);
                if (pluginDirectory.Exists)
                {
                    PopulateCatalogFromDirectory(catalog, pluginDirectory, uniquePluginFilePaths);
                }
            }
        }

        private void PopulateCatalogFromDirectory(IPluginCatalog catalog, DirectoryInfo pluginDirectory, HashSet<string> uniquePluginFilePaths)
        {
            FileInfo[] pluginFiles = pluginDirectory.GetFiles("*.plugin", SearchOption.AllDirectories);

            foreach (FileInfo pluginFile in pluginFiles)
            {
                PopulateCatalogFromFile(catalog, pluginFile, uniquePluginFilePaths);
            }
        }

        private void PopulateCatalogFromFile(IPluginCatalog catalog, FileInfo pluginFile, HashSet<string> uniquePluginFilePaths)
        {
            string pluginFilePath = pluginFile.FullName;
            if (uniquePluginFilePaths.Contains(pluginFilePath))
                return;

            uniquePluginFilePaths.Add(pluginFilePath);

            Plugin plugin = ReadPluginMetadata(pluginFile);
            catalog.AddPlugin(plugin, pluginFile.Directory);
        }

        private Plugin ReadPluginMetadata(FileInfo pluginFile)
        {
            try
            {
                using (var stream = pluginFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                    return (Plugin)pluginXmlSerializer.Deserialize(stream);
            }
            catch (Exception ex)
            {
                throw new RuntimeException(string.Format("Failed to read and parse plugin metadata file '{0}'.", pluginFile.FullName), ex);
            }
        }
    }
}
