// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Diagnostics;
using Gallio.Contexts;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Logging;
using Gallio.Reflection;
using Gallio.Model.Serialization;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A test plan based on a topological sort of the tests according to
    /// their dependencies.
    /// </summary>
    /// FIXME: Missing topological sort
    /// FIXME: Should override Passed / Inconclusive outcome with Failed / Inconclusive if
    ///        a child test step passes/fails.
    /// FIXME: Need high-level cancelation support to tolerate buggy test controllers
    ///        that have somehow managed to get hung up.
    public class DependencyTestPlan : ITestPlan
    {
        private readonly object syncRoot = new object();

        private readonly IContextManager contextManager;
        private readonly ITestListener listener;

        private bool isRunning;
        private ContextualTestMonitor rootTestMonitor;
        private InternalContextHandler handler;

        /// <summary>
        /// Creates an empty dependency test plan.
        /// </summary>
        /// <param name="contextManager">The context manager</param>
        /// <param name="listener">The listener</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextManager"/>
        /// or <paramref name="listener"/> is null</exception>
        public DependencyTestPlan(IContextManager contextManager, ITestListener listener)
        {
            if (contextManager == null)
                throw new ArgumentNullException(@"contextManager");
            if (listener == null)
                throw new ArgumentNullException(@"listener");

            this.contextManager = contextManager;
            this.listener = listener;
        }

        /// <inheritdoc />
        public bool ScheduleTests(IProgressMonitor progressMonitor, TestModel testModel, TestExecutionOptions options)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (testModel == null)
                throw new ArgumentNullException(@"testModel");
            if (options == null)
                throw new ArgumentNullException(@"options");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Preparing the test plan.", 1);

                InternalContextHandler newHandler;
                lock (syncRoot)
                {
                    if (handler != null)
                        throw new InvalidOperationException("The currently scheduled tests must be run or cleaned up before a new batch of tests can be scheduled.");

                    newHandler = new InternalContextHandler(listener, contextManager);

                    Dictionary<ITest, ContextualTestMonitor> lookupTable = new Dictionary<ITest, ContextualTestMonitor>();
                    rootTestMonitor = ScheduleFilteredClosure(newHandler, lookupTable, testModel.RootTest, options);

                    if (rootTestMonitor != null)
                    {
                        handler = newHandler;
                        return true;
                    }
                }

                newHandler.Dispose();
                return false;
            }
        }

        /// <inheritdoc />
        public void RunTests(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            // Note we just pass the progress monitor straight into the RootTestController
            // after setting up the environent appropriately without calling BeginTask first.
            using (progressMonitor)
            {
                InternalContextHandler cachedHandler = null;
                try
                {
                    // Synchronize in preparation for the run.
                    ITestMonitor cachedRootTestMonitor;
                    lock (syncRoot)
                    {
                        if (isRunning)
                            throw new InvalidOperationException("The test plan is already running or has not been cleaned up yet.");
                        if (rootTestMonitor == null)
                            return; // no work to do!

                        isRunning = true;
                        cachedRootTestMonitor = rootTestMonitor;
                        cachedHandler = handler;
                    }

                    // We do not run the tests themselves within the lock.
                    // Test execution could be interrupted non-deterministically.
                    // The idea is that CleanUpTests can barge in and cause cleanup
                    // to begin from a different thread even while the RunTests method
                    // is in progress to invalidate all monitors and help during a non-deterministic
                    // shutdown due to a timeout or cancelation.
                    RecursivelyRunAllTestsWithinANullContext(progressMonitor, cachedRootTestMonitor);
                }
                finally
                {
                    DisposeHandler(cachedHandler);
                }
            }
        }

        /// <inheritdoc />
        public void CleanUpTests(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Cleaning up the tests.", 10);

                DisposeHandler(handler);
            }
        }

        private void DisposeHandler(InternalContextHandler cachedHandler)
        {
            lock (syncRoot)
            {
                // Clean up after running.
                if (cachedHandler != null && cachedHandler == handler)
                {
                    handler = null;
                    rootTestMonitor = null;
                    isRunning = false;
                }
            }

            if (cachedHandler != null)
                cachedHandler.Dispose();
        }

        private ContextualTestMonitor ScheduleFilteredClosure(InternalContextHandler handler, IDictionary<ITest, ContextualTestMonitor> lookupTable, ITest test, TestExecutionOptions options)
        {
            if (options.Filter.IsMatch(test))
                return ScheduleSubtree(handler, lookupTable, test, true);

            List<ContextualTestMonitor> children = new List<ContextualTestMonitor>(test.Children.Count);

            foreach (ITest child in test.Children)
            {
                ContextualTestMonitor childMonitor = ScheduleFilteredClosure(handler, lookupTable, child, options);
                if (childMonitor != null)
                    children.Add(childMonitor);
            }

            if (children.Count != 0)
                return ScheduleTest(handler, lookupTable, test, children, false);

            return null;
        }

        private ContextualTestMonitor ScheduleSubtree(InternalContextHandler handler, IDictionary<ITest, ContextualTestMonitor> lookupTable, ITest test, bool isExplicit)
        {
            List<ContextualTestMonitor> children = new List<ContextualTestMonitor>(test.Children.Count);

            foreach (ITest child in test.Children)
                children.Add(ScheduleSubtree(handler, lookupTable, child, false));

            return ScheduleTest(handler, lookupTable, test, children, isExplicit);
        }

        private ContextualTestMonitor ScheduleTest(InternalContextHandler handler, IDictionary<ITest, ContextualTestMonitor> lookupTable, ITest test, IEnumerable<ContextualTestMonitor> children, bool isExplicit)
        {
            ContextualTestMonitor testMonitor = new ContextualTestMonitor(handler, test, isExplicit);
            foreach (ContextualTestMonitor child in children)
                testMonitor.AddChild(child);

            lookupTable.Add(test, testMonitor);
            return testMonitor;
        }

        private static void RecursivelyRunAllTestsWithinANullContext(IProgressMonitor progressMonitor, ITestMonitor cachedRootTestMonitor)
        {
            using (Context.EnterContext(null))
            {
                Factory<ITestController> rootTestControllerFactory = cachedRootTestMonitor.Test.TestControllerFactory;

                if (rootTestControllerFactory != null)
                {
                    using (ITestController controller = rootTestControllerFactory())
                        controller.RunTests(progressMonitor, cachedRootTestMonitor);
                }
            }
        }


        private sealed class InternalContextHandler : IContextHandler, IDisposable
        {
            private readonly ITestListener listener;
            private readonly IContextManager contextManager;
            private readonly List<InternalContext> unfinishedContexts;

            private bool disposed;

            public InternalContextHandler(ITestListener listener, IContextManager contextManager)
            {
                this.listener = listener;
                this.contextManager = contextManager;

                unfinishedContexts = new List<InternalContext>();
            }

            public ITestListener Listener
            {
                get { return listener; }
            }

            public Context CreateContext(ITestStep step)
            {
                return new InternalContext(contextManager.CurrentContext, step, this);
            }

            public void StartStep(Context context)
            {
                InternalContext internalContext = (InternalContext) context;

                // Track step start in critical section.
                lock (this)
                {
                    ThrowIfDisposed();

                    if (internalContext.Parent == null)
                        contextManager.GlobalContext = internalContext;

                    unfinishedContexts.Add(internalContext);
                }

                internalContext.StartStep();
            }

            public void FinishStep(Context context, TestStatus status, TestOutcome outcome, TimeSpan? actualDuration)
            {
                InternalContext internalContext = (InternalContext) context;

                // Track step finish in critical section.
                lock (this)
                {
                    ThrowIfDisposed();

                    unfinishedContexts.Remove(internalContext);
                }

                internalContext.FinishStep(status, outcome, actualDuration);
            }

            public void AddMetadata(Context context, string key, string value)
            {
                InternalContext internalContext = (InternalContext) context;
                internalContext.AddMetadata(key, value);
            }

            public void SetLifecyclePhase(Context context, string phase)
            {
                InternalContext internalContext = (InternalContext) context;
                internalContext.LifecyclePhase = phase;
            }

            public void SetInterimOutcome(Context context, TestOutcome outcome)
            {
                InternalContext internalContext = (InternalContext) context;
                internalContext.SetInterimOutcome(outcome);
            }

            public void Dispose()
            {
                lock (this)
                {
                    if (disposed)
                        return;

                    disposed = true;
                }

                try
                {
                    foreach (InternalContext context in unfinishedContexts)
                        context.CleanUpOrphan();
                    unfinishedContexts.Clear();
                }
                finally
                {
                    contextManager.GlobalContext = null;
                }
            }

            private void ThrowIfDisposed()
            {
                if (disposed)
                    throw new ObjectDisposedException("The context handler has been disposed.");
            }
        }

        private sealed class InternalContext : Context
        {
            private readonly ITestStep testStep;
            private readonly InternalContextHandler handler;

            private ContextCookie cookie;
            private bool wasStarted;
            private bool wasFinished;

            private Stopwatch stopwatch;

            public InternalContext(Context parent, ITestStep testStep, InternalContextHandler handler)
                : base(parent, new TestStepInfo(testStep), new TestListenerLogWriter(handler.Listener, testStep.Id))
            {
                this.testStep = testStep;
                this.handler = handler;
            }

            public ITestListener Listener
            {
                get { return handler.Listener; }
            }

            public void StartStep()
            {
                lock (this)
                {
                    if (wasStarted)
                        throw new InvalidOperationException("Step has already been started.");

                    stopwatch = Stopwatch.StartNew();
                    cookie = Enter();

                    // Dispatch the start notification.
                    if (testStep.Parent == null)
                        Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateNewInstanceEvent(new TestInstanceData(testStep.TestInstance)));

                    Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateStartEvent(new TestStepData(testStep)));

                    wasStarted = true;
                }
            }

            public void FinishStep(TestStatus status, TestOutcome outcome, TimeSpan? actualDuration)
            {
                Dispose();

                lock (this)
                {
                    if (! wasStarted || wasFinished)
                        return; // ignore spurious finish

                    cookie.ExitContext();
                    cookie = null;

                    wasFinished = true;

                    DispatchFinishEvent(status, outcome, actualDuration);
                }
            }

            protected override void SetLifecyclePhaseImpl(string lifecyclePhase)
            {
                lock (this)
                {
                    ThrowIfNotRunning();

                    base.SetLifecyclePhaseImpl(lifecyclePhase);

                    Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateSetPhaseEvent(testStep.Id, lifecyclePhase));
                }
            }

            protected override void AddMetadataImpl(string metadataKey, string metadataValue)
            {
                lock (this)
                {
                    ThrowIfNotRunning();

                    testStep.Metadata.CopyOnWriteAdd(metadataKey, metadataValue);

                    Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateAddMetadataEvent(testStep.Id, metadataKey, metadataValue));
                }
            }

            public void SetInterimOutcome(TestOutcome outcome)
            {
                lock (this)
                {
                    ThrowIfNotRunning();

                    SetOutcomeImpl(outcome);
                }
            }

            public void CleanUpOrphan()
            {
                lock (this)
                {
                    if (wasStarted && !wasFinished)
                    {
                        cookie = null;
                        wasFinished = true;

                        LogWriter[LogStreamNames.Failures].WriteLine("The test step was orphaned by the test runner!");
                        DispatchFinishEvent(TestStatus.Error, TestOutcome.Failed, null);
                    }
                }
            }

            protected override Context RunStepImpl(string name, ICodeElementInfo codeElement, Block block)
            {
                ContextualTestStepMonitor stepMonitor = new ContextualTestStepMonitor(handler,
                    new BaseTestStep(TestInstance, name, codeElement, TestStep));

                stepMonitor.Start();
                try
                {
                    stepMonitor.LifecyclePhase = LifecyclePhases.Execute;

                    block();

                    stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Passed, null);

                    return stepMonitor.Context;
                }
                catch (Exception ex)
                {
                    // TODO: Other exception types might signal ignored or inconclusive states.
                    stepMonitor.LogWriter[LogStreamNames.Failures].Write(ex);

                    stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Failed, null);

                    // Allow the exception to bubble up out of the block.
                    throw;
                }
            }

            private void DispatchFinishEvent(TestStatus status, TestOutcome outcome, TimeSpan? actualDuration)
            {
                TestResult result = new TestResult();
                result.AssertCount = AssertCount;
                result.Duration = actualDuration.GetValueOrDefault(stopwatch.Elapsed).TotalSeconds;
                result.Status = status;
                result.Outcome = outcome;

                Listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateFinishEvent(testStep.Id, result));
            }

            private void ThrowIfNotRunning()
            {
                if (! wasStarted ||wasFinished)
                    throw new InvalidOperationException("The test step is not running.");
            }
        }
    }
}
