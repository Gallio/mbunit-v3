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
using Gallio.Collections;

namespace Gallio.Data
{
    /// <summary>
    /// An data source object provides a simple way of aggregating data sets
    /// together.  It also provides a simple translation mechanism for mapping
    /// binding paths to binding indexes which is useful for providing named
    /// aliases for columns in indexed data sets.
    /// </summary>
    public class DataSource : IDataSet
    {
        private readonly string name;
        private List<IDataSet> dataSets;
        private Dictionary<string, int> indexAliases;
        private int columnCount;

        /// <summary>
        /// Creates a data source with a given name.
        /// </summary>
        /// <param name="name">The name of the data source, or an empty string if it is anonymous</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public DataSource(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets the name of the data source, or an empty string if it is anonymous.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public bool IsDynamic
        {
            get
            {
                return dataSets.Exists(delegate(IDataSet dataSet)
                {
                    return dataSet.IsDynamic;
                });
            }
        }

        /// <inheritdoc />
        public int ColumnCount
        {
            get { return columnCount; }
        }

        /// <inheritdoc />
        public bool CanBind(DataBinding binding)
        {
            binding = TranslateBinding(binding);

            return dataSets.TrueForAll(delegate(IDataSet dataSet)
            {
                return dataSet.CanBind(binding);
            });
        }

        /// <summary>
        /// Adds a data set to the data source.
        /// </summary>
        /// <param name="dataSet">The data set to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dataSet"/> is null</exception>
        public void AddDataSet(IDataSet dataSet)
        {
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");

            if (dataSets == null)
                dataSets = new List<IDataSet>();

            dataSets.Add(dataSet);
            columnCount = Math.Max(columnCount, dataSet.ColumnCount);
        }

        /// <summary>
        /// Adds an alias for a binding path to map it to the specified index.
        /// </summary>
        /// <param name="path">The binding path to match in a case-insensitive manner</param>
        /// <param name="index">The associated index to use instead</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        public void AddIndexAlias(string path, int index)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (indexAliases == null)
                indexAliases = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

            indexAliases.Add(path, index);
        }


        /// <inheritdoc />
        public IEnumerable<IDataRow> GetRows(ICollection<DataBinding> bindings)
        {
            if (bindings == null)
                throw new ArgumentNullException("bindings");

            if (dataSets != null)
            {
                if (indexAliases != null)
                    bindings = GenericUtils.ConvertAllToArray<DataBinding, DataBinding>(bindings, TranslateBinding);

                foreach (IDataSet dataSet in dataSets)
                {
                    foreach (IDataRow row in dataSet.GetRows(bindings))
                    {
                        yield return indexAliases == null ? row : new TranslatedDataRow(this, row);
                    }
                }
            }
        }

        private DataBinding TranslateBinding(DataBinding binding)
        {
            string path = binding.Path;

            int index;
            if (path != null && indexAliases.TryGetValue(path, out index))
                return binding.ReplaceIndex(index);

            return binding;
        }

        private sealed class TranslatedDataRow : IDataRow
        {
            private readonly DataSource source;
            private readonly IDataRow row;

            public TranslatedDataRow(DataSource source, IDataRow row)
            {
                this.source = source;
                this.row = row;
            }

            public IEnumerable<KeyValuePair<string, string>> GetMetadata()
            {
                return row.GetMetadata();
            }

            public object GetRawValue(DataBinding binding)
            {
                return row.GetRawValue(source.TranslateBinding(binding));
            }

            /// <inheritdoc />
            public void Dispose()
            {
                row.Dispose();
            }
        }
    }
}
