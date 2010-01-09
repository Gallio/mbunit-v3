// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

            Events.DisposeStarted += delegate(object sender, DisposeStartedEventArgs e)
            {
                LogDebugFormat("[DisposeStarted]");
            };

            Events.DisposeFinished += delegate(object sender, DisposeFinishedEventArgs e)
            {
                LogDebugFormat("[DisposeFinished]\n\tSuccess: {0}", e.Success);
            };

            Events.TestDiscovered += delegate(object sender, TestDiscoveredEventArgs e)
            {
                LogDebugFormat("[TestDiscovered]\n\tTest: {0}", e.Test.FullName);
            };

            Events.AnnotationDiscovered += delegate(object sender, AnnotationDiscoveredEventArgs e)
            {
                LogDebugFormat("[AnnotationDiscovered]\n\tType: {0}\n\tCode Location: {1}\n\tCode Reference: {2}\n\tMessage: {3}\n\tDetails: {4}",
                    e.Annotation.Type, e.Annotation.CodeLocation, e.Annotation.CodeReference, e.Annotation.Message, e.Annotation.Details);
            };

            Events.TestStepStarted += delegate(object sender, TestStepStartedEventArgs e)
            {
                LogDebugFormat("[TestStepStarted({0})]",
                    e.TestStepRun.Step.FullName);
            };

            Events.TestStepFinished += delegate(object sender, TestStepFinishedEventArgs e)
            {
                LogDebugFormat("[TestStepFinished({0})]\n\tOutcome: {1}\n\tAsserts: {2}\n\tDuration: {3:0.000}s",
                    e.TestStepRun.Step.FullName,
                    e.TestStepRun.Result.Outcome,
                    e.TestStepRun.Result.AssertCount,
                    e.TestStepRun.Result.DurationInSeconds);
            };

            Events.TestStepLifecyclePhaseChanged += delegate(object sender, TestStepLifecyclePhaseChangedEventArgs e)
            {
                LogDebugFormat("[TestStepLifecyclePhaseChanged({0})]\n\tPhase: {1}",
                    e.TestStepRun.Step.FullName,
                    e.LifecyclePhase);
            };

            Events.TestStepMetadataAdded += delegate(object sender, TestStepMetadataAddedEventArgs e)
            {
                LogDebugFormat("[TestStepMetadataAddedEventArgs({0})]\n\tKey: {1}\n\tValue: {2}",
                    e.TestStepRun.Step.FullName,
                    e.MetadataKey,
                    e.MetadataValue);
            };

            Events.TestStepLogAttach += delegate(object sender, TestStepLogAttachEventArgs e)
            {
                LogDebugFormat("[TestStepLogAttach({0})]\n\tName: {1}\n\tContentType: {2}",
                    e.TestStepRun.Step.FullName,
                    e.Attachment.Name,
                    e.Attachment.ContentType);
            };

            Events.TestStepLogStreamWrite += delegate(object sender, TestStepLogStreamWriteEventArgs e)
            {
                LogDebugFormat("[TestStepLogStreamWrite({0})]\n\tStream: {1}\n\tText: {2}",
                    e.TestStepRun.Step.FullName,
                    e.LogStreamName,
                    e.Text);
            };

            Events.TestStepLogStreamEmbed += delegate(object sender, TestStepLogStreamEmbedEventArgs e)
            {
                LogDebugFormat("[TestStepLogStreamEmbed({0})]\n\tStream: {1}\n\tAttachmentName: {2}",
                    e.TestStepRun.Step.FullName,
                    e.LogStreamName,
                    e.AttachmentName);
            };

            Events.TestStepLogStreamBeginSectionBlock += delegate(object sender, TestStepLogStreamBeginSectionBlockEventArgs e)
            {
                LogDebugFormat("[TestStepLogStreamBeginSectionBlock({0})]\n\tStream: {1}\n\tSection Name: {2}",
                    e.TestStepRun.Step.FullName,
                    e.LogStreamName,
                    e.SectionName);
            };

            Events.TestStepLogStreamBeginMarkerBlock += delegate(object sender, TestStepLogStreamBeginMarkerBlockEventArgs e)
            {
                LogDebugFormat("[TestStepLogStreamBeginMarkerBlock({0})]\n\tStream: {1}\n\tClass: {2}",
                    e.TestStepRun.Step.FullName,
                    e.LogStreamName,
                    e.Marker);
            };

            Events.TestStepLogStreamEndBlock += delegate(object sender, TestStepLogStreamEndBlockEventArgs e)
            {
                LogDebugFormat("[TestStepLogStreamEndBlock({0})]\n\tStream: {1}",
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
