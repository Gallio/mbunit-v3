// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(RegistryResourceLocator))]
    public class RegistryResourceLocatorTest
    {
        [Test]
        public void Constructor_WhenBaseDirectoryIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RegistryResourceLocator(null));
        }

        [Test]
        public void Constructor_WhenArgumentsValid_InitializesProperties()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();

            var locator = new RegistryResourceLocator(registry);

            Assert.AreSame(registry, locator.Registry);
        }

        [Test]
        public void ResolveResourcePath_WhenResourceUriNull_Throws()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();
            var locator = new RegistryResourceLocator(registry);

            Assert.Throws<ArgumentNullException>(() => locator.ResolveResourcePath(null));
        }

        [Test]
        public void ResourceResourcePath_WhenResourceUriIsInFileScheme_ReturnsLocalPath()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();
            var locator = new RegistryResourceLocator(registry);

            var path = locator.ResolveResourcePath(new Uri("file:///c:/somefile.txt"));

            Assert.AreEqual(@"c:\somefile.txt", path);
        }

        [Test]
        public void ResourceResourcePath_WhenResourceUriIsInPluginSchemeAndPluginCanBeResolved_ReturnsPathRelativeToPluginBaseDirectory()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();
            var plugins = MockRepository.GenerateStub<IPlugins>();
            var pluginDescriptor = MockRepository.GenerateStub<IPluginDescriptor>();
            var pluginList = new List<IPluginDescriptor>(new[] { pluginDescriptor });
            registry.Stub(x => x.Plugins).Return(plugins);
            plugins.Stub(x => x.GetEnumerator()).Return(pluginList.GetEnumerator());
            pluginDescriptor.Stub(x => x.PluginId).Return("pluginId");
            pluginDescriptor.Stub(x => x.BaseDirectory).Return(new DirectoryInfo(@"C:\PluginBase"));
            var locator = new RegistryResourceLocator(registry);

            var path = locator.ResolveResourcePath(new Uri("plugin://pluginId/Path/somefile.txt"));

            Assert.AreEqual(@"C:\PluginBase\Path\somefile.txt", path);
        }

        [Test]
        public void ResourceResourcePath_WhenResourceUriIsInPluginSchemeAndPluginCannotBeResolved_Throws()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();
            var plugins = MockRepository.GenerateStub<IPlugins>();
            var pluginList = new List<IPluginDescriptor>(new IPluginDescriptor[] { });
            registry.Stub(x => x.Plugins).Return(plugins);
            plugins.Stub(x => x.GetEnumerator()).Return(pluginList.GetEnumerator());
            var locator = new RegistryResourceLocator(registry);

            var ex = Assert.Throws<RuntimeException>(() => locator.ResolveResourcePath(new Uri("plugin://pluginId/PluginBase/somefile.txt")));
            Assert.AreEqual("Could not resolve resource uri 'plugin://pluginid/PluginBase/somefile.txt' because no plugin appears to be registered with the requested id.", ex.Message);
        }

        [Test]
        public void ResourceResourcePath_WhenResourceUriIsInUnrecognized_Throws()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();
            var locator = new RegistryResourceLocator(registry);

            var ex = Assert.Throws<RuntimeException>(() => locator.ResolveResourcePath(new Uri("bad-scheme:///somefile.txt")));
            Assert.AreEqual("Could not resolve resource uri 'bad-scheme:///somefile.txt' because the scheme was not recognized.  The uri scheme must be 'file' or 'plugin'.", ex.Message);
        }
    }
}
