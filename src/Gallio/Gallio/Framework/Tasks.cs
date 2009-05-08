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
using System.Diagnostics;
using System.Text;
using Gallio;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Concurrency;
using Gallio.Framework;
using Gallio.Model.Logging;

namespace Gallio.Framework
{
    /// <summary>
    /// <para>
    /// The tasks class provides a mechanism for coordinating the actions of multiple
    /// tasks within a test case.
    /// </para>
    /// <para>
    /// Each task started by a test case is monitored.  When the test exits, any
    /// remaining tasks are automatically aborted and disposed.
    /// </para>
    /// </summary>
    public static class Tasks
    {
        private static readonly Key<TaskContainer> ContainerKey = new Key<TaskContainer>("Tasks.Container");
        private static readonly Key<object> FailureFlagKey = new Key<object>("Tasks.Failure");

        private static readonly TimeSpan JoinBeforeAbortTimeout = new TimeSpan(0, 0, 3);
        private static readonly TimeSpan JoinAfterAbortTimeout = new TimeSpan(0, 0, 2);

        /// <summary>
        /// Gets the task container for the current <see cref="TestContext" />.
        /// </summary>
        public static TaskContainer TaskContainer
        {
            get { return GetTaskContainer(); }
        }

        /// <summary>
        /// Adds a new task for the task manager to watch.
        /// </summary>
        /// <para>
        /// The task manager will track when the task starts and finishes, ensure
        /// that the task is aborted when the test ends, report any exception thrown
        /// by the task as a warning in the log, and include the task in the list of those
        /// to join and/or verify.
        /// </para>
        /// <param name="task">The task to watch</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="task"/> is null</exception>
        public static void WatchTask(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            GetTaskContainer().Watch(task);
        }

        /// <summary>
        /// Creates a new thread task but does not start it.
        /// </summary>
        /// <remarks>
        /// There is no need to call <see cref="WatchTask" /> on the returned task.
        /// </remarks>
        /// <param name="name">The name of the task, or null to create a new name based
        /// on the method associated with the action</param>
        /// <param name="action">The action to perform</param>
        /// <returns>The new thread task</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static ThreadTask CreateThreadTask(string name, Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            ThreadTask task = new ThreadTask(GetTaskName(name, action), action);
            WatchTask(task);
            return task;
        }

        /// <summary>
        /// Starts a new thread task.
        /// </summary>
        /// <remarks>
        /// There is no need to call <see cref="WatchTask" /> on the returned task.
        /// </remarks>
        /// <param name="name">The name of the task, or null to create a new name based
        /// on the method associated with the action</param>
        /// <param name="action">The action to perform</param>
        /// <returns>The new thread task</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static ThreadTask StartThreadTask(string name, Action action)
        {
            ThreadTask task = CreateThreadTask(name, action);
            task.Start();
            return task;
        }

        /// <summary>
        /// <para>
        /// Creates a new process task but does not start it.
        /// </para>
        /// <para>
        /// The output of the process will be logged and included as part of the test results.  It
        /// may also be examined using the <see cref="ProcessTask.ConsoleOutput" /> and
        /// <see cref="ProcessTask.ConsoleError" /> properties while the process executes and
        /// after it terminates.
        /// </para>
        /// </summary>
        /// <remarks>
        /// There is no need to call <see cref="WatchTask" /> on the returned task.
        /// </remarks>
        /// <param name="executablePath">The path of the executable executable</param>
        /// <param name="arguments">The arguments for the executable</param>
        /// <param name="workingDirectory">The working directory</param>
        /// <returns>The new thread task</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>,
        /// <paramref name="arguments"/> or <paramref name="workingDirectory"/> is null</exception>
        public static ProcessTask CreateProcessTask(string executablePath, string arguments, string workingDirectory)
        {
            if (executablePath == null)
                throw new ArgumentNullException("executablePath");
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if (workingDirectory == null)
                throw new ArgumentNullException("workingDirectory");

            ProcessTask task = new ProcessTask(executablePath, arguments, workingDirectory);
            task.CaptureConsoleOutput = true;
            task.CaptureConsoleError = true;

            ConfigureProcessTaskForLogging(task, TestLog.Default);
            WatchTask(task);
            return task;
        }

        private static void ConfigureProcessTaskForLogging(ProcessTask task, TestLogStreamWriter writer)
        {
            task.Started += delegate
            {
                writer.BeginSection(String.Format("Run Process: {0} {1}", task.ExecutablePath, task.Arguments));
                writer.WriteLine("Working Directory: {0}", task.WorkingDirectory);
                writer.BeginMarker(Marker.Monospace);
            };

            task.ConsoleOutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                    writer.WriteLine(e.Data);
            };

