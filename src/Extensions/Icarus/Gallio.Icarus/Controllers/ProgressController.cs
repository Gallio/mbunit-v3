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
using System.Timers;
using Gallio.Common.Policies;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Controllers
{
    internal class ProgressController : IProgressController
    {
        private readonly ITaskManager taskManager;
        private readonly Timer timer = new Timer { AutoReset = false };

        public event EventHandler DisplayProgressDialog;

        public ProgressController(ITaskManager taskManager, IOptionsController optionsController)
        {
            this.taskManager = taskManager;

            timer.Interval = TimeSpan.FromSeconds(2).TotalMilliseconds;
            timer.Elapsed += (sender, e) =>
            {
                if (!optionsController.ShowProgressDialogs)
                    return;

                // HACK: we don't want to display the progress dialog when running tests
                // but relying on the task name is not ideal
                if (taskManager.ProgressMonitor.TaskName == "Running tests")
                    return;

                EventHandlerPolicy.SafeInvoke(DisplayProgressDialog, this,
                    System.EventArgs.Empty);
            };

            taskManager.TaskStarted += (sender, e) => timer.Start();
            taskManager.TaskCanceled += (sender, e) => timer.Stop();
            taskManager.TaskCompleted += (sender, e) => timer.Stop();
        }

        public void Cancel()
        {
            // cancel any running work
            if (taskManager.ProgressMonitor != null)
                taskManager.ProgressMonitor.Cancel();

            // remove anything else in the queue
            taskManager.ClearQueue();
        }
    }
}
