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
using System.ComponentModel;
using System.Drawing;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model.Serialization;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    internal class TestResultsControllerTest
    {
        [Test]
        public void TestStatusBarStyle_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            string testStatusBarStyle = "testStatusBarStyle";
            optionsController.TestStatusBarStyle = testStatusBarStyle;
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(testStatusBarStyle, testResultsController.TestStatusBarStyle);
        }

        [Test]
        public void PassedColor_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var passedColour = Color.Green;
            optionsController.PassedColor = passedColour;
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(passedColour, testResultsController.PassedColor);
        }

        [Test]
        public void FailedColor_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var failedColour = Color.Red;
            optionsController.FailedColor = failedColour;
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(failedColour, testResultsController.FailedColor);
        }

        [Test]
        public void InconclusiveColor_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var inconclusiveColour = Color.Gold;
            optionsController.InconclusiveColor = inconclusiveColour;
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(inconclusiveColour, testResultsController.InconclusiveColor);
        }

        [Test]
        public void SkippedColor_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var skippedColour = Color.Gold;
            optionsController.SkippedColor = skippedColour;
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(skippedColour, testResultsController.SkippedColor);
        }

        [Test]
        public void PassedTestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int passedTestCount = 5;
            testController.Stub(tc => tc.Passed).Return(passedTestCount);
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(passedTestCount, testResultsController.PassedTestCount);
        }

        [Test]
        public void FailedTestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int failedTestCount = 5;
            testController.Stub(tc => tc.Failed).Return(failedTestCount);
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(failedTestCount, testResultsController.FailedTestCount);
        }

        [Test]
        public void SkippedTestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int skippedTestCount = 5;
            testController.Stub(tc => tc.Skipped).Return(skippedTestCount);
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(skippedTestCount, testResultsController.SkippedTestCount);
        }

        [Test]
        public void InconclusiveTestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int inconclusiveTestCount = 5;
            testController.Stub(tc => tc.Inconclusive).Return(inconclusiveTestCount);
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(inconclusiveTestCount, testResultsController.InconclusiveTestCount);
        }

        [Test]
        public void ElapsedTime_should_be_zero_before_test_run_starts()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(new TimeSpan(), testResultsController.ElapsedTime);
        }

        [Test]
        public void ElapsedTime_should_be_greater_than_zero_once_test_run_has_started()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var testResultsController = new TestResultsController(testController, optionsController);
            testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);

            Assert.GreaterThan(testResultsController.ElapsedTime, new TimeSpan());
        }

        [Test]
        public void ElapsedTime_should_stop_increasing_once_test_run_has_finished()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var testResultsController = new TestResultsController(testController, optionsController);
            
            testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);
            Assert.GreaterThan(testResultsController.ElapsedTime, new TimeSpan());

            testController.Raise(tc => tc.RunFinished += null, testController, EventArgs.Empty);
            var elapsed = testResultsController.ElapsedTime;
            Assert.AreEqual(elapsed, testResultsController.ElapsedTime);
        }

        [Test]
        public void ResultsCount_is_set_to_zero_when_test_run_is_started_and_notification_is_raised()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var testResultsController = new TestResultsController(testController, optionsController);
            testResultsController.SynchronizationContext = new TestSynchronizationContext();
            var propertyChangedFlag = false;
            testResultsController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != "ResultsCount")
                    return;

                Assert.AreEqual(0, testResultsController.ResultsCount);
                propertyChangedFlag = true;
            };

            testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);

            Assert.AreEqual(true, propertyChangedFlag);
        }

        [Test]
        public void TestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int testCount = 5;
            testController.Stub(tc => tc.TestCount).Return(testCount);
            var testResultsController = new TestResultsController(testController, optionsController);

            Assert.AreEqual(testCount, testResultsController.TestCount);
        }
    }
}
