// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Collections;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// Abstract base class for data items.
    /// </summary>
    public abstract class BaseDataItem : IDataItem
    {
        /// <inheritdoc />
        public abstract bool IsDynamic { get; }

        /// <inheritdoc />
        public void PopulateMetadata(PropertyBag map)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            PopulateMetadataImpl(map);
        }

        /// <inheritdoc />
        public object GetValue(DataBinding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            return GetValueImpl(binding);
        }

        /// <inheritdoc />
        public abstract IEnumerable<DataBinding> GetBindingsForInformalDescription();

        /// <summary>
        /// Implements <see cref="GetValue" />.
        /// </summary>
        /// <param name="binding">The binding, never null.</param>
        /// <returns>The associated value.</returns>
        protected abstract object GetValueImpl(DataBinding binding);

        /// <summary>
        /// Implements <see cref="PopulateMetadata" />.
        /// </summary>
        /// <param name="map">The metadata map to populate, never null.</param>
        protected abstract void PopulateMetadataImpl(PropertyBag map);
    }
}
