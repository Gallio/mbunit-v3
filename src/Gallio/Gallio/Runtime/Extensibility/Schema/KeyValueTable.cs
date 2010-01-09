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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Collections;

namespace Gallio.Runtime.Extensibility.Schema
{
    /// <summary>
    /// Represents a table of key and value pairs as a sequence of Xml elements whose local name
    /// represents the key and whose text body contains the value.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class KeyValueTable
    {
        private PropertySet propertySet;

        /// <summary>
        /// Creates an empty key value table.
        /// </summary>
        public KeyValueTable()
        {
        }

        /// <summary>
        /// Gets or sets the Xml elements that describe the property entries
        /// during serialization.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAnyElement]
        public XmlElement[] Entries
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                return GenericCollectionUtils.ConvertAllToArray(PropertySet, entry =>
                {
                    XmlElement element = doc.CreateElement(entry.Key, SchemaConstants.XmlNamespace);
                    element.InnerText = entry.Value;
                    return element;
                });
            }
            set
            {
                if (value == null || Array.IndexOf(value, null) >= 0)
                    throw new ArgumentNullException("value");

                PropertySet.Clear();
                foreach (XmlElement element in value)
                {
                    PropertySet.Add(element.LocalName, element.InnerText ?? "");
                }
            }
        }

        /// <summary>
        /// Gets or sets a mutable view of the properties as a property set.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlIgnore]
        public PropertySet PropertySet
        {
            get
            {
                if (propertySet == null)
                    propertySet = new PropertySet();
                return propertySet;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                propertySet = value;
            }
        }
    }
}
