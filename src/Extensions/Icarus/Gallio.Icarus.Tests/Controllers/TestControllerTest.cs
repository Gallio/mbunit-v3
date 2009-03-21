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

using Gallio.Icarus.Controllers;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(TestController))]
    class TestControllerTest
    {
        [Test]
        public void ApplyFilter_Test()
        {
            Filter<ITest> filter = new NoneFilter<ITest>();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            testController.ApplyFilter(filter);
            testTreeModel.AssertWasCalled(ttm => ttm.ApplyFilter(filter));
        }

        [Test]
        public void Explore_Test()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            var exploreStartedFlag = false;
            testController.ExploreStarted += delegate { exploreStartedFlag = true; };
            var exploreFinishedFlag = false;
            testController.ExploreFinished += delegate { exploreFinishedFlag = true; };
            var testRunnerFactory = MockRepository.GenerateStub<ITestRunnerFactory>();
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(tr => tr.Events).Return(testRunnerEvents);
            testRunnerFactory.Stub(trf => trf.CreateTestRunner()).Return(testRunner);
            testController.SetTestRunnerFactory(testRunnerFactory);
            const string treeViewCategory = "treeViewCategory";
            testController.TreeViewCategory = treeViewCategory;

            testController.Explore(progressMonitor);
            
            Assert.IsTrue(exploreStartedFlag);
            testRunner.AssertWasCalled(tr => tr.Initialize(Arg<TestRunnerOptions>.Is.Anything, 
                Arg<ILogger>.Is.Anything, Arg.Is(progressMonitor)));
            testRunner.AssertWasCalled(tr => tr.Explore(Arg<TestPackageConfig>.Is.Anything, 
                Arg<TestExplorationOptions>.Is.Anything, Arg.Is(progressMonitor)));
            testTreeModel.AssertWasCalled(ttm => ttm.BuildTestTree(Arg<TestModelData>.Is.Anything, 
                Arg.Is(treeViewCategory)));
            Assert.IsTrue(exploreFinishedFlag);
        }
    }
}
