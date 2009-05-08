using Gallio.Common;

namespace Gallio.Icarus
{
    public interface ITaskManager
    {
        void BackgroundTask(Action action);
        void QueueTask(Action action);
        void Stop();
        bool TaskRunning { get; }
    }
}
