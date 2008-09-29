using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Model.Serialization;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Tests
{
    [MbUnit.Framework.Category("Views")]
    class TestResultsTest : MockTest
    {
        private ITestController testController;
        private IOptionsController optionsController;
        private IEventRaiser testStepFinished;
        private IEventRaiser runStarted;

        [SetUp]
        public void SetUp()
        {
            testController = mocks.CreateMock<ITestController>();
            testController.TestStepFinished += null;
            testStepFinished = LastCall.IgnoreArguments().GetEventRaiser();
            Expect.Call(testController.SelectedTests).Return(new BindingList<TestTreeNode>(new List<TestTreeNode>()));
            testController.RunStarted += null;
            runStarted = LastCall.IgnoreArguments().GetEventRaiser();
            optionsController = mocks.CreateMock<IOptionsController>();
        }

        [Test]
        public void UpdateTestResults_Test()
        {
            Expect.Call(testController.Model).Return(new TestTreeModel());
            mocks.ReplayAll();
            TestResults testResults = new TestResults(testController, optionsController);
            testStepFinished.Raise(testController,
                new TestStepFinishedEventArgs(new Report(), new TestData("id", "name", "fullName"),
                    new TestStepRun(new TestStepData("id", "name", "fullName", "testId"))));
        }

        [Test]
        public void Reset_Test()
        {
            Expect.Call(testController.TestCount).Return(0);
            mocks.ReplayAll();
            TestResults testResults = new TestResults(testController, optionsController);
            runStarted.Raise(testController, EventArgs.Empty);
        }
    }
}
