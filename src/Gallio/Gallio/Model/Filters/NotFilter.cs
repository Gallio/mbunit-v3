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

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter combinator that is the negation of another filter.
    /// </summary>
    [Serializable]
    public class NotFilter<T> : Filter<T>
    {
        private Filter<T> filter;

        /// <summary>
        /// Creates a NOT-filter.
        /// </summary>
        /// <param name="filter">The filter to be negated.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filter"/> is null.</exception>
        public NotFilter(Filter<T> filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            this.filter = filter;
        }

        /// <summary>
        /// Gets the filter to be negated.
        /// </summary>
        public Filter<T> Filter
        {
            get { return filter; }
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            return !filter.IsMatch(value);
        }

        /// <inheritdoc />
        public override void Accept(IFilterVisitor visitor)
        {
            visitor.VisitNotFilter(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Not(" + filter + @")";
        }
    }
}