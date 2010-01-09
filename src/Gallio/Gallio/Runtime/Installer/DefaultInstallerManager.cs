// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Collections;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// The default installer manager implementation.
    /// </summary>
    public class DefaultInstallerManager : IInstallerManager
    {
        private readonly ComponentHandle<IInstaller, InstallerTraits>[] installerHandles;
        private readonly IElevationManager elevationManager;

        /// <summary>
        /// Creates the installer manager.
        /// </summary>
        /// <param name="installerHandles">The installer handles, not null.</param>
        /// <param name="elevationManager">The elevation manager, not null.</param>
        public DefaultInstallerManager(ComponentHandle<IInstaller, InstallerTraits>[] installerHandles,
            IElevationManager elevationManager)
        {
            this.installerHandles = installerHandles;
            this.elevationManager = elevationManager;
        }

        /// <inheritdoc />
        public bool Install(IList<string> installerIds, IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            return InstallOrUninstall(installerIds, elevationContext, progressMonitor, InstallerOperation.Install);
        }

        /// <inheritdoc />
        public bool Uninstall(IList<string> installerIds, IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            return InstallOrUninstall(installerIds, elevationContext, progressMonitor, InstallerOperation.Uninstall);
        }

        private bool InstallOrUninstall(IList<string> installerIds, IElevationContext elevationContext, IProgressMonitor progressMonitor,
            InstallerOperation operation)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            var filteredInstallerHandles = FilterInstallers(installerIds);

            using (progressMonitor.BeginTask(operation == InstallerOperation.Install ? "Installing components." : "Uninstalling components.",
                filteredInstallerHandles.Count + 1))
            {
                if (progressMonitor.IsCanceled)
                    return false;

                if (elevationContext != null
                    || elevationManager.HasElevatedPrivileges
                    || ! IsElevationRequired(filteredInstallerHandles))
                {
                    return InstallOrUninstallWithElevationContext(filteredInstallerHandles, elevationContext, progressMonitor, operation);
                }

                return elevationManager.TryElevate(
                    newElevationContext => InstallOrUninstallWithElevationContext(filteredInstallerHandles, newElevationContext, progressMonitor, operation),
                    "Administrative access required to install or uninstall certain components.");
            }
        }

        private static bool InstallOrUninstallWithElevationContext(IEnumerable<ComponentHandle<IInstaller, InstallerTraits>> installerHandles,
            IElevationContext elevationContext, IProgressMonitor progressMonitor,
            InstallerOperation operation)
        {
            foreach (var installerHandle in installerHandles)
            {
                if (progressMonitor.IsCanceled)
                    return false;

                IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1);
                if (elevationContext != null && installerHandle.GetTraits().RequiresElevation)
                {
                    elevationContext.Execute(InstallerElevatedCommand.ElevatedCommandId,
                        new InstallerElevatedCommand.Arguments(installerHandle.Id, operation),
                        subProgressMonitor);
                }
                else
                {
                    IInstaller installer = installerHandle.GetComponent();

                    if (operation == InstallerOperation.Install)
                        installer.Install(progressMonitor.CreateSubProgressMonitor(1));
                    else
                        installer.Uninstall(progressMonitor.CreateSubProgressMonitor(1));
                }
            }

            return true;
        }

        private static bool IsElevationRequired(IEnumerable<ComponentHandle<IInstaller, InstallerTraits>> installerHandles)
        {
            foreach (var installerHandle in installerHandles)
                if (installerHandle.GetTraits().RequiresElevation)
                    return true;

            return false;
        }

        private IList<ComponentHandle<IInstaller, InstallerTraits>> FilterInstallers(IList<string> installerIds)
        {
            if (installerIds == null)
                return installerHandles;

            var filteredInstallerHandles = new List<ComponentHandle<IInstaller, InstallerTraits>>();
            foreach (string installerId in installerIds)
            {
                var installerHandle = GenericCollectionUtils.Find(installerHandles, x => x.Id == installerId);
                if (installerHandle == null)
                    throw new InvalidOperationException(string.Format("Could not find installer with id '{0}'.", installerId));

                filteredInstallerHandles.Add(installerHandle);
            }

            return filteredInstallerHandles;
        }
    }
}
