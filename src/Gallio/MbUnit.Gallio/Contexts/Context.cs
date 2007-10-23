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
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using MbUnit.Logging;
using MbUnit.Model;
using MbUnit.Hosting;

namespace MbUnit.Contexts
{
    /// <summary>
    /// <para>
    /// The context provides information about the environment in which
    /// a test is executing.  A new context is created each time a test or
    /// test step begins execution.
    /// </para>
    /// <para>
    /// Contexts are arranged in a hierarchy
    /// that mirrors the containment relationship among test assemblies,
    /// fixtures, methods, steps and other test components.  Thus it is
    /// possible to access information about the containing test fixture or
    /// test assembly from within a test.
    /// </para>
    /// <para>
    /// Arbitrary user data can be associated with a context.  Furthermore, an
    /// event is dispatched when the context is disposed (because the test is exiting).
    /// The <see cref="Disposed" /> event can be used to implement robust resource
    /// reclamation rules that do not depend on direct framework support or the use
    /// of tear-down methods.
    /// </para>
    /// </summary>
    public abstract class Context
    {
        private static IContextManager cachedContextManager;

        private readonly Context parent;

        private Dictionary<string, object> data;

        private bool isDisposed;
        private event EventHandler disposedHandlers;
        private int assertCount;

        static Context()
        {
            Runtime.InstanceChanged += delegate { cachedContextManager = null; };
        }

        /// <summary>
        /// Creates a context.
        /// </summary>
        /// <param name="parent">The parent context, or null if this is the root context</param>
        protected Context(Context parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets the context manager.
        /// </summary>
        public static IContextManager ContextManager
        {
            get
            {
                if (cachedContextManager == null)
                    cachedContextManager = Runtime.Instance.Resolve<IContextManager>();
                return cachedContextManager;
            }
        }

        /// <summary>
        /// Gets the context of the current thread, or null if there is no
        /// current context.
        /// </summary>
        public static Context CurrentContext
        {
            get { return ContextManager.CurrentContext; }
        }

        /// <summary>
        /// Gets the root context of the environment, or null if there is no
        /// root context.
        /// </summary>
        public static Context RootContext
        {
            get { return ContextManager.RootContext; }
        }

        /// <summary>
        /// Gets reflection information about the current test.
        /// </summary>
        public static TestInfo CurrentTest
        {
            get { return CurrentContext.Test; }
        }

        /// <summary>
        /// Gets reflection information about the  current step.
        /// </summary>
        public static StepInfo CurrentStep
        {
            get { return CurrentContext.Step; }
        }

        /// <summary>
        /// <para>
        /// Sets the default context for the specified thread.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default context for a thread is <see cref="RootContext" /> unless the thread's
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
        /// thread's default context to the root context</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thread"/> is null</exception>
        public static void SetThreadDefaultContext(Thread thread, Context context)
        {
            ContextManager.SetThreadDefaultContext(thread, context);
        }

        /// <summary>
        /// <para>
        /// Gets the default context for the specified thread.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default context for a thread is <see cref="RootContext" /> unless the thread's
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
            return ContextManager.GetThreadDefaultContext(thread);
        }

        /// <summary>
        /// Gets the parent of this context or null if this is the root context.
        /// </summary>
        public Context Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Gets the test associated with the context.
        /// </summary>
        public abstract TestInfo Test { get; }

        /// <summary>
        /// Gets the test step associated with the context.
        /// </summary>
        public abstract StepInfo Step { get; }

        /// <summary>
        /// <para>
        /// Gets the log writer for this context.
        /// </para>
        /// <para>
        /// Each test step gets its own log writer that is distinct from those
        /// of other steps.  So the log write returned by this property is
        /// particular to the step represented by this test context.
        /// </para>
        /// </summary>
        public abstract LogWriter LogWriter { get; }

        /// <summary>
        /// Gets or sets the lifecycle phase the context is in.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string LifecyclePhase
        {
            get { return LifecyclePhaseImpl; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                LifecyclePhaseImpl = value;
            }
        }

