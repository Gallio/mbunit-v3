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

using System;
using System.Collections.Generic;
using Gallio.Common.Collections;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// The pairwise strategy constructs a limited number of combinations of the
    /// items within the data providers such that they cover all possible pairs
    /// of values from each data source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This strategy can be more efficient than one based on exhaustively testing all
    /// combinations since many test failures result from the interaction of a relatively
    /// small number of factors: often just two of them.
    /// </para>
    /// <para>
    /// Computing orthogonal arrays for pairwise joins for large domains can be very expensive
    /// computationally.  Therefore most practical algorithms use approximations.
    /// </para>
    /// <para>
    /// The algorithm used here makes a list of all pairs that must be covered.  Then it constructs
    /// combinations greedily by trying to include as many uncovered pairs in each one as possible,
    /// preferring pairs used less often over other ones.  The combinations produced may not be
    /// optimal but empirically it covers all pairs using a sufficiently small number of cases for
    /// most practical purposes.
    /// </para>
    /// <para>
    /// Many thanks to James Bach at Satisfice (www.satisfice.com) for his AllPairs program from
    /// which the algorithm used here was adapted.
    /// </para>
    /// <para>
    /// The choice of considering only 2 factors was made for the sake of implementation
    /// convenience.  It turns out that an N-wise combination strategy is much more complex
    /// to implement efficiently because the general problem is NP-Complete.  However, if you
    /// should choose to implement such a join strategy on your own, please consider contributing
    /// it back to the Gallio project for others to use.  Thanks!
    /// </para>
    /// <para>
    /// See also: <a href="http://www.pairwise.org/"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="CombinatorialJoinStrategy"/>
    public sealed class PairwiseJoinStrategy : IJoinStrategy
    {
        /// <summary>
        /// Gets the singleton instance of the strategy.
        /// </summary>
        public static readonly PairwiseJoinStrategy Instance = new PairwiseJoinStrategy();

        private PairwiseJoinStrategy()
        {
        }

        /// <inheritdoc />
        public IEnumerable<IList<IDataItem>> Join(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider,
            bool includeDynamicItems)
        {
            // Handle degenerate cases.
            int providerCount = providers.Count;
            if (providerCount <= 1)
                return SequentialJoinStrategy.Instance.Join(providers, bindingsPerProvider, includeDynamicItems);

            // Get all items from all providers.
            var itemLists = new List<IDataItem>[providerCount];
            int[] counts = new int[providerCount];
            for (int i = 0; i < providerCount; i++)
            {
                var items = new List<IDataItem>(providers[i].GetItems(bindingsPerProvider[i], includeDynamicItems));
                int count = items.Count;
                if (count == 0)
                    return EmptyArray<IList<IDataItem>>.Instance; // there must be at least one value from each provider

                itemLists[i] = items;
                counts[i] = count;
            }

            var generator = new PairwiseGenerator(counts);
            return GenerateCombinations(generator, itemLists);
        }

        private static IEnumerable<IList<IDataItem>> GenerateCombinations(PairwiseGenerator generator, List<IDataItem>[] itemLists)
        {
            int providerCount = itemLists.Length;
            int[] indices = new int[providerCount];
            while (generator.Next(indices))
            {
                IDataItem[] combination = new IDataItem[providerCount];
                for (int i = 0; i < providerCount; i++)
                    combination[i] = itemLists[i][indices[i]];

                yield return combination;
            }
        }

        private sealed class PairwiseGenerator
        {
            private readonly int[] counts;
            private readonly Random tieBreaker;

            private readonly int dimensions;
            private readonly CoveringTable[][] coveringTables;

            public PairwiseGenerator(int[] counts)
            {
                this.counts = counts;

                dimensions = counts.Length;

                coveringTables = new CoveringTable[dimensions][];
                for (int i = 0; i < dimensions; i++)
                {
                    coveringTables[i] = new CoveringTable[dimensions];

                    for (int j = 0; j < i; j++)
                    {
                        int[,] coverings = new int[counts[i], counts[j]];
                        coveringTables[i][j] = new CoveringTable(coverings, false);
                        coveringTables[j][i] = new CoveringTable(coverings, true);
                    }
                }

                tieBreaker = new Random(0); // Note: Uses a constant seed to be deterministic.
            }

            public bool Next(int[] indices)
            {
                for (int i = 0; i < dimensions; i++)
                    indices[i] = -1;

                bool foundUncovered = false;
                for (int firstDimension = 0; firstDimension < dimensions; firstDimension++)
                {
                    if (indices[firstDimension] >= 0)
                        continue;

                    // Find the value in the first dimension that produces the best score over all values in all other dimensions.
                    int firstCount = counts[firstDimension];
                    int bestFirstScore = 0;
                    int bestFirstIndex = 0;
                    int ties = 0;

                    for (int firstIndex = 0; firstIndex < firstCount; firstIndex++)
                    {
                        int firstScore = 0;
                        for (int secondDimension = 0; secondDimension < dimensions; secondDimension++)
                        {
                            if (firstDimension == secondDimension)
                                continue;

                            CoveringTable coverings = coveringTables[firstDimension][secondDimension];

                            if (firstDimension < secondDimension)
                            {
                                int secondCount = counts[secondDimension];
                                for (int secondIndex = 0; secondIndex < secondCount; secondIndex++)
                                {
                                    if (coverings.GetCoveringCount(firstIndex, secondIndex) == 0)
                                    {
                                        firstScore += 1;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (coverings.GetCoveringCount(firstIndex, indices[secondDimension]) == 0)
                                    firstScore += 1;
                            }
                        }

                        if (firstScore < bestFirstScore)
                            continue;

                        if (firstScore == bestFirstScore)
                        {
                            // Randomly choose which of the ties to keep with equal probability.
                            // This helps to reduce the variance between covered pairs.
                            ties += 1;
                            if (tieBreaker.Next(ties) != 0)
                                continue;
                        }
                        else
                        {
                            bestFirstScore = firstScore;
                        }

                        bestFirstIndex = firstIndex;
                    }

                    if (bestFirstScore != 0)
                    {
                        // If the best score is non-zero, then we know there exists an uncovered pair that will be
                        // covered by this choice of first index because we always prefer indexes that produce new coverings.
                        foundUncovered = true;
                        indices[firstDimension] = bestFirstIndex;
                    }
                    else
                    {
                        // If the best score is zero, then it makes no difference which index we choose so we
                        // pick one at random.  This helps to reduce the variance between covered pairs
                        // as otherwise we'd constantly be choosing the 0th element.
                        indices[firstDimension] = tieBreaker.Next(firstCount);
                    }
                }

                // If we did not find any uncovered pairs, then there are none to be found.
                if (!foundUncovered)
                    return false;

                // Mark all pairs that were covered.
                for (int i = 0; i < dimensions; i++)
                    for (int j = i + 1; j < dimensions; j++)
                        coveringTables[i][j].IncrementCoveringCount(indices[i], indices[j]);
                return true;
            }
        }

        private struct CoveringTable
        {
            private readonly int[,] coverings;
            private readonly bool swapped;

            public CoveringTable(int[,] coverings, bool swapped)
            {
                this.coverings = coverings;
                this.swapped = swapped;
            }

            public int GetCoveringCount(int firstIndex, int secondIndex)
            {
                return swapped ? coverings[secondIndex, firstIndex] : coverings[firstIndex, secondIndex];
            }

            public void IncrementCoveringCount(int firstIndex, int secondIndex)
            {
                if (swapped)
                    coverings[secondIndex, firstIndex] += 1;
                else
                    coverings[firstIndex, secondIndex] += 1;
            }
        }
    }
}
