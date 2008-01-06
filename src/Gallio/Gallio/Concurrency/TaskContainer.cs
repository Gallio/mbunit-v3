// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Diagnostics;
using Gallio.Collections;

namespace Gallio.Concurrency
{
    /// <summary>
    /// <para>
    /// A <see cref="TaskContainer" /> manages the lifecycle of any number of <see cref="Task" />s
    /// and monitors their run-time behavior.
    /// </para>
    /// <para>
    /// For example, when a <see cref="Task" /> terminates abruptly due to an exception, its container
    /// will send out a notification that may cause all of the other tasks to be aborted and
    /// for the currently executing test case to fail.
    /// </para>
    /// </summary>
    public class TaskContainer
    {
        private readonly HashSet<Task> activeTasks;
        private event EventHandler<TaskEventArgs> started;
        private event EventHandler<TaskEventArgs> aborted;
        private event EventHandler<TaskEventArgs> terminated;

        /// <summary>
        /// Creates an empty task container.
        /// </summary>
        public TaskContainer()
        {
            activeTasks = new HashSet<Task>();
        }

        /// <summary>
        /// Adds or removes an event handler that is signaled when any watched task is started.
        /// </summary>
        public event EventHandler<TaskEventArgs> TaskStarted
        {
            add { lock (this) started += value; }
            remove { lock (this) started -= value; }
        }

        /// <summary>
        /// Adds or removes an event handler that is signaled when any watched task is aborted.
        /// </summary>
        public event EventHandler<TaskEventArgs> TaskAborted
        {
            add { lock (this) aborted += value; }
            remove { lock (this) aborted -= value; }
        }

        /// <summary>
        /// Adds or removes an event handler that is signaled when any watched task is terminated.
        /// </summary>
        public event EventHandler<TaskEventArgs> TaskTerminated
        {
            add { lock (this) terminated += value; }
            remove { lock (this) terminated -= value; }
        }

        /// <summary>
        /// Adds a new task for this container to watch.
        /// </summary>
        /// <param name="task">The task to monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="task"/> is null</exception>
        public void Watch(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            task.Started += HandleTaskStarted;
            task.Aborted += HandleTaskAborted;
            task.Terminated += HandleTaskTerminated;
        }

        /// <summary>
        /// Asynchronously interrupts all of the tasks currently running within the container.
        /// </summary>
        /// <seealso cref="Task.Interrupt"/>
        /// <seealso cref="AbortAll"/>
        public void InterruptAll()
        {
            lock (this)
            {
                foreach (Task task in activeTasks)
                    task.Interrupt();
            }
        }

        /// <summary>
        /// Asynchronously aborts all of the tasks currently running within the container.
        /// </summary>
        /// <seealso cref="Task.Abort"/>
        /// <seealso cref="InterruptAll"/>
        public void AbortAll()
        {
            lock (this)
            {
                foreach (Task task in activeTasks)
                    task.Abort();
            }
        }

        /// <summary>
        /// Waits for all of currently running tasks to terminate.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait</param>
        /// <returns>True if no tasks are running as of the time this method exits,
        /// false if a timeout occurred while waiting</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/>
        /// represents a negative time span</exception>
        /// <seealso cref="Task.Join"/>
        public bool JoinAll(TimeSpan timeout)
        {
            if (timeout.Ticks < 0)
                throw new ArgumentOutOfRangeException("timeout");

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Loop repeatedly until no active tasks remain.
            // We do this in case new tasks are spawned while we wait for others to finish up.
            bool elapsed = false;
            for (; ; )
            {
                // Collect a snapshot of the list of active tasks.
                Task[] cachedActiveTasks;
                lock (this)
                {
                    if (activeTasks.Count == 0)
                        return true;
                    if (elapsed)
                        return false;

                    cachedActiveTasks = new Task[activeTasks.Count];
                    activeTasks.CopyTo(cachedActiveTasks, 0);
                }

                // Loop over them all at least once until a timeout occurs.
                // This ensures we continue cleaning up all possible tasks
                // We keep going even after a timeout to ensure that we clean up
                // all tasks that have finished even when the timeout is 0.
                foreach (Task task in cachedActiveTasks)
                {
                    TimeSpan remainingTimeout;
                    if (!elapsed)
                    {
                        remainingTimeout = timeout - stopwatch.Elapsed;

                        if (remainingTimeout.Ticks <= 0)
                        {
                            elapsed = true;
                            remainingTimeout = new TimeSpan();
                        }
                    }
                    else
                    {
                        remainingTimeout = new TimeSpan();
                    }

                    task.Join(remainingTimeout);
                }
            }
        }

        private void HandleTaskStarted(object sender, TaskEventArgs e)
        {
            EventHandler<TaskEventArgs> cachedChain;
            lock (this)
            {
                activeTasks.Add(e.Task);
                cachedChain = started;
            }

            if (cachedChain != null)
                cachedChain(this, e);
        }

        private void HandleTaskAborted(object sender, TaskEventArgs e)
        {
            EventHandler<TaskEventArgs> cachedChain;
            lock (this)
                cachedChain = aborted;

            if (cachedChain != null)
                cachedChain(this, e);
        }

        private void HandleTaskTerminated(object sender, TaskEventArgs e)
        {
            EventHandler<TaskEventArgs> cachedChain;
            lock (this)
            {
                activeTasks.Remove(e.Task);
                cachedChain = terminated;
            }

            if (cachedChain != null)
                cachedChain(this, e);
        }
    }
}
