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
using Gallio.Runner.Events;
using Gallio.Runtime.Logging;
using Gallio.Runner;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Monitors <see cref="ITestRunner" /> events and writes debug messages to the
    /// runner's logger.
    /// </summary>
    public class DebugExtension : TestRunnerExtension
    {
        /// <inheritdoc />
        protected override void Initialize()
        {
            Events.InitializeStarted += delegate(object sender, InitializeStartedEventArgs e)
            {
                LogDebugFormat("[InitializeStarted]");
            };

            Events.InitializeFinished += delegate(object sender, InitializeFinishedEventArgs e)
            {
                LogDebugFormat("[InitializeFinished]\n\tSuccess: {0}", e.Success);
            };

            Events.LoadStarted += delegate(object sender, LoadStartedEventArgs e)
            {
                LogDebugFormat("[LoadStarted]");
            };

            Events.LoadFinished += delegate(object sender, LoadFinishedEventArgs e)
            {
                LogDebugFormat("[LoadFinished]\n\tSuccess: {0}", e.Success);
            };

            Events.ExploreStarted += delegate(object sender, ExploreStartedEventArgs e)
            {
                LogDebugFormat("[ExploreStarted]");
            };

            Events.ExploreFinished += delegate(object sender, ExploreFinishedEventArgs e)
            {
                LogDebugFormat("[ExploreFinished]\n\tSuccess: {0}", e.Success);
            };

            Events.RunStarted += delegate(object sender, RunStartedEventArgs e)
            {
                LogDebugFormat("[RunStarted]");
            };

            Events.RunFinished += delegate(object sender, RunFinishedEventArgs e)
            {
                LogDebugFormat("[RunFinished]\n\tSuccess: {0}", e.Success);
            };

            Events.UnloadStarted += delegate(object sender, UnloadStartedEventArgs e)
            {
                LogDebugFormat("[UnloadStarted]");
            };

            Events.UnloadFinished += delegate(object sender, UnloadFinishedEventArgs e)
            {
                LogDebugFormat("[UnloadFinished]\n\tSuccess: {0}", e.Success);
            };

            Events.DisposeStarted += delegate(object sender, DisposeStartedEventArgs e)
            {
                LogDebugFormat("[DisposeStarted]");
            };

            Events.DisposeFinished += delegate(object sender, DisposeFinishedEventArgs e)
            {
                LogDebugFormat("[DisposeFinished]\n\tSuccess: {0}", e.Success);
            };

            Events.TestStepStarted += delegate(object sender, TestStepStartedEventArgs e)
            {
                LogDebugFormat("[TestStepStarted({0}]",
                    e.TestStepRun.Step.FullName);
            };

            Events.TestStepFinished += delegate(object sender, TestStepFinishedEventArgs e)
            {
                LogDebugFormat("[TestStepFinished({0})]\n\tOutcome: {1}\n\tAsserts: {2}\n\tDuration: {3:0.000}s",
                    e.TestStepRun.Step.FullName,
                    e.TestStepRun.Result.Outcome,
                    e.TestStepRun.Result.AssertCount,
                    e.TestStepRun.Result.Duration);
            };

            Events.TestStepLifecyclePhaseChanged += delegate(object sender, TestStepLifecyclePhaseChangedEventArgs e)
            {
                LogDebugFormat("[TestStepLifecyclePhaseChanged({0}]\n\tPhase: {1}",
                    e.TestStepRun.Step.FullName,
                    e.LifecyclePhase);
            };

            Events.TestStepMetadataAdded += delegate(object sender, TestStepMetadataAddedEventArgs e)
            {
                LogDebugFormat("[TestStepMetadataAddedEventArgs({0}]\n\tKey: {1}\n\tValue: {2}",
                    e.TestStepRun.Step.FullName,
                    e.MetadataKey,
                    e.MetadataValue);
            };

            Events.TestStepLogBinaryAttachmentAdded += delegate(object sender, TestStepLogBinaryAttachmentAddedEventArgs e)
            {
                LogDebugFormat("[TestStepLogBinaryAttachmentAdded({0}]\n\tName: {1}\n\tContentType: {2}",
                    e.TestStepRun.Step.FullName,
                    e.AttachmentName,
                    e.ContentType);
            };

            Events.TestStepLogTextAttachmentAdded += delegate(object sender, TestStepLogTextAttachmentAddedEventArgs e)
            {
                LogDebugFormat("[TestStepLogTextAttachmentAdded({0}]\n\tName: {1}\n\tContentType: {2}",
                    e.TestStepRun.Step.FullName,
                    e.AttachmentName,
                    e.ContentType);
            };

            Events.TestStepLogStreamTextWritten += delegate(object sender, TestStepLogStreamTextWrittenEventArgs e)
            {
                LogDebugFormat("[TestStepLogStreamTextWritten({0})]\n\tStream: {1}\n\tText: {2}",
                    e.TestStepRun.Step.FullName,
                    e.LogStreamName,
                    e.Text);
            };

            Events.TestStepLogStreamAttachmentEmbedded += delegate(object sender, TestStepLogStreamAttachmentEmbeddedEventArgs e)
            {
                LogDebugFormat("[TestStepLogStreamAttachmentEmbedded({0})]\n\tStream: {1}\n\tAttachmentName: {2}",
                    e.TestStepRun.Step.FullName,
                    e.LogStreamName,
                    e.AttachmentName);
            };

            Events.TestStepLogStreamSectionStarted += delegate(object sender, TestStepLogStreamSectionStartedEventArgs e)
            {
                LogDebugFormat("[TestStepLogStreamSectionStarted({0})]\n\tStream: {1}\n\tSection Name: {2}",
                    e.TestStepRun.Step.FullName,
                    e.LogStreamName,
                    e.SectionName);
            };

            Events.TestStepLogStreamSectionFinished += delegate(object sender, TestStepLogStreamSectionFinishedEventArgs e)
            {
                LogDebugFormat("[TestStepLogStreamSectionStarted({0})]\n\tStream: {1}",
                    e.TestStepRun.Step.FullName,
                    e.LogStreamName);
            };
        }

        private void LogDebugFormat(string format, params object[] args)
        {
            Logger.Log(LogSeverity.Debug, String.Format(format, args));
        }
    }
}
