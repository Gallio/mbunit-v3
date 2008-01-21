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
using System.Collections;
using System.Collections.Generic;

namespace Gallio.Data
{
    /// <summary>
    /// A data set constructed from an enumerated sequence of column values.
    /// </summary>
    public sealed class ColumnSequenceDataSet : IDataSet
    {
        private readonly IEnumerable values;
        private readonly IEnumerable<KeyValuePair<string, string>> metadata;
        private readonly bool isDynamic;

        /// <summary>
        /// Creates a column sequence data set with optional metadata.
        /// </summary>
        /// <param name="values">The sequence of column values, each generating a row</param>
        /// <param name="metadata">The metadata enumeration, or null if none</param>
        /// <param name="isDynamic">True if the sequence is dynamic</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is null</exception>
        public ColumnSequenceDataSet(IEnumerable values, IEnumerable<KeyValuePair<string, string>> metadata, bool isDynamic)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            this.values = values;
            this.metadata = metadata;
            this.isDynamic = isDynamic;
        }

        /// <inheritdoc />
        public bool IsDynamic
        {
            get { return isDynamic; }
        }

        /// <inheritdoc />
        public int ColumnCount
        {
            get { return 1; }
        }

        /// <inheritdoc />
        public bool CanBind(DataBinding binding)
        {
            return binding.Index.GetValueOrDefault(-1) == 0;
        }

        /// <inheritdoc />
        public IEnumerable<IDataRow> GetRows(ICollection<DataBinding> bindings)
        {
            foreach (object value in values)
                yield return new ScalarDataRow<object>(value, metadata);
        }
    }
}
