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
using Gallio.Common.Markup;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// A <see cref="ILogger" /> implementation that logs messages to the specified <see cref="MarkupStreamWriter" />.
    /// This can be used to write log messages to a markup stream.
    /// </summary>
    public sealed class MarkupStreamLogger : BaseLogger
    {
        private readonly MarkupStreamWriter writer;

        /// <summary>
        /// Creates a logger for the markup stream writer.
        /// </summary>
        /// <param name="writer">The markup stream writer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public MarkupStreamLogger(MarkupStreamWriter writer)
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