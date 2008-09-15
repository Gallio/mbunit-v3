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
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using MbUnit.Framework;
using Rhino.Mocks;
using System.Windows.Forms;

namespace Gallio.Icarus.Tests.Controllers
{
    class TestControllerTest : MockTest
    {
        [Test]
        public void ApplyFilter_Test()
        {
            Filter<ITest> filter = new NoneFilter<ITest>();

            ITestRunnerService testRunnerService = SetupTestRunnerService();
            testRunnerService.SetFilter(filter);

            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();
            testTreeModel.ApplyFilter(filter);
            
            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            testController.ApplyFilter(filter);
        }

        ITestRunnerService SetupTestRunnerService()
        {
            ITestRunnerService testRunnerService = mocks.CreateMock<ITestRunnerService>();
            testRunnerService.TestStepFinished += delegate { };
            LastCall.IgnoreArguments();
            testRunnerService.ProgressUpdate += delegate { };
            LastCall.IgnoreArguments();
            return testRunnerService;
        }

        [Test]
        public void Cancel_Test()
        {
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            testRunnerService.Cancel();

            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            testController.Cancel();
        }

        [Test]
        public void GetCurrentFilter_Test_AnyFilter()
        {
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            testRunnerService.SetFilter(null);
            LastCall.IgnoreArguments();

            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();
            TestTreeNode root = new TestTreeNode("root", "root", "root");
            root.CheckState = CheckState.Checked;
            Expect.Call(testTreeModel.Root).Return(root);

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual("*", testController.GetCurrentFilter().ToFilterExpr());
        }

        [Test]
        public void Selected_Tests_Test()
        {
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(0, testController.SelectedTests.Count);
        }

        [Test]
        public void Model_Test()
        {
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(testTreeModel, testController.Model);
        }

        [Test]
        public void TestFrameworks_Test()
        {
            List<string> testFrameworks = new List<string>();
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            Expect.Call(testRunnerService.TestFrameworks).Return(testFrameworks);
            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(testFrameworks, testController.TestFrameworks);
        }

        [Test]
        public void TestCount_Test()
        {
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();
            const int testCount = 0;
            Expect.Call(testTreeModel.TestCount).Return(testCount);

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(testCount, testController.TestCount);
        }

        [Test]
        public void ResetTests_Test()
        {
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            ITestTreeModel testTreeModel = mocks.CreateMock<ITestTreeModel>();
            testTreeModel.ResetTestStatus();

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            testController.ResetTests();
        }
    }
}
