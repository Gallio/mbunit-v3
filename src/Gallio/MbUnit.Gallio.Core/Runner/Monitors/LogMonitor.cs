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

using System.Collections.Generic;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Results;

namespace MbUnit.Core.Runner.Monitors
{
    /// <summary>
    /// An <see cref="ITestRunnerMonitor" /> implementation that listens to
    /// <see cref="ITestRunner" /> events and makes easier to keep track of
    /// executed test cases and their stack traces if they fail.
    /// </summary>
    public abstract class LogMonitor : BaseTestRunnerMonitor
    {
        private readonly object lockObject = new object();
        private readonly Dictionary<string, string> stepNames = new Dictionary<string, string>();
        // A dictionary with the test cases and maybe a stack trace if they fail
        private readonly Dictionary<string, string> testCases = new Dictionary<string, string>();

        /// <inheritdoc />
        protected override void OnAttach()
        {
            base.OnAttach();

            Runner.EventDispatcher.Lifecycle += HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog += HandleExecutionLogEvent;
        }

        private void HandleLifecycleEvent(object sender, LifecycleEventArgs e)
        {
            lock (lockObject)
            {
                switch (e.EventType)
                {
                    case LifecycleEventType.Start:
                        ProcessStartEvent(e);
                        break;

                    case LifecycleEventType.Finish:
                        // We are only interested in test cases
                        if (!testCases.ContainsKey(e.StepId))
                            return;
                        ProcessFinishEvent(e);
                        break;
                }
            }
        }

        private void ProcessStartEvent(LifecycleEventArgs e)
        {
            // Construct the test name
            string stepName;
            if (e.StepInfo.ParentId != null)
                stepName = GetStepName(e.StepInfo.ParentId) + @" / " + e.StepInfo.Name;
            else
                stepName = Runner.TestModel.Tests[e.StepInfo.TestId].Name;
            stepNames.Add(e.StepId, stepName);

            // We maintain a list of the tests cases so we can show only them in the
            // output. e.StepInfo is null on the Finish event so we have to check here.
            if (Runner.TestModel.Tests[e.StepInfo.TestId].IsTestCase)
            {
                testCases.Add(e.StepId, null);
                OnTestCaseStart();
            }
        }

        private void ProcessFinishEvent(LifecycleEventArgs e)
        {
            string stepName = GetStepName(e.StepId);
            string stackTrace = null;

            // If testCases[e.StepId] is not null, then it contains the stack trace
            // associated with this test
            if (testCases.ContainsKey(e.StepId) && testCases[e.StepId] != null)
            {
                stackTrace = testCases[e.StepId];
            }
            OnTestCaseFinished(stepName, stackTrace, e.Result);
        }

        private void HandleExecutionLogEvent(object sender, ExecutionLogEventArgs e)
        {
            lock (lockObject)
            {
                // Save the stack trace
                if (testCases.ContainsKey(e.StepId)
                    && e.EventType == ExecutionLogEventType.WriteText
                    && e.StreamName.CompareTo("Failures") == 0)
                {
                    testCases[e.StepId] = e.Text;
                }
            }
        }

        protected virtual void OnTestCaseStart()
        {

        }

        protected virtual void OnTestCaseFinished(string testName, string stackTrace, TestResult testResult)
        {
            
        }

        private string GetStepName(string stepId)
        {
            string stepName;
            return stepNames.TryGetValue(stepId, out stepName) ? stepName : stepId;
        }
    }
}
