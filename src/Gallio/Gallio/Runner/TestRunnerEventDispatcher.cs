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
        public event EventHandler<TestStepLogTextAttachmentAddedEventArgs> TestStepLogTextAttachmentAdded;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogBinaryAttachmentAddedEventArgs> TestStepLogBinaryAttachmentAdded;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamTextWrittenEventArgs> TestStepLogStreamTextWritten;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamAttachmentEmbeddedEventArgs> TestStepLogStreamAttachmentEmbedded;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamSectionStartedEventArgs> TestStepLogStreamSectionStarted;

        /// <inheritdoc/>
        public event EventHandler<TestStepLogStreamSectionFinishedEventArgs> TestStepLogStreamSectionFinished;

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
        /// Dispatches the <see cref="TestStepLogTextAttachmentAdded" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogTextAttachmentAdded(TestStepLogTextAttachmentAddedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogTextAttachmentAdded, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogBinaryAttachmentAdded" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogBinaryAttachmentAdded(TestStepLogBinaryAttachmentAddedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogBinaryAttachmentAdded, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamTextWritten" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogStreamTextWritten(TestStepLogStreamTextWrittenEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogStreamTextWritten, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamAttachmentEmbedded" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogStreamAttachmentEmbedded(TestStepLogStreamAttachmentEmbeddedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogStreamAttachmentEmbedded, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamSectionStarted" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogStreamSectionStarted(TestStepLogStreamSectionStartedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogStreamSectionStarted, this, e);
        }

        /// <summary>
        /// Dispatches the <see cref="TestStepLogStreamSectionFinished" /> event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void NotifyTestStepLogStreamSectionFinished(TestStepLogStreamSectionFinishedEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(TestStepLogStreamSectionFinished, this, e);
        }
    }
}
