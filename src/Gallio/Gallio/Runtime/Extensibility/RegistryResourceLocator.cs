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
using System.Text;
using Gallio.Common.Collections;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A resource locator implementation based on a registry.
    /// </summary>
    public class RegistryResourceLocator : IResourceLocator
    {
        private readonly IRegistry registry;

        /// <summary>
        /// Creates a resource locator based on a registry.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registry"/> is null.</exception>
        public RegistryResourceLocator(IRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException("registry");

            this.registry = registry;
        }

        /// <summary>
        /// Gets the registry.
        /// </summary>
        public IRegistry Registry
        {
            get { return registry; }
        }

        /// <inheritdoc />
        public string ResolveResourcePath(Uri resourceUri)
        {
            if (resourceUri == null)
                throw new ArgumentNullException("resourceUri");

            if (resourceUri.IsFile)
            {
                return Path.GetFullPath(resourceUri.LocalPath);
            }

            if (resourceUri.Scheme == @"plugin")
            {
                string pluginId = resourceUri.Host;
                string relativePath = resourceUri.PathAndQuery;
                IPluginDescriptor plugin = GenericCollectionUtils.Find(registry.Plugins,
                    p => string.Compare(pluginId, p.PluginId, true) == 0);
                if (plugin == null)
                    throw new RuntimeException(String.Format("Could not resolve resource uri '{0}' because no plugin appears to be registered with the requested id.", resourceUri));

                string pluginPath = plugin.BaseDirectory.FullName;
                if (relativePath.Length == 0 || relativePath == @"/")
                    return pluginPath;

                string relativeLocalPath = relativePath.Substring(1).Replace('/', Path.DirectorySeparatorChar);
                return Path.Combine(pluginPath, relativeLocalPath);
            }

            throw new RuntimeException(String.Format("Could not resolve resource uri '{0}' because the scheme was not recognized.  The uri scheme must be 'file' or 'plugin'.", resourceUri));
        }
    }
}
