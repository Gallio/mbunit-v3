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
using System.Runtime.CompilerServices;
using System.Threading;
using Gallio;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Messaging;
using Gallio.Common.Policies;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using Gallio.Model.Contexts;
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;

namespace Gallio.Framework
{
    /// <summary>
    /// The test context provides information about the environment in which
    /// a test is executing.  A new context is created each time a test or
    /// test step begins execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Test contexts are arranged in a hierarchy that corresponds to the order in which
    /// the contexts were entered.  Thus the context for a test likely has as
    /// its parent the context for its containing test fixture.
    /// </para>
    /// <para>
    /// Arbitrary user data can be associated with a test context.  Furthermore, client
    /// code may attach <see cref="Finishing" /> event handlers to perform resource
    /// reclamation just prior to marking the test step as finished.
    /// </para>
    /// </remarks>
    /// <seealso cref="Framework.TestStep"/>
    [SystemInternal]
    public sealed class TestContext
    {
        private static readonly Key<TestContext> GallioFrameworkContextKey = new Key<TestContext>("Gallio.Framework.Context");
        private readonly ITestContext inner;
        private readonly Sandbox sandbox;
        private EventHandler finishingHandlers;

        /// <summary>
        /// Creates a wrapper for a <see cref="ITestContext" />.
        /// </summary>
        /// <param name="inner">The context to wrap.</param>
        /// <param name="sandbox">The sandbox to use, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null.</exception>
        private TestContext(ITestContext inner, Sandbox sandbox)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
            this.sandbox = sandbox;
        }

        private static ITestContextTracker ContextTracker
        {
            get { return TestContextTrackerAccessor.Instance; }
        }

        /// <summary>
        /// Gets the context of the current thread, or null if there is no
        /// current context.
        /// </summary>
        public static TestContext CurrentContext
        {
            get { return WrapContext(ContextTracker.CurrentContext); }
        }

        /// <summary>
        /// Gets the global context of the environment, or null if there is no
        /// such context.
        /// </summary>
        public static TestContext GlobalContext
        {
            get { return WrapContext(ContextTracker.GlobalContext); }
        }

        /// <summary>
        /// Sets the default context for the specified thread.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default context for a thread is <see cref="GlobalContext" /> unless the thread's
        /// default context has been overridden with <see cref="SetThreadDefaultContext" />.
        /// </para>
        /// <para>
        /// Changing the default context of a thread is useful for capturing existing threads created
        /// outside of a test into a particular context.  Among other things, this ensures that side-effects
        /// of the thread, such as writing text to the console, are recorded as part of the step
        /// represented by the specified context.
        /// </para>
        /// </remarks>
        /// <param name="thread">The thread.</param>
        /// <param name="context">The context to associate with the thread, or null to reset the
        /// thread's default context to inherit the <see cref="GlobalContext" /> once again.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thread"/> is null.</exception>
        public static void SetThreadDefaultContext(Thread thread, TestContext context)
        {
            ContextTracker.SetThreadDefaultContext(thread, context.inner);
        }

        /// <summary>
        /// Gets the default context for the specified thread.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default context for a thread is <see cref="GlobalContext" /> unless the thread's
        /// default context has been overridden with <see cref="SetThreadDefaultContext" />.
        /// </para>
        /// <para>
        /// Changing the default context of a thread is useful for capturing existing threads created
        /// outside of a test into a particular context.  Among other things, this ensures that side-effects
        /// of the thread, such as writing text to the console, are recorded as part of the step
        /// represented by the specified context.
        /// </para>
        /// </remarks>
        /// <param name="thread">The thread.</param>
        /// <returns>The default context.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thread"/> is null.</exception>
        public static TestContext GetThreadDefaultContext(Thread thread)
        {
            return WrapContext(ContextTracker.GetThreadDefaultContext(thread));
        }

        /// <summary>
        /// Enters the specified context with the current thread.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Conceptually this method pushes the specified context onto the context
        /// stack for the current thread.  It then returns a cookie that can be used
        /// to restore the current thread's context to its previous value.
        /// </para>
        /// </remarks>
        /// <param name="context">The context to enter, or null to enter a scope
        /// without a context.</param>
        /// <returns>A cookie that can be used to restore the current thread's context to its previous value.</returns>
        /// <seealso cref="TestContextCookie"/>
        public static TestContextCookie EnterContext(TestContext context)
        {
            return new TestContextCookie(ContextTracker.EnterContext(context.inner));
        }

        /// <summary>
        /// Gets the parent context or null if this context has no parent.
        /// </summary>
        public TestContext Parent
        {
            get { return WrapContext(inner.Parent); }
        }

