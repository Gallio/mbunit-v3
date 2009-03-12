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

using System.Collections.Generic;
using Gallio.Collections;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// A null data item simply returns a null value on each request.
    /// It has no metadata.
    /// </summary>
    public sealed class NullDataItem : BaseDataItem
    {
        /// <summary>
        /// Gets the singleton null data item instance.
        /// </summary>
        public static readonly NullDataItem Instance = new NullDataItem();

        private NullDataItem()
        {
        }

        /// <inheritdoc />
        public override IEnumerable<DataBinding> GetBindingsForInformalDescription()
        {
            return EmptyArray<DataBinding>.Instance;
        }

        /// <inheritdoc />
        protected override object GetValueImpl(DataBinding binding)
        {
            return null;
        }

        /// <inheritdoc />
        protected override void PopulateMetadataImpl(PropertyBag map)
        {
        }

        /// <inheritdoc />
        public override bool IsDynamic
        {
            get { return false; }
        }
    }
}
