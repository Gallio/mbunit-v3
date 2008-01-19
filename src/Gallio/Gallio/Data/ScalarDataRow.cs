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
    /// A scalar data row represents a single static data values combined with
    /// optional metadata for the row.  Data binding occurs whenever the binding
    /// index is 0.
    /// </para>
    /// <para>
    /// The original data value is disposed when the data row is disposed.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public class ScalarDataRow<T> : BaseDataRow
    {
        private readonly T value;

        /// <summary>
        /// Creates a scalar data row with optional metadata.
        /// </summary>
        /// <param name="value">The value to hold</param>
        /// <param name="metadata">The metadata enumeration, or null if none</param>
        public ScalarDataRow(T value, IEnumerable<KeyValuePair<string, string>> metadata)
            : base(metadata)
        {
            this.value = value;
        }

        /// <inheritdoc />
        public override object GetValue(DataBinding binding)
        {
            if (binding.Index.GetValueOrDefault(-1) == 0)
                return value;

            throw new DataBindingException("Binding index not available or out of range.");
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            IDisposable disposable = value as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}