        /// <summary>
        /// Gets the test associated with the context.
        /// </summary>
        public Test Test
        {
            get { return TestStep.Test; }
        }

        /// <summary>
        /// Gets the test step associated with the context.
        /// </summary>
        public Model.Tree.TestStep TestStep
        {
            get { return inner.TestStep; }
        }

        /// <summary>
        /// Gets the log writer for this context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each test step gets its own log writer that is distinct from those
        /// of other steps.  So the log writer returned by this property is
        /// particular to the step represented by this test context.
        /// </para>
        /// </remarks>
        public MarkupDocumentWriter LogWriter
        {
            get { return inner.LogWriter; }
        }

        /// <summary>
        /// Gets the sandbox of the test step, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value will typically only be null in the case where the test step
        /// belongs to a different test framework that does not use sandboxes.
        /// </para>
        /// </remarks>
        public Sandbox Sandbox
        {
            get { return sandbox; }
        }

        /// <summary>
        /// Gets or sets the lifecycle phase the context is in.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string LifecyclePhase
        {
            get { return inner.LifecyclePhase; }
            set { inner.LifecyclePhase = value; }
        }

        /// <summary>
        /// Gets the current assertion count.
        /// </summary>
        public int AssertCount
        {
            get { return inner.AssertCount; }
        }

        /// <summary>
        /// Gets the step's outcome or its interim outcome if the test is still running.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value of this property is initially <see cref="TestOutcome.Passed" /> but may change
        /// over the course of execution to reflect the anticipated outcome of the test.  When
        /// the test finishes, its outcome is frozen.
        /// </para>
        /// <para>
        /// For example, this property enables code running as part of the tear down phase to
        /// determine whether the test is failing and to perform different actions in that case.
        /// </para>
        /// </remarks>
        /// <seealso cref="SetInterimOutcome"/>
        public TestOutcome Outcome
        {
            get { return inner.Outcome; }
        }

        /// <summary>
        /// Gets a copy of the step's final result or null if the test has not finished yet.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The test result cannot be changed.
        /// </para>
        /// </remarks>
        public TestResult Result
        {
            get { return inner.Result.Copy(); }
        }

        /// <summary>
        /// Returns true if the step associated with the context has finished execution
        /// and completed all <see cref="Finishing" /> actions.
        /// </summary>
        public bool IsFinished
        {
            get { return inner.IsFinished; }
        }

        /// <summary>
        /// Gets the user data collection associated with the context.  It may be used
        /// to associate arbitrary key/value pairs with the context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each context has its own distinct user data collection.
        /// </para>
        /// </remarks>
        public UserDataCollection Data
        {
            get { return inner.Data; }
        }

        /// <summary>
        /// The <see cref="Finishing" /> event is raised when the test is finishing to provide
        /// clients with an opportunity to perform additional clean up tasks after all ordinary
        /// test processing is finished.
        /// </summary>
        public event EventHandler Finishing
        {
            add
            {
                lock (this)
                    finishingHandlers += value;
            }
            remove
            {
                lock (this)
                    finishingHandlers -= value;
            }
        }

        /// <summary>
        /// Automatically executes an action when a triggering event occurs.
        /// </summary>
        /// <param name="triggerEvent">The triggering event.</param>
        /// <param name="triggerAction">The action to execute when triggered.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="triggerAction"/> is null.</exception>
        public void AutoExecute(TriggerEvent triggerEvent, Action triggerAction)
        {
            AutoExecute(triggerEvent, triggerAction, null);
        }

        /// <summary>
        /// Automatically executes an action when a triggering event occurs.
        /// </summary>
        /// <param name="triggerEvent">The triggering event.</param>
        /// <param name="triggerAction">The action to execute when triggered.</param>
        /// <param name="cleanupAction">The action to execute to clean up after triggering or when the test finishes without triggering having occurred, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="triggerAction"/> is null.</exception>
        [UserCodeEntryPoint]
        public void AutoExecute(TriggerEvent triggerEvent, Action triggerAction, Action cleanupAction)
        {
            if (triggerAction == null)
                throw new ArgumentNullException("triggerAction");

            Finishing += (sender, e) =>
            {
                if (IsTriggerEventSatisfied(triggerEvent))
                {
                    try
                    {
                        triggerAction();
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report("An exception occurred while performing an auto-execute trigger action.", ex);
                    }
                }

                if (cleanupAction != null)
                {
                    try
                    {
                        cleanupAction();
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report("An exception occurred while performing an auto-execute cleanup action.", ex);
                    }
                }
            };
        }

