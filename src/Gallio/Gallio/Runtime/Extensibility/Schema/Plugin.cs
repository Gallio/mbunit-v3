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
using System.Text;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Validation;

namespace Gallio.Runtime.Extensibility.Schema
{
    /// <summary>
    /// Represents a plugin descriptor in Xml.
    /// </summary>
    [Serializable]
    [XmlRoot("plugin", Namespace=SchemaConstants.XmlNamespace)]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class Plugin : IValidatable
    {
        private string pluginId;
        private string pluginType;
        private readonly List<Dependency> dependencies;
        private readonly List<File> files;
        private readonly List<Assembly> assemblies;
        private readonly List<Service> services;
        private readonly List<Component> components;
        private readonly List<string> probingPaths;

        /// <summary>
        /// Creates an uninitialized plugin descriptor for XML deserialization.
        /// </summary>
        private Plugin()
        {
            dependencies = new List<Dependency>();
            files = new List<File>();
            assemblies = new List<Assembly>();
            services = new List<Service>();
            components = new List<Component>();
            probingPaths = new List<string>();
        }

        /// <summary>
        /// Creates a plugin descriptor with an id.
        /// </summary>
        /// <param name="pluginId">The plugin id.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginId"/> is null.</exception>
        public Plugin(string pluginId)
            : this()
        {
            if (pluginId == null)
                throw new ArgumentNullException("pluginId");

            this.pluginId = pluginId;
        }

        /// <summary>
        /// Gets or sets the plugin id.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("pluginId")]
        public string PluginId
        {
            get { return pluginId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                pluginId = value;
            }
        }

        /// <summary>
        /// Gets or sets the recommended installation path for the plugin files relative to
        /// the runtime installation directory, or null if there is no preference.
        /// </summary>
        [XmlAttribute("recommendedInstallationPath")]
        public string RecommendedInstallationPath { get; set; }

        /// <summary>
        /// Gets or sets the assembly-qualified plugin type name, or null if the default
        /// implementation may be used.
        /// </summary>
        [XmlAttribute("pluginType")]
        public string PluginType
        {
            get { return pluginType; }
            set { pluginType = value; }
        }

        /// <summary>
        /// Gets the mutable list of plugin dependencies.
        /// </summary>
        [XmlArray("dependencies", IsNullable = false)]
        [XmlArrayItem("dependency", typeof(Dependency), IsNullable = false)]
        public List<Dependency> Dependencies
        {
            get { return dependencies; }
        }

        /// <summary>
        /// Gets the mutable list of files that belong to a plugin.
        /// </summary>
        [XmlArray("files", IsNullable = false)]
        [XmlArrayItem("file", typeof(File), IsNullable = false)]
        public List<File> Files
        {
            get { return files; }
        }

        /// <summary>
        /// Gets the mutable list of plugin assembly bindings.
        /// </summary>
        [XmlArray("assemblies", IsNullable = false)]
        [XmlArrayItem("assembly", typeof(Assembly), IsNullable = false)]
        public List<Assembly> Assemblies
        {
            get { return assemblies; }
        }

        /// <summary>
        /// Gets the mutable list of services belonging to the plugin.
        /// </summary>
        [XmlArray("services", IsNullable = false)]
        [XmlArrayItem("service", typeof(Service), IsNullable = false)]
        public List<Service> Services
        {
            get { return services; }
        }

        /// <summary>
        /// Gets the mutable list of components belonging to the plugin.
        /// </summary>
        [XmlArray("components", IsNullable = false)]
        [XmlArrayItem("component", typeof(Component), IsNullable = false)]
        public List<Component> Components
        {
            get { return components; }
        }

        /// <summary>
        /// Gets or sets the plugin parameters, or null if there are none.
        /// </summary>
        [XmlElement("parameters", IsNullable = false)]
        public KeyValueTable Parameters { get; set; }

        /// <summary>
        /// Gets or sets the plugin traits, or null if there are none.
        /// </summary>
        [XmlElement("traits", IsNullable = false)]
        public KeyValueTable Traits { get; set; }

        /// <summary>
        /// Gets the mutable list of additional probing paths in which to
        /// attempt to locate referenced assemblies.
        /// </summary>
        [XmlArray("probingPaths", IsNullable = false)]
        [XmlArrayItem("probingPath", typeof(string), IsNullable = false)]
        public List<string> ProbingPaths
        {
            get { return probingPaths; }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("pluginId", pluginId);
            ValidationUtils.ValidateElementsAreNotNull("dependencies", dependencies);
            ValidationUtils.ValidateAll(dependencies);
            ValidationUtils.ValidateElementsAreNotNull("files", files);
            ValidationUtils.ValidateAll(files);
            ValidationUtils.ValidateElementsAreNotNull("assemblies", assemblies);
            ValidationUtils.ValidateAll(assemblies);
            ValidationUtils.ValidateElementsAreNotNull("services", services);
            ValidationUtils.ValidateAll(services);
            ValidationUtils.ValidateElementsAreNotNull("components", components);
            ValidationUtils.ValidateAll(components);
            ValidationUtils.ValidateElementsAreNotNull("probingPaths", probingPaths);
        }
    }
}
