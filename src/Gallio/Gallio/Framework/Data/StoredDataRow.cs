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
using Gallio.Model;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// An implementation of <see cref="IDataRow" /> that stores the dynamic flag
    /// and metadata.
    /// </summary>
    public abstract class StoredDataRow : BaseDataRow
    {
        private readonly IEnumerable<KeyValuePair<string, string>> metadataPairs;
        private readonly bool isDynamic;

        /// <summary>
        /// Creates a data row.
        /// </summary>
        /// <param name="metadataPairs">The metadata key/value pairs for the row, or null if none</param>
        /// <param name="isDynamic">True if the row is dynamic</param>
        public StoredDataRow(IEnumerable<KeyValuePair<string, string>> metadataPairs, bool isDynamic)
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
        protected override void PopulateMetadataImpl(MetadataMap map)
        {
            if (metadataPairs != null)
                map.AddAll(metadataPairs);
        }
    }
}
