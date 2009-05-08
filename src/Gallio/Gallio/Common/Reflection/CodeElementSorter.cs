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

using System.Collections.Generic;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Sorts code elements in various ways.
    /// </summary>
    public static class CodeElementSorter
    {
        /// <summary>
        /// Sorts members such that those declared by supertypes appear before those
        /// declared by subtypes.
        /// </summary>
        /// <typeparam name="T">The type of member</typeparam>
        /// <param name="members">The members to sort</param>
        /// <returns>The sorted members</returns>
        /// <seealso cref="DeclaringTypeComparer{T}"/>
        public static IList<T> SortMembersByDeclaringType<T>(IEnumerable<T> members)
            where T : IMemberInfo
        {
            List<T> sortedMembers = new List<T>(members);
            sortedMembers.Sort(DeclaringTypeComparer<T>.Instance);
            return sortedMembers;
        }
    }
}
