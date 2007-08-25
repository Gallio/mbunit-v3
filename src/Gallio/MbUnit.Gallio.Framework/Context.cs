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
using System.Threading;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Contexts;
using MbUnit.Framework.Kernel.Runtime;

namespace MbUnit.Framework
{
    /// <summary>
    /// The context class provides services for manipulating the test execution
    /// context.  When a test runs, a certain amount of information is made available
    /// to the test framework so that it can reflect upon the state of its own execution,
    /// track its progress across multiple threads and better manage its resources.
    /// Exposing the context enables a greater degree of integration for custom test
    /// framework code embedded within the MbUnit runtime.
    /// </summary>
    public static class Context
    {
        private const string AttachedResourcesKey = "$$AttachedResources$$";

        /// <summary>
        /// Gets the singleton context manager.
        /// </summary>
        public static IContextManager ContextManager
        {
            get { return RuntimeHolder.Instance.Resolve<IContextManager>(); }
        }

        /// <summary>
        /// Gets the context of the current thread.
        /// </summary>
        public static IContext CurrentContext
        {
            get { return ContextManager.CurrentContext; }
        }

        /// <summary>
        /// Gets the current test.
        /// </summary>
        public static ITest CurrentTest
        {
            get { return CurrentContext.CurrentTest; }
        }

        /// <summary>
        /// Gets the current step.
        /// </summary>
        public static IStep CurrentStep
        {
            get { return CurrentContext.CurrentStep; }
        }

        /// <summary>
        /// The exited event is raised when the context is about to be exited.
        /// Clients may attach handlers to this event to perform cleanup
        /// activities and other tasks as needed.
        /// </summary>
        public static event EventHandler Exiting
        {
            add { CurrentContext.Exiting += value; }
            remove { CurrentContext.Exiting -= value; }
        }
        
        /// <summary>
        /// Creates a new thread, attaches it to the current context but does not start it.
        /// The thread will be automatically aborted when the context exits.
        /// <seealso cref="AttachThread"/>, <seealso cref="SpawnThread"/>.
        /// </summary>
        /// <param name="name">The name of the thread</param>
        /// <param name="threadStart">The delegate to run when the thread is started</param>
        /// <returns>The new thread, initially not started</returns>
        public static Thread CreateThread(string name, ThreadStart threadStart)
        {
            IContext ownerContext = CurrentContext;
            Thread thread = new Thread((ThreadStart) delegate
            {
                RunWithContext(ownerContext, delegate
                {
                    try
                    {
                        threadStart();
                    }
                    catch (Exception ex)
                    {
                        if (! AppDomain.CurrentDomain.IsFinalizingForUnload())
                            Assert.Warning("An exception bubbled up to the entry point of a thread created by the test.\n{0}", ex);
                    }
                });
            });
            thread.IsBackground = true;
            thread.Name = name;

            return thread;
        }

        /// <summary>
        /// Creates a new thread, attaches it to the current context and starts it.
        /// The thread will be automatically aborted when the context exits.
        /// <seealso cref="AttachThread"/>, <seealso cref="CreateThread"/>.
        /// </summary>
        /// <param name="name">The name of the thread</param>
        /// <param name="threadStart">The delegate to run when the thread is started</param>
        /// <returns>The new thread, has been started</returns>
        public static Thread SpawnThread(ThreadStart threadStart, string name)
        {
            Thread thread = CreateThread(name, threadStart);
            thread.Start();

            return thread;
        }

        /// <summary>
        /// Runs a block of code within the specified context.
        /// The thread's current context is switched to that which is specified
        /// for the duration of the block then restored to what it was before.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="block">The block to run</param>
        public static void RunWithContext(IContext context, Block block)
        {
            ContextManager.RunWithContext(context, block);
        }

        /// <summary>
        /// Sets the context of the specified thread, overriding any that it
        /// might normally have inherited from its environment.
        /// Context information becomes accessible to code running on that thread
        /// so that assertion failures and execution log messages generated by the
        /// thread are captured by the context and included in the output.
        /// If the context is null, resets the context of the thread to just that it
        /// would otherwise have acquired.
        /// Unlike <see cref="AttachThread" /> the thread is not attached to the context,
        /// so it will not be aborted automatically when the context exits.
        /// </summary>
        /// <param name="thread">The thread whose context is to be set</param>
        /// <param name="context">The context to associate with the thread, or null to reset to the default</param>
        public static void SetThreadContext(Thread thread, IContext context)
        {
            ContextManager.SetThreadContext(thread, context);
        }

