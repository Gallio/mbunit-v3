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

using System.Collections.Generic;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// The sequential join strategy combines items from each data provider sequentially.
    /// If one provider contains fewer items than the others, the sequential join strategy
    /// will insert a <see cref="NullDataItem"/> in its place until all other providers have been exhausted.
    /// This is similar to performing an "outer join" of multiple sources by ordinal index.
    /// </summary>
    public sealed class SequentialJoinStrategy : IJoinStrategy
    {
        /// <summary>
        /// Gets the singleton instance of the strategy.
        /// </summary>
        public static readonly SequentialJoinStrategy Instance = new SequentialJoinStrategy();

        private SequentialJoinStrategy()
        {
        }

        /// <inheritdoc />
        public IEnumerable<IList<IDataItem>> Join(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider,
            bool includeDynamicItems)
        {
            IEnumerator<IDataItem>[] enumerators = new IEnumerator<IDataItem>[providers.Count];
            for (int i = 0; i < providers.Count; i++)
                enumerators[i] = providers[i].GetItems(bindingsPerProvider[i], includeDynamicItems).GetEnumerator();

            int providerCount = providers.Count;
            for (;;)
            {
                IDataItem[] itemList = new IDataItem[providerCount];
                bool gotOne = false;

                for (int i = 0; i < providerCount; i++)
                {
                    if (enumerators[i].MoveNext())
                    {
                        itemList[i] = enumerators[i].Current;
                        gotOne = true;
                    }
                    else
                    {
                        itemList[i] = NullDataItem.Instance;
                    }
                }

                if (! gotOne)
                    break;

                yield return itemList;
            }
        }
    }
}