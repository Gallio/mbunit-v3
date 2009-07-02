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

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// An elevated command used for resetting the installation id.
    /// </summary>
    public class ResetInstallationIdElevatedCommand : BaseElevatedCommand<ResetInstallationIdElevatedCommand.Arguments, object>
    {
        /// <summary>
        /// The id of this elevated command.
        /// </summary>
        public static readonly string ElevatedCommandId = "Gallio.ResetInstallationIdElevatedCommand";

        /// <summary>
        /// Initializes the command.
        /// </summary>
        public ResetInstallationIdElevatedCommand()
        {
        }

        /// <inheritdoc />
        protected override object Execute(Arguments arguments, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Resetting the installation id.", 1))
            {
                var config = InstallationConfiguration.LoadFromRegistry();

                config.InstallationId = Guid.NewGuid();
                config.SaveInstallationIdToRegistry();
            }

            return null;
        }

        /// <summary>
        /// Arguments for the command.
        /// </summary>
        [Serializable]
        public class Arguments
        {
        }
    }
}
