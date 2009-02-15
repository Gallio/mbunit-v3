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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// A scalar data item represents a single static data value combined with
    /// optional metadata for the item.  Data binding occurs whenever the binding
    /// index is 0.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public sealed class ScalarDataItem<T> : SimpleDataItem
    {
        private readonly T value;

        /// <summary>
        /// Creates a scalar data item with optional metadata.
        /// </summary>
        /// <param name="value">The value to hold</param>
        /// <param name="metadataPairs">The metadata key/value pairs, or null if none</param>
        /// <param name="isDynamic">True if the item contains dynamic data</param>
        public ScalarDataItem(T value, IEnumerable<KeyValuePair<string, string>> metadataPairs, bool isDynamic)
            : base(metadataPairs, isDynamic)
        {
            this.value = value;
        }

        /// <inheritdoc />
        public override IEnumerable<DataBinding> GetBindingsForInformalDescription()
        {
            yield return new DataBinding(0, null);
        }

        /// <inheritdoc />
        protected override object GetValueImpl(DataBinding binding)
        {
            if (binding.Index.GetValueOrDefault(-1) == 0)
                return value;

            throw new DataBindingException("Binding index not available or out of range.", binding);
        }
    }
}