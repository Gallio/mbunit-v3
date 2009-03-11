using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Gallio.Runtime.Debugging
{
    /// <summary>
    /// Provides control over a debugger.
    /// </summary>
    public interface IDebugger
    {
        /// <summary>
        /// Returns true if the debugger is attached to a process.
        /// </summary>
        /// <param name="process">The process to which the debugger should be attached</param>
        /// <returns>True if the debugger is already attached</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is null</exception>
        bool IsAttachedToProcess(Process process);

        /// <summary>
        /// Attaches the debugger to a process.
        /// </summary>
        /// <param name="process">The process to which the debugger should be attached</param>
        /// <returns>A result code to indicate whether the debugger was attached</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is null</exception>
        AttachDebuggerResult AttachToProcess(Process process);

        /// <summary>
        /// Detaches the debugger from a process.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Does nothing if the process was not attached.
        /// </para>
        /// </remarks>
        /// <param name="process">The process from which the debugger should be detached</param>
        /// <returns>A result code to indicate whether the debugger was detached</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is null</exception>
        DetachDebuggerResult DetachFromProcess(Process process);
    }
}
