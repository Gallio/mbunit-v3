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
using System.Runtime.Remoting.Messaging;
using System.Threading;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.RuntimeSupport
{
    /// <summary>
    /// The default context manager tracks the current context by way
    /// of the thread's <see cref="ExecutionContext" /> and <see cref="CallContext" />.
    /// The .Net framework ensures that this context information flows across threads
    /// during asynchronous callbacks, timer events and thread pool work item execution.
    /// </summary>
    public class DefaultContextManager : ICoreContextManager
    {
        private const int CleanupInterval = 60000;

        private readonly string contextKey;
        private readonly Dictionary<Thread, InternalContext> threadOverrides;

        private InternalContext rootContext;
        private Timer threadCleanupTimer;

        /// <summary>
        /// Initializes the context manager.
        /// </summary>
        public DefaultContextManager()
        {
            contextKey = @"DefaultContextManager." + Guid.NewGuid();
            threadOverrides = new Dictionary<Thread, InternalContext>();
        }

        private object SyncRoot
        {
            get { return threadOverrides; }
        }

        /// <inheritdoc />
        public Context RootContext
        {
            get { return rootContext; }
        }

        /// <inheritdoc />
        public Context CurrentContext
        {
            get { return GetCurrentContextImpl(); }
        }

        /// <inheritdoc />
        public Context InitializeRootContext(ICoreContextServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(@"serviceProvider");

            lock (SyncRoot)
            {
                if (rootContext != null)
                    throw new InvalidOperationException("The root context has already been initialized.");

                rootContext = new InternalContext(this, null, serviceProvider);
                rootContext.Initialize();
                return rootContext;
            }
        }

        /// <inheritdoc />
        public void DisposeRootContext()
        {
            // Dispose the old root context.
            InternalContext oldRootContext;
            lock (SyncRoot)
            {
                if (rootContext == null)
                    return;

                oldRootContext = rootContext;
            }

            oldRootContext.Dispose();

            // Clear out remaining state assuming no other thread has barged
            // in here and done it already.
            lock (SyncRoot)
            {
                if (rootContext != oldRootContext)
                    return; // check for race condition with other disposers!

                rootContext = null;
                threadOverrides.Clear();
                ConfigureThreadCleanupTimerWithLock();
            }
        }

        /// <inheritdoc />
        public Context CreateChildContext(Context parent, ICoreContextServiceProvider serviceProvider)
        {
            if (parent == null)
                throw new ArgumentNullException(@"parent");
            if (serviceProvider == null)
                throw new ArgumentNullException(@"serviceProvider");
            InternalContext internalParent = CastContextArgument(parent, @"parent");

            return new InternalContext(this, internalParent, serviceProvider);
        }

        /// <inheritdoc />
        public void DisposeContext(Context context)
        {
            if (context == null)
                throw new ArgumentNullException(@"context");
            InternalContext internalContext = CastContextArgument(context, @"parent");

            internalContext.Dispose();
        }

        /// <inheritdoc />
        public void SetThreadDefaultContext(Thread thread, Context context)
        {
            if (thread == null)
                throw new ArgumentNullException(@"thread");
            InternalContext internalContext = CastContextArgument(context, @"context");
            
            lock (SyncRoot)
            {
                if (context == null || context == rootContext)
                    threadOverrides.Remove(thread);
                else
                    threadOverrides[thread] = internalContext;

                ConfigureThreadCleanupTimerWithLock();
            }
        }

        /// <inheritdoc />
        public Context GetThreadDefaultContext(Thread thread)
        {
            if (thread == null)
                throw new ArgumentNullException(@"thread");

            return GetThreadDefaultContext(thread);
        }

        private InternalContext GetCurrentContextImpl()
        {
            InternalContextLink contextLink = TopContextLinkForCurrentThread;

            if (contextLink != null)
                return contextLink.Context;

            return GetThreadDefaultContextImpl(Thread.CurrentThread);
        }

        private InternalContext GetThreadDefaultContextImpl(Thread thread)
        {
            lock (SyncRoot)
            {
                InternalContext context;
                if (threadOverrides.TryGetValue(thread, out context))
                    return context;

                return rootContext;
            }
        }

        private ContextCookie EnterContext(InternalContext context)
        {
            InternalContextLink previousTopLink = TopContextLinkForCurrentThread;
            TopContextLinkForCurrentThread = new InternalContextLink(previousTopLink, context);

            return new InternalContextCookie(this, previousTopLink);
        }

        private void ExitContext(InternalContextLink previousTopLink, int threadId)
        {
            if (Thread.CurrentThread.ManagedThreadId != threadId)
                throw new InvalidOperationException("The context cookie does not belong to this thread.");

            InternalContextLink currentLink = TopContextLinkForCurrentThread;
            while (currentLink != null)
            {
                currentLink = currentLink.ParentLink;

                if (currentLink == previousTopLink)
                {
                    TopContextLinkForCurrentThread = currentLink;
                    return;
                }
            }

            throw new InvalidOperationException("The context has already been exited.");
        }

        /// <summary>
        /// Gets or sets the top context link for the current thread.
        /// </summary>
        private InternalContextLink TopContextLinkForCurrentThread
        {
            get { return (InternalContextLink)CallContext.GetData(contextKey); }
            set { CallContext.SetData(contextKey, value); }
        }

        /// <summary>
        /// Ensures the cleanup timer runs if and only if there is at least 1 thread override.
        /// </summary>
        private void ConfigureThreadCleanupTimerWithLock()
        {
            if (threadOverrides.Count == 0)
            {
                if (threadCleanupTimer != null)
                {
                    threadCleanupTimer.Dispose();
                    threadCleanupTimer = null;
                }
            }
            else
            {
                if (threadCleanupTimer == null)
                {
                    threadCleanupTimer = new Timer(CleanupThreads, null, CleanupInterval, CleanupInterval);
                }
            }
        }

        /// <summary>
        /// Removes overrides for threads that are no longer alive.
        /// </summary>
        private void CleanupThreads(object dummy)
        {
            lock (SyncRoot)
            {
                List<Thread> deadThreads = null;
                foreach (Thread thread in threadOverrides.Keys)
                {
                    if (!thread.IsAlive)
                    {
                        if (deadThreads == null)
                            deadThreads = new List<Thread>();

                        deadThreads.Add(thread);
                    }
                }

                if (deadThreads != null)
                {
                    foreach (Thread thread in deadThreads)
                        threadOverrides.Remove(thread);

                    ConfigureThreadCleanupTimerWithLock();
                }
            }
        }

        private InternalContext CastContextArgument(Context context, string argumentName)
        {
            InternalContext internalContext = context as InternalContext;

            if (internalContext == null && context != null)
                throw new ArgumentException("Context is not of the expected type.", argumentName);
            if (!internalContext.BelongsTo(this))
                throw new ArgumentException("Context does not belong to this context manager.", argumentName);

            return internalContext;
        }

        /// <summary>
        /// Represents a single link in a chain of contexts per-thread.
        /// We use a linked list because we want each stack of contexts to be distinct
        /// across threads even when the execution context is cloned to flow across
        /// threads.
        /// </summary>
        private sealed class InternalContextLink
        {
            private readonly InternalContextLink parentLink;
            private readonly InternalContext context;

            public InternalContextLink(InternalContextLink parentLink, InternalContext context)
            {
                this.parentLink = parentLink;
                this.context = context;
            }

            public InternalContextLink ParentLink
            {
                get { return parentLink; }
            }

            public InternalContext Context
            {
                get { return context; }
            }
        }

        private sealed class InternalContextCookie : ContextCookie
        {
            private readonly DefaultContextManager contextManager;
            private readonly InternalContextLink previousTopLink;
            private readonly int threadId;

            public InternalContextCookie(DefaultContextManager contextManager, InternalContextLink previousTopLink)
            {
                this.contextManager = contextManager;
                this.previousTopLink = previousTopLink;

                threadId = Thread.CurrentThread.ManagedThreadId;
            }

            public override void ExitContext()
            {
                contextManager.ExitContext(previousTopLink, threadId);
            }
        }

        private sealed class InternalContext : Context, IDisposable
        {
            private readonly DefaultContextManager contextManager;
            private readonly ICoreContextServiceProvider serviceProvider;

            private TestInfo testInfo;
            private StepInfo stepInfo;

            public InternalContext(DefaultContextManager contextManager,
                InternalContext parent, ICoreContextServiceProvider serviceProvider)
                : base(parent)
            {
                this.contextManager = contextManager;
                this.serviceProvider = serviceProvider;
            }

            new public void Initialize()
            {
                base.Initialize();
            }

            public bool BelongsTo(DefaultContextManager contextManager)
            {
                return this.contextManager == contextManager;
            }

            public void Dispose()
            {
                Dispose(true);
            }

            public override TestInfo Test
            {
                get
                {
                    if (testInfo == null)
                        testInfo = new TestInfo(serviceProvider.Step.Test);
                    return testInfo;
                }
            }

            public override StepInfo Step
            {
                get
                {
                    if (stepInfo == null)
                        stepInfo = new StepInfo(serviceProvider.Step);
                    return stepInfo;
                }
            }

            public override LogWriter LogWriter
            {
                get { return serviceProvider.LogWriter; }
            }

            public override string LifecyclePhase
            {
                get { return serviceProvider.LifecyclePhase; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException(@"value");

                    serviceProvider.LifecyclePhase = value;
                }
            }

            public override void RunStep(string name, Block block)
            {
                if (name == null)
                    throw new ArgumentNullException("name");
                if (name.Length == 0)
                    throw new ArgumentException("Name must not be empty.", "name");
                if (block == null)
                    throw new ArgumentNullException("block");

                serviceProvider.RunStep(name, block);
            }

            public override ContextCookie Enter()
            {
                return contextManager.EnterContext(this);
            }
        }
    }
}