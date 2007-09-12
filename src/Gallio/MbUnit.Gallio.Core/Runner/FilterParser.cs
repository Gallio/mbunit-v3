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
using System.Collections.Generic;
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Provides functions for construncting test filters a string representation.
    /// </summary>
    public static class FilterParser
    {
        /// <summary>
        /// Parses a description of a list of filters that must be jointly satisfied
        /// in the format "FilterKey1=Value1;FilterKey2=Value2a,Value2b;..."
        /// and constructs a <see cref="Filter{ITest}" /> from it.  The format allows for
        /// compact specification of alternative values delimited by commas.
        /// </summary>
        /// <typeparam name="T">The type to filter</typeparam>
        /// <param name="filterListDescription">The filter list description</param>
        /// <returns>The constructed filter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filterListDescription"/> is null</exception>
        public static Filter<T> ParseFilterList<T>(string filterListDescription)
            where T : IModelComponent
        {
            if (filterListDescription == null)
                throw new ArgumentNullException("filterListDescription");

            string[] filterDescriptions = filterListDescription.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Filter<T>[] filters = GenericUtils.ConvertAllToArray<string, Filter<T>>(filterDescriptions, ParseFilter<T>);

            return new AndFilter<T>(filters);
        }

        /// <summary>
        /// Parses a description of a filter in the format "FilterKey=Value1,Value2,Value3,..."
        /// and constructs a <see cref="Filter{ITest}" /> from it.  The format allows for
        /// compact specification of alternative values delimited by commas.
        /// </summary>
        /// <param name="filterDescription">The filter description</param>
        /// <returns>The constructed filter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filterDescription"/> is null</exception>
        public static Filter<T> ParseFilter<T>(string filterDescription)
            where T : IModelComponent
        {
            if (filterDescription == null)
                throw new ArgumentNullException("filterDescription");

            int equalsPos = filterDescription.IndexOf('=');
            if (equalsPos <= 0)
                throw new ArgumentException("Missing '=' between filter key and values.", "filterDescription");

            string key = filterDescription.Substring(0, equalsPos);
            string values = equalsPos + 1 == filterDescription.Length ? "" : filterDescription.Substring(equalsPos + 1);
            string[] splitValues = values.Split(',');
            return BuildFilter<T>(key, splitValues);
        }

        /// <summary>
        /// Builds a filter given a filter key and a list of accepted values for that filter.
        /// </summary>
        /// <remarks>
        /// Recognizes the following filter keys:
        /// <list type="bullet">
        /// <item>Id: Filter by id</item>
        /// <item>Assembly: Filter by assembly name</item>
        /// <item>Namespace: Filter by namespace name</item>
        /// <item>Type: Filter by type name</item>
        /// <item>Member: Filter by member name</item>
        /// <item>*: All other names are assumed to correspond to metadata keys</item>
        /// </list>
        /// </remarks>
        /// <param name="filterKey">The filter key</param>
        /// <param name="filterValues">The accepted values</param>
        /// <typeparam name="T">The type to filter</typeparam>
        /// <returns>The constructed filter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filterKey"/> or <paramref name="filterValues"/> is null</exception>
        public static Filter<T> BuildFilter<T>(string filterKey, string[] filterValues)
            where T : IModelComponent
        {
            if (filterKey == null)
                throw new ArgumentNullException("filterKey");
            if (filterValues == null)
                throw new ArgumentNullException("filterValues");

            Filter<T>[] filters = GenericUtils.ConvertAllToArray<string, Filter<T>>(filterValues, delegate(string filterValue)
            {
                return BuildFilter<T>(filterKey, filterValue);
            });

            return new OrFilter<T>(filters);
        }

        private static Filter<T> BuildFilter<T>(string filterKey, string filterValue)
            where T : IModelComponent
        {
            switch (filterKey)
            {
                case "Id":
                    return new IdFilter<T>(filterValue);
                case "Assembly":
                    return new AssemblyFilter<T>(filterValue);
                case "Namespace":
                    return new NamespaceFilter<T>(filterValue);
                case "Type":
                    return new TypeFilter<T>(filterValue, true);
                case "Member":
                    return new MemberFilter<T>(filterValue);
                default:
                    return new MetadataFilter<T>(filterKey, filterValue);
            }
        }
    }
}
