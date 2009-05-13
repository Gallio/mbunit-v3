using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies the scope to which certain attributes apply.
    /// </summary>
    /// <seealso cref="ParallelizableAttribute"/>
    public enum TestScope
    {
        /// <summary>
        /// Applies to this test only.
        /// </summary>
        Self,

        /// <summary>
        /// Applies to the descendants of this test only.
        /// </summary>
        Descendants,

        /// <summary>
        /// Applies to this test and its descendants.
        /// </summary>
        All
    }
}
