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
    /// Abstract base class for filters that encapsulate a rule
    /// for selecting some property of an object to be compared
    /// against a string value filter.
    /// </summary>
    [Serializable]
    public abstract class PropertyFilter<T> : Filter<T>
    {
        private readonly Filter<string> valueFilter;

        /// <summary>
        /// Creates a filter for some property by string value.
        /// </summary>
        /// <param name="valueFilter">The string value filter.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="valueFilter"/> is null.</exception>
        public PropertyFilter(Filter<string> valueFilter)
        {
            if (valueFilter == null)
                throw new ArgumentNullException("valueFilter");

            this.valueFilter = valueFilter;
        }

        /// <summary>
        /// Gets the key that represents the filtered property.
        /// </summary>
        public abstract string Key { get; }

        /// <summary>
        /// Gets the string value filter.
        /// </summary>
        public Filter<string> ValueFilter
        {
            get { return valueFilter; }
        }

        /// <inheritdoc />
        public override void Accept(IFilterVisitor visitor)
        {
            visitor.VisitPropertyFilter(this);
        }
    }
}
