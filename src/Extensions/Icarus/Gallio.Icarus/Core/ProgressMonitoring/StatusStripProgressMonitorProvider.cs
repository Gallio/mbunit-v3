// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Text;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Core.CustomEventArgs;

namespace Gallio.Icarus.Core.ProgressMonitoring
{
    /// <summary>
    /// Runs tasks with a <see cref="StatusStripProgressMonitorPresenter" />.
    /// </summary>
    public class StatusStripProgressMonitorProvider : BaseProgressMonitorProvider
    {
        private ObservableProgressMonitor progressMonitor;

        public IProgressMonitor ProgressMonitor
        {
            get { return progressMonitor; }
        }

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        /// <inheritdoc />
        public override void Run(TaskWithProgress task)
        {
            IProgressMonitorPresenter presenter = GetPresenter();
            using (progressMonitor = new ObservableProgressMonitor())
            {
                presenter.Present(progressMonitor);

                progressMonitor.ThrowIfCanceled();
                task(progressMonitor);
                progressMonitor.ThrowIfCanceled();
            }
        }

        /// <inheritdoc />
        protected override IProgressMonitorPresenter GetPresenter()
        {
            StatusStripProgressMonitorPresenter presenter = new StatusStripProgressMonitorPresenter();
            presenter.ProgressUpdate += delegate(object sender, ProgressUpdateEventArgs e) { if (ProgressUpdate != null) ProgressUpdate(this, e); };
            return presenter;
        }
    }
}