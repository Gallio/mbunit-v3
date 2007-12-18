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

namespace Gallio.PowerShellCommands
{
    /// <exclude />
    /// <summary>
    /// Logs messages using a <see cref="Cmdlet" />'s logging functions.
    /// </summary>
    internal class CmdletLogger : LevelFilteredLogger
    {
        private readonly Cmdlet cmdlet;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdletLogger" /> class.
        /// </summary>
        /// <param name="cmdlet">The <see cref="Cmdlet" /> instance to channel
        /// log messages to.</param>
        public CmdletLogger(Cmdlet cmdlet)
        {
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            this.cmdlet = cmdlet;
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
            // The PowerShell logging methods may throw InvalidOperationException
            // or NotImplementedException if the PowerShell host is not connected
            // or does not support the requested service.  So we eat those exceptions.
            try
            {
                switch (level)
                {
                    case LoggerLevel.Fatal:
                    case LoggerLevel.Error:
                        if (exception == null)
                            exception = new Exception(message);

                        cmdlet.WriteError(new ErrorRecord(exception, message, ErrorCategory.NotSpecified, "Gallio"));
                        break;

                    case LoggerLevel.Warn:
                        cmdlet.WriteWarning(message);
                        break;

                    case LoggerLevel.Info:
                        cmdlet.WriteVerbose(message);
                        break;

                    case LoggerLevel.Debug:
                        cmdlet.WriteDebug(message);
                        break;
                }
            }
            catch (NotImplementedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <inheritdoc />
        public override ILogger CreateChildLogger(string name)
        {
            // We ignore the name because we do not use the child logger
            // implementation pattern in Gallio.
            return new CmdletLogger(cmdlet);
        }
    }
}
