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
        private readonly Dictionary<string, string> stepNames = new Dictionary<string, string>();

        /// <summary>
        /// Creates a console monitor.
        /// </summary>
        /// <param name="writer">The text writer for all output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public DebugMonitor(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(@"writer");

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

        /// <inheritdoc />
        protected override void OnDetach()
        {
            base.OnDetach();

            Runner.EventDispatcher.Message -= HandleMessageEvent;
            Runner.EventDispatcher.Lifecycle -= HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog -= HandleExecutionLogEvent;
        }

        private void HandleMessageEvent(object sender, MessageEventArgs e)
        {
            lock (this)
            {
                writer.WriteLine(Resources.DebugMonitor_MessageEvent_EventFormat, e.MessageType, e.Message);
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
                        stepName = e.StepInfo.FullName;
                        stepNames.Add(e.StepId, stepName);

                        writer.WriteLine(Resources.DebugMonitor_LifecycleEvent_Start_EventFormat, stepName);
                        break;

                    case LifecycleEventType.EnterPhase:
                        writer.WriteLine(Resources.DebugMonitor_LifecycleEvent_Phase_EventFormat, stepName);
                        writer.WriteLine(Resources.DebugMonitor_LifecycleEvent_Phase_NameFormat, e.PhaseName);
                        break;

                    case LifecycleEventType.Finish:
                        writer.WriteLine(Resources.DebugMonitor_LifecycleEvent_Finish_EventFormat, stepName);
                        writer.WriteLine(Resources.DebugMonitor_LifecycleEvent_Finish_StateFormat, e.Result.State);
                        writer.WriteLine(Resources.DebugMonitor_LifecycleEvent_Finish_OutcomeFormat, e.Result.Outcome);
                        writer.WriteLine(Resources.DebugMonitor_LifecycleEvent_Finish_AssertCountFormat, e.Result.AssertCount);
                        writer.WriteLine(Resources.DebugMonitor_LifecycleEvent_Finish_DurationFormat, e.Result.Duration);
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
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_WriteText_EventFormat, stepName);
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_StreamNameFormat, e.StreamName);
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_TextFormat, e.Text);
                        break;

                    case ExecutionLogEventType.WriteAttachment:
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_WriteAttachment_EventFormat, stepName);
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_StreamNameFormat, e.StreamName ?? @"<null>");
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_WriteAttachment_AttachmentNameFormat, e.Attachment.Name);
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_WriteAttachment_ContentTypeFormat, e.Attachment.ContentType);
                        break;

                    case ExecutionLogEventType.BeginSection:
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_BeginSection_EventFormat, stepName);
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_StreamNameFormat, e.StreamName);
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_BeginSection_SectionNameFormat, e.SectionName);
                        break;

                    case ExecutionLogEventType.EndSection:
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_EndSection_EventFormat, stepName);
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_StreamNameFormat, e.StreamName);
                        break;

                    case ExecutionLogEventType.Close:
                        writer.WriteLine(Resources.DebugMonitor_ExecutionLogEvent_Close_EventFormat, stepName);
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
