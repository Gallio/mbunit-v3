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
using Gallio.Icarus.Commands;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using System.Collections.Generic;
using Action=Gallio.Common.Action;

namespace Gallio.Icarus.Tests.Utilities
{
    internal class TestTaskManager : ITaskManager
    {
        public bool TaskRunning
        {
            get;
            private set;
        }

        public ProgressMonitorPresenter ProgressMonitor
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        public event EventHandler TaskStarted;
        public event EventHandler TaskCompleted;
        public event EventHandler TaskCanceled;

        private readonly List<ICommand> queue = new List<ICommand>();

        public List<ICommand> Queue
        {
            get { return queue; }
        }

        public void BackgroundTask(Action action)
        {
            action();
        }

        public void QueueTask(ICommand command)
        {
            queue.Add(command);
        }

        public void Stop()
        { }

        public void Start()
        { }

        protected void OnTaskStarted()
        {
            if (TaskStarted != null)
                TaskStarted(this, EventArgs.Empty);
        }

        protected void OnTaskCompleted()
        {
            if (TaskCompleted != null)
                TaskCompleted(this, EventArgs.Empty);
        }

        protected void OnTaskCanceled()
        {
            if (TaskCanceled != null)
                TaskCanceled(this, EventArgs.Empty);
        }

        protected void OnProgressUpdate(ProgressUpdateEventArgs e)
        {
            if (ProgressUpdate != null)
                ProgressUpdate(this, e);
        }
    }
}
