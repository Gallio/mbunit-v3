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
using Gallio.Utilities;

namespace Gallio.Hosting.ProgressMonitoring
{
    /// <summary>
    /// An observable progress monitor tracks validates arguments and tracks the state
    /// of the progress monitor but it does not implement any of its own behavior in response
    /// to the notifications received.  Instead, it is intended to be observed by a presenter that
    /// translates state change events into changes of the view.
    /// </summary>
    /// <seealso cref="IProgressMonitor"/> for important thread-safety and usage remarks.
    [Serializable]
    public class ObservableProgressMonitor : CancelableProgressMonitor
    {
        private double totalWorkUnits = double.NaN;
        private double completedWorkUnits = 0;
		
        private bool isRunning;
        private bool isDone;
        private ObservableProgressMonitor child;

        private string taskName = @"";
        private string status = @"";

        /// <summary>
        /// Adds or removes an event handler that is called whenever the state of the progress
        /// monitor changes in any way.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Adds or removes an event handler that is called when the task is starting.
        /// </summary>
        public event EventHandler TaskStarting;

        /// <summary>
        /// Adds or removes an event handler that is called when the task is finished.
        /// </summary>
        public event EventHandler TaskFinished;

        /// <summary>
        /// Adds or removes an event handler that is called whenever a new sub-progress monitor
        /// is created so that the observer can attach its event handlers.
        /// </summary>
        public event EventHandler<SubProgressMonitorCreatedEventArgs> SubProgressMonitorCreated;

        /// <summary>
        /// Returns true if <see cref="BeginTask" /> has been called and <see cref="Done" />
        /// has not yet also been called.
        /// </summary>
        /// <remarks>
        /// This property is not affected by cancelation.
        /// </remarks>
        public bool IsRunning
        {
            get { return isRunning; }
        }

        /// <summary>
        /// Returns true if <see cref="Done" /> has been called.
        /// </summary>
        /// <remarks>
        /// This property is not affected by cancellation.
        /// </remarks>
        public bool IsDone
        {
            get { return isDone; }
        }
		
        /// <summary>
        /// Gets the total number of work units to perform, or
        /// NaN to indicate that an indeterminate amount of work is
        /// to be performed.
        /// </summary>
        /// <remarks>
        /// Returns NaN if <see cref="BeginTask" /> has not been called.
        /// </remarks>
        public double TotalWorkUnits
        {
            get { return totalWorkUnits; }
        }
		
        /// <summary>
        /// Gets the number of work units completed so far.  It is the sum
        /// of all values passed to the <see cref="Worked" /> method while
        /// the task has been running.  This value is never NaN because at each
        /// step a finite amount of work must be recorded.
        /// </summary>
        /// <remarks>
        /// Returns 0 if <see cref="BeginTask" /> and <see cref="Worked" /> have
        /// not been called.
        /// </remarks>
        public double CompletedWorkUnits
        {
            get { return completedWorkUnits; }
        }
		
        /// <summary>
        /// Gets the number of remaining work units to perform, or
        /// NaN to indicate that an indeterminate amount of work remains
        /// to be performed because <see cref="TotalWorkUnits" /> is NaN
        /// and the operation is not done.
        /// </summary>
        /// <remarks>
        /// Returns NaN if <see cref="BeginTask" /> has not been called.
        /// </remarks>
        public double RemainingWorkUnits
        {
            get
            {
                // If total work units is NaN then the result will also be NaN unless done.
                return isDone ? 0 : totalWorkUnits - completedWorkUnits;
            }
        }

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
        /// Gets the active child sub-progress monitor, or null if none.
        /// </summary>
        public ObservableProgressMonitor Child
        {
            get { return child; }
        }

        /// <summary>
        /// Gets the currently active leaf sub-progress monitor, or this one if there are
        /// no sub-progress monitors.
        /// </summary>
        public ObservableProgressMonitor Leaf
        {
            get
            {
                ObservableProgressMonitor leaf = this;
                while (leaf.Child != null)
                    leaf = leaf.Child;

                return leaf;
            }
        }

        /// <summary>
        /// Gets the name of the most deeply nested sub-task, or an empty string if
        /// there are no sub-progress monitors.
        /// </summary>
        public string LeafSubTaskName
        {
            get
            {
                ObservableProgressMonitor leaf = Leaf;
                return leaf == this ? "" : leaf.TaskName;
            }
        }

