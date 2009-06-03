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
using Gallio.Common.Validation;

namespace Gallio.Schema.Plugins
{
    /// <summary>
    /// Describes a dependency on another plugin.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class Dependency : IValidatable
    {
        private string pluginId;

        /// <summary>
        /// Creates an uninitialized dependency for XML deserialization.
        /// </summary>
        private Dependency()
        {
        }

        /// <summary>
        /// Creates an plugin dependency reference.
        /// </summary>
        /// <param name="pluginId">The referenced plugin id.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginId"/> is null.</exception>
        public Dependency(string pluginId)
        {
            if (pluginId == null)
                throw new ArgumentNullException("pluginId");

            this.pluginId = pluginId;
        }

        /// <summary>
        /// Gets or sets the referenced plugin id.
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

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("pluginId", pluginId);
        }
    }
}
