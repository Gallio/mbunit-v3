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

using System.Collections.Generic;
using Gallio.Collections;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// The union merge strategy combines the rows from multiple providers by
    /// discarding all rows whose values duplicate those of other rows that
    /// have already been enumerated.
    /// </para>
    /// <para>
    /// Uniqueness is determined by the natural equality of each bound value in the row.
    /// </para>
    /// </summary>
    public sealed class UnionMergeStrategy : IMergeStrategy
    {
        /// <summary>
        /// Gets the singleton instance of the strategy.
        /// </summary>
        public static readonly UnionMergeStrategy Instance = new UnionMergeStrategy();

        private UnionMergeStrategy()
        {
        }

        /// <inheritdoc />
        public IEnumerable<IDataRow> Merge(IList<IDataProvider> providers, ICollection<DataBinding> bindings,
            bool includeDynamicRows)
        {
            HashSet<object[]> previousValues = new HashSet<object[]>(new ArrayEqualityComparer<object>());

            foreach (IDataProvider provider in providers)
            {
                foreach (IDataRow row in provider.GetRows(bindings, includeDynamicRows))
                {
                    try
                    {
                        object[] values = GenericUtils.ConvertAllToArray<DataBinding, object>(bindings, delegate(DataBinding binding)
                        {
                            return row.GetValue(binding);
                        });

                        if (previousValues.Contains(values))
                            continue;

                        previousValues.Add(values);
                    }
                    catch
                    {
                        // Always consider rows whose bindings cannot be evaluated correctly as distinct.
                    }

                    yield return row;
                }
            }
        }
    }
}
