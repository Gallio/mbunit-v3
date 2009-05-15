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
using Gallio.Common;
using Gallio.Icarus.Commands;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using System.Collections.Generic;

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
            get { throw new System.NotImplementedException(); }
        }

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        public event EventHandler TaskStarted;
        public event EventHandler TaskCompleted;
        public event EventHandler TaskCanceled;

        public void BackgroundTask(Gallio.Common.Action action)
        {
            RunTask(action);
        }

        public void QueueTask(Gallio.Common.Action action)
        {
            RunTask(action);
        }

        private List<ICommand> queue = new List<ICommand>();

        public List<ICommand> Queue
        {
            get { return queue; }
        }

        public void QueueTask(ICommand command)
        {
            queue.Add(command);
        }

        private void RunTask(Gallio.Common.Action action)
        {
            TaskRunning = true;
            action();
            TaskRunning = true;
        }

        public void Stop()
        { }
    }
}
