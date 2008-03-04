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
using System.IO;

namespace Gallio.Logging
{
    /// <summary>
    /// <para>
    /// The text log writer is a simplified implementation of <see cref="LogWriter" />
    /// that forwards log writing activites to a text writer.  It discards any attachments
    /// but writes messages to show where they would have been embedded.
    /// </para>
    /// </summary>
    public class TextLogWriter : TrackingLogWriter
    {
        private readonly TextWriter textWriter;

        /// <summary>
        /// Creates a text log writer.
        /// </summary>
        /// <param name="textWriter">The text writer to which log output should be written</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="textWriter"/> is null</exception>
        public TextLogWriter(TextWriter textWriter)
        {
            if (textWriter == null)
                throw new ArgumentNullException("textWriter");

            this.textWriter = textWriter;
        }

        /// <inheritdoc />
        protected override LogStreamWriter GetLogStreamWriterImpl(string streamName)
        {
            return new StubLogStreamWriter(textWriter, streamName);
        }

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            textWriter.WriteLine("[Attach '{0}': {1}]", attachment.Name, attachment.ContentType);
        }

        private sealed class StubLogStreamWriter : LogStreamWriter
        {
            private readonly TextWriter textWriter;

            public StubLogStreamWriter(TextWriter textWriter, string streamName)
                : base(streamName)
            {
                this.textWriter = textWriter;
            }

            protected override void WriteImpl(string text)
            {
                textWriter.Write(text);
            }

            protected override void EmbedImpl(Attachment attachment)
            {
                textWriter.WriteLine("[Embed '{0}': {1}]", attachment.Name, attachment.ContentType);
            }

            protected override void EmbedExistingImpl(string attachmentName)
            {
                textWriter.WriteLine("[Embed '{0}']", attachmentName);
            }

            protected override void BeginSectionImpl(string sectionName)
            {
                textWriter.WriteLine("[Begin Section '{0}']", sectionName);
            }

            protected override void EndSectionImpl()
            {
                textWriter.WriteLine("[End Section]");
            }
        }
    }
}
