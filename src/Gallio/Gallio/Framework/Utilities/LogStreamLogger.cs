// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.Logging;
using Gallio.Framework;

namespace Gallio.Framework.Utilities
{
    /// <summary>
    /// A <see cref="ILogger" /> implementation that logs messages to the specified <see cref="LogStreamWriter" />.
    /// This can be used to write log messages to the test execution log.
    /// </summary>
    public sealed class LogStreamLogger : BaseLogger
    {
        private readonly LogStreamWriter writer;

        /// <summary>
        /// Creates a logger for the default log stream.
        /// </summary>
        public LogStreamLogger()
            : this(Gallio.Framework.Log.Default)
        {
        }

        /// <summary>
        /// Creates a logger for the specified log stream.
        /// </summary>
        /// <param name="writer">The log stream writer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public LogStreamLogger(LogStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, Exception exception)
        {
            message = String.Format("[{0}] {1}", severity.ToString().ToLowerInvariant(), message);

            if (exception != null)
                writer.WriteException(exception, message);
            else
                writer.WriteLine(message);
        }
    }
}