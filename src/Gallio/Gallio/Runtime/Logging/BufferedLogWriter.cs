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
using System.IO;
using System.Text;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// A buffered log writer is an adapter for <see cref="TextWriter" /> that writes
    /// long full lines to a <see cref="ILogger" />.
    /// </summary>
    public class BufferedLogWriter : TextWriter
    {
        private readonly ILogger logger;
        private readonly LogSeverity logSeverity;
        private readonly StringBuilder logBuffer;
        private readonly Encoding encoding;

        /// <summary>
        /// Creates a buffered log writer that reports a <see cref="System.Text.Encoding.Unicode"/> encoding.
        /// </summary>
        /// <param name="logger">The logger to which messages are written.</param>
        /// <param name="logSeverity">The log severity for messages.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        public BufferedLogWriter(ILogger logger, LogSeverity logSeverity)
            : this(logger, logSeverity, Encoding.Unicode)
        {
        }

        /// <summary>
        /// Creates a buffered log writer that reports a particular encoding.
        /// </summary>
        /// <param name="logger">The logger to which messages are written.</param>
        /// <param name="logSeverity">The log severity for messages.</param>
        /// <param name="encoding">The encoding to report.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> 
        /// or <paramref name="encoding"/> is null.</exception>
        public BufferedLogWriter(ILogger logger, LogSeverity logSeverity, Encoding encoding)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            this.logger = logger;
            this.logSeverity = logSeverity;
            this.encoding = encoding;

            NewLine = "\n";
            logBuffer = new StringBuilder();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Flush();
        }

        /// <inheritdoc />
        public override Encoding Encoding
        {
            get { return encoding; }
        }

        /// <inheritdoc cref="TextWriter.NewLine" />
        new public string NewLine
        {
            get { return base.NewLine; }
            private set { base.NewLine = value; }
        }

        /// <inheritdoc />
        public sealed override void Flush()
        {
            WriteLine();
        }

        /// <inheritdoc />
        public sealed override void Write(string value)
        {
            if (value != null)
            {
                logBuffer.Append(value);
                FlushIfBufferEndsWithNewLine();
            }
        }

        /// <inheritdoc />
        public sealed override void Write(char[] buffer, int index, int count)
        {
            logBuffer.Append(buffer, index, count);
            FlushIfBufferEndsWithNewLine();
        }

        /// <inheritdoc />
        public sealed override void WriteLine(object value)
        {
            Write(value);
            WriteLine();
        }

        private void FlushIfBufferEndsWithNewLine()
        {
            int length = logBuffer.Length;
            if (length != 0 && logBuffer[length - 1] == '\n')
            {
                // Trim End
                do
                {
                    length -= 1;
                }
                while (length != 0 && char.IsWhiteSpace(logBuffer[length - 1]));

                // Trim Start
                int start = 0;
                while (start < length && char.IsWhiteSpace(logBuffer[start]))
                    start += 1;

                // Write Middle
                if (length != 0)
                {
                    logger.Log(logSeverity, logBuffer.ToString(start, length - start));
                    logBuffer.Length = 0;
                }
            }
        }
    }
}
