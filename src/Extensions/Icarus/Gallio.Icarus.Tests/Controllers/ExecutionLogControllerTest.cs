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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Model.Logging;
using Gallio.Model.Serialization;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), Author("Graham Hay")]
    class ExecutionLogControllerTest : MockTest
    {
        private IEventRaiser eventRaiser;
        private BindingList<TestTreeNode> list;
        readonly string executionLogFolder = Path.Combine(Path.GetTempPath(), "ExecutionLog");

        [SetUp]
        public void SetUp()
        {
            list = new BindingList<TestTreeNode>(new List<TestTreeNode>());
        }

        [Test]
        public void TestStepFinished_Test()
        {
            TestStepRun testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testId"));
            testStepRun.TestLog = new StructuredTestLog();
            testStepRun.TestLog.Attachments.Add(new TextAttachment("name", "contentType", "text").ToAttachmentData());
            TestStepFinishedEventArgs e = new TestStepFinishedEventArgs(new Report(), new TestData("id", "name", "fullName"), testStepRun);
            ITestController testController = SetupTestController();
            Report report = new Report();
            report.TestPackageRun = new TestPackageRun();
            Expect.Call(testController.Report).Return(report).Repeat.Any();
            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();
            Expect.Call(testController.Model).Return(testTreeModel).Repeat.Twice();
            TestTreeNode root = new TestTreeNode("root", "root", "root");
            Expect.Call(testTreeModel.Root).Return(root).Repeat.Twice();
            mocks.ReplayAll();
            Directory.Delete(executionLogFolder, true);
            ExecutionLogController executionLogController = new ExecutionLogController(testController, executionLogFolder);
            bool finished = false;
            executionLogController.ExecutionLogUpdated +=
                delegate { finished = true; };
            Assert.IsNull(executionLogController.ExecutionLog);
            eventRaiser.Raise(testController, e);
            int count = 0;
            do
            {
                Thread.Sleep(100);
                count++;
            } while (!finished && count < 10);
            Assert.IsNotNull(executionLogController.ExecutionLog);
        }

        [Test]
        public void UpdateExecutionLog_Test()
        {
            ITestController testController = SetupTestController();
            Report report = new Report();
            report.TestPackageRun = new TestPackageRun();
            Expect.Call(testController.Report).Return(report).Repeat.Times(3);
            mocks.ReplayAll();
            ExecutionLogController executionLogController = new ExecutionLogController(testController, executionLogFolder);
            bool finished = false;
            executionLogController.ExecutionLogUpdated += delegate { finished = true; };
            list.Add(new TestTreeNode("text", "name", "nodeType"));
            int count = 0;
            do
            {
                Thread.Sleep(100);
                count++;
            } while (!finished && count < 10);
            Assert.IsNotNull(executionLogController.ExecutionLog);
        }

        [Test]
        public void UpdateExecutionLog_Empty_Test()
        {
            ITestController testController = SetupTestController();
            Report report = new Report();
            Expect.Call(testController.Report).Return(report);
            mocks.ReplayAll();
            ExecutionLogController executionLogController = new ExecutionLogController(testController, executionLogFolder);
            bool finished = false;
            executionLogController.ExecutionLogUpdated += delegate { finished = true; };
            list.Add(new TestTreeNode("text", "name", "nodeType"));
            int count = 0;
            do
            {
                Thread.Sleep(100);
                count++;
            } while (!finished && count < 10);
            Assert.IsNull(executionLogController.ExecutionLog);
        }

        ITestController SetupTestController()
        {
            ITestController testController = mocks.CreateMock<ITestController>();
            Expect.Call(testController.SelectedTests).Return(list).Repeat.Any();
            testController.TestStepFinished += null;
            eventRaiser = LastCall.IgnoreArguments().GetEventRaiser();
            LastCall.IgnoreArguments();
            return testController;
        }
    }
}
