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
using Gallio.Concurrency;
using Gallio.Icarus.Utilities;

namespace Gallio.Icarus
{
    public sealed class TaskManager : ITaskManager
    {
        private Task currentWorkerTask;
        private readonly Queue<Action> queue = new Queue<Action>();
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;

        public bool TaskRunning
        {
            get { return (currentWorkerTask != null); }
        }

        public TaskManager() : this(new UnhandledExceptionPolicy())
        { }

        public TaskManager(IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
        }

        public void QueueTask(Action action)
        {
            queue.Enqueue(action);

            if (currentWorkerTask == null)
                RunTask();
        }

        public void BackgroundTask(Action action)
        {
            ThreadPool.QueueUserWorkItem(cb => action());
        }

        private void RunTask()
        {
            Action action = queue.Dequeue();

            Task workerTask = new ThreadTask("Icarus Worker", action);
            currentWorkerTask = workerTask;

            workerTask.Terminated += delegate
            {
                if (!workerTask.IsAborted)
                {
                    if (workerTask.Result.Exception != null && !(workerTask.Result.Exception is OperationCanceledException))
                        unhandledExceptionPolicy.Report("An exception occurred in a background task.",
                            currentWorkerTask.Result.Exception);
                }

                lock (this)
                {
                    if (currentWorkerTask == workerTask)
                        currentWorkerTask = null;
                }

                if (queue.Count > 0)
                    RunTask();
            };

            workerTask.Start();
        }

        public void Stop()
        {
            queue.Clear();
        }
    }
}
