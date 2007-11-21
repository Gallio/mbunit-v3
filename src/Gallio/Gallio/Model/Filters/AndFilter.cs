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

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter combinator that matches a value when all of its constituent filters
    /// match the value.
    /// </summary>
    [Serializable]
    public class AndFilter<T> : Filter<T>
    {
        private readonly Filter<T>[] filters;

        /// <summary>
        /// Creates an AND-filter.
        /// </summary>
        /// <param name="filters">The filters that must all jointly be matched.
        /// If the array is empty, the filter matches everything.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filters"/> is null</exception>
        public AndFilter(Filter<T>[] filters)
        {
            if (filters == null)
                throw new ArgumentNullException("filters");

            this.filters = filters;
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            return Array.TrueForAll(filters, delegate(Filter<T> filter)
            {
                return filter.IsMatch(value);
            });
        }

        /// <inheritdoc />
        public override string ToString()
        {
            string innerValues = string.Empty;
            Array.ForEach(filters, delegate(Filter<T> filter)
            {
                innerValues += " {" + filter + "} ";
            });
            return " And(" + innerValues + ") ";
        }
    }
}
