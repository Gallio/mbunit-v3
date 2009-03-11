using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Debugging
{
    /// <summary>
    /// Describes different possible outcomes from attaching a debugger to a process.
    /// </summary>
    public enum AttachDebuggerResult
    {
        /// <summary>
        /// The debugger could not attach to the process.
        /// </summary>
        CouldNotAttach,

        /// <summary>
        /// The debugger was already attached to the process.
        /// </summary>
        AlreadyAttached,

        /// <summary>
        /// The debugger has just been attached to the process.
        /// </summary>
        Attached
    }
}
