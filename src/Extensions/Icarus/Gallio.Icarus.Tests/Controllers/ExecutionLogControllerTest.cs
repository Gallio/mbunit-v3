// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Common.Markup;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Icarus.Tests.Utilities;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(ExecutionLogController))]
    public class ExecutionLogControllerTest
    {
        private ExecutionLogController executionLogController;
        private ITestController testController;
        private ITestTreeModel testTreeModel;

        [SetUp]
        public void SetUp()
        {
            testController = MockRepository.GenerateStub<ITestController>();
            var taskManager = new TestTaskManager();
            testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            executionLogController = new ExecutionLogController(testController, testTreeModel, taskManager);
        }

        [Test]
        public void TestStepFinished_Test()
        {
            var testStepRun = new TestStepRun(new TestStepData("root", "name", "fullName", "root"))
              {
                  TestLog = new StructuredDocument()
              };
            testStepRun.TestLog.Attachments.Add(new TextAttachment("name", "contentType", "text").ToAttachmentData());
            
            var report = new Report
                {
                    TestPackageRun = new TestPackageRun(),
                    TestModel = new TestModelData()
                };
            report.TestPackageRun.RootTestStepRun = testStepRun;

            testController.Stub(x => x.ReadReport(null)).IgnoreArguments().Do((Action<ReadAction<Report>>)(action => action(report)));
            testTreeModel.Stub(x => x.Root).Return(new TestTreeNode("root", "root"));

            var flag = false;
            executionLogController.ExecutionLogUpdated += (sender, e) =>
            {
                Assert.AreEqual(1, e.TestStepRuns.Count);
                flag = true;
            };
            var testData = new TestData("root", "name", "fullName")
            {
                IsTestCase = true
            };

            executionLogController.Handle(new TestStepFinished(testData, null));

            Assert.IsTrue(flag);
        }

        [Test]
        public void RunStarted_Test()
        {
            bool executionLogResetFlag = false;
            executionLogController.ExecutionLogReset += (sender, e) =>
                {
                    executionLogResetFlag = true;
                };

            executionLogController.Handle(new RunStarted());

            Assert.IsTrue(executionLogResetFlag);
        }

        [Test]
        public void ExecutionLog_should_be_updated_when_test_selection_changes()
        {
            var testStepRun = new TestStepRun(new TestStepData("rootStep", "name", 
                "fullName", "root"));
            var selectedTests = new List<TestTreeNode>
                {
                    new TestTreeNode("name", "rootStep")
                };
            var report = new Report
                {
                    TestPackageRun = new TestPackageRun(),
                    TestModel = new TestModelData()
                };
            report.TestPackageRun.RootTestStepRun = testStepRun;

            testController.Stub(x => x.ReadReport(Arg<ReadAction<Report>>.Is.Anything))
                .Do((Action<ReadAction<Report>>)(action => action(report)));
            testTreeModel.Stub(x => x.Root).Return(new TestTreeNode("root", "name"));
            var flag = false;
            executionLogController.ExecutionLogUpdated += delegate { flag = true; };

            executionLogController.Handle(new TestSelectionChanged(selectedTests));

            Assert.IsTrue(flag);
        }
    }
}