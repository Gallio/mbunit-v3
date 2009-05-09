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
    /// Describes information about a plugin whose metadata has been cached.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type supports the framework and is not intended to be used by client code.
    /// </para>
    /// </remarks>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class CachePluginInfo : IValidatable
    {
        /// <summary>
        /// Gets or sets the plugin file path.
        /// </summary>
        [XmlAttribute("pluginFile")]
        public string PluginFile { get; set; }

        /// <summary>
        /// Gets or sets the plugin file modification time stamp.
        /// </summary>
        [XmlAttribute("pluginFileModificationTime")]
        public DateTime PluginFileModificationTime { get; set; }

        /// <summary>
        /// Gets or sets the plugin metadata.
        /// </summary>
        [XmlElement("plugin", IsNullable = false)]
        public Plugin Plugin { get; set; }

        /// <summary>
        /// Gets or sets the plugin base directory path.
        /// </summary>
        [XmlAttribute("baseDirectory")]
        public string BaseDirectory { get; set; }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("pluginFile", PluginFile);
            ValidationUtils.ValidateNotNull("plugin", Plugin);
            Plugin.Validate();
            ValidationUtils.ValidateNotNull("baseDirectory", BaseDirectory);
        }
    }
}
