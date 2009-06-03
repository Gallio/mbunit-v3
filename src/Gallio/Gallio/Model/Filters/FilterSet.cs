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
using System.Collections.ObjectModel;
using System.Text;
using Gallio.Common.Collections;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filterset combines zero or more filters as a sequence of inclusion and exclusion rules.
    /// </summary>
    [Serializable]
    public sealed class FilterSet<T>
    {
        private readonly FilterRule<T>[] rules;

        /// <summary>
        /// Creates a filter set consisting of a single inclusion filter.
        /// </summary>
        /// <param name="inclusionFilter">The inclusion filter.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inclusionFilter"/> is null.</exception>
        public FilterSet(Filter<T> inclusionFilter)
        {
            if (inclusionFilter == null)
                throw new ArgumentNullException("inclusionFilter");

            rules = new[] { new FilterRule<T>(FilterRuleType.Inclusion, inclusionFilter) };
        }

        /// <summary>
        /// Creates a filter set consisting of a collection of rules.
        /// </summary>
        /// <param name="rules">The filter rules.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rules"/> is null.</exception>
        public FilterSet(ICollection<FilterRule<T>> rules)
        {
            if (rules == null || rules.Contains(null))
                throw new ArgumentNullException("rules");

            this.rules = GenericCollectionUtils.ToArray(rules);
        }

        /// <summary>
        /// Returns an empty filter set.
        /// </summary>
        /// <remarks>
        /// An empty filter set neither includes nor excludes any values on its own.
        /// However when filtering tests, Gallio applies an implicit inclusion rule when
        /// none is otherwise specified so the net effect if that all tests will be
        /// included (although "explicit" tests will not be selected explicitly so they
        /// will not run).
        /// </remarks>
        public static FilterSet<T> Empty
        {
            get { return new FilterSet<T>(EmptyArray<FilterRule<T>>.Instance); }
        }

        /// <summary>
        /// Gets the read-only list of filter rules.
        /// </summary>
        public IList<FilterRule<T>> Rules
        {
            get { return new ReadOnlyCollection<FilterRule<T>>(rules); }
        }

        /// <summary>
        /// Returns true if the filter set is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return rules.Length == 0; }
        }

        /// <summary>
        /// Returns true if the filter set has at least one inclusion rule.
        /// </summary>
        public bool HasInclusionRules
        {
            get { return Array.Exists(rules, rule => rule.RuleType == FilterRuleType.Inclusion); }
        }

        /// <summary>
        /// Evaluates the rules of this filter set and indicates whether the item is included, excluded
        /// or not matched by the list of rules.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The result</returns>
        public FilterSetResult Evaluate(T item)
        {
            foreach (var rule in rules)
            {
                if (rule.Filter.IsMatch(item))
                    return rule.RuleType  == FilterRuleType.Inclusion ? FilterSetResult.Include : FilterSetResult.Exclude;
            }

            return FilterSetResult.Unmatched;
        }

        /// <summary>
        /// Formats the filter set to a string suitable for parsing by <see cref="FilterParser{T}" />.
        /// </summary>
        /// <remarks>
        /// The empty filter is represented by an empty string.
        /// </remarks>
        /// <returns>The formatted filter set expression</returns>
        public string ToFilterSetExpr()
        {
            if (rules.Length == 0)
                return "";

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < rules.Length; i++)
            {
                if (rules[i].RuleType == FilterRuleType.Inclusion)
                {
                    if (i != 0)
                        result.Append(" include "); // only show "include" as an infix, no need to have as prefix on first rule
                }
                else
                {
                    if (i != 0)
                        result.Append(' ');
                    result.Append("exclude ");
                }

                result.Append(rules[i].Filter.ToFilterExpr());
            }

            return result.ToString();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToFilterSetExpr();
        }
    }
}
