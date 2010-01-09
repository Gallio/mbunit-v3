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
using Gallio.Common.Concurrency;
using Gallio.Common.Policies;
using Gallio.Common.Diagnostics;
using System.Collections.Generic;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// A logger that dispatches log messages via events and listeners.
    /// </summary>
    public class DispatchLogger : BaseLogger
    {
        private readonly LockBox<Data> dataBox = new LockBox<Data>(new Data());

        /// <summary>
        /// An event that is fired when a log message is received.
        /// </summary>
        public event EventHandler<LogEntrySubmittedEventArgs> LogMessage
        {
            add { dataBox.Write(data => data.LogMessage += value); }
            remove { dataBox.Write(data => data.LogMessage -= value); }
        }

        /// <summary>
        /// Adds a log listener that will receive log messages dispatched to the runtime logger.
        /// </summary>
        /// <param name="logger">The log listener to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        public void AddLogListener(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            dataBox.Write(data => data.Listeners.Add(logger));
        }

        /// <summary>
        /// Removes a log listener so that it will no longer receive log messages dispatched to the runtime logger.
        /// </summary>
        /// <param name="logger">The log listener to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        public void RemoveLogListener(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            dataBox.Write(data => data.Listeners.Remove(logger));
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            dataBox.Read(data => data.Dispatch(severity, message, exceptionData));
        }

        private sealed class Data
        {
            public readonly List<ILogger> Listeners = new List<ILogger>();
            public event EventHandler<LogEntrySubmittedEventArgs> LogMessage;

            public void Dispatch(LogSeverity severity, string message, ExceptionData exceptionData)
            {
                EventHandlerPolicy.SafeInvoke(LogMessage, this, new LogEntrySubmittedEventArgs(severity, message, exceptionData));

                foreach (ILogger logger in Listeners)
                {
                    try
                    {
                        logger.Log(severity, message, exceptionData);
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report(
                            "An exception occurred while dispatching a message to a log listener.", ex);
                    }
                }
            }
        }
    }
}
