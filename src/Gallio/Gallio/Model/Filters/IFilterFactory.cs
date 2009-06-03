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
using System.Text;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// Creates filters of a given type based on a specification that
    /// consists of a filter key and a rule for matching values.
    /// </summary>
    /// <typeparam name="T">The filter type.</typeparam>
    public interface IFilterFactory<T>
    {
        /// <summary>
        /// Creates a filter from a specification 
        /// </summary>
        /// <param name="key">The filter key that identifies the kind of filter to create.</param>
        /// <param name="valueFilter">The filter to use as a rule for matching values.</param>
        /// <returns>The filter.</returns>
        Filter<T> CreateFilter(string key, Filter<string> valueFilter);
    }
}
