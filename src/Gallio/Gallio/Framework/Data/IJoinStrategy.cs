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

using System.Collections.Generic;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// A join strategy combines items from multiple providers into products
    /// according to some algorithm.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// A join strategy performs much the same purpose as a "JOIN" in
    /// query languages.  It specifies how items from multiple providers are to
    /// be correlated to create a sequence of aggregate items.
    /// </para>
    /// </remarks>
    /// <seealso cref="JoinedDataSet"/>
    public interface IJoinStrategy
    {
        /// <summary>
        /// Joins the items from each provider into a sequence of aggregate items.
        /// </summary>
        /// <remarks>
        /// The number of elements in each item-list must equal the number
        /// of providers in the <paramref name="providers"/> list because
        /// each item-list should contain exactly one item taken from each
        /// provider.
        /// </remarks>
        /// <param name="providers">The list of providers.</param>
        /// <param name="bindingsPerProvider">The list of bindings per provider.</param>
        /// <param name="includeDynamicItems">If true, includes items that may be dynamically
        /// generated in the result set.  Otherwise excludes such items and only returns
        /// those that are statically known a priori.</param>
        /// <returns>An enumeration of item-lists consisting of exactly one item from
        /// each provider and indexed in the same order as the <paramref name="providers"/>
        /// collection</returns>
        IEnumerable<IList<IDataItem>> Join(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider,
            bool includeDynamicItems);
    }
}
