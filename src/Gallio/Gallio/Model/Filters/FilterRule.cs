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
    /// Specifies an inclusion or exclusion filter rule.
    /// </summary>
    [Serializable]
    public sealed class FilterRule<T>
    {
        private readonly FilterRuleType ruleType;
        private readonly Filter<T> filter;

        /// <summary>
        /// Creates a filter rule.
        /// </summary>
        /// <param name="ruleType">The rule type: inclusion or exclusion</param>
        /// <param name="filter">The filter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filter"/> is null</exception>
        public FilterRule(FilterRuleType ruleType, Filter<T> filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            this.ruleType = ruleType;
            this.filter = filter;
        }

        /// <summary>
        /// Gets the filter rule type: inclusion or exclusion.
        /// </summary>
        public FilterRuleType RuleType
        {
            get { return ruleType; }
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        public Filter<T> Filter
        {
            get { return filter; }
        }
    }
}
