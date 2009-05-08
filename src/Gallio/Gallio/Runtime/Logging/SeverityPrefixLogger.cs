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
using Gallio.Runtime.Diagnostics;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// The severity prefix logger wraps another loggers and encodes the severity of a
    /// log entry into the log message itself in a standard form.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This wrapper is useful when the base logger may not be able to
    /// report the severity via other means or when severity information must be
    /// reconstructed from plain text.
    /// </para>
    /// </remarks>
    /// <seealso cref="SeverityPrefixParser"/>
    public class SeverityPrefixLogger : BaseLogger
    {
        private readonly ILogger inner;

        internal static readonly Dictionary<LogSeverity, string> SeverityPrefixes = new Dictionary<LogSeverity, string>()
        {
            { LogSeverity.Debug, "[Debug] " },
            { LogSeverity.Info, "[Info] " },
            { LogSeverity.Important, "[Important] " },
            { LogSeverity.Warning, "[Warning] " },
            { LogSeverity.Error, "[Error] " },
        };

        /// <summary>
        /// Creates a prefix logger.
        /// </summary>
        /// <param name="inner">The inner logger</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null</exception>
        public SeverityPrefixLogger(ILogger inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            inner.Log(severity, SeverityPrefixes[severity] + message, exceptionData);
        }
    }
}
