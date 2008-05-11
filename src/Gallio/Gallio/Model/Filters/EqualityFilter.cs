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

namespace Gallio.Model.Filters
{
    /// <summary>
    /// An equality filter compares values for strict equality.
    /// </summary>
    [Serializable]
    public sealed class EqualityFilter<T> : Filter<T>
        where T : class, IEquatable<T>
    {
        private readonly T comparand;

        /// <summary>
        /// Creates an equality filter.
        /// </summary>
        /// <param name="comparand">The value to compare for equality</param>
        public EqualityFilter(T comparand)
        {
            if (comparand == null)
                throw new ArgumentNullException("comparand");
            this.comparand = comparand;
        }

        /// <summary>
        /// Gets the value to compare for equality.
        /// </summary>
        public T Comparand
        {
            get { return comparand; }
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            return comparand.Equals(value);
        }

        /// <inheritdoc />
        public override void Accept(IFilterVisitor visitor)
        {
            visitor.VisitEqualityFilter(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Equality('" + comparand + @"')";
        }
    }
}
