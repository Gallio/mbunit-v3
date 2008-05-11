// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Serialization;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Wraps a test listener so that it can be accessed remotely.
    /// </summary>
    public class RemoteTestListener : MarshalByRefObject, ITestListener
    {
        private readonly ITestListener listener;

        /// <summary>
        /// Creates a wrapper for the specified test listener.
        /// </summary>
        /// <param name="listener">The logger</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="listener"/> is null</exception>
        public RemoteTestListener(ITestListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            this.listener = listener;
        }

        /// <inheritdoc />
        public void NotifyTestStepStarted(TestStepData step)
        {
            listener.NotifyTestStepStarted(step);
        }

        /// <inheritdoc />
        public void NotifyTestStepLifecyclePhaseChanged(string stepId, string lifecyclePhase)
        {
            listener.NotifyTestStepLifecyclePhaseChanged(stepId, lifecyclePhase);
        }

        /// <inheritdoc />
        public void NotifyTestStepMetadataAdded(string stepId, string metadataKey, string metadataValue)
        {
            listener.NotifyTestStepMetadataAdded(stepId, metadataKey, metadataValue);
        }

        /// <inheritdoc />
        public void NotifyTestStepFinished(string stepId, TestResult result)
        {
            listener.NotifyTestStepFinished(stepId, result);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogTextAttachmentAdded(string stepId, string attachmentName, string contentType,
            string text)
        {
            listener.NotifyTestStepLogTextAttachmentAdded(stepId, attachmentName, contentType, text);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogBinaryAttachmentAdded(string stepId, string attachmentName, string contentType,
            byte[] bytes)
        {
            listener.NotifyTestStepLogBinaryAttachmentAdded(stepId, attachmentName, contentType, bytes);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogStreamTextWritten(string stepId, string streamName, string text)
        {
            listener.NotifyTestStepLogStreamTextWritten(stepId, streamName, text);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogStreamAttachmentEmbedded(string stepId, string streamName, string attachmentName)
        {
            listener.NotifyTestStepLogStreamAttachmentEmbedded(stepId, streamName, attachmentName);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogStreamSectionStarted(string stepId, string streamName, string sectionName)
        {
            listener.NotifyTestStepLogStreamSectionStarted(stepId, streamName, sectionName);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogStreamSectionFinished(string stepId, string streamName)
        {
            listener.NotifyTestStepLogStreamSectionFinished(stepId, streamName);
        }
    }
}
