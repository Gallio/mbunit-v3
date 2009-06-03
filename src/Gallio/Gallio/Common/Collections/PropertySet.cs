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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Gallio.Common.Xml;

namespace Gallio.Common.Collections
{
    /// <summary>
    /// A property set associates keys with values where each key may have exactly one associated value.
    /// All keys and values must be non-null strings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This collection supports Xml-serialization and content-based equality comparison.
    /// </para>
    /// </remarks>
    [Serializable]
    [XmlRoot("propertySet", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlSchemaProvider("ProvideXmlSchema")]
    public sealed class PropertySet : IDictionary<string, string>, IEquatable<PropertySet>, IXmlSerializable
    {
        private readonly IDictionary<string, string> contents;

        private PropertySet(IDictionary<string, string> contents)
        {
            this.contents = contents;
        }

        /// <summary>
        /// Creates an empty property set.
        /// </summary>
        public PropertySet()
            : this(new Dictionary<string, string>())
        {
        }

        /// <summary>
        /// Adds all elements of another collection to this dictionary.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="collection"/> is null.</exception>
        public void AddAll(IEnumerable<KeyValuePair<string, string>> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (IsReadOnly)
                throw new NotSupportedException("Collection is read only.");

            foreach (KeyValuePair<string, string> entry in collection)
                Add(entry);
        }

        /// <summary>
        /// Creates a full copy of this property set.
        /// </summary>
        /// <returns>The copy</returns>
        public PropertySet Copy()
        {
            var copy = new PropertySet();
            copy.AddAll(this);
            return copy;
        }

        /// <summary>
        /// Gets a read-only view of this property set.
        /// </summary>
        /// <returns>A read-only view</returns>
        public PropertySet AsReadOnly()
        {
            return contents.IsReadOnly ? this : new PropertySet(new ReadOnlyDictionary<string, string>(contents));
        }

        /// <summary>
        /// Gets the value associated with a key, or null if absent.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The associated value, or null if none</returns>
        public string GetValue(string key)
        {
            string value;
            TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Sets the value associated with a key, removes an existing value if null.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The new value, or null to remove the existing value.</param>
        public void SetValue(string key, string value)
        {
            Remove(key);

            if (value != null)
                Add(key, value);
        }

        #region IDictionary delegating members
        /// <inheritdoc />
        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            contents.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<string, string> item)
        {
            if (item.Value == null)
                throw new ArgumentException("The item Value must not be null.", "item");

            return contents.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            contents.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<string, string> item)
        {
            if (item.Value == null)
                throw new ArgumentException("The item Value must not be null.", "item");

            return contents.Remove(item);
        }

        /// <inheritdoc />
        public int Count
        {
            get { return contents.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return contents.IsReadOnly; }
        }

        /// <inheritdoc />
        public bool ContainsKey(string key)
        {
            return contents.ContainsKey(key);
        }

        /// <inheritdoc />
        public void Add(string key, string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            contents.Add(key, value);
        }

        /// <inheritdoc />
        public bool Remove(string key)
        {
            return contents.Remove(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(string key, out string value)
        {
            return contents.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public string this[string key]
        {
            get { return contents[key]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                contents[key] = value;
            }
        }

        /// <inheritdoc />
        public ICollection<string> Keys
        {
            get { return contents.Keys; }
        }

        /// <inheritdoc />
        public ICollection<string> Values
        {
            get { return contents.Values; }
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Equality
        /// <inheritdoc />
        public bool Equals(PropertySet other)
        {
            if (other == null)
                return false;
            if (Count != other.Count)
                return false;

            foreach (KeyValuePair<string, string> entry in this)
            {
                string otherValue;
                if (!other.TryGetValue(entry.Key, out otherValue))
                    return false;
                if (entry.Value != otherValue)
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as PropertySet);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // FIXME: Can make this better...
            return Count;
        }
        #endregion

        #region Xml Serialization
        /// <summary>
        /// Provides the Xml schema for this element.
        /// </summary>
        /// <param name="schemas">The schema set.</param>
        /// <returns>The schema type of the element</returns>
        public static XmlQualifiedName ProvideXmlSchema(XmlSchemaSet schemas)
        {
            schemas.Add(new XmlSchema()
            {
                TargetNamespace = XmlSerializationUtils.GallioNamespace,
                Items =
                {
                    new XmlSchemaComplexType()
                    {
                        Name = "PropertySet",
                        Particle = new XmlSchemaSequence()
                        {
                            Items = 
                            {
                                new XmlSchemaElement()
                                {
                                    Name = "entry",
                                    IsNillable = false,
                                    MinOccursString = "0",
                                    MaxOccursString = "unbounded",
                                    SchemaType = new XmlSchemaComplexType()
                                    {
                                        Attributes =
                                        {
                                            new XmlSchemaAttribute()
                                            {
                                                Name = "key",
                                                Use = XmlSchemaUse.Required,
                                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                            },
                                            new XmlSchemaAttribute()
                                            {
                                                Name = "value",
                                                Use = XmlSchemaUse.Required,
                                                SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });

            return new XmlQualifiedName("PropertySet", XmlSerializationUtils.GallioNamespace);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotSupportedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            bool isEmptyContainer = reader.IsEmptyElement;
            reader.ReadStartElement();

            if (isEmptyContainer)
                return;

            while (reader.MoveToContent() == XmlNodeType.Element)
            {
                bool isEmpty = reader.IsEmptyElement;
                string key = reader.GetAttribute(@"key");
                string value = reader.GetAttribute(@"value");
                reader.ReadStartElement(@"entry");

                if (key == null || value == null)
                    throw new XmlException("Expected key and value attributes.");

                Add(key, value);

                if (!isEmpty && reader.NodeType == XmlNodeType.EndElement)
                    reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (KeyValuePair<string, string> entry in this)
            {
                writer.WriteStartElement(@"entry");
                writer.WriteAttributeString(@"key", entry.Key);
                writer.WriteAttributeString(@"value", entry.Value);
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
