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

using System.Collections.Generic;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// A merge strategy combines items from multiple providers into a sequence
    /// according to some algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A merge strategy performs much the same purpose as a "UNION" or other
    /// combinator in query languages.  It specifies how to select items among
    /// multiple providers to create an aggregate sequence of items.
    /// </para>
    /// </remarks>
    /// <seealso cref="MergedDataSet"/>
    public interface IMergeStrategy
    {
        /// <summary>
        /// Merges the items from each provider into a new sequence.
        /// </summary>
        /// <param name="providers">The list of providers.</param>
        /// <param name="bindings">The bindings.</param>
        /// <param name="includeDynamicItems">If true, includes items that may be dynamically
        /// generated in the result set.  Otherwise excludes such items and only returns
        /// those that are statically known a priori.</param>
        /// <returns>The merged sequence of items.</returns>
        IEnumerable<IDataItem> Merge(IList<IDataProvider> providers, ICollection<DataBinding> bindings,
            bool includeDynamicItems);
    }
}
