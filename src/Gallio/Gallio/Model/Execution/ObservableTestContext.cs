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
using System.Diagnostics;
using System.Threading;
using Gallio.Collections;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Utilities;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// An observable test context monitors translates state changes on the
    /// test context into events on a <see cref="ITestListener" />.
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
        private readonly ITestStep testStep;
        private readonly ObservableTestLogWriter logWriter;
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
        /// <param name="manager">The test context manager</param>
        /// <param name="testStep">The test step</param>
        /// <param name="parent">The parent test context</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="manager"/> or <paramref name="testStep"/> is null</exception>
        public ObservableTestContext(ObservableTestContextManager manager, ITestStep testStep, ITestContext parent)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");
            if (testStep == null)
                throw new ArgumentNullException("testStep");

            this.manager = manager;
            this.testStep = testStep;
            this.parent = parent;

            logWriter = new ObservableTestLogWriter(Listener, testStep.Id);
        }

        /// <inheritdoc />
        public ITestContext Parent
        {
            get { return parent; }
        }

        /// <inheritdoc />
        public ITestStep TestStep
        {
            get { return testStep; }
        }

        /// <inheritdoc />
        public ITestLogWriter LogWriter
        {
            get { return logWriter; }
        }

        /// <inheritdoc />
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

                    Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateSetPhaseEvent(testStep.Id, lifecyclePhase));
                    lifecyclePhase = value;
                }
            }
        }

        /// <inheritdoc />
        public TestOutcome Outcome
        {
            get { return outcome; }
        }

        /// <inheritdoc />
        public UserDataCollection Data
        {
            get
            {
                if (data == null)
                    Interlocked.CompareExchange(ref data, new UserDataCollection(), null);
                return data;
            }
        }

        /// <inheritdoc />
        public int AssertCount
        {
            get { return assertCount; }
        }

        /// <inheritdoc />
        public bool IsFinished
        {
            get { return executionStatus == StatusFinished; }
        }

        /// <inheritdoc />
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

                EventHandlerUtils.SafeInvoke(value, this, EventArgs.Empty);
            }
            remove
            {
                lock (syncRoot)
                    finishingHandlers -= value;
            }
        }

        /// <inheritdoc />
        public void AddAssertCount(int value)
        {
            Interlocked.Add(ref assertCount, value);
        }

        /// <inheritdoc />
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

                testStep.Metadata.CopyOnWriteAdd(metadataKey, metadataValue);
                Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateAddMetadataEvent(testStep.Id, metadataKey, metadataValue));
            }
        }

        /// <inheritdoc />
        public void SetInterimOutcome(TestOutcome outcome)
        {
            lock (syncRoot)
            {
                if (! IsRunning)
                    throw new InvalidOperationException("Cannot set the interim outcome unless the test step is running.");

                this.outcome = outcome;
            }
        }

        /// <inheritdoc />
        public ITestContext StartChildStep(ITestStep childStep)
        {
            if (childStep == null)
                throw new ArgumentNullException(@"childStep");
            if (childStep.Parent != testStep || childStep.TestInstance != testStep.TestInstance)
                throw new ArgumentException("Expected a child of this step.", "childStep");

            lock (syncRoot)
            {
                if (!IsRunning)
                    throw new InvalidOperationException("Cannot start a new child step unless the test step is running.");
            }

            return manager.StartStep(childStep);
        }

        /// <inheritdoc />
        public ITestContext StartChildStep(string name, ICodeElementInfo codeElement)
        {
            return StartChildStep(new BaseTestStep(testStep.TestInstance, name, codeElement, testStep));
        }

        /// <inheritdoc />
        public void FinishStep(TestOutcome outcome, TimeSpan? actualDuration)
        {
            FinishStep(outcome, actualDuration, false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            FinishStep(TestOutcome.Error, null, true);
        }

        /// <summary>
        /// Initializes and starts the step.
        /// </summary>
        internal void InitializeAndStartStep()
        {
            lock (syncRoot)
            {
                if (executionStatus != StatusCreated)
                    throw new InvalidOperationException("Cannot initialize and start the test step twice.");

                stopwatch = Stopwatch.StartNew();

                // Dispatch the start notification.
                if (testStep.Parent == null)
                    Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateNewInstanceEvent(new TestInstanceData(testStep.TestInstance)));
                Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateStartEvent(new TestStepData(testStep)));

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
                    EventHandlerUtils.SafeInvoke(cachedFinishingHandlers, this, EventArgs.Empty);

                if (isDisposing)
                    logWriter.Write(LogStreamNames.Failures, "The test step was orphaned by the test runner!\n");

                logWriter.Close();

                TestResult result = new TestResult();
                result.AssertCount = assertCount;
                result.Duration = actualDuration.GetValueOrDefault(stopwatch.Elapsed).TotalSeconds;
                result.Outcome = outcome;

                Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateFinishEvent(testStep.Id, result));

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

        private ITestListener Listener
        {
            get { return manager.Listener; }
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