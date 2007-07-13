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
using System.Text;
using System.Threading;
using MbUnit.Framework.Model;
using MbUnit.Framework;
using MbUnit.Framework.Services.Contexts;

namespace MbUnit.Core.Services.Contexts
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

        /// <summary>
        /// Initializes the context manager.
        /// </summary>
        public DefaultContextManager()
        {
            rootContext = CreateContext(null, null);
        }

        private static IContext CurrentCallContextSlotValue
        {
            get { return CallContext.GetData(ContextKey) as IContext; }
            set { CallContext.SetData(ContextKey, value); }
        }

        /// <inheritdoc />
        public IContext RootContext
        {
            get { return rootContext; }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IContext CreateContext(IContext parent, ITest test)
        {
            return new DefaultContext(parent, test);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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