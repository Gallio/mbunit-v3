using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MbUnit.Framework.Kernel.Events
{
	/// <summary>
	/// A progress monitor provides facilities for core functionality to report
	/// progress of a long running operation.  The interface is typically
	/// implemented by a UI component such as a progress dialog.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface is inspired from IProgressMonitor in the Eclipse API
    /// and was derived from the Castle.FlexBridge client-side components.
    /// </para>
    /// <para>
    /// This interface is mostly not safe for use by multiple concurrent
    /// threads.  Do not share a progress monitor instance across threads!
    /// <seealso cref="Canceled"/> for special rules regarding the handling
    /// the cancelation event.
    /// </para>
    /// <para>
    /// <see cref="IDisposable" /> is implemented as a convenience for use
    /// with the C# "using" statement.  Calling <see cref="IDisposable.Dispose" />
    /// is precisely equivalent to calling <see cref="Done" />.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Copy a number of files to another directory, updating the progress
    /// // monitor before each file is copied to provide status feedback and
    /// // then after each file is copied to provide progress feedback.
    /// const int CostOfCopyingFile = 1;
    /// const int CostOfRunningExpensiveTask = 10;
    /// 
    /// using (IProgressMonitor progressMonitor = progressMonitorDialog.GetProgressMonitor())
    /// {
    ///     progressMonitor.BeginTask("Copy files", files.Length * CostOfCopyingFile + CostOfRunningExpensiveTask);
    /// 
    ///     foreach (FileInfo file in files)
    ///     {
    ///         if (progressMonitor.IsCancelled)
    ///             return;
    /// 
    ///         progressMonitor.SetStatus("Copying: " + file.Name);
    ///         File.Copy(file.FullName, Path.Combine(destinationFolder, file.Name))
    ///         progressMonitor.Worked(CostOfCopyingFile);
    ///     }
    ///     progressMonitor.SetStatus("");
    /// 
    ///     DoExpensiveTask(new SubProgressMonitor(progressMonitor, CostOfRunningExpensiveTask));
    /// }
    /// </code>
    /// </example>
    public interface IProgressMonitor : IDisposable
	{
        /// <summary>
        /// Adds or removes an event handler to be called when the operation is canceled.
        /// If the operation has already been canceled, then the event handler will be
        /// called immediately.
        /// </summary>
        /// <remarks>
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
		/// Clients should poll this value periodically or listen for the
        /// <see cref="Canceled" /> event to ensure the operation is
		/// canceled in a timely fashion.
        /// </summary>
        /// <remarks>
        /// Not safe for use by multiple concurrent threads.
        /// </remarks>
		bool IsCanceled { get; }

        /// <summary>
		/// Notifies that the main task is starting.
		/// Must be called at most once on the progress monitor.
        /// </summary>
        /// <remarks>
        /// Not safe for use by multiple concurrent threads.
        /// </remarks>
        /// <param name="taskName">The name of the task being monitored</param>
        /// <param name="totalWorkUnits">The total number of work units to perform.  Must
        /// be greater than 0, or <see cref="double.NaN" /> if an indeterminate amount
        /// of work is to be performed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskName" /> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="totalWorkUnits" /> is not valid</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="BeginTask" /> or <see cref="Done" /> have already been called</exception>
		void BeginTask(string taskName, double totalWorkUnits);

        /// <summary>
        /// Sets detailed status information for the current task or subtask.
        /// A status message is an optional fine-grained description of the current
        /// activity being performed.  For instance, a status message might specify 
        /// the name of a file being copied as part of a task that copies many files.
        /// </summary>
        /// <remarks>
        /// Not safe for use by multiple concurrent threads.
        /// </remarks>
        /// <param name="status">The name of the current subtask</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="status"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the task is not running</exception>
        void SetStatus(string status);

        /// <summary>
	    /// Notifies that a given number of work units of the main task
		/// have been completed.  Note that this amount represents an installment,
		/// as opposed to a cumulative amount of work done to date.  If the sum of this
		/// value and the currently completed work units exceeds the total work units
        /// to be performed, the excess portion is discarded.
        /// </summary>
        /// <remarks>
        /// Not safe for use by multiple concurrent threads.
        /// </remarks>
        /// <param name="workUnits">The number of work units completed so far.  Must be
		/// a finite value greater than or equal to 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="workUnits"/> is not valid</exception>
        /// <exception cref="InvalidOperationException">Thrown if the task is not running</exception>
        void Worked(double workUnits);

		/// <summary>
		/// Notifies that the operation is to be canceled.
        /// The method always causes the <see cref="IsCanceled" /> flag to be
        /// set, even if the operation is already done.
        /// </summary>
        /// <remarks>
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
        /// was cancelled by the user.  If already done, the method has no effect.
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
        /// ended by this method exactly as if <see cref="EndSubTask" /> had been
        /// called a sufficient number of times to end them.  The reason we do this
        /// (rather than throw an error) is to simplify error recovery in situations
        /// where the main task may have been abruptly aborted without proper cleanup
        /// of its inner state.  This can happen in situations where severe exceptions
        /// bubble up to the main task.  It seems preferable to tolerate the situation
        /// than to exacerbate it by enforcing stricter rules.
        /// </para>
        /// </remarks>
        void Done();

        /// <summary>
        /// Begins a subtask nested within the current task or subtask.
        /// Resets the current status to the empty string.
        /// A subtask represents a segment of the total work being performed
        /// by a task.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called by <see cref="SubProgressMonitor" /> as a new
        /// task or sub-task begins.
        /// </para>
        /// <para>
        /// Not safe for use by multiple concurrent threads.
        /// </para>
        /// </remarks>
        /// <param name="subTaskName">The name of the current subtask</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="subTaskName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the task is not running</exception>
        void BeginSubTask(string subTaskName);

        /// <summary>
        /// Ends the current subtask.
        /// Resets the current status to the empty string.
        /// Subtasks are usually managed by a <see cref="SubProgressMonitor" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called by <see cref="SubProgressMonitor" /> as a new
        /// task or sub-task begins.
        /// </para>
        /// <para>
        /// Not safe for use by multiple concurrent threads.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the task is not running
        /// or if there is no current subtask</exception>
        void EndSubTask();
	}
}
