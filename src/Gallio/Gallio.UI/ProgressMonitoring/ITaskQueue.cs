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

namespace Gallio.UI.ProgressMonitoring 
{
    ///<summary>
    /// A task queue, for use by the <see cref="TaskManager"/>.
    ///</summary>
    public interface ITaskQueue 
    {
        /// <summary>
        /// Adds a task to the specified queue.
        /// </summary>
        /// <param name="queueId">The id of the queue to use.</param>
        /// <param name="command">An <see cref="ICommand"/> to queue.</param>
        void AddTask(string queueId, ICommand command);

        /// <summary>
        /// Get the next task for the specified queue.
        /// </summary>
        /// <param name="queueId">The id of the queue to use.</param>
        /// <returns>An <see cref="ICommand"/>, or null if the queue is empty.</returns>
        ICommand GetNextTask(string queueId);

        /// <summary>
        /// Removes all tasks from the specified queue.
        /// </summary>
        /// <param name="queueId">The id of the queue to use.</param>
        void RemoveAllTasks(string queueId);
    }
}
