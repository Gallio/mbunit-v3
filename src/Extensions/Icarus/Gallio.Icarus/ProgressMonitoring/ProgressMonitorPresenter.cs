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
using Gallio.Common.Policies;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.ProgressMonitoring
{
    public class ProgressMonitorPresenter : BaseProgressMonitorPresenter
    {
        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        protected override void Initialize()
        {
            ProgressMonitor.TaskStarting += HandleTaskStarting;
            ProgressMonitor.Changed += HandleChanged;
        }

        private void HandleTaskStarting(object sender, System.EventArgs e)
        {
            EventHandlerPolicy.SafeInvoke(ProgressUpdate, this, new ProgressUpdateEventArgs(string.Empty, 
                string.Empty, 0, ProgressMonitor.TotalWorkUnits));
        }

        /// <inheritdoc />
        private void HandleChanged(object sender, System.EventArgs e)
        {
            if (ProgressMonitor.IsCanceled)
            {
                EventHandlerPolicy.SafeInvoke(ProgressUpdate, this, new ProgressUpdateEventArgs("Operation cancelled", 
                    string.Empty, 0, 0));
            }
            else if (ProgressMonitor.IsDone)
            {
                EventHandlerPolicy.SafeInvoke(ProgressUpdate, this, new ProgressUpdateEventArgs(string.Empty, 
                    string.Empty, 0, 0));
            }
            else
            {
                EventHandlerPolicy.SafeInvoke(ProgressUpdate, this,
                    new ProgressUpdateEventArgs(ProgressMonitor.TaskName, ProgressMonitor.LeafSubTaskName,
                        ProgressMonitor.CompletedWorkUnits, ProgressMonitor.TotalWorkUnits));
            }
        }

        public void Cancel()
        {
            ProgressMonitor.Cancel();
        }
    }
}