namespace Gallio.UI.ProgressMonitoring 
{
    ///<summary>
    /// A task queue, for use by the <see cref="TaskManager"/>.
    ///</summary>
    public interface ITaskQueue 
    {
        /// <summary>
        /// Adds a task to the specified queue.
        /// </summary>
        /// <param name="queueId">The id of the queue to use.</param>
        /// <param name="command">An <see cref="ICommand"/> to queue.</param>
        void AddTask(string queueId, ICommand command);

        /// <summary>
        /// Get the next task for the specified queue.
        /// </summary>
        /// <param name="queueId">The id of the queue to use.</param>
        /// <returns>An <see cref="ICommand"/>, or null if the queue is empty.</returns>
        ICommand GetNextTask(string queueId);

        /// <summary>
        /// Removes all tasks from the specified queue.
        /// </summary>
        /// <param name="queueId">The id of the queue to use.</param>
        void RemoveAllTasks(string queueId);
    }
}
