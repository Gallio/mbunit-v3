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
using MbUnit.Framework.Kernel.Results;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// A test lifecycle event is fired as a test progresses through the
    /// steps of its lifecycle from start to finish.
    /// </summary>
    [Serializable]
    public class TestLifecycleEventArgs : TestEventArgs
    {
        private TestLifecycleEventType eventType;
        private string stepName;
        private TestResult result;

        private TestLifecycleEventArgs(string testId, TestLifecycleEventType eventType)
            : base(testId)
        {
            this.eventType = eventType;
        }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public TestLifecycleEventType EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Gets the step name.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="TestLifecycleEventType.Step" />, non-null</item>
        /// </list>
        /// </remarks>
        public string StepName
        {
            get { return stepName; }
        }

        /// <summary>
        /// Gets the test result.
        /// </summary>
        /// <remarks>
        /// Valid for events of the following types:
        /// <list type="bullet">
        /// <item><see cref="TestLifecycleEventType.Finish" />, non-null</item>
        /// </list>
        /// </remarks>
        public TestResult Result
        {
            get { return result; }
        }

        /// <summary>
        /// Creates a <see cref="TestLifecycleEventType.Start" /> event.
        /// </summary>
        /// <param name="testId">The id of the test this event is about</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> is null</exception>
        public static TestLifecycleEventArgs CreateStartEvent(string testId)
        {
            TestLifecycleEventArgs e = new TestLifecycleEventArgs(testId, TestLifecycleEventType.Start);
            return e;
        }

        /// <summary>
        /// Creates a <see cref="TestLifecycleEventType.Step" /> event.
        /// </summary>
        /// <seealso cref="TestStep"/>
        /// <param name="testId">The id of the test this event is about</param>
        /// <param name="stepName">The step name.</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> or <paramref name="stepName"/> is null</exception>
        public static TestLifecycleEventArgs CreateStepEvent(string testId, string stepName)
        {
            if (stepName == null)
                throw new ArgumentNullException("stepName");

            TestLifecycleEventArgs e = new TestLifecycleEventArgs(testId, TestLifecycleEventType.Step);
            e.stepName = stepName;
            return e;
        }

        /// <summary>
        /// Creates a <see cref="TestLifecycleEventType.Finish" /> event.
        /// </summary>
        /// <param name="testId">The id of the test this event is about</param>
        /// <param name="result">The test result</param>
        /// <returns>The event</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> or <paramref name="result"/> is null</exception>
        public static TestLifecycleEventArgs CreateFinishEvent(string testId, TestResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            TestLifecycleEventArgs e = new TestLifecycleEventArgs(testId, TestLifecycleEventType.Finish);
            e.result = result;
            return e;
        }
    }
}