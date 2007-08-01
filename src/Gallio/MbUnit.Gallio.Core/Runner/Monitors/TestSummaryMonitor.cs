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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner.Monitors
{
    /// <summary>
    /// A test summary monitor tracks <see cref="ITestRunner" /> events and builds
    /// a table of the final test results and execution log output of all tests that run.
    /// </summary>
    /// <todo author="jeff">
    /// Tentative.  Subject to change!!
    /// </todo>
    public class TestSummaryMonitor : BaseTestRunnerMonitor
    {
        private Dictionary<string, TestSummary> summaries;

        /// <summary>
        /// Creates a test summary tracker initially with no contents.
        /// </summary>
        public TestSummaryMonitor()
        {
            summaries = new Dictionary<string, TestSummary>();
        }

        /// <summary>
        /// Gets a dictionary of test summaries indexed by test id.
        /// </summary>
        public IDictionary<string, TestSummary> Summaries
        {
            get { return summaries; }
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            Runner.EventDispatcher.TestLifecycle += HandleTestLifecycleEvent;
            Runner.EventDispatcher.TestExecutionLog += HandleTestExecutionLogEvent;
        }

        private void HandleTestLifecycleEvent(object sender, TestLifecycleEventArgs e)
        {
            switch (e.EventType)
            {
                case TestLifecycleEventType.Start:
                case TestLifecycleEventType.Step:
                    break;

                case TestLifecycleEventType.Finish:
                    lock (this)
                        GetOrCreateSummary(e.TestId).Result = e.Result;
                    break;
            }
        }

        private void HandleTestExecutionLogEvent(object sender, TestExecutionLogEventArgs e)
        {
            lock (this)
            {
                TestSummary summary = GetOrCreateSummary(e.TestId);

                switch (e.EventType)
                {
                    case TestExecutionLogEventType.WriteText:
                        summary.ExecutionLogWriter.WriteText(e.StreamName, e.Text);
                        break;

                    case TestExecutionLogEventType.WriteAttachment:
                        summary.ExecutionLogWriter.WriteAttachment(e.StreamName, e.Attachment);
                        break;

                    case TestExecutionLogEventType.BeginSection:
                        summary.ExecutionLogWriter.BeginSection(e.StreamName, e.SectionName);
                        break;

                    case TestExecutionLogEventType.EndSection:
                        summary.ExecutionLogWriter.EndSection(e.StreamName);
                        break;

                    case TestExecutionLogEventType.Close:
                        summary.ExecutionLogWriter.Close();
                        break;
                }
            }
        }

        private TestSummary GetOrCreateSummary(string testId)
        {
            TestSummary summary;
            if (!summaries.TryGetValue(testId, out summary))
            {
                summary = new TestSummary();
                summaries.Add(testId, summary);
            }

            return summary;
        }
    }
}
