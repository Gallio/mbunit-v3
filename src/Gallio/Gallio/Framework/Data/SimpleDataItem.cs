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
    /// An implementation of <see cref="IDataItem" /> that stores the dynamic flag
    /// and metadata.
    /// </summary>
    public abstract class SimpleDataItem : BaseDataItem
    {
        private readonly IEnumerable<KeyValuePair<string, string>> metadataPairs;
        private readonly bool isDynamic;

        /// <summary>
        /// Creates a data item.
        /// </summary>
        /// <param name="metadataPairs">The metadata key/value pairs for the item, or null if none.</param>
        /// <param name="isDynamic">True if the item is dynamic.</param>
        public SimpleDataItem(IEnumerable<KeyValuePair<string, string>> metadataPairs, bool isDynamic)
        {
            this.metadataPairs = metadataPairs;
            this.isDynamic = isDynamic;
        }

        /// <inheritdoc />
        public override bool IsDynamic
        {
            get { return isDynamic; }
        }

        /// <inheritdoc />
        protected override void PopulateMetadataImpl(PropertyBag map)
        {
            if (metadataPairs != null)
                map.AddAll(metadataPairs);
        }
    }
}
