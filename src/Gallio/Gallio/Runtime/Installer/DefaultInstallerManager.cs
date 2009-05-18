using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// The default installer manager implementation.
    /// </summary>
    public class DefaultInstallerManager : IInstallerManager
    {
        private readonly ComponentHandle<IInstaller, InstallerTraits>[] installerHandles;

        /// <summary>
        /// Creates the installer manager.
        /// </summary>
        /// <param name="installerHandles">The installer handles, not null</param>
        public DefaultInstallerManager(ComponentHandle<IInstaller, InstallerTraits>[] installerHandles)
        {
            this.installerHandles = installerHandles;
        }

        /// <inheritdoc />
        public void Install(ILogger logger, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Installing components.", installerHandles.Length + 1))
            {
                foreach (var installerHandle in installerHandles)
                {
                    installerHandle.GetComponent().Install(logger, progressMonitor.CreateSubProgressMonitor(1));
                }
            }
        }

        /// <inheritdoc />
        public void Uninstall(ILogger logger, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Uninstalling components.", installerHandles.Length + 1))
            {
                foreach (var installerHandle in installerHandles)
                {
                    installerHandle.GetComponent().Uninstall(logger, progressMonitor.CreateSubProgressMonitor(1));
                }
            }
        }
    }
}
