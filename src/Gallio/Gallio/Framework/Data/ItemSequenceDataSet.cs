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
    /// A data set constructed from a sequence of data items.
    /// </summary>
    public sealed class ItemSequenceDataSet : BaseDataSet
    {
        private readonly IEnumerable<IDataItem> items;
        private readonly int columnCount;

        /// <summary>
        /// Creates a row data set.
        /// </summary>
        /// <param name="items">The sequence of items.</param>
        /// <param name="columnCount">The column count.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="items"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="columnCount"/> is negative.</exception>
        public ItemSequenceDataSet(IEnumerable<IDataItem> items, int columnCount)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (columnCount < 0)
                throw new ArgumentOutOfRangeException("columnCount", columnCount, "Column count must not be negative.");

            this.items = items;
            this.columnCount = columnCount;
        }

        /// <inheritdoc />
        public override int ColumnCount
        {
            get { return columnCount; }
        }

        /// <inheritdoc />
        protected override bool CanBindImpl(DataBinding binding)
        {
            int bindingIndex = binding.Index.GetValueOrDefault(int.MaxValue);
            return bindingIndex >= 0 && bindingIndex < columnCount;
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataItem> GetItemsImpl(ICollection<DataBinding> bindings, bool includeDynamicItems)
        {
            if (includeDynamicItems)
                return items;

            return GetStaticItems();
        }

        private IEnumerable<IDataItem> GetStaticItems()
        {
            foreach (IDataItem item in items)
                if (!item.IsDynamic)
                    yield return item;
        }
    }
}