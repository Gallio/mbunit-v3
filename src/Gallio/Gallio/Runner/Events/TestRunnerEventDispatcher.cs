// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Policies;
using Gallio.Runtime.Logging;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Dispatches test runner events to listeners.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each notification function ensures that the event is safely dispatched
    /// via <see cref="EventHandlerPolicy.SafeInvoke" />.
    /// </para>
    /// </remarks>
    public sealed class TestRunnerEventDispatcher : ITestRunnerEvents
    {
        /// <inheritdoc/>
        public event EventHandler<LogEntrySubmittedEventArgs> LogEntrySubmitted;

        /// <inheritdoc/>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <inheritdoc/>
        public event EventHandler<InitializeStartedEventArgs> InitializeStarted;

        /// <inheritdoc/>
        public event EventHandler<InitializeFinishedEventArgs> InitializeFinished;

        /// <inheritdoc/>
        public event EventHandler<DisposeStartedEventArgs> DisposeStarted;
        
        /// <inheritdoc/>
        public event EventHandler<DisposeFinishedEventArgs> DisposeFinished;

        /// <inheritdoc/>
        public event EventHandler<ExploreStartedEventArgs> ExploreStarted;

        /// <inheritdoc/>
        public event EventHandler<ExploreFinishedEventArgs> ExploreFinished;

        /// <inheritdoc/>
        public event EventHandler<RunStartedEventArgs> RunStarted;

        /// <inheritdoc/>
        public event EventHandler<RunFinishedEventArgs> RunFinished;

        /// <inheritdoc/>
        public event EventHandler<TestDiscoveredEventArgs> TestDiscovered;

        /// <inheritdoc/>
        public event EventHandler<AnnotationDiscoveredEventArgs> AnnotationDiscovered;

        /// <inheritdoc/>
        public event EventHandler<TestStepStartedEventArgs> TestStepStarted;

        /// <inheritdoc/>
        public event EventHandler<TestStepFinishedEventArgs> TestStepFinished;

        /// <inheritdoc/>
        public event EventHandler<TestStepLifecyclePhaseChangedEventArgs> TestStepLifecyclePhaseChanged;

        /// <inheritdoc/>
        public event EventHandler<TestStepMetadataAddedEventArgs> TestStepMetadataAdded;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogAttachEventArgs> TestStepLogAttach;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamWriteEventArgs> TestStepLogStreamWrite;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamEmbedEventArgs> TestStepLogStreamEmbed;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamBeginSectionBlockEventArgs> TestStepLogStreamBeginSectionBlock;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamBeginMarkerBlockEventArgs> TestStepLogStreamBeginMarkerBlock;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamEndBlockEventArgs> TestStepLogStreamEndBlock;

        /// <summary>
        /// Dispatches the <see cref="LogEntrySubmitted" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyLogEntrySubmitted(LogEntrySubmittedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(LogEntrySubmitted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="MessageReceived" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyMessageReceived(MessageReceivedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(MessageReceived, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="InitializeStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyInitializeStarted(InitializeStartedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(InitializeStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="InitializeFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyInitializeFinished(InitializeFinishedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(InitializeFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="DisposeStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyDisposeStarted(DisposeStartedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(DisposeStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="DisposeFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyDisposeFinished(DisposeFinishedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(DisposeFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="ExploreStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyExploreStarted(ExploreStartedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(ExploreStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="ExploreFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyExploreFinished(ExploreFinishedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(ExploreFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="RunStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyRunStarted(RunStartedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(RunStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="RunFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyRunFinished(RunFinishedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(RunFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestDiscovered" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestDiscovered(TestDiscoveredEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestDiscovered, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="AnnotationDiscovered" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyAnnotationDiscovered(AnnotationDiscoveredEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(AnnotationDiscovered, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepStarted(TestStepStartedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepFinished(TestStepFinishedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLifecyclePhaseChanged" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepLifecyclePhaseChanged(TestStepLifecyclePhaseChangedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepLifecyclePhaseChanged, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepMetadataAdded" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepMetadataAdded(TestStepMetadataAddedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepMetadataAdded, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogAttach" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepLogAttach(TestStepLogAttachEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepLogAttach, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamWrite" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepLogStreamWrite(TestStepLogStreamWriteEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepLogStreamWrite, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamEmbed" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepLogStreamEmbed(TestStepLogStreamEmbedEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepLogStreamEmbed, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamBeginSectionBlock" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepLogStreamBeginSectionBlock(TestStepLogStreamBeginSectionBlockEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepLogStreamBeginSectionBlock, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamBeginMarkerBlock" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepLogStreamBeginMarkerBlock(TestStepLogStreamBeginMarkerBlockEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepLogStreamBeginMarkerBlock, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamEndBlock" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void NotifyTestStepLogStreamEndBlock(TestStepLogStreamEndBlockEventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(TestStepLogStreamEndBlock, this, e);
        }

        /// <summary>
        /// Subscribes to another event object such that the events will be forwarded
        /// to the handlers registered on this dispatcher.
        /// </summary>
        /// <param name="events">The other events object.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="events"/> is null.</exception>
        public void SubscribeTo(ITestRunnerEvents events)
        {
            if (events == null)
                throw new ArgumentNullException("events");

            events.LogEntrySubmitted += (sender, e) => NotifyLogEntrySubmitted(e);
            events.MessageReceived += (sender, e) => NotifyMessageReceived(e);
            events.InitializeFinished += (sender, e) => NotifyInitializeFinished(e);
            events.InitializeStarted += (sender, e) => NotifyInitializeStarted(e);
            events.DisposeFinished += (sender, e) => NotifyDisposeFinished(e);
            events.DisposeStarted += (sender, e) => NotifyDisposeStarted(e);
            events.ExploreFinished += (sender, e) => NotifyExploreFinished(e);
            events.ExploreStarted += (sender, e) => NotifyExploreStarted(e);
            events.RunFinished += (sender, e) => NotifyRunFinished(e);
            events.RunStarted += (sender, e) => NotifyRunStarted(e);
            events.TestDiscovered += (sender, e) => NotifyTestDiscovered(e);
            events.AnnotationDiscovered += (sender, e) => NotifyAnnotationDiscovered(e);
            events.TestStepFinished += (sender, e) => NotifyTestStepFinished(e);
            events.TestStepLifecyclePhaseChanged += (sender, e) => NotifyTestStepLifecyclePhaseChanged(e);
            events.TestStepLogAttach += (sender, e) => NotifyTestStepLogAttach(e);
            events.TestStepLogStreamBeginMarkerBlock += (sender, e) => NotifyTestStepLogStreamBeginMarkerBlock(e);
            events.TestStepLogStreamBeginSectionBlock += (sender, e) => NotifyTestStepLogStreamBeginSectionBlock(e);
            events.TestStepLogStreamEmbed += (sender, e) => NotifyTestStepLogStreamEmbed(e);
            events.TestStepLogStreamEndBlock += (sender, e) => NotifyTestStepLogStreamEndBlock(e);
            events.TestStepLogStreamWrite += (sender, e) => NotifyTestStepLogStreamWrite(e);
            events.TestStepMetadataAdded += (sender, e) => NotifyTestStepMetadataAdded(e);
            events.TestStepStarted += (sender, e) => NotifyTestStepStarted(e);
        }
    }
}