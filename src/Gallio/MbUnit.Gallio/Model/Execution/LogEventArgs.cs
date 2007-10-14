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
using MbUnit.Logging;

namespace MbUnit.Model.Execution
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
        private string text;
        private Attachment attachment;
        private string attachmentName;
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
        /// <item><see cref="LogEventType.EmbedExisting" />, possibly null</item>
        /// <item><see cref="LogEventType.BeginSection" />, non-null</item>
        /// <item><see cref="LogEventType.EndSection" />, non-null</item>
        /// </list>
        /// </remarks>
        public string StreamName
        {
            get { return streamName; }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LogEventType.Write" />, non-null</item>
        /// </list>
        /// </remarks>
        public string Text
        {
            get { return text; }
        }

        /// <summary>
        /// Gets the attachment.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LogEventType.Attach" />, non-null</item>
        /// </list>
        /// </remarks>
        public Attachment Attachment
        {
            get { return attachment; }
        }

        /// <summary>
        /// Gets the name of the attachment to embed.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LogEventType.EmbedExisting" />, non-null</item>
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
        /// Creates a <see cref="LogEventType.Attach" /> event.
        /// </summary>
        /// <seealso cref="LogWriter.Attach"/>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="attachment">The attachment</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/> or <paramref name="attachment"/> is null</exception>
        public static LogEventArgs CreateAttachEvent(string stepId, Attachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(@"attachment");

            LogEventArgs e = new LogEventArgs(stepId, LogEventType.Attach);
            e.attachment = attachment;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="LogEventType.Write" /> event.
        /// </summary>
        /// <seealso cref="LogStreamWriter.Write(string)"/>
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
        /// Creates a <see cref="LogEventType.EmbedExisting" /> event.
        /// </summary>
        /// <seealso cref="LogWriter.Attach"/>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="streamName">The stream name</param>
        /// <param name="attachmentName">The attachment name</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/>, <paramref name="streamName" />
        /// or <paramref name="attachmentName"/> is null</exception>
        public static LogEventArgs CreateEmbedExistingEvent(string stepId, string streamName, string attachmentName)
        {
            if (streamName == null)
                throw new ArgumentNullException(@"streamName");
            if (attachmentName == null)
                throw new ArgumentNullException(@"attachmentName");

            LogEventArgs e = new LogEventArgs(stepId, LogEventType.EmbedExisting);
            e.streamName = streamName;
            e.attachmentName = attachmentName;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="LogEventType.BeginSection" /> event.
        /// </summary>
        /// <seealso cref="LogStreamWriter.BeginSection"/>
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
        /// <seealso cref="LogStreamWriter.EndSection"/>
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
        public void ApplyToLogWriter(LogWriter logWriter)
        {
            switch (eventType)
            {
                case LogEventType.Attach:
                    logWriter.Attach(attachment);
                    break;

                case LogEventType.Write:
                    logWriter[streamName].Write(text);
                    break;

                case LogEventType.BeginSection:
                    logWriter[streamName].BeginSection(sectionName);
                    break;

                case LogEventType.EndSection:
                    logWriter[streamName].EndSection();
                    break;

                case LogEventType.EmbedExisting:
                    logWriter[streamName].EmbedExisting(attachmentName);
                    break;
            }
        }
    }
}