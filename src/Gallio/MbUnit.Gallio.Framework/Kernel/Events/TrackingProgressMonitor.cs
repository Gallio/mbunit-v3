using System;
using System.Collections.Generic;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// The tracking progress monitor is a refinement of <see cref="BaseProgressMonitor" />
    /// that remembers the current task name, status and sub-task stack.
    /// </summary>
    public abstract class TrackingProgressMonitor : BaseProgressMonitor
    {
        private string taskName = "";
        private string status = "";
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
                return subTaskNames == null || subTaskNames.Count == 0 ? "" : subTaskNames.Peek();
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
            status = "";
            SubTaskNames.Push(subTaskName);
        }

        /// <inheritdoc />
        protected override void OnEndSubTask()
        {
            status = "";
            SubTaskNames.Pop();
        }

        /// <inheritdoc />
        protected override void OnSetStatus(string status)
        {
            this.status = status;
        }
    }
}
