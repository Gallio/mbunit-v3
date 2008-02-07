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
using System.IO;
using System.Windows.Forms;

using Gallio.Icarus.Adapter;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.Presenter;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner.Projects;
using Gallio.Runner.Reports;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Tests
{
    [TestFixture]
    public class ProjectAdapterTest : MockTest
    {
        private IProjectAdapterView mockView;
        private IProjectAdapterModel mockModel;
        private IProjectPresenter mockPresenter;
        private ProjectAdapter projectAdapter;

        private IEventRaiser addAssembliesEvent;
        private IEventRaiser removeAssembliesEvent;
        private IEventRaiser removeAssemblyEvent;
        private IEventRaiser getTestTreeEvent;
        private IEventRaiser runTestsEvent;
        private IEventRaiser generateReportEvent;
        private IEventRaiser stopTestsEvent;
        private IEventRaiser setFilterEvent;
        private IEventRaiser getReportTypesEvent;
        private IEventRaiser saveReportAsEvent;
        private IEventRaiser saveProjectEvent;
        private IEventRaiser openProjectEvent;
        private IEventRaiser newProjectEvent;
        private IEventRaiser getTestFrameworksEvent;
        private IEventRaiser getSourceLocationEvent;

        [SetUp]
        public void SetUp()
        {
            mockView = mocks.CreateMock<IProjectAdapterView>();
            mockModel = mocks.CreateMock<IProjectAdapterModel>();
            
            mockView.AddAssemblies += null;
            LastCall.IgnoreArguments();
            addAssembliesEvent = LastCall.GetEventRaiser();
            
            mockView.RemoveAssemblies += null;
            LastCall.IgnoreArguments();
            removeAssembliesEvent = LastCall.GetEventRaiser();

            mockView.RemoveAssembly += null;
            LastCall.IgnoreArguments();
            removeAssemblyEvent = LastCall.GetEventRaiser();

            mockView.GetTestTree += null;
            LastCall.IgnoreArguments();
            getTestTreeEvent = LastCall.GetEventRaiser();

            mockView.RunTests += null;
            LastCall.IgnoreArguments();
            runTestsEvent = LastCall.GetEventRaiser();

            mockView.GenerateReport += null;
            LastCall.IgnoreArguments();
            generateReportEvent = LastCall.GetEventRaiser();

            mockView.StopTests += null;
            LastCall.IgnoreArguments();
            stopTestsEvent = LastCall.GetEventRaiser();

            mockView.SetFilter += null;
            LastCall.IgnoreArguments();
            setFilterEvent = LastCall.GetEventRaiser();

            mockView.GetReportTypes += null;
            LastCall.IgnoreArguments();
            getReportTypesEvent = LastCall.GetEventRaiser();

            mockView.SaveReportAs += null;
            LastCall.IgnoreArguments();
            saveReportAsEvent = LastCall.GetEventRaiser();

            mockView.SaveProject += null;
            LastCall.IgnoreArguments();
            saveProjectEvent = LastCall.GetEventRaiser();

            mockView.OpenProject += null;
            LastCall.IgnoreArguments();
            openProjectEvent = LastCall.GetEventRaiser();

            mockView.NewProject += null;
            LastCall.IgnoreArguments();
            newProjectEvent = LastCall.GetEventRaiser();

            mockView.GetTestFrameworks += null;
            LastCall.IgnoreArguments();
            getTestFrameworksEvent = LastCall.GetEventRaiser();

            mockView.GetSourceLocation += null;
            LastCall.IgnoreArguments();
            getSourceLocationEvent = LastCall.GetEventRaiser();
        }

        [Test]
        public void AddAssembliesEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel);
            addAssembliesEvent.Raise(mockView, new AddAssembliesEventArgs(new string[] { "test.dll" }));
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Contains("test.dll"));
        }

        [Test]
        public void RemoveAssembliesEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.Project.TestPackageConfig.AssemblyFiles.Add("test.dll");
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count == 1);
            removeAssembliesEvent.Raise(mockView, EventArgs.Empty);
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count == 0);
        }

        [Test]
        public void RemoveAssemblyEventHandler_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            TestData testData = new TestData("test", "test");
            testData.Metadata.SetValue(MetadataKeys.CodeBase, "test3.dll");
            projectAdapter.TestModelData = new TestModelData(testData);
            projectAdapter.Project.TestPackageConfig.AssemblyFiles.AddRange(new string[] { "test.dll", "test3.dll" });
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count == 2);
            removeAssemblyEvent.Raise(mockView, new SingleStringEventArgs("test2.dll"));
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count == 2);
            removeAssemblyEvent.Raise(mockView, new SingleStringEventArgs("test.dll"));
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count == 1);
            removeAssemblyEvent.Raise(mockView, new SingleStringEventArgs("test"));
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count == 0);
        }

        [Test]
        public void GetTestTreeEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.GetTestTree(projectAdapter, new GetTestTreeEventArgs("mode", true, true, projectAdapter.Project.TestPackageConfig));
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.GetTestTree += new EventHandler<GetTestTreeEventArgs>(mockPresenter.GetTestTree);
            getTestTreeEvent.Raise(mockView, new GetTestTreeEventArgs("mode", true));
        }

        [Test]
        public void RunTestsEventHandler_Test()
        {
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            mockPresenter.RunTests(projectAdapter, EventArgs.Empty);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.RunTests += new EventHandler<EventArgs>(mockPresenter.RunTests);
            runTestsEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void GenerateReport_Test()
        {
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            mockPresenter.OnGenerateReport(projectAdapter, EventArgs.Empty);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.GenerateReport += new EventHandler<EventArgs>(mockPresenter.OnGenerateReport);
            generateReportEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void StopTestsEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.StopTests(projectAdapter, EventArgs.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.StopTests += new EventHandler<EventArgs>(mockPresenter.StopTests);
            stopTestsEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void SetFilterEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.SetFilter(projectAdapter, new SetFilterEventArgs("test", new NoneFilter<ITest>()));
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.GetFilter(null)).Return(null);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.SetFilter += new EventHandler<SetFilterEventArgs>(mockPresenter.SetFilter);
            setFilterEvent.Raise(mockView, new SetFilterEventArgs("test", (TreeNodeCollection)null));
        }

        [Test]
        public void GetReportTypesEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.GetReportTypes(projectAdapter, EventArgs.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.GetReportTypes += new EventHandler<EventArgs>(mockPresenter.GetReportTypes);
            getReportTypesEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void SaveReportAsEventHandler_Test()
        {
            string fileName = @"C:\test.html";
            string format = "html";
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.SaveReportAs(projectAdapter, new SaveReportAsEventArgs(fileName, format));
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.SaveReportAs += new EventHandler<SaveReportAsEventArgs>(mockPresenter.SaveReportAs);
            saveReportAsEvent.Raise(mockView, new SaveReportAsEventArgs(fileName, format));
        }

        [Test]
        public void SaveProjectEventHandler_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.Project.TestPackageConfig.AssemblyFiles.Add("test.dll");
            string fileName = Path.GetTempFileName();
            saveProjectEvent.Raise(mockView, new SingleStringEventArgs(fileName));
            Project project = SerializationUtils.LoadFromXml<Project>(fileName);
            Assert.AreEqual(1, project.TestPackageConfig.AssemblyFiles.Count);
            File.Delete(fileName);
        }

        [Test]
        public void OpenProjectEventHandler_Test()
        {
            IdFilter<ITest> idFilter = new IdFilter<ITest>(new EqualityFilter<string>("test"));
            string mode = "Namespace";
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            GetTestTreeEventArgs e = new GetTestTreeEventArgs(mode, true, false, new TestPackageConfig());
            mockPresenter.GetTestTree(projectAdapter, e);
            LastCall.IgnoreArguments();
            mockView.ApplyFilter(idFilter);
            LastCall.IgnoreArguments();
            mockPresenter.SetFilter(projectAdapter, new SetFilterEventArgs("Latest", idFilter));
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            Project project = new Project();
            project.TestPackageConfig.AssemblyFiles.Add("test.dll");
            project.TestFilters.Add(new FilterInfo("Latest", idFilter.ToFilterExpr()));
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.GetTestTree += new EventHandler<GetTestTreeEventArgs>(mockPresenter.GetTestTree);
            projectAdapter.SetFilter += new EventHandler<SetFilterEventArgs>(mockPresenter.SetFilter);
            string fileName = Path.GetTempFileName();
            SerializationUtils.SaveToXml(project, fileName);
            Assert.AreEqual(0, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
            openProjectEvent.Raise(mockView, new OpenProjectEventArgs(fileName, mode));
            Assert.AreEqual(1, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
        }

        //[Test, ExpectedException(typeof(InvalidOperationException))]
        //public void OpenProjectEventHandlerProjectDoesNotExist_Test()
        //{
        //    mocks.ReplayAll();
        //    projectAdapter = new ProjectAdapter(mockView, mockModel);
        //    string fileName = Path.GetTempFileName();
        //    openProjectEvent.Raise(mockView, new OpenProjectEventArgs(fileName, "Namespace"));
        //}

        [Test]
        public void NewProjectEventHandler_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.Project.TestPackageConfig.AssemblyFiles.Add("test.dll");
            Assert.AreEqual(1, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
            newProjectEvent.Raise(mockView, EventArgs.Empty);
            Assert.AreEqual(0, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
        }

        [Test]
        public void GetTestFrameworks_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.OnGetTestFrameworks(projectAdapter, EventArgs.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.GetTestFrameworks += new EventHandler<EventArgs>(mockPresenter.OnGetTestFrameworks);
            getTestFrameworksEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void GetSourceLocation_Test()
        {
            CodeLocation codeLocation = new CodeLocation("path", 1, 1);
            TestData testData = new TestData("test", "test");
            testData.CodeLocation = codeLocation;
            mockView.SourceCodeLocation = codeLocation;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.TestModelData = new TestModelData(testData);
            getSourceLocationEvent.Raise(mockView, new SingleStringEventArgs("test"));
        }

        [Test]
        public void StatusText_Test()
        {
            mockView.StatusText = "blah blah";
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.StatusText = "blah blah";
        }

        [Test]
        public void CompletedWorkUnits_Test()
        {
            mockView.CompletedWorkUnits = 2;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.CompletedWorkUnits = 2;
        }

        [Test]
        public void TotalWorkUnits_Test()
        {
            mockView.TotalWorkUnits = 5;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.TotalWorkUnits = 5;
        }

        [Test]
        public void ReportPath_Test()
        {
            string reportPath = @"C:\somepath.html";
            mockView.ReportPath = reportPath;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.ReportPath = reportPath;
        }

        [Test]
        public void TestModel_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            Assert.IsNull(projectAdapter.TestModelData);
            TestModelData testModelData = new TestModelData(new TestData("id", "name"));
            projectAdapter.TestModelData = testModelData;
            Assert.AreEqual(testModelData, projectAdapter.TestModelData);
        }

        [Test]
        public void Project_Test()
        {
            mocks.ReplayAll();
            Project project = new Project();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.Project = project;
            Assert.AreEqual(project, projectAdapter.Project);
        }

        [Test]
        public void ReportTypes_Test()
        {
            List<string> reportTypes = new List<string>();
            mockView.ReportTypes = reportTypes;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.ReportTypes = reportTypes;
        }

        [Test]
        public void TestFrameworks_Test()
        {
            List<string> frameworks = new List<string>();
            mockView.TestFrameworks = frameworks;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.TestFrameworks = frameworks;
        }

        [Test]
        public void Exception_Test()
        {
            Exception exception = new Exception("message");
            mockView.Exception = exception;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.Exception = exception;
        }

        [Test]
        public void DataBind_Test()
        {
            TestPackageConfig testPackageConfig = new TestPackageConfig();
            mockView.Assemblies = null;
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.BuildAssemblyList(testPackageConfig.AssemblyFiles)).Return(new ListViewItem[0]);
            mockView.TestTreeCollection = null;
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.BuildTestTree(null, "", true)).IgnoreArguments().Return(new TreeNode[0]);
            mockView.TotalTests = 0;
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.CountTests(null)).IgnoreArguments().Return(0);
            
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.DataBind("mode", true);
        }

        [Test]
        public void Update_Test()
        {
            TestData testData = new TestData("test1", "test1");
            TestStepRun testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testInstanceId"));
            mockView.Update(testData, testStepRun);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.Update(testData, testStepRun);
        }

        [Test]
        public void UpdateProjectFilter_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.Project.TestFilters.Add(new FilterInfo("test", new NoneFilter<ITest>().ToFilterExpr()));
            Assert.AreEqual(1, projectAdapter.Project.TestFilters.Count);
            projectAdapter.UpdateProjectFilter("test", new IdFilter<ITest>(new EqualityFilter<string>("test")));
            Assert.AreEqual(1, projectAdapter.Project.TestFilters.Count);
        }

        [Test]
        public void WriteToLog_Test()
        {
            mockView.WriteToLog("test", "test");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel);
            projectAdapter.WriteToLog("test", "test");
        }
    }
}
