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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Gallio.Contexts;

namespace Gallio.Contexts
{
    /// <summary>
    /// The default context manager tracks the current context by way
    /// of the thread's <see cref="ExecutionContext" /> and <see cref="CallContext" />.
    /// The .Net framework ensures that this context information flows across threads
    /// during asynchronous callbacks, timer events and thread pool work item execution.
    /// </summary>
    public class DefaultContextManager : IContextManager
    {
        private const int CleanupInterval = 60000;

        private readonly string contextKey;
        private readonly Dictionary<Thread, Context> threadOverrides;

        private Context globalContext;
        private Timer threadCleanupTimer;

        /// <summary>
        /// Initializes the context manager.
        /// </summary>
        public DefaultContextManager()
        {
            contextKey = @"DefaultContextManager." + Guid.NewGuid();
            threadOverrides = new Dictionary<Thread, Context>();
        }

        private object SyncRoot
        {
            get { return threadOverrides; }
        }

        /// <inheritdoc />
        public Context GlobalContext
        {
            get { return globalContext; }
            set { globalContext = value; }
        }

        /// <inheritdoc />
        public Context CurrentContext
        {
            get { return GetCurrentContextImpl(); }
        }

        /// <inheritdoc />
        public ContextCookie EnterContext(Context context)
        {
            InternalContextLink previousTopLink = TopContextLinkForCurrentThread;
            TopContextLinkForCurrentThread = new InternalContextLink(previousTopLink, context);

            return new InternalContextCookie(this, previousTopLink);
        }

        /// <inheritdoc />
        public void SetThreadDefaultContext(Thread thread, Context context)
        {
            if (thread == null)
                throw new ArgumentNullException(@"thread");
            
            lock (SyncRoot)
            {
                if (context == null || context == globalContext)
                    threadOverrides.Remove(thread);
                else
                    threadOverrides[thread] = context;

                ConfigureThreadCleanupTimerWithLock();
            }
        }

        /// <inheritdoc />
        public Context GetThreadDefaultContext(Thread thread)
        {
            if (thread == null)
                throw new ArgumentNullException(@"thread");

            return GetThreadDefaultContextImpl(thread);
        }

        private Context GetCurrentContextImpl()
        {
            InternalContextLink contextLink = TopContextLinkForCurrentThread;

            if (contextLink != null)
                return contextLink.Context;

            return GetThreadDefaultContextImpl(Thread.CurrentThread);
        }

        private Context GetThreadDefaultContextImpl(Thread thread)
        {
            lock (SyncRoot)
            {
                Context context;
                if (threadOverrides.TryGetValue(thread, out context))
                    return context;

                return globalContext;
            }
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
        /// <remarks>
        /// The trick here is to wrap the context link up in an <see cref="ObjectHandle" />.
        /// This enables the context link to reside in the logical call context (so it flows
        /// across threads) while preventing it from actually getting serialized during
        /// remote calls (which won't work).  On the other hand, during callbacks from
        /// remote calls we will be able to unwrap the context link and keep going.
        /// </remarks>
        private InternalContextLink TopContextLinkForCurrentThread
        {
            get
            {
                ObjectHandle handle = (ObjectHandle) CallContext.LogicalGetData(contextKey);
                if (handle == null)
                    return null;

                return (InternalContextLink)handle.Unwrap();
            }
            set
            {
                if (value == null)
                    CallContext.FreeNamedDataSlot(contextKey);
                else
                    CallContext.LogicalSetData(contextKey, new ObjectHandle(value));
            }
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

        /// <summary>
        /// Represents a single link in a chain of contexts per-thread.
        /// We use a linked list because we want each stack of contexts to be distinct
        /// across threads even when the execution context is cloned to flow across
        /// threads.
        /// </summary>
        private sealed class InternalContextLink
        {
            private readonly InternalContextLink parentLink;
            private readonly Context context;

            public InternalContextLink(InternalContextLink parentLink, Context context)
            {
                this.parentLink = parentLink;
                this.context = context;
            }

            public InternalContextLink ParentLink
            {
                get { return parentLink; }
            }

            public Context Context
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
    }
}