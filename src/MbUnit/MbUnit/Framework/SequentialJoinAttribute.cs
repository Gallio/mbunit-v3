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
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Sets the join strategy of a test to be sequential.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The test will be executed using values drawn from each data source and combined sequentially
    /// with elements from each data source chosen in order.
    /// </para>
    /// <para>
    /// If there are two data sources, A and B with values A1, A2, B1 and B2, then the test will be
    /// run twice with inputs: (A1, B1) and (A2, B2).
    /// </para>
    /// </remarks>
    /// <seealso cref="SequentialJoinStrategy"/>
    /// <seealso cref="CombinatorialJoinAttribute"/>
    /// <seealso cref="PairwiseJoinAttribute"/>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class SequentialJoinAttribute : JoinAttribute
    {
        /// <inheritdoc />
        protected override IJoinStrategy GetJoinStrategy()
        {
            return SequentialJoinStrategy.Instance;
        }
    }
}
