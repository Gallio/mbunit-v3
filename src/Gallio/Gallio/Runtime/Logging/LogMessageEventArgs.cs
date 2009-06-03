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
using Gallio.Common.Diagnostics;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Event arguments for the <see cref="EventLogger"/>
    /// </summary>
    public class LogMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a log event.
        /// </summary>
        /// <param name="severity">The log severity.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exceptionData">The exception data, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        public LogMessageEventArgs(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Severity = severity;
            Message = message;
            ExceptionData = exceptionData;
        }

        /// <summary>
        /// Gets the log severity.
        /// </summary>
        public LogSeverity Severity { get; private set; }

        /// <summary>
        /// Gets the log message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the exception, or null if none.
        /// </summary>
        public ExceptionData ExceptionData { get; private set; }
    }
}
