// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using System.Management.Automation;
using Castle.Core.Logging;

namespace Gallio.PowerShellCommands
{
    /// <summary>
    /// Abstract base class for PowerShell commands.
    /// Provides some useful runtime support.
    /// </summary>
    public abstract class BaseCommand : PSCmdlet
    {
        private CmdletLogger logger;

        /// <summary>
        /// Gets the logger for the cmdlet.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                if (logger == null)
                    logger = new CmdletLogger(this);
                return logger;
            }
        }
    }
}
