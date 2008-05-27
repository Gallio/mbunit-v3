// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Provides distinct instances of a particular type.
    /// </summary>
    /// <typeparam name="T">The type of object to provide</typeparam>
    public interface IDistinctInstanceProvider<T>
    {
        /// <summary>
        /// <para>
        /// Gets distinct instances of <typeparamref name="T"/>.
        /// </para>
        /// <para>
        /// Each instance within the sequence should be distinct from all of the others within
        /// the same sequence.  However, this method should return sequences that are equal
        /// to each other each time.
        /// </para>
        /// <para>
        /// The sequence should contain at least 2 distinct instances.
        /// </para>
        /// </summary>
        /// <returns>The distinct instances</returns>
        IEnumerable<T> GetDistinctInstances();
    }
}
