using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Events;

namespace Gallio.UI.ProgressMonitoring
{
    ///<summary>
    /// Event fired when a task is started.
    ///</summary>
    public class TaskStarted : Event
    {
        ///<summary>
        /// The id of the queue.
        ///</summary>
        public string QueueId { get; private set; }
        ///<summary>
        /// A progress monitor attached to the task.
        ///</summary>
        public ObservableProgressMonitor ProgressMonitor { get; private set; }

        ///<summary>
        /// Ctor.
        ///</summary>
        ///<param name="queueId">The queue id.</param>
        ///<param name="progressMonitor">A progress monitor.</param>
        public TaskStarted(string queueId, ObservableProgressMonitor progressMonitor)
        {
            QueueId = queueId;
            ProgressMonitor = progressMonitor;
        }
    }
}