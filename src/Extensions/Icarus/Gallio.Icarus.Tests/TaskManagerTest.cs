using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using System.Threading;
using Gallio.Runtime;
using Gallio.Icarus.Utilities;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    class TaskManagerTest
    {
        private TaskManager taskManager;
        private IUnhandledExceptionPolicy unhandledExceptionPolicy;

        [SetUp]
        public void SetUp()
        {
            unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            taskManager = new TaskManager(unhandledExceptionPolicy);
        }

        [Test]
        public void StartTask_Test()
        {
            bool flag = true;
            taskManager.StartTask(delegate
            {
                do
                { }
                while (flag);
            });
            Assert.IsTrue(taskManager.TaskRunning);
            flag = false;
            Thread.Sleep(100);
            Assert.IsFalse(taskManager.TaskRunning);
        }

        [Test]
        public void ExceptionHandling_Test()
        {
            Exception ex = new Exception();
            taskManager.StartTask(delegate { throw ex; });
            Thread.Sleep(200);
            unhandledExceptionPolicy.AssertWasCalled(x => 
                x.Report("An exception occurred in a background task.", ex));
        }

        [Test]
        public void OperationCanceled_Test()
        {
            taskManager.StartTask(delegate { throw new OperationCanceledException(); });
            Thread.Sleep(200);
            unhandledExceptionPolicy.AssertWasNotCalled(x => 
                x.Report(Arg<string>.Is.Anything, Arg<Exception>.Is.Anything));
        }

        [Test]
        public void Queue_Test()
        {
            bool flag1 = false;
            bool flag2 = false;
            taskManager.StartTask(delegate { Thread.Sleep(100); flag1 = true; });
            taskManager.StartTask(delegate { flag2 = true; });
            Thread.Sleep(200);
            Assert.IsTrue(flag1);
            Assert.IsTrue(flag2);
        }
    }
}
