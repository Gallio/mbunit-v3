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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Gallio.Common.Xml;

namespace Gallio.Common.Collections
{
    /// <summary>
    /// A property bag associates keys with values where each key may have one or more associated value.
    /// All keys and values must be non-null strings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This collection supports Xml-serialization and content-based equality comparison.
    /// </para>
    /// </remarks>
    [Serializable]
    [XmlRoot("propertyBag", Namespace=SchemaConstants.XmlNamespace)]
    [XmlSchemaProvider("ProvideXmlSchema")]
    public sealed class PropertyBag : IMultiMap<string, string>, IXmlSerializable, IEquatable<PropertyBag>
    {
        private readonly IMultiMap<string, string> contents;

        private PropertyBag(IMultiMap<string, string> contents)
        {
            this.contents = contents;
        }

        /// <summary>
        /// Creates an empty property bag.
        /// </summary>
        public PropertyBag()
            : this(new MultiMap<string, string>())
        {
        }

        /// <summary>
        /// Creates a copy of this property bag.
        /// </summary>
        /// <returns>The copy.</returns>
        public PropertyBag Copy()
        {
            PropertyBag copy = new PropertyBag();
            copy.AddAll(this);
            return copy;
        }

        /// <summary>
        /// Gets a read-only view of this property set.
        /// </summary>
        /// <returns>A read-only view.</returns>
        public PropertyBag AsReadOnly()
        {
            return contents.IsReadOnly ? this : new PropertyBag(MultiMap<string, string>.ReadOnly(contents));
        }

        /// <summary>
        /// Gets the first value associated with a key.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there are multiple values, returns only the first one.
        /// </para>
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <returns>The first associated value, or null if none.</returns>
        public string GetValue(string key)
        {
            IList<string> values = this[key];
            if (values.Count == 0)
                return null;

            return values[0];
        }

        /// <summary>
        /// Sets the value associated with a key.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Removes all values previously associated with that key.
        /// </para>
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="value">The new value, or null to remove the existing values.</param>
        public void SetValue(string key, string value)
        {
            Remove(key);

            if (value != null)
                Add(key, value);
        }

        #region MultiMap delegating methods
        /// <inheritdoc />
        public void Add(string key, string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            contents.Add(key, value);
        }

        /// <inheritdoc />
        public void AddAll(IEnumerable<KeyValuePair<string, IList<string>>> map)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            foreach (KeyValuePair<string, IList<string>> entry in map)
                Add(entry.Key, entry.Value);
        }

        /// <inheritdoc />
        public void AddAll(IEnumerable<KeyValuePair<string, string>> pairs)
        {
            if (pairs == null)
                throw new ArgumentNullException("pairs");

            foreach (KeyValuePair<string, string> entry in pairs)
                Add(entry.Key, entry.Value);
        }

        /// <inheritdoc />
        public bool Contains(string key, string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return contents.Contains(key, value);
        }

        /// <inheritdoc />
        public bool Remove(string key, string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return contents.Remove(key, value);
        }

        /// <inheritdoc />
        public bool ContainsKey(string key)
        {
            return contents.ContainsKey(key);
        }

        /// <inheritdoc />
        public void Add(string key, IList<string> values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            foreach (string value in values)
                Add(key, value);
        }

        /// <inheritdoc />
        public bool Remove(string key)
        {
            return contents.Remove(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(string key, out IList<string> value)
        {
            return contents.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public IList<string> this[string key]
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
        public ICollection<IList<string>> Values
        {
            get { return contents.Values; }
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, string>> Pairs
        {
            get { return contents.Pairs; }
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<string, IList<string>> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            contents.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<string, IList<string>> item)
        {
            return contents.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<string, IList<string>>[] array, int arrayIndex)
        {
            contents.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<string, IList<string>> item)
        {
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

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public IEnumerator<KeyValuePair<string, IList<string>>> GetEnumerator()
        {
            return contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return contents.GetEnumerator();
        }
        #endregion

        #region Equality
        /// <inheritdoc />
        public bool Equals(PropertyBag other)
        {
            if (other == null)
                return false;
            if (Count != other.Count)
                return false;

            foreach (KeyValuePair<string, IList<string>> entry in this)
            {
                IList<string> otherValues;
                if (!other.TryGetValue(entry.Key, out otherValues))
                    return false;

                if (!GenericCollectionUtils.ElementsEqualOrderIndependent(entry.Value, otherValues))
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as PropertyBag);
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
        /// <returns>The schema type of the element.</returns>
        public static XmlQualifiedName ProvideXmlSchema(XmlSchemaSet schemas)
        {
            schemas.Add(new XmlSchema()
            {
                TargetNamespace = SchemaConstants.XmlNamespace,
                Items =
                    {
                        new XmlSchemaComplexType()
                        {
                            Name = "PropertyBag",
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
                                                            SchemaTypeName =
                                                                new XmlQualifiedName("string",
                                                                    "http://www.w3.org/2001/XMLSchema")
                                                        },
                                                    },
                                                Particle = new XmlSchemaSequence()
                                                {
                                                    Items =
                                                        {
                                                            new XmlSchemaElement()
                                                            {
                                                                Name = "value",
                                                                IsNillable = false,
                                                                MinOccursString = "1",
                                                                MaxOccursString = "unbounded",
                                                                SchemaTypeName =
                                                                    new XmlQualifiedName("string",
                                                                        "http://www.w3.org/2001/XMLSchema")
                                                            }
                                                        }
                                                }
                                            }
                                        }
                                    }
                            }
                        }
                    }
            });

            return new XmlQualifiedName("PropertyBag", SchemaConstants.XmlNamespace);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotSupportedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            bool isEmptyMetadata = reader.IsEmptyElement;
            reader.ReadStartElement();

            if (isEmptyMetadata)
                return;

            while (reader.MoveToContent() == XmlNodeType.Element)
            {
                bool isEmpty = reader.IsEmptyElement;
                string key = reader.GetAttribute(@"key");
                reader.ReadStartElement(@"entry");

                if (key == null)
                    throw new XmlException("Expected key attribute.");

                if (!isEmpty)
                {
                    while (reader.MoveToContent() == XmlNodeType.Element)
                    {
                        if (reader.LocalName != @"value")
                            throw new XmlException("Expected <value> element");

                        string value = reader.ReadElementContentAsString();
                        Add(key, value);
                    }

                    if (reader.NodeType == XmlNodeType.EndElement)
                        reader.ReadEndElement();
                }
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (KeyValuePair<string, IList<string>> entry in this)
            {
                writer.WriteStartElement(@"entry");
                writer.WriteAttributeString(@"key", entry.Key);

                foreach (string value in entry.Value)
                {
                    if (value == null)
                        throw new InvalidOperationException("The property bag contains an invalid null value.");

                    writer.WriteStartElement(@"value");
                    writer.WriteValue(value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
        #endregion
    }
}