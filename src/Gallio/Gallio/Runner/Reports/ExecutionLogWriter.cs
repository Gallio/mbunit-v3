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
using System.Collections.Generic;
using System.Text;
using Gallio.Model.Execution;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Writes execution logs to an Xml-serializable format.
    /// </summary>
    public sealed class ExecutionLogWriter : BaseTestLogWriter
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
        protected override void CloseImpl()
        {
            if (streamWriters != null)
            {
                foreach (ExecutionLogStreamWriter writer in streamWriters.Values)
                    writer.Flush();

                streamWriters = null;
            }
        }

        /// <inheritdoc />
        protected override void AttachTextImpl(string attachmentName, string contentType, string text)
        {
            executionLog.Attachments.Add(ExecutionLogAttachment.CreateTextAttachment(attachmentName, contentType, text));
        }

        /// <inheritdoc />
        protected override void AttachBytesImpl(string attachmentName, string contentType, byte[] bytes)
        {
            executionLog.Attachments.Add(ExecutionLogAttachment.CreateBinaryAttachment(attachmentName, contentType, bytes));
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
        protected override void EndSectionImpl(string streamName)
        {
            GetLogStreamWriter(streamName).EndSection();
        }

        /// <inheritdoc />
        private ExecutionLogStreamWriter GetLogStreamWriter(string streamName)
        {
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

            streamWriter = new ExecutionLogStreamWriter(streamName);
            streamWriters.Add(streamName, streamWriter);

            executionLog.Streams.Add(streamWriter.ExecutionLogStream);
            return streamWriter;
        }

        private sealed class ExecutionLogStreamWriter
        {
            private readonly ExecutionLogStream executionLogStream;
            private readonly Stack<ExecutionLogStreamContainerTag> containerStack;
            private readonly StringBuilder textBuilder;

            public ExecutionLogStreamWriter(string streamName)
            {
                executionLogStream = new ExecutionLogStream(streamName);
                containerStack = new Stack<ExecutionLogStreamContainerTag>();
                textBuilder = new StringBuilder();

                containerStack.Push(executionLogStream.Body);
            }

            public ExecutionLogStream ExecutionLogStream
            {
                get { return executionLogStream; }
            }

            public void Flush()
            {
                if (textBuilder.Length != 0)
                {
                    containerStack.Peek().Contents.Add(new ExecutionLogStreamTextTag(textBuilder.ToString()));
                    textBuilder.Length = 0;
                }
            }

            public void Write(string text)
            {
                textBuilder.Append(text);
            }

            public void BeginSection(string sectionName)
            {
                Flush();

                ExecutionLogStreamSectionTag tag = new ExecutionLogStreamSectionTag(sectionName);
                containerStack.Peek().Contents.Add(tag);
                containerStack.Push(tag);
            }

            public void EndSection()
            {
                if (containerStack.Count == 1)
                    throw new InvalidOperationException("There is no current section to be ended.");

                Flush();
                containerStack.Pop();
            }

            public void Embed(string attachmentName)
            {
                Flush();
                containerStack.Peek().Contents.Add(new ExecutionLogStreamEmbedTag(attachmentName));
            }
        }
    }
}
