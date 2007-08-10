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

using System.Collections.Generic;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Utilities
{
    /// <summary>
    /// Allows to construct test filters from a string representation.
    /// </summary>
    public static class FilterParser
    {
        /// <summary>
        /// Parses a string in the format "FilterName1=Value1;FilterName2=Value2,..."
        /// and construct a Filter<ITest> from it.
        /// </summary>
        /// <param name="filter">The string representation of the filter.</param>
        /// <returns>The Constructed Filter<ITest> object.</returns>
        public static Filter<ITest> ParseFilterFromString(string filter)
        {
            List<Filter<ITest>> filters = new List<Filter<ITest>>();
            //Filter<ITest> filters = new AnyFilter<ITest>();

            //if (FilterCategories.Length != 0)
            //{
            //    List<Filter<ITest>> categoryFilters = new List<Filter<ITest>>();

            //    foreach (string category in FilterCategories)
            //        categoryFilters.Add(new MetadataFilter<ITest>(MetadataConstants.CategoryNameKey, category));

            //    filters.Add(new OrFilter<ITest>(categoryFilters.ToArray()));
            //}

            //if (FilterAuthors.Length != 0)
            //{
            //    List<Filter<ITest>> authorFilters = new List<Filter<ITest>>();

            //    foreach (string author in FilterAuthors)
            //        authorFilters.Add(new MetadataFilter<ITest>(MetadataConstants.AuthorNameKey, author));

            //    filters.Add(new OrFilter<ITest>(authorFilters.ToArray()));
            //}

            //if (FilterImportances.Length != 0)
            //{
            //    List<Filter<ITest>> importanceFilters = new List<Filter<ITest>>();

            //    foreach (TestImportance importance in FilterImportances)
            //        importanceFilters.Add(new MetadataFilter<ITest>(MetadataConstants.ImportanceKey, importance.ToString()));

            //    filters.Add(new OrFilter<ITest>(importanceFilters.ToArray()));
            //}

            //if (FilterNamespaces.Length != 0)
            //{
            //    List<Filter<ITest>> namespaceFilters = new List<Filter<ITest>>();

            //    foreach (string @namespace in FilterNamespaces)
            //        namespaceFilters.Add(new NamespaceFilter<ITest>(@namespace));

            //    filters.Add(new OrFilter<ITest>(namespaceFilters.ToArray()));
            //}

            //if (FilterTypes.Length != 0)
            //{
            //    List<Filter<ITest>> typeFilters = new List<Filter<ITest>>();

            //    // FIXME: Should we always include derived types?
            //    foreach (string type in FilterTypes)
            //        typeFilters.Add(new TypeFilter<ITest>(type, true));

            //    filters.Add(new OrFilter<ITest>(typeFilters.ToArray()));
            //}

            return new AndFilter<ITest>(filters.ToArray());
        }
    }
}
