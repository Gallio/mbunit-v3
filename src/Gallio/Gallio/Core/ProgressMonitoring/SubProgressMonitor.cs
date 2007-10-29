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
    /// A sub-progress monitor represents a portion of the work to be
    /// performed by a parent progress monitor as part of a sub-task.
    /// It allows long-running
    /// operations to be composed into longer sequences that each
    /// contribute a predetermined portion of the total work.
    /// As the child performes work its parent is notified of progress
    /// in proportion to the number  of work units it represents.
    /// Likewise the parent is notified
    /// of cancelation if the child is canceled and vice-versa.
    /// </summary>
    /// <seealso cref="IProgressMonitor"/> for important thread-safety and usage remarks.
    public class SubProgressMonitor : BaseProgressMonitor
    {
        private IProgressMonitor parent;
        private double parentWorkUnits;
        private bool beganTask;
		
        /// <summary>
        /// Creates a sub-progress monitor that represents a given number of
        /// work-units as a sub-task of the parent progress monitor.
        /// </summary>
        /// <remarks>
        /// It it still necessary to call <see cref="IProgressMonitor.BeginTask" /> on the
        /// sub-progress monitor to begin processing the sub-task.
        /// </remarks>
        /// <param name="parent">The parent progress monitor</param>
        /// <param name="parentWorkUnits">The total number of work units of the parent progress
        /// monitor that are to be represented by this instance.  When the child monitor
        /// completes, this much work will have been performed on the parent.  Must
        /// be a finite value greater than or equal to 0.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="parentWorkUnits"/> is not valid</exception>
        public SubProgressMonitor(IProgressMonitor parent, double parentWorkUnits)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (double.IsInfinity(parentWorkUnits) || double.IsNaN(parentWorkUnits) || parentWorkUnits < 0.0)
                throw new ArgumentOutOfRangeException("parentWorkUnits", parentWorkUnits, "Parent work units must be finite and non-negative.");
			
            this.parent = parent;
            this.parentWorkUnits = parentWorkUnits;

            parent.Canceled += parent_Canceled;
        }

        /// <summary>
        /// Gets the parent progress monitor.
        /// </summary>
        public IProgressMonitor Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Gets the total number of work units of the parent progress
        /// monitor that are to be represented by this instance.  When the child monitor
        /// completes, this much work will have been performed on the parent.  Must
        /// be a finite value greater than 0.
        /// </summary>
        public double ParentWorkUnits
        {
            get { return parentWorkUnits; }
        }

        /// <inheritdoc />
        protected override void OnBeginTask(string taskName, double totalWorkUnits)
        {
            parent.BeginSubTask(taskName);
            beganTask = true;
        }

        /// <inheritdoc />
        protected override void OnSetStatus(string status)
        {
            parent.SetStatus(status);
        }

        /// <inheritdoc />
        protected override void OnWorked(double workUnits)
        {
            if (! double.IsNaN(TotalWorkUnits))
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

            parent.Canceled -= parent_Canceled;

            if (beganTask)
                parent.EndSubTask();
        }

        /// <inheritdoc />
        protected override void OnBeginSubTask(string subTaskName)
        {
            parent.BeginSubTask(subTaskName);
        }

        /// <inheritdoc />
        protected override void OnEndSubTask()
        {
            parent.EndSubTask();
        }

        private void parent_Canceled(object sender, EventArgs e)
        {
            NotifyCanceled();
        }
    }
}