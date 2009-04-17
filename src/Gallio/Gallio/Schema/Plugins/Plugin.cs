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
using Gallio.Collections;

namespace Gallio.Schema.Plugins
{
    /// <summary>
    /// Represents a plugin descriptor in Xml.
    /// </summary>
    [Serializable]
    [XmlRoot("plugin", Namespace=SchemaConstants.XmlNamespace)]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class Plugin
    {
        private string pluginId;
        private string pluginType;
        private readonly List<Service> services;
        private readonly List<Component> components;
        private PropertySet parameters;
        private PropertySet traits;

        /// <summary>
        /// Creates an uninitialized plugin descriptor for XML deserialization.
        /// </summary>
        private Plugin()
        {
            services = new List<Service>();
            components = new List<Component>();
        }

        /// <summary>
        /// Creates a plugin descriptor with an id.
        /// </summary>
        /// <param name="pluginId">The plugin id</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginId"/> is null</exception>
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
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
        /// Gets or sets the plugin parameters.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("parameters", IsNullable = true)]
        public PropertySet Parameters
        {
            get
            {
                if (parameters == null)
                    parameters = new PropertySet();
                return parameters;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                parameters = value;
            }
        }

        /// <summary>
        /// Gets or sets the plugin traits.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("traits", IsNullable = true)]
        public PropertySet Traits
        {
            get
            {
                if (traits == null)
                    traits = new PropertySet();
                return traits;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                traits = value;
            }
        }
    }
}
