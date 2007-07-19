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
using System.Text;
using MbUnit.Framework.Kernel.Collections;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A metadata map is a multi-valued dictionary of metadata keys and values associated 
    /// with a model element.  Metadata is used to communicate declarative 
    /// properties of the model in an extensible manner.
    /// </summary>
    public class MetadataMap
    {
        private MultiMap<string, object> entries;

        /// <summary>
        /// Creates an empty metadata map.
        /// </summary>
        public MetadataMap()
        {
            entries = new MultiMap<string, object>();
        }

        /// <summary>
        /// Gets the multi-valued dictionary of metadata entries.
        /// </summary>
        public MultiMap<string, object> Entries
        {
            get { return entries; }
        }

        /// <summary>
        /// Gets the value associated with the metadata key.
        /// If there are multiple values, returns only the first one.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value, or null if none</returns>
        public object GetValue(string key)
        {
            IList<object> values = entries[key];
            if (values.Count == 0)
                return null;

            return values[0];
        }

        /// <summary>
        /// Sets the value associated with the metadata key.
        /// Removes all values previously associated with that key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The new value, or null to remove the value</param>
        public void SetValue(string key, object value)
        {
            entries.RemoveKey(key);

            if (value != null)
                entries.Add(key, value);
        }
    }
}
