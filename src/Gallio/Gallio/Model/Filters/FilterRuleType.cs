using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// Specifies whether a filter rule is an inclusion or exclusion rule.
    /// </summary>
    /// <seealso cref="FilterRule{T}"/>
    public enum FilterRuleType
    {
        /// <summary>
        /// The filter rule is an inclusion rule.
        /// </summary>
        Inclusion,

        /// <summary>
        /// The filter rule is an exclusion rule.
        /// </summary>
        Exclusion
    }
}
