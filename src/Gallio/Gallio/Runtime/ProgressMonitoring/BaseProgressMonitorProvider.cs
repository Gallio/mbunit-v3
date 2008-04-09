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


namespace Gallio.Runtime.ProgressMonitoring
{
    /// <summary>
    /// An abstract base class to assist with the implementation of conformant
    /// <see cref="IProgressMonitorProvider" /> classes.
    /// </summary>
    public abstract class BaseProgressMonitorProvider : IProgressMonitorProvider
    {
        /// <inheritdoc />
        public virtual void Run(TaskWithProgress task)
        {
            IProgressMonitorPresenter presenter = GetPresenter();
            using (ObservableProgressMonitor progressMonitor = new ObservableProgressMonitor())
            {
                presenter.Present(progressMonitor);

                progressMonitor.ThrowIfCanceled();
                task(progressMonitor);
                progressMonitor.ThrowIfCanceled();
            }
        }

        /// <summary>
        /// Gets a presenter for the progress monitor.
        /// </summary>
        /// <returns>The presenter</returns>
        protected abstract IProgressMonitorPresenter GetPresenter();
    }
}
