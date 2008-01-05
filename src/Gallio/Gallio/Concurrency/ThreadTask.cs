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
using System.Threading;
using Gallio;

namespace Gallio.Concurrency
{
    /// <summary>
    /// An implementation of <see cref="Task" /> based on a locally running thread.
    /// </summary>
    public class ThreadTask : Task
    {
        private static readonly object abortToken = new object();

        private readonly Factory<object> block;
        private volatile Thread thread;

        /// <summary>
        /// Creates a task that will execute code within a new locally running thread.
        /// When the task terminates successfully, its result will contain the value
        /// returned by <paramref name="block"/>.
        /// </summary>
        /// <param name="name">The name of the task</param>
        /// <param name="block">The block of code to run within the thread</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> 
        /// or <paramref name="block"/> is null</exception>
        public ThreadTask(string name, Factory<object> block)
            : base(name)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            this.block = block;
        }

        /// <summary>
        /// Creates a task that will execute code within a new locally running thread.
        /// When the task terminates successfully, its result will contain the value <c>null</c>.
        /// </summary>
        /// <param name="name">The name of the task</param>
        /// <param name="block">The block of code to run within the thread</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> 
        /// or <paramref name="block"/> is null</exception>
        public ThreadTask(string name, Block block)
            : base(name)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            this.block = delegate
            {
                block();
                return null;
            };
        }

        /// <summary>
        /// Gets the thread on which the task is running, or null if the
        /// task is not running.
        /// </summary>
        public Thread Thread
        {
            get { return thread; }
        }

        /// <inheritdoc />
        protected override void StartImpl()
        {
            lock (this)
            {
                Thread newThread = new Thread(Run);
                newThread.IsBackground = true;
                newThread.Name = String.Format("Task: {0}", Name);
                newThread.Start();
            }
        }

        /// <inheritdoc />
        protected override void InterruptImpl()
        {
            lock (this)
            {
                if (thread != null)
                    thread.Interrupt();
            }
        }

        /// <inheritdoc />
        protected override void AbortImpl()
        {
            lock (this)
            {
                if (thread != null)
                    thread.Abort(abortToken);
            }
        }

        /// <inheritdoc />
        protected override bool JoinImpl(TimeSpan timeout)
        {
            Thread cachedThread = thread;

            if (cachedThread != null)
                return cachedThread.Join(timeout);

            return true;
        }

        private void Run()
        {
            try
            {
                object value = RunUserCode();
                ClearInterruptedFlag();

                NotifyTerminated(TaskResult.CreateFromValue(value));
            }
            catch (ThreadAbortException ex)
            {
                if (ex.ExceptionState == abortToken)
                {
                    Thread.ResetAbort();
                    ClearInterruptedFlag();
                }

                NotifyTerminated(TaskResult.CreateFromException(ex));
            }
            catch (Exception ex)
            {
                ClearInterruptedFlag();

                NotifyTerminated(TaskResult.CreateFromException(ex));
            }
        }

        private object RunUserCode()
        {
            // Note: The two identical finally blocks here serve a purpose.
            // There are 3 possible cases where the thread may be aborted.
            // 
            // 1. It may occur within user code in which case we will reset the thread field in the innermost finally.
            // 2. It may occur within the innermost finally block in which case we will reset the thread field in the outermost finally.
            // 3. It may occur somewhere else in which case we leave it alone.  This can only happen due to some
            //    other code outside of this class causing the Abort because of how the thread field governs aborts.
            try
            {
                try
                {
                    thread = Thread.CurrentThread;

                    // Cause a thread abort here in case we were aborted while we were not running user code.
                    if (IsAborted)
                        Thread.Abort(abortToken);

                    return block();
                }
                finally
                {
                    thread = null;
                }
            }
            finally
            {
                thread = null;
            }
        }

        private static void ClearInterruptedFlag()
        {
            try
            {
                Thread.Sleep(0);
            }
            catch (ThreadInterruptedException)
            {
                // Ignore it.
            }
        }
    }
}
