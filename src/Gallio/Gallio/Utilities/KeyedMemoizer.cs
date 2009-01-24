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

namespace Gallio.Utilities
{
    /// <summary>
    /// A structure that memoizes the result of some computation for later reuse.
    /// Maintains an internal dictionary to memoize results by key.
    /// </summary>
    /// <remarks>
    /// Not thread safe.
    /// </remarks>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    public struct KeyedMemoizer<TKey, TValue>
    {
        // Using a list for storage since we generally expect a relatively small number
        // of values to be memoized.  Moreover we want to be able to handle null
        // keys correctly (more difficult with a dictionary).
        private List<KeyValuePair<TKey, TValue>> items;

        /// <summary>
        /// Gets the memoized value for the given key if available, otherwise populates it
        /// using the specified populator function and stores it in association with its keys
        /// for later reuse.
        /// </summary>
        /// <param name="key">The key by which to look up a memoized result</param>
        /// <param name="populator">The populator for the value associated with the key</param>
        /// <returns>The value returned by the populator, possibly memoized</returns>
        public TValue Memoize(TKey key, Func<TValue> populator)
        {
            if (items != null)
            {
                foreach (KeyValuePair<TKey, TValue> item in items)
                    if (object.Equals(key, item.Key))
                        return item.Value;
            }
            else
            {
                items = new List<KeyValuePair<TKey, TValue>>();
            }

            TValue value = populator();
            items.Add(new KeyValuePair<TKey,TValue>(key, value));
            return value;
        }
    }
}