        /// <summary>
        /// Returns true if a trigger event has been satisfied.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method assumes that the body of the test has already finished and is
        /// currently running tear down, dispose, or finishing actions.
        /// </para>
        /// </remarks>
        /// <param name="triggerEvent">The trigger event.</param>
        /// <returns>True if the trigger event is satisfied.</returns>
        public bool IsTriggerEventSatisfied(TriggerEvent triggerEvent)
        {
            switch (triggerEvent)
            {
                case TriggerEvent.TestFinished:
                    return true;

                case TriggerEvent.TestPassed:
                    return Outcome.Status == TestStatus.Passed;

                case TriggerEvent.TestPassedOrInconclusive:
                    return Outcome.Status == TestStatus.Passed || Outcome.Status == TestStatus.Inconclusive;

                case TriggerEvent.TestInconclusive:
                    return Outcome.Status == TestStatus.Inconclusive;

                case TriggerEvent.TestFailed:
                    return Outcome.Status == TestStatus.Failed;

                case TriggerEvent.TestFailedOrInconclusive:
                    return Outcome.Status == TestStatus.Failed || Outcome.Status == TestStatus.Inconclusive;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Performs an action as a new step within the current context and associates it
        /// with the specified code reference.  Does not verify the outcome of the step.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="Model.Tree.TestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// <para>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </para>
        /// <para>
        /// This method does not verify that the test step completed successfully.  Check the
        /// <see cref="TestContext.Outcome" /> of the test step or call <see cref="RunStepAndVerifyOutcome"/>
        /// to ensure that the expected outcome was obtained.
        /// </para>
        /// </remarks>
        /// <param name="name">The name of the step.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="timeout">The step execution timeout, or null if none.</param>
        /// <param name="isTestCase">True if the step represents an independent test case.</param>
        /// <param name="codeElement">The associated code element, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null.</exception>
        /// <returns>The context of the step that ran.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is negative.</exception>
        public TestContext RunStep(string name, Action action, TimeSpan? timeout, bool isTestCase, ICodeElementInfo codeElement)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (action == null)
                throw new ArgumentNullException("action");
            if (timeout.HasValue && timeout.Value.Ticks < 0)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must not be negative.");

            TestContext childContext = StartChildStep(name, codeElement, isTestCase);
            TestOutcome outcome = TestOutcome.Error;
            try
            {
                childContext.LifecyclePhase = LifecyclePhases.Execute;
                using (childContext.Sandbox.StartTimer(timeout))
                {
                    outcome = childContext.Sandbox.Run(childContext.LogWriter, action, null);
                }
            }
            finally
            {
                childContext.FinishStep(outcome);
            }

            return childContext;
        }

        /// <summary>
        /// Performs an action as a new step within the current context and associates it
        /// with the specified code reference.  Verifies that the step produced the expected outcome.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="Model.Tree.TestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// <para>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </para>
        /// <para>
        /// This method verifies that the step produced the expected outcome.  If a different outcome
        /// was obtained, then raises an assertion failure.
        /// </para>
        /// </remarks>
        /// <param name="name">The name of the step.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="timeout">The step execution timeout, or null if none.</param>
        /// <param name="isTestCase">True if the step represents an independent test case.</param>
        /// <param name="codeElement">The associated code element, or null if none.</param>
        /// <param name="expectedOutcome">The expected outcome of the step.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null.</exception>
        /// <returns>The context of the step that ran.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is negative.</exception>
        /// <exception cref="AssertionFailureException">Thrown if the expected outcome was not obtained.</exception>
        public TestContext RunStepAndVerifyOutcome(string name, Action action, TimeSpan? timeout, bool isTestCase, ICodeElementInfo codeElement, TestOutcome expectedOutcome)
        {
            TestContext childContext = RunStep(name, action, timeout, isTestCase, codeElement);

            AssertionHelper.Verify(() =>
            {
                TestOutcome actualOutcome = childContext.Outcome;
                if (actualOutcome == expectedOutcome)
                    return null;

                return new AssertionFailureBuilder("The test step did not produce the expected outcome.")
                    .AddLabeledValue("Expected Outcome", expectedOutcome.ToString())
                    .AddLabeledValue("Actual Outcome", actualOutcome.ToString())
                    .ToAssertionFailure();
            });

            return childContext;
        }

        /// <summary>
        /// Adds metadata to the step that is running in the context.
        /// </summary>
        /// <param name="metadataKey">The metadata key.</param>
        /// <param name="metadataValue">The metadata value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadataKey"/>
        /// or <paramref name="metadataValue"/> is null.</exception>
        public void AddMetadata(string metadataKey, string metadataValue)
        {
            inner.AddMetadata(metadataKey, metadataValue);
        }

        /// <summary>
        /// Sets the step's interim <see cref="Outcome" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The interim outcome is used to communicate the anticipated outcome 
        /// of the step to later phases of execution.
        /// </para>
        /// <para>
        /// The value set here will be overridden by whatever final outcome the step
        /// returns.  Consequently the actual outcome may still differ from the anticipated outcome
        /// that was set using this method.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the outcome while the test is not running.</exception>
        /// <seealso cref="Outcome"/>
        public void SetInterimOutcome(TestOutcome outcome)
        {
            inner.SetInterimOutcome(outcome);
        }

        /// <summary>
        /// Enters this context with the current thread.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Conceptually this method pushes this context onto the context
        /// stack for the current thread.  It then returns a cookie that can be used
        /// to restore the current thread's context to its previous value.
        /// </para>
        /// </remarks>
        /// <returns>A cookie that can be used to restore the current thread's context to its previous value.</returns>
        /// <seealso cref="TestContextCookie"/>
        public TestContextCookie Enter()
        {
            return EnterContext(this);
        }

        /// <summary>
        /// Increments the assert count atomically.
        /// </summary>
        public void IncrementAssertCount()
        {
            AddAssertCount(1);
        }

        /// <summary>
        /// Adds the specified amount to the assert count atomically.
        /// </summary>
        /// <param name="value">The amount to add to the assert count.</param>
        public void AddAssertCount(int value)
        {
            inner.AddAssertCount(value);
        }

        /// <summary>
        /// Publishes a message to the message sink.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        internal void PublishMessage(Message message)
        {
            inner.MessageSink.Publish(message); // Callee checks argument.
        }

        /// <summary>
        /// Starts a child step of the context.
        /// </summary>
        /// <param name="name">The name of the step.</param>
        /// <param name="codeElement">The code element, or null if none.</param>
        /// <param name="isTestCase">True if the step represents an independent test case.</param>
        /// <returns>The context of the child step.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        internal TestContext StartChildStep(string name, ICodeElementInfo codeElement, bool isTestCase)
        {
            Model.Tree.TestStep testStep = new Model.Tree.TestStep(inner.TestStep.Test, inner.TestStep, name, codeElement, false);
            testStep.IsTestCase = isTestCase;
            testStep.IsDynamic = true;
            return PrepareContext(inner.StartChildStep(testStep), Sandbox.CreateChild());
        }

        /// <summary>
        /// Finishes the step represented by the context.
        /// </summary>
        /// <param name="outcome">The outcome.</param>
        /// <returns>The final test result.</returns>
        internal TestResult FinishStep(TestOutcome outcome)
        {
            return inner.FinishStep(outcome, null);
        }

        /// <summary>
        /// Prepares a <see cref="TestContext" /> wrapper for the given inner context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The wrapper is cached for the duration of the lifetime of the inner context.
        /// </para>
        /// </remarks>
        /// <param name="inner">The new inner context.</param>
        /// <param name="sandbox">The sandbox to use, or null if none.</param>
        /// <returns>The wrapper context.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null.</exception>
        internal static TestContext PrepareContext(ITestContext inner, Sandbox sandbox)
        {
            TestContext context = new TestContext(inner, sandbox);
            inner.Data.SetValue(GallioFrameworkContextKey, context);
            inner.Finishing += context.NotifyFinishing;
            return context;
        }

        /// <summary>
        /// Wraps an existing context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the context has already been prepared, returns
        /// the prepared context.  Otherwise creates a new wrapper.
        /// </para>
        /// </remarks>
        /// <param name="inner">The context to wrap, or null if none.</param>
        /// <returns>The wrapped context, or null if none.</returns>
        internal static TestContext WrapContext(ITestContext inner)
        {
            if (inner == null)
                return null;

            UserDataCollection data = inner.Data;
            lock (data)
            {
                TestContext context;
                if (! inner.Data.TryGetValue(GallioFrameworkContextKey, out context))
                    context = PrepareContext(inner, null);

                return context;
            }
        }

        private void NotifyFinishing(object sender, EventArgs e)
        {
            try
            {
                EventHandlerPolicy.SafeInvoke(finishingHandlers, this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                LogWriter.Failures.WriteException(ex, "An exception during Finishing event processing.");
            }
        }

        internal static TestContext GetCurrentContextOrThrow()
        {
            TestContext context = CurrentContext;
            if (context == null)
                throw new InvalidOperationException("There is no current test context.");
            return context;
        }
    }
}