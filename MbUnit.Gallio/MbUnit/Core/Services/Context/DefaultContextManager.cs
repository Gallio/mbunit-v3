using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using MbUnit.Core.Services.Reflection;
using MbUnit.Framework;

namespace MbUnit.Core.Services.Context
{
    /// <summary>
    /// The default context manager tracks the current context by way
    /// of the <see cref="CallContext" />.
    /// </summary>
    public sealed class DefaultContextManager : IContextManager
    {
        private const string ContextKey = "$$DefaultContextManager.Context$$";

        private readonly object syncRoot = new object();

        private IContext rootContext;
        private IDictionary<Thread, IContext> threadOverrides;

        public DefaultContextManager()
        {
            rootContext = CreateContext(null, TestInfo.CreateRootTestInfo());
        }

        /// <summary>
        /// Gets or sets the current call context slot value.
        /// </summary>
        private static IContext CurrentCallContextSlotValue
        {
            get { return CallContext.GetData(ContextKey) as IContext; }
            set { CallContext.SetData(ContextKey, value); }
        }

        public IContext RootContext
        {
            get { return rootContext; }
        }

        public IContext CurrentContext
        {
            get
            {
                lock (syncRoot)
                {
                    IContext context;

                    if (threadOverrides != null)
                    {
                        if (threadOverrides.TryGetValue(Thread.CurrentThread, out context))
                        {
                            return context;
                        }
                    }

                    context = CurrentCallContextSlotValue;
                    if (context == null)
                        return rootContext;

                    return context;
                }
            }
        }

        public IContext CreateContext(IContext parent, TestInfo testInfo)
        {
            return new DefaultContext(parent, testInfo);
        }

        public void SetThreadContext(Thread thread, IContext context)
        {
            lock (syncRoot)
            {
                if (context != null)
                {
                    if (threadOverrides != null)
                        threadOverrides.Remove(thread);
                }
                else
                {
                    if (threadOverrides == null)
                        threadOverrides = new Dictionary<Thread, IContext>();

                    threadOverrides[thread] = context;
                }
            }
        }

        private IContext GetOverriddenThreadContext(Thread thread)
        {
            lock (syncRoot)
            {
                if (threadOverrides != null)
                {
                    IContext context;
                    if (threadOverrides.TryGetValue(thread, out context))
                        return context;
                }

                return null;
            }
        }

        public void RunWithContext(IContext context, Block block)
        {
            // Create a new execution context within which to run the block.
            // This ensures that call-context based inheritance of the context
            // object works correctly within the block.
            Thread thread = Thread.CurrentThread;
            ExecutionContext innerExecutionContext = thread.ExecutionContext.CreateCopy();
            ExecutionContext.Run(innerExecutionContext, delegate
            {
                // Set the context.
                CurrentCallContextSlotValue = context;

                IContext oldThreadContext;
                lock (syncRoot)
                {
                    oldThreadContext =
                        GetOverriddenThreadContext(thread);
                    SetThreadContext(thread, null);
                }

                try
                {
                    // Run the supplied code block.
                    block();
                }
                finally
                {
                    // Restore the context.
                    SetThreadContext(thread, oldThreadContext);
                }
            }, null);
        }
    }
}