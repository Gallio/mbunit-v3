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

using System;
using System.Threading;

namespace Gallio.Runtime.ProgressMonitoring
{
    /// <summary>
    /// A progress monitor provides facilities for core functionality to report
    /// progress of a long running operation.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// The interface is typically implemented by a UI component such as a progress dialog.
    /// </para>
    /// <para>
    /// This interface is inspired from IProgressMonitor in the Eclipse API
    /// and was derived from the Castle.FlexBridge client-side components.
    /// </para>
    /// <para>
    /// This interface is mostly not safe for use by multiple concurrent
    /// threads.  Do not share a progress monitor instance across threads!
    /// <seealso cref="Canceled"/> for special rules regarding the handling
    /// of the cancelation event.
    /// </para>
    /// <para>
    /// The <see cref="BeginTask" /> method returns a <see cref="ProgressMonitorTaskCookie" />
    /// which implements <see cref="IDisposable"/> as a convenience for use
    /// with the C# "using" statement.  Disposing the cookie is precisely equivalent to
    /// calling the <see cref="Done"/> method of the progress monitor.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// // Copy a number of files to another directory, updating the progress
    /// // monitor before each file is copied to provide status feedback and
    /// // then after each file is copied to provide progress feedback.
    /// const int CostOfCopyingFile = 1;
    /// const int CostOfRunningExpensiveTask = 10;
    /// 
    /// IProgressMonitor progressMonitor = progressMonitorDialog.GetProgressMonitor())
    /// 
    /// using (progressMonitor.BeginTask("Copy files", files.Length * CostOfCopyingFile + CostOfRunningExpensiveTask))
    /// { 
    ///     foreach (FileInfo file in files)
    ///     {
    ///         if (progressMonitor.IsCancelled)
    ///             return;
    /// 
    ///         progressMonitor.SetStatus("Copying: " + file.Name);
    ///         File.Copy(file.FullName, Path.Combine(destinationFolder, file.Name))
    ///         progressMonitor.Worked(CostOfCopyingFile);
    ///     }
    ///     
    ///     progressMonitor.SetStatus("");
    ///     DoExpensiveTask(progressMonitor.CreateSubProgressMonitor(CostOfRunningExpensiveTask));
    /// }
    /// ]]></code>
    /// </example>
    public interface IProgressMonitor : IDisposable
    {
        /// <summary>
        /// Adds or removes an event handler to be called when the operation is canceled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the operation has already been canceled, then the event handler will be
        /// called immediately.
        /// </para>
        /// <para>
        /// Because the event may be delivered on a different thread from
        /// the one executing the main task, it is an error for the handler
        /// of this event to call any methods of this <see cref="IProgressMonitor" />.
        /// Instead it should simply notify its main task to quit if possible
        /// (possibly by interrupting the main task's thread).  Thus when the
        /// main task terminates, it will call <see cref="Done" /> itself
        /// to clean up.
        /// </para>
        /// <para>
        /// The event handler should not block or throw exceptions.  If it
        /// must perform any possibly long-running tasks of its own (such as
        /// synchronizing with processing threads) it should queue up an
        /// asynchronous task (possibly using the <see cref="ThreadPool" />).
        /// </para>
        /// <para>
        /// Unlike the other members of this interface, adding and removing event
        /// handlers is safe for use by multiple concurrent threads.
        /// </para>
        /// </remarks>
        event EventHandler Canceled;

        /// <summary>
        /// Returns true if the operation has been canceled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Clients should poll this value periodically or listen for the
        /// <see cref="Canceled" /> event to ensure the operation is
        /// canceled in a timely fashion.
        /// </para>
        /// <para>
        /// Not safe for use by multiple concurrent threads.
        /// </para>
        /// </remarks>
        bool IsCanceled { get; }

        /// <summary>
        /// Notifies that the main task is starting.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Must be called at most once on the progress monitor.
        /// </para>
        /// <para>
        /// Not safe for use by multiple concurrent threads.
        /// </para>
        /// </remarks>
        /// <param name="taskName">The name of the task being monitored.</param>
        /// <param name="totalWorkUnits">The total number of work units to perform.  Must
        /// be greater than 0, or <see cref="double.NaN" /> if an indeterminate amount
        /// of work is to be performed.</param>
        /// <returns>An object that calls <see cref="Done"/> when disposed.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskName" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="totalWorkUnits" /> is not valid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="BeginTask" /> or <see cref="Done" /> have already been called.</exception>
        ProgressMonitorTaskCookie BeginTask(string taskName, double totalWorkUnits);

