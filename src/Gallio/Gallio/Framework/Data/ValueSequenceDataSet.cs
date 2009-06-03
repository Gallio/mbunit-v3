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
using System.Collections;
using System.Collections.Generic;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// A data set constructed from an enumerated sequence of values.
    /// </summary>
    public sealed class ValueSequenceDataSet : BaseDataSet
    {
        private readonly IEnumerable values;
        private readonly IEnumerable<KeyValuePair<string, string>> metadataPairs;
        private readonly bool isDynamic;

        /// <summary>
        /// Creates a column sequence data set with optional metadata.
        /// </summary>
        /// <param name="values">The sequence of column values, each generating an item.</param>
        /// <param name="metadataPairs">The metadata key/value pair enumeration, or null if none.</param>
        /// <param name="isDynamic">True if the sequence is dynamic.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is null.</exception>
        public ValueSequenceDataSet(IEnumerable values, IEnumerable<KeyValuePair<string, string>> metadataPairs, bool isDynamic)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            this.values = values;
            this.metadataPairs = metadataPairs;
            this.isDynamic = isDynamic;
        }

        /// <inheritdoc />
        public override int ColumnCount
        {
            get { return 1; }
        }

        /// <inheritdoc />
        protected override bool CanBindImpl(DataBinding binding)
        {
            return binding.Index.GetValueOrDefault(-1) == 0;
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataItem> GetItemsImpl(ICollection<DataBinding> bindings, bool includeDynamicItems)
        {
            if (!isDynamic || includeDynamicItems)
            {
                foreach (object value in values)
                    yield return new ScalarDataItem<object>(value, metadataPairs, isDynamic);
            }
        }
    }
}
