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
        /// <param name="progressMonitor">The progress monitor, not null</param>
        void Install(IProgressMonitor progressMonitor);

        /// <summary>
        /// Uninstalls components.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor, not null</param>
        void Uninstall(IProgressMonitor progressMonitor);
    }
}
