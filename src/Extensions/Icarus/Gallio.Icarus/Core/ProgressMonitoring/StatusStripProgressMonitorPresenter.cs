// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Icarus.Core.Interfaces;
using Gallio.Hosting.ProgressMonitoring;

namespace Gallio.Icarus.Core.ProgressMonitoring
{
    public class StatusStripProgressMonitorPresenter : BaseProgressMonitorPresenter
    {
        private readonly IProjectPresenter presenter;

        public StatusStripProgressMonitorPresenter(IProjectPresenter presenter)
        {
            this.presenter = presenter;
        }

        protected override void Initialize()
        {
            ProgressMonitor.TaskStarting += HandleTaskStarting;
            ProgressMonitor.TaskFinished += HandleTaskFinished;
            ProgressMonitor.Canceled += HandleCanceled;
            ProgressMonitor.Changed += HandleChanged;
        }

        private void HandleTaskStarting(object sender, EventArgs e)
        {
            presenter.TotalWorkUnits = Convert.ToInt32(ProgressMonitor.TotalWorkUnits);
        }

        /// <inheritdoc />
        private void HandleChanged(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ProgressMonitor.TaskName);

            string currentSubTaskName = ProgressMonitor.LeafSubTaskName;
            if (currentSubTaskName != "")
            {
                sb.Append(" - ");
                sb.Append(currentSubTaskName);
            }

            presenter.StatusText = sb.ToString();
            presenter.CompletedWorkUnits = Convert.ToInt32(ProgressMonitor.CompletedWorkUnits);
        }

        private void HandleTaskFinished(object sender, EventArgs e)
        {
            presenter.CompletedWorkUnits = 0;
        }

        private void HandleCanceled(object sender, EventArgs e)
        {
            presenter.StatusText = "Tests cancelled";
        }
    }
}