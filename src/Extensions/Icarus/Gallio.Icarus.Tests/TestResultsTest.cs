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
