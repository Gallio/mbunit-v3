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
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Gallio.Common.Platform;

namespace Gallio.Model.Contexts
{
    /// <summary>
    /// The default context tracker tracks the current context by way
    /// of the thread's <see cref="ExecutionContext" /> and <see cref="CallContext" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The .Net framework ensures that this context information flows across threads
    /// during asynchronous callbacks, timer events and thread pool work item execution.
    /// </para>
    /// </remarks>
    public class DefaultTestContextTracker : ITestContextTracker
    {
        private const int CleanupInterval = 60000;

        private readonly string contextKey;
        private readonly string illogicalContextKey;
        private readonly Dictionary<Thread, ITestContext> threadOverrides;

        private ITestContext globalContext;
        private Timer threadCleanupTimer;

        /// <summary>
        /// Initializes the context tracker.
        /// </summary>
        public DefaultTestContextTracker()
        {
            contextKey = typeof(DefaultTestContextTracker).Name + @"." + Guid.NewGuid();
            illogicalContextKey = contextKey + @".Illogical";
            threadOverrides = new Dictionary<Thread, ITestContext>();
        }

        private object SyncRoot
        {
            get { return threadOverrides; }
        }

        /// <inheritdoc />
        public ITestContext GlobalContext
        {
            get { return globalContext; }
            set { globalContext = value; }
        }

        /// <inheritdoc />
        public ITestContext CurrentContext
        {
            get { return GetCurrentContextImpl(); }
        }

        /// <inheritdoc />
        public IDisposable EnterContext(ITestContext context)
        {
            InternalContextLink previousTopLink = TopContextLinkForCurrentThread;
            TopContextLinkForCurrentThread = new InternalContextLink(previousTopLink, context);

            return new InternalContextCookie(this, previousTopLink);
        }

        /// <inheritdoc />
        public void SetThreadDefaultContext(Thread thread, ITestContext context)
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
        public ITestContext GetThreadDefaultContext(Thread thread)
        {
            if (thread == null)
                throw new ArgumentNullException(@"thread");

            return GetThreadDefaultContextImpl(thread);
        }

        private ITestContext GetCurrentContextImpl()
        {
            InternalContextLink contextLink = TopContextLinkForCurrentThread;

            if (contextLink != null)
                return contextLink.Context;

            return GetThreadDefaultContextImpl(Thread.CurrentThread);
        }

        private ITestContext GetThreadDefaultContextImpl(Thread thread)
        {
            lock (SyncRoot)
            {
                ITestContext context;
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

        // Gets or sets the top context link for the current thread.
        //
        // The trick here is to wrap the context link up in an <see cref="ObjectHandle" />.
        // This enables the context link to reside in the logical call context (so it flows
        // across threads) while preventing it from actually getting serialized during
        // remote calls (which won't work).  On the other hand, during callbacks from
        // remote calls we will be able to unwrap the context link and keep going.
        // 
        // We also use an illogical call context slot as a backup in case the logical
        // call context slot gets wiped.  This seems to happen during certain remote calls
        // which do not appear to correctly preserve the logical call context during
        // the round trip.  This backup value will not be transmitted across threads, however.
        // FIXME.  -- Jeff.
        private InternalContextLink TopContextLinkForCurrentThread
        {
            get
            {
                ObjectHandle handle = (ObjectHandle)
                    (LogicalGetData(contextKey) ?? CallContext.GetData(illogicalContextKey));
                if (handle == null)
                    return null;

                return (InternalContextLink)handle.Unwrap();
            }
            set
            {
                if (value == null)
                {
                    CallContext.FreeNamedDataSlot(contextKey);
                    CallContext.FreeNamedDataSlot(illogicalContextKey);
                }
                else
                {
                    ObjectHandle handle = new ObjectHandle(value);
                    LogicalSetData(contextKey, handle);
                    CallContext.SetData(illogicalContextKey, handle);
                }
            }
        }

        private static void LogicalSetData(string key, object value)
        {
            if (! DotNetRuntimeSupport.IsUsingMono) // not implemented in mono
                CallContext.LogicalSetData(key, value);
        }

        private static object LogicalGetData(string key)
        {
            if (!DotNetRuntimeSupport.IsUsingMono) // not implemented in mono
                return CallContext.LogicalGetData(key);
            return null;
        }

        // Ensures the cleanup timer runs if and only if there is at least 1 thread override.
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

        // Removes overrides for threads that are no longer alive.
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

        // Represents a single link in a chain of contexts per-thread.
        // We use a linked list because we want each stack of contexts to be distinct
        // across threads even when the execution context is cloned to flow across
        // threads.
        private sealed class InternalContextLink
        {
            private readonly InternalContextLink parentLink;
            private readonly ITestContext context;

            public InternalContextLink(InternalContextLink parentLink, ITestContext context)
            {
                this.parentLink = parentLink;
                this.context = context;
            }

            public InternalContextLink ParentLink
            {
                get { return parentLink; }
            }

            public ITestContext Context
            {
                get { return context; }
            }
        }

        private sealed class InternalContextCookie : IDisposable
        {
            private readonly DefaultTestContextTracker contextTracker;
            private readonly InternalContextLink previousTopLink;
            private readonly int threadId;

            public InternalContextCookie(DefaultTestContextTracker contextTracker, InternalContextLink previousTopLink)
            {
                this.contextTracker = contextTracker;
                this.previousTopLink = previousTopLink;

                threadId = Thread.CurrentThread.ManagedThreadId;
            }

            public void Dispose()
            {
                contextTracker.ExitContext(previousTopLink, threadId);
            }
        }
    }
}