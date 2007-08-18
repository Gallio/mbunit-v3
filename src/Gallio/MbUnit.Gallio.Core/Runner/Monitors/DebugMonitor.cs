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
using System.IO;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner.Monitors
{
    /// <summary>
    /// Monitors <see cref="ITestRunner" /> events and writes debug messages to
    /// an output stream.
    /// </summary>
    public class DebugMonitor : BaseTestRunnerMonitor
    {
        private TextWriter writer;

        /// <summary>
        /// Creates a console monitor.
        /// </summary>
        /// <param name="writer">The text writer for all output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportMonitor" or
        /// <paramref name="writer"/> is null</exception>
        public DebugMonitor(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            Runner.EventDispatcher.Message += HandleMessageEvent;
            Runner.EventDispatcher.TestLifecycle += HandleTestLifecycleEvent;
            Runner.EventDispatcher.TestExecutionLog += HandleTestExecutionLogEvent;
        }

        private void HandleMessageEvent(object sender, MessageEventArgs e)
        {
            writer.WriteLine("[Message: {0}] - {1}", e.MessageType, e.Message);
            writer.WriteLine();
        }

        private void HandleTestLifecycleEvent(object sender, TestLifecycleEventArgs e)
        {
            TestInfo testInfo = Runner.TestModel.Tests[e.TestId];

            switch (e.EventType)
            {
                case TestLifecycleEventType.Start:
                    writer.WriteLine("[Lifecycle: Start ({0})]", testInfo.Name);
                    break;

                case TestLifecycleEventType.Step:
                    writer.WriteLine("[Lifecycle: Step ({0})]", testInfo.Name);
                    writer.WriteLine("\tStep Name: {0}", e.StepName);
                    break;

                case TestLifecycleEventType.Finish:
                    writer.WriteLine("[Lifecycle: Finish ({0})]", testInfo.Name);
                    writer.WriteLine("\tState: {0}", e.Result.State);
                    writer.WriteLine("\tOutcome: {0}", e.Result.Outcome);
                    writer.WriteLine("\tAsserts: {0}", e.Result.AssertCount);
                    writer.WriteLine("\tDuration: {0}", e.Result.Duration);
                    break;
            }

            writer.WriteLine();
        }

        private void HandleTestExecutionLogEvent(object sender, TestExecutionLogEventArgs e)
        {
            TestInfo testInfo = Runner.TestModel.Tests[e.TestId];

            switch (e.EventType)
            {
                case TestExecutionLogEventType.WriteText:
                    writer.WriteLine("[Execution Log: Write Text ({0})]", testInfo.Name);
                    writer.WriteLine("\tStream Name: {0}", e.StreamName);
                    writer.WriteLine("\tText: {0}", e.Text);
                    break;

                case TestExecutionLogEventType.WriteAttachment:
                    writer.WriteLine("[Execution Log: Write Attachment ({0})]", testInfo.Name);
                    writer.WriteLine("\tStream Name: {0}", e.StreamName ?? "<null>");
                    writer.WriteLine("\tAttachment Name: {0}", e.Attachment.Name);
                    writer.WriteLine("\tAttachment Content Type: {0}", e.Attachment.ContentType);
                    break;

                case TestExecutionLogEventType.BeginSection:
                    writer.WriteLine("[Execution Log: Being Section ({0})]", testInfo.Name);
                    writer.WriteLine("\tStream Name: {0}", e.StreamName);
                    writer.WriteLine("\tSection Name: {0}", e.SectionName);
                    break;

                case TestExecutionLogEventType.EndSection:
                    writer.WriteLine("[Execution Log: End Section ({0})]", testInfo.Name);
                    writer.WriteLine("\tStream Name: {0}", e.StreamName);
                    break;

                case TestExecutionLogEventType.Close:
                    writer.WriteLine("[Execution Log: Close ({0})]", testInfo.Name);
                    break;
            }

            writer.WriteLine();
        }
    }
}
