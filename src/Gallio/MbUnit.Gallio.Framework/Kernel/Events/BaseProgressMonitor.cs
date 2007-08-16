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
using System.Threading;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// A base progress monitor tracks validates arguments and tracks the state
    /// of the progress monitor but it does not implement any behavior in response
    /// to the notifications received.  Subclasses should provide this behavior
    /// such as by integration with a user interface control.
    /// </summary>
    /// <seealso cref="IProgressMonitor"/> for important thread-safety and usage remarks.
    [Serializable]
    public abstract class BaseProgressMonitor : IProgressMonitor
    {
        private event EventHandler canceledHandler;

        private double totalWorkUnits = double.NaN;
        private double completedWorkUnits = 0;
		
        private bool isCanceled;
        private bool isRunning;
        private bool isDone;
        private int subTaskNestingDepth;

        /// <inheritdoc />
        public event EventHandler Canceled
        {
            add
            {
                // There's no point adding the handler to the chain if the operation
                // has already been canceled.  Per IProgressMonitor's spec, we automatically
                // invoke the handler now.
                lock (this)
                {
                    if (! isCanceled)
                    {
                        canceledHandler += value;
                        return;
                    }
                }

                value(this, EventArgs.Empty);
            }
            remove
            {
                lock (this)
                    canceledHandler -= value;
            }
        }

        /// <inheritdoc />
        public bool IsCanceled
        {
            get { return isCanceled; }
        }

        /// <summary>
        /// Returns true if <see cref="BeginTask" /> has been called and <see cref="Done" />
        /// has not yet also been called.
        /// </summary>
        /// <remarks>
        /// This property is not affected by cancellation.
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
        /// Gets the current sub-task nesting depth which is the number of times
        /// <see cref="BeginSubTask" /> has been called without a matching <see cref="EndSubTask" />.
        /// </summary>
        public int SubTaskNestingDepth
        {
            get { return subTaskNestingDepth; }
        }

        /// <inheritdoc />
        public void BeginTask(string taskName, double totalWorkUnits)
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

            OnBeginTask(taskName, totalWorkUnits);
        }

        /// <inheritdoc />
        public void SetStatus(string status)
        {
            if (status == null)
                throw new ArgumentNullException("status");

            if (!isRunning)
                throw new InvalidOperationException("Status cannot be set unless the task is running.");

            OnSetStatus(status);
        }

        /// <inheritdoc />
        public void Worked(double workUnits)
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
        public void Cancel()
        {
            if (NotifyCanceled())
                OnCancel();
        }

        /// <inheritdoc />
        public void Done()
        {
            while (subTaskNestingDepth > 0)
                EndSubTask();

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
        public void BeginSubTask(string subTaskName)
        {
            if (subTaskName == null)
                throw new ArgumentNullException("subTaskName");

            if (!isRunning)
                throw new InvalidOperationException("Subtasks may not be started unless the task is running.");

            subTaskNestingDepth += 1;
            OnBeginSubTask(subTaskName);
        }

        /// <inheritdoc />
        public void EndSubTask()
        {
            if (!isRunning)
                throw new InvalidOperationException("Subtasks may not be ended unless the task is running.");

            if (subTaskNestingDepth <= 0)
                throw new InvalidOperationException("There is no current subtask.");

            subTaskNestingDepth -= 1;
            OnEndSubTask();
        }

        /// <inheritdoc />
        public void ThrowIfCanceled()
        {
            if (isCanceled)
                throw new OperationCanceledException("The user canceled the operation.");
        }

        /// <summary>
        /// Notifies that the task has actually been canceled.
        /// If this is the first time <see cref="NotifyCanceled" /> has been called,
        /// sets <see cref="IsCanceled" /> to true and fires the <see cref="Canceled" /> event.
        /// Otherwise does nothing.
        /// </summary>
        /// <remarks>
        /// This is the ONLY method of the progress monitor that is allowed to
        /// be called concurrently from multiple threads.  It needs to be this
        /// way so that cancellation can be initiated by either participant in the
        /// progress monitoring dialog.
        /// </remarks>
        /// <returns>True if cancelation has just occurred, false if no
        /// state change was performed</returns>
        protected bool NotifyCanceled()
        {
            EventHandler currentCanceledHandler;

            lock (this)
            {
                if (isCanceled)
                    return false;

                isCanceled = true;
                currentCanceledHandler = canceledHandler;

                // We will never call these handlers again so there's no point
                // keeping them around anymore.
                canceledHandler = null;
            }

            if (currentCanceledHandler != null)
                currentCanceledHandler(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Called after <see cref="BeginTask" /> performs its updates.
        /// </summary>
        /// <param name="taskName">The task name</param>
        /// <param name="totalWorkUnits">The total number of work units</param>
        protected virtual void OnBeginTask(string taskName, double totalWorkUnits)
        {
        }
		
        /// <summary>
        /// Called when <see cref="Worked" /> performs its updates.
        /// </summary>
        protected virtual void OnWorked(double workUnits)
        {
        }
		
        /// <summary>
        /// Called when <see cref="Cancel" /> performs its updates.
        /// </summary>
        /// <remarks>
        /// This method is not called by <see cref="NotifyCanceled" />.
        /// </remarks>
        protected virtual void OnCancel()
        {
        }
		
        /// <summary>
        /// Called when <see cref="Done" /> performs its updates.
        /// </summary>
        protected virtual void OnDone()
        {
        }
		
        /// <summary>
        /// Called when <see cref="SetStatus" /> performs its updates.
        /// </summary>
        /// <param name="status">The status message</param>
        protected virtual void OnSetStatus(string status)
        {
        }

        /// <summary>
        /// Called when <see cref="OnBeginSubTask" /> performs its updates.
        /// </summary>
        /// <param name="subTaskName">The sub-task name</param>
        protected virtual void OnBeginSubTask(string subTaskName)
        {
        }

        /// <summary>
        /// Called when <see cref="OnEndSubTask" /> performs its updates.
        /// </summary>
        protected virtual void OnEndSubTask()
        {
        }

        void IDisposable.Dispose()
        {
            Done();
        }
    }
}