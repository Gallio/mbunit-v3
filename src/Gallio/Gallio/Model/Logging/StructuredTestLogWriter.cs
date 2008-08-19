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
using System.Collections.Generic;
using System.Text;
using Gallio.Model.Logging;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// Writes a <see cref="Logging.StructuredTestLog" /> in memory.
    /// </summary>
    public class StructuredTestLogWriter : TestLogWriter
    {
        private readonly StructuredTestLog testLog;
        private Dictionary<string, StreamState> streamWriters;

        /// <summary>
        /// Creates an log writer that builds a new <see cref="Logging.StructuredTestLog" />.
        /// </summary>
        public StructuredTestLogWriter()
        {
            testLog = new StructuredTestLog();
        }

        /// <summary>
        /// Gets the test log under construction.
        /// </summary>
        /// <remarks>
        /// The contents of the test log will change as more text is written.
        /// </remarks>
        public StructuredTestLog TestLog
        {
            get { return testLog; }
        }

        /// <inheritdoc />
        protected override void CloseImpl()
        {
            if (streamWriters != null)
            {
                foreach (StreamState writer in streamWriters.Values)
                    writer.Flush();

                streamWriters = null;
            }
        }

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            testLog.Attachments.Add(attachment.ToAttachmentData());
        }

        /// <inheritdoc />
        protected override void StreamWriteImpl(string streamName, string text)
        {
            GetLogStreamWriter(streamName).Write(text);
        }

        /// <inheritdoc />
        protected override void StreamEmbedImpl(string streamName, string attachmentName)
        {
            GetLogStreamWriter(streamName).Embed(attachmentName);
        }

        /// <inheritdoc />
        protected override void StreamBeginSectionImpl(string streamName, string sectionName)
        {
            GetLogStreamWriter(streamName).BeginSection(sectionName);
        }

        /// <inheritdoc />
        protected override void StreamBeginMarkerImpl(string streamName, Marker marker)
        {
            GetLogStreamWriter(streamName).BeginMarker(marker);
        }

        /// <inheritdoc />
        protected override void StreamEndImpl(string streamName)
        {
            GetLogStreamWriter(streamName).End();
        }

        /// <inheritdoc />
        protected override void StreamFlushImpl(string streamName)
        {
            GetLogStreamWriter(streamName).Flush();
        }

        /// <inheritdoc />
        private StreamState GetLogStreamWriter(string streamName)
        {
            StreamState streamState;
            if (streamWriters != null)
            {
                if (streamWriters.TryGetValue(streamName, out streamState))
                    return streamState;
            }
            else
            {
                streamWriters = new Dictionary<string, StreamState>();
            }

            streamState = new StreamState(streamName);
            streamWriters.Add(streamName, streamState);

            testLog.Streams.Add(streamState.TestLogStream);
            return streamState;
        }

        private sealed class StreamState
        {
            private readonly StructuredTestLogStream testLogStream;
            private readonly Stack<StructuredTestLogStream.ContainerTag> containerStack;
            private readonly StringBuilder textBuilder;

            public StreamState(string streamName)
            {
                testLogStream = new StructuredTestLogStream(streamName);
                containerStack = new Stack<StructuredTestLogStream.ContainerTag>();
                textBuilder = new StringBuilder();

                containerStack.Push(testLogStream.Body);
            }

            public StructuredTestLogStream TestLogStream
            {
                get { return testLogStream; }
            }

            public void Flush()
            {
                // Normalize newlines before writing out the tags.
                textBuilder.Replace("\r", "");

                if (textBuilder.Length != 0)
                {
                    containerStack.Peek().Contents.Add(new StructuredTestLogStream.TextTag(textBuilder.ToString()));
                    textBuilder.Length = 0;
                }
            }

            public void Write(string text)
            {
                textBuilder.Append(text);
            }

            public void BeginSection(string sectionName)
            {
                Begin(new StructuredTestLogStream.SectionTag(sectionName));
            }

            public void BeginMarker(Marker marker)
            {
                Begin(new StructuredTestLogStream.MarkerTag(marker));
            }

            private void Begin(StructuredTestLogStream.ContainerTag tag)
            {
                Flush();

                containerStack.Peek().Contents.Add(tag);
                containerStack.Push(tag);
            }

            public void End()
            {
                if (containerStack.Count == 1)
                    throw new InvalidOperationException("There is no current section to be ended.");

                Flush();
                containerStack.Pop();
            }

            public void Embed(string attachmentName)
            {
                Flush();
                containerStack.Peek().Contents.Add(new StructuredTestLogStream.EmbedTag(attachmentName));
            }
        }
    }
}