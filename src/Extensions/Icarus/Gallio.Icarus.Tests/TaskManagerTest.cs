// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Icarus.Commands;
using Gallio.Icarus.Utilities;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.Progress;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [TestsOn(typeof(TaskManager))]
    internal class TaskManagerTest
    {
        private class DelegateCommand : ICommand
        {
            private readonly Action<IProgressMonitor> action;

            public DelegateCommand(Action<IProgressMonitor> action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");

                this.action = action;
            }

            public void Execute(IProgressMonitor progressMonitor)
            {
                action(progressMonitor);
            }
        }

        [Test]
        public void Task_should_run_if_first_in_queue_and_started()
        {
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var taskManager = new TaskManager(unhandledExceptionPolicy);

            var manualResetEvent = new ManualResetEvent(false);
            taskManager.QueueTask(new DelegateCommand(pm => manualResetEvent.Set()));

            manualResetEvent.WaitOne();
        }

        [Test]
        public void TaskRunning_should_return_true_when_a_task_is_running()
        {
            var taskManager = new TaskManager(MockRepository.GenerateStub<IUnhandledExceptionPolicy>());

            var mre = new ManualResetEvent(false);
            taskManager.Start();
            taskManager.QueueTask(new DelegateCommand(pm => mre.WaitOne()));

            Assert.AreEqual(true, taskManager.TaskRunning);
            mre.Set();
        }

        [Test]
        public void QueueTask_should_throw_if_not_started()
        {
            var taskManager = new TaskManager(MockRepository.GenerateStub<IUnhandledExceptionPolicy>());
            Assert.Throws<Exception>(() => taskManager.QueueTask(new DelegateCommand(pm => Assert.Fail())));
        }

        [Test]
        public void ProgressUpdates_should_bubble_up_from_task()
        {
            var taskManager = new TaskManager(MockRepository.GenerateStub<IUnhandledExceptionPolicy>());

            var manualResetEvent = new ManualResetEvent(false);
            taskManager.ProgressUpdate += (sender, e) =>
                {
                    Assert.AreEqual(string.Empty, e.TaskName);
                    Assert.AreEqual(string.Empty, e.SubTaskName);
                    Assert.AreEqual(0, e.CompletedWorkUnits);
                    Assert.AreEqual(100, e.TotalWorkUnits);
                    manualResetEvent.Set();
                };
            taskManager.Start();
            taskManager.QueueTask(new DelegateCommand(pm => pm.BeginTask("taskName", 100)));

            manualResetEvent.WaitOne();
        }

        [Test]
        public void BackgroundTask_should_run()
        {
            var taskManager = new TaskManager(MockRepository.GenerateStub<IUnhandledExceptionPolicy>());

            var mre = new ManualResetEvent(false);
            taskManager.Start();
            taskManager.BackgroundTask(() => mre.Set());

            mre.WaitOne();
        }

        [Test]
        public void BackgroundTask_should_throw_if_not_started()
        {
            var taskManager = new TaskManager(MockRepository.GenerateStub<IUnhandledExceptionPolicy>());
            Assert.Throws<Exception>(() => taskManager.BackgroundTask(Assert.Fail));
        }

        [Test]
        public void UnhandledExceptions_from_task_should_be_reported()
        {
            var ex = new Exception();
            var manualResetEvent = new ManualResetEvent(false);
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            unhandledExceptionPolicy.Stub(uep => uep.Report("An exception occurred while running a task.", 
                ex)).Do((Action<string, Exception>) ((s, e) => manualResetEvent.Set()));
            var taskManager = new TaskManager(unhandledExceptionPolicy);
            taskManager.Start();

            taskManager.QueueTask(new DelegateCommand(pm => { throw ex; } ));

            manualResetEvent.WaitOne();
        }

        [Test]
        public void TaskCanceled_should_be_raised_where_necessary()
        {
            var taskManager = new TaskManager(MockRepository.GenerateStub<IUnhandledExceptionPolicy>());
            taskManager.Start();
            var manualResetEvent = new ManualResetEvent(false);
            taskManager.TaskCanceled += (sender, e) => manualResetEvent.Set();

            taskManager.QueueTask(new DelegateCommand(pm => { throw new OperationCanceledException(); } ));

            manualResetEvent.WaitOne();
        }

        [Test]
        public void TaskStarted_should_be_raised_when_a_task_is_started()
        {
            var taskManager = new TaskManager(MockRepository.GenerateStub<IUnhandledExceptionPolicy>());
            taskManager.Start();
            var manualResetEvent = new ManualResetEvent(false);
            taskManager.TaskStarted += (sender, e) => manualResetEvent.Set();

            taskManager.QueueTask(new DelegateCommand(pm => { }));

            manualResetEvent.WaitOne();
        }

        [Test]
        public void TaskCompleted_should_be_raised_when_a_task_finishes()
        {
            var taskManager = new TaskManager(MockRepository.GenerateStub<IUnhandledExceptionPolicy>());
            taskManager.Start();
            var manualResetEvent = new ManualResetEvent(false);
            taskManager.TaskCompleted += (sender, e) => manualResetEvent.Set();

            taskManager.QueueTask(new DelegateCommand(pm => { }));

            manualResetEvent.WaitOne();
        }

        [Test]
        public void Queue_should_be_cleared_when_stopped()
        {
            var taskManager = new TaskManager(MockRepository.GenerateStub<IUnhandledExceptionPolicy>());
            taskManager.Start();

            var mre = new ManualResetEvent(false);
            taskManager.QueueTask(new DelegateCommand(pm => mre.WaitOne()));
            taskManager.QueueTask(new DelegateCommand(pm => Assert.Fail()));
            taskManager.Stop();
            mre.Set();
            
            taskManager.Start();
        }
    }
}
