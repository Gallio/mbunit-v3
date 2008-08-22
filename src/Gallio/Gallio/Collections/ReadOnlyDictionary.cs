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
using System.Collections;
using System.Collections.Generic;

namespace Gallio.Collections
{
    /// <summary>
    /// A read-only dictionary wrapper.
    /// </summary>
    [Serializable]
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> inner;

        /// <summary>
        /// Creates a read-only dictionary wrapper for another dictionary.
        /// </summary>
        /// <param name="inner">The dictionary to wrap</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null</exception>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ThrowReadOnlyException();
        }

        /// <inheritdoc />
        public void Clear()
        {
            ThrowReadOnlyException();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return inner.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ThrowReadOnlyException();
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            ThrowReadOnlyException();
            return false;
        }

        /// <inheritdoc />
        public int Count
        {
            get { return inner.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            return inner.ContainsKey(key);
        }

        /// <inheritdoc />
        public void Add(TKey key, TValue value)
        {
            ThrowReadOnlyException();
        }

        /// <inheritdoc />
        public bool Remove(TKey key)
        {
            ThrowReadOnlyException();
            return false;
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            return inner.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get { return inner[key]; }
            set { ThrowReadOnlyException(); }
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys
        {
            get { return inner.Keys; }
        }

        /// <inheritdoc />
        public ICollection<TValue> Values
        {
            get { return inner.Values; }
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static void ThrowReadOnlyException()
        {
            throw new NotSupportedException("Collection is read only.");
        }
    }
}
