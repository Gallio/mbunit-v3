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
using System.Collections.Generic;

namespace Gallio.Common.Collections
{
    /// <summary>
    /// A multi-map allows a list of values to be associated with a single key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The value collections provided by the multi-map are always read-only.
    /// They can only be modified by calling the appropriate methods of the multi-map
    /// to add or remove items.  This behavior helps multi-map implementations
    /// better maintain their invariants.
    /// </para>
    /// </remarks>
    public interface IMultiMap<TKey, TValue> : IDictionary<TKey, IList<TValue>>
    {
        /// <summary>
        /// Gets the contents of the multi-map as an enumeration of pairs of keys and lists of values.
        /// </summary>
        IEnumerable<KeyValuePair<TKey, TValue>> Pairs { get; }

        /// <summary>
        /// Adds a value to the list of those associated with a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to associate.</param>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Adds all of the values from the specified map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="map"/> is null.</exception>
        void AddAll(IEnumerable<KeyValuePair<TKey, IList<TValue>>> map);

        /// <summary>
        /// Adds all of the values from the specified enumeration of key-value pairs.
        /// </summary>
        /// <param name="pairs">The key-value pairs.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pairs"/> is null.</exception>
        void AddAll(IEnumerable<KeyValuePair<TKey, TValue>> pairs);

        /// <summary>
        /// Returns true if the map contains an entry with the specified key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>True if the map contains an entry with the specified key and value.</returns>
        bool Contains(TKey key, TValue value);

        /// <summary>
        /// Removes a value from the list of those associated with a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to remove from the key.</param>
        /// <returns>True if the value was removed.</returns>
        bool Remove(TKey key, TValue value);
    }
}