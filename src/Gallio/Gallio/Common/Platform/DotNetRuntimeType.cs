using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Platform
{
    /// <summary>
    /// Specifies the .Net runtime type.
    /// </summary>
    /// <seealso cref="DotNetRuntimeSupport"/>
    public enum DotNetRuntimeType
    {
        /// <summary>
        /// Using the Microsoft CLR.
        /// </summary>
        CLR = 0,

        /// <summary>
        /// Using the Mono runtime.
        /// </summary>
        Mono = 1
    }
}
