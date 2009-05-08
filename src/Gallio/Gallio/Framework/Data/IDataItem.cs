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
using Gallio.Common.Collections;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// A data item is an opaque representation of a collection of values
    /// and metadata that may be retrieved using data bindings that encode
    /// the appropriate lookup rules into the item.
    /// </para>
    /// <para>
    /// Data items may have very different forms:
    /// <list type="bullet">
    /// <item>Scalar-like items: Items that always yield a single value, possibly a constant</item>
    /// <item>Row-like items: Items that yield several values in response to index-based
    /// data bindings or named column paths</item>
    /// <item>Structured items: Items that yield values by binding to paths within the item
    /// or by resolving custom data binding expressions</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <seealso cref="IDataAccessor"/>
    public interface IDataItem
    {
        /// <summary>
        /// <para>
        /// Returns true if the item contains data that is obtained dynamically from sources
        /// whose content may change over time or may be expensive to query ahead of time.
        /// </para>
        /// <para>
        /// For example, data obtained from a database should be considered dynamic.
        /// On the other hand, data obtained from declarative metadata defined as part
        /// of the test should be considered static.
        /// </para>
        /// </summary>
        bool IsDynamic { get; }

        /// <summary>
        /// <para>
        /// Populates the specified metadata map with key/value pairs associated with
        /// the data item, if any.
        /// </para>
        /// <para>
        /// For example, the metadata may contain a description that serves
        /// as documentation of the contents of the data item or of the test
        /// scenario that is exercised by the contents of the data item.
        /// This metadata may be injected into test instances created with
        /// the contents of this data item.
        /// </para>
        /// </summary>
        /// <param name="map">The metadata map</param>
        void PopulateMetadata(PropertyBag map);

        /// <summary>
        /// Gets the value of the specified binding.
        /// </summary>
        /// <param name="binding">The data binding, never null</param>
        /// <returns>The value</returns>
        /// <exception cref="DataBindingException">Thrown if the <paramref name="binding"/>
        /// cannot be resolved or if its value cannot be obtained</exception>
        object GetValue(DataBinding binding);

        /// <summary>
        /// Gets an enumeration of the data bindings that may be queried to informally describe
        /// its contents.  The enumeration of bindings may not be complete.
        /// </summary>
        /// <returns>The enumeration of bindings</returns>
        IEnumerable<DataBinding> GetBindingsForInformalDescription();
    }
}
