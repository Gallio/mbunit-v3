namespace Gallio.UI.ProgressMonitoring {
    ///<summary>
    /// Takes care of running queued tasks.
    ///</summary>
    public interface ITaskRunner {
        ///<summary>
        /// Run the next task for a named queue, if there is one.
        ///</summary>
        ///<param name="queueId">The id of the queue to use.</param>
        void RunTask(string queueId);
    }
}

