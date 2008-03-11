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

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A log event describes an incremental operation being performed on the execution
    /// log associated with a test step.
    /// </summary>
    [Serializable]
    public class LogEventArgs : StepEventArgs
    {
        private readonly LogEventType eventType;
        private string streamName;
        private string attachmentName;
        private string contentType;
        private string text;
        private byte[] bytes;
        private string sectionName;

        private LogEventArgs(string stepId, LogEventType eventType)
            : base(stepId)
        {
            this.eventType = eventType;
        }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public LogEventType EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Gets the stream name.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LogEventType.Write" />, non-null</item>
        /// <item><see cref="LogEventType.Embed" />, possibly null</item>
        /// <item><see cref="LogEventType.BeginSection" />, non-null</item>
        /// <item><see cref="LogEventType.EndSection" />, non-null</item>
        /// </list>
        /// </remarks>
        public string StreamName
        {
            get { return streamName; }
        }

        /// <summary>
        /// Gets the text contents.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LogEventType.Write" />, non-null</item>
        /// <item><see cref="LogEventType.AttachText" />, non-null</item>
        /// </list>
        /// </remarks>
        public string Text
        {
            get { return text; }
        }

        /// <summary>
        /// Gets the binary contents.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LogEventType.AttachBytes" />, non-null</item>
        /// </list>
        /// </remarks>
        public byte[] Bytes
        {
            get { return bytes; }
        }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LogEventType.AttachText" />, non-null</item>
        /// <item><see cref="LogEventType.AttachBytes" />, non-null</item>
        /// </list>
        /// </remarks>
        public string ContentType
        {
            get { return contentType; }
        }

        /// <summary>
        /// Gets the attachment name.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LogEventType.Embed" />, non-null</item>
        /// <item><see cref="LogEventType.AttachText" />, non-null</item>
        /// <item><see cref="LogEventType.AttachBytes" />, non-null</item>
        /// </list>
        /// </remarks>
        public string AttachmentName
        {
            get { return attachmentName; }
        }

        /// <summary>
        /// Gets the section name.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LogEventType.BeginSection" />, non-null</item>
        /// </list>
        /// </remarks>
        public string SectionName
        {
            get { return sectionName; }
        }

        /// <summary>
        /// Creates a <see cref="LogEventType.AttachText" /> event.
        /// </summary>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="text">The text contents</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/>, <paramref name="attachmentName"/>,
        /// <paramref name="contentType"/> or <paramref name="text"/> is null</exception>
        public static LogEventArgs CreateAttachTextEvent(string stepId, string attachmentName, string contentType, string text)
        {
            if (attachmentName == null)
                throw new ArgumentNullException(@"attachmentName");
            if (contentType == null)
                throw new ArgumentNullException(@"contentType");
            if (text == null)
                throw new ArgumentNullException(@"text");

            LogEventArgs e = new LogEventArgs(stepId, LogEventType.AttachText);
            e.attachmentName = attachmentName;
            e.contentType = contentType;
            e.text = text;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="LogEventType.AttachBytes" /> event.
        /// </summary>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="bytes">The binary contents</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/>, <paramref name="attachmentName"/>,
        /// <paramref name="contentType"/> or <paramref name="bytes"/> is null</exception>
        public static LogEventArgs CreateAttachBytesEvent(string stepId, string attachmentName, string contentType, byte[] bytes)
        {
            if (attachmentName == null)
                throw new ArgumentNullException(@"attachmentName");
            if (contentType == null)
                throw new ArgumentNullException(@"contentType");
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            LogEventArgs e = new LogEventArgs(stepId, LogEventType.AttachBytes);
            e.attachmentName = attachmentName;
            e.contentType = contentType;
            e.bytes = bytes;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="LogEventType.Write" /> event.
        /// </summary>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="streamName">The stream name</param>
        /// <param name="text">The text</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/>, <paramref name="streamName"/> or <paramref name="text"/> is null</exception>
        public static LogEventArgs CreateWriteEvent(string stepId, string streamName, string text)
        {
            if (streamName == null)
                throw new ArgumentNullException(@"streamName");
            if (text == null)
                throw new ArgumentNullException(@"text");

            LogEventArgs e = new LogEventArgs(stepId, LogEventType.Write);
            e.streamName = streamName;
            e.text = text;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="LogEventType.Embed" /> event.
        /// </summary>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="streamName">The stream name</param>
        /// <param name="attachmentName">The attachment name</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/>, <paramref name="streamName" />
        /// or <paramref name="attachmentName"/> is null</exception>
        public static LogEventArgs CreateEmbedEvent(string stepId, string streamName, string attachmentName)
        {
            if (streamName == null)
                throw new ArgumentNullException(@"streamName");
            if (attachmentName == null)
                throw new ArgumentNullException(@"attachmentName");

            LogEventArgs e = new LogEventArgs(stepId, LogEventType.Embed);
            e.streamName = streamName;
            e.attachmentName = attachmentName;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="LogEventType.BeginSection" /> event.
        /// </summary>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="streamName">The stream name</param>
        /// <param name="sectionName">The section name</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/>, <paramref name="streamName"/> or <paramref name="sectionName"/> is null</exception>
        public static LogEventArgs CreateBeginSectionEvent(string stepId, string streamName, string sectionName)
        {
            if (streamName == null)
                throw new ArgumentNullException(@"streamName");
            if (sectionName == null)
                throw new ArgumentNullException(@"sectionName");

            LogEventArgs e = new LogEventArgs(stepId, LogEventType.BeginSection);
            e.streamName = streamName;
            e.sectionName = sectionName;
            return e;
        }
        
        /// <summary>
        /// Creates a <see cref="LogEventType.EndSection" /> event.
        /// </summary>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="streamName">The stream name</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/> or <paramref name="streamName"/> is null</exception>
        public static LogEventArgs CreateEndSectionEvent(string stepId, string streamName)
        {
            if (streamName == null)
                throw new ArgumentNullException(@"streamName");

            LogEventArgs e = new LogEventArgs(stepId, LogEventType.EndSection);
            e.streamName = streamName;
            return e;
        }

        /// <summary>
        /// Applies the event to the specified log writer.
        /// </summary>
        /// <param name="logWriter">The log writer</param>
        public void ApplyToLogWriter(ITestLogWriter logWriter)
        {
            switch (eventType)
            {
                case LogEventType.AttachBytes:
                    logWriter.AttachBytes(attachmentName, contentType, bytes);
                    break;

                case LogEventType.AttachText:
                    logWriter.AttachText(attachmentName, contentType, text);
                    break;

                case LogEventType.Write:
                    logWriter.Write(streamName, text);
                    break;

                case LogEventType.BeginSection:
                    logWriter.BeginSection(streamName, sectionName);
                    break;

                case LogEventType.EndSection:
                    logWriter.EndSection(streamName);
                    break;

                case LogEventType.Embed:
                    logWriter.Embed(streamName, attachmentName);
                    break;
            }
        }
    }
}