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
using Gallio;
using Gallio.Concurrency;
using Gallio.Contexts;
using Gallio.Logging;

namespace MbUnit.Framework.Concurrency
{
    /// <summary>
    /// <para>
    /// The task manager provides a mechanism for coordinating the actions of multiple
    /// tasks within a test case.
    /// </para>
    /// <para>
    /// Each task started by a test case is monitored.  If the task fails, 
    /// </para>
    /// 
    /// The task manager uses the current <see cref="Context" /> to ensure that tasks that
    /// are started by a test are automatically disposed when the test finishes.
    /// </summary>
    public static class TaskManager
    {
        private const string ContainerKey = "TaskManager.Container";
        private const string FailureFlagKey = "TaskManager.Failure";

        private static readonly TimeSpan DisposeTimeout = new TimeSpan(0, 0, 30);

        /// <summary>
        /// Gets the task container for the current <see cref="Context" />.
        /// </summary>
        public static TaskContainer TaskContainer
        {
            get { return GetTaskContainer(); }
        }

        /// <summary>
        /// Adds a new task for the task manager to watch.
        /// </summary>
        /// <param name="task">The task to watch</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="task"/> is null</exception>
        public static void WatchTask(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            GetTaskContainer().Watch(task);
        }

        /// <summary>
        /// Starts a new task running in a local thread and begins watching it.
        /// </summary>
        /// <param name="name">The name of the task, or null to create a new name based
        /// on the method associated with the action</param>
        /// <param name="action">The action to perform</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static ThreadTask StartThreadTask(string name, Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            ThreadTask task = new ThreadTask(GetTaskName(name, action), action);
            WatchTask(task);
            task.Start();
            return task;
        }

        /// <summary>
        /// Waits for all tasks to complete or for timeout to occur.
        /// Then verifies that no failures have occurred in any of the tasks.
        /// </summary>
        /// <param name="timeout">The timeout</param>
        /// <exception cref="AssertionException">Thrown if some of the tasks did not complete
        /// or if any of the tasks failed</exception>
        public static void JoinAndVerify(TimeSpan timeout)
        {
            if (!GetTaskContainer().JoinAll(timeout))
                Assert.Fail("One or more test tasks did not terminate before the timeout expired.");

            Verify();
        }

        /// <summary>
        /// Verifies that no failures have occurred in any of the tasks.
        /// </summary>
        /// <exception cref="AssertionException">Thrown if any of the tasks failed</exception>
        public static void Verify()
        {
            if (FailureFlag)
                Assert.Fail("One or more test tasks failed.");
        }

        private static TaskContainer GetTaskContainer()
        {
            Context context = Context.CurrentContext;
            if (context == null || context.IsDisposed)
                throw new InvalidOperationException("The task manager cannot be used because there is no current context.");

            lock (context)
            {
                TaskContainer container = (TaskContainer)context.GetData(ContainerKey);
                if (container == null)
                {
                    container = CreateContainer(context);
                    context.SetData(ContainerKey, container);
                }

                return container;
            }
        }

        private static TaskContainer CreateContainer(Context context)
        {
            TaskContainer container = new TaskContainer();

            context.Disposed += delegate { ReapTasks(context, container); };
            container.TaskTerminated += delegate(object sender, TaskEventArgs e) { RecordTaskResult(context, e.Task); };

            return container;
        }

        private static void ReapTasks(Context context, TaskContainer container)
        {
            container.AbortAll();

            if (!container.JoinAll(DisposeTimeout))
            {
                context.LogWriter[LogStreamNames.Warnings].WriteLine("Some tasks failed to abort within {0} seconds!", DisposeTimeout.TotalSeconds);
            }
        }

        private static void RecordTaskResult(Context context, Task task)
        {
            if (task.Result != null && task.Result.Exception != null)
            {
                context.LogWriter[LogStreamNames.Failures].WriteException(task.Result.Exception, "Task '{0}' failed.", task.Name);
                FailureFlag = true;
            }
        }

        private static string GetTaskName(string name, Delegate d)
        {
            return name ?? d.Method.Name;
        }

        private static bool FailureFlag
        {
            get
            {
                object value = Context.CurrentContext.GetData(FailureFlagKey);
                return value != null ? (bool)value : false;
            }
            set { Context.CurrentContext.SetData(FailureFlagKey, value); }
        }
    }
}
