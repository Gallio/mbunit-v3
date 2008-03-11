// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Castle.Core.Logging;
using Gallio.Runner;
using Gallio.Model.Execution;
using Gallio.Properties;

namespace Gallio.Runner.Monitors
{
    /// <summary>
    /// Monitors <see cref="ITestRunner" /> events and writes debug messages to a logger.
    /// </summary>
    public class DebugMonitor : BaseTestRunnerMonitor
    {
        private readonly ILogger logger;
        private readonly Dictionary<string, string> stepNames = new Dictionary<string, string>();

        /// <summary>
        /// Creates a console monitor.
        /// </summary>
        /// <param name="logger">The logger for writing debug output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null</exception>
        public DebugMonitor(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(@"logger");

            this.logger = logger;
        }

        /// <inheritdoc />
        protected override void OnAttach()
        {
            base.OnAttach();

            Runner.EventDispatcher.Lifecycle += HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog += HandleExecutionLogEvent;
        }

        /// <inheritdoc />
        protected override void OnDetach()
        {
            base.OnDetach();

            Runner.EventDispatcher.Lifecycle -= HandleLifecycleEvent;
            Runner.EventDispatcher.ExecutionLog -= HandleExecutionLogEvent;
        }

        private void HandleLifecycleEvent(object sender, LifecycleEventArgs e)
        {
            switch (e.EventType)
            {
                case LifecycleEventType.NewInstance:
                    logger.DebugFormat(Resources.DebugMonitor_LifecycleEvent_NewInstance_EventFormat, e.TestInstanceData.Name);
                    break;

                case LifecycleEventType.Start:
                    {
                        string stepName = e.TestStepData.FullName;
                        lock (stepNames)
                            stepNames.Add(e.StepId, stepName);

                        logger.DebugFormat(Resources.DebugMonitor_LifecycleEvent_Start_EventFormat, stepName);
                        break;
                    }

                case LifecycleEventType.SetPhase:
                    logger.DebugFormat(Resources.DebugMonitor_LifecycleEvent_SetPhase_EventFormat, GetStepName(e.StepId), e.PhaseName);
                    break;

                case LifecycleEventType.AddMetadata:
                    logger.DebugFormat(Resources.DebugMonitor_LifecycleEvent_AddMetadata_EventFormat, GetStepName(e.StepId), e.MetadataKey, e.MetadataValue);
                    break;

                case LifecycleEventType.Finish:
                    logger.DebugFormat(Resources.DebugMonitor_LifecycleEvent_Finish_EventFormat,
                        GetStepName(e.StepId), e.Result.Outcome, e.Result.AssertCount, e.Result.Duration);
                    break;
            }
        }

        private void HandleExecutionLogEvent(object sender, LogEventArgs e)
        {
            string stepName = GetStepName(e.StepId);

            switch (e.EventType)
            {
                case LogEventType.AttachText:
                    logger.DebugFormat(Resources.DebugMonitor_ExecutionLogEvent_AttachText_EventFormat,
                        stepName, e.AttachmentName, e.ContentType);
                    break;

                case LogEventType.AttachBytes:
                    logger.DebugFormat(Resources.DebugMonitor_ExecutionLogEvent_AttachBytes_EventFormat,
                        stepName, e.AttachmentName, e.ContentType);
                    break;

                case LogEventType.Embed:
                    logger.DebugFormat(Resources.DebugMonitor_ExecutionLogEvent_EmbedExisting_EventFormat,
                        stepName, e.StreamName, e.AttachmentName);
                    break;

                case LogEventType.Write:
                    logger.DebugFormat(Resources.DebugMonitor_ExecutionLogEvent_Write_EventFormat,
                        stepName, e.StreamName, e.Text);
                    break;

                case LogEventType.BeginSection:
                    logger.DebugFormat(Resources.DebugMonitor_ExecutionLogEvent_BeginSection_EventFormat,
                        stepName, e.StreamName, e.SectionName);
                    break;

                case LogEventType.EndSection:
                    logger.DebugFormat(Resources.DebugMonitor_ExecutionLogEvent_EndSection_EventFormat,
                        stepName, e.StreamName);
                    break;
            }
        }

        private string GetStepName(string stepId)
        {
            lock (stepNames)
            {
                string stepName;
                return stepNames.TryGetValue(stepId, out stepName) ? stepName : stepId;
            }
        }
    }
}
