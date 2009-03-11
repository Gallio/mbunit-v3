using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Gallio.Runtime.Debugging
{
    /// <summary>
    /// A null implementation of a debugger.  All services return do nothing results.
    /// </summary>
    public class NullDebugger : IDebugger
    {
        /// <inheritdoc />
        public bool IsAttachedToProcess(Process process)
        {
            return false;
        }

        /// <inheritdoc />
        public AttachDebuggerResult AttachToProcess(Process process)
        {
            return AttachDebuggerResult.CouldNotAttach;
        }

        /// <inheritdoc />
        public DetachDebuggerResult DetachFromProcess(Process process)
        {
            return DetachDebuggerResult.CouldNotDetach;
        }
    }
}
