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
using MbUnit.Framework.Services.ExecutionLogs;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// A test execution log event describes an incremental operation being performed
    /// on the execution log associated with a test.
    /// </summary>
    [Serializable]
    public class TestExecutionLogEventArgs : TestEventArgs
    {
        private TestExecutionLogEventType eventType;
        private string streamName;
        private string text;
        private Attachment attachment;
        private string sectionName;

        private TestExecutionLogEventArgs(string testId, TestExecutionLogEventType eventType)
            : base(testId)
        {
            this.eventType = eventType;
        }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public TestExecutionLogEventType EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Gets the stream name.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="TestExecutionLogEventType.WriteText" />, non-null</item>
        /// <item><see cref="TestExecutionLogEventType.WriteAttachment" />, possibly null</item>
        /// <item><see cref="TestExecutionLogEventType.BeginSection" />, non-null</item>
        /// <item><see cref="TestExecutionLogEventType.EndSection" />, non-null</item>
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
        /// <item><see cref="TestExecutionLogEventType.WriteText" />, non-null</item>
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
        /// <item><see cref="TestExecutionLogEventType.WriteAttachment" />, non-null</item>
        /// </list>
        /// </remarks>
        public Attachment Attachment
        {
            get { return attachment; }
        }

        /// <summary>
        /// Gets the section name.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="TestExecutionLogEventType.BeginSection" />, non-null</item>
        /// </list>
        /// </remarks>
        public string SectionName
        {
            get { return sectionName; }
        }

        /// <summary>
        /// Creates a <see cref="TestExecutionLogEventType.WriteText" /> event.
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.WriteText"/>
        /// <param name="testId">The id of the test this event is about</param>
        /// <param name="streamName">The stream name</param>
        /// <param name="text">The text</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/>, <paramref name="streamName"/> or <paramref name="text"/> is null</exception>
        public static TestExecutionLogEventArgs CreateWriteTextEvent(string testId, string streamName, string text)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            if (text == null)
                throw new ArgumentNullException("text");

            TestExecutionLogEventArgs e = new TestExecutionLogEventArgs(testId, TestExecutionLogEventType.WriteText);
            e.streamName = streamName;
            e.text = text;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="TestExecutionLogEventType.WriteAttachment" /> event.
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.WriteAttachment"/>
        /// <param name="testId">The id of the test this event is about</param>
        /// <param name="streamName">The stream name, possibly null</param>
        /// <param name="attachment">The attachment</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> or <paramref name="attachment"/> is null</exception>
        public static TestExecutionLogEventArgs CreateWriteAttachmentEvent(string testId, string streamName, Attachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");

            TestExecutionLogEventArgs e = new TestExecutionLogEventArgs(testId, TestExecutionLogEventType.WriteAttachment);
            e.streamName = streamName;
            e.attachment = attachment;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="TestExecutionLogEventType.BeginSection" /> event.
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.BeginSection"/>
        /// <param name="testId">The id of the test this event is about</param>
        /// <param name="streamName">The stream name</param>
        /// <param name="sectionName">The section name</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/>, <paramref name="streamName"/> or <paramref name="sectionName"/> is null</exception>
        public static TestExecutionLogEventArgs CreateBeginSectionEvent(string testId, string streamName, string sectionName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            TestExecutionLogEventArgs e = new TestExecutionLogEventArgs(testId, TestExecutionLogEventType.BeginSection);
            e.streamName = streamName;
            e.sectionName = sectionName;
            return e;
        }
        
        /// <summary>
        /// Creates a <see cref="TestExecutionLogEventType.EndSection" /> event.
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.EndSection"/>
        /// <param name="testId">The id of the test this event is about</param>
        /// <param name="streamName">The stream name</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> or <paramref name="streamName"/> is null</exception>
        public static TestExecutionLogEventArgs CreateEndSectionEvent(string testId, string streamName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");

            TestExecutionLogEventArgs e = new TestExecutionLogEventArgs(testId, TestExecutionLogEventType.EndSection);
            e.streamName = streamName;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="TestExecutionLogEventType.Close" /> event.
        /// </summary>
        /// <seealso cref="IExecutionLogWriter.Close"/>
        /// <param name="testId">The id of the test this event is about</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> is null</exception>
        public static TestExecutionLogEventArgs CreateCloseEvent(string testId)
        {
            TestExecutionLogEventArgs e = new TestExecutionLogEventArgs(testId, TestExecutionLogEventType.Close);
            return e;
        }
    }
}
