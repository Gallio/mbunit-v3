// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.Text;
using MbUnit.Logging;

namespace MbUnit.Runner.Reports
{
    /// <summary>
    /// Writes execution logs to an Xml-serializable format.
    /// </summary>
    public sealed class ExecutionLogWriter : TrackingLogWriter
    {
        private readonly ExecutionLog executionLog;
        private Dictionary<string, ExecutionLogStreamWriter> streamWriters;

        /// <summary>
        /// Creates an execution log writer that builds a new execution log.
        /// </summary>
        public ExecutionLogWriter()
        {
            executionLog = new ExecutionLog();
        }

        /// <summary>
        /// Gets the execution log under construction.
        /// </summary>
        public ExecutionLog ExecutionLog
        {
            get { return executionLog; }
        }

        /// <inheritdoc />
        protected override LogStreamWriter GetLogStreamWriterImpl(string streamName)
        {
            lock (executionLog)
            {
                ThrowIfClosed();

                ExecutionLogStreamWriter streamWriter;
                if (streamWriters != null)
                {
                    if (streamWriters.TryGetValue(streamName, out streamWriter))
                        return streamWriter;
                }
                else
                {
                    streamWriters = new Dictionary<string, ExecutionLogStreamWriter>();
                }

                streamWriter = new ExecutionLogStreamWriter(this, streamName);
                streamWriters.Add(streamName, streamWriter);

                executionLog.Streams.Add(streamWriter.ExecutionLogStream);
                return streamWriter;
            }
        }

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            lock (executionLog)
            {
                ThrowIfClosed();

                InternalAttach(attachment);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            lock (executionLog)
            {
                base.Dispose(disposing);

                if (streamWriters != null)
                {
                    foreach (ExecutionLogStreamWriter writer in streamWriters.Values)
                        writer.InternalFlush();

                    streamWriters = null;
                }
            }
        }

        private void InternalAttach(Attachment attachment)
        {
            if (TrackAttachment(attachment))
                executionLog.Attachments.Add(new ExecutionLogAttachment(attachment));
        }

        private sealed class ExecutionLogStreamWriter : LogStreamWriter
        {
            private readonly ExecutionLogWriter logWriter;
            private readonly ExecutionLogStream executionLogStream;
            private readonly Stack<ExecutionLogStreamContainerTag> containerStack;
            private readonly StringBuilder textBuilder;

            public ExecutionLogStreamWriter(ExecutionLogWriter logWriter, string streamName)
                : base(streamName)
            {
                this.logWriter = logWriter;

                executionLogStream = new ExecutionLogStream(streamName);
                containerStack = new Stack<ExecutionLogStreamContainerTag>();
                textBuilder = new StringBuilder();

                containerStack.Push(executionLogStream.Body);
            }

            public ExecutionLogStream ExecutionLogStream
            {
                get { return executionLogStream; }
            }

            protected override void FlushImpl()
            {
                lock (logWriter.executionLog)
                {
                    logWriter.ThrowIfClosed();
                    InternalFlush();
                }
            }

            protected override void WriteImpl(string text)
            {
                lock (logWriter.executionLog)
                {
                    logWriter.ThrowIfClosed();
                    textBuilder.Append(text);
                }
            }

            protected override void BeginSectionImpl(string sectionName)
            {
                lock (logWriter.executionLog)
                {
                    logWriter.ThrowIfClosed();
                    InternalFlush();

                    ExecutionLogStreamSectionTag tag = new ExecutionLogStreamSectionTag(sectionName);
                    containerStack.Peek().Contents.Add(tag);
                    containerStack.Push(tag);
                }
            }

            protected override void EndSectionImpl()
            {
                lock (logWriter.executionLog)
                {
                    logWriter.ThrowIfClosed();

                    if (containerStack.Count == 1)
                        throw new InvalidOperationException("There is no current section to be ended.");

                    InternalFlush();
                    containerStack.Pop();
                }
            }

            protected override void EmbedImpl(Attachment attachment)
            {
                lock (logWriter.executionLog)
                {
                    logWriter.ThrowIfClosed();
                    logWriter.InternalAttach(attachment);

                    InternalEmbedExisting(attachment.Name);
                }
            }

            protected override void EmbedExistingImpl(string attachmentName)
            {
                lock (logWriter.executionLog)
                {
                    logWriter.ThrowIfClosed();
                    logWriter.VerifyAttachmentExists(attachmentName);

                    InternalEmbedExisting(attachmentName);
                }
            }

            private void InternalEmbedExisting(string attachmentName)
            {
                InternalFlush();
                containerStack.Peek().Contents.Add(new ExecutionLogStreamEmbedTag(attachmentName));
            }

            public void InternalFlush()
            {
                if (textBuilder.Length != 0)
                {
                    containerStack.Peek().Contents.Add(new ExecutionLogStreamTextTag(textBuilder.ToString()));
                    textBuilder.Length = 0;
                }
            }
        }
    }
}
