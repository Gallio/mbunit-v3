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
using System.Globalization;
using System.Text;
using MbUnit.Framework.Services.ExecutionLogs;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// Writes execution logs to an Xml-serializable format.
    /// </summary>
    public class ExecutionLogWriter : IExecutionLogWriter
    {
        private ExecutionLog executionLog;
        private Dictionary<string, ExecutionLogStreamData> streamDataMap;
        private Dictionary<string, Attachment> attachmentMap;

        /// <summary>
        /// Creates an execution log writer that builds a new execution log.
        /// </summary>
        public ExecutionLogWriter()
        {
            executionLog = new ExecutionLog();
            streamDataMap = new Dictionary<string, ExecutionLogStreamData>();
            attachmentMap = new Dictionary<string, Attachment>();
        }

        /// <summary>
        /// Gets the execution log under construction.
        /// </summary>
        public ExecutionLog ExecutionLog
        {
            get { return executionLog; }
        }

        /// <inheritdoc />
        public void WriteText(string streamName, string text)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            if (text == null)
                throw new ArgumentNullException("text");

            lock (this)
            {
                ThrowIfClosed();

                GetStreamData(streamName).WriteText(text);
            }
        }

        /// <inheritdoc />
        public void WriteAttachment(string streamName, Attachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");

            lock (this)
            {
                ThrowIfClosed();

                Attachment existingAttachment;
                if (attachmentMap.TryGetValue(attachment.Name, out existingAttachment))
                {
                    if (attachment != existingAttachment)
                        throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                            "Attempted to add a different attachment instance with the name '{0}'.", attachment.Name));
                }
                else
                {
                    attachmentMap.Add(attachment.Name, attachment);
                    executionLog.Attachments.Add(ExecutionLogAttachment.XmlSerialize(attachment));
                }

                if (streamName != null)
                {
                    GetStreamData(streamName).WriteAttachment(attachment.Name);
                }
            }
        }

        /// <inheritdoc />
        public void BeginSection(string streamName, string sectionName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            lock (this)
            {
                ThrowIfClosed();

                GetStreamData(streamName).BeginSection(sectionName);
            }
        }

        /// <inheritdoc />
        public void EndSection(string streamName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");

            lock (this)
            {
                ThrowIfClosed();

                GetStreamData(streamName).EndSection();
            }
        }

        /// <inheritdoc />
        public void Close()
        {
            lock (this)
            {
                if (streamDataMap == null)
                    return; // already closed

                foreach (ExecutionLogStreamData entry in streamDataMap.Values)
                    entry.Flush();

                streamDataMap = null;
                attachmentMap = null;
            }
        }

        private ExecutionLogStreamData GetStreamData(string streamName)
        {
            ExecutionLogStreamData streamData;
            if (!streamDataMap.TryGetValue(streamName, out streamData))
            {
                streamData = new ExecutionLogStreamData(streamName);
                streamDataMap.Add(streamName, streamData);

                executionLog.Streams.Add(streamData.ExecutionLogStream);
            }

            return streamData;
        }

        private void ThrowIfClosed()
        {
            if (streamDataMap == null)
                throw new InvalidOperationException("Cannot perform this operation because the execution log writer has been closed.");
        }

        private class ExecutionLogStreamData
        {
            private ExecutionLogStream executionLogStream;
            private Stack<ExecutionLogStreamContainerTag> containerStack;
            private StringBuilder textBuilder;

            public ExecutionLogStreamData(string streamName)
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

            public void WriteText(string text)
            {
                textBuilder.Append(text);
            }

            public void WriteAttachment(string attachmentName)
            {
                Flush();

                containerStack.Peek().Contents.Add(new ExecutionLogStreamEmbedTag(attachmentName));
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
        }
    }
}
