// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Messages;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A log writer that sends messages to a <see cref="ITestExecutionListener" />.
    /// </summary>
    public class ObservableTestLogWriter : MarkupDocumentWriter
    {
        private ITestExecutionListener testExecutionListener;
        private readonly string stepId;

        /// <summary>
        /// Creates a log writer.
        /// </summary>
        /// <param name="testExecutionListener">The test listener to which notifications are dispatched.</param>
        /// <param name="stepId">The step id.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testExecutionListener"/> or
        /// <paramref name="stepId"/> is null.</exception>
        public ObservableTestLogWriter(ITestExecutionListener testExecutionListener, string stepId)
        {
            if (testExecutionListener == null)
                throw new ArgumentNullException("testExecutionListener");
            if (stepId == null)
                throw new ArgumentNullException(@"stepId");

            this.testExecutionListener = testExecutionListener;
            this.stepId = stepId;
        }

        /// <inheritdoc />
        protected override void CloseImpl()
        {
            testExecutionListener = null;
        } 

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            testExecutionListener.NotifyTestStepLogAttach(stepId, attachment);
        }

        /// <inheritdoc />
        protected override void StreamWriteImpl(string streamName, string text)
        {
            testExecutionListener.NotifyTestStepLogStreamWrite(stepId, streamName, text);
        }

        /// <inheritdoc />
        protected override void StreamEmbedImpl(string streamName, string attachmentName)
        {
            testExecutionListener.NotifyTestStepLogStreamEmbed(stepId, streamName, attachmentName);
        }

        /// <inheritdoc />
        protected override void StreamBeginSectionImpl(string streamName, string sectionName)
        {
            testExecutionListener.NotifyTestStepLogStreamBeginSection(stepId, streamName, sectionName);
        }

        /// <inheritdoc />
        protected override void StreamBeginMarkerImpl(string streamName, Marker marker)
        {
            testExecutionListener.NotifyTestStepLogStreamBeginMarker(stepId, streamName, marker);
        }

        /// <inheritdoc />
        protected override void StreamEndImpl(string streamName)
        {
            testExecutionListener.NotifyTestStepLogStreamEnd(stepId, streamName);
        }
    }
}
