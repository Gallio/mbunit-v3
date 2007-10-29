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
using System.Collections.Generic;

namespace Gallio.Core.ProgressMonitoring
{
    /// <summary>
    /// The tracking progress monitor is a refinement of <see cref="BaseProgressMonitor" />
    /// that remembers the current task name, status and sub-task stack.
    /// </summary>
    public abstract class TrackingProgressMonitor : BaseProgressMonitor
    {
        private string taskName = @"";
        private string status = @"";
        private Stack<string> subTaskNames;

        /// <summary>
        /// Gets the name of the task or an empty string if <see cref="IProgressMonitor.BeginTask" /> has
        /// not been called.
        /// </summary>
        public string TaskName
        {
            get { return taskName; }
        }

        /// <summary>
        /// Gets the current status message set by <see cref="IProgressMonitor.SetStatus" /> or an empty
        /// string by default.
        /// </summary>
        public string Status
        {
            get { return status; }
        }

        /// <summary>
        /// Gets the stack of sub-task names.
        /// </summary>
        public Stack<string> SubTaskNames
        {
            get
            {
                if (subTaskNames == null)
                    subTaskNames = new Stack<string>();
                return subTaskNames;
            }
        }

        /// <summary>
        /// Gets the current sub-task name, or an empty string if none.
        /// </summary>
        public string CurrentSubTaskName
        {
            get
            {
                return subTaskNames == null || subTaskNames.Count == 0 ? @"" : subTaskNames.Peek();
            }
        }

        /// <inheritdoc />
        protected override void OnBeginTask(string taskName, double totalWorkUnits)
        {
            this.taskName = taskName;
        }

        /// <inheritdoc />
        protected override void OnBeginSubTask(string subTaskName)
        {
            status = @"";
            SubTaskNames.Push(subTaskName);
        }

        /// <inheritdoc />
        protected override void OnEndSubTask()
        {
            status = @"";
            SubTaskNames.Pop();
        }

        /// <inheritdoc />
        protected override void OnSetStatus(string status)
        {
            this.status = status;
        }
    }
}