using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Indentifies a particular version of Visual Studio.
    /// </summary>
    public enum VisualStudioVersion
    {
        /// <summary>
        /// Used to find an instance of any version of Visual Studio.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Visual Studio 2005.
        /// </summary>
        VS2005 = 1,

        /// <summary>
        /// Visual Studio 2008.
        /// </summary>
        VS2008 = 2,

        /// <summary>
        /// Visual Studio 2010.
        /// </summary>
        VS2010 = 3
    }
}
