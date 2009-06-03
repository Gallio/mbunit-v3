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
using System.Collections.ObjectModel;
using System.Threading;

namespace Gallio.Common.Collections
{
    /// <summary>
    /// A multi-map allows a list of values to be associated with a single key.
    /// </summary>
    [Serializable]
    public class MultiMap<TKey, TValue> : IMultiMap<TKey, TValue>
    {
        private readonly IDictionary<TKey, IList<TValue>> entries;

        /// <summary>
        /// Creates an empty multi-map.
        /// </summary>
        public MultiMap()
        {
            entries = new Dictionary<TKey, IList<TValue>>();
        }

        /// <summary>
        /// Gets the number of distinct keys in the map.
        /// </summary>
        public int Count
        {
            get { return entries.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the collection of keys in the multi-map.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return entries.Keys; }
        }

        /// <summary>
        /// Gets the collection of lists of values in the multi-map.
        /// </summary>
        public ICollection<IList<TValue>> Values
        {
            get { return entries.Values; }
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<TKey, TValue>> Pairs
        {
            get
            {
                foreach (KeyValuePair<TKey, IList<TValue>> entry in entries)
                    foreach (TValue value in entry.Value)
                        yield return new KeyValuePair<TKey, TValue>(entry.Key, value);
            }
        }

        /// <summary>
        /// Gets or sets the list of values associated with the specified key.
        /// Returns an empty list if there are none.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The list of values.</returns>
        public IList<TValue> this[TKey key]
        {
            get
            {
                IList<TValue> list;
                if (entries.TryGetValue(key, out list))
                    return list;

                return EmptyArray<TValue>.Instance;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                if (value.Count != 0)
                    entries[key] = new ValueCollection(value);
                else
                    entries.Remove(key);
            }
        }

        /// <summary>
        /// Adds a value to the list of those associated with a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to associate.</param>
        public void Add(TKey key, TValue value)
        {
            IList<TValue> list;
            ValueCollection valueCollection;
            if (!entries.TryGetValue(key, out list))
            {
                valueCollection = new ValueCollection();
                entries.Add(key, valueCollection);
            }
            else
            {
                valueCollection = (ValueCollection)list;
            }

            valueCollection.MutableList.Add(value);
        }

        /// <summary>
        /// Adds all values in the pair to the specified key.
        /// </summary>
        /// <param name="item">The key and values pair.</param>
        public void Add(KeyValuePair<TKey, IList<TValue>> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds all values in the pair to the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        public void Add(TKey key, IList<TValue> values)
        {
            foreach (TValue value in values)
                Add(key, value);
        }

        /// <inheritdoc />
        public void AddAll(IEnumerable<KeyValuePair<TKey, IList<TValue>>> map)
        {
            if (map == null)
                throw new ArgumentNullException(@"map");

            foreach (KeyValuePair<TKey, IList<TValue>> entry in map)
                Add(entry);
        }

        /// <inheritdoc />
        public void AddAll(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            if (pairs == null)
                throw new ArgumentNullException("pairs");

            foreach (KeyValuePair<TKey, TValue> entry in pairs)
                Add(entry.Key, entry.Value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            entries.Clear();
        }

        /// <summary>
        /// Returns true if the map contains an entry with the specified key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>True if the map contains an entry with the specified key and value.</returns>
        public bool Contains(TKey key, TValue value)
        {
            return this[key].Contains(value);
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, IList<TValue>> item)
        {
            IList<TValue> values = this[item.Key];

            foreach (TValue value in item.Value)
            {
                if (!values.Contains(value))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the map contains at least one value associated with
        /// the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if there is at least one value associated with the key.</returns>
        public bool ContainsKey(TKey key)
        {
            return this[key].Count != 0;
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex)
        {
            entries.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes all values associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the key existed and was removed.</returns>
        public bool Remove(TKey key)
        {
            return entries.Remove(key);
        }

        /// <summary>
        /// Removes a value from the list of those associated with a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to remove from the key.</param>
        /// <returns>True if the value was removed.</returns>
        public bool Remove(TKey key, TValue value)
        {
            IList<TValue> list;
            if (!entries.TryGetValue(key, out list))
                return false;

            ValueCollection valueCollection = (ValueCollection)list;
            if (!valueCollection.MutableList.Remove(value))
                return false;

            if (valueCollection.Count == 0)
                entries.Remove(key);

            return true;
        }

        /// <summary>
        /// Removes all values in the pair from the specified key.
        /// </summary>
        /// <param name="item">The key and values pair.</param>
        /// <returns>True if at least one value was removed.</returns>
        public bool Remove(KeyValuePair<TKey, IList<TValue>> item)
        {
            bool removedOne = false;
            foreach (TValue value in item.Value)
                if (Remove(item.Key, value))
                    removedOne = true;

            return removedOne;
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out IList<TValue> value)
        {
            return entries.TryGetValue(key, out value);
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        /// <summary>
        /// Obtains a read-only view of another multi-map.
        /// </summary>
        /// <param name="map">The multi-map.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="map"/> is null.</exception>
        public static IMultiMap<TKey, TValue> ReadOnly(IMultiMap<TKey, TValue> map)
        {
            return new ReadOnlyMultiMap<TKey, TValue>(map);
        }

        [Serializable]
        private sealed class ValueCollection : ReadOnlyCollection<TValue>
        {
            public ValueCollection()
                : base(new List<TValue>())
            {
            }

            public ValueCollection(IEnumerable<TValue> originalValues)
                : base(new List<TValue>(originalValues))
            {
            }

            public IList<TValue> MutableList
            {
                get { return Items; }
            }
        }
    }
}
