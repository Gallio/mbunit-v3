// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Core.ProgressMonitoring
{
    /// <summary>
    /// Abstract base class for objects whose purpose is to present progress
    /// information to the user.
    /// </summary>
    public abstract class BaseProgressMonitorPresenter : IProgressMonitorPresenter
    {
        private ObservableProgressMonitor progressMonitor;

        /// <summary>
        /// Gets the attached progress monitor.
        /// </summary>
        protected ObservableProgressMonitor ProgressMonitor
        {
            get { return progressMonitor; }
        }

        /// <inheritdoc />
        public void Present(ObservableProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (this.progressMonitor != null)
                throw new InvalidOperationException("The presenter cannot be reused multiple times.");

            this.progressMonitor = progressMonitor;
            Initialize();
        }

        /// <summary>
        /// Initializes the presenter after a progress monitor has been attached.
        /// </summary>
        protected virtual void Initialize()
        {
        }
    }
}
