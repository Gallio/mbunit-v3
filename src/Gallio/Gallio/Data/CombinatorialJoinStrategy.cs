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

namespace Gallio.Data
{
    /// <summary>
    /// The combinatorial join strategy combines rows by constructing the cartesian
    /// product of the rows of each provider.
    /// </summary>
    /// <seealso cref="PairwiseJoinStrategy"/>
    public sealed class CombinatorialJoinStrategy : IJoinStrategy
    {
        /// <summary>
        /// Gets the singleton instance of the strategy.
        /// </summary>
        public static readonly CombinatorialJoinStrategy Instance = new CombinatorialJoinStrategy();

        private CombinatorialJoinStrategy()
        {
        }

        /// <inheritdoc />
        public IEnumerable<IList<IDataRow>> Join(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider)
        {
            int providerCount = providers.Count;
            if (providerCount == 0)
                yield break;

            IEnumerable<IDataRow>[] sequences = new IEnumerable<IDataRow>[providers.Count];
            for (int i = 0; i < providers.Count; i++)
                sequences[i] = providers[i].GetRows(bindingsPerProvider[i]);

            IEnumerator<IDataRow>[] enumerators = new IEnumerator<IDataRow>[providerCount];
            enumerators[0] = sequences[0].GetEnumerator();

            int enumeratorCount = 0;
            for (; ; )
            {
                IEnumerator<IDataRow> top = sequences[enumeratorCount - 1].GetEnumerator();

                if (top.MoveNext())
                {
                    if (enumeratorCount < providerCount)
                    {
                        enumerators[enumeratorCount] = sequences[enumeratorCount].GetEnumerator();
                        enumeratorCount += 1;
                    }
                    else
                    {
                        IDataRow[] rowList = new IDataRow[providerCount];
                        for (int i = 0; i < providerCount; i++)
                            rowList[i] = enumerators[i].Current;

                        yield return rowList;
                    }
                }
                else
                {
                    enumeratorCount -= 1;
                    enumerators[enumeratorCount] = null;

                    if (enumeratorCount == 0)
                        break;
                }
            }
        }
    }
}
