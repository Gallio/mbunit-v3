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
using System.Reflection;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="ITestDescriptor.CodeElement" />
    /// matches the specified member name filter.  This filter should normally be used together with
    /// a <see cref="TypeFilter" /> to ensure the accuracy of the member match.
    /// </summary>
    [Serializable]
    public class MemberFilter<T> : PropertyFilter<T> where T : ITestDescriptor
    {
        /// <summary>
        /// Creates a member filter.
        /// </summary>
        /// <param name="memberNameFilter">A filter for the member name as returned by <see cref="MemberInfo.Name" /></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="memberNameFilter"/> is null.</exception>
        public MemberFilter(Filter<string> memberNameFilter)
            : base(memberNameFilter)
        {
        }

        /// <inheritdoc />
        public override string Key
        {
            get { return @"Member"; }
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            IMemberInfo member = ReflectionUtils.GetMember(value.CodeElement);
            if (member == null)
                return false;

            return ValueFilter.IsMatch(member.Name);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Member(" + ValueFilter + @")";
        }
    }
}