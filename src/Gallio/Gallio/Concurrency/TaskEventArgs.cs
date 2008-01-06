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

namespace Gallio.Concurrency
{
    /// <summary>
    /// A task-related event.
    /// </summary>
    public class TaskEventArgs : EventArgs
    {
        private readonly Task task;

        /// <summary>
        /// Creates event arguments for a task-related event.
        /// </summary>
        /// <param name="task">The task that the event is about</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="task"/> is null</exception>
        public TaskEventArgs(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            this.task = task;
        }

        /// <summary>
        /// Gets the task that the event is about.
        /// </summary>
        public Task Task
        {
            get { return task; }
        }
    }
}
