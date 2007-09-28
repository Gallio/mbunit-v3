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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Metadata
{
    /// <summary>
    /// A metadata map is a multi-valued dictionary of metadata keys and values associated 
    /// with a model element.  Metadata is used to communicate declarative 
    /// properties of the model in an extensible manner.
    /// </summary>
    [Serializable]
    [XmlRoot("metadata", Namespace=SerializationUtils.XmlNamespace)]
    public sealed class MetadataMap : IXmlSerializable
    {
        private readonly MultiMap<string, string> entries;

        /// <summary>
        /// Creates an empty metadata map.
        /// </summary>
        public MetadataMap()
        {
            entries = new MultiMap<string, string>();
        }

        /// <summary>
        /// Gets the multi-valued dictionary of metadata entries.
        /// </summary>
        [XmlIgnore]
        public MultiMap<string, string> Entries
        {
            get { return entries; }
        }

        /// <summary>
        /// Creates a copy of the metadata map.
        /// </summary>
        /// <returns>The copy</returns>
        public MetadataMap Copy()
        {
            MetadataMap copy = new MetadataMap();
            copy.Entries.AddAll(entries);
            return copy;
        }

        /// <summary>
        /// Gets the value associated with the metadata key.
        /// If there are multiple values, returns only the first one.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value, or null if none</returns>
        public string GetValue(string key)
        {
            IList<string> values = entries[key];
            if (values.Count == 0)
                return null;

            return values[0];
        }

        /// <summary>
        /// Sets the value associated with the metadata key.
        /// Removes all values previously associated with that key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The new value, or null to remove the value</param>
        public void SetValue(string key, string value)
        {
            entries.Remove(key);

            if (value != null)
                entries.Add(key, value);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("metadata");

            while (reader.MoveToContent() == XmlNodeType.Element)
            {
                bool isEmpty = reader.IsEmptyElement;
                string key = reader.GetAttribute("key");
                reader.ReadStartElement("entry");

                if (key == null)
                    throw new XmlException("Expected key attribute.");

                if (!isEmpty)
                {
                    while (reader.MoveToContent() == XmlNodeType.Element)
                    {
                        if (reader.LocalName != "value")
                            throw new XmlException("Expected <value> element");

                        string value = reader.ReadElementContentAsString();
                        entries.Add(key, value);
                    }

                    if (reader.NodeType == XmlNodeType.EndElement)
                        reader.ReadEndElement();
                }
            }

            if (reader.NodeType == XmlNodeType.EndElement)
                reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (KeyValuePair<string, IList<string>> entry in entries)
            {
                writer.WriteStartElement("entry");
                writer.WriteAttributeString("key", entry.Key);

                foreach (string value in entry.Value)
                {
                    if (value == null)
                        throw new InvalidOperationException("The metadata map contains an invalid null value.");

                    writer.WriteStartElement("value");
                    writer.WriteValue(value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}