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
using Gallio.Runtime.Logging;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Declares all of the events that may be raised by a <see cref="ITestRunner" />.
    /// </summary>
    public interface ITestRunnerEvents
    {
        /// <summary>
        /// The event raised to indicate that a log message was written by the test runner.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The runner automatically writes out these log messages to the logger with which it
        /// is initialized.  These events are raised to allow other subscribers to receive
        /// log messages as well.
        /// </para>
        /// </remarks>
        event EventHandler<LogMessageEventArgs> LogMessage;

        /// <summary>
        /// The event raised to indicate that a raw message was received by the test runner.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// The event raised to indicate that the test runner initialization has started.
        /// </summary>
        event EventHandler<InitializeStartedEventArgs> InitializeStarted;

        /// <summary>
        /// The event raised to indicate that the test runner initialization has finished.
        /// </summary>
        event EventHandler<InitializeFinishedEventArgs> InitializeFinished;

        /// <summary>
        /// The event raised to indicate that the test runner disposal has started.
        /// </summary>
        event EventHandler<DisposeStartedEventArgs> DisposeStarted;

        /// <summary>
        /// The event raised to indicate that the test runner disposal has finished.
        /// </summary>
        event EventHandler<DisposeFinishedEventArgs> DisposeFinished;

        /// <summary>
        /// The event raised to indicate that test exploration has started.
        /// </summary>
        event EventHandler<ExploreStartedEventArgs> ExploreStarted;

        /// <summary>
        /// The event raised to indicate that test exploration has finished.
        /// </summary>
        event EventHandler<ExploreFinishedEventArgs> ExploreFinished;

        /// <summary>
        /// The event raised to indicate that test execution has started.
        /// </summary>
        event EventHandler<RunStartedEventArgs> RunStarted;

        /// <summary>
        /// The event raised to indicate that test execution has finished.
        /// </summary>
        event EventHandler<RunFinishedEventArgs> RunFinished;

        /// <summary>
        /// The event raised to indicate that a subtree of tests was merged into the test model.
        /// </summary>
        event EventHandler<TestDiscoveredEventArgs> TestDiscovered;

        /// <summary>
        /// The event raised to indicate that an annotation was added to the test model.
        /// </summary>
        event EventHandler<AnnotationDiscoveredEventArgs> AnnotationDiscovered;

        /// <summary>
        /// The event raised to indicate that a test step has started execution.
        /// </summary>
        event EventHandler<TestStepStartedEventArgs> TestStepStarted;

        /// <summary>
        /// The event raised to indicate that a test step has finished execution.
        /// </summary>
        event EventHandler<TestStepFinishedEventArgs> TestStepFinished;

        /// <summary>
        /// The event raised to indicate that a test step has entered a new lifecycle phase.
        /// </summary>
        event EventHandler<TestStepLifecyclePhaseChangedEventArgs> TestStepLifecyclePhaseChanged;

        /// <summary>
        /// The event raised to indicate that a test step dynamically added metadata to itself.
        /// </summary>
        event EventHandler<TestStepMetadataAddedEventArgs> TestStepMetadataAdded;

        /// <summary>
        /// The event raised to indicate that an attachment has been added to a test step log.
        /// </summary>
        event EventHandler<TestStepLogAttachEventArgs> TestStepLogAttach;

        /// <summary>
        /// The event raised to indicate that text has been written to a test step log stream.
        /// </summary>
        event EventHandler<TestStepLogStreamWriteEventArgs> TestStepLogStreamWrite;

        /// <summary>
        /// The event raised to indicate that an attachment has been embedded into a test step log stream.
        /// </summary>
        event EventHandler<TestStepLogStreamEmbedEventArgs> TestStepLogStreamEmbed;

        /// <summary>
        /// The event raised to indicate that a section has been started within a test step log stream.
        /// </summary>
        event EventHandler<TestStepLogStreamBeginSectionBlockEventArgs> TestStepLogStreamBeginSectionBlock;

        /// <summary>
        /// The event raised to indicate that a marker has been started within a test step log stream.
        /// </summary>
        event EventHandler<TestStepLogStreamBeginMarkerBlockEventArgs> TestStepLogStreamBeginMarkerBlock;

        /// <summary>
        /// The event raised to indicate that a block has finished within a test step log stream.
        /// </summary>
        event EventHandler<TestStepLogStreamEndBlockEventArgs> TestStepLogStreamEndBlock;
    }
}
