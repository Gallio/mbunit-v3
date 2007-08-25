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
        private readonly TextWriter writer;
        private Dictionary<string, string> stepNames;

        /// <summary>
        /// Creates a console monitor.
        /// </summary>
        /// <param name="writer">The text writer for all output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public DebugMonitor(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        /// <inheritdoc />
        protected override void OnAttach()
        {
            base.OnAttach();

            Runner.EventDispatcher.Message += HandleMessageEvent;
            Runner.EventDispatcher.Lifecycle += HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog += HandleExecutionLogEvent;
        }

        private void HandleMessageEvent(object sender, MessageEventArgs e)
        {
            lock (this)
            {
                writer.WriteLine("[Message: {0}] - {1}", e.MessageType, e.Message);
                writer.WriteLine();
            }
        }

        private void HandleLifecycleEvent(object sender, LifecycleEventArgs e)
        {
            lock (this)
            {
                string stepName = GetStepName(e.StepId);

                switch (e.EventType)
                {
                    case LifecycleEventType.Start:
                        if (e.StepInfo.ParentId != null)
                            stepName = GetStepName(e.StepInfo.ParentId) + " / " + e.StepInfo.Name;
                        else
                            stepName = Runner.TestModel.Tests[e.StepInfo.TestId].Name + ": " + e.StepInfo.Name;

                        stepNames.Add(e.StepId, stepName);

                        writer.WriteLine("[Lifecycle: Start ({0})]", stepName);
                        break;

                    case LifecycleEventType.EnterPhase:
                        writer.WriteLine("[Lifecycle: Enter Phase ({0})]", stepName);
                        writer.WriteLine("\tPhase Name: {0}", e.PhaseName);
                        break;

                    case LifecycleEventType.Finish:
                        writer.WriteLine("[Lifecycle: Finish ({0})]", stepName);
                        writer.WriteLine("\tState: {0}", e.Result.State);
                        writer.WriteLine("\tOutcome: {0}", e.Result.Outcome);
                        writer.WriteLine("\tAsserts: {0}", e.Result.AssertCount);
                        writer.WriteLine("\tDuration: {0}", e.Result.Duration);
                        break;
                }

                writer.WriteLine();
            }
        }

        private void HandleExecutionLogEvent(object sender, ExecutionLogEventArgs e)
        {
            lock (this)
            {
                string stepName = GetStepName(e.StepId);

                switch (e.EventType)
                {
                    case ExecutionLogEventType.WriteText:
                        writer.WriteLine("[Execution Log: Write Text ({0})]", stepName);
                        writer.WriteLine("\tStream Name: {0}", e.StreamName);
                        writer.WriteLine("\tText: {0}", e.Text);
                        break;

                    case ExecutionLogEventType.WriteAttachment:
                        writer.WriteLine("[Execution Log: Write Attachment ({0})]", stepName);
                        writer.WriteLine("\tStream Name: {0}", e.StreamName ?? "<null>");
                        writer.WriteLine("\tAttachment Name: {0}", e.Attachment.Name);
                        writer.WriteLine("\tAttachment Content Type: {0}", e.Attachment.ContentType);
                        break;

                    case ExecutionLogEventType.BeginSection:
                        writer.WriteLine("[Execution Log: Being Section ({0})]", stepName);
                        writer.WriteLine("\tStream Name: {0}", e.StreamName);
                        writer.WriteLine("\tSection Name: {0}", e.SectionName);
                        break;

                    case ExecutionLogEventType.EndSection:
                        writer.WriteLine("[Execution Log: End Section ({0})]", stepName);
                        writer.WriteLine("\tStream Name: {0}", e.StreamName);
                        break;

                    case ExecutionLogEventType.Close:
                        writer.WriteLine("[Execution Log: Close ({0})]", stepName);
                        break;
                }

                writer.WriteLine();
            }
        }

        private string GetStepName(string stepId)
        {
            string stepName;
            return stepNames.TryGetValue(stepId, out stepName) ? stepName : stepId;
        }
    }
}
