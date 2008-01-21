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

namespace Gallio.Data
{
    /// <summary>
    /// <para>
    /// The pairwise strategy constructs a limited number of combinations of the
    /// rows within the data providers such that each combination differs by
    /// at most two different factors.
    /// </para>
    /// <para>
    /// This strategy can be more efficient than one based on exhaustively testing all
    /// combinations since many test failures result from the interaction of a relatively
    /// small number of factors: often just two of them.
    /// </para>
    /// </summary>
    /// <remarks author="jeff">
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
        public static readonly PairwiseJoinStrategy PairwiseInstance = new PairwiseJoinStrategy();

        private PairwiseJoinStrategy()
        {
        }

        /// <inheritdoc />
        public IEnumerable<IList<IDataRow>> Join(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider)
        {
            throw new NotImplementedException();
        }
    }
}
