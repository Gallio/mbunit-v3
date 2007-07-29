// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Reflection;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="IModelComponent.CodeReference" />
    /// contains the specified member.  This filter should normally be used together with
    /// a <see cref="TypeFilter" /> to ensure the accuracy of the member match.
    /// </summary>
    [Serializable]
    public class MemberFilter<T> : Filter<T> where T : IModelComponent
    {
        private string memberName;

        /// <summary>
        /// Creates a member filter.
        /// </summary>
        /// <param name="memberName">The member name exactly as returned by <see cref="MemberInfo.Name" /></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="memberName"/> is null</exception>
        public MemberFilter(string memberName)
        {
            if (memberName == null)
                throw new ArgumentNullException("typeName");

            this.memberName = memberName;
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            MemberInfo member = value.CodeReference.Member;
            if (member == null)
                return false;

            return memberName == member.Name;
        }
    }
}