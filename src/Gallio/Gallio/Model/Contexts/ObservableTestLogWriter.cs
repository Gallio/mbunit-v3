// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Markup;
using Gallio.Model.Messages.Execution;
using Gallio.Common.Messaging;

namespace Gallio.Model.Contexts
{
    /// <summary>
    /// A log writer that sends messages to a <see cref="IMessageSink" />.
    /// </summary>
    public class ObservableTestLogWriter : MarkupDocumentWriter
    {
        private IMessageSink messageSink;
        private readonly string stepId;

        /// <summary>
        /// Creates a log writer.
        /// </summary>
        /// <param name="messageSink">The test listener to which test messages are published.</param>
        /// <param name="stepId">The step id.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="messageSink"/> or
        /// <paramref name="stepId"/> is null.</exception>
        public ObservableTestLogWriter(IMessageSink messageSink, string stepId)
        {
            if (messageSink == null)
                throw new ArgumentNullException("messageSink");
            if (stepId == null)
                throw new ArgumentNullException(@"stepId");

            this.messageSink = messageSink;
            this.stepId = stepId;
        }

        /// <inheritdoc />
        protected override void CloseImpl()
        {
            messageSink = null;
        } 

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            messageSink.Publish(new TestStepLogAttachMessage()
            {
                StepId = stepId,
                Attachment = attachment
            });
        }

        /// <inheritdoc />
        protected override void StreamWriteImpl(string streamName, string text)
        {
            messageSink.Publish(new TestStepLogStreamWriteMessage()
            {
                StepId = stepId,
                StreamName = streamName,
                Text = text
            });
        }

        /// <inheritdoc />
        protected override void StreamEmbedImpl(string streamName, string attachmentName)
        {
            messageSink.Publish(new TestStepLogStreamEmbedMessage()
            {
                StepId = stepId,
                StreamName = streamName,
                AttachmentName = attachmentName
            });
        }

        /// <inheritdoc />
        protected override void StreamBeginSectionImpl(string streamName, string sectionName)
        {
            messageSink.Publish(new TestStepLogStreamBeginSectionBlockMessage()
            {
                StepId = stepId,
                StreamName = streamName,
                SectionName = sectionName
            });
        }

        /// <inheritdoc />
        protected override void StreamBeginMarkerImpl(string streamName, Marker marker)
        {
            messageSink.Publish(new TestStepLogStreamBeginMarkerBlockMessage()
            {
                StepId = stepId,
                StreamName = streamName,
                Marker = marker
            });
        }

        /// <inheritdoc />
        protected override void StreamEndImpl(string streamName)
        {
            messageSink.Publish(new TestStepLogStreamEndBlockMessage()
            {
                StepId = stepId,
                StreamName = streamName
            });
        }
    }
}