using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Schema.Plugins;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(FileSystemPluginLoader))]
    public class FileSystemPluginLoaderTest
    {
        [Test]
        public void PopulateCatalog_WhenCatalogIsNull_Throws()
        {
            var loader = new FileSystemPluginLoader();
            var pluginPaths = new string[0];

            Assert.Throws<ArgumentNullException>(() => loader.PopulateCatalog(null, pluginPaths));
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathsIsNull_Throws()
        {
            var loader = new FileSystemPluginLoader();
            var catalog = MockRepository.GenerateStub<IPluginCatalog>();

            Assert.Throws<ArgumentNullException>(() => loader.PopulateCatalog(catalog, null));
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathsContainssNull_Throws()
        {
            var loader = new FileSystemPluginLoader();
            var catalog = MockRepository.GenerateStub<IPluginCatalog>();
            var pluginPaths = new string[] { null };

            Assert.Throws<ArgumentNullException>(() => loader.PopulateCatalog(catalog, pluginPaths));
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathContainsInvalidDirectory_IgnoresIt()
        {
            var loader = new FileSystemPluginLoader();
            var catalog = MockRepository.GenerateMock<IPluginCatalog>();
            var pluginPaths = new string[] { @"C:\This\Directory\Does\Not\Exist" };

            loader.PopulateCatalog(catalog, pluginPaths);

            catalog.VerifyAllExpectations(); // nothing added to catalog
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathContainsDirectoriesWithALeastOnePluginFile_LoadsIt()
        {
            Plugin samplePlugin = new Plugin("pluginId")
            {
                Services =
                {
                    new Service("serviceId", "Service, Assembly")
                },
                Components =
                {
                    new Component("componentId", "serviceId", "Component, Assembly")
                }
            };

            string pluginContents = Assert.XmlSerialize(samplePlugin);

            RunWithTemporaryPluginFile((pluginDir, pluginFile) =>
            {
                var loader = new FileSystemPluginLoader();
                var catalog = MockRepository.GenerateMock<IPluginCatalog>();
                var pluginPaths = new string[] { pluginDir };

                Plugin plugin = null;
                DirectoryInfo baseDirectory = null;
                catalog.Expect(x => x.AddPlugin(null, null)).IgnoreArguments()
                    .Do((Action<Plugin, DirectoryInfo>) delegate(Plugin pluginArg, DirectoryInfo baseDirectoryArg)
                {
                    plugin = pluginArg;
                    baseDirectory = baseDirectoryArg;
                });

                loader.PopulateCatalog(catalog, pluginPaths);

                catalog.VerifyAllExpectations(); // added one plugin

                Assert.Multiple(() =>
                {
                    Assert.AreEqual("pluginId", plugin.PluginId);
                    Assert.AreEqual("serviceId", plugin.Services[0].ServiceId);
                    Assert.AreEqual("componentId", plugin.Components[0].ComponentId);

                    Assert.AreEqual(pluginDir, baseDirectory.ToString());
                });
            }, pluginContents);
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathContainsUnparsablePluginFile_Throws()
        {
            RunWithTemporaryPluginFile((pluginDir, pluginFile) =>
            {
                var loader = new FileSystemPluginLoader();
                var catalog = MockRepository.GenerateMock<IPluginCatalog>();
                var pluginPaths = new string[] { pluginDir };

                var ex = Assert.Throws<RuntimeException>(() => loader.PopulateCatalog(catalog, pluginPaths));
                Assert.AreEqual(string.Format("Failed to read and parse plugin metadata file '{0}'.", pluginFile), ex.Message);

                catalog.VerifyAllExpectations(); // no plugins added
            }, "this is not valid xml");
        }

        private void RunWithTemporaryPluginFile(Action<string, string> action, string pluginFileContents)
        {
            var pluginDir = Path.Combine(Path.GetTempPath(), "FileSystemPluginLoaderTest");
            if (Directory.Exists(pluginDir))
                Directory.Delete(pluginDir, true);
            var pluginFile = Path.Combine(pluginDir, "Sample.plugin");
            try
            {
                Directory.CreateDirectory(pluginDir);
                File.WriteAllText(pluginFile, pluginFileContents);

                action(pluginDir, pluginFile);
            }
            finally
            {
                if (Directory.Exists(pluginDir))
                    Directory.Delete(pluginDir, true);
            }
        }
    }
}
