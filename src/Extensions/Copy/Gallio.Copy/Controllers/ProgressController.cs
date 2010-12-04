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
using System.Text;
using System.Timers;
using Gallio.Common.Policies;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Copy.Controllers
{
    internal class ProgressController : IProgressController, Handles<TaskStarted>,
        Handles<TaskCancelled>, Handles<TaskCompleted>
    {
        private readonly string queueId = ConfigurationManager.AppSettings["TaskManager.DefaultQueueId"];

        private readonly ITaskManager taskManager;
        private readonly Timer timer = new Timer { AutoReset = false };
        private ObservableProgressMonitor progressMonitor;

        private ObservableProgressMonitor ProgressMonitor
        {
            get
            {
                return progressMonitor;
            }
            set
            {
                progressMonitor = value;
                progressMonitor.Changed += (s, e) => UpdateProgress();
            }
        }

        public event EventHandler<ProgressEvent> DisplayProgressDialog;

        public Observable<string> Status { get; private set; }
        public Observable<double> TotalWork { get; private set; }
        public Observable<double> CompletedWork { get; private set; }

        public ProgressController(ITaskManager taskManager)
        {
            this.taskManager = taskManager;

            timer.Interval = TimeSpan.FromSeconds(2).TotalMilliseconds;
            timer.Elapsed += (sender, e) => EventHandlerPolicy.SafeInvoke(DisplayProgressDialog, 
                this, new ProgressEvent(ProgressMonitor));

            Status = new Observable<string>();
            TotalWork = new Observable<double>();
            CompletedWork = new Observable<double>();
        }

        public void Cancel()
        {
            if (ProgressMonitor != null && false == ProgressMonitor.IsDone)
            {
                ProgressMonitor.Cancel();
            }
            taskManager.ClearQueue(queueId);
        }

        public void Handle(TaskStarted @event)
        {
            if (@event.QueueId != queueId)
                return;

            ProgressMonitor = @event.ProgressMonitor;
            timer.Start();
        }

        public void Handle(TaskCancelled @event)
        {
            if (@event.QueueId != queueId)
                return;

            timer.Stop();
        }

        public void Handle(TaskCompleted @event)
        {
            if (@event.QueueId != queueId)
                return;

            timer.Stop();
        }

        private void UpdateProgress()
        {
            if (progressMonitor.IsDone)
            {
                Status.Value = "";
                TotalWork.Value = 0;
                CompletedWork.Value = 0;
                return;
            }

            TotalWork.Value = progressMonitor.TotalWorkUnits;
            CompletedWork.Value = progressMonitor.CompletedWorkUnits;

            Status.Value = GetCurrentTask();
        }

        private string GetCurrentTask()
        {
            var builder = new StringBuilder();
            builder.Append(progressMonitor.TaskName);

            if (!string.IsNullOrEmpty(progressMonitor.LeafSubTaskName))
            {
                builder.Append(" - ");
                builder.Append(progressMonitor.LeafSubTaskName);
            }

            if (progressMonitor.TotalWorkUnits > 0)
            {
                var progress = progressMonitor.CompletedWorkUnits / progressMonitor.TotalWorkUnits;
                builder.Append(string.Format(" ({0:P0})", progress));
            }

            return builder.ToString();
        }
    }
}
