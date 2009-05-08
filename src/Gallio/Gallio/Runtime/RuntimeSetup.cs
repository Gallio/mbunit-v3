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
using System.Reflection;
using System.Xml.Serialization;
using Gallio.Common.IO;
using Gallio.Common.Xml;
using Gallio.Common.Reflection;
using Gallio.Runtime;

namespace Gallio.Runtime
{
    /// <summary>
    /// Provides configuration parameters for setting up the <see cref="RuntimeAccessor" />.
    /// </summary>
    [Serializable]
    [XmlRoot("runtimeSetup", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class RuntimeSetup
    {
        private readonly List<string> pluginDirectories;
        private string runtimePath;
        private InstallationConfiguration installationConfiguration;
        private string configurationFilePath;

        /// <summary>
        /// Creates a default runtime setup.
        /// </summary>
        public RuntimeSetup()
        {
            pluginDirectories = new List<string>();
        }

        /// <summary>
        /// Gets list of relative or absolute paths of directories to be
        /// searched for plugin configuration files in addition to the
        /// primary Gallio directories.
        /// </summary>
        [XmlArray("pluginDirectories", IsNullable = false)]
        [XmlArrayItem("pluginDirectory", typeof(string), IsNullable = false)]
        public List<string> PluginDirectories
        {
            get { return pluginDirectories; }
        }

        /// <summary>
        /// Gets or sets the runtime path, or null to determine it automatically
        /// based on the location of the primary runtime assemblies.  The runtime
        /// path specifies where the standard runtime plugins are located.
        /// </summary>
        /// <value>
        /// The installation path.  Default is <c>null</c>.
        /// </value>
        [XmlAttribute("runtimePath")]
        public string RuntimePath
        {
            get { return runtimePath; }
            set { runtimePath = value; }
        }

        /// <summary>
        /// Gets or sets the installation configuration which specifies characteristics
        /// about the Gallio installation on the local machine, or null to determine it automatically.
        /// </summary>
        /// <value>
        /// The installation configuration.  Default is <c>null</c>.
        /// </value>
        [XmlElement("installationConfiguration")]
        public InstallationConfiguration InstallationConfiguration
        {
            get { return installationConfiguration; }
            set { installationConfiguration = value; }
        }

        /// <summary>
        /// Gets or sets the path of the primary configuration file to be
        /// loaded by the runtime (if it exists).  This is useful
        /// when Gallio is launched by a library instead of as a standalone
        /// executable.
        /// </summary>
        /// <value>
        /// The primary configuration file path.  Default is null to load the
        /// configuration from the <see cref="AppDomain" />.
        /// </value>
        [XmlAttribute("configurationFilePath")]
        public string ConfigurationFilePath
        {
            get { return configurationFilePath; }
            set { configurationFilePath = value; }
        }

        /// <summary>
        /// Creates a deep copy of the runtime setup parameters.
        /// </summary>
        /// <returns>The copy</returns>
        public RuntimeSetup Copy()
        {
            RuntimeSetup copy = new RuntimeSetup();
            copy.pluginDirectories.AddRange(pluginDirectories);
            copy.runtimePath = runtimePath;
            copy.installationConfiguration = installationConfiguration;
            copy.configurationFilePath = configurationFilePath;
            return copy;
        }

        /// <summary>
        /// Makes all paths in this instance absolute.
        /// </summary>
        /// <param name="baseDirectory">The base directory for resolving relative paths,
        /// or null to use the current directory</param>
        public void Canonicalize(string baseDirectory)
        {
            FileUtils.CanonicalizePaths(baseDirectory, pluginDirectories);
            runtimePath = FileUtils.CanonicalizePath(baseDirectory, runtimePath);
            configurationFilePath = FileUtils.CanonicalizePath(baseDirectory, configurationFilePath);
        }

        /// <summary>
        /// Sets the configuration file path to the *.config file associated with an assembly.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public void SetConfigurationFilePathFromAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            configurationFilePath = AssemblyUtils.GetAssemblyLocalPath(assembly) + @".config";
        }
    }
}