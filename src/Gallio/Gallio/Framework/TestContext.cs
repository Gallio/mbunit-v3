// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Collections;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Logging;
using Gallio.Reflection;
using Gallio.Utilities;

namespace Gallio.Framework
{
    /// <summary>
    /// <para>
    /// The test context provides information about the environment in which
    /// a test is executing.  A new context is created each time a test or
    /// test step begins execution.
    /// </para>
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
    /// </summary>
    /// <seealso cref="Framework.TestStep"/>
    public sealed class TestContext
    {
        private static readonly Key<TestContext> GallioFrameworkContextKey = new Key<TestContext>("Gallio.Framework.Context");
        private readonly ITestContext inner;
        private readonly Sandbox sandbox;
        private EventHandler finishingHandlers;

        /// <summary>
        /// Creates a wrapper for a <see cref="ITestContext" />.
        /// </summary>
        /// <param name="inner">The context to wrap</param>
        /// <param name="sandbox">The sandbox to use, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null</exception>
        private TestContext(ITestContext inner, Sandbox sandbox)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
            this.sandbox = sandbox;
        }

        private static ITestContextTracker ContextTracker
        {
            get { return TestContextTrackerAccessor.GetInstance(); }
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
        /// <para>
        /// Sets the default context for the specified thread.
        /// </para>
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
        /// <param name="thread">The thread</param>
        /// <param name="context">The context to associate with the thread, or null to reset the
        /// thread's default context to inherit the <see cref="GlobalContext" /> once again</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thread"/> is null</exception>
        public static void SetThreadDefaultContext(Thread thread, TestContext context)
        {
            ContextTracker.SetThreadDefaultContext(thread, context.inner);
        }

        /// <summary>
        /// <para>
        /// Gets the default context for the specified thread.
        /// </para>
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
        /// <param name="thread">The thread</param>
        /// <returns>The default context</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thread"/> is null</exception>
        public static TestContext GetThreadDefaultContext(Thread thread)
        {
            return WrapContext(ContextTracker.GetThreadDefaultContext(thread));
        }

        /// <summary>
        /// Enters the specified context with the current thread.
        /// </summary>
        /// <remarks>
        /// Conceptually this method pushes the specified context onto the context
        /// stack for the current thread.  It then returns a cookie that can be used
        /// to restore the current thread's context to its previous value.
        /// </remarks>
        /// <param name="context">The context to enter, or null to enter a scope
        /// without a context</param>
        /// <returns>A cookie that can be used to restore the current thread's context to its previous value</returns>
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
        public TestInfo Test
        {
            get { return TestStep.Test; }
        }

        /// <summary>
        /// Gets the test step associated with the context.
        /// </summary>
        public TestStepInfo TestStep
        {
            get { return new TestStepInfo(inner.TestStep); }
        }

        /// <summary>
        /// <para>
        /// Gets the log writer for this context.
        /// </para>
        /// <para>
        /// Each test step gets its own log writer that is distinct from those
        /// of other steps.  So the log writer returned by this property is
        /// particular to the step represented by this test context.
        /// </para>
        /// </summary>
        public TestLogWriter LogWriter
        {
            get { return inner.LogWriter; }
        }

        /// <summary>
        /// Gets the sandbox of the test step, or null if none.
        /// </summary>
        /// <remarks>
        /// The value will typically only be null in the case where the test step
        /// belongs to a different test framework that does not use sandboxes.
        /// </remarks>
        public Sandbox Sandbox
        {
            get { return sandbox; }
        }

        /// <summary>
        /// Gets or sets the lifecycle phase the context is in.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
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
        /// <para>
        /// Gets the step's outcome or its interim outcome if the test is still running.
        /// </para>
        /// <para>
        /// The value of this property is initially <see cref="TestOutcome.Passed" /> but may change
        /// over the course of execution to reflect the anticipated outcome of the test.  When
        /// the test finishes, its outcome is frozen.
        /// </para>
        /// </summary>
        /// <remarks>
        /// For example, this property enables code running as part of the tear down phase to
        /// determine whether the test is failing and to perform different actions in that case.
        /// </remarks>
        /// <seealso cref="SetInterimOutcome"/>
        public TestOutcome Outcome
        {
            get { return inner.Outcome; }
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
        /// <para>
        /// Gets the user data collection associated with the context.  It may be used
        /// to associate arbitrary key/value pairs with the context.
        /// </para>
        /// <para>
        /// When a new child context is created, it inherits a copy of its parent's data.
        /// </para>
        /// </summary>
        public UserDataCollection Data
        {
            get { return inner.Data; }
        }

        /// <summary>
        /// <para>
        /// The <see cref="Finishing" /> event is raised when the test is finishing to provide
        /// clients with an opportunity to perform additional clean up tasks after all ordinary
        /// test processing is finished.
        /// </para>
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
        /// <para>
        /// Performs an action as a new step within the current context and associates
        /// it with the calling function.
        /// </para>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="ITestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </remarks>
        /// <param name="name">The name of the step</param>
        /// <param name="action">The action to perform</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null</exception>
        /// <returns>The context of the step that ran</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string</exception>
        /// <exception cref="Exception">Any exception thrown by the action</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public TestContext RunStep(string name, Action action)
        {
            return RunStep(name, Reflector.GetCallingFunction(), action);
        }

        /// <summary>
        /// <para>
        /// Performs an action as a new step within the current context and associates it
        /// with the specified code reference.
        /// </para>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="ITestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </remarks>
        /// <param name="name">The name of the step</param>
        /// <param name="codeElement">The associated code element, or null if none</param>
        /// <param name="action">The action to perform</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null</exception>
        /// <returns>The context of the step that ran</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string</exception>
        /// <exception cref="Exception">Any exception thrown by the action</exception>
        public TestContext RunStep(string name, ICodeElementInfo codeElement, Action action)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (action == null)
                throw new ArgumentNullException("action");

            TestContext childContext = StartChildStep(name, codeElement);

            childContext.LifecyclePhase = LifecyclePhases.Execute;
            TestOutcome outcome = childContext.Sandbox.Run(action, null);

            childContext.FinishStep(outcome);
            return childContext;
        }

        /// <summary>
        /// Adds metadata to the step that is running in the context.
        /// </summary>
        /// <param name="metadataKey">The metadata key</param>
        /// <param name="metadataValue">The metadata value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadataKey"/>
        /// or <paramref name="metadataValue"/> is null</exception>
        public void AddMetadata(string metadataKey, string metadataValue)
        {
            inner.AddMetadata(metadataKey, metadataValue);
        }

        /// <summary>
        /// <para>
        /// Sets the step's interim <see cref="Outcome" />.  The interim outcome is used
        /// to communicate the anticipated outcome of the step to later phases of execution.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value set here will be overridden by whatever final outcome the step
        /// returns.  Consequently the actual outcome may still differ from the anticipated outcome
        /// that was set using this method.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the outcome while the test is not running</exception>
        /// <seealso cref="Outcome"/>
        public void SetInterimOutcome(TestOutcome outcome)
        {
            inner.SetInterimOutcome(outcome);
        }

        /// <summary>
        /// Enters this context with the current thread.
        /// </summary>
        /// <remarks>
        /// Conceptually this method pushes this context onto the context
        /// stack for the current thread.  It then returns a cookie that can be used
        /// to restore the current thread's context to its previous value.
        /// </remarks>
        /// <returns>A cookie that can be used to restore the current thread's context to its previous value</returns>
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
        /// <param name="value">The amount to add to the assert count</param>
        public void AddAssertCount(int value)
        {
            inner.AddAssertCount(value);
        }

        /// <summary>
        /// Starts a child step of the context.
        /// </summary>
        /// <param name="name">The name of the step</param>
        /// <param name="codeElement">The code element, or null if none</param>
        /// <returns>The context of the child step</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        internal TestContext StartChildStep(string name, ICodeElementInfo codeElement)
        {
            return PrepareContext(inner.StartChildStep(name, codeElement), Sandbox.CreateChild());
        }

        /// <summary>
        /// Finishes the step represented by the context.
        /// </summary>
        /// <param name="outcome">The outcome</param>
        internal void FinishStep(TestOutcome outcome)
        {
            inner.FinishStep(outcome, null);
        }

        /// <summary>
        /// Prepares a <see cref="TestContext" /> wrapper for the given inner context.
        /// The wrapper is cached for the duration of the lifetime of the inner context.
        /// </summary>
        /// <param name="inner">The new inner context</param>
        /// <param name="sandbox">The sandbox to use, or null if none</param>
        /// <returns>The wrapper context</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null</exception>
        internal static TestContext PrepareContext(ITestContext inner, Sandbox sandbox)
        {
            TestContext context = new TestContext(inner, sandbox);
            inner.Data.SetValue(GallioFrameworkContextKey, context);
            inner.Finishing += context.NotifyFinishing;
            return context;
        }

        /// <summary>
        /// Wraps an existing context.  If the context has already been prepared, returns
        /// the prepared context.  Otherwise creates a new wrapper.
        /// </summary>
        /// <param name="inner">The context to wrap, or null if none</param>
        /// <returns>The wrapped context, or null if none</returns>
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
                EventHandlerUtils.SafeInvoke(finishingHandlers, this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                LogWriter.Failures.WriteException(ex, "An exception during Finishing event processing.");
            }
        }
    }
}