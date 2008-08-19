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

using Gallio.Model.Logging;
using Gallio.Model.Serialization;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// An test listener observes the progress of test execution as a
    /// series of events.
    /// </summary>
    public interface ITestListener
    {
        /// <summary>
        /// Notifies the listener that a test step has started execution.
        /// </summary>
        /// <param name="step">Information about the test step that is about to start, not null</param>
        void NotifyTestStepStarted(TestStepData step);

        /// <summary>
        /// Notifies the listener that a test step has changed lifecycle phase.
        /// </summary>
        /// <param name="stepId">The id of the test step, not null</param>
        /// <param name="lifecyclePhase">The lifecycle phase name, not null</param>
        void NotifyTestStepLifecyclePhaseChanged(string stepId, string lifecyclePhase);

        /// <summary>
        /// Notifies the listener that a test step has dynamically added metadata to itself.
        /// </summary>
        /// <param name="stepId">The id of the test step, not null</param>
        /// <param name="metadataKey">The metadata key, not null</param>
        /// <param name="metadataValue">The metadata value, not null</param>
        void NotifyTestStepMetadataAdded(string stepId, string metadataKey, string metadataValue);

        /// <summary>
        /// Notifies the listener that a test step has finished execution.
        /// </summary>
        /// <param name="stepId">The id of the test step, not null</param>
        /// <param name="result">The test result, not null</param>
        void NotifyTestStepFinished(string stepId, TestResult result);

        /// <summary>
        /// Notifies the listener that an attachment has been added to a test step log.
        /// </summary>
        /// <param name="stepId">The id of the test step, not null</param>
        /// <param name="attachment">The attachment, not null</param>
        void NotifyTestStepLogAttach(string stepId, Attachment attachment);

        /// <summary>
        /// Notifies the listener that text has been written to a test step log stream.
        /// </summary>
        /// <param name="stepId">The id of the test step, not null</param>
        /// <param name="streamName">The stream name, not null</param>
        /// <param name="text">The text, not null</param>
        void NotifyTestStepLogStreamWrite(string stepId, string streamName, string text);

        /// <summary>
        /// Notifies the listener that an attachment has been embedded into a test step log stream.
        /// </summary>
        /// <param name="stepId">The id of the test step, not null</param>
        /// <param name="streamName">The stream name, not null</param>
        /// <param name="attachmentName">The attachment name, not null</param>
        void NotifyTestStepLogStreamEmbed(string stepId, string streamName, string attachmentName);

        /// <summary>
        /// Notifies the listener that a section region has been started within a test step log stream.
        /// </summary>
        /// <param name="stepId">The id of the test step, not null</param>
        /// <param name="streamName">The stream name, not null</param>
        /// <param name="sectionName">The section name, not null</param>
        void NotifyTestStepLogStreamBeginSection(string stepId, string streamName, string sectionName);

        /// <summary>
        /// Notifies the listener that a marker region has been started within a test step log stream.
        /// </summary>
        /// <param name="stepId">The id of the test step, not null</param>
        /// <param name="streamName">The stream name, not null</param>
        /// <param name="marker">The marker</param>
        void NotifyTestStepLogStreamBeginMarker(string stepId, string streamName, Marker marker);

        /// <summary>
        /// Notifies the listener that a region started with Begin* has finished within a test step log stream.
        /// </summary>
        /// <param name="stepId">The id of the test step, not null</param>
        /// <param name="streamName">The stream name, not null</param>
        void NotifyTestStepLogStreamEnd(string stepId, string streamName);
    }
}