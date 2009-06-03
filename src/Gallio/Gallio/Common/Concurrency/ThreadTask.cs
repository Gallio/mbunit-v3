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
using System.Diagnostics;
using System.Threading;
using Gallio;
using Gallio.Common.Policies;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// An implementation of <see cref="Task" /> based on a locally running thread.
    /// </summary>
    public class ThreadTask : Task
    {
        private readonly Invoker invoker;
        private readonly ThreadAbortScope threadAbortScope = new ThreadAbortScope();
        private Thread thread;
        private ApartmentState apartmentState = ApartmentState.Unknown;

        /// <summary>
        /// Creates a task that will execute code within a new locally running thread.
        /// When the task terminates successfully, its result will contain the value
        /// returned by <paramref name="action"/>.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <param name="action">The action to perform within the thread.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> 
        /// or <paramref name="action"/> is null.</exception>
        public ThreadTask(string name, Func<object> action)
            : base(name)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            invoker = new Invoker(action);
        }

        /// <summary>
        /// Creates a task that will execute code within a new locally running thread.
        /// When the task terminates successfully, its result will contain the value <c>null</c>.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <param name="action">The action to perform within the thread.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> 
        /// or <paramref name="action"/> is null.</exception>
        public ThreadTask(string name, Action action)
            : base(name)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            invoker = new Invoker(action);
        }

        /// <summary>
        /// Gets or sets the desired <see cref="ApartmentState"/> to use for
        /// the thread when it is started.
        /// </summary>
        /// <value>
        /// The default value is <see cref="System.Threading.ApartmentState.Unknown" /> which
        /// causes the new thread to use the runtime's default apartment state.
        /// </value>
        /// <exception cref="InvalidOperationException">Thrown if this method is called
        /// after the thread has started.</exception>
        public ApartmentState ApartmentState
        {
            get { return apartmentState; }
            set
            {
                if (thread != null)
                    throw new InvalidOperationException("thread");
                apartmentState = value;
            }
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
                thread = new Thread(Run);
                thread.IsBackground = true;
                thread.Name = String.Format("Task: {0}", Name);

                if (apartmentState != ApartmentState.Unknown)
                    thread.SetApartmentState(apartmentState);

                thread.Start();
            }
        }

        /// <inheritdoc />
        protected override void AbortImpl()
        {
            lock (this)
            {
                threadAbortScope.Abort();
            }
        }

        /// <inheritdoc />
        protected override bool JoinImpl(TimeSpan? timeout)
        {
            Thread cachedThread;
            lock (this)
            {
                cachedThread = thread;
            }

            if (cachedThread != null)
                return cachedThread.Join(timeout.HasValue ? (int)timeout.Value.TotalMilliseconds : Timeout.Infinite);

            return true;
        }

        [DebuggerHidden, DebuggerStepThrough]
        private void Run()
        {
            try
            {
                try
                {
                    ThreadAbortException ex = threadAbortScope.Run(invoker.Invoke);

                    if (ex != null)
                    {
                        NotifyTerminated(TaskResult.CreateFromException(ex));
                    }
                    else
                    {
                        NotifyTerminated(TaskResult.CreateFromValue(invoker.Result));
                    }
                }
                catch (Exception ex)
                {
                    NotifyTerminated(TaskResult.CreateFromException(ex));
                }
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An unhandled exception occurred in a thread task.", ex);
            }
        }

        // Wraps the delegate in a manner that is compatible with ThreadStart such that
        // we can apply [DebuggerHidden] and [DebuggerStepThrough] to ease debugging.
        private sealed class Invoker
        {
            private readonly Delegate @delegate;
            public object Result { get; set; }

            public Invoker(Action action)
            {
                @delegate = action;
            }

            public Invoker(Func<object> action)
            {
                @delegate = action;
            }

            [DebuggerHidden, DebuggerStepThrough]
            public void Invoke()
            {
                Action action = @delegate as Action;
                if (action != null)
                    action();
                else
                    Result = ((Func<object>)@delegate)();
            }
        }
    }
}
