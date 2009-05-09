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
using System.Management.Automation;
using Gallio.Common.Diagnostics;
using Gallio.Runtime.Logging;

namespace Gallio.PowerShellCommands
{
    /// <exclude />
    /// <summary>
    /// Logs messages using a <see cref="BaseCommand" />'s logging functions.
    /// </summary>
    internal class CommandLogger : BaseLogger
    {
        private readonly BaseCommand cmdlet;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLogger" /> class.
        /// </summary>
        /// <param name="cmdlet">The <see cref="Cmdlet" /> instance to channel
        /// log messages to.</param>
        public CommandLogger(BaseCommand cmdlet)
        {
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            this.cmdlet = cmdlet;
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="severity">The log message severity.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exceptionData">The exception to log (it can be null).</param>
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            // The PowerShell logging methods may throw InvalidOperationException
            // or NotImplementedException if the PowerShell host is not connected
            // or does not support the requested service.  So we eat those exceptions.
            cmdlet.PostMessage(delegate
            {
                try
                {
                    if (exceptionData != null)
                        message = string.Concat(message, "\n", exceptionData.ToString());

                    switch (severity)
                    {
                        case LogSeverity.Error:
                            cmdlet.WriteError(new ErrorRecord(new Exception(message), "Error", ErrorCategory.NotSpecified, "Gallio"));
                            break;

                        case LogSeverity.Warning:
                            cmdlet.WriteWarning(message);
                            break;

                        case LogSeverity.Important:
                        case LogSeverity.Info:
                            cmdlet.WriteVerbose(message);
                            break;

                        case LogSeverity.Debug:
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
            });
        }
    }
}
