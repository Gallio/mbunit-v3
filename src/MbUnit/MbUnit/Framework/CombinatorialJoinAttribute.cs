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

using System;
using Gallio.Framework.Data;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Sets the join strategy of a test to be combinatorial.
    /// </para>
    /// <para>
    /// The test will be executed using all possible combinations of values from each data source.
    /// </para>
    /// <para>
    /// If there are two data sources, A and B with values A1, A2, B1 and B2, then the test will
    /// be run four times with inputs: (A1, B1), (A1, B2), (A2, B1) and (A2, B2).
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the default join strategy for tests.  So you do not need to specify this attribute
    /// unless you want to be explicit about selecting it.
    /// </para>
    /// </remarks>
    /// <seealso cref="CombinatorialJoinStrategy"/>
    /// <seealso cref="SequentialJoinAttribute"/>
    /// <seealso cref="PairwiseJoinAttribute"/>
    public class CombinatorialJoinAttribute : JoinAttribute
    {
        /// <inheritdoc />
        protected override IJoinStrategy GetJoinStrategy()
        {
            return CombinatorialJoinStrategy.Instance;
        }
    }
}
