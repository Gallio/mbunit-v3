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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common;
using Gallio.Common.Policies;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Extensibility.Schema;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;
using File = System.IO.File;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(CachingPluginLoader))]
    public class CachingPluginLoaderTest
    {
        [Test]
        public void GetCurrentUserPluginCacheDir_Always_IsInLocalApplicationData()
        {
            Assert.StartsWith(CachingPluginLoader.GetCurrentUserPluginCacheDir(),
                SpecialPathPolicy.For("Plugin Metadata Cache").GetLocalUserApplicationDataDirectory().FullName);
        }

        [Test]
        public void ClearCurrentUserCache_IfCacheExists_RemovesCacheDirectory()
        {
            var cacheDir = CachingPluginLoader.GetCurrentUserPluginCacheDir();

            CachingPluginLoader.ClearCurrentUserPluginCache();
            Assert.IsFalse(Directory.Exists(cacheDir));
        }

        [Test]
        public void PopulateCatalog_WhenPluginXmlContainsPreprocessorInstructions_AppliesThem()
        {
            string pluginContents = "<plugin pluginId=\"pluginId\" xmlns=\"http://www.gallio.org/\"><traits><?ifdef A?><name>A</name><?endif?><?ifdef B?><property>B</property><?endif?></traits></plugin>";

            PluginLoaderTest.RunWithTemporaryPluginFile((pluginDir, pluginFile) =>
            {
                Guid installationId = Guid.NewGuid();
                var loader = new CachingPluginLoader();
                loader.InstallationId = installationId;
                loader.AddPluginPath(pluginFile);
                loader.DefinePreprocessorConstant("A");

                Hash64 hash = new Hash64().Add(pluginFile).Add("A").Add(installationId.ToString());
                var cacheDir = CachingPluginLoader.GetCurrentUserPluginCacheDir();
                string cacheFilePath = Path.Combine(cacheDir, hash + ".xml");

                if (System.IO.File.Exists(cacheFilePath))
                    System.IO.File.Delete(cacheFilePath);

                // First pass.
                {
                    Plugin plugin = null;
                    var catalog = MockRepository.GenerateMock<IPluginCatalog>();
                    catalog.Expect(x => x.AddPlugin(null, null)).IgnoreArguments()
                        .Do((Gallio.Common.Action<Plugin, DirectoryInfo>)delegate(Plugin pluginArg, DirectoryInfo baseDirectoryArg)
                        {
                            plugin = pluginArg;
                        });

                    loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance());

                    catalog.VerifyAllExpectations(); // added one plugin

                    Assert.AreEqual(new PropertySet() { { "name", "A" } }, plugin.Traits.PropertySet);
                }

                // Check cache file.
                {
                    Assert.IsTrue(File.Exists(cacheFilePath));

                    Cache cache = Assert.XmlDeserialize<Cache>(File.ReadAllText(cacheFilePath));

                    Assert.AreEqual(installationId.ToString(), cache.InstallationId);
                    Assert.AreEqual(1, cache.PluginInfos.Count);
                    Assert.AreEqual(pluginDir, cache.PluginInfos[0].BaseDirectory);
                    Assert.AreEqual("pluginId", cache.PluginInfos[0].Plugin.PluginId);
                    Assert.AreEqual(pluginFile, cache.PluginInfos[0].PluginFile);
                    Assert.AreEqual(File.GetLastWriteTimeUtc(pluginFile), cache.PluginInfos[0].PluginFileModificationTime);
                }

                // Second pass should restore from cache.
                {
                    Plugin plugin = null;
                    var catalog = MockRepository.GenerateMock<IPluginCatalog>();
                    catalog.Expect(x => x.AddPlugin(null, null)).IgnoreArguments()
                        .Do((Gallio.Common.Action<Plugin, DirectoryInfo>)delegate(Plugin pluginArg, DirectoryInfo baseDirectoryArg)
                        {
                            plugin = pluginArg;
                        });

                    loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance());

                    catalog.VerifyAllExpectations(); // added one plugin

                    Assert.AreEqual(new PropertySet() { { "name", "A" } }, plugin.Traits.PropertySet);
                }
            }, pluginContents);
        }
    }
}