        /// <inheritdoc />
        public override void BeginTask(string taskName, double totalWorkUnits)
        {
            if (taskName == null)
                throw new ArgumentNullException("taskName");
            if (double.IsInfinity(totalWorkUnits) || totalWorkUnits <= 0.0)
                throw new ArgumentOutOfRangeException("totalWorkUnits", totalWorkUnits, "Total work units must be finite and positive or NaN.");

            if (isRunning)
                throw new InvalidOperationException("Task may not be started twice.");

            if (isDone)
                throw new InvalidOperationException("Task may not be restarted after it completes.");

            isRunning = true;
            this.totalWorkUnits = totalWorkUnits;
            this.taskName = taskName;

            OnBeginTask(taskName, totalWorkUnits);
        }

        /// <inheritdoc />
        public override void SetStatus(string status)
        {
            if (status == null)
                throw new ArgumentNullException("status");

            if (!isRunning)
                throw new InvalidOperationException("Status cannot be set unless the task is running.");

            if (this.status != status)
            {
                this.status = status;

                OnSetStatus(status);
            }
        }

        /// <inheritdoc />
        public override void Worked(double workUnits)
        {
            if (double.IsInfinity(workUnits) || double.IsNaN(workUnits) || workUnits < 0.0)
                throw new ArgumentOutOfRangeException("workUnits", workUnits, "Work units must be finite and non-negative.");

            if (!isRunning)
                throw new InvalidOperationException("Work may not be performed unless the task is running.");

            double newWorkUnits = completedWorkUnits + workUnits;
            if (!double.IsNaN(totalWorkUnits) && newWorkUnits > totalWorkUnits)
            {
                newWorkUnits = totalWorkUnits;
                workUnits = newWorkUnits - completedWorkUnits;
            }

            if (completedWorkUnits != newWorkUnits)
            {
                completedWorkUnits = newWorkUnits;

                OnWorked(workUnits);
            }
        }

        /// <inheritdoc />
        public override void Done()
        {
            if (child != null)
                child.Done();

            if (!isDone)
            {
                isRunning = false;
                isDone = true;

                if (!double.IsNaN(totalWorkUnits) && completedWorkUnits != totalWorkUnits)
                {
                    double workUnits = totalWorkUnits - completedWorkUnits;
                    completedWorkUnits = totalWorkUnits;

                    OnWorked(workUnits);
                }

                OnDone();
            }
        }

        /// <inheritdoc />
        public override IProgressMonitor CreateSubProgressMonitor(double parentWorkUnits)
        {
            ObservableProgressMonitor subProgressMonitor = new SubProgressMonitor(this, parentWorkUnits);
            OnSubProgressMonitorCreated(subProgressMonitor);

            return subProgressMonitor;
        }

        /// <summary>
        /// Called after <see cref="BeginTask" /> performs its updates.
        /// </summary>
        /// <param name="taskName">The task name</param>
        /// <param name="totalWorkUnits">The total number of work units</param>
        protected virtual void OnBeginTask(string taskName, double totalWorkUnits)
        {
            EventHandlerUtils.SafeInvoke(TaskStarting, this, EventArgs.Empty);

            OnChange();
        }

        /// <summary>
        /// Called when <see cref="Worked" /> performs its updates.
        /// </summary>
        protected virtual void OnWorked(double workUnits)
        {
            OnChange();
        }

