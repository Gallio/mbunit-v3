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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// A list data item contains a list of static data values combined with
    /// optional metadata for the item.  Data binding occurs based on the binding
    /// index alone.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public sealed class ListDataItem<T> : SimpleDataItem
    {
        private readonly IList<T> values;

        /// <summary>
        /// Creates a list data item with optional metadata.
        /// </summary>
        /// <param name="values">The list of values</param>
        /// <param name="metadataPairs">The metadata key/value pairs, or null if none</param>
        /// <param name="isDynamic">True if the item contains dynamic data</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is null</exception>
        public ListDataItem(IList<T> values, IEnumerable<KeyValuePair<string, string>> metadataPairs, bool isDynamic)
            : base(metadataPairs, isDynamic)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            this.values = values;
        }

        /// <inheritdoc />
        protected override object GetValueImpl(DataBinding binding)
        {
            int? index = binding.Index;
            if (index.HasValue && index.Value >= 0 && index.Value < values.Count)
                return values[index.Value];

            throw new DataBindingException("Binding index not available or out of range.");
        }
    }
}
