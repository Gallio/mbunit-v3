// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using MbUnit.Model.Serialization;

namespace MbUnit.Hosting
{
    /// <summary>
    /// Provides configuration parameters for setting up the runtime.
    /// </summary>
    [Serializable]
    [XmlRoot("runtimeSetup", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class RuntimeSetup
    {
        private readonly List<string> pluginDirectories;
        private string runtimeFactoryType;

        /// <summary>
        /// Creates a default runtime setup.
        /// </summary>
        public RuntimeSetup()
        {
            pluginDirectories = new List<string>();
        }

        /// <summary>
        /// Gets or sets the full assembly-qualified name of a type that
        /// implements <see cref="IRuntimeFactory" /> that should be used
        /// to create the runtime.  If the value is null, the built-in
        /// default runtime factory will be used.
        /// </summary>
        [XmlElement("runtimeFactoryType", IsNullable=true)]
        public string RuntimeFactoryType
        {
            get { return runtimeFactoryType; }
            set { runtimeFactoryType = value; }
        }

        /// <summary>
        /// Gets list of relative or absolute paths of directories to be
        /// searched for plugin configuration files in addition to the
        /// primary MbUnit directories.
        /// </summary>
        [XmlArray("pluginDirectories", IsNullable = false)]
        [XmlArrayItem("pluginDirectory", typeof(string), IsNullable = false)]
        public List<string> PluginDirectories
        {
            get { return pluginDirectories; }
        }

        /// <summary>
        /// Creates a deep copy of the runtime setup parameters.
        /// </summary>
        /// <returns>The copy</returns>
        public virtual RuntimeSetup Copy()
        {
            RuntimeSetup copy = new RuntimeSetup();
            copy.pluginDirectories.AddRange(pluginDirectories);
            copy.runtimeFactoryType = runtimeFactoryType;
            return copy;
        }
    }
}
