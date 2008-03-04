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
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using Gallio;
using Gallio.Collections;
using Gallio.Framework;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Gallio.Utilities;

namespace Gallio.Framework
{
    /// <summary>
    /// <para>
    /// The context provides information about the environment in which
    /// a test is executing.  A new context is created each time a test or
    /// test step begins execution.
    /// </para>
    /// <para>
    /// Contexts are arranged in a hierarchy that corresponds to the order in which
    /// the contexts were entered.  Thus the context for a test likely has as
    /// its parent the context for its containing test fixture.
    /// </para>
    /// <para>
    /// Arbitrary user data can be associated with a context.  Furthermore, client
    /// code may attach <see cref="CleanUp" /> event handlers to perform resource
    /// reclamation just prior to marking the test step as finished.
    /// </para>
    /// </summary>
    /// <seealso cref="Step"/>
    public sealed class Context
    {
        private readonly ITestContext inner;

        /// <summary>
        /// Creates a wrapper for a <see cref="ITestContext" />.
        /// </summary>
        /// <param name="inner">The context to wrap</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null</exception>
        private Context(ITestContext inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
        }

        private static ITestContextTracker ContextTracker
        {
            get { return TestContextTrackerAccessor.GetInstance(); }
        }

        /// <summary>
        /// Gets the context of the current thread, or null if there is no
        /// current context.
        /// </summary>
        public static Context CurrentContext
        {
            get { return Wrap(ContextTracker.CurrentContext); }
        }

