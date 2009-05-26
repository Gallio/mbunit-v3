using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.Security
{
    /// <summary>
    /// Provides a context within which commands may be executed with elevated privileges.
    /// </summary>
    public interface IElevationContext : IDisposable
    {
        /// <summary>
        /// Executes an elevated command.
        /// </summary>
        /// <param name="elevatedCommandId">The command id</param>
        /// <param name="arguments">The command arguments, must be null or serializable</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <returns>The command result</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elevatedCommandId"/>
        /// or <paramref name="progressMonitor"/> is null</exception>
        object Execute(string elevatedCommandId, object arguments, IProgressMonitor progressMonitor);
    }
}