        /// <summary>
        /// Called when <see cref="Done" /> performs its updates.
        /// </summary>
        protected virtual void OnDone()
        {
            OnChange();

            EventHandlerUtils.SafeInvoke(TaskFinished, this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when <see cref="SetStatus" /> performs its updates.
        /// </summary>
        /// <param name="status">The status message</param>
        protected virtual void OnSetStatus(string status)
        {
            OnChange();
        }

        /// <summary>
        /// Called when the active sub-task <see cref="BeginSubTask" /> performs its updates.
        /// </summary>
        /// <param name="subProgressMonitor">The sub-task's progress monitor</param>
        protected virtual void OnBeginSubTask(ObservableProgressMonitor subProgressMonitor)
        {
            OnChange();
        }

        /// <summary>
        /// Called when <see cref="EndSubTask" /> performs its updates.
        /// </summary>
        /// <param name="subProgressMonitor">The sub-task's progress monitor</param>
        protected virtual void OnEndSubTask(ObservableProgressMonitor subProgressMonitor)
        {
            OnChange();
        }

        /// <summary>
        /// Called when a state change occurs.
        /// </summary>
        protected virtual void OnChange()
        {
            EventHandlerUtils.SafeInvoke(Changed, this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when a new progress monitor is created.
        /// </summary>
        protected virtual void OnSubProgressMonitorCreated(ObservableProgressMonitor subProgressMonitor)
        {
            EventHandlerUtils.SafeInvoke(SubProgressMonitorCreated, this, new SubProgressMonitorCreatedEventArgs(subProgressMonitor));
        }

        /// <summary>
        /// Begins a sub-task in a sub-progress monitor.
        /// </summary>
        /// <param name="subProgressMonitor">The sub-progress monitor</param>
        protected void BeginSubTask(ObservableProgressMonitor subProgressMonitor)
        {
            if (subProgressMonitor == null)
                throw new ArgumentNullException("subProgressMonitor");

            if (!isRunning)
                throw new InvalidOperationException("Subtasks may not be started unless the task is running.");

            if (child != null)
                throw new InvalidOperationException("There is already an active sub-task.");

            child = subProgressMonitor;

            OnBeginSubTask(subProgressMonitor);
        }

        /// <summary>
        /// Ends a sub-task in a sub-progress monitor.
        /// </summary>
        /// <param name="subProgressMonitor">The sub-progress monitor</param>
        protected void EndSubTask(ObservableProgressMonitor subProgressMonitor)
        {
            if (subProgressMonitor == null)
                throw new ArgumentNullException("subProgressMonitor");

            if (!isRunning)
                throw new InvalidOperationException("Subtasks may not be ended unless the task is running.");

            if (child != subProgressMonitor)
                throw new InvalidOperationException("The sub-progress monitor does not represent the current sub-task.");

            child = null;

            OnEndSubTask(subProgressMonitor);
        }

        /// <summary>
        /// A standard sub-progress monitor implementation that notifies its parent as work is performed.
        /// </summary>
        protected class SubProgressMonitor : ObservableProgressMonitor
        {
            private readonly ObservableProgressMonitor parent;
            private readonly double parentWorkUnits;
            private bool beganTask;

            /// <summary>
            /// Creates a sub-progress monitor.
            /// </summary>
            /// <param name="parent">The parent progress monitor</param>
            /// <param name="parentWorkUnits">The total number of work units of the parent task
            /// that are to be represented by the sub-task.</param>
            public SubProgressMonitor(ObservableProgressMonitor parent, double parentWorkUnits)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");
                if (double.IsInfinity(parentWorkUnits) || double.IsNaN(parentWorkUnits) || parentWorkUnits < 0.0)
                    throw new ArgumentOutOfRangeException("parentWorkUnits", parentWorkUnits, "Parent work units must be finite and non-negative.");

                this.parent = parent;
                this.parentWorkUnits = parentWorkUnits;
            }

            /// <inheritdoc />
            protected override void OnBeginTask(string taskName, double totalWorkUnits)
            {
                parent.BeginSubTask(this);
                beganTask = true;

                parent.Canceled += parent_Canceled;
            }

            /// <inheritdoc />
            protected override void OnWorked(double workUnits)
            {
                if (!double.IsNaN(TotalWorkUnits))
                {
                    parent.Worked(workUnits * parentWorkUnits / TotalWorkUnits);
                }
            }

            /// <inheritdoc />
            protected override void OnCancel()
            {
                parent.Cancel();
            }

            /// <inheritdoc />
            protected override void OnDone()
            {
                if (double.IsNaN(TotalWorkUnits))
                {
                    parent.Worked(parentWorkUnits);
                }

                if (beganTask)
                {
                    parent.EndSubTask(this);

                    parent.Canceled -= parent_Canceled;
                }
            }

            private void parent_Canceled(object sender, EventArgs e)
            {
                NotifyCanceled();
            }
        }
    }
}