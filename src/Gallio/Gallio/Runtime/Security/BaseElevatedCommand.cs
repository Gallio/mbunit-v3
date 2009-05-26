using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.Security
{
    /// <summary>
    /// Abstract base class for implementing elevated commands.
    /// </summary>
    /// <typeparam name="TArg">The argument type for the command, must be serializable</typeparam>
    /// <typeparam name="TResult">The result type for the command, must be serializable</typeparam>
    public abstract class BaseElevatedCommand<TArg, TResult> : IElevatedCommand
    {
        /// <inheritdoc />
        public object Execute(object arguments, IProgressMonitor progressMonitor)
        {
            return Execute((TArg)arguments, progressMonitor);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="arguments">The command arguments</param>
        /// <param name="progressMonitor">The progress monitor, non-null</param>
        /// <returns>The command result, must be null or serializable</returns>
        protected abstract TResult Execute(TArg arguments, IProgressMonitor progressMonitor);
    }
}
