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
    /// A data binder encapsulates an algorithm for binding values from
    /// rows in a data set.  The binder is intended to be the last step of
    /// the data binding process, so its job is to apply any necessary
    /// conversions or other transformations to the data rows to obtain
    /// the final bound values.
    /// </para>
    /// </summary>
    public class DataBinder
    {
        /// <summary>
        /// Creates a data binder.
        /// </summary>
        public DataBinder()
        {
        }

        /// <summary>
        /// Gets a sequence of data items produced by binding to rows in a data set
        /// and converting the resulting values if necessary.
        /// </summary>
        /// <param name="dataSet">The data set</param>
        /// <param name="bindings">The bindings</param>
        /// <returns>The enumeration of data items</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dataSet"/>
        /// or <paramref name="bindings"/> is null</exception>
        public IEnumerable<Item> Bind(IDataSet dataSet, IList<DataBinding> bindings)
        {
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");
            if (bindings == null)
                throw new ArgumentNullException("bindings");

            foreach (IDataRow row in dataSet.GetRows(bindings))
            {
                yield return new Item(this, row, bindings);
            }
        }

        private object Convert(object rawValue, Type desiredType)
        {
            if (!desiredType.IsInstanceOfType(rawValue))
                throw new NotImplementedException("Type conversions not implemented yet.");

            return rawValue;
        }

        /// <summary>
        /// An item contains 
        /// </summary>
        public sealed class Item
        {
            private static readonly object nullPlaceholder = new object();

            private readonly DataBinder binder;
            private readonly IDataRow row;
            private readonly IList<DataBinding> bindings;
            private object[] values;

            internal Item(DataBinder binder, IDataRow row, IList<DataBinding> bindings)
            {
                this.binder = binder;
                this.row = row;
                this.bindings = bindings;
            }

            /// <summary>
            /// Gets the metadata associated with the item.
            /// </summary>
            public IEnumerable<KeyValuePair<string, string>> GetMetadata()
            {
                return row.GetMetadata();
            }

            /// <summary>
            /// Gets the value of binding with the specified index.
            /// </summary>
            /// <remarks>
            /// The value is computed lazily and is memoized for subsequent requests.
            /// </remarks>
            /// <param name="bindingIndex">The binding index</param>
            /// <returns>The value</returns>
            public object GetValue(int bindingIndex)
            {
                object value;
                if (values == null)
                {
                    values = new object[bindings.Count];
                }
                else
                {
                    value = values[bindingIndex];
                    if (value != null)
                        return value == nullPlaceholder ? null : value;
                }

                DataBinding binding = bindings[bindingIndex];
                value = binder.Convert(row.GetValue(binding), binding.ValueType);

                values[bindingIndex] = value;
                return value;
            }
        }
    }
}
