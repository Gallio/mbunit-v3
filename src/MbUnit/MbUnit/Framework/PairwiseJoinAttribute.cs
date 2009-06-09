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

using System;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Sets the join strategy of a test to be pairwise.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The test will be executed using values drawn from each data source and combined so
    /// that all possible pairings of values from each data source are produced.  This constraint vastly
    /// reduces the number of combinations because we only consider all interactions among
    /// pairs of variables rather than among all variables at once.  This strategy still
    /// provides a high degree of variability among combinations and has a high likelihood of
    /// finding bugs while incurring much less cost than testing all possible combinations.
    /// </para>
    /// <para>
    /// If there are three data sources, A, B and C with values A1, A2, B1, B2, C1 and C2 then
    /// the test will be run four times with inputs: (A1, B1, C1), (A2, B2, C1), (A1, B2, C2), (A2, B1, C2).
    /// Compare this with the eight times that a standard cross-product combinatorial join
    /// strategy would have required.
    /// </para>
    /// <para>
    /// The algorithm used to compute pairwise coverings is approximate and might not find
    /// a minimal pairwise covering of values.  However, the relative size of a full combinatorial
    /// cross product will still be exponentially larger than that of the covering that is produced.
    /// </para>
    /// </remarks>
    /// <seealso cref="PairwiseJoinStrategy"/>
    /// <seealso cref="SequentialJoinAttribute"/>
    /// <seealso cref="CombinatorialJoinAttribute"/>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class PairwiseJoinAttribute : JoinAttribute
    {
        /// <inheritdoc />
        protected override IJoinStrategy GetJoinStrategy()
        {
            return PairwiseJoinStrategy.Instance;
        }
    }
}
