// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using MbUnit.Framework;
using MbUnit.Icarus.Core.CustomEventArgs;
using MbUnit.Icarus.Core.Interfaces;
using MbUnit.Icarus.Core.Presenter;
using MbUnit.Runner;
using Rhino.Mocks;

namespace MbUnit.Icarus.Tests.Core.Presenter
{
    [TestFixture]
    public class IProjectPresenterTests : MockTest
    {
        private IProjectAdapter mockAdapter;
        private ITestRunnerModel mockModel;
        private IProjectPresenter projectPresenter;

        [SetUp]
        public void SetUp()
        {
            mockAdapter = MockRepository.GenerateStub<IProjectAdapter>();
            mockModel = MockRepository.GenerateStub<ITestRunnerModel>();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
        }

        [Test]
        public void StatusText_Test()
        {
            mockAdapter.StatusText = "blah blah";
            mocks.ReplayAll();
            projectPresenter.StatusText = "blah blah";
            Assert.AreEqual("blah blah", mockAdapter.StatusText);
        }

        [Test]
        public void CompletedWorkUnits_Test()
        {
            mockAdapter.CompletedWorkUnits = 2;
            mocks.ReplayAll();
            projectPresenter.CompletedWorkUnits = 2;
            Assert.AreEqual(2, mockAdapter.CompletedWorkUnits);
        }

        [Test]
        public void TotalWorkUnits_Test()
        {
            mockAdapter.TotalWorkUnits = 5;
            mocks.ReplayAll();
            projectPresenter.TotalWorkUnits = 5;
            Assert.AreEqual(5, mockAdapter.TotalWorkUnits);
        }

        [Test]
        public void GetTestTree_Test()
        {
            mockAdapter.TestModel = null;
            LastCall.IgnoreArguments();
            mockAdapter.DataBind();
            mocks.ReplayAll();
            projectPresenter.GetTestTree(mockAdapter, new ProjectEventArgs(new TestPackage()));
        }

        [Test]
        public void RunTests_Test()
        {
        }

        [Test]
        public void Passed_Test()
        {
            mockAdapter.Passed("test1");
            mocks.ReplayAll();
            projectPresenter.Passed("test1");
        }

        [Test]
        public void Failed_Test()
        {
            mockAdapter.Failed("test1");
            mocks.ReplayAll();
            projectPresenter.Failed("test1");
        }

        [Test]
        public void Skipped_Test()
        {
            mockAdapter.Skipped("test1");
            mocks.ReplayAll();
            projectPresenter.Skipped("test1");
        }

        [Test]
        public void Ignored_Test()
        {
            mockAdapter.Ignored("test1");
            mocks.ReplayAll();
            projectPresenter.Ignored("test1");
        }
    }
}