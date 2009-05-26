using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.Security
{
    /// <summary>
    /// Represents a command to be executed in an elevated context.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The command parameters and results must be serializable because they may be
    /// transmitted across processes.
    /// </para>
    /// </remarks>
    public interface IElevatedCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="arguments">The command arguments</param>
        /// <param name="progressMonitor">The progress monitor, non-null</param>
        /// <returns>The command result, must be null or serializable</returns>
        object Execute(object arguments, IProgressMonitor progressMonitor);
    }
}
