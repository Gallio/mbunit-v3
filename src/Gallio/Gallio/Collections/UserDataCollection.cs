// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;

namespace Gallio.Collections
{
    /// <summary>
    /// <para>
    /// A user data collection stores arbitrary key/value pairs that may be associated
    /// with other objects in the system.
    /// </para>
    /// <para>
    /// Accesses to the collection are synchronized during access.  To safely manupulate
    /// multiple keys at one time, simply lock the <see cref="UserDataCollection" />
    /// for the duration of the operation.
    /// </para>
    /// </summary>
    public class UserDataCollection
    {
        private Dictionary<string, object> items;

        /// <summary>
        /// Tries to get a value from the collection.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">Set to the value, or <c>default(T)</c> if none present</param>
        /// <returns>True if a value was obtained</returns>
        /// <typeparam name="T">The value type</typeparam>
        public bool TryGetValue<T>(Key<T> key, out T value)
        {
            lock (this)
            {
                if (items != null)
                {
                    object rawValue;
                    if (items.TryGetValue(key.Name, out rawValue))
                    {
                        value = (T)rawValue;
                        return true;
                    }
                }

                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Gets a value from the collection.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The associated value</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="key"/> was not found.</exception>
        /// <typeparam name="T">The value type</typeparam>
        public T GetValue<T>(Key<T> key)
        {
            T value;
            if (!TryGetValue(key, out value))
                throw new KeyNotFoundException(String.Format("Did not find a value associated with key '{0}'.", key));
            return value;
        }

        /// <summary>
        /// Determines whether a key has an associated value in the collection.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>True if the key has an associated value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        /// <typeparam name="T">The value type</typeparam>
        public bool HasValue<T>(Key<T> key)
        {
            lock (this)
            {
                return items != null && items.ContainsKey(key.Name);
            }
        }

        /// <summary>
        /// Sets a value in the collection.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value to set</param>
        /// <typeparam name="T">The value type</typeparam>
        public void SetValue<T>(Key<T> key, T value)
        {
            lock (this)
            {
                if (items == null)
                    items = new Dictionary<string, object>();
                items[key.Name] = value;
            }
        }

        /// <summary>
        /// Removes a value from the collection.
        /// </summary>
        /// <param name="key">The key</param>
        /// <typeparam name="T">The value type</typeparam>
        public void RemoveValue<T>(Key<T> key)
        {
            lock (this)
            {
                if (items != null)
                    items.Remove(key.Name);
            }
        }
    }
}