        /// <summary>
        /// Gets the current assertion count.
        /// </summary>
        public int AssertCount
        {
            get { return assertCount; }
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
        public abstract TestOutcome Outcome { get; }

        /// <summary>
        /// Returns true if the context has been disposed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The context is disposed when the step that it represents finishes execution.
        /// The properties and methods of a disposed context can still be accessed
        /// as usual while cleanup activities are in progress.
        /// </para>
        /// </remarks>
        public bool IsDisposed
        {
            get { return isDisposed; }
        }

        /// <summary>
        /// <para>
        /// The disposed event is raised when the context has been disposed.
        /// The context is disposed when the step that it represents finishes execution.
        /// </para>
        /// <para>
        /// The context is disposed when the step that it represents finishes execution.
        /// The properties and methods of a disposed context can still be accessed
        /// as usual while cleanup activities are in progress.
        /// </para>
        /// <para>
        /// Clients may attach handlers to this event to perform cleanup
        /// activities and other tasks as needed.  If a new event handler is
        /// added and the context has already been disposed, the handler
        /// is immediately invoked.
        /// </para>
        /// </summary>
        public event EventHandler Disposed
        {
            add
            {
                lock (this)
                {
                    if (!isDisposed)
                    {
                        disposedHandlers += value;
                        return;
                    }
                }

                value(this, EventArgs.Empty);
            }
            remove
            {
                lock (this)
                    disposedHandlers -= value;
            }
        }

        /// <summary>
        /// Gets the context that represents the initial (root) step of the
        /// execution of the test associated with this context.
        /// </summary>
        /// <returns>The initial context of the test associated with this context</returns>
        public Context GetInitialContext()
        {
            Context context = this;
            TestInfo test = Test;
            for (;;)
            {
                Context parent = context.Parent;

                if (parent == null || parent.Test != test)
                    return context;

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
        /// Gets the value from the context with the specified key.
        /// </summary>
        /// <remarks>
        /// This method can still be used after the context has been disposed.
        /// </remarks>
        /// <param name="key">The context data key</param>
        /// <returns>The associated value, or null if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        public object GetData(string key)
        {
            lock (this)
            {
                if (data == null)
                    return null;

                object value;
                data.TryGetValue(key, out value);
                return value;
            }
        }

        /// <summary>
        /// Sets the value of the context data with the specified key.
        /// </summary>
        /// <remarks>
        /// This method can still be used after the context has been disposed.
        /// </remarks>
        /// <param name="key">The context data key</param>
        /// <param name="value">The value to store or null to remove it</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        public void SetData(string key, object value)
        {
            lock (this)
            {
                if (data == null)
                {
                    if (value != null)
                    {
                        data = new Dictionary<string, object>();
                        data[key] = value;
                    }
                }
                else
                {
                    if (value == null)
                        data.Remove(key);
                    else
                        data[key] = value;
                }
            }
        }

        /// <summary>
        /// Runs a block of code with this context.
        /// </summary>
        /// <remarks>
        /// Conceptually this method pushes this test context onto the context stack
        /// of the current thread so that it becomes the current context, runs the block,
        /// then unwinds the context stack for the current thread until this test context
        /// is again popped off the top.
        /// </remarks>
        /// <param name="block">The block to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="block "/> is null</exception>
        /// <exception cref="Exception">Any exception thrown by the block</exception>
        public void Run(Block block)
        {
            if (block == null)
                throw new ArgumentNullException(@"block");

            using (Enter())
                block();
        }

        /// <summary>
        /// Runs a block of code with this context using <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)" />.
        /// </summary>
        /// <param name="block">The block to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="block "/> is null</exception>
        public void RunBackground(Block block)
        {
            if (block == null)
                throw new ArgumentNullException(@"block");

            ThreadPool.QueueUserWorkItem(delegate
            {
                using (Enter())
                    block();
            });
        }

        /// <summary>
        /// Runs a block of code with this context asynchronously.
        /// </summary>
        /// <param name="block">The block to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="block "/> is null</exception>
        public IAsyncResult RunAsync(Block block)
        {
            return RunAsync(block, null, null);
        }

        /// <summary>
        /// Runs a block of code with this context asynchronously.
        /// </summary>
        /// <param name="block">The block to run</param>
        /// <param name="callback">The asynchronous callback, or null if none</param>
        /// <param name="state">The asynchronous state object, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="block "/> is null</exception>
        public IAsyncResult RunAsync(Block block, AsyncCallback callback, object state)
        {
            if (block == null)
                throw new ArgumentNullException(@"block");

            Block wrappedBlock = delegate
            {
                using (Enter())
                    block();
            };

            return wrappedBlock.BeginInvoke(callback, state);
        }

        /// <summary>
        /// <para>
        /// Runs a block of code as a new step within the current context and associates
        /// it with the caller's code reference.
        /// </para>
        /// <para>
        /// This method creates a new child context to represent the <see cref="Step" />,
        /// enters the child context, runs the block of code, then exits the child context.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </remarks>
        /// <param name="name">The name of the step</param>
        /// <param name="block">The block of code to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="block"/> is null</exception>
        /// <returns>The context of the step that ran</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string</exception>
        /// <exception cref="Exception">Any exception thrown by the block</exception>
        [SecurityPermission(SecurityAction.Demand)] // Prevent this method from being inlined.
        public Context RunStep(string name, Block block)
        {
            return RunStep(name, block, CodeReference.CreateFromCallingMethod());
        }

        /// <summary>
        /// <para>
        /// Runs a block of code as a new step within the current context and associates it
        /// with the specified code reference.
        /// </para>
        /// <para>
        /// This method creates a new child context to represent the <see cref="Step" />,
        /// enters the child context, runs the block of code, then exits the child context.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </remarks>
        /// <param name="name">The name of the step</param>
        /// <param name="block">The block of code to run</param>
        /// <param name="codeReference">The code reference, or null to use the calling method
        /// as the code reference.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="block"/> is null</exception>
        /// <returns>The context of the step that ran</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string</exception>
        /// <exception cref="Exception">Any exception thrown by the block</exception>
        [SecurityPermission(SecurityAction.Demand)] // Prevent this method from being inlined.
        public Context RunStep(string name, Block block, CodeReference codeReference)
        {
            if (name == null)
                throw new ArgumentNullException(@"name");
            if (block == null)
                throw new ArgumentNullException(@"block");
            if (codeReference == null)
                codeReference = CodeReference.CreateFromCallingMethod();

            return RunStepImpl(name, block, codeReference);
        }

        /// <summary>
        /// Enters this context with the current thread.
        /// </summary>
        /// <remarks>
        /// Conceptually this method pushes the specified context onto the context
        /// stack for the current thread.  It then returns a cookie that can be used
        /// to restore the current thread's context to its previous value.
        /// </remarks>
        /// <returns>A cookie that can be used to restore the current thread's context to its previous value</returns>
        /// <seealso cref="ContextCookie"/>
        public abstract ContextCookie Enter();

        /// <summary>
        /// Increments the assert count atomically.
        /// </summary>
        public void IncrementAssertCount()
        {
            Interlocked.Increment(ref assertCount);
        }

        /// <summary>
        /// Adds the specified amount to the assert count atomically.
        /// </summary>
        /// <param name="value">The amount to add to the assert count</param>
        public void AddAssertCount(int value)
        {
            Interlocked.Add(ref assertCount, value);
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
            if (metadataKey == null)
                throw new ArgumentNullException(@"metadataKey");
            if (metadataValue == null)
                throw new ArgumentNullException(@"metadataValue");

            AddMetadataImpl(metadataKey, metadataValue);
        }

        /// <summary>
        /// Completes the initialization of the context after it has been instantiated.
        /// </summary>
        protected virtual void Initialize()
        {
            if (parent != null)
                parent.Disposed += OnParentDisposed;
        }

        /// <summary>
        /// Disposes of the context.
        /// </summary>
        /// <param name="disposing">True if disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            LifecyclePhase = LifecyclePhases.Dispose;

            if (parent != null)
                parent.Disposed -= OnParentDisposed;

            EventHandler oldDisposedHandlers;
            lock (this)
            {
                if (isDisposed)
                    return;

                isDisposed = true;
                oldDisposedHandlers = disposedHandlers;
                disposedHandlers = null;
            }

            if (oldDisposedHandlers != null)
            {
                // Run all of the disposed handlers inside the context.
                // Log any exceptions that occur.
                using (Enter())
                {
                    foreach (EventHandler handler in oldDisposedHandlers.GetInvocationList())
                    {
                        try
                        {
                            handler(this, EventArgs.Empty);
                        }
                        catch (Exception ex)
                        {
                            LogWriter[LogStreamNames.Failures].WriteLine(
                                "An exception occurred while executing a Context Dispose handler:\n{0}", ex);
                        }
                    }
                }
            }
        }

        private void OnParentDisposed(object sender, EventArgs e)
        {
            Dispose(true);
        }

        /// <summary>
        /// Implementation of <see cref="LifecyclePhase" />.
        /// </summary>
        /// <remarks>
        /// The arguments will already have been validated is called and will all be non-null.
        /// </remarks>
        protected abstract string LifecyclePhaseImpl { get; set; }

        /// <summary>
        /// Implementation of <see cref="RunStep(string, Block, CodeReference)" />.
        /// </summary>
        /// <remarks>
        /// The arguments will already have been validated is called and will all be non-null.
        /// </remarks>
        protected abstract Context RunStepImpl(string name, Block block, CodeReference codeReference);

        /// <summary>
        /// Adds metadata to the step that is running in the context.
        /// </summary>
        /// <remarks>
        /// The arguments will already have been validated is called and will all be non-null.
        /// </remarks>
        protected abstract void AddMetadataImpl(string metadataKey, string metadataValue);
    }
}