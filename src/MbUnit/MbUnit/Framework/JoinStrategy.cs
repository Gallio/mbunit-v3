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
using System.Text;

namespace MbUnit.Framework
{
    /// <summary>
    /// Defines a strategy for joining data.
    /// </summary>
    /// <seealso cref="DataGenerators.Join{T1,T2}(IEnumerable{T1},IEnumerable{T2},JoinStrategy)"/>
    /// <seealso cref="DataGenerators.Join{T1,T2,T3}(IEnumerable{T1},IEnumerable{T2},IEnumerable{T3},JoinStrategy)"/>
    /// <seealso cref="CombinatorialJoinAttribute"/>
    /// <seealso cref="PairwiseJoinAttribute"/>
    /// <seealso cref="SequentialJoinAttribute"/>
    public enum JoinStrategy
    {
        /// <summary>
        /// Combinatorial join strategy.
        /// </summary>
        /// <seealso cref="CombinatorialJoinAttribute"/>
        Combinatorial,

        /// <summary>
        /// Sequential join strategy.
        /// </summary>
        /// <seealso cref="SequentialJoinAttribute"/>
        Sequential,

        /// <summary>
        /// Pairwise join strategy.
        /// </summary>
        /// <seealso cref="PairwiseJoinAttribute"/>
        Pairwise,
    }
}
