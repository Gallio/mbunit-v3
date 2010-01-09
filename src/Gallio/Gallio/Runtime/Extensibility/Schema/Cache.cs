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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Validation;

namespace Gallio.Runtime.Extensibility.Schema
{
    /// <summary>
    /// Describes a plugin cache file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type supports the framework and is not intended to be used by client code.
    /// </para>
    /// </remarks>
    [Serializable]
    [XmlRoot("cache", Namespace = SchemaConstants.XmlNamespace)]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class Cache : IValidatable
    {
        private readonly List<CachePluginInfo> pluginInfos;

        /// <summary>
        /// Creates an empty cache.
        /// </summary>
        public Cache()
        {
            pluginInfos = new List<CachePluginInfo>();
        }

        /// <summary>
        /// Gets or sets the installation id of the Gallio installation whose plugin information has been recorded in the cache, or null if not known.
        /// </summary>
        [XmlElement("installationId", IsNullable = false)]
        public string InstallationId { get; set; }

        /// <summary>
        /// Gets the mutable list of plugin information records in the cache.
        /// </summary>
        [XmlArray("pluginInfos", IsNullable = false)]
        [XmlArrayItem("pluginInfo", typeof(CachePluginInfo), IsNullable = false)]
        public List<CachePluginInfo> PluginInfos
        {
            get { return pluginInfos; }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateElementsAreNotNull("pluginInfos", pluginInfos);
            ValidationUtils.ValidateAll(pluginInfos);
        }
    }
}
