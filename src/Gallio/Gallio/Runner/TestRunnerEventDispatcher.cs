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
using Gallio.Runner.Events;
using Gallio.Utilities;

namespace Gallio.Runner
{
    /// <summary>
    /// Dispatches test runner events to listeners.
    /// Each notification function ensures that the event is safely dispatched
    /// via <see cref="EventHandlerUtils.SafeInvoke" />.
    /// </summary>
    public sealed class TestRunnerEventDispatcher : ITestRunnerEvents
    {
        /// <inheritdoc/>
        public event EventHandler<InitializeStartedEventArgs> InitializeStarted;

        /// <inheritdoc/>
        public event EventHandler<InitializeFinishedEventArgs> InitializeFinished;

        /// <inheritdoc/>
        public event EventHandler<DisposeStartedEventArgs> DisposeStarted;
        
        /// <inheritdoc/>
        public event EventHandler<DisposeFinishedEventArgs> DisposeFinished;

        /// <inheritdoc/>
        public event EventHandler<LoadStartedEventArgs> LoadStarted;

        /// <inheritdoc/>
        public event EventHandler<LoadFinishedEventArgs> LoadFinished;

        /// <inheritdoc/>
        public event EventHandler<ExploreStartedEventArgs> ExploreStarted;

        /// <inheritdoc/>
        public event EventHandler<ExploreFinishedEventArgs> ExploreFinished;

        /// <inheritdoc/>
        public event EventHandler<RunStartedEventArgs> RunStarted;

        /// <inheritdoc/>
        public event EventHandler<RunFinishedEventArgs> RunFinished;

        /// <inheritdoc/>
        public event EventHandler<UnloadStartedEventArgs> UnloadStarted;

        /// <inheritdoc/>
        public event EventHandler<UnloadFinishedEventArgs> UnloadFinished;

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
        public event EventHandler<TestStepLogStreamBeginSectionEventArgs> TestStepLogStreamBeginSection;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamBeginMarkerEventArgs> TestStepLogStreamBeginMarker;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamEndEventArgs> TestStepLogStreamEnd;

        /// <summary>
        /// Dispatches the <see cref="InitializeStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyInitializeStarted(InitializeStartedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(InitializeStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="InitializeFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyInitializeFinished(InitializeFinishedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(InitializeFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="DisposeStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyDisposeStarted(DisposeStartedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(DisposeStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="DisposeFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyDisposeFinished(DisposeFinishedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(DisposeFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="LoadStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyLoadStarted(LoadStartedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(LoadStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="LoadFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyLoadFinished(LoadFinishedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(LoadFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="ExploreStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyExploreStarted(ExploreStartedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(ExploreStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="ExploreFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyExploreFinished(ExploreFinishedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(ExploreFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="RunStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyRunStarted(RunStartedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(RunStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="RunFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyRunFinished(RunFinishedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(RunFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="UnloadStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyUnloadStarted(UnloadStartedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(UnloadStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="UnloadFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyUnloadFinished(UnloadFinishedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(UnloadFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepStarted(TestStepStartedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepFinished(TestStepFinishedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepFinished, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLifecyclePhaseChanged" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLifecyclePhaseChanged(TestStepLifecyclePhaseChangedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLifecyclePhaseChanged, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepMetadataAdded" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepMetadataAdded(TestStepMetadataAddedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepMetadataAdded, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogAttach" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogAttach(TestStepLogAttachEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogAttach, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamWrite" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogStreamWrite(TestStepLogStreamWriteEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogStreamWrite, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamEmbed" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogStreamEmbed(TestStepLogStreamEmbedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogStreamEmbed, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamBeginSection" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogStreamBeginSection(TestStepLogStreamBeginSectionEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogStreamBeginSection, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamBeginMarker" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogStreamBeginMarker(TestStepLogStreamBeginMarkerEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogStreamBeginMarker, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamEnd" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogStreamEnd(TestStepLogStreamEndEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogStreamEnd, this, e);
        }

        /// <summary>
        /// Subscribes to another event object such that the events will be forwarded
        /// to the handlers registered on this dispatcher.
        /// </summary>
        /// <param name="events">The other events object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="events"/> is null</exception>
        public void SubscribeTo(ITestRunnerEvents events)
        {
            if (events == null)
                throw new ArgumentNullException("events");

            events.DisposeFinished += (sender, e) => NotifyDisposeFinished(e);
            events.DisposeStarted += (sender, e) => NotifyDisposeStarted(e);
            events.ExploreFinished += (sender, e) => NotifyExploreFinished(e);
            events.ExploreStarted += (sender, e) => NotifyExploreStarted(e);
            events.InitializeFinished += (sender, e) => NotifyInitializeFinished(e);
            events.InitializeStarted += (sender, e) => NotifyInitializeStarted(e);
            events.LoadFinished += (sender, e) => NotifyLoadFinished(e);
            events.LoadStarted += (sender, e) => NotifyLoadStarted(e);
            events.RunFinished += (sender, e) => NotifyRunFinished(e);
            events.RunStarted += (sender, e) => NotifyRunStarted(e);
            events.TestStepFinished += (sender, e) => NotifyTestStepFinished(e);
            events.TestStepLifecyclePhaseChanged += (sender, e) => NotifyTestStepLifecyclePhaseChanged(e);
            events.TestStepLogAttach += (sender, e) => NotifyTestStepLogAttach(e);
            events.TestStepLogStreamBeginMarker += (sender, e) => NotifyTestStepLogStreamBeginMarker(e);
            events.TestStepLogStreamBeginSection += (sender, e) => NotifyTestStepLogStreamBeginSection(e);
            events.TestStepLogStreamEmbed += (sender, e) => NotifyTestStepLogStreamEmbed(e);
            events.TestStepLogStreamEnd += (sender, e) => NotifyTestStepLogStreamEnd(e);
            events.TestStepLogStreamWrite += (sender, e) => NotifyTestStepLogStreamWrite(e);
            events.TestStepMetadataAdded += (sender, e) => NotifyTestStepMetadataAdded(e);
            events.TestStepStarted += (sender, e) => NotifyTestStepStarted(e);
            events.UnloadFinished += (sender, e) => NotifyUnloadFinished(e);
            events.UnloadStarted += (sender, e) => NotifyUnloadStarted(e);
        }
    }
}
