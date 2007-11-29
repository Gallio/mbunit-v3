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

using System;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.Presenter;
using Gallio.Icarus.Tests;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Core.Presenter.Tests
{
    [TestFixture]
    public class ProjectPresenterTest : MockTest
    {
        private IProjectAdapter mockAdapter;
        private ITestRunnerModel mockModel;
        private ProjectPresenter projectPresenter;

        private IEventRaiser getTestTreeEvent;
        private IEventRaiser runTestsEvent;
        private IEventRaiser stopTestsEvent;
        private IEventRaiser setFilterEvent;
        private IEventRaiser getLogStreamEvent;
        private IEventRaiser generateReportEvent;

        [SetUp]
        public void SetUp()
        {
            projectPresenter = null;

            mockAdapter = mocks.CreateMock<IProjectAdapter>();
            mockModel = mocks.CreateMock<ITestRunnerModel>();

            mockModel.ProjectPresenter = null;
            LastCall.IgnoreArguments();

            mockAdapter.GetTestTree += null;
            LastCall.IgnoreArguments();
            getTestTreeEvent = LastCall.GetEventRaiser();

            mockAdapter.RunTests += null;
            LastCall.IgnoreArguments();
            runTestsEvent = LastCall.GetEventRaiser();

            mockAdapter.StopTests += null;
            LastCall.IgnoreArguments();
            stopTestsEvent = LastCall.GetEventRaiser();

            mockAdapter.SetFilter += null;
            LastCall.IgnoreArguments();
            setFilterEvent = LastCall.GetEventRaiser();

            mockAdapter.GetLogStream += null;
            LastCall.IgnoreArguments();
            getLogStreamEvent = LastCall.GetEventRaiser();

            mockAdapter.GenerateReport += null;
            LastCall.IgnoreArguments();
            generateReportEvent = LastCall.GetEventRaiser();
        }

        [Test]
        public void StatusText_Test()
        {
            mockAdapter.StatusText = "blah blah";
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            projectPresenter.StatusText = "blah blah";
        }

        [Test, Category("ProjectPresenter"), Category("AnotherCategory"), Author("Graham Hay")]
        public void CompletedWorkUnits_Test()
        {
            mockAdapter.CompletedWorkUnits = 2;
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            projectPresenter.CompletedWorkUnits = 2;
        }

        [Test]
        public void TotalWorkUnits_Test()
        {
            mockAdapter.TotalWorkUnits = 5;
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            projectPresenter.TotalWorkUnits = 5;
        }

        [Test]
        public void GetTestTree_Test()
        {
            TestPackage testPackage = new TestPackage();
            mockModel.LoadPackage(testPackage);
            mockModel.BuildTemplates();
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.BuildTests()).Return(null);
            mockAdapter.TestModel = null;
            LastCall.IgnoreArguments();
            mockAdapter.DataBind();
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            getTestTreeEvent.Raise(mockAdapter, new ProjectEventArgs(testPackage));
        }

        [Test]
        public void RunTests_Test()
        {
            mockModel.RunTests();
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            runTestsEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void StopTests_Test()
        {
            mockModel.StopTests();
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            stopTestsEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void SetFilter_Test()
        {
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            Filter<ITest> filter = new NoneFilter<ITest>();
            setFilterEvent.Raise(mockAdapter, new SetFilterEventArgs(filter));
            Assert.AreEqual(filter, projectPresenter.TestRunner.TestExecutionOptions.Filter);
        }

        [Test]
        public void GetLogStream_Test()
        {
            Expect.Call(mockModel.GetLogStream("getLogStreamTest")).Return("blah blah");
            mockAdapter.LogBody = "blah blah";
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            getLogStreamEvent.Raise(mockAdapter, new SingleStringEventArgs("getLogStreamTest"));
        }

        [Test]
        public void GenerateReport_Test()
        {
            mockModel.GenerateReport("generateReportTest");
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            generateReportEvent.Raise(mockAdapter, new SingleStringEventArgs("generateReportTest"));
        }

        [Test]
        public void Passed_Test()
        {
            mockAdapter.Passed("test1");
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            projectPresenter.Passed("test1");
        }

        [Test]
        public void Failed_Test()
        {
            mockAdapter.Failed("test1");
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            projectPresenter.Failed("test1");
        }

        [Test]
        public void Skipped_Test()
        {
            mockAdapter.Skipped("test1");
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            projectPresenter.Skipped("test1");
        }

        [Test]
        public void Ignored_Test()
        {
            mockAdapter.Ignored("test1");
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            projectPresenter.Ignored("test1");
        }
    }
}