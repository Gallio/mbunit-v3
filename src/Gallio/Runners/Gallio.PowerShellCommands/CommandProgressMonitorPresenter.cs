// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Management.Automation;
using Gallio.Core.ProgressMonitoring;

namespace Gallio.PowerShellCommands
{
    /// <exclude />
    internal class CommandProgressMonitorPresenter : BaseProgressMonitorPresenter
    {
        private readonly BaseCommand cmdlet;
        private readonly int parentActivityId;
        private readonly int activityId;

        public CommandProgressMonitorPresenter(BaseCommand cmdlet, int parentActivityId)
        {
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            this.cmdlet = cmdlet;
            this.parentActivityId = parentActivityId;

            activityId = parentActivityId + 1;
        }

        protected override void Initialize()
        {
            ProgressMonitor.TaskFinished += HandleTaskFinished;
            ProgressMonitor.Changed += HandleChanged;
            ProgressMonitor.SubProgressMonitorCreated += HandleSubProgressMonitorCreated;

            cmdlet.StopRequested += HandleStopRequested;

            if (cmdlet.Stopping)
                ProgressMonitor.Cancel();
        }

        private void HandleSubProgressMonitorCreated(object sender, SubProgressMonitorCreatedEventArgs e)
        {
            new CommandProgressMonitorPresenter(cmdlet, activityId).Present(e.SubProgressMonitor); 
        }

        private void HandleTaskFinished(object sender, EventArgs e)
        {
            cmdlet.StopRequested -= HandleStopRequested;
        }

        private void HandleChanged(object sender, EventArgs e)
        {
            string status = ProgressMonitor.Status;
            if (status.Length == 0)
                status = @" "; // workaround Cmdlet API constraints.

            ProgressRecord progressRecord = new ProgressRecord(activityId, ProgressMonitor.TaskName, status);
            progressRecord.RecordType = ProgressMonitor.IsRunning ? ProgressRecordType.Processing : ProgressRecordType.Completed;
            progressRecord.ParentActivityId = parentActivityId;

            if (! double.IsNaN(ProgressMonitor.RemainingWorkUnits))
                progressRecord.PercentComplete = (int)Math.Ceiling((ProgressMonitor.TotalWorkUnits - ProgressMonitor.RemainingWorkUnits) * 100 / ProgressMonitor.TotalWorkUnits);

            cmdlet.PostMessage(delegate
            {
                cmdlet.WriteProgress(progressRecord);
            });
        }

        private void HandleStopRequested(object sender, EventArgs e)
        {
            ProgressMonitor.Cancel();
        }
    }
}
