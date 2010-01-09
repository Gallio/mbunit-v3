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

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="ITestDescriptor.Name" />
    /// matches the specified name filter.
    /// </summary>
    [Serializable]
    public class NameFilter<T> : PropertyFilter<T> where T : ITestDescriptor
    {
        /// <summary>
        /// Creates a name filter.
        /// </summary>
        /// <param name="nameFilter">A filter for the name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nameFilter"/> is null.</exception>
        public NameFilter(Filter<string> nameFilter)
            : base(nameFilter)
        {
        }

        /// <inheritdoc />
        public override string Key
        {
            get { return @"Name"; }
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            return ValueFilter.IsMatch(value.Name);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Name(" + ValueFilter + @")";
        }
    }
}