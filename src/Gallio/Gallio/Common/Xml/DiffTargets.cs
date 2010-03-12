using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// The target of a diff result.
    /// </summary>
    [Flags]
    public enum DiffTargets
    {
        /// <summary>
        /// The actual XML fragment.
        /// </summary>
        Actual = 1,

        /// <summary>
        /// The expected XML fragment.
        /// </summary>
        Expected = 2,

        /// <summary>
        /// The actual and the expected XML fragments.
        /// </summary>
        Both = Actual | Expected,
    }
}
