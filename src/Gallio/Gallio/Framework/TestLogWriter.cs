// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Execution;

namespace Gallio.Framework
{
    /// <summary>
    /// An implementation of <see cref="LogWriter" /> that writes to a
    /// particular <see cref="ITestLogWriter" />.
    /// </summary>
    public sealed class TestLogWriter : LogWriter
    {
        private readonly ITestLogWriter logWriter;

        /// <summary>
        /// Creates the log writer.
        /// </summary>
        /// <param name="logWriter">The underlying test log writer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logWriter"/> is null</exception>
        public TestLogWriter(ITestLogWriter logWriter)
        {
            if (logWriter == null)
                throw new ArgumentNullException("logWriter");

            this.logWriter = logWriter;
        }

        /// <inheritdoc />
        protected override LogStreamWriter GetLogStreamWriterImpl(string streamName)
        {
            return new TestLogStreamWriter(logWriter, streamName);
        }

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            attachment.Attach(logWriter);
        }
    }
}
