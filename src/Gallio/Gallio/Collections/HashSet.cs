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
    /// A hashtable-based set implementation.
    /// </summary>
    /// <remarks author="jeff">
    /// This will probably be replaced by the new HashSet{T} class in
    /// the new System.Core of .Net 3.5.
    /// </remarks>
    public class HashSet<T> : ICollection<T>
    {
        private readonly Dictionary<T, bool> objects;

        /// <summary>
        /// Creates an empty set.
        /// </summary>
        public HashSet()
            : this(null)
        {
        }

        /// <summary>
        /// Creates an empty set using the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer, or null to use the default comparer</param>
        public HashSet(IEqualityComparer<T> comparer)
        {
            objects = new Dictionary<T, bool>(comparer);
        }

        /// <inheritdoc />
        public int Count
        {
            get { return objects.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            objects[item] = false;
        }

        /// <inheritdoc />
        public void Clear()
        {
            objects.Clear();
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return objects.ContainsKey(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            objects.Keys.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            return objects.Remove(item);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return objects.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
