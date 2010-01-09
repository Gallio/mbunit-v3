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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// The concatenation merge strategy combines the items from multiple providers
    /// by simply concatenating the items in the order in which they
    /// appear and proceeding from one provider to the next until all providers
    /// have been fully processed.
    /// </summary>
    public sealed class ConcatenationMergeStrategy : IMergeStrategy
    {
        /// <summary>
        /// Gets the singleton instance of the strategy.
        /// </summary>
        public static readonly ConcatenationMergeStrategy Instance = new ConcatenationMergeStrategy();

        private ConcatenationMergeStrategy()
        {
        }

        /// <inheritdoc />
        public IEnumerable<IDataItem> Merge(IList<IDataProvider> providers, ICollection<DataBinding> bindings,
            bool includeDynamicItems)
        {
            foreach (IDataProvider provider in providers)
                foreach (IDataItem item in provider.GetItems(bindings, includeDynamicItems))
                    yield return item;
        }
    }
}
