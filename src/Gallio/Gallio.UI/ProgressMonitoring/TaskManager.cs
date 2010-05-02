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
using System.Configuration;
using System.Threading;
using Gallio.Common;

namespace Gallio.UI.ProgressMonitoring
{
    /// <inheritdoc />
    public class TaskManager : ITaskManager
    {
        private readonly ITaskQueue taskQueue;
        private readonly ITaskRunner taskRunner;
        private string defaultQueueId;

        private string DefaultQueueId
        {
            get
            {
                if (defaultQueueId == null)
                {
                    defaultQueueId = ConfigurationManager.AppSettings["TaskManager.DefaultQueueId"] ?? 
                        "Gallio.UI.TaskManager";
                }
                return defaultQueueId;
            }
        }

        ///<summary>
        /// Default constructor.
        ///</summary>
        ///<param name="taskQueue">A task queue.</param>
        ///<param name="taskRunner">A task runner.</param>
        public TaskManager(ITaskQueue taskQueue, ITaskRunner taskRunner)
        {
            this.taskQueue = taskQueue;
            this.taskRunner = taskRunner;
        }

        /// <inheritdoc />
        public void QueueTask(ICommand command)
        {
            QueueTask(DefaultQueueId, command);
        }

        /// <inheritdoc />
        public void QueueTask(string queueId, ICommand command)
        {
            taskQueue.AddTask(queueId, command);
            taskRunner.RunTask(queueId);
        }

        /// <inheritdoc />
        [Obsolete("Use a named queue instead.")]
        public void BackgroundTask(Action action)
        {
            ThreadPool.QueueUserWorkItem(cb => action());
        }

        /// <inheritdoc />
        public void ClearQueue()
        {
            ClearQueue(DefaultQueueId);
        }

        /// <inheritdoc />
        public void ClearQueue(string queueId)
        {
            taskQueue.RemoveAllTasks(queueId);
        }
    }
}
