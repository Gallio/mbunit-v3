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
        /// Gets a value from the collection.
        /// </summary>
        /// <typeparam name="T">The type of value to retrieve</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The associated value, or <c>default(T)</c> if none present</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        public T GetValue<T>(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (this)
            {
                if (items != null)
                {
                    object value;
                    if (items.TryGetValue(key, out value))
                        return (T)value;
                }

                return default(T);
            }
        }

        /// <summary>
        /// Sets a value in the collection.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value to set, or null to remove the value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        public void SetValue(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (this)
            {
                if (value == null)
                {
                    if (items != null)
                        items.Remove(key);
                }
                else
                {
                    if (items == null)
                        items = new Dictionary<string, object>();
                    items[key] = value;
                }
            }
        }
    }
}
