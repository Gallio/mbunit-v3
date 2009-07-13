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
using System.Diagnostics;
using System.Threading;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Common.Markup;
using Gallio.Model.Messages;
using Gallio.Model.Messages.Execution;
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.Common.Messaging;

namespace Gallio.Model.Contexts
{
    /// <summary>
    /// An observable test context monitors translates state changes on the
    /// test context into notifications on a <see cref="IMessageSink" />.
    /// </summary>
    internal class ObservableTestContext : ITestContext
    {
        private const int StatusCreated = 0;
        private const int StatusStarted = 1;
        private const int StatusFinishing = 2;
        private const int StatusFinished = 3;

        private readonly object syncRoot = new object();
        private readonly ObservableTestContextManager manager;
        private readonly ITestContext parent;
        private readonly TestStep testStep;
        private readonly ObservableTestLogWriter logWriter;
        private readonly FallbackMarkupDocumentWriter externallyVisibleLogWriter;
        private UserDataCollection data;

        private string lifecyclePhase = @"";
        private TestOutcome outcome = TestOutcome.Passed;
        private event EventHandler finishingHandlers;
        private int assertCount;

        private int executionStatus;
        private Stopwatch stopwatch;
        private IDisposable contextCookie;

        /// <summary>
        /// Creates an observable test context.
        /// </summary>
        /// <param name="manager">The test context manager.</param>
        /// <param name="testStep">The test step.</param>
        /// <param name="parent">The parent test context.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="manager"/> or <paramref name="testStep"/> is null.</exception>
        public ObservableTestContext(ObservableTestContextManager manager, TestStep testStep, ITestContext parent)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");
            if (testStep == null)
                throw new ArgumentNullException("testStep");

            this.manager = manager;
            this.testStep = testStep;
            this.parent = parent;

            logWriter = new ObservableTestLogWriter(MessageSink, testStep.Id);
            externallyVisibleLogWriter = new FallbackMarkupDocumentWriter(logWriter, 
                parent != null ? parent.LogWriter : new NullMarkupDocumentWriter());

            data = new UserDataCollection();
        }

        public ITestContext Parent
        {
            get { return parent; }
        }

        public TestStep TestStep
        {
            get { return testStep; }
        }

        public MarkupDocumentWriter LogWriter
        {
            get { return externallyVisibleLogWriter; }
        }

        public string LifecyclePhase
        {
            get { return lifecyclePhase; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                lock (syncRoot)
                {
                    if (! IsRunning)
                        throw new InvalidOperationException("Cannot set the lifecycle phase unless the test step is running.");

                    if (lifecyclePhase == value)
                        return;

                    lifecyclePhase = value;
                    MessageSink.Publish(new TestStepLifecyclePhaseChangedMessage()
                    {
                        StepId = testStep.Id,
                        LifecyclePhase = lifecyclePhase
                    });
                }
            }
        }

        public TestOutcome Outcome
        {
            get { return outcome; }
        }

        public UserDataCollection Data
        {
            get { return data; }
        }

        public int AssertCount
        {
            get { return assertCount; }
        }

        public IMessageSink MessageSink
        {
            get { return manager.MessageSink; }
        }

        public bool IsFinished
        {
            get { return executionStatus == StatusFinished; }
        }

        public event EventHandler Finishing
        {
            add
            {
                lock (syncRoot)
                {
                    if (executionStatus < StatusFinishing)
                    {
                        finishingHandlers += value;
                        return;
                    }
                }

                EventHandlerPolicy.SafeInvoke(value, this, EventArgs.Empty);
            }
            remove
            {
                lock (syncRoot)
                    finishingHandlers -= value;
            }
        }

        public void AddAssertCount(int value)
        {
            Interlocked.Add(ref assertCount, value);

            if (parent != null)
                parent.AddAssertCount(value);
        }

