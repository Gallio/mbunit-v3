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
using System.Collections.ObjectModel;
using System.Text;
using Gallio.Common.Collections;

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
        /// If the list is empty, the filter matches everything.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filters"/> is null</exception>
        public AndFilter(ICollection<Filter<T>> filters)
        {
            if (filters == null || filters.Contains(null))
                throw new ArgumentNullException("filters");

            this.filters = GenericCollectionUtils.ToArray(filters);
        }

        /// <summary>
        /// Gets the filters that must jointly be matched.
        /// If the list is empty, the filter matches everything.
        /// </summary>
        public IList<Filter<T>> Filters
        {
            get { return new ReadOnlyCollection<Filter<T>>(filters); }
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
        public override void Accept(IFilterVisitor visitor)
        {
            visitor.VisitAndFilter(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(@"And({ ");

            for (int i = 0; i < filters.Length; i++)
            {
                if (i != 0)
                    result.Append(@", ");

                result.Append(filters[i]);
            }

            result.Append(@" })");
            return result.ToString();
        }
    }
}
