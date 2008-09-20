using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
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
            mocks.ReplayAll();
            ExecutionLogController executionLogController = new ExecutionLogController(testController);
            Assert.IsNull(executionLogController.ExecutionLog);
            eventRaiser.Raise(testController, e);
            Assert.IsNotNull(executionLogController.ExecutionLog);
        }

        [Test]
        public void UpdateExecutionLog_Test()
        {
            ITestController testController = SetupTestController();
            Report report = new Report();
            Expect.Call(testController.Report).Return(report);
            mocks.ReplayAll();
            ExecutionLogController executionLogController = new ExecutionLogController(testController);
            list.Add(new TestTreeNode("text", "name", "nodeType"));
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
