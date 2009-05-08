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
using System.Collections.Generic;

namespace Gallio.Common.Collections
{
    /// <summary>
    /// Lazily populates and caches values associated with a particular key.
    /// </summary>
    /// <remarks>
    /// Instances of this type are safe for use by multiple concurrent threads.
    /// In the case of a race occurs between two threads that result in the population of the
    /// same key, the populator function may be called concurrently with two requests for the same
    /// key.  The winner of the race gets to store its value in the cache for later use.
    /// However, the loser of the race will discard its value and use the newly cached instead.
    /// </remarks>
    public class LazyCache<TKey, TValue>
    {
        private readonly Func<TKey, TValue> populator;
        private readonly Dictionary<TKey, TValue> contents;

        /// <summary>
        /// Creates a cache with the specified populator function.
        /// </summary>
        /// <param name="populator">A function that provides a value given a key</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="populator"/> is null</exception>
        public LazyCache(Func<TKey, TValue> populator)
        {
            if (populator == null)
                throw new ArgumentNullException("populator");

            this.populator = populator;

            contents = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// Populates it on demand if not already cached.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The associated value</returns>
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                lock (contents)
                {
                    // Check whether the value is already cached.
                    if (contents.TryGetValue(key, out value))
                        return value;
                }

                value = populator(key);
                lock (contents)
                {
                    // Check for a race condition.
                    // If the value was already populated then discard the new value we
                    // just computed redundantly.
                    TValue cachedValue;
                    if (contents.TryGetValue(key, out cachedValue))
                        return cachedValue;

                    // Otherwise cache the new value.
                    contents.Add(key, value);
                }

                return value;
            }
        }
    }
}
