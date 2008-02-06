// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Collections;

namespace Gallio.Data
{
    /// <summary>
    /// Base class for data rows with static metadata.
    /// </summary>
    public abstract class BaseDataRow : IDataRow
    {
        private readonly IEnumerable<KeyValuePair<string, string>> metadata;

        /// <summary>
        /// Creates a data row with optional metadata.
        /// </summary>
        /// <param name="metadata">The metadata enumeration, or null if none</param>
        public BaseDataRow(IEnumerable<KeyValuePair<string, string>> metadata)
        {
            this.metadata = metadata ?? EmptyArray<KeyValuePair<string, string>>.Instance;
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            return metadata;
        }

        /// <inheritdoc />
        public object GetValue(DataBinding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            return GetValueInternal(binding);
        }

        /// <summary>
        /// Implements <see cref="GetValue" />.
        /// </summary>
        /// <param name="binding">The binding, never null</param>
        /// <returns>The associated value</returns>
        protected abstract object GetValueInternal(DataBinding binding);
    }
}
