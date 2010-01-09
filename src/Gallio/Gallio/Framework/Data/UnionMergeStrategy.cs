// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Collections;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// The union merge strategy combines the items from multiple providers by
    /// discarding all items whose values duplicate those of other items that
    /// have already been enumerated.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uniqueness is determined by the natural equality of each bound value in the item.
    /// </para>
    /// </remarks>
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
        public IEnumerable<IDataItem> Merge(IList<IDataProvider> providers, ICollection<DataBinding> bindings,
            bool includeDynamicItems)
        {
            var previousValues = new HashSet<object[]>(new ArrayEqualityComparer<object>());

            foreach (IDataProvider provider in providers)
            {
                foreach (IDataItem item in provider.GetItems(bindings, includeDynamicItems))
                {
                    try
                    {
                        object[] values = GenericCollectionUtils.ConvertAllToArray<DataBinding, object>(bindings, delegate(DataBinding binding)
                        {
                            return item.GetValue(binding);
                        });

                        if (previousValues.Contains(values))
                            continue;

                        previousValues.Add(values);
                    }
                    catch
                    {
                        // Always consider items whose bindings cannot be evaluated correctly as distinct.
                    }

                    yield return item;
                }
            }
        }
    }
}
