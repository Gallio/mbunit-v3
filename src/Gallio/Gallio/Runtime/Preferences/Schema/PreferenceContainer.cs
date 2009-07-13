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
using Gallio.Common.Validation;

namespace Gallio.Runtime.Preferences.Schema
{
    /// <summary>
    /// Represents a collection of preferences in Xml.
    /// </summary>
    [Serializable]
    [XmlRoot("preferences", Namespace=SchemaConstants.XmlNamespace)]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class PreferenceContainer : IValidatable
    {
        private readonly List<PreferenceSetting> settings;

        /// <summary>
        /// Creates a preference container.
        /// </summary>
        public PreferenceContainer()
        {
            settings = new List<PreferenceSetting>();
        }

        /// <summary>
        /// Gets the mutable list of preference settings.
        /// </summary>
        [XmlArray("settings", IsNullable = false)]
        [XmlArrayItem("setting", typeof(PreferenceSetting), IsNullable = false)]
        public List<PreferenceSetting> Settings
        {
            get { return settings; }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateAll(settings);
            ValidationUtils.ValidateElementsAreNotNull("settings", settings);
        }
    }
}