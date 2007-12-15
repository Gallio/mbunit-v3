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
using System.Collections.Generic;

namespace Gallio.Model
{
    /// <summary>
    /// A user data collection associates arbitrary key/value pairs
    /// with model objects.
    /// </summary>
    public class UserDataCollection
    {
        private readonly Dictionary<string, object> items = new Dictionary<string, object>();

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

            object value;
            if (items.TryGetValue(key, out value))
                return (T)value;
            else
                return default(T);
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

            if (value == null)
                items.Remove(key);
            else
                items[key] = value;
        }
    }
}
