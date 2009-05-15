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
using System.Collections.Generic;
using Gallio.Common.Concurrency;
using Gallio.Common.Policies;
using Gallio.Icarus.Commands;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.ProgressMonitoring
{
    /// <summary>
    /// Runs tasks with a <see cref="ProgressMonitorPresenter" />.
    /// </summary>
    public class ProgressMonitorProvider : BaseProgressMonitorProvider
    {
        private ProgressMonitorPresenter progressMonitor;

        public ProgressMonitorPresenter ProgressMonitor
        {
            get { return progressMonitor; }
        }

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;

        /// <inheritdoc />
        public void Run(ICommand command)
        {
            var presenter = GetPresenter();
            using (var observableProgressMonitor = new ObservableProgressMonitor())
            {
                presenter.Present(observableProgressMonitor);

                observableProgressMonitor.ThrowIfCanceled();

                command.Execute(observableProgressMonitor);

                observableProgressMonitor.ThrowIfCanceled();
            }
        }

        /// <inheritdoc />
        protected override IProgressMonitorPresenter GetPresenter()
        {
            var presenter = new ProgressMonitorPresenter();
            presenter.ProgressUpdate += (sender, e) => EventHandlerPolicy.SafeInvoke(ProgressUpdate, this, e);
            progressMonitor = presenter;
            return presenter;
        }
    }
}