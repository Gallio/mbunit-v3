// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.Presenter;
using Gallio.Icarus.Tests;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
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
        private IEventRaiser generateReportEvent;
        private IEventRaiser stopTestsEvent;
        private IEventRaiser setFilterEvent;
        private IEventRaiser getReportTypesEvent;
        private IEventRaiser saveReportAsEvent;

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

            mockAdapter.GenerateReport += null;
            LastCall.IgnoreArguments();
            generateReportEvent = LastCall.GetEventRaiser();

            mockAdapter.StopTests += null;
            LastCall.IgnoreArguments();
            stopTestsEvent = LastCall.GetEventRaiser();

            mockAdapter.SetFilter += null;
            LastCall.IgnoreArguments();
            setFilterEvent = LastCall.GetEventRaiser();

            mockAdapter.GetReportTypes += null;
            LastCall.IgnoreArguments();
            getReportTypesEvent = LastCall.GetEventRaiser();

            mockAdapter.SaveReportAs += null;
            LastCall.IgnoreArguments();
            saveReportAsEvent = LastCall.GetEventRaiser();
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
            TestPackageConfig testPackageConfig = new TestPackageConfig();
            mockModel.LoadPackage(testPackageConfig);
            Expect.Call(mockModel.BuildTests()).Return(null);
            mockAdapter.TestModelData = null;
            LastCall.IgnoreArguments();
            mockAdapter.DataBind("mode", true);
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            getTestTreeEvent.Raise(mockAdapter, new GetTestTreeEventArgs("mode", true, true, testPackageConfig));
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
        public void GenerateReport_Test()
        {
            mockModel.GenerateReport();
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            generateReportEvent.Raise(mockAdapter, EventArgs.Empty);
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
            string filterName = "test";
            Filter<ITest> filter = new NoneFilter<ITest>();
            //mockModel.SetFilter(filterName, filter);
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            setFilterEvent.Raise(mockAdapter, new SetFilterEventArgs(filterName, filter));
        }

        [Test]
        public void GetReportTypes_Test()
        {
            IList<string> reportTypes = new List<string>();
            Expect.Call(mockModel.GetReportTypes()).Return(reportTypes);
            mockAdapter.ReportTypes = reportTypes;
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            getReportTypesEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void SaveReportAs_Test()
        {
            string fileName = @"c:\test.txt";
            string format = "html";
            mockModel.SaveReportAs(fileName, format);
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            saveReportAsEvent.Raise(mockAdapter, new SaveReportAsEventArgs(fileName, format));
        }

        [Test]
        public void Update_Test()
        {
            TestData testData = new TestData("test1", "test1");
            TestStepRun testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testInstanceId"));
            mockAdapter.Update(testData, testStepRun);
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            projectPresenter.Update(testData, testStepRun);
        }
    }
}
