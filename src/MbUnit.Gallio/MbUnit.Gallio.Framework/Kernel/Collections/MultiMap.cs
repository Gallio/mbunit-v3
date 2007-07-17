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
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Collections
{
    /// <summary>
    /// A multi-map allows a list of values to be associated with a single key.
    /// </summary>
    public class MultiMap<TKey, TValue> : ICollection<KeyValuePair<TKey, IList<TValue>>>
    {
        private IDictionary<TKey, IList<TValue>> map;

        /// <summary>
        /// Creates an empty multi-map.
        /// </summary>
        public MultiMap()
        {
            map = new Dictionary<TKey, IList<TValue>>();
        }

        /// <summary>
        /// Gets the number of distinct keys in the map.
        /// </summary>
        public int Count
        {
            get { return map.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the list of values associated with the specified key.
        /// Returns an empty list if there are none.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The list of values</returns>
        public IList<TValue> this[TKey key]
        {
            get
            {
                IList<TValue> list;
                if (map.TryGetValue(key, out list))
                    return list;

                return EmptyArray<TValue>.Instance;
            }
        }

        /// <summary>
        /// Adds a value to the list of those associated with a key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value to associate</param>
        public void Add(TKey key, TValue value)
        {
            IList<TValue> list;
            if (! map.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                map.Add(key, list);
            }

            list.Add(value);
        }

        /// <summary>
        /// Adds all values in the pair to the specified key.
        /// </summary>
        /// <param name="item">The key and values pair</param>
        public void Add(KeyValuePair<TKey, IList<TValue>> item)
        {
            foreach (TValue value in item.Value)
                Add(item.Key, value);
        }

        /// <summary>
        /// Removes a value from the list of those associated with a key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value to remove from the key</param>
        /// <returns>True if the value was removed</returns>
        public bool Remove(TKey key, TValue value)
        {
            IList<TValue> list;
            return map.TryGetValue(key, out list) && list.Remove(value);
        }

        /// <summary>
        /// Removes all values in the pair from the specified key.
        /// </summary>
        /// <param name="item">The key and values pair</param>
        /// <returns>True if at least one value was removed</returns>
        public bool Remove(KeyValuePair<TKey, IList<TValue>> item)
        {
            bool removedOne = false;
            foreach (TValue value in item.Value)
                if (Remove(item.Key, value))
                    removedOne = true;

            return removedOne;
        }

        /// <summary>
        /// Removes all values associated with the specified key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>True if at least one value was removed</returns>
        public bool RemoveKey(TKey key)
        {
            return map.Remove(key);
        }

        /// <inheritdoc />
        public void Clear()
        {
            map.Clear();
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, IList<TValue>> item)
        {
            throw new NotImplementedException("YAGNI");
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex)
        {
            throw new NotImplementedException("YAGNI");
        }
    }
}
