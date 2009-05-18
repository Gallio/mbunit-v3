// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
