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
using Gallio.Runtime.Diagnostics;
using Gallio.Runtime.Logging;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// A <see cref="ILogger" /> implementation that logs messages to the specified <see cref="TestLogStreamWriter" />.
    /// This can be used to write log messages to the test execution log.
    /// </summary>
    public sealed class TestLogStreamLogger : BaseLogger
    {
        private readonly TestLogStreamWriter writer;

        /// <summary>
        /// Creates a logger for the log stream writer.
        /// </summary>
        /// <param name="writer">The log stream writer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public TestLogStreamLogger(TestLogStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            message = String.Format("[{0}] {1}", severity.ToString().ToLowerInvariant(), message);

            if (exceptionData != null)
                writer.WriteException(exceptionData, message);
            else
                writer.WriteLine(message);
        }
    }
}