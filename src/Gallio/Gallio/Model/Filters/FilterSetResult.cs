using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// Specifies whether a filter set includes, excludes or does not match a particular item.
    /// </summary>
    public enum FilterSetResult
    {
        /// <summary>
        /// The filter set neither includes nor excludes the item.
        /// </summary>
        Unmatched = 0,

        /// <summary>
        /// The filter set includes the item.
        /// </summary>
        Include = 1,

        /// <summary>
        /// The filter set excludes the item.
        /// </summary>
        Exclude = -1,
    }
}
