using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// Performs installation tasks and manages the set of registered <see cref="IInstaller" /> components.
    /// </summary>
    public interface IInstallerManager
    {
        /// <summary>
        /// Installs components.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/>
        /// or <paramref name="progressMonitor"/> is null</exception>
        void Install(ILogger logger, IProgressMonitor progressMonitor);

        /// <summary>
        /// Uninstalls components.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/>
        /// or <paramref name="progressMonitor"/> is null</exception>
        void Uninstall(ILogger logger, IProgressMonitor progressMonitor);
    }
}
