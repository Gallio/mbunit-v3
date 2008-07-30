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

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Writes test logs to an in-memory Xml-serializable format.
    /// </summary>
    public sealed class TestLogWriter : UnifiedTestLogWriter
    {
        private readonly TestLog testLog;
        private Dictionary<string, TestLogStreamWriter> streamWriters;

        /// <summary>
        /// Creates an test log writer that builds a new test log.
        /// </summary>
        public TestLogWriter()
        {
            testLog = new TestLog();
        }

        /// <summary>
        /// Gets the test log under construction.
        /// </summary>
        public TestLog TestLog
        {
            get { return testLog; }
        }

        /// <inheritdoc />
        protected override void CloseImpl()
        {
            if (streamWriters != null)
            {
                foreach (TestLogStreamWriter writer in streamWriters.Values)
                    writer.Flush();

                streamWriters = null;
            }
        }

        /// <inheritdoc />
        protected override void AttachTextImpl(string attachmentName, string contentType, string text)
        {
            testLog.Attachments.Add(TestLogAttachment.CreateTextAttachment(attachmentName, contentType, text));
        }

        /// <inheritdoc />
        protected override void AttachBytesImpl(string attachmentName, string contentType, byte[] bytes)
        {
            testLog.Attachments.Add(TestLogAttachment.CreateBinaryAttachment(attachmentName, contentType, bytes));
        }

        /// <inheritdoc />
        protected override void WriteImpl(string streamName, string text)
        {
            GetLogStreamWriter(streamName).Write(text);
        }

        /// <inheritdoc />
        protected override void EmbedImpl(string streamName, string attachmentName)
        {
            GetLogStreamWriter(streamName).Embed(attachmentName);
        }

        /// <inheritdoc />
        protected override void BeginSectionImpl(string streamName, string sectionName)
        {
            GetLogStreamWriter(streamName).BeginSection(sectionName);
        }

        /// <inheritdoc />
        protected override void BeginMarkerImpl(string streamName, string @class)
        {
            GetLogStreamWriter(streamName).BeginMarker(@class);
        }

        /// <inheritdoc />
        protected override void EndImpl(string streamName)
        {
            GetLogStreamWriter(streamName).End();
        }

        /// <inheritdoc />
        private TestLogStreamWriter GetLogStreamWriter(string streamName)
        {
            TestLogStreamWriter streamWriter;
            if (streamWriters != null)
            {
                if (streamWriters.TryGetValue(streamName, out streamWriter))
                    return streamWriter;
            }
            else
            {
                streamWriters = new Dictionary<string, TestLogStreamWriter>();
            }

            streamWriter = new TestLogStreamWriter(streamName);
            streamWriters.Add(streamName, streamWriter);

            testLog.Streams.Add(streamWriter.TestLogStream);
            return streamWriter;
        }

        private sealed class TestLogStreamWriter
        {
            private readonly TestLogStream testLogStream;
            private readonly Stack<TestLogStreamContainerTag> containerStack;
            private readonly StringBuilder textBuilder;

            public TestLogStreamWriter(string streamName)
            {
                testLogStream = new TestLogStream(streamName);
                containerStack = new Stack<TestLogStreamContainerTag>();
                textBuilder = new StringBuilder();

                containerStack.Push(testLogStream.Body);
            }

            public TestLogStream TestLogStream
            {
                get { return testLogStream; }
            }

            public void Flush()
            {
                if (textBuilder.Length != 0)
                {
                    containerStack.Peek().Contents.Add(new TestLogStreamTextTag(textBuilder.ToString()));
                    textBuilder.Length = 0;
                }
            }

            public void Write(string text)
            {
                textBuilder.Append(text);
            }

            public void BeginSection(string sectionName)
            {
                Begin(new TestLogStreamSectionTag(sectionName));
            }

            public void BeginMarker(string @class)
            {
                Begin(new TestLogStreamMarkerTag(@class));
            }

            private void Begin(TestLogStreamContainerTag tag)
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
                containerStack.Peek().Contents.Add(new TestLogStreamEmbedTag(attachmentName));
            }
        }
    }
}