            task.ConsoleErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                    writer.WriteLine(e.Data);
            };

            task.Aborted += delegate
            {
                if (task.IsRunning)
                    writer.BeginSection("Abort requested.  Killing the process!").Dispose();
            };

            task.Terminated += delegate
            {
                writer.End();
                writer.WriteLine("Exit Code: {0}", task.ExitCode);
                writer.End();
            };
        }

        /// <summary>
        /// <para>
        /// Starts a new process and begins watching it.
        /// </para>
        /// <para>
        /// The output of the process will be logged and included as part of the test results.  It
        /// may also be examined using the <see cref="ProcessTask.ConsoleOutput" /> and
        /// <see cref="ProcessTask.ConsoleError" /> properties while the process executes and
        /// after it terminates.
        /// </para>
        /// </summary>
        /// <remarks>
        /// There is no need to call <see cref="WatchTask" /> on the returned task.
        /// </remarks>
        /// <param name="executablePath">The path of the executable executable</param>
        /// <param name="arguments">The arguments for the executable</param>
        /// <param name="workingDirectory">The working directory</param>
        /// <returns>The new thread task</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>,
        /// <paramref name="arguments"/> or <paramref name="workingDirectory"/> is null</exception>
        public static ProcessTask StartProcessTask(string executablePath, string arguments, string workingDirectory)
        {
            ProcessTask task = CreateProcessTask(executablePath, arguments, workingDirectory);
            task.Start();
            return task;
        }

        /// <summary>
        /// Waits for all tasks to complete or for timeout to occur.
        /// Then verifies that no failures have occurred in any of the tasks.
        /// </summary>
        /// <param name="timeout">The timeout</param>
        /// <exception cref="TestException">Thrown if some of the tasks did not complete
        /// or if any of the tasks failed</exception>
        public static void JoinAndVerify(TimeSpan timeout)
        {
            if (!GetTaskContainer().JoinAll(timeout))
                throw new TestFailedException("Some tasks did not terminate before the timeout expired.");

            Verify();
        }

        /// <summary>
        /// Verifies that no failures have occurred in any of the tasks.
        /// </summary>
        /// <exception cref="TestException">Thrown if any of the tasks failed</exception>
        public static void Verify()
        {
            if (FailureFlag)
                throw new TestFailedException("Some tasks failed.");
        }

        private static TaskContainer GetTaskContainer()
        {
            TestContext context = TestContext.CurrentContext;
            if (context == null || context.IsFinished)
                throw new InvalidOperationException("This operation cannot be performed because there is no current context.");

            lock (context)
            {
                TaskContainer container;
                if (! context.Data.TryGetValue(ContainerKey, out container))
                {
                    container = CreateContainer(context);
                    context.Data.SetValue(ContainerKey, container);
                }

                return container;
            }
        }

        private static TaskContainer CreateContainer(TestContext context)
        {
            TaskContainer container = new TaskContainer();

            context.Finishing += delegate { ReapTasks(context, container); };
            container.TaskTerminated += delegate(object sender, TaskEventArgs e) { RecordTaskResult(context, e.Task); };

            return container;
        }

        private static void ReapTasks(TestContext context, TaskContainer container)
        {
            if (!container.JoinAll(JoinBeforeAbortTimeout))
            {
                LogMessageAboutActiveTasks(context, container,
                    String.Format("Some tasks failed to complete within {0} seconds of test termination: ", JoinBeforeAbortTimeout.TotalSeconds));

                container.AbortAll();

                if (!container.JoinAll(JoinAfterAbortTimeout))
                {
                    LogMessageAboutActiveTasks(context, container,
                        String.Format("Some tasks failed to abort within {0} seconds of test termination: ", (JoinBeforeAbortTimeout + JoinAfterAbortTimeout).TotalSeconds));
                }
            }
        }

        private static void LogMessageAboutActiveTasks(TestContext context, TaskContainer container, string messagePrefix)
        {
            IList<Task> activeTasks = container.GetActiveTasks();
            if (activeTasks.Count == 0)
                return;

            StringBuilder message = new StringBuilder(messagePrefix);

            for (int i = 0; i < activeTasks.Count; i++)
            {
                if (i != 0)
                    message.Append(", ");
                message.Append(activeTasks[i].Name);
            }

            context.LogWriter.Warnings.WriteLine(message.ToString());
        }

        private static void RecordTaskResult(TestContext context, Task task)
        {
            if (task.Result != null && task.Result.Exception != null)
            {
                context.LogWriter.Warnings.WriteException(task.Result.Exception,
                    String.Format("Task '{0}' failed.", task.Name));
                FailureFlag = true;
            }
        }

        private static string GetTaskName(string name, Delegate d)
        {
            return name ?? d.Method.Name;
        }

        private static bool FailureFlag
        {
            get { return TestContext.CurrentContext.Data.HasValue(FailureFlagKey); }
            set
            {
                if (value)
                    TestContext.CurrentContext.Data.SetValue(FailureFlagKey, null);
                else
                    TestContext.CurrentContext.Data.RemoveValue(FailureFlagKey);
            }
        }
    }
}