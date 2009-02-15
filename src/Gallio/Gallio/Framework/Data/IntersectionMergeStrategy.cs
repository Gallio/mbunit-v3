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
using Gallio.Collections;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// The intersection merge strategy combines the items from multiple providers
    /// by discarding items whose values do not appear in the items of all other
    /// providers.  If the same item appears more than once within any given provider,
    /// when it will be enumerated only as often as the least number of repetitions
    /// of that item that appear in other providers.
    /// </para>
    /// <para>
    /// Uniqueness is determined by the natural equality of each bound value in the item.
    /// </para>
    /// </summary>
    public sealed class IntersectionMergeStrategy : IMergeStrategy
    {
        /// <summary>
        /// Gets the singleton instance of the strategy.
        /// </summary>
        public static readonly IntersectionMergeStrategy Instance = new IntersectionMergeStrategy();

        private IntersectionMergeStrategy()
        {
        }

        /// <inheritdoc />
        public IEnumerable<IDataItem> Merge(IList<IDataProvider> providers, ICollection<DataBinding> bindings,
            bool includeDynamicItems)
        {
            int providerCount = providers.Count;

            if (providerCount == 0)
                return EmptyArray<IDataItem>.Instance;

            if (providerCount == 1)
                return providers[0].GetItems(bindings, includeDynamicItems);

            Dictionary<object[], int> tally = null;
            for (int i = 0; i < providerCount - 1; i++)
                tally = UpdateTally(tally, providers[i], bindings, includeDynamicItems);

            return GetItemsAccordingToTally(tally, providers[providerCount - 1], bindings, includeDynamicItems);
        }

        private static Dictionary<object[], int> UpdateTally(Dictionary<object[], int> oldTally, IDataProvider provider, ICollection<DataBinding> bindings,
            bool includeDynamicItems)
        {
            Dictionary<object[], int> tally = new Dictionary<object[], int>(ArrayEqualityComparer<object>.Default);
            int maxCount = int.MaxValue;

            foreach (IDataItem item in provider.GetItems(bindings, includeDynamicItems))
            {
                object[] values = GetValues(item, bindings);

                if (values != null)
                {
                    if (oldTally != null && !oldTally.TryGetValue(values, out maxCount))
                        continue;

                    int count;
                    tally.TryGetValue(values, out count);

                    if (count < maxCount)
                    {
                        count += 1;
                        tally[values] = count;
                    }
                }
            }

            return tally;
        }

        private static IEnumerable<IDataItem> GetItemsAccordingToTally(Dictionary<object[], int> tally, IDataProvider provider, ICollection<DataBinding> bindings,
            bool includeDynamicItems)
        {
            foreach (IDataItem item in provider.GetItems(bindings, includeDynamicItems))
            {
                object[] values = GetValues(item, bindings);

                int count;
                if (values != null && tally.TryGetValue(values, out count))
                {
                    yield return item;

                    count -= 1;
                    if (count == 0)
                        tally.Remove(values);
                    else
                        tally[values] = count;
                }
            }
        }

        private static object[] GetValues(IDataItem item, ICollection<DataBinding> bindings)
        {
            try
            {
                return GenericUtils.ConvertAllToArray<DataBinding, object>(bindings, delegate(DataBinding binding)
                {
                    return item.GetValue(binding);
                });
            }
            catch
            {
                return null;
            }
        }
    }
}
