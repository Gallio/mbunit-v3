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
using System.Text;

using MbUnit.Core.ProgressMonitoring;
using MbUnit.Icarus.Core.CustomEventArgs;
using MbUnit.Icarus.Core.Interfaces;

namespace MbUnit.Icarus.Core.ProgressMonitoring
{
    public class StatusStripProgressMonitor : TextualProgressMonitor
    {
        private IProjectPresenter presenter;

        public StatusStripProgressMonitor(IProjectPresenter presenter)
        {
            this.presenter = presenter;
        }

        protected override void OnBeginTask(string taskName, double totalWorkUnits)
        {
            presenter.TotalWorkUnits = Convert.ToInt32(totalWorkUnits);
            base.OnBeginTask(taskName, totalWorkUnits);
        }

        /// <inheritdoc />
        protected override void UpdateDisplay()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TaskName);
            if (CurrentSubTaskName != "")
            {
                sb.Append(" - ");
                sb.Append(CurrentSubTaskName);
            }
            presenter.StatusText = sb.ToString();
            presenter.CompletedWorkUnits = Convert.ToInt32(CompletedWorkUnits);
        }

        protected override void OnDone()
        {
            base.OnDone();
            presenter.StatusText = "Ready...";
            presenter.CompletedWorkUnits = 0;
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            presenter.StatusText = "Cancelled";
        }
    }
}