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
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Model.Events
{
    /// <summary>
    /// A test lifecycle event is fired as a test progresses through the
    /// steps of its lifecycle from start to finish.
    /// </summary>
    [Serializable]
    public class LifecycleEventArgs : StepEventArgs
    {
        private readonly LifecycleEventType eventType;
        private StepData stepData;
        private string phaseName;
        private TestResult result;
        private string metadataKey;
        private string metadataValue;

        private LifecycleEventArgs(string stepId, LifecycleEventType eventType)
            : base(stepId)
        {
            this.eventType = eventType;
        }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public LifecycleEventType EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Gets information about the step just started.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LifecycleEventType.Start" />, non-null</item>
        /// </list>
        /// </remarks>
        public StepData StepData
        {
            get { return stepData; }
        }

        /// <summary>
        /// Gets the phase name.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LifecycleEventType.SetPhase" />, non-null</item>
        /// </list>
        /// </remarks>
        public string PhaseName
        {
            get { return phaseName; }
        }

        /// <summary>
        /// Gets the test result.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LifecycleEventType.Finish" />, non-null</item>
        /// </list>
        /// </remarks>
        public TestResult Result
        {
            get { return result; }
        }

        /// <summary>
        /// Gets the metadata key.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LifecycleEventType.AddMetadata" />, non-null</item>
        /// </list>
        /// </remarks>
        public string MetadataKey
        {
            get { return metadataKey; }
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="LifecycleEventType.AddMetadata" />, non-null</item>
        /// </list>
        /// </remarks>
        public string MetadataValue
        {
            get { return metadataValue; }
        }

        /// <summary>
        /// Creates a <see cref="LifecycleEventType.Start" /> event.
        /// </summary>
        /// <param name="stepData">Information about the step that is about to start</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepData"/> is null</exception>
        public static LifecycleEventArgs CreateStartEvent(StepData stepData)
        {
            if (stepData == null)
                throw new ArgumentNullException(@"stepData");

            LifecycleEventArgs e = new LifecycleEventArgs(stepData.Id, LifecycleEventType.Start);
            e.stepData = stepData;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="LifecycleEventType.SetPhase" /> event.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="phaseName">The phase name</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/> or <paramref name="phaseName"/> is null</exception>
        public static LifecycleEventArgs CreateSetPhaseEvent(string stepId, string phaseName)
        {
            if (phaseName == null)
                throw new ArgumentNullException(@"phaseName");

            LifecycleEventArgs e = new LifecycleEventArgs(stepId, LifecycleEventType.SetPhase);
            e.phaseName = phaseName;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="LifecycleEventType.AddMetadata" /> event.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="metadataKey">The metadata key</param>
        /// <param name="metadataValue">The metadata value</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/>,
        /// <paramref name="metadataKey"/> or <paramref name="metadataValue" /> is null</exception>
        public static LifecycleEventArgs CreateAddMetadataEvent(string stepId, string metadataKey, string metadataValue)
        {
            if (metadataKey == null)
                throw new ArgumentNullException(@"metadataKey");
            if (metadataValue == null)
                throw new ArgumentNullException(@"metadataValue");

            LifecycleEventArgs e = new LifecycleEventArgs(stepId, LifecycleEventType.AddMetadata);
            e.metadataKey = metadataKey;
            e.metadataValue = metadataValue;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="LifecycleEventType.Finish" /> event.
        /// </summary>
        /// <param name="stepId">The id of the test step this event is about</param>
        /// <param name="result">The test result</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/> or <paramref name="result"/> is null</exception>
        public static LifecycleEventArgs CreateFinishEvent(string stepId, TestResult result)
        {
            if (result == null)
                throw new ArgumentNullException(@"result");

            LifecycleEventArgs e = new LifecycleEventArgs(stepId, LifecycleEventType.Finish);
            e.result = result;
            return e;
        }
    }
}