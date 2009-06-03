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
using Gallio.Common.Diagnostics;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Filters another logger to exclude messages below a given level of severity.
    /// </summary>
    public class FilteredLogger : BaseLogger
    {
        private readonly ILogger logger;
        private readonly LogSeverity minSeverity;

        /// <summary>
        /// Creates a filtered logger.
        /// </summary>
        /// <param name="logger">The logger to which filtered log messages are sent.</param>
        /// <param name="minSeverity">The lowest severity message type to retain.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        public FilteredLogger(ILogger logger, LogSeverity minSeverity)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.logger = logger;
            this.minSeverity = minSeverity;
        }

        /// <summary>
        /// Creates a filtered logger.
        /// </summary>
        /// <param name="logger">The logger to which filtered log messages are sent.</param>
        /// <param name="verbosity">The verbosity to use.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        public FilteredLogger(ILogger logger, Verbosity verbosity)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            this.logger = logger;

            switch (verbosity)
            {
                case Verbosity.Quiet:
                    minSeverity = LogSeverity.Warning;
                    break;
                case Verbosity.Verbose:
                    minSeverity = LogSeverity.Info;
                    break;
                case Verbosity.Debug:
                    minSeverity = LogSeverity.Debug;
                    break;
                default:
                    minSeverity = LogSeverity.Important;
                    break;
            }
        }

        /// <summary>
        /// Gets the minimum severity of the messages that are included in the filtered log.
        /// </summary>
        public LogSeverity MinSeverity
        {
            get { return minSeverity; }
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            if (severity >= minSeverity)
                logger.Log(severity, message, exceptionData);
        }
    }
}
