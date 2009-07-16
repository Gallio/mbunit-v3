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
using Gallio.Common.Collections;

namespace Gallio.Model.Isolation
{
    /// <summary>
    /// Provides options that control test isolation.
    /// </summary>
    [Serializable]
    public sealed class TestIsolationOptions
    {
        private readonly PropertySet properties;

        /// <summary>
        /// Creates a default set of options.
        /// </summary>
        public TestIsolationOptions()
        {
            properties = new PropertySet();
        }

        /// <summary>
        /// Gets a read-only collection of configuration properties for the test isolation context.
        /// </summary>
        public PropertySet Properties
        {
            get { return properties.AsReadOnly(); }
        }

        /// <summary>
        /// Clears the collection of properties.
        /// </summary>
        public void ClearProperties()
        {
            properties.Clear();
        }

        /// <summary>
        /// Adds a property key/value pair.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The property value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="key"/> is already in the property set.</exception>
        public void AddProperty(string key, string value)
        {
            properties.Add(key, value); // note: implicitly checks arguments
        }

        /// <summary>
        /// Removes a property key/value pair.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        public void RemoveProperty(string key)
        {
            properties.Remove(key); // note: implicitly checks arguments
        }

        /// <summary>
        /// Creates a copy of the options.
        /// </summary>
        /// <returns>The copy.</returns>
        public TestIsolationOptions Copy()
        {
            TestIsolationOptions copy = new TestIsolationOptions();
            copy.properties.AddAll(properties);

            return copy;
        }
    }
}