        /// <summary>
        /// Gets the global context of the environment, or null if there is no
        /// such context.
        /// </summary>
        public static Context GlobalContext
        {
            get { return Wrap(ContextTracker.GlobalContext); }
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
        public static void SetThreadDefaultContext(Thread thread, Context context)
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
        public static Context GetThreadDefaultContext(Thread thread)
        {
            return Wrap(ContextTracker.GetThreadDefaultContext(thread));
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
        /// <seealso cref="ContextCookie"/>
        public static ContextCookie EnterContext(Context context)
        {
            return new ContextCookie(ContextTracker.EnterContext(context.inner));
        }

        /// <summary>
        /// Performs an action within the specified context.
        /// </summary>
        /// <remarks>
        /// Conceptually this method pushes the specified context onto the context stack
        /// of the current thread so that it becomes the current context, performs the action,
        /// then unwinds the context stack for the current thread until this test context
        /// is again popped off the top.
        /// </remarks>
        /// <param name="context">The context within which to perform the action, or null to 
        /// perform it without a context</param>
        /// <param name="action">The action to perform</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action "/> is null</exception>
        /// <exception cref="Exception">Any exception thrown by the action</exception>
        public static void RunWithContext(Context context, Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            using (EnterContext(context))
                action();
        }

        /// <summary>
        /// Gets the parent context or null if this context has no parent.
        /// </summary>
        public Context Parent
        {
            get { return Wrap(inner.Parent); }
        }

        /// <summary>
        /// Gets the test associated with the context.
        /// </summary>
        public TestInfo Test
        {
            get { return TestInstance.Test; }
        }

        /// <summary>
        /// Gets the test instance associated with the context.
        /// </summary>
        public TestInstanceInfo TestInstance
        {
            get { return TestStep.TestInstance; }
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
        public LogWriter LogWriter
        {
            get { return inner.LogWriter; }
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
        /// Gets the step's outcome.  Ths value of this property is initially
        /// <see cref="TestOutcome.Passed" /> but may change over the course of execution
        /// depending on how particular lifecycle phases behave.  The step's outcome value
        /// becomes frozen once the step finishes.
        /// </summary>
        /// <remarks>
        /// For example, this property enables code running in a tear down method to
        /// determine whether the test failed and to perform different actions in that case.
        /// </remarks>
        public TestOutcome Outcome
        {
            get { return inner.Outcome; }
        }

        /// <summary>
        /// Returns true if the step associated with the context has finished execution
        /// and completed all <see cref="CleanUp" /> actions.
        /// </summary>
        public bool IsFinished
        {
            get { return inner.IsFinished; }
        }

        /// <summary>
        /// Gets the user data collection associated with the context.  It may be used
        /// to associate arbitrary key/value pairs with the context.
        /// </summary>
        public UserDataCollection Data
        {
            get { return inner.Data; }
        }

        /// <summary>
        /// <para>
        /// The <see cref="CleanUp" /> event is raised just before the test step
        /// finishes execution to perform resource reclamation.
        /// </para>
        /// <para>
        /// Clients may attach handlers to this event to perform cleanup
        /// activities and other tasks as needed.  If a new event handler is
        /// added and the step has already finished, the handler is immediately invoked.
        /// </para>
        /// </summary>
        public event EventHandler CleanUp
        {
            add
            {
                EventHandler wrapper;
                lock (inner.Data)
                {
                    IDictionary<EventHandler, EventHandler> wrappers = CleanUpWrappers;
                    if (wrappers == null)
                    {
                        wrappers = new Dictionary<EventHandler, EventHandler>();
                        CleanUpWrappers = wrappers;
                    }
                    else if (wrappers.ContainsKey(value))
                        return;

                    wrapper = delegate { value(this, EventArgs.Empty); };
                    wrappers.Add(value, wrapper);
                }

                inner.CleanUp += wrapper;
            }
            remove
            {
                EventHandler wrapper;
                lock (inner.Data)
                {
                    IDictionary<EventHandler, EventHandler> wrappers = CleanUpWrappers;
                    if (wrappers == null || ! wrappers.TryGetValue(value, out wrapper))
                        return;

                    wrappers.Remove(value);
                }

                inner.CleanUp -= wrapper;
            }
        }
        private IDictionary<EventHandler, EventHandler> CleanUpWrappers
        {
            get { return inner.Data.GetValue<IDictionary<EventHandler, EventHandler>>(CleanUpWrappersKey); }
            set { inner.Data.SetValue(CleanUpWrappersKey, value); }
        }
        private const string CleanUpWrappersKey = "Context.CleanUpHandlerWrappers";

        /// <summary>
        /// Gets the context that represents the initial (root) step of the
        /// execution of the test associated with this context.
        /// </summary>
        /// <returns>The initial context of the test associated with this context</returns>
        public Context GetInitialContext()
        {
            ITestContext context = inner;
            ITest test = inner.TestStep.TestInstance.Test;
            for (;;)
            {
                ITestContext parent = context.Parent;

                if (parent == null || parent.TestStep.TestInstance.Test != test)
                    return Wrap(context);

                context = parent;
            }
        }

        /// <summary>
        /// Gets the context that represents the initial (root) step of the
        /// execution of the parent of the test associated with this context.
        /// </summary>
        /// <returns>The initial context of the parent of the test associated
        /// with this context, or null if there is no parent test</returns>
        public Context GetInitialContextOfParentTest()
        {
            Context parent = GetInitialContext().Parent;
            return parent == null ? null : parent.GetInitialContext();
        }

        /// <summary>
        /// Performs an action within this context.
        /// </summary>
        /// <remarks>
        /// Conceptually this method pushes this test context onto the context stack
        /// of the current thread so that it becomes the current context, performs the action,
        /// then unwinds the context stack for the current thread until this test context
        /// is again popped off the top.
        /// </remarks>
        /// <param name="action">The action to perform</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action "/> is null</exception>
        /// <exception cref="Exception">Any exception thrown by the action</exception>
        public void Run(Action action)
        {
            RunWithContext(this, action);
        }

        /// <summary>
        /// Performs an action within this context using <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)" />.
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action "/> is null</exception>
        public void RunBackground(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            ThreadPool.QueueUserWorkItem(delegate
            {
                using (Enter())
                    action();
            });
        }

        /// <summary>
        /// Performs an action asynchronously within this context.
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action "/> is null</exception>
        public IAsyncResult RunAsync(Action action)
        {
            return RunAsync(action, null, null);
        }

        /// <summary>
        /// Performs an action within this context asynchronously.
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="callback">The asynchronous callback, or null if none</param>
        /// <param name="state">The asynchronous state object, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action "/> is null</exception>
        public IAsyncResult RunAsync(Action action, AsyncCallback callback, object state)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Action wrappedAction = delegate
            {
                using (Enter())
                    action();
            };

            return wrappedAction.BeginInvoke(callback, state);
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
        [NonInlined(SecurityAction.Demand)]
        public Context RunStep(string name, Action action)
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
        public Context RunStep(string name, ICodeElementInfo codeElement, Action action)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (action == null)
                throw new ArgumentNullException("action");

            ITestContext childContext = inner.StartChildStep(name, codeElement);

            childContext.LifecyclePhase = LifecyclePhases.Execute;
            TestOutcome outcome = TestActionInvoker.Run(action, null, null);

            childContext.FinishStep(outcome, null);

            return new Context(childContext);
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
        /// Enters this context with the current thread.
        /// </summary>
        /// <remarks>
        /// Conceptually this method pushes this context onto the context
        /// stack for the current thread.  It then returns a cookie that can be used
        /// to restore the current thread's context to its previous value.
        /// </remarks>
        /// <returns>A cookie that can be used to restore the current thread's context to its previous value</returns>
        /// <seealso cref="ContextCookie"/>
        public ContextCookie Enter()
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

        private static Context Wrap(ITestContext inner)
        {
            return inner != null ? new Context(inner) : null;
        }
    }
}