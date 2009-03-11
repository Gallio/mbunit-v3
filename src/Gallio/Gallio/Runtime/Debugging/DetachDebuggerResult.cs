using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Debugging
{
    /// <summary>
    /// Describes different possible outcomes from detaching a debugger from a process.
    /// </summary>
    public enum DetachDebuggerResult
    {
        /// <summary>
        /// The debugger could not be detached from the process and remains attached.
        /// </summary>
        CouldNotDetach,

        /// <summary>
        /// The debugger was already detached from the process.
        /// </summary>
        AlreadyDetached,

        /// <summary>
        /// The debugger has just been detached from the process.
        /// </summary>
        Detached
    }
}
