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
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.Presenter;
using Gallio.Icarus.Interfaces;
using Gallio.Icarus.Tests;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using System.IO;
using Gallio.Runner.Events;

namespace Gallio.Icarus.Tests.Core.Presenter
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
        private IEventRaiser cancelOperationEvent;
        private IEventRaiser setFilterEvent;
        private IEventRaiser getReportTypesEvent;
        private IEventRaiser saveReportAsEvent;
        private IEventRaiser getTestFrameworksEvent;
        private IEventRaiser getExecutionLogEvent;
        private IEventRaiser unloadTestPackageEvent;
        private IEventRaiser progressUpdateEvent;
        private IEventRaiser testStepFinishedEvent;

        [SetUp]
        public void SetUp()
        {
            projectPresenter = null;

            mockAdapter = mocks.CreateMock<IProjectAdapter>();
            mockModel = mocks.CreateMock<ITestRunnerModel>();

            mockAdapter.GetTestTree += null;
            LastCall.IgnoreArguments();
            getTestTreeEvent = LastCall.GetEventRaiser();

            mockAdapter.RunTests += null;
            LastCall.IgnoreArguments();
            runTestsEvent = LastCall.GetEventRaiser();

            mockAdapter.GenerateReport += null;
            LastCall.IgnoreArguments();
            generateReportEvent = LastCall.GetEventRaiser();

            mockAdapter.CancelOperation += null;
            LastCall.IgnoreArguments();
            cancelOperationEvent = LastCall.GetEventRaiser();

            mockAdapter.SetFilter += null;
            LastCall.IgnoreArguments();
            setFilterEvent = LastCall.GetEventRaiser();

            mockAdapter.GetReportTypes += null;
            LastCall.IgnoreArguments();
            getReportTypesEvent = LastCall.GetEventRaiser();

            mockAdapter.SaveReportAs += null;
            LastCall.IgnoreArguments();
            saveReportAsEvent = LastCall.GetEventRaiser();

            mockAdapter.GetTestFrameworks += null;
            LastCall.IgnoreArguments();
            getTestFrameworksEvent = LastCall.GetEventRaiser();

            mockAdapter.GetExecutionLog += null;
            LastCall.IgnoreArguments();
            getExecutionLogEvent = LastCall.GetEventRaiser();
            
            mockAdapter.UnloadTestPackage += null;
            LastCall.IgnoreArguments();
            unloadTestPackageEvent = LastCall.GetEventRaiser();

            mockModel.ProgressUpdate += null;
            LastCall.IgnoreArguments();
            progressUpdateEvent = LastCall.GetEventRaiser();

            mockModel.TestStepFinished += null;
            LastCall.IgnoreArguments();
            testStepFinishedEvent = LastCall.GetEventRaiser();
        }

        [Test]
        public void GetTestTree_Test_ShadowCopyEnabled()
        {
            TestPackageConfig testPackageConfig = new TestPackageConfig();
            TestModelData testModelData = new TestModelData(new TestData("test", "test", "test"));
            mockModel.Load(testPackageConfig);
            Expect.Call(mockModel.Explore()).Return(testModelData);
            mockAdapter.TestModelData = testModelData;
            mockAdapter.DataBind();

            mocks.ReplayAll();

            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            getTestTreeEvent.Raise(mockAdapter, new GetTestTreeEventArgs(true, testPackageConfig));
        }

        [Test]
        public void GetTestTree_Test_NoShadowCopy()
        {
            TestPackageConfig testPackageConfig = new TestPackageConfig();
            TestModelData testModelData = new TestModelData(new TestData("test", "test", "test"));
            mockModel.Load(testPackageConfig);
            Expect.Call(mockModel.Explore()).Return(testModelData);
            mockAdapter.TestModelData = testModelData;
            mockAdapter.DataBind();
            mockModel.Unload();
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            getTestTreeEvent.Raise(mockAdapter, new GetTestTreeEventArgs(false, testPackageConfig));
        }

        [Test]
        public void Run_Test_NoShadowCopy()
        {
            mockModel.Unload();
            mockModel.Load(null);
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.Explore()).Return(new TestModelData(new TestData("test", "test", "test")));
            mockModel.Run();

            mocks.ReplayAll();

            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            runTestsEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void Run_Test_ShadowCopyEnabled()
        {
            mockModel.Unload();
            mockModel.Load(null);
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.Explore()).Return(new TestModelData(new TestData("test", "test", "test")));
            mockModel.Run();

            mocks.ReplayAll();

            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            runTestsEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void GenerateReport_Test()
        {
            Expect.Call(mockModel.GenerateReport()).Return("test");
            mockAdapter.ReportPath = "test";
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            generateReportEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void CancelOperation_Test()
        {
            mockModel.CancelOperation();
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            cancelOperationEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void SetFilter_Test()
        {
            Filter<ITest> filter = new NoneFilter<ITest>();
            mockModel.SetFilter(filter);
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            setFilterEvent.Raise(mockAdapter, new SingleEventArgs<Filter<ITest>>(filter));
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
        public void GetTestFrameworks_Test()
        {
            IList<string> frameworks = new List<string>();
            Expect.Call(mockModel.GetTestFrameworks()).Return(frameworks);
            mockAdapter.TestFrameworks = frameworks;
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            getTestFrameworksEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void GetExecutionLog_Test()
        {
            List<string> testIds = new List<string>() { "test" };
            TestModelData testModelData = new TestModelData(new TestData("test", "test", "test"));
            MemoryStream memoryStream = new MemoryStream();
            Expect.Call(mockAdapter.TestModelData).Return(testModelData);
            Expect.Call(mockModel.GetExecutionLog(testIds, testModelData)).Return(memoryStream);
            mockAdapter.ExecutionLog = memoryStream;
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            getExecutionLogEvent.Raise(mockAdapter, new SingleEventArgs<IList<string>>(testIds));
        }

        [Test]
        public void Unload_Test()
        {
            TestPackageConfig testPackageConfig = new TestPackageConfig();
            TestModelData testModelData = new TestModelData(new TestData("test", "test", "test"));
            mockModel.Load(testPackageConfig);
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.Explore()).Return(testModelData);
            mockModel.Unload();
            mockAdapter.TestModelData = testModelData;
            mockAdapter.DataBind();

            mocks.ReplayAll();

            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            getTestTreeEvent.Raise(mockAdapter, new GetTestTreeEventArgs(false, testPackageConfig));
            unloadTestPackageEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void UnloadTestPackage_Package_Not_Loaded_Test()
        {
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            unloadTestPackageEvent.Raise(mockAdapter, EventArgs.Empty);
        }

        [Test]
        public void ProgressUpdate_Test()
        {
            ProgressUpdateEventArgs e = new ProgressUpdateEventArgs("taskName", "subTaskName", 0, 0);
            mockAdapter.TaskName = e.TaskName;
            mockAdapter.SubTaskName = e.SubTaskName;
            mockAdapter.CompletedWorkUnits = e.CompletedWorkUnits;
            mockAdapter.TotalWorkUnits = e.TotalWorkUnits;
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            progressUpdateEvent.Raise(mockAdapter, e);
        }

        [Test]
        public void TestStepFinished_Test()
        {
            TestData testData =  new TestData("id", "name", "fullName");
            TestStepRun testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testId"));
            TestStepFinishedEventArgs e = new TestStepFinishedEventArgs(new Report(), testData, testStepRun);
            mockAdapter.Update(testData, testStepRun);
            mocks.ReplayAll();
            projectPresenter = new ProjectPresenter(mockAdapter, mockModel);
            testStepFinishedEvent.Raise(mockAdapter, e);
        }
    }
}