        /// <summary>
        /// Sets detailed status information for the current task or subtask.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A status message is an optional fine-grained description of the current
        /// activity being performed.  For instance, a status message might specify 
        /// the name of a file being copied as part of a task that copies many files.
        /// </para>
        /// <para>
        /// Not safe for use by multiple concurrent threads.
        /// </para>
        /// </remarks>
        /// <param name="status">The name of the current subtask.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="status"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the task is not running.</exception>
        void SetStatus(string status);

        /// <summary>
        /// Notifies that a given number of work units of the main task
        /// have been completed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that this amount represents an installment,
        /// as opposed to a cumulative amount of work done to date.  If the sum of this
        /// value and the currently completed work units exceeds the total work units
        /// to be performed, the excess portion is discarded.
        /// </para>
        /// <para>
        /// Not safe for use by multiple concurrent threads.
        /// </para>
        /// </remarks>
        /// <param name="workUnits">The number of work units completed so far.  Must be
        /// a finite value greater than or equal to 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="workUnits"/> is not valid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the task is not running.</exception>
        void Worked(double workUnits);

        /// <summary>
        /// Notifies that the operation is to be canceled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The method always causes the <see cref="IsCanceled" /> flag to be
        /// set, even if the operation is already done.
        /// </para>
        /// <para>
        /// It is safe to call this method before <see cref="BeginTask" /> in which
        /// case the task will begin in an already canceled state.
        /// </para>
        /// <para>
        /// Not safe for use by multiple concurrent threads.
        /// </para>
        /// </remarks>
        void Cancel();

        /// <summary>
        /// Notifies that the work is done, either the main task is completed
        /// was cancelled by the user. If already done, the method has no effect.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method automatically sets the amount of work remaining to zero.
        /// It also sets the amount of work completed to the total unless it was
        /// indeterminate or <see cref="BeginTask" /> was never called.
        /// </para>
        /// <para>
        /// It is safe to call this method without calling <see cref="BeginTask" />
        /// first in which case the task is immediately completed without ever
        /// having run.  However, <see cref="BeginTask" /> may not subsequently be
        /// called.
        /// </para>
        /// <para>
        /// If there are any currently active subtasks, they are automatically
        /// ended by this method exactly as if each subtask's <see cref="IProgressMonitor" />
        /// had been notified that the work was done.  This policy simplifies error
        /// recovery in situations where the main task may have been abruptly aborted
        /// without proper cleanup of its subtasks such as when severe exceptions occur.
        /// It is preferable to tolerate the error rather than to exacerbate it by enforcing
        /// stricter rules on subtasks.
        /// </para>
        /// </remarks>
        void Done();

        /// <summary>
        /// Creates a sub-progress monitor that represents a given number of
        /// work-units as a sub-task of this progress monitor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Using sub-tasks allows multiple tasks to be composed into longer sequences
        /// that each contribute a predetermined portion of the total work.
        /// </para>
        /// <para>
        /// As the sub-task performs work its parent task is notified of progress
        /// in proportion to the number of work units that it represents.  Likewise the
        /// parent is notified of cancelation if the child is canceled and vice-versa.
        /// </para>
        /// <para>
        /// It it still necessary to call <see cref="IProgressMonitor.BeginTask" /> on the
        /// sub-progress monitor to begin processing the sub-task.
        /// </para>
        /// </remarks>
        /// <param name="parentWorkUnits">The total number of work units of the parent task
        /// that are to be represented by the sub-task.  When the sub-task completes, this much
        /// work will have been performed on the parent.
        /// Must be a finite value greater than or equal to 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="parentWorkUnits"/> is not valid.</exception>
        IProgressMonitor CreateSubProgressMonitor(double parentWorkUnits);

        /// <summary>
        /// Throws an <see cref="OperationCanceledException" /> if the operation
        /// has been canceled.
        /// </summary>
        /// <exception cref="OperationCanceledException">Thrown if <see cref="IsCanceled"/> is true.</exception>
        void ThrowIfCanceled();
    }
}