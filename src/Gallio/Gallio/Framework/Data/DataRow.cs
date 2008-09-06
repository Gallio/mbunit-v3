// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// A data row is a simple immutable value type that encpasulates an
    /// array of values along with associated metadata for use in data binding expressions.
    /// It presents a fluent interface that users may find more convenient than
    /// other <see cref="IDataItem" /> implementations.
    /// </para>
    /// <para>
    /// The data in a <see cref="DataRow" /> is always considered dynamic.
    /// </para>
    /// </summary>
    public class DataRow : BaseDataItem
    {
        private readonly object[] values;
        private readonly MetadataNode metadata;

        private DataRow(object[] values, MetadataNode metadata)
        {
            this.values = values;
            this.metadata = metadata;
        }

        /// <summary>
        /// Creates a data row with the specified array of values.
        /// </summary>
        /// <param name="values">The array of values</param>
        public DataRow(params object[] values)
            : this(values ?? new object[] { null }, null)
        {
        }

        /// <summary>
        /// Returns an augmented data row with added metadata.
        /// </summary>
        /// <remarks>
        /// The original data row is not modified!
        /// </remarks>
        /// <param name="key">The metadata key</param>
        /// <param name="value">The metadata value</param>
        /// <returns>The augmented data row</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/>
        /// or <paramref name="value"/> is null</exception>
        public DataRow WithMetadata(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            return new DataRow(values, new MetadataNode(metadata, key, value));
        }

        /// <summary>
        /// Returns an augmented data row with added metadata from the
        /// specified metadata map.
        /// </summary>
        /// <remarks>
        /// The original data row is not modified!
        /// </remarks>
        /// <param name="map">The metadata map</param>
        /// <returns>The augmented data row</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="map"/>
        /// is null</exception>
        public DataRow WithMetadata(MetadataMap map)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            DataRow result = this;
            foreach (KeyValuePair<string, string> pair in map.Pairs)
                result = result.WithMetadata(pair.Key, pair.Value);
            return result;
        }

        /// <inheritdoc />
        public override bool IsDynamic
        {
            get { return true; }
        }

        /// <inheritdoc />
        protected override void PopulateMetadataImpl(MetadataMap map)
        {
            for (MetadataNode node = metadata; node != null; node = node.Predecessor)
                map.Add(node.Key, node.Value);
        }

        /// <inheritdoc />
        public override IEnumerable<DataBinding> GetBindingsForInformalDescription()
        {
            for (int i = 0; i < values.Length; i++)
                yield return new DataBinding(i, null);
        }

        /// <inheritdoc />
        protected override object GetValueImpl(DataBinding binding)
        {
            int index = binding.Index.GetValueOrDefault(-1);
            if (index >= 0 && index < values.Length)
                return values[index];

            throw new DataBindingException("Binding index not available or out of range.", binding);
        }

        private sealed class MetadataNode
        {
            public readonly MetadataNode Predecessor;
            public readonly string Key;
            public readonly string Value;

            public MetadataNode(MetadataNode predecessor, string key, string value)
            {
                Predecessor = predecessor;
                Key = key;
                Value = value;
            }
        }
    }
}
