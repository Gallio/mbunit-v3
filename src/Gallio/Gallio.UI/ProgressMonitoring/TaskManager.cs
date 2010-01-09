// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Threading;
using Gallio.Common;
using Gallio.Common.Concurrency;
using Gallio.Common.Policies;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;

namespace Gallio.UI.ProgressMonitoring
{
    /// <inheritdoc />
    public class TaskManager : ITaskManager
    {
        private Task currentWorkerTask;
        private readonly Queue<ICommand> queue = new Queue<ICommand>();
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private ObservableProgressMonitor progressMonitor;

        /// <inheritdoc />
        public event EventHandler ProgressUpdate;

        /// <inheritdoc />
        public ObservableProgressMonitor ProgressMonitor
        {
            get { return progressMonitor; }
            private set
            {
                progressMonitor = value;
                progressMonitor.Changed += (sender, e) => 
                    EventHandlerPolicy.SafeInvoke(ProgressUpdate, this, EventArgs.Empty);
            }
        }
        /// <inheritdoc />
        public bool TaskRunning
        {
            get { return (currentWorkerTask != null); }
        }

        /// <inheritdoc />
        public event EventHandler TaskStarted;
        /// <inheritdoc />
        public event EventHandler TaskCompleted;
        /// <inheritdoc />
        public event EventHandler TaskCanceled;

        ///<summary>
        /// Default constructor.
        ///</summary>
        ///<param name="unhandledExceptionPolicy">An unhandled exception policy.</param>
        public TaskManager(IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
            ProgressMonitor = new ObservableProgressMonitor();
        }

        /// <inheritdoc />
        public void QueueTask(ICommand command)
        {
            queue.Enqueue(command);

            if (currentWorkerTask == null)
                RunTask();
        }

        /// <inheritdoc />
        public void BackgroundTask(Action action)
        {
            ThreadPool.QueueUserWorkItem(cb => action());
        }

        private void RunTask()
        {
            if (queue.Count == 0)
                return;

            var command = queue.Dequeue();

            var workerTask = new ThreadTask("Icarus Worker", () =>
            {
                ProgressMonitor = new ObservableProgressMonitor();
                command.Execute(ProgressMonitor);
            });

            currentWorkerTask = workerTask;

            workerTask.Terminated += delegate
            {
                if (!workerTask.IsAborted)
                {
                    if (! workerTask.Result.HasValue)
                    {
                        if (workerTask.Result.Exception is OperationCanceledException)
                        {
                            EventHandlerPolicy.SafeInvoke(TaskCanceled, this, EventArgs.Empty);
                        }
                        else
                        {
                            unhandledExceptionPolicy.Report("An exception occurred while running a task.", 
                                currentWorkerTask.Result.Exception);
                        }
                    }
                }

                lock (this)
                {
                    if (currentWorkerTask == workerTask)
                        currentWorkerTask = null;
                }

                EventHandlerPolicy.SafeInvoke(TaskCompleted, this, EventArgs.Empty);

                // if there's more to do, do it
                if (queue.Count > 0)
                    RunTask();
            };

            workerTask.Start();

            EventHandlerPolicy.SafeInvoke(TaskStarted, this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void ClearQueue()
        {
            queue.Clear();
        }
    }
}
