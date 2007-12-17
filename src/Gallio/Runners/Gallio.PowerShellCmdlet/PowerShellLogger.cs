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

using System;
using System.Management.Automation;
using Castle.Core.Logging;

namespace Gallio.PowerShellCmdlet
{
    ///<summary>
    /// Logs messages to a <see cref="GallioCmdlet" /> instance.
    ///</summary>
    public class PowerShellLogger : LevelFilteredLogger
    {
        private readonly GallioCmdlet cmdletLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellLogger" /> class.
        /// </summary>
        /// <param name="cmdletLogger">The <see cref="GallioCmdlet" /> instance to channel
        /// log messages to.</param>
        public PowerShellLogger(GallioCmdlet cmdletLogger)
        {
            if (cmdletLogger == null)
                throw new ArgumentNullException("cmdletLogger");
            this.cmdletLogger = cmdletLogger;
            Level = LoggerLevel.Debug;
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="name">Not used.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log (it can be null).</param>
        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            switch (level)
            {
                case LoggerLevel.Fatal:
                case LoggerLevel.Error:
                    if (exception == null)
                    {
                        exception = new Exception(message);
                    }
                    cmdletLogger.WriteError(
                        new ErrorRecord
                        (
                            exception,
                            message,
                            ErrorCategory.NotSpecified,
                            "GallioCmdlet"
                        ));
                    break;

                // For some reason calling WriteWarning, WriteVerbose or WriteDebug
                // cause PowerShell to crash, so we ignore other message types.
            }
        }

        /// <inheritdoc />
        public override ILogger CreateChildLogger(string name)
        {
            //TODO: Check why are we ignoring the name
            return new PowerShellLogger(cmdletLogger);
        }
    }
}
