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
using System.Threading;
using Gallio.Model.Execution;
using Gallio.Logging;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// An implementation of <see cref="LogWriter" /> that sends events to a
    /// <see cref="ITestListener" />.
    /// </summary>
    public sealed class TestListenerLogWriter : TrackingLogWriter
    {
        private readonly ReaderWriterLock rwLock = new ReaderWriterLock();
        private readonly string stepId;
        private ITestListener listener;

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
        protected override void Dispose(bool disposing)
        {
            try
            {
                rwLock.AcquireWriterLock(Timeout.Infinite);

                base.Dispose(disposing);
                listener = null;
            }
            finally
            {
                if (rwLock.IsWriterLockHeld)
                    rwLock.ReleaseWriterLock();
            }
        }

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            DoWithListener(delegate(ITestListener listener)
            {
                if (!TrackAttachment(attachment))
                    return;

                listener.NotifyLogEvent(LogEventArgs.CreateAttachEvent(stepId, attachment));
            });
        }

        /// <inheritdoc />
        protected override LogStreamWriter GetLogStreamWriterImpl(string streamName)
        {
            ThrowIfClosed();
            return new TestEventLogStreamWriter(this, streamName);
        }

        private void DoWithListener(Action<ITestListener> action)
        {
            try
            {
                rwLock.AcquireReaderLock(Timeout.Infinite);

                ThrowIfClosed();
                action(listener);
            }
            finally
            {
                if (rwLock.IsReaderLockHeld)
                    rwLock.ReleaseReaderLock();
            }
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
                logWriter.DoWithListener(delegate(ITestListener listener)
                {
                    listener.NotifyLogEvent(LogEventArgs.CreateWriteEvent(logWriter.stepId, StreamName, text));
                });
            }

            protected override void BeginSectionImpl(string sectionName)
            {
                logWriter.DoWithListener(delegate(ITestListener listener)
                {
                    listener.NotifyLogEvent(LogEventArgs.CreateBeginSectionEvent(logWriter.stepId, StreamName, sectionName));
                });
            }

            protected override void EndSectionImpl()
            {
                logWriter.DoWithListener(delegate(ITestListener listener)
                {
                    listener.NotifyLogEvent(LogEventArgs.CreateEndSectionEvent(logWriter.stepId, StreamName));
                });
            }

            protected override void EmbedImpl(Attachment attachment)
            {
                // Note: This is outside the critical section to avoid reader lock re-entrance.
                logWriter.Attach(attachment);

                logWriter.DoWithListener(delegate(ITestListener listener)
                {
                    InternalEmbedExisting(listener, attachment.Name);
                });
            }

            protected override void EmbedExistingImpl(string attachmentName)
            {
                logWriter.DoWithListener(delegate(ITestListener listener)
                {
                    logWriter.VerifyAttachmentExists(attachmentName);
                    InternalEmbedExisting(listener, attachmentName);
                });
            }

            private void InternalEmbedExisting(ITestListener listener, string attachmentName)
            {
                listener.NotifyLogEvent(LogEventArgs.CreateEmbedExistingEvent(logWriter.stepId, StreamName, attachmentName));
            }
        }
    }
}