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

using System.Collections.Generic;
using Gallio.Collections;

namespace Gallio.Data
{
    /// <summary>
    /// <para>
    /// The intersection merge strategy combines the rows from multiple providers
    /// by discarding rows whose values do not appear in the rows of all other
    /// providers.  If the same row appears more than once within any given provider,
    /// when it will be enumerated only as often as the least number of repetitions
    /// of that row that appear in other providers.
    /// </para>
    /// <para>
    /// Uniqueness is determined by the natural equality of each bound value in the row.
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
        public IEnumerable<IDataRow> Merge(IList<IDataProvider> providers, ICollection<DataBinding> bindings)
        {
            int providerCount = providers.Count;

            if (providerCount == 0)
                return EmptyArray<IDataRow>.Instance;

            if (providerCount == 1)
                return providers[0].GetRows(bindings);

            Dictionary<object[], int> tally = null;
            for (int i = 0; i < providerCount - 1; i++)
                tally = UpdateTally(tally, providers[i], bindings);

            return GetRowsAccordingToTally(tally, providers[providerCount - 1], bindings);
        }

        private Dictionary<object[], int> UpdateTally(Dictionary<object[], int> oldTally, IDataProvider provider, ICollection<DataBinding> bindings)
        {
            Dictionary<object[], int> tally = new Dictionary<object[], int>(ArrayEqualityComparer<object>.Default);
            int maxCount = int.MaxValue;

            foreach (IDataRow row in provider.GetRows(bindings))
            {
                object[] values = GetValues(row, bindings);

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

        private static IEnumerable<IDataRow> GetRowsAccordingToTally(Dictionary<object[], int> tally, IDataProvider provider, ICollection<DataBinding> bindings)
        {
            foreach (IDataRow row in provider.GetRows(bindings))
            {
                object[] values = GetValues(row, bindings);

                int count;
                if (values != null && tally.TryGetValue(values, out count))
                {
                    yield return row;

                    count -= 1;
                    if (count == 0)
                        tally.Remove(values);
                    else
                        tally[values] = count;
                }
            }
        }

        private static object[] GetValues(IDataRow row, ICollection<DataBinding> bindings)
        {
            try
            {
                return GenericUtils.ConvertAllToArray<DataBinding, object>(bindings, delegate(DataBinding binding)
                {
                    return row.GetValue(binding);
                });
            }
            catch
            {
                return null;
            }
        }
    }
}
