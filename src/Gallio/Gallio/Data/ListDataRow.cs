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

namespace Gallio.Data
{
    /// <summary>
    /// <para>
    /// A list data row contains a list of static data values combined with
    /// optional metadata for the row.  Data binding occurs based on the binding
    /// index alone.
    /// </para>
    /// <para>
    /// The original data values are disposed when the data row is disposed.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public class ListDataRow<T> : BaseDataRow
    {
        private readonly IList<T> values;

        /// <summary>
        /// Creates a list data row with optional metadata.
        /// </summary>
        /// <param name="values">The list of values</param>
        /// <param name="metadata">The metadata enumeration, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is null</exception>
        public ListDataRow(IList<T> values, IEnumerable<KeyValuePair<string, string>> metadata)
            : base(metadata)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            this.values = values;
        }

        /// <inheritdoc />
        public override object GetValue(DataBinding binding)
        {
            int? index = binding.Index;
            if (index.HasValue && index.Value >= 0 && index.Value < values.Count)
                return values[index.Value];

            throw new DataBindingException("Binding index not available or out of range.");
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            foreach (T value in values)
            {
                IDisposable disposable = value as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }
}
