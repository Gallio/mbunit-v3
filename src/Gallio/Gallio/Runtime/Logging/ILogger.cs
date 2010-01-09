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
using Gallio.Common.Diagnostics;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Provides support for logging messages from system components.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="severity">The log message severity.</param>
        /// <param name="message">The log message.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        void Log(LogSeverity severity, string message);
        
        /// <summary>
        /// Logs a message with an associated exception.
        /// </summary>
        /// <param name="severity">The log message severity.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The associated exception, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        void Log(LogSeverity severity, string message, Exception exception);

        /// <summary>
        /// Logs a message with an associated exception.
        /// </summary>
        /// <param name="severity">The log message severity.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exceptionData">The associated exception data, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        void Log(LogSeverity severity, string message, ExceptionData exceptionData);
    }
}
