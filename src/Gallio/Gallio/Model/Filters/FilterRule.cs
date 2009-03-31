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
