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

namespace Gallio.Common.Collections
{
    /// <summary>
    /// Provides a read-only view of another multi-map.
    /// </summary>
    [Serializable]
    internal class ReadOnlyMultiMap<TKey, TValue> : IMultiMap<TKey, TValue>
    {
        private readonly IMultiMap<TKey, TValue> map;

        /// <summary>
        /// Creates a read-only view of the specified multi-map.
        /// </summary>
        /// <param name="map">The multi-map.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="map"/> is null.</exception>
        public ReadOnlyMultiMap(IMultiMap<TKey, TValue> map)
        {
            if (map == null)
                throw new ArgumentNullException(@"map");

            this.map = map;
        }

        public int Count
        {
            get { return map.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ICollection<TKey> Keys
        {
            get { return map.Keys; }
        }

        public ICollection<IList<TValue>> Values
        {
            get { return map.Values; }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Pairs
        {
            get { return map.Pairs; }
        }

        public IList<TValue> this[TKey key]
        {
            get { return map[key]; }
        }
        IList<TValue> IDictionary<TKey, IList<TValue>>.this[TKey key]
        {
            get { return map[key]; }
            set { ThrowReadOnlyException(); }
        }

        public bool Contains(TKey key, TValue value)
        {
            return map.Contains(key, value);
        }

        public bool Contains(KeyValuePair<TKey, IList<TValue>> item)
        {
            return map.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return map.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TKey, IList<TValue>> entry in this)
                array[arrayIndex++] = entry;
        }

        public bool TryGetValue(TKey key, out IList<TValue> value)
        {
            return map.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return map.GetEnumerator();
        }

        #region Unsupported operations
        void ICollection<KeyValuePair<TKey, IList<TValue>>>.Add(KeyValuePair<TKey, IList<TValue>> item)
        {
            ThrowReadOnlyException();
        }

        void IMultiMap<TKey, TValue>.Add(TKey key, TValue value)
        {
            ThrowReadOnlyException();
        }

        void IDictionary<TKey, IList<TValue>>.Add(TKey key, IList<TValue> value)
        {
            ThrowReadOnlyException();
        }

        void IMultiMap<TKey, TValue>.AddAll(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            ThrowReadOnlyException();
        }

        void IMultiMap<TKey, TValue>.AddAll(IEnumerable<KeyValuePair<TKey, IList<TValue>>> map)
        {
            ThrowReadOnlyException();
        }

        void ICollection<KeyValuePair<TKey, IList<TValue>>>.Clear()
        {
            ThrowReadOnlyException();
        }

        bool IDictionary<TKey, IList<TValue>>.Remove(TKey key)
        {
            ThrowReadOnlyException();
            return false;
        }

        bool IMultiMap<TKey, TValue>.Remove(TKey key, TValue value)
        {
            ThrowReadOnlyException();
            return false;
        }

        bool ICollection<KeyValuePair<TKey, IList<TValue>>>.Remove(KeyValuePair<TKey, IList<TValue>> item)
        {
            ThrowReadOnlyException();
            return false;
        }
        #endregion

        private static void ThrowReadOnlyException()
        {
            throw new NotSupportedException("The collection is read-only.");
        }
    }
}
