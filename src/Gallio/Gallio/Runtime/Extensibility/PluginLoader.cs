using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Collections;
using Gallio.Schema.Plugins;
using Gallio.Utilities;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Loads plugins by recursively scanning plugin paths in the file system
    /// for *.plugin files that contain plugin metadata.
    /// </summary>
    public class PluginLoader : IPluginLoader
    {
        private readonly Memoizer<XmlSerializer> pluginXmlSerializerMemoizer = new Memoizer<XmlSerializer>();

        private readonly List<string> pluginPaths;
        private readonly List<Pair<string, DirectoryInfo>> pluginXmls;
        private readonly HashSet<string> initialPreprocessorConstants;

        /// <summary>
        /// The plugin callback type used by <see cref="LoadPlugins"/>.
        /// </summary>
        protected delegate void PluginCallback(Plugin plugin, DirectoryInfo baseDirectory, string pluginFilePath);

        /// <summary>
        /// Creates a file system plugin loader.
        /// </summary>
        public PluginLoader()
        {
            pluginPaths = new List<string>();
            pluginXmls = new List<Pair<string, DirectoryInfo>>();
            initialPreprocessorConstants = new HashSet<string>();
        }

        private XmlSerializer PluginXmlSerializer
        {
            get { return pluginXmlSerializerMemoizer.Memoize(() => new XmlSerializer(typeof(Plugin))); }
        }

        /// <inheritdoc />
        public void AddPluginPath(string pluginPath)
        {
            if (pluginPath == null)
                throw new ArgumentNullException("pluginPath");

            pluginPaths.Add(pluginPath);
        }

        /// <inheritdoc />
        public void AddPluginXml(string pluginXml, DirectoryInfo baseDirectory)
        {
            if (pluginXml == null)
                throw new ArgumentNullException("pluginXml");
            if (baseDirectory == null)
                throw new ArgumentNullException("baseDirectory");

            pluginXmls.Add(new Pair<string, DirectoryInfo>(pluginXml, baseDirectory));
        }

        /// <inheritdoc />
        public void DefinePreprocessorConstant(string constant)
        {
            if (constant == null)
                throw new ArgumentNullException("constant");

            initialPreprocessorConstants.Add(constant);
        }

        /// <inheritdoc />
        public void PopulateCatalog(IPluginCatalog catalog)
        {
            if (catalog == null)
                throw new ArgumentNullException("catalog");

            LoadPlugins((plugin, baseDirectory, pluginFilePath) => catalog.AddPlugin(plugin, baseDirectory));

            foreach (Pair<string, DirectoryInfo> pair in pluginXmls)
            {
                Plugin plugin = ReadPluginMetadataFromXml(pair.First);
                catalog.AddPlugin(plugin, pair.Second);
            }
        }

        /// <summary>
        /// Gets the list of plugin paths.
        /// </summary>
        protected IList<string> PluginPaths
        {
            get { return pluginPaths; }
        }

        /// <summary>
        /// Gets the collection of initial preprocessor constants.
        /// </summary>
        protected ICollection<string> InitialPreprocessorConstants
        {
            get { return initialPreprocessorConstants; }
        }

        /// <summary>
        /// Loads plugins by recursively searching the plugin paths for *.plugin files.
        /// </summary>
        /// <param name="pluginCallback">A function that receives plugin metadata as it
        /// becomes available, not null</param>
        protected virtual void LoadPlugins(PluginCallback pluginCallback)
        {
            HashSet<string> uniquePluginFilePaths = new HashSet<string>();

            foreach (string pluginPath in pluginPaths)
            {
                DirectoryInfo pluginDirectory = new DirectoryInfo(pluginPath);
                if (pluginDirectory.Exists)
                {
                    LoadPluginsFromDirectory(pluginDirectory, uniquePluginFilePaths, pluginCallback);
                }
                else
                {
                    FileInfo pluginFile = new FileInfo(pluginPath);
                    if (pluginFile.Exists)
                    {
                        LoadPluginsFromFile(pluginFile, uniquePluginFilePaths, pluginCallback);
                    }
                }
            }

        }

        private void LoadPluginsFromDirectory(DirectoryInfo pluginDirectory, HashSet<string> uniquePluginFilePaths,
            PluginCallback pluginCallback)
        {
            FileInfo[] pluginFiles = pluginDirectory.GetFiles("*.plugin", SearchOption.AllDirectories);

            foreach (FileInfo pluginFile in pluginFiles)
            {
                LoadPluginsFromFile(pluginFile, uniquePluginFilePaths, pluginCallback);
            }
        }

        private void LoadPluginsFromFile(FileInfo pluginFile, HashSet<string> uniquePluginFilePaths,
            PluginCallback pluginCallback)
        {
            string pluginFilePath = pluginFile.FullName;
            if (uniquePluginFilePaths.Contains(pluginFilePath))
                return;

            uniquePluginFilePaths.Add(pluginFilePath);

            Plugin plugin = ReadPluginMetadataFromFile(pluginFile);
            pluginCallback(plugin, pluginFile.Directory, pluginFilePath);
        }

        private Plugin ReadPluginMetadataFromFile(FileInfo pluginFile)
        {
            try
            {
                using (var reader = new StreamReader(pluginFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    return PreprocessAndDeserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new RuntimeException(string.Format("Failed to read and parse plugin metadata file '{0}'.", pluginFile.FullName), ex);
            }
        }

        private Plugin ReadPluginMetadataFromXml(string pluginXml)
        {
            try
            {
                using (var reader = new StringReader(pluginXml))
                {
                    return PreprocessAndDeserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new RuntimeException("Failed to read and parse plugin metadata from Xml configuration.", ex);
            }
        }

        private Plugin PreprocessAndDeserialize(TextReader reader)
        {
            XmlPreprocessor preprocessor = new XmlPreprocessor();

            foreach (string constant in initialPreprocessorConstants)
                preprocessor.Define(constant);

            StringBuilder preprocessedXml = new StringBuilder();
            using (XmlReader xmlReader = XmlReader.Create(reader))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(preprocessedXml))
                    preprocessor.Preprocess(xmlReader, xmlWriter);
            }

            var plugin = (Plugin)PluginXmlSerializer.Deserialize(new StringReader(preprocessedXml.ToString()));
            plugin.Validate();
            return plugin;
        }
    }
}
