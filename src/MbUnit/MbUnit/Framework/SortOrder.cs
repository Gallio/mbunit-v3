using System;
using System.Collections.Generic;
using System.Text;
using Gallio;

namespace MbUnit.Framework
{
    /// <summary>
    /// Expected sorting order for the <see cref="Assert.Sorted{T}(IEnumerable{T}, SortOrder)"/> assertion.
    /// </summary>
	/// <seealso cref="Assert.Sorted{T}(IEnumerable{T}, SortOrder)"/>
	public enum SortOrder
    {
        /// <summary>
        /// Each value is expected to be greater than or equal to the previous value.
        /// </summary>
        Increasing,
        
        /// <summary>
        /// Each value is expected to be strictly greater than the previous value.
        /// </summary>
        StrictlyIncreasing,
        
        /// <summary>
        /// Each value is expected to be less than or equal to the previous value.
        /// </summary>
        Decreasing,
        
        /// <summary>
        /// Each value is expected to be strictly less than the previous value.
        /// </summary>
        StrictlyDecreasing,
    }
}
