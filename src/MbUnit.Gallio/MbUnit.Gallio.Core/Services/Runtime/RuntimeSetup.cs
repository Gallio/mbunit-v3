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
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Runtime
{
    /// <summary>
    /// Provides configuration parameters for setting up the runtime.
    /// </summary>
    [Serializable]
    [XmlRoot("runtimeSetup", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class RuntimeSetup
    {
        private List<string> pluginDirectories;

        /// <summary>
        /// Creates a default runtime setup.
        /// </summary>
        public RuntimeSetup()
        {
            pluginDirectories = new List<string>();
        }

        /// <summary>
        /// Gets or sets the relative or absolute paths of directories to be
        /// searched for plugin configuration files in addition to the
        /// primary MbUnit directories.
        /// </summary>
        [XmlArray("pluginDirectories", IsNullable = false)]
        [XmlArrayItem("pluginDirectory", IsNullable = false)]
        public string[] PluginDirectories
        {
            get { return pluginDirectories.ToArray(); }
            set { pluginDirectories = new List<string>(value); }
        }

        /// <summary>
        /// Adds a plugin directory to the list.
        /// </summary>
        /// <param name="pluginDirectory">The plugin directory to add</param>
        public void AddPluginDirectory(string pluginDirectory)
        {
            pluginDirectories.Add(pluginDirectory);
        }

        /// <summary>
        /// Creates a deep copy of the runtime setup parameters.
        /// </summary>
        /// <returns>The copy</returns>
        public virtual RuntimeSetup Copy()
        {
            RuntimeSetup copy = new RuntimeSetup();
            copy.pluginDirectories = new List<string>(pluginDirectories);
            return copy;
        }
    }
}
