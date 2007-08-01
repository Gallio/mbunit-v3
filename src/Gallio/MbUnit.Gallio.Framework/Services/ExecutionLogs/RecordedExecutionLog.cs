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
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework.Services.ExecutionLogs;

namespace MbUnit.Framework.Services.ExecutionLogs
{
    /// <summary>
    /// An implementation of <see cref="IExecutionLog" /> that uses a <see cref="IExecutionLogWriter" />
    /// to write the execution log.
    /// </summary>
    public class RecordedExecutionLog : IExecutionLog
    {
        private IExecutionLogWriter writer;
        private RecordedExecutionLogStreamCollection streams;

        /// <summary>
        /// Creates an execution log that forwards writing operations
        /// to the specified execution log writer.
        /// </summary>
        /// <param name="writer">The execution log writer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public RecordedExecutionLog(IExecutionLogWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
            streams = new RecordedExecutionLogStreamCollection(this);
        }

        /// <summary>
        /// Gets the execution log writer.
        /// </summary>
        public IExecutionLogWriter Writer
        {
            get { return writer; }
        }

        /// <inheritdoc />
        public IExecutionLogStreamCollection Streams
        {
            get { return streams; }
        }

        /// <inheritdoc />
        public Attachment Attach(Attachment attachment)
        {
            writer.WriteAttachment(null, attachment);
            return attachment;
        }

        /// <inheritdoc />
        public TextAttachment AttachPlainText(string text)
        {
            return (TextAttachment)Attach(AttachmentUtils.CreatePlainTextAttachment(null, text));
        }

        /// <inheritdoc />
        public TextAttachment AttachHtml(string html)
        {
            return (TextAttachment)Attach(AttachmentUtils.CreateHtmlAttachment(null, html));
        }

        /// <inheritdoc />
        public XmlAttachment AttachXHtml(string xhtml)
        {
            return (XmlAttachment)Attach(AttachmentUtils.CreateXHtmlAttachment(null, xhtml));
        }

        /// <inheritdoc />
        public XmlAttachment AttachXml(string xml)
        {
            return (XmlAttachment)Attach(AttachmentUtils.CreateXmlAttachment(null, xml));
        }

        /// <inheritdoc />
        public BinaryAttachment AttachImage(Image image)
        {
            return (BinaryAttachment)Attach(AttachmentUtils.CreateImageAttachment(null, image));
        }

        /// <inheritdoc />
        public XmlAttachment AttachObjectAsXml(object obj, XmlSerializer xmlSerializer)
        {
            return (XmlAttachment)Attach(AttachmentUtils.CreateObjectAsXmlAttachment(null, obj, xmlSerializer));
        }

        private class RecordedExecutionLogStreamCollection : IExecutionLogStreamCollection
        {
            private RecordedExecutionLog executionLog;

            public RecordedExecutionLogStreamCollection(RecordedExecutionLog executionLog)
            {
                this.executionLog = executionLog;
            }

            public IExecutionLogStream this[string streamName]
            {
                get { return new RecordedExecutionLogStream(executionLog, streamName); }
            }
        }

        private class RecordedExecutionLogStream : TextWriter, IExecutionLogStream
        {
            private RecordedExecutionLog executionLog;
            private string streamName;

            public RecordedExecutionLogStream(RecordedExecutionLog executionLog, string streamName)
            {
                this.executionLog = executionLog;
                this.streamName = streamName;
            }

            public IExecutionLog ExecutionLog
            {
                get { return executionLog; }
            }

            public string Name
            {
                get { return streamName; }
            }

            public TextWriter TextWriter
            {
                get { return this; }
            }

            public override Encoding Encoding
            {
                get { return Encoding.Unicode; }
            }

            public void BeginSection(string sectionName)
            {
                executionLog.writer.BeginSection(streamName, sectionName);
            }

            public void EndSection()
            {
                executionLog.writer.EndSection(streamName);
            }

            public override void Write(string value)
            {
                executionLog.writer.WriteText(streamName, value);
            }

            public override void Write(char[] buffer, int index, int count)
            {
                Write(new String(buffer, index, count));
            }

            public Attachment Embed(Attachment attachment)
            {
                executionLog.writer.WriteAttachment(streamName, attachment);
                return attachment;
            }

            public TextAttachment EmbedPlainText(string text)
            {
                return (TextAttachment)Embed(AttachmentUtils.CreatePlainTextAttachment(null, text));
            }

            public TextAttachment EmbedHtml(string html)
            {
                return (TextAttachment)Embed(AttachmentUtils.CreateHtmlAttachment(null, html));
            }

            public XmlAttachment EmbedXHtml(string xhtml)
            {
                return (XmlAttachment)Embed(AttachmentUtils.CreateXHtmlAttachment(null, xhtml));
            }

            public XmlAttachment EmbedXml(string xml)
            {
                return (XmlAttachment)Embed(AttachmentUtils.CreateXmlAttachment(null, xml));
            }

            public BinaryAttachment EmbedImage(Image image)
            {
                return (BinaryAttachment)Embed(AttachmentUtils.CreateImageAttachment(null, image));
            }

            public XmlAttachment EmbedObjectAsXml(object obj, XmlSerializer xmlSerializer)
            {
                return (XmlAttachment)Embed(AttachmentUtils.CreateObjectAsXmlAttachment(null, obj, xmlSerializer));
            }
        }
    }
}
