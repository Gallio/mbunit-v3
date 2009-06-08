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
    /// A filter is a serializable predicate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The framework uses filters to select among tests discovered through 
    /// the process of test enumeration.
    /// </para>
    /// <para>
    /// Filters must be serializable objects defined in the framework assembly
    /// or in plugins.  The reason is that filter definitions may need to be
    /// transferred across AppDomains so the filter's concrete type must be
    /// known to the recipient.  Filters do not implement <see cref="MarshalByRefObject" />
    /// because there is no guarantee that a valid remoting context will exist
    /// in which to carry out the operation.  So, if you need a new filter type
    /// put it in a plugin.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class Filter<T>
    {
        /// <summary>
        /// Determines whether the filter matches the value.
        /// </summary>
        /// <param name="value">The value to consider, never null.</param>
        /// <returns>True if the filter matches the value.</returns>
        public abstract bool IsMatch(T value);

        /// <summary>
        /// Accepts a visitor and performs double-dispatch.
        /// </summary>
        /// <param name="visitor">The visitor, never null.</param>
        public abstract void Accept(IFilterVisitor visitor);

        /// <summary>
        /// Formats the filter to a string suitable for parsing by <see cref="FilterParser{T}" />.
        /// </summary>
        /// <returns>The formatted filter expression.</returns>
        public string ToFilterExpr()
        {
            FilterFormatter formatter = new FilterFormatter();
            Accept(formatter);
            return formatter.ToString();
        }
    }
}
