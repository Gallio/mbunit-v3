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

namespace Gallio.Core.ProgressMonitoring
{
    /// <summary>
    /// Provides a reference to the newly created sub-progress monitor.
    /// </summary>
    public class SubProgressMonitorCreatedEventArgs : EventArgs
    {
        private readonly ObservableProgressMonitor subProgressMonitor;

        /// <summary>
        /// Creates an event object.
        /// </summary>
        /// <param name="subProgressMonitor">The new sub-progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="subProgressMonitor"/> is null</exception>
        public SubProgressMonitorCreatedEventArgs(ObservableProgressMonitor subProgressMonitor)
        {
            if (subProgressMonitor == null)
                throw new ArgumentNullException("subProgressMonitor");

            this.subProgressMonitor = subProgressMonitor;
        }

        /// <summary>
        /// Gets the newly created sub-progress monitor.
        /// </summary>
        public ObservableProgressMonitor SubProgressMonitor
        {
            get { return subProgressMonitor; }
        }
    }
}