        /// <summary>
        /// Sets the context of the specified thread and attaches it to the current context
        /// so that it will be aborted automatically when the current context exits.
        /// Current context information becomes accessible to code running on that thread.
        /// <seealso cref="SetThreadContext"/>
        /// </summary>
        /// <remarks>
        /// A reference to the thread is maintained until detached or the context exits.
        /// A thread should be attached to at most one context at a time.
        /// This method is equivalent to setting the thread context and attaching the thread
        /// to the current context as a resource with a cleanup action to abort the thread.
        /// </remarks>
        /// <param name="thread">The thread to attach.</param>
        public static void AttachThread(Thread thread)
        {
            SetThreadContext(thread, CurrentContext);
            AttachResource(thread, AbortThread);
        }

        /// <summary>
        /// Resets the context of the specified thread and detaches it from the current context.
        /// Current context information becomes inaccessible to core running on that thread.
        /// </summary>
        /// <remarks>
        /// This method does nothing if the thread is not attached to the current context.
        /// This method is requivalent to resetting the thread context and detaching the
        /// thread from the current context as a resource.
        /// </remarks>
        /// <param name="thread">The thread to detach</param>
        public static void DetachThread(Thread thread)
        {
            SetThreadContext(thread, null);
            DetachResource(thread);
        }

        /// <summary>
        /// Attaches a disposable resource to the context such that it will be disposed automatically
        /// when the context exits.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A reference to the resource is maintained until detached or the context exits.
        /// This method is equivalent to calling <see cref="AttachResource{T}" /> with an 
        /// action that disposes the resource.
        /// </para>
        /// <para>
        /// This method overrides the previous cleanup action if the resource is already attached
        /// to the current thread.
        /// </para>
        /// </remarks>
        /// <param name="resource">The disposable resource to attach</param>
        public static void AttachResource(IDisposable resource)
        {
            AttachResource(resource, DisposeResource);
        }

        /// <summary>
        /// Attaches a resource to the context such that it will be cleaned up automatically
        /// when the context exits.
        /// A reference to the resource is maintained until detached or the context exits.
        /// </summary>
        /// <remarks>
        /// This method overrides the previous cleanup action if the resource is already attached
        /// to the current thread.
        /// </remarks>
        /// <param name="resource">The resource to attach</param>
        /// <param name="cleanupAction">The cleanup action to apply to the resource when the context exits</param>
        public static void AttachResource<T>(T resource, Action<T> cleanupAction)
        {
            lock (CurrentContext.SyncRoot)
            {
                IDictionary<object, Block> resources;
                if (! CurrentContext.TryGetData(AttachedResourcesKey, out resources))
                {
                    resources = new Dictionary<object, Block>();
                    CurrentContext.SetData(AttachedResourcesKey, resources);
                    CurrentContext.Exiting += CleanupAttachedResources;
                }

                resources[resource] = delegate
                {
                    cleanupAction(resource);
                };
            }
        }

        /// <summary>
        /// Detaches a disposable resource from the context such that it will no longer
        /// be cleaned up automatically when the context exits.
        /// </summary>
        /// <remarks>
        /// This method does nothing if the resource is not attached to the current context.
        /// </remarks>
        /// <param name="resource">The resource to detach</param>
        public static void DetachResource(object resource)
        {
            lock (CurrentContext.SyncRoot)
            {
                IDictionary<object, Block> resources;
                if (CurrentContext.TryGetData(AttachedResourcesKey, out resources))
                {
                    resources.Remove(resource);

                    if (resources.Count == 0)
                    {
                        CurrentContext.RemoveData(AttachedResourcesKey);
                        CurrentContext.Exiting -= CleanupAttachedResources;
                    }
                }
            }
        }

        private static void CleanupAttachedResources(object sender, EventArgs e)
        {
            lock (CurrentContext.SyncRoot)
            {
                IDictionary<object, Block> resources;
                if (CurrentContext.TryGetData(AttachedResourcesKey, out resources))
                {
                    foreach (Block cleanupAction in resources.Values)
                        cleanupAction();
                }
            }
        }

        private static void DisposeResource(IDisposable resource)
        {
            resource.Dispose();
        }

        private static void AbortThread(Thread thread)
        {
            thread.Abort();
        }
    }
}
