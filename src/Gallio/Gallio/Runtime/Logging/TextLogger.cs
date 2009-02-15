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
using System.IO;
using Gallio.Utilities;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// <para>
    /// A logger that writes output to a <see cref="TextWriter"/>.
    /// </para>
    /// </summary>
    public class TextLogger : BaseLogger
    {
        private readonly TextWriter textWriter;

        /// <summary>
        /// Creates a text logger.
        /// </summary>
        /// <param name="textWriter">The text writer to which the log output should be written</param>
        public TextLogger(TextWriter textWriter)
        {
            if (textWriter == null)
                throw new ArgumentNullException("textWriter");

            this.textWriter = textWriter;
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, Exception exception)
        {
            textWriter.WriteLine("[{0}] {1}", severity, message);
            if (exception != null)
            {
                textWriter.Write("  ");
                textWriter.WriteLine(ExceptionUtils.SafeToString(exception));
            }

            textWriter.Flush();
        }
    }
}
