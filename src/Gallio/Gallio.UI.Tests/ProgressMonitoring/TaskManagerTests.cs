using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.UI.Tests.ProgressMonitoring 
{
    public class TaskManagerTests 
    {
        private ITaskQueue taskQueue;
        private ITaskRunner taskRunner;
        private TaskManager taskManager;

        [SetUp]
        public void SetUp() 
        {
            taskQueue = MockRepository.GenerateStub<ITaskQueue>();
            taskRunner = MockRepository.GenerateStub<ITaskRunner>();
            taskManager = new TaskManager(taskQueue, taskRunner);
        }

        [Test]
        public void QueueTask_should_add_command_to_named_queue() 
        {
            const string queueId = "test";
            var command = MockRepository.GenerateStub<ICommand>();

            taskManager.QueueTask(queueId, command);

            taskQueue.AssertWasCalled(tq => tq.AddTask(queueId, command));
        }

        [Test]
        public void QueueTask_should_try_to_run_the_task() 
        {
            const string queueId = "test";

            taskManager.QueueTask(queueId, null);

            taskRunner.AssertWasCalled(tr => tr.RunTask(queueId));
        }

        [Test]
        public void ClearQueue_should_remove_all_pending_tasks() {
            const string queueId = "test";

            taskManager.ClearQueue(queueId);

            taskQueue.AssertWasCalled(tq => tq.RemoveAllTasks(queueId));
        }
    }
}
