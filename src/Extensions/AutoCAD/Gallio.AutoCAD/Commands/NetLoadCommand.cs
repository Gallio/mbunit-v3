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
using System.IO;
using System.Reflection;
using Gallio.Runtime.Logging;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// Maps to the <c>NETLOAD</c> command.
    /// </summary>
    public class NetLoadCommand : AcadCommand
    {
        private readonly ILogger logger;
        private readonly IAcadPluginLocator pluginLocator;

        /// <summary>
        /// Initializes a new <see cref="NetLoadCommand"/> object.
        /// </summary>
        public NetLoadCommand(ILogger logger, IAcadPluginLocator pluginLocator)
            : base("NETLOAD")
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (pluginLocator == null)
                throw new ArgumentNullException("pluginLocator");
            this.logger = logger;
            this.pluginLocator = pluginLocator;
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetArgumentsImpl(object application)
        {
            var version = GetAcadVersion(application);
            var path = pluginLocator.GetPluginPath(version);
            yield return path;
        }

        private string GetAcadVersion(object application)
        {
            try
            {
                return application.GetType().InvokeMember(@"Version", BindingFlags.GetProperty, null, application, null) as string;
            }
            catch (MemberAccessException e)
            {
                logger.Log(LogSeverity.Warning, "Unable to retrieve AutoCAD version.", e);
            }
            return null;
        }
    }
}
