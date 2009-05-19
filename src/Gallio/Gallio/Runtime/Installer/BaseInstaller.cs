using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// Abstract base class for installers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The base implementation does nothing.
    /// </para>
    /// </remarks>
    public abstract class BaseInstaller : IInstaller
    {
        /// <inheritdoc />
        public virtual void Install(IProgressMonitor progressMonitor)
        {
        }

        /// <inheritdoc />
        public virtual void Uninstall(IProgressMonitor progressMonitor)
        {
        }
    }
}
