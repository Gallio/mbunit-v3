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
using Gallio.Runtime.Security;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// A utility command to reset the installation id.
    /// </summary>
    public class ResetInstallationIdUtilityCommand : BaseUtilityCommand<object>
    {
        private readonly IElevationManager elevationManager;

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="elevationManager">The elevation manager, not null.</param>
        public ResetInstallationIdUtilityCommand(IElevationManager elevationManager)
        {
            this.elevationManager = elevationManager;
        }

        /// <inheritdoc />
        public override int Execute(UtilityCommandContext context, object arguments)
        {
            context.Logger.Log(LogSeverity.Important, "Resetting the installation id.\nThe plugin list will be refreshed the next time a Gallio application is started.");

            bool result = elevationManager.TryElevate(elevationContext =>
            {
                context.ProgressMonitorProvider.Run(progressMonitor =>
                {
                    elevationContext.Execute(ResetInstallationIdElevatedCommand.ElevatedCommandId, null, progressMonitor);
                });

                return true;
            }, "Administrative access required to reset the installation id.");
            return result ? 0 : 1;
        }
    }
}
