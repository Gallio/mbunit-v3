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
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Icarus.ProgressMonitoring
{
    /// <summary>
    /// Runs tasks with a <see cref="ProgressMonitorPresenter" />.
    /// </summary>
    public class ProgressMonitorProvider : BaseProgressMonitorProvider
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
            ProgressMonitorPresenter presenter = new ProgressMonitorPresenter();
            presenter.ProgressUpdate += ((sender, e) => EventHandlerUtils.SafeInvoke(ProgressUpdate, this, e));
            return presenter;
        }
    }
}