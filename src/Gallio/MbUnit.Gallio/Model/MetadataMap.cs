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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MbUnit.Collections;
using MbUnit.Model.Serialization;

namespace MbUnit.Model
{
    /// <summary>
    /// A metadata map is a multi-valued dictionary of metadata keys and values associated 
    /// with a model element.  Metadata is used to communicate declarative 
    /// properties of the model in an extensible manner.
    /// </summary>
    [Serializable]
    [XmlRoot("metadata", Namespace=SerializationUtils.XmlNamespace)]
    public sealed class MetadataMap : IMultiMap<string, string>, IXmlSerializable
    {
        private IMultiMap<string, string> contents;

        private MetadataMap(IMultiMap<string, string> contents)
        {
            this.contents = contents;
        }

        /// <summary>
        /// Creates an empty metadata map.
        /// </summary>
        public MetadataMap()
            : this(new MultiMap<string, string>())
        {
        }

        /// <summary>
        /// Creates a copy of the metadata map.
        /// </summary>
        /// <returns>The copy</returns>
        public MetadataMap Copy()
        {
            MetadataMap copy = new MetadataMap();
            copy.AddAll(this);
            return copy;
        }

        /// <summary>
        /// Creates a read-only view of the metadata map.
        /// </summary>
        /// <returns>The read-only view</returns>
        public MetadataMap AsReadOnly()
        {
            if (contents.IsReadOnly)
                return this;
            return new MetadataMap(MultiMap<string, string>.ReadOnly(this));
        }

        /// <summary>
        /// Gets the value associated with the metadata key.
        /// If there are multiple values, returns only the first one.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value, or null if none</returns>
        public string GetValue(string key)
        {
            IList<string> values = this[key];
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
            Remove(key);

            if (value != null)
                Add(key, value);
        }

        /// <summary>
        /// Adds metadata in a thread-safe manner by performing an atomic copy-on-write of the
        /// internal storage collection.  This method must be used when a component's metadata
        /// is updated while tests are in flight and may potentially be accessed concurrently.
        /// </summary>
        /// <param name="metadataKey">The metadata key</param>
        /// <param name="metadataValue">The metadata value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadataKey"/>
        /// or <paramref name="metadataValue"/> is null</exception>
        internal void CopyOnWriteAdd(string metadataKey, string metadataValue)
        {
            if (metadataKey == null)
                throw new ArgumentNullException(@"metadataKey");
            if (metadataValue == null)
                throw new ArgumentNullException(@"metadataValue");
            if (IsReadOnly)
                throw new NotSupportedException("The metadata is read-only.");

            for (; ; )
            {
                IMultiMap<string, string> original = contents;
                IMultiMap<string, string> copy = new MultiMap<string, string>();
                copy.AddAll(original);
                copy.Add(metadataKey, metadataValue);

                if (Interlocked.CompareExchange(ref contents, copy, original) == original)
                    return;
            }
        }

        #region MultiMap delegating methods
        /// <inheritdoc />
        public void Add(string key, string value)
        {
            contents.Add(key, value);
        }

        /// <inheritdoc />
        public void AddAll(IMultiMap<string, string> map)
        {
            contents.AddAll(map);
        }

        /// <inheritdoc />
        public bool Contains(string key, string value)
        {
            return contents.Contains(key, value);
        }

        /// <inheritdoc />
        public bool Remove(string key, string value)
        {
            return contents.Remove(key, value);
        }

        /// <inheritdoc />
        public bool ContainsKey(string key)
        {
            return contents.ContainsKey(key);
        }

        /// <inheritdoc />
        public void Add(string key, IList<string> value)
        {
            contents.Add(key, value);
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
            set { contents[key] = value; }
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
        public void Add(KeyValuePair<string, IList<string>> item)
        {
            contents.Add(item);
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

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, IList<string>>> GetEnumerator()
        {
            return contents.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return contents.GetEnumerator();
        }
        #endregion

        #region Xml Serialization
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(@"metadata");

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

            if (reader.NodeType == XmlNodeType.EndElement)
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
                        throw new InvalidOperationException("The metadata map contains an invalid null value.");

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