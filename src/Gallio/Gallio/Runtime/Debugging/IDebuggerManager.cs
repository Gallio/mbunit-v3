using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Debugging
{
    /// <summary>
    /// Obtains a reference to a debugger.
    /// </summary>
    public interface IDebuggerManager
    {
        /// <summary>
        /// Gets the default debugger.
        /// </summary>
        /// <returns>The default debugger</returns>
        IDebugger GetDefaultDebugger();
    }
}
