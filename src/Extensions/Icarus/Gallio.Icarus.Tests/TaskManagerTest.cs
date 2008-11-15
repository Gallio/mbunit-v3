// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
