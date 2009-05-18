using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// Installs or uninstalls components on the local machine.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An installer enables plugins to register and unregister their components with
    /// the operating system as needed.  Typically the installers are invoked
    /// by the Gallio installation package (ie. the originally downloaded MSI file)
    /// but they can also be invoked by the user via the <see cref="SetupCommand" />
    /// utility command.
    /// </para>
    /// </remarks>
    [Traits(typeof(InstallerTraits))]
    public interface IInstaller
    {
        /// <summary>
        /// Installs components.
        /// </summary>
        /// <param name="logger">The logger, not null</param>
        /// <param name="progressMonitor">The progress monitor, not null</param>
        void Install(ILogger logger, IProgressMonitor progressMonitor);

        /// <summary>
        /// Uninstalls components.
        /// </summary>
        /// <param name="logger">The logger, not null</param>
        /// <param name="progressMonitor">The progress monitor, not null</param>
        void Uninstall(ILogger logger, IProgressMonitor progressMonitor);
    }
}
