// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Events;

namespace Gallio.UI.ProgressMonitoring
{
    ///<summary>
    /// Event fired when a task is started.
    ///</summary>
    public class TaskStarted : Event
    {
        ///<summary>
        /// The id of the queue.
        ///</summary>
        public string QueueId { get; private set; }
        ///<summary>
        /// A progress monitor attached to the task.
        ///</summary>
        public ObservableProgressMonitor ProgressMonitor { get; private set; }

        ///<summary>
        /// Ctor.
        ///</summary>
        ///<param name="queueId">The queue id.</param>
        ///<param name="progressMonitor">A progress monitor.</param>
        public TaskStarted(string queueId, ObservableProgressMonitor progressMonitor)
        {
            QueueId = queueId;
            ProgressMonitor = progressMonitor;
        }
    }
}
