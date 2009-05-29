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
using System.Threading;
using Gallio.Common;
using Gallio.Common.Concurrency;
using Gallio.Icarus.Utilities;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.Commands;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Common.Policies;

namespace Gallio.Icarus
{
    public sealed class TaskManager : ITaskManager
    {
        private Task currentWorkerTask;
        private readonly Queue<ICommand> queue = new Queue<ICommand>();
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private readonly ProgressMonitorProvider progressMonitorProvider = new ProgressMonitorProvider();
        private bool canExecute;

        public ProgressMonitorPresenter ProgressMonitor
        {
            get { return progressMonitorProvider.ProgressMonitor; }
        }

        public bool TaskRunning
        {
            get { return (currentWorkerTask != null); }
        }

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        public event EventHandler TaskStarted;
        public event EventHandler TaskCompleted;
        public event EventHandler TaskCanceled;

        public TaskManager(IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;

            progressMonitorProvider.ProgressUpdate += (sender, e) => EventHandlerPolicy.SafeInvoke(ProgressUpdate, this, e);
        }

        public void QueueTask(ICommand command)
        {
            if (!canExecute)
                throw new Exception("TaskManager is stopped.");

            queue.Enqueue(command);

            if (currentWorkerTask == null)
                RunTask();
        }

        public void BackgroundTask(Action action)
        {
            if (!canExecute)
                throw new Exception("TaskManager is stopped.");

            ThreadPool.QueueUserWorkItem(cb => action());
        }

        private void RunTask()
        {
            if (queue.Count == 0)
                return;

            var command = queue.Dequeue();

            var workerTask = new ThreadTask("Icarus Worker", 
                () => progressMonitorProvider.Run(command.Execute));

            currentWorkerTask = workerTask;

            workerTask.Terminated += delegate
            {
                if (!workerTask.IsAborted)
                {
                    if (workerTask.Result.Exception != null)
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

            // don't display progress dialog when running tests
            if (!(command is RunTestsCommand))
                EventHandlerPolicy.SafeInvoke(TaskStarted, this, EventArgs.Empty);
        }

        public void Start()
        {
            canExecute = true;
            RunTask();
        }

        public void Stop()
        {
            canExecute = false;
            queue.Clear();
        }
    }
}
