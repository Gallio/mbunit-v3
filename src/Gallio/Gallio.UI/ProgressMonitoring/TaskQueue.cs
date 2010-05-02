using System.Collections.Generic;

namespace Gallio.UI.ProgressMonitoring
{
    /// <summary>
    /// Manages multiple queues of tasks.
    /// </summary>
    /// <remarks>
    /// Queue id is an arbitrary string, used as a key.
    /// </remarks>
    public class TaskQueue : ITaskQueue
    {
        private readonly IDictionary<string, Queue<ICommand>> allTasks;

        ///<summary>
        /// Ctor.
        ///</summary>
        public TaskQueue()
        {
            allTasks = new Dictionary<string, Queue<ICommand>>();
        }

        /// <summary>
        /// Adds a task to the specified queue.
        /// </summary>
        /// <param name="queueId">The id of the queue to use.</param>
        /// <param name="command">An <see cref="ICommand"/> to queue.</param>
        public void AddTask(string queueId, ICommand command)
        {
            var tasks = GetTasks(queueId);
            tasks.Enqueue(command);
        }

        private Queue<ICommand> GetTasks(string queueId)
        {
            Queue<ICommand> tasks;
            allTasks.TryGetValue(queueId, out tasks);
            if (tasks == null)
            {
                tasks = new Queue<ICommand>();
                allTasks.Add(queueId, tasks);
            }
            return tasks;
        }

        /// <summary>
        /// Get the next task for the specified queue.
        /// </summary>
        /// <param name="queueId">The id of the queue to use.</param>
        /// <returns>An <see cref="ICommand"/>, or null if the queue is empty.</returns>
        public ICommand GetNextTask(string queueId)
        {
            var tasks = GetTasks(queueId);
            return tasks.Count > 0 ? tasks.Dequeue() : null;
        }

        /// <summary>
        /// Removes all tasks from the specified queue.
        /// </summary>
        /// <param name="queueId">The id of the queue to use.</param>
        public void RemoveAllTasks(string queueId)
        {
            var tasks = GetTasks(queueId);
            tasks.Clear();
        }
    }
}

