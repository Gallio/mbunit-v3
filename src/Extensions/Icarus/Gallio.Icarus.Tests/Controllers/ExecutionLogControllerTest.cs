using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
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

        [SetUp]
        public void SetUp()
        {
            list = new BindingList<TestTreeNode>(new List<TestTreeNode>());
        }

        [Test]
        public void TestStepFinished_Test()
        {
            TestStepFinishedEventArgs e = new TestStepFinishedEventArgs(new Report(),
                new TestData("id", "name", "fullName"),
                new TestStepRun(new TestStepData("id", "name", "fullName", "testId")));
            ITestController testController = SetupTestController();
            Report report = new Report();
            report.TestPackageRun = new TestPackageRun();
            Expect.Call(testController.Report).Return(report).Repeat.Any();
            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();
            Expect.Call(testController.Model).Return(testTreeModel).Repeat.Twice();
            TestTreeNode root = new TestTreeNode("root", "root", "root");
            Expect.Call(testTreeModel.Root).Return(root).Repeat.Twice();
            mocks.ReplayAll();
            ExecutionLogController executionLogController = new ExecutionLogController(testController);
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
            ExecutionLogController executionLogController = new ExecutionLogController(testController);
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
            ExecutionLogController executionLogController = new ExecutionLogController(testController);
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
