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

namespace Gallio.Model.Filters
{
    /// <summary>
    /// Provides utilities for manipulating filters.
    /// </summary>
    public static class FilterUtils
    {
        /// <summary>
        /// Parses a test filter.
        /// </summary>
        /// <param name="filterExpr">The filter expression</param>
        /// <returns>The parsed filter</returns>
        public static Filter<ITest> ParseTestFilter(string filterExpr)
        {
            FilterParser<ITest> parser = new FilterParser<ITest>(new ModelComponentFilterFactory<ITest>());
            return parser.Parse(filterExpr);
        }
    }
}
