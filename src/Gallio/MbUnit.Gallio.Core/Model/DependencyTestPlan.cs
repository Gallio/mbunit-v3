// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Threading;
using MbUnit.Core.Model.Events;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Core.RuntimeSupport;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Model
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
        private readonly ICoreContextManager contextManager;
        private readonly ITestListener listener;
        private readonly ReaderWriterLock inUse;

        private TestMonitor rootTestMonitor;
        private bool isRunning;

        private int currentGeneration;
        private readonly List<StepMonitor> activeStepMonitors;

        /// <summary>
        /// Creates an empty dependency test plan.
        /// </summary>
        /// <param name="contextManager">The context manager</param>
        /// <param name="listener">The listener</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextManager"/>
        /// or <paramref name="listener"/> is null</exception>
        public DependencyTestPlan(ICoreContextManager contextManager, ITestListener listener)
        {
            if (contextManager == null)
                throw new ArgumentNullException(@"contextManager");
            if (listener == null)
                throw new ArgumentNullException(@"listener");

            this.contextManager = contextManager;
            this.listener = listener;
            this.inUse = new ReaderWriterLock();

            activeStepMonitors = new List<StepMonitor>();
        }

        /// <inheritdoc />
        public bool ScheduleTests(IProgressMonitor progressMonitor, ITest rootTest, TestExecutionOptions options)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (rootTest == null)
                throw new ArgumentNullException(@"rootTest");
            if (rootTest.Parent != null)
                throw new ArgumentException("The test must be the root of a test tree.", @"rootTest");
            if (options == null)
                throw new ArgumentNullException(@"options");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Preparing the test plan.", 1);

                try
                {
                    inUse.AcquireWriterLock(Timeout.Infinite);

                    if (rootTestMonitor != null)
                        throw new InvalidOperationException("The currently scheduled tests must be run or cleaned up before a new batch of tests can be scheduled.");

                    Dictionary<ITest, TestMonitor> lookupTable = new Dictionary<ITest, TestMonitor>();
                    rootTestMonitor = ScheduleFilteredClosure(lookupTable, rootTest, options);
                    return rootTestMonitor != null;
                }
                finally
                {
                    if (inUse.IsWriterLockHeld)
                        inUse.ReleaseWriterLock();
                }
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
                try
                {
                    // Acquire the writer lock and start running.
                    inUse.AcquireWriterLock(Timeout.Infinite);

                    if (isRunning)
                        throw new InvalidOperationException("The test plan is already running or has not been cleaned up yet.");
                    if (rootTestMonitor == null)
                        return; // no work to do!

                    isRunning = true;

                    // We do not run the tests themselves within a reader or writer
                    // lock.  The idea is that CleanUpTests can barge in and cause cleanup
                    // to begin from a different thread even while the RunTests method
                    // is in progress to invalidate all monitors and help during a non-deterministic
                    // shutdown due to a timeout or cancelation.
                    ITestMonitor cachedRootTestMonitor = rootTestMonitor;

                    // Release the lock!
                    inUse.ReleaseWriterLock();

                    // Now run the tests.
                    // Test execution could be interrupted non-deterministically.
                    using (RootTestController controller = new RootTestController())
                        controller.RunTests(progressMonitor, cachedRootTestMonitor);
                }
                finally
                {
                    try
                    {
                        if (!inUse.IsWriterLockHeld)
                            inUse.AcquireWriterLock(Timeout.Infinite);

                        // Set the state to not running and increment the generation counter
                        // to invalidate all existing monitors.
                        isRunning = false;
                        currentGeneration += 1;
                    }
                    finally
                    {
                        if (inUse.IsWriterLockHeld)
                            inUse.ReleaseWriterLock();
                    }
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

                try
                {
                    // Wait for all readers to get out before we clean up.
                    progressMonitor.SetStatus("Invalidating test and step monitors.");
                    inUse.AcquireReaderLock(Timeout.Infinite);
                    LockCookie lockCookie = inUse.UpgradeToWriterLock(Timeout.Infinite);
                    currentGeneration += 1;
                    progressMonitor.Worked(1);

                    // Downgrade to reader lock then dispose of the root context.
                    // This is to guard against possible re-entrance resulting from
                    // Dispose events defined on the contexts.
                    inUse.DowngradeFromWriterLock(ref lockCookie);
                    progressMonitor.SetStatus("Disposing root context.");
                    contextManager.DisposeRootContext();
                    progressMonitor.Worked(1);

                    // Now upgrade back to a writer lock and clean up everything else.
                    inUse.UpgradeToWriterLock(Timeout.Infinite);

                    // While holding the lock, cleanup everything else
                    int totalWorkUnits = activeStepMonitors.Count + (rootTestMonitor != null ? rootTestMonitor.TestCount : 0);
                    if (totalWorkUnits != 0)
                    {
                        double perItemWork = 8.0 / totalWorkUnits;

                        // Clean up all remaining tests.
                        if (rootTestMonitor != null)
                        {
                            progressMonitor.SetStatus("Cleaning up test monitors.");

                            foreach (TestMonitor testMonitor in rootTestMonitor.PostOrderTraversal)
                            {
                                testMonitor.CleanUpWithWriterLock();
                                progressMonitor.Worked(perItemWork);
                            }
                        }

                        // Clean up all remaining steps.
                        if (activeStepMonitors.Count != 0)
                        {
                            progressMonitor.SetStatus("Cleaning up step monitors.");

                            foreach (StepMonitor stepMonitor in activeStepMonitors)
                            {
                                stepMonitor.CleanUpWithWriterLock();
                                progressMonitor.Worked(perItemWork);
                            }

                            activeStepMonitors.Clear();
                        }

                        rootTestMonitor = null;
                    }
                }
                finally
                {
                    if (inUse.IsWriterLockHeld)
                        inUse.ReleaseWriterLock();
                    if (inUse.IsReaderLockHeld)
                        inUse.ReleaseReaderLock();
                }
            }
        }

        private TestMonitor ScheduleFilteredClosure(IDictionary<ITest, TestMonitor> lookupTable, ITest test, TestExecutionOptions options)
        {
            if (options.Filter.IsMatch(test))
                return ScheduleSubtree(lookupTable, test, true);

            List<TestMonitor> children = new List<TestMonitor>(test.Children.Count);

            foreach (ITest child in test.Children)
            {
                TestMonitor childMonitor = ScheduleFilteredClosure(lookupTable, child, options);
                if (childMonitor != null)
                    children.Add(childMonitor);
            }

            if (children.Count != 0)
                return ScheduleTest(lookupTable, test, children.ToArray(), false);

            return null;
        }

        private TestMonitor ScheduleSubtree(IDictionary<ITest, TestMonitor> lookupTable, ITest test, bool isExplicit)
        {
            List<TestMonitor> children = new List<TestMonitor>(test.Children.Count);

            foreach (ITest child in test.Children)
                children.Add(ScheduleSubtree(lookupTable, child, false));

            return ScheduleTest(lookupTable, test, children.ToArray(), isExplicit);
        }

        private TestMonitor ScheduleTest(IDictionary<ITest, TestMonitor> lookupTable, ITest test, TestMonitor[] children, bool isExplicit)
        {
            TestMonitor testMonitor = new TestMonitor(this, test, children, isExplicit);
            lookupTable.Add(test, testMonitor);

            return testMonitor;
        }

        private void RegisterStepMonitorWithReaderLock(StepMonitor stepMonitor)
        {
            lock (activeStepMonitors)
                activeStepMonitors.Add(stepMonitor);
        }

        private void UnregisterStepMonitorWithReaderLock(StepMonitor stepMonitor)
        {
            lock (activeStepMonitors)
                activeStepMonitors.Remove(stepMonitor);
        }

        private void VerifyGenerationWithReaderLock(int generation)
        {
            if (currentGeneration != generation)
                throw new InvalidOperationException("The monitor is invalid.");
        }

        private sealed class TestMonitor : ITestMonitor
        {
            private readonly int generation;
            private readonly DependencyTestPlan testPlan;
            private readonly ITest test;
            private readonly bool isExplicit;
            private readonly TestMonitor[] children;
            private readonly int testCount;

            private bool hasStarted;

            public TestMonitor(DependencyTestPlan testPlan, ITest test, TestMonitor[] children, bool isExplicit)
            {
                this.generation = testPlan.currentGeneration;
                this.testPlan = testPlan;
                this.test = test;
                this.children = children;
                this.isExplicit = isExplicit;

                testCount = 1;
                foreach (TestMonitor child in children)
                    testCount += child.testCount;
            }

            public bool IsExplicit
            {
                get { return isExplicit; }
            }

            public bool IsPending
            {
                get { return ! hasStarted; }
            }

            public ITest Test
            {
                get { return test; }
            }

            public int TestCount
            {
                get { return testCount; }
            }

            public IEnumerable<ITestMonitor> Children
            {
                get { return children; }
            }

            public IEnumerable<ITestMonitor> PreOrderTraversal
            {
                get
                {
                    yield return this;

                    foreach (TestMonitor child in children)
                        foreach (TestMonitor descendant in child.PreOrderTraversal)
                            yield return descendant;
                }
            }

            public IEnumerable<ITestMonitor> PostOrderTraversal
            {
                get
                {
                    foreach (TestMonitor child in children)
                        foreach (TestMonitor descendant in child.PostOrderTraversal)
                            yield return descendant;

                    yield return this;
                }
            }

            public IList<ITestMonitor> GetAllMonitors()
            {
                ITestMonitor[] testMonitors = new ITestMonitor[testCount];

                int i = 0;
                foreach (ITestMonitor monitor in PreOrderTraversal)
                    testMonitors[i++] = monitor;

                return testMonitors;
            }

            public IStepMonitor StartRootStep()
            {
                try
                {
                    // Create a monitor inside a reader lock.
                    testPlan.inUse.AcquireReaderLock(Timeout.Infinite);

                    VerifyNotStartedWithReaderLock();
                    testPlan.VerifyGenerationWithReaderLock(generation);

                    hasStarted = true;

                    StepMonitor stepMonitor = new StepMonitor(testPlan, CreateRootStep(), null);
                    testPlan.inUse.ReleaseReaderLock();

                    // Start the monitor outside the lock.
                    stepMonitor.EnterContext();
                    return stepMonitor;
                }
                finally
                {
                    if (testPlan.inUse.IsReaderLockHeld)
                        testPlan.inUse.ReleaseReaderLock();
                }
            }

            public void CleanUpWithWriterLock()
            {
                if (hasStarted)
                    return;

                hasStarted = true;

                StepData stepData = new StepData(CreateRootStep());
                testPlan.listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateStartEvent(stepData));

                TestResult result = new TestResult();
                result.Status = TestStatus.NotRun;
                result.Outcome = TestOutcome.Inconclusive;
                testPlan.listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateFinishEvent(stepData.Id, result));
            }

            private void VerifyNotStartedWithReaderLock()
            {
                if (hasStarted)
                    throw new InvalidOperationException("The test has already started execution.");
            }

            private BaseStep CreateRootStep()
            {
                return new BaseStep(BaseStep.RootStepName, test.CodeReference, test, null);
            }
        }

        private sealed class StepMonitor : IStepMonitor
        {
            private readonly int generation;
            private readonly DependencyTestPlan testPlan;
            private readonly BaseStep step;
            private readonly LogWriter logWriter;
            private readonly Stopwatch stopwatch;
            private readonly Context context;

            private ContextCookie contextCookie;
            private string lifecyclePhase = @"";

            public StepMonitor(DependencyTestPlan testPlan, BaseStep step, StepMonitor parentStepMonitor)
            {
                this.generation = testPlan.currentGeneration;
                this.testPlan = testPlan;
                this.step = step;

                ITestListener listener = testPlan.listener;
                logWriter = new TestListenerLogWriter(listener, step.Id);
                stopwatch = Stopwatch.StartNew();

                Context parentContext;
                if (parentStepMonitor == null)
                    parentContext = testPlan.contextManager.CurrentContext;
                else
                    parentContext = parentStepMonitor.context;

                if (parentContext == null)
                    context = testPlan.contextManager.InitializeRootContext(this);
                else
                    context = testPlan.contextManager.CreateChildContext(parentContext, this);

                DispatchStartEvent();

                testPlan.RegisterStepMonitorWithReaderLock(this);
            }

            public IStep Step
            {
                get { return step; }
            }

            public LogWriter LogWriter
            {
                get { return logWriter; }
            }

            public Context Context
            {
                get { return context; }
            }

            public string LifecyclePhase
            {
                get { return lifecyclePhase; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException(@"value");

                    try
                    {
                        testPlan.inUse.AcquireReaderLock(Timeout.Infinite);

                        testPlan.VerifyGenerationWithReaderLock(generation);

                        lock (this)
                        {
                            VerifyNotFinishedWithLocalLock();

                            if (lifecyclePhase != value)
                            {
                                lifecyclePhase = value;
                                testPlan.listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateSetPhaseEvent(step.Id, lifecyclePhase));
                            }
                        }
                    }
                    finally
                    {
                        if (testPlan.inUse.IsReaderLockHeld)
                            testPlan.inUse.ReleaseReaderLock();
                    }
                }
            }

            public Context RunStep(string name, Block block, CodeReference codeReference)
            {
                if (block == null)
                    throw new ArgumentNullException(@"block");

                StepMonitor childStepMonitor = StartChildStepImpl(name, codeReference);
                childStepMonitor.RunBlock(block);
                return childStepMonitor.context;
            }

            public void AddMetadata(string metadataKey, string metadataValue)
            {
                if (metadataKey == null)
                    throw new ArgumentNullException(@"metadataKey");
                if (metadataValue == null)
                    throw new ArgumentNullException(@"metadataValue");

                try
                {
                    testPlan.inUse.AcquireReaderLock(Timeout.Infinite);

                    testPlan.VerifyGenerationWithReaderLock(generation);

                    lock (this)
                    {
                        VerifyNotFinishedWithLocalLock();

                        testPlan.listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateAddMetadataEvent(step.Id, metadataKey, metadataValue));
                    }

                    step.Metadata.CopyOnWriteAdd(metadataKey, metadataValue);
                }
                finally
                {
                    if (testPlan.inUse.IsReaderLockHeld)
                        testPlan.inUse.ReleaseReaderLock();
                }
            }

            public IStepMonitor StartChildStep(string name, CodeReference codeReference)
            {
                return StartChildStepImpl(name, codeReference);
            }

            private StepMonitor StartChildStepImpl(string name, CodeReference codeReference)
            {
                if (name == null)
                    throw new ArgumentNullException(@"name");
                if (name.Length == 0)
                    throw new ArgumentException("Name must not be empty.", @"name");
                if (codeReference == null)
                    throw new ArgumentNullException(@"codeReference");

                try
                {
                    // Create a monitor inside a reader lock.
                    testPlan.inUse.AcquireReaderLock(Timeout.Infinite);

                    testPlan.VerifyGenerationWithReaderLock(generation);

                    StepMonitor stepMonitor;
                    lock (this)
                    {
                        VerifyNotFinishedWithLocalLock();

                        BaseStep childStep = new BaseStep(name, codeReference, step.Test, step);
                        stepMonitor = new StepMonitor(testPlan, childStep, this);
                    }

                    testPlan.inUse.ReleaseReaderLock();

                    // Start the monitor outside the lock.
                    stepMonitor.EnterContext();
                    return stepMonitor;
                }
                finally
                {
                    if (testPlan.inUse.IsReaderLockHeld)
                        testPlan.inUse.ReleaseReaderLock();
                }
            }

            public void FinishStep(TestStatus status, TestOutcome outcome, TimeSpan? actualDuration)
            {
                // Dispose of the context and send the event notification inside
                // the reader lock.  The reason we do this is to allow the CleanUpTests
                // routine to barge in and cause this code to be skipped by incrementing
                // the generation number.
                try
                {
                    testPlan.inUse.AcquireReaderLock(Timeout.Infinite);

                    testPlan.VerifyGenerationWithReaderLock(generation);

                    ICoreContextManager contextManager = testPlan.contextManager;
                    if (context.Parent == null)
                        contextManager.DisposeRootContext();
                    else
                        contextManager.DisposeContext(context);

                    // Exit the context.
                    lock (this)
                    {
                        VerifyNotFinishedWithLocalLock();

                        contextCookie.ExitContext();
                        contextCookie = null;
                    }

                    // Send the final notification.
                    DispatchFinishEvent(status, outcome, actualDuration);
                    
                    // Now that we're done, unregister this step monitor.
                    testPlan.UnregisterStepMonitorWithReaderLock(this);
                }
                finally
                {
                    if (testPlan.inUse.IsReaderLockHeld)
                        testPlan.inUse.ReleaseReaderLock();
                }
            }

            public void EnterContext()
            {
                lock (this)
                {
                    contextCookie = context.Enter();
                }
            }

            public void CleanUpWithWriterLock()
            {
                lock (this)
                {
                    if (contextCookie != null)
                    {
                        contextCookie = null;

                        logWriter[LogStreamNames.Failures].WriteLine("The test step was orphaned by the test runner!");
                        DispatchFinishEvent(TestStatus.Error, TestOutcome.Failed, null);
                    }
                }
            }

            private void DispatchStartEvent()
            {
                testPlan.listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateStartEvent(new StepData(step)));
            }

            private void DispatchFinishEvent(TestStatus status, TestOutcome outcome, TimeSpan? actualDuration)
            {
                TestResult result = new TestResult();
                result.AssertCount = context.AssertCount;
                result.Duration = actualDuration.GetValueOrDefault(stopwatch.Elapsed).TotalSeconds;
                result.Status = status;
                result.Outcome = outcome;

                testPlan.listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateFinishEvent(step.Id, result));
            }

            private void RunBlock(Block block)
            {
                try
                {
                    LifecyclePhase = LifecyclePhases.Execute;

                    block();

                    FinishStep(TestStatus.Executed, TestOutcome.Passed, null);
                }
                catch (Exception ex)
                {
                    // TODO: Other exception types might signal ignored or inconclusive states.
                    logWriter[LogStreamNames.Failures].Write(ex);

                    FinishStep(TestStatus.Executed, TestOutcome.Failed, null);

                    // Allow the exception to bubble up out of the block.
                    throw;
                }
            }

            private void VerifyNotFinishedWithLocalLock()
            {
                if (contextCookie == null)
                    throw new InvalidOperationException("The step has already finished.");
            }
        }
    }
}
