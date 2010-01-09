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
using System.IO;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Populates a plugin catalog with plugin metadata.
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// Gets or sets the installation id.
        /// </summary>
        Guid InstallationId { get; set; }

        /// <summary>
        /// Adds a plugin path.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the plugin path refers to a specific plugin file, it will be loaded when
        /// the catalog is populated.  Otherwise if the plugin path refers to a directory,
        /// then the directory will be scanned for *.plugin files when the catalog is
        /// populated.
        /// </para>
        /// </remarks>
        /// <param name="pluginPath">The plugin path to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginPath"/> is null.</exception>
        void AddPluginPath(string pluginPath);

        /// <summary>
        /// Adds a plugin definition as XML.
        /// </summary>
        /// <param name="pluginXml">The plugin xml.</param>
        /// <param name="baseDirectory">The plugin base directory.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginXml"/>
        /// or <paramref name="baseDirectory"/> is null.</exception>
        void AddPluginXml(string pluginXml, DirectoryInfo baseDirectory);

        /// <summary>
        /// Defines a constant to be used by the preprocessor.
        /// </summary>
        /// <param name="constant">The constant name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constant"/> is null.</exception>
        void DefinePreprocessorConstant(string constant);

        /// <summary>
        /// Populates a catalog with plugin metadata derived from the currently
        /// registered plugin data sources.
        /// </summary>
        /// <param name="catalog">The plugin catalog to populate.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="catalog"/>
        /// is null.</exception>
        void PopulateCatalog(IPluginCatalog catalog);
    }
}
