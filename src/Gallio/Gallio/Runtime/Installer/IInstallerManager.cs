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
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;

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
        /// <param name="installerIds">The ids of the specific installers to include, or null to include all.</param>
        /// <param name="elevationContext">A privilege elevation context, or null if the installer manager should obtain its own when needed.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <returns>True on success, false if the user canceled the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null.</exception>
        bool Install(IList<string> installerIds, IElevationContext elevationContext, IProgressMonitor progressMonitor);

        /// <summary>
        /// Uninstalls components.
        /// </summary>
        /// <param name="installerIds">The ids of the specific installers to include, or null to include all.</param>
        /// <param name="elevationContext">A privilege elevation context, or null if the installer manager should obtain its own when needed.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <returns>True on success, false if the user canceled the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null.</exception>
        bool Uninstall(IList<string> installerIds, IElevationContext elevationContext, IProgressMonitor progressMonitor);
    }
}
