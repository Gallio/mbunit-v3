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
    /// The pairwise strategy constructs a limited number of combinations of the
    /// rows within the data providers such that they cover all possible pairs
    /// of values from each data source.
    /// </para>
    /// <para>
    /// This strategy can be more efficient than one based on exhaustively testing all
    /// combinations since many test failures result from the interaction of a relatively
    /// small number of factors: often just two of them.
    /// </para>
    /// </summary>
    /// <remarks>
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
    /// The choice of considering only 2 factors was made for the sake of implementation
    /// convenience.  It turns out that an N-wise combination strategy is much more complex
    /// to implement efficiently because the general problem is NP-Complete.  However, if you
    /// should choose to implement such a join strategy on your own, please consider contributing
    /// it back to the Gallio project for others to use.  Thanks!
    /// </para>
    /// <para>
    /// See also: http://www.pairwise.org/.
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
        public IEnumerable<IList<IDataRow>> Join(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider,
            bool includeDynamicRows)
        {
            // Handle degenerate cases.
            int providerCount = providers.Count;
            if (providerCount <= 1)
                return SequentialJoinStrategy.Instance.Join(providers, bindingsPerProvider, includeDynamicRows);

            // Get all rows from all providers.
            List<IDataRow>[] rowLists = new List<IDataRow>[providerCount];
            int[] counts = new int[providerCount];
            for (int i = 0; i < providerCount; i++)
            {
                List<IDataRow> rows = new List<IDataRow>(providers[i].GetRows(bindingsPerProvider[i], includeDynamicRows));
                int count = rows.Count;
                if (count == 0)
                    return EmptyArray<IList<IDataRow>>.Instance; // there must be at least one value from each provider

                rowLists[i] = rows;
                counts[i] = count;
            }

            PairwiseGenerator generator = new PairwiseGenerator(counts);
            return GenerateCombinations(generator, rowLists);
        }

        private static IEnumerable<IList<IDataRow>> GenerateCombinations(PairwiseGenerator generator, List<IDataRow>[] rowLists)
        {
            int providerCount = rowLists.Length;
            int[] indices = new int[providerCount];
            while (generator.Next(indices))
            {
                IDataRow[] combination = new IDataRow[providerCount];
                for (int i = 0; i < providerCount; i++)
                    combination[i] = rowLists[i][indices[i]];

                yield return combination;
            }
        }

        private sealed class PairwiseGenerator
        {
            private readonly int[] counts;

            private readonly int dimensions;
            private readonly ScoreTable[][] dimensionPairScores;

            public PairwiseGenerator(int[] counts)
            {
                this.counts = counts;

                dimensions = counts.Length;

                dimensionPairScores = new ScoreTable[dimensions][];
                for (int i = 0; i < dimensions; i++)
                {
                    dimensionPairScores[i] = new ScoreTable[dimensions];

                    for (int j = 0; j < i; j++)
                    {
                        int[,] scores = new int[counts[i], counts[j]];
                        dimensionPairScores[i][j] = new ScoreTable(scores, false);
                        dimensionPairScores[j][i] = new ScoreTable(scores, true);
                    }
                }
            }

            public bool Next(int[] indices)
            {
                for (int i = 0; i < dimensions; i++)
                    indices[i] = -1;

                bool foundUncovered = false;
                int bestFirstIndex = 0;
                int bestSecondIndex = 0;
                int bestSecondDimension = 0;

                for (bool fill = false; ; fill = true)
                {
                    for (int firstDimension = 0; firstDimension < dimensions; firstDimension++)
                    {
                        if (indices[firstDimension] >= 0)
                            continue;

                        // Search for a second dimension that forms a pair that has been visited least.
                        int firstCount = counts[firstDimension];
                        int bestScore = int.MaxValue;
                        for (int secondDimension = 0; secondDimension < dimensions; secondDimension++)
                        {
                            if (firstDimension == secondDimension)
                                continue;

                            ScoreTable scores = dimensionPairScores[firstDimension][secondDimension];

                            int secondIndex = indices[secondDimension];
                            if (secondIndex < 0)
                            {
                                // Handle the case where neither the first nor second index has been chosen.
                                int secondCount = counts[secondDimension];
                                for (int firstIndex = 0; firstIndex < firstCount; firstIndex++)
                                {
                                    for (secondIndex = 0; secondIndex < secondCount; secondIndex++)
                                    {
                                        int score = scores.GetScore(firstIndex, secondIndex);
                                        if (score < bestScore)
                                        {
                                            bestScore = score;
                                            bestFirstIndex = firstIndex;
                                            bestSecondIndex = secondIndex;
                                            bestSecondDimension = secondDimension;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Handle the case where the second index has already been chosen but not the first.
                                for (int firstIndex = 0; firstIndex < firstCount; firstIndex++)
                                {
                                    int score = scores.GetScore(firstIndex, secondIndex);
                                    if (score < bestScore)
                                    {
                                        bestScore = score;
                                        bestFirstIndex = firstIndex;
                                        bestSecondIndex = secondIndex;
                                        bestSecondDimension = secondDimension;
                                    }
                                }
                            }
                        }

                        // If the best score is zero, then we have found an uncovered pair.
                        if (bestScore == 0)
                            foundUncovered = true;

                        // Record the pair.  We defer recording already covered pairs until we reach the second iteration
                        // to ensure that we will find remaining uncovered pairs during the first iteration.
                        if (foundUncovered || fill)
                        {
                            indices[firstDimension] = bestFirstIndex;
                            indices[bestSecondDimension] = bestSecondIndex;
                        }
                    }

                    // We're done when we have finished filling the list of indices.
                    if (fill)
                        break;

                    // If we have just finished the first iteration and still not found any uncovered pairs,
                    // then there are none left to be found.
                    if (!foundUncovered)
                        return false;
                }

                // Mark all pairs that were covered.
                for (int i = 0; i < dimensions; i++)
                    for (int j = i + 1; j < dimensions; j++)
                        dimensionPairScores[i][j].IncrementScore(indices[i], indices[j]);
                return true;
            }
        }

        private struct ScoreTable
        {
            private readonly int[,] scores;
            private readonly bool swapped;

            public ScoreTable(int[,] scores, bool swapped)
            {
                this.scores = scores;
                this.swapped = swapped;
            }

            public int GetScore(int firstIndex, int secondIndex)
            {
                return swapped ? scores[secondIndex, firstIndex] : scores[firstIndex, secondIndex];
            }

            public void IncrementScore(int firstIndex, int secondIndex)
            {
                if (swapped)
                    scores[secondIndex, firstIndex] += 1;
                else
                    scores[firstIndex, secondIndex] += 1;
            }
        }
    }
}
