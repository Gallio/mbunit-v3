// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Policies;
using Gallio.Runtime.Extensibility.Schema;
using Gallio.Runtime.ProgressMonitoring;
using File = System.IO.File;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A caching plugin loader.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The cache alleviates the need to search, load and preprocess individual plugin files
    /// by storing an aggregate summary of the plugin files into the per-user local
    /// application data path based on a hash of the plugin paths and preprocessor constants.
    /// </para>
    /// </remarks>
    public class CachingPluginLoader : PluginLoader
    {
        private Memoizer<XmlSerializer> cacheXmlSerializerMemoizer = new Memoizer<XmlSerializer>();

        /// <summary>
        /// Creates a caching plugin loader.
        /// </summary>
        public CachingPluginLoader()
        {
        }

        private XmlSerializer CacheXmlSerializer
        {
            get { return cacheXmlSerializerMemoizer.Memoize(() => new XmlSerializer(typeof(Cache))); }
        }

        /// <summary>
        /// Clears the plugin cache belonging to the current user.
        /// </summary>
        public static void ClearCurrentUserPluginCache()
        {
            string cacheDir = GetCurrentUserPluginCacheDir();
            if (Directory.Exists(cacheDir))
                Directory.Delete(cacheDir, true);
        }

        /// <inheritdoc />
        protected override void LoadPlugins(PluginCallback pluginCallback, IProgressMonitor progressMonitor)
        {
            // Attempt to read the old cache.
            string cacheFilePath;
            try
            {
                Hash64 hash = new Hash64();
                foreach (string pluginPath in PluginPaths)
                    hash = hash.Add(pluginPath);
                foreach (string constant in InitialPreprocessorConstants)
                    hash = hash.Add(constant);
                hash = hash.Add(InstallationId.ToString());

                string cacheDirPath = GetCurrentUserPluginCacheDir();
                string cacheFileName = hash + ".xml";
                cacheFilePath = Path.Combine(cacheDirPath, cacheFileName);

                if (Directory.Exists(cacheDirPath))
                {
                    if (File.Exists(cacheFilePath))
                    {
                        Cache oldCache = ReadCacheFile(cacheFilePath);
                        if (oldCache != null)
                        {
                            foreach (var pluginInfo in oldCache.PluginInfos)
                                pluginCallback(pluginInfo.Plugin, new DirectoryInfo(pluginInfo.BaseDirectory), pluginInfo.PluginFile);
                            return;
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(cacheDirPath);
                }
            }
            catch (Exception)
            {
                // Fallback on any failure.
                // There can be all sorts of weird security exceptions that will prevent
                // us from manipulating the local application data directory.
                base.LoadPlugins(pluginCallback, progressMonitor);
                return;
            }

            // Load plugin metadata.
            var newCache = new Cache
            {
                InstallationId = InstallationId.ToString()
            };

            base.LoadPlugins((plugin, baseDirectory, pluginFile) =>
            {
                newCache.PluginInfos.Add(new CachePluginInfo
                {
                    Plugin = plugin,
                    BaseDirectory = baseDirectory.FullName,
                    PluginFile = pluginFile,
                    PluginFileModificationTime = File.GetLastWriteTimeUtc(pluginFile)
                });

                pluginCallback(plugin, baseDirectory, pluginFile);
            }, progressMonitor);

            // Attempt to store it in the cache.
            try
            {
                WriteCacheFile(cacheFilePath, newCache);
            }
            catch (Exception)
            {
                // Ignore any failure.
            }
        }

        private Cache ReadCacheFile(string cacheFilePath)
        {
            Cache cache;
            using (var xmlReader = XmlReader.Create(cacheFilePath))
            {
                cache = (Cache)CacheXmlSerializer.Deserialize(xmlReader);
            }

            cache.Validate();

            if (cache.InstallationId != InstallationId.ToString())
                return null;

            foreach (CachePluginInfo pluginInfo in cache.PluginInfos)
            {
                if (! System.IO.File.Exists(pluginInfo.PluginFile))
                    return null;

                if (System.IO.File.GetLastWriteTimeUtc(pluginInfo.PluginFile) > pluginInfo.PluginFileModificationTime)
                    return null;
            }

            return cache;
        }

        private void WriteCacheFile(string cacheFilePath, Cache cache)
        {
            using (var xmlWriter = XmlWriter.Create(cacheFilePath))
            {
                CacheXmlSerializer.Serialize(xmlWriter, cache);
            }
        }

        internal static string GetCurrentUserPluginCacheDir()
        {
            return SpecialPathPolicy.For("Plugin Metadata Cache").GetLocalUserApplicationDataDirectory().FullName;
        }
    }
}
