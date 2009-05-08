// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System;
using Gallio.Common.Policies;

namespace Gallio.Runtime.ProgressMonitoring
{
    /// <summary>
    /// An abstract base class for progress monitors that implements the cancelation
    /// semantics only.
    /// </summary>
    [Serializable]
    public abstract class CancelableProgressMonitor : IProgressMonitor
    {
        private event EventHandler canceledHandler;
        private bool isCanceled;

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
                    if (!isCanceled)
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

        /// <inheritdoc />
        public void Cancel()
        {
            if (NotifyCanceled())
                OnCancel();
        }

        /// <inheritdoc />
        public void ThrowIfCanceled()
        {
            if (isCanceled)
                throw new OperationCanceledException("The user canceled the operation.");
        }
        
        /// <summary>
        /// Disposes the progress monitor, including cleaning up an marking as <see cref="Done"/>
        /// the current task, if any.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc />
        public abstract ProgressMonitorTaskCookie BeginTask(string taskName, double totalWorkUnits);

        /// <inheritdoc />
        public abstract void SetStatus(string status);

        /// <inheritdoc />
        public abstract void Worked(double workUnits);

        /// <inheritdoc />
        public abstract void Done();

        /// <inheritdoc />
        public abstract IProgressMonitor CreateSubProgressMonitor(double parentWorkUnits);

        /// <summary>
        /// Notifies that the task has actually been canceled.
        /// If this is the first time <see cref="NotifyCanceled" /> has been called,
        /// sets <see cref="IsCanceled" /> to true and fires the <see cref="Canceled" /> event.
        /// Otherwise does nothing.
        /// </summary>
        /// <remarks>
        /// This is the ONLY method of the progress monitor that is allowed to
        /// be called concurrently from multiple threads.  It needs to be this
        /// way so that cancelation can be initiated by either participant in the
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

            EventHandlerPolicy.SafeInvoke(currentCanceledHandler, this, EventArgs.Empty);
            return true;
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
        /// Disposes the progress monitor, including cleaning up an marking as <see cref="Done"/>
        /// the current task, if any.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()" /> was called</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Done();
        }
    }
}
