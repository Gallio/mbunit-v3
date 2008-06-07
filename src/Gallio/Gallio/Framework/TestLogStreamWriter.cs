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
    /// An implementation of <see cref="LogStreamWriter" /> that writes to a
    /// particular stream of a <see cref="ITestLogWriter" />.
    /// </summary>
    [Serializable]
    public sealed class TestLogStreamWriter : LogStreamWriter
    {
        private readonly ITestLogWriter logWriter;

        /// <summary>
        /// Creates the log stream writer.
        /// </summary>
        /// <param name="logWriter">The underlying test log writer</param>
        /// <param name="streamName">The stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logWriter"/>
        /// or <paramref name="streamName"/> is null</exception>
        public TestLogStreamWriter(ITestLogWriter logWriter, string streamName)
            : base(streamName)
        {
            if (logWriter == null)
                throw new ArgumentNullException("logWriter");

            this.logWriter = logWriter;
        }

        /// <inheritdoc />
        protected override void WriteImpl(string text)
        {
            logWriter.Write(StreamName, text);
        }

        /// <inheritdoc />
        protected override void EmbedImpl(Attachment attachment)
        {
            attachment.Attach(logWriter);
            EmbedExistingImpl(attachment.Name);
        }

        /// <inheritdoc />
        protected override void EmbedExistingImpl(string attachmentName)
        {
            logWriter.Embed(StreamName, attachmentName);
        }

        /// <inheritdoc />
        protected override void BeginSectionImpl(string sectionName)
        {
            logWriter.BeginSection(StreamName, sectionName);
        }

        /// <inheritdoc />
        protected override void EndSectionImpl()
        {
            logWriter.EndSection(StreamName);
        }
    }
}
