// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Xml.Serialization;

using Gallio.Model.Serialization;

namespace Gallio.Icarus
{
    [Serializable]
    [XmlRoot("settings", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class Settings : ICloneable
    {
        private bool restorePreviousSettings = true;
        private List<string> pluginDirectories;

        [XmlAttribute("restorePreviousSettings")]
        public bool RestorePreviousSettings
        {
            get { return restorePreviousSettings; }
            set { restorePreviousSettings = value; }
        }

        [XmlArray("pluginDirectories", IsNullable = false)]
        [XmlArrayItem("pluginDirectory", typeof(string), IsNullable = false)]
        public List<string> PluginDirectories
        {
            get { return pluginDirectories; }
            set
            {
                pluginDirectories.Clear();
                foreach (string dir in value)
                    pluginDirectories.Add(dir);
            }
        }

        public Settings()
        {
            pluginDirectories = new List<string>();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Settings Clone()
        {
            return (Settings)MemberwiseClone();
        }
    }
}