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

namespace Gallio.Runtime.Preferences.Schema
{
    /// <summary>
    /// Represents a preference setting in Xml.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public class PreferenceSetting : IValidatable
    {
        private string name;
        private string value;

        /// <summary>
        /// Creates an uninitialized preference setting for XML deserialization.
        /// </summary>
        private PreferenceSetting()
        {
        }

        /// <summary>
        /// Creates a preference setting.
        /// </summary>
        /// <param name="name">The setting name.</param>
        /// <param name="value">The setting value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="value"/> is null.</exception>
        public PreferenceSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the preference setting name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the preference setting value.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("value")]
        public string Value
        {
            get { return value; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.value = value;
            }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("name", Name);
            ValidationUtils.ValidateNotNull("value", Value);
        }
    }
}