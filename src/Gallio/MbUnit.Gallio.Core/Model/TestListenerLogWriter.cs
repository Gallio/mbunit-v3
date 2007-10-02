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
using MbUnit.Core.Model.Events;
using MbUnit.Framework.Kernel.ExecutionLogs;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// An implementation of <see cref="LogWriter" /> that sends events to a
    /// <see cref="ITestListener" />.
    /// </summary>
    public sealed class TestListenerLogWriter : TrackingLogWriter
    {
        private readonly string stepId;
        private readonly ITestListener listener;

        /// <summary>
        /// Creates a log writer.
        /// </summary>
        /// <param name="listener">The event listener</param>
        /// <param name="stepId">The step id</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="listener"/> or
        /// <paramref name="stepId"/> is null</exception>
        public TestListenerLogWriter(ITestListener listener, string stepId)
        {
            if (listener == null)
                throw new ArgumentNullException(@"listener");
            if (stepId == null)
                throw new ArgumentNullException(@"stepId");

            this.listener = listener;
            this.stepId = stepId;
        }

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            lock (this)
            {
                ThrowIfClosed();

                if (!TrackAttachment(attachment))
                    return;
            }

            listener.NotifyLogEvent(LogEventArgs.CreateAttachEvent(stepId, attachment));
        }

        /// <inheritdoc />
        protected override LogStreamWriter GetLogStreamWriterImpl(string streamName)
        {
            ThrowIfClosed();
            return new TestEventLogStreamWriter(this, streamName);
        }

        private sealed class TestEventLogStreamWriter : LogStreamWriter
        {
            private readonly TestListenerLogWriter logWriter;

            public TestEventLogStreamWriter(TestListenerLogWriter logWriter, string streamName)
                : base(streamName)
            {
                this.logWriter = logWriter;
            }

            protected override void WriteImpl(string text)
            {
                logWriter.ThrowIfClosed();
                logWriter.listener.NotifyLogEvent(LogEventArgs.CreateWriteEvent(logWriter.stepId, StreamName, text));
            }

            protected override void BeginSectionImpl(string sectionName)
            {
                logWriter.ThrowIfClosed();
                logWriter.listener.NotifyLogEvent(LogEventArgs.CreateBeginSectionEvent(logWriter.stepId, StreamName, sectionName));
            }

            protected override void EndSectionImpl()
            {
                logWriter.ThrowIfClosed();
                logWriter.listener.NotifyLogEvent(LogEventArgs.CreateEndSectionEvent(logWriter.stepId, StreamName));
            }

            protected override void EmbedImpl(Attachment attachment)
            {
                logWriter.ThrowIfClosed();
                logWriter.Attach(attachment);

                InternalEmbedExisting(attachment.Name);
            }

            protected override void EmbedExistingImpl(string attachmentName)
            {
                lock (logWriter)
                {
                    logWriter.ThrowIfClosed();
                    logWriter.VerifyAttachmentExists(attachmentName);
                }

                InternalEmbedExisting(attachmentName);
            }

            private void InternalEmbedExisting(string attachmentName)
            {
                logWriter.listener.NotifyLogEvent(LogEventArgs.CreateEmbedExistingEvent(logWriter.stepId, StreamName, attachmentName));
            }
        }
    }
}
