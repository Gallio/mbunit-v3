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
using Gallio.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Model;
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
        private IEventRaiser testRunStartedEventRaiser;
        private IEventRaiser testRunFinishedEventRaiser;

        private BindingList<TestTreeNode> selectedTests;

        [SetUp]
        public void SetUp()
        {
            selectedTests = new BindingList<TestTreeNode>(new List<TestTreeNode>());
        }

        [Test]
        public void TestStepFinished_Test()
        {
            TestStepRun testStepRun = new TestStepRun(new TestStepData("rootStep", "name", "fullName", "root"));
            testStepRun.TestLog = new StructuredTestLog();
            testStepRun.TestLog.Attachments.Add(new TextAttachment("name", "contentType", "text").ToAttachmentData());
            TestStepFinishedEventArgs e = new TestStepFinishedEventArgs(new Report(), new TestData("root", "name", "fullName"), testStepRun);
            ITestController testController = SetupTestController();
            Report report = new Report();
            report.TestPackageRun = new TestPackageRun();
            report.TestModel = new TestModelData(new TestData(new RootTest()));
            report.TestPackageRun.RootTestStepRun = testStepRun;

            SetupResult.For(testController.Report).Return(new LockBox<Report>(report));
            ITestTreeModel testTreeModel = mocks.StrictMock<ITestTreeModel>();
            SetupResult.For(testController.Model).Return(testTreeModel);
            TestTreeNode root = new TestTreeNode("root", "root", "root");
            SetupResult.For(testTreeModel.Root).Return(root);
            mocks.ReplayAll();

            ExecutionLogController executionLogController = new ExecutionLogController(testController);
            bool updated = false;
            executionLogController.ExecutionLogUpdated += delegate { updated = true; };

            selectedTests.Add(root);
            testRunStartedEventRaiser.Raise(testController, System.EventArgs.Empty);
            testRunFinishedEventRaiser.Raise(testController, e);

            Thread.Sleep(200); // wait for threadpool to run task

            Assert.AreEqual(true, updated);
            Assert.AreSame(report.TestModel, executionLogController.TestModelData);
            Assert.AreElementsEqual(new[] { testStepRun }, executionLogController.TestStepRuns);
        }

        ITestController SetupTestController()
        {
            ITestController testController = mocks.StrictMock<ITestController>();
            SetupResult.For(testController.SelectedTests).Return(selectedTests);

            testController.TestStepFinished += null;
            testRunFinishedEventRaiser = LastCall.IgnoreArguments().GetEventRaiser();

            testController.RunStarted += null;
            testRunStartedEventRaiser = LastCall.IgnoreArguments().GetEventRaiser();

            return testController;
        }
    }
}
