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
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// An elevated command used for installation.
    /// </summary>
    public class InstallerElevatedCommand : BaseElevatedCommand<InstallerElevatedCommand.Arguments, object>
    {
        private readonly IInstallerManager installerManager;

        /// <summary>
        /// The id of this elevated command.
        /// </summary>
        public static readonly string ElevatedCommandId = "Gallio.InstallerElevatedCommand";

        /// <summary>
        /// Initializes the command.
        /// </summary>
        /// <param name="installerManager">The installer manager, not null</param>
        public InstallerElevatedCommand(IInstallerManager installerManager)
        {
            this.installerManager = installerManager;
        }

        /// <inheritdoc />
        protected override object Execute(Arguments arguments, IProgressMonitor progressMonitor)
        {
            if (arguments.InstallerOperation == InstallerOperation.Install)
            {
                installerManager.Install(new[] { arguments.InstallerId }, null, progressMonitor);
            }
            else
            {
                installerManager.Uninstall(new[] { arguments.InstallerId }, null, progressMonitor);
            }

            return null;
        }

        /// <summary>
        /// Arguments for the command.
        /// </summary>
        [Serializable]
        public class Arguments
        {
            private readonly string installerId;
            private readonly InstallerOperation installerOperation;

            /// <summary>
            /// Creates the arguments.
            /// </summary>
            /// <param name="installerId">The installer id</param>
            /// <param name="installerOperation">The requested installer operation</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="installerId"/> is null</exception>
            public Arguments(string installerId, InstallerOperation installerOperation)
            {
                if (installerId == null)
                    throw new ArgumentNullException("installerId");

                this.installerId = installerId;
                this.installerOperation = installerOperation;
            }

            /// <summary>
            /// Gets the installer id.
            /// </summary>
            public string InstallerId
            {
                get { return installerId; }
            }

            /// <summary>
            /// Gets the requested installer operation.
            /// </summary>
            public InstallerOperation InstallerOperation
            {
                get { return installerOperation; }
            }
        }
    }
}
