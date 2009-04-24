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
