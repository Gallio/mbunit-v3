// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// A data set constructed from a sequence of rows.
    /// </summary>
    public sealed class RowSequenceDataSet : BaseDataSet
    {
        private readonly IEnumerable<IDataRow> rows;
        private readonly int columnCount;

        /// <summary>
        /// Creates a row data set.
        /// </summary>
        /// <param name="rows">The sequence of rows</param>
        /// <param name="columnCount">The column count</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rows"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="columnCount"/> is negative</exception>
        public RowSequenceDataSet(IEnumerable<IDataRow> rows, int columnCount)
        {
            if (rows == null)
                throw new ArgumentNullException("rows");
            if (columnCount < 0)
                throw new ArgumentOutOfRangeException("columnCount", columnCount, "Column count must not be negative.");

            this.rows = rows;
            this.columnCount = columnCount;
        }

        /// <inheritdoc />
        public override int ColumnCount
        {
            get { return columnCount; }
        }

        /// <inheritdoc />
        protected override bool CanBindInternal(DataBinding binding)
        {
            int bindingIndex = binding.Index.GetValueOrDefault(int.MaxValue);
            return bindingIndex >= 0 && bindingIndex < columnCount;
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataRow> GetRowsInternal(ICollection<DataBinding> bindings, bool includeDynamicRows)
        {
            if (includeDynamicRows)
                return rows;

            return GetStaticRows();
        }

        private IEnumerable<IDataRow> GetStaticRows()
        {
            foreach (IDataRow row in rows)
                if (!row.IsDynamic)
                    yield return row;
        }
    }
}