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
using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Common.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Common.Markup;
using Gallio.Model.Schema;
using Gallio.Runner.Events;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Icarus.Tests.Utilities;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(ExecutionLogController))]
    class ExecutionLogControllerTest
    {
        [Test]
        public void TestStepFinished_Test()
        {
            var testStepRun = new TestStepRun(new TestStepData("root", "name", "fullName", "root"))
                                  {TestLog = new StructuredDocument()};
            testStepRun.TestLog.Attachments.Add(new TextAttachment("name", "contentType", "text").ToAttachmentData());
            var testStepFinishedEventArgs = new TestStepFinishedEventArgs(new Report(), 
                new TestData("root", "name", "fullName"), testStepRun);
            
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(x => x.SelectedTests).Return(new BindingList<TestTreeNode>(new List<TestTreeNode>()));
            var report = new Report
                             {
                                 TestPackageRun = new TestPackageRun(),
                                 TestModel = new TestModelData()
                             };
            report.TestPackageRun.RootTestStepRun = testStepRun;

            testController.Stub(x => x.ReadReport(null)).IgnoreArguments().Do((Action<ReadAction<Report>>)(action => action(report)));
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testTreeModel.Stub(x => x.Root).Return(new TestTreeNode("root", "root"));
            testController.Stub(x => x.Model).Return(testTreeModel);
            var taskManager = new TestTaskManager();

            var executionLogController = new ExecutionLogController(testController, taskManager);
            var flag = false;
            executionLogController.ExecutionLogUpdated += (sender, e) =>
            {
                Assert.AreEqual(1, e.TestStepRuns.Count);
                flag = true;
            };

            testController.Raise(x => x.TestStepFinished += null, testController, testStepFinishedEventArgs);

            Assert.IsTrue(flag);
        }

        [Test]
        public void RunStarted_Test()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(x => x.SelectedTests).Return(new BindingList<TestTreeNode>(new List<TestTreeNode>()));
            var taskManager = new TestTaskManager();

            var executionLogController = new ExecutionLogController(testController, taskManager);

            bool executionLogResetFlag = false;
            executionLogController.ExecutionLogReset += (sender, e) =>
            {
                executionLogResetFlag = true;
            };

            testController.Raise(tc => tc.RunStarted += null, this, EventArgs.Empty);

            Assert.IsTrue(executionLogResetFlag);
        }

        [Test]
        public void ExecutionLog_should_be_updated_when_test_selection_changes()
        {
            var testStepRun = new TestStepRun(new TestStepData("rootStep", "name", 
                "fullName", "root"));
            var testController = MockRepository.GenerateStub<ITestController>();
            var selectedTests = new List<TestTreeNode>(new[]
            {
                new TestTreeNode("text", "rootStep")
            });
            testController.Stub(x => x.SelectedTests).Return(selectedTests);
            var report = new Report
            {
                TestPackageRun = new TestPackageRun(),
                TestModel = new TestModelData()
            };
            report.TestPackageRun.RootTestStepRun = testStepRun;

            testController.Stub(x => x.ReadReport(null)).IgnoreArguments()
                .Do((Action<ReadAction<Report>>)(action => action(report)));
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testTreeModel.Stub(x => x.Root).Return(new TestTreeNode("root", "name"));
            testController.Stub(x => x.Model).Return(testTreeModel);
            var taskManager = new TestTaskManager();

            var executionLogController = new ExecutionLogController(testController, taskManager);

            var flag = false;
            executionLogController.ExecutionLogUpdated += delegate { flag = true; };
            testController.Raise(tc => tc.PropertyChanged += null, testController, 
                new PropertyChangedEventArgs("SelectedTests"));

            Assert.IsTrue(flag);
        }

        [Test]
        public void Ctor_should_throw_if_TestController_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ExecutionLogController(null, null));
        }

        [Test]
        public void Ctor_should_throw_if_TaskManager_is_null()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            Assert.Throws<ArgumentNullException>(() => new ExecutionLogController(testController, null));
        }
    }
}