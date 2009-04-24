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
using System.Collections.Generic;
using System.Text;
using Gallio;

namespace MbUnit.Framework
{
    /// <summary>
    /// Expected sorting order for the <see cref="Assert.Sorted{T}(IEnumerable{T}, SortOrder)"/> assertion.
    /// </summary>
	/// <seealso cref="Assert.Sorted{T}(IEnumerable{T}, SortOrder)"/>
	public enum SortOrder
    {
        /// <summary>
        /// Each value is expected to be greater than or equal to the previous value.
        /// </summary>
        Increasing,
        
        /// <summary>
        /// Each value is expected to be strictly greater than the previous value.
        /// </summary>
        StrictlyIncreasing,
        
        /// <summary>
        /// Each value is expected to be less than or equal to the previous value.
        /// </summary>
        Decreasing,
        
        /// <summary>
        /// Each value is expected to be strictly less than the previous value.
        /// </summary>
        StrictlyDecreasing,
    }
}
