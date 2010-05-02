using Gallio.UI.Events;

namespace Gallio.UI.ProgressMonitoring
{
    ///<summary>
    /// Event fired when a task is cancelled.
    ///</summary>
    public class TaskCancelled : Event
    {
        ///<summary>
        /// The id of the queue.
        ///</summary>
        public string QueueId { get; private set; }

        ///<summary>
        /// Ctor.
        ///</summary>
        ///<param name="queueId">The id of the queue.</param>
        public TaskCancelled(string queueId)
        {
            QueueId = queueId;
        }
    }
}