        public void AddMetadata(string metadataKey, string metadataValue)
        {
            if (metadataKey == null)
                throw new ArgumentNullException(@"metadataKey");
            if (metadataValue == null)
                throw new ArgumentNullException(@"metadataValue");

            lock (syncRoot)
            {
                if (! IsRunning)
                    throw new InvalidOperationException("Cannot add metadata unless the test step is running.");

                testStep.Metadata.Add(metadataKey, metadataValue);
                MessageSink.Publish(new TestStepMetadataAddedMessage()
                {
                    StepId = testStep.Id,
                    MetadataKey = metadataKey,
                    MetadataValue = metadataValue
                });
            }
        }

        public void SetInterimOutcome(TestOutcome outcome)
        {
            lock (syncRoot)
            {
                if (! IsRunning)
                    throw new InvalidOperationException("Cannot set the interim outcome unless the test step is running.");

                this.outcome = outcome;
            }
        }

        public ITestContext StartChildStep(TestStep childStep)
        {
            if (childStep == null)
                throw new ArgumentNullException(@"childStep");
            if (childStep.Parent != testStep)
                throw new ArgumentException("Expected a child of this step.", "childStep");

            lock (syncRoot)
            {
                if (!IsRunning)
                    throw new InvalidOperationException("Cannot start a new child step unless the test step is running.");
            }

            return manager.StartStep(childStep);
        }

        public void FinishStep(TestOutcome outcome, TimeSpan? actualDuration)
        {
            FinishStep(outcome, actualDuration, false);
        }

        public void Dispose()
        {
            FinishStep(TestOutcome.Error, null, true);
        }

        internal void InitializeAndStartStep()
        {
            lock (syncRoot)
            {
                if (executionStatus != StatusCreated)
                    throw new InvalidOperationException("Cannot initialize and start the test step twice.");

                stopwatch = Stopwatch.StartNew();

                // Dispatch the start notification.
                MessageSink.Publish(new TestStepStartedMessage()
                {
                    Step = new TestStepData(testStep),
                    CodeElement = testStep.CodeElement
                });

                // Consider the test started.
                executionStatus = StatusStarted;
                LifecyclePhase = LifecyclePhases.Starting;

                // Enter the context.
                contextCookie = Enter();
            }

            // Note: We exit the lock before manipulating the parent context to avoid reentry.
            if (parent != null)
                parent.Finishing += HandleParentFinishedBeforeThisContext;
        }

        private void FinishStep(TestOutcome outcome, TimeSpan? actualDuration, bool isDisposing)
        {
            EventHandler cachedFinishingHandlers;
            lock (syncRoot)
            {
                if (! IsRunning)
                {
                    if (isDisposing)
                        return;
                    throw new InvalidOperationException("Cannot finish a step unless the test step is running.");
                }

                this.outcome = outcome;
                executionStatus = StatusFinishing;
                LifecyclePhase = LifecyclePhases.Finishing;

                cachedFinishingHandlers = finishingHandlers;
                finishingHandlers = null;
            }

            // Note: We no longer need to hold the lock because none of the state used from here on can change
            //       since the status is now StatusFinishing.
            try
            {
                if (parent != null)
                    parent.Finishing -= HandleParentFinishedBeforeThisContext;

                using (Enter())
                    EventHandlerPolicy.SafeInvoke(cachedFinishingHandlers, this, EventArgs.Empty);

                if (isDisposing)
                    logWriter.Failures.Write("The test step was orphaned by the test runner!\n");

                logWriter.Close();

                var result = new TestResult();
                result.AssertCount = assertCount;
                result.Duration = actualDuration.GetValueOrDefault(stopwatch.Elapsed).TotalSeconds;
                result.Outcome = outcome;

                MessageSink.Publish(new TestStepFinishedMessage()
                {
                    StepId = testStep.Id,
                    Result = result
                });

                if (contextCookie != null)
                {
                    if (!isDisposing)
                        contextCookie.Dispose();
                    contextCookie = null;
                }
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An unhandled exception occurred while finishing a test step.", ex);
            }
        }

        private bool IsRunning
        {
            get { return executionStatus == StatusStarted || executionStatus == StatusFinishing; }
        }

        private ITestContextTracker ContextTracker
        {
            get { return manager.ContextTracker; }
        }

        private IDisposable Enter()
        {
            return ContextTracker.EnterContext(this);
        }

        private void HandleParentFinishedBeforeThisContext(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}