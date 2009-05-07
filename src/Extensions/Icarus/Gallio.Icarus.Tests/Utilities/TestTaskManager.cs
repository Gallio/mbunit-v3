namespace Gallio.Icarus.Tests.Utilities
{
    internal class TestTaskManager : ITaskManager
    {
        public void BackgroundTask(Action action)
        {
            RunTask(action);
        }

        public void QueueTask(Action action)
        {
            RunTask(action);
        }

        private void RunTask(Action action)
        {
            TaskRunning = true;
            action();
            TaskRunning = true;
        }

        public void Stop()
        { }

        public bool TaskRunning
        {
            get;
            private set;
        }
    }
}
