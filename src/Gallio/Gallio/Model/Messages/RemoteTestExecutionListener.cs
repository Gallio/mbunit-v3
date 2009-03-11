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
using Gallio.Model.Logging;
using Gallio.Model.Serialization;
using Gallio.Runtime.Remoting;

namespace Gallio.Model.Messages
{
    /// <summary>
    /// Wraps a test execution listener so that it can be accessed remotely.
    /// </summary>
    public class RemoteTestExecutionListener : LongLivedMarshalByRefObject, ITestExecutionListener
    {
        private readonly ITestExecutionListener testExecutionListener;

        /// <summary>
        /// Creates a wrapper for the specified listener.
        /// </summary>
        /// <param name="testExecutionListener">The listener to wrap</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testExecutionListener"/> is null</exception>
        public RemoteTestExecutionListener(ITestExecutionListener testExecutionListener)
        {
            if (testExecutionListener == null)
                throw new ArgumentNullException("testExecutionListener");

            this.testExecutionListener = testExecutionListener;
        }

        /// <inheritdoc />
        public void NotifyTestStepStarted(TestStepData step)
        {
            testExecutionListener.NotifyTestStepStarted(step);
        }

        /// <inheritdoc />
        public void NotifyTestStepLifecyclePhaseChanged(string stepId, string lifecyclePhase)
        {
            testExecutionListener.NotifyTestStepLifecyclePhaseChanged(stepId, lifecyclePhase);
        }

        /// <inheritdoc />
        public void NotifyTestStepMetadataAdded(string stepId, string metadataKey, string metadataValue)
        {
            testExecutionListener.NotifyTestStepMetadataAdded(stepId, metadataKey, metadataValue);
        }

        /// <inheritdoc />
        public void NotifyTestStepFinished(string stepId, TestResult result)
        {
            testExecutionListener.NotifyTestStepFinished(stepId, result);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogAttach(string stepId, Attachment attachment)
        {
            testExecutionListener.NotifyTestStepLogAttach(stepId, attachment);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogStreamWrite(string stepId, string streamName, string text)
        {
            testExecutionListener.NotifyTestStepLogStreamWrite(stepId, streamName, text);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogStreamEmbed(string stepId, string streamName, string attachmentName)
        {
            testExecutionListener.NotifyTestStepLogStreamEmbed(stepId, streamName, attachmentName);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogStreamBeginSection(string stepId, string streamName, string sectionName)
        {
            testExecutionListener.NotifyTestStepLogStreamBeginSection(stepId, streamName, sectionName);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogStreamBeginMarker(string stepId, string streamName, Marker marker)
        {
            testExecutionListener.NotifyTestStepLogStreamBeginMarker(stepId, streamName, marker);
        }

        /// <inheritdoc />
        public void NotifyTestStepLogStreamEnd(string stepId, string streamName)
        {
            testExecutionListener.NotifyTestStepLogStreamEnd(stepId, streamName);
        }
    }
}