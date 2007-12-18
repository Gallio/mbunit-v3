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
    internal class CommandProgressMonitor : TextualProgressMonitor
    {
        private readonly BaseCommand cmdlet;

        public CommandProgressMonitor(BaseCommand cmdlet)
        {
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            this.cmdlet = cmdlet;
        }

        protected override void OnBeginTask(string taskName, double totalWorkUnits)
        {
            base.OnBeginTask(taskName, totalWorkUnits);

            cmdlet.StopRequested += StopRequested;

            if (cmdlet.Stopping)
                Cancel();
        }

        protected override void OnDone()
        {
            base.OnDone();

            cmdlet.StopRequested -= StopRequested;
        }

        protected override void UpdateDisplay()
        {
            // FIXME: Handling subtasks will require API changes to IProgressMonitor to enable
            //        the progress monitor to receive progress events for all subtasks.
            //        The simplest such change would be to add a CreateSubProgressMonitor method
            //        such that the progress monitor may have control over how sub progress monitors
            //        are created.
            ProgressRecord progressRecord = new ProgressRecord(0, TaskName, Status.Length == 0 ? " " : Status);
            progressRecord.RecordType = IsRunning ? ProgressRecordType.Processing : ProgressRecordType.Completed;

            if (! double.IsNaN(RemainingWorkUnits))
                progressRecord.PercentComplete = (int) Math.Ceiling((TotalWorkUnits - RemainingWorkUnits) * 100 / TotalWorkUnits);

            cmdlet.PostMessage(delegate
            {
                cmdlet.WriteProgress(progressRecord);
            });
        }

        private void StopRequested(object sender, EventArgs e)
        {
            Cancel();
        }
    }
}
