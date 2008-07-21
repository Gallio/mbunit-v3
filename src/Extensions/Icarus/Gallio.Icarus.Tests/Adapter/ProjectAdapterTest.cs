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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Interfaces;
using Gallio.Icarus.Tests;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner.Projects;
using Gallio.Runner.Reports;
using Gallio.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Gallio.Icarus.Controls.Interfaces;

namespace Gallio.Icarus.Adapter.Tests
{
    [TestFixture, Category("Adapter")]
    public class ProjectAdapterTest : MockTest
    {
        private IProjectAdapterView mockView;
        private IProjectAdapterModel mockModel;
        private IProjectPresenter mockPresenter;
        private IProjectTreeModel projectTreeModel;
        private Project project;
        private ProjectAdapter projectAdapter;

        private IEventRaiser addAssembliesEvent;
        private IEventRaiser removeAssembliesEvent;
        private IEventRaiser removeAssemblyEvent;
        private IEventRaiser getTestTreeEvent;
        private IEventRaiser runTestsEvent;
        private IEventRaiser generateReportEvent;
        private IEventRaiser cancelOperationEvent;
        private IEventRaiser saveFilterEvent;
        private IEventRaiser deleteFilterEvent;
        private IEventRaiser getReportTypesEvent;
        private IEventRaiser saveReportAsEvent;
        private IEventRaiser saveProjectEvent;
        private IEventRaiser openProjectEvent;
        private IEventRaiser newProjectEvent;
        private IEventRaiser getTestFrameworksEvent;
        private IEventRaiser getSourceLocationEvent;
        private IEventRaiser updateHintDirectoriesEvent;
        private IEventRaiser updateApplicationBaseDirectoryEvent;
        private IEventRaiser updateWorkingDirectoryEvent;
        private IEventRaiser updateShadowCopyEvent;
        private IEventRaiser resetTestStatusEvent;
        private IEventRaiser applyFilterEvent;
        private IEventRaiser getExecutionLogEvent;
        private IEventRaiser unloadTestPackageEvent;

        [SetUp]
        public void SetUp()
        {
            mockView = mocks.CreateMock<IProjectAdapterView>();
            mockModel = mocks.CreateMock<IProjectAdapterModel>();

            projectTreeModel = mocks.CreateMock<IProjectTreeModel>();
            project = new Project();
            Expect.Call(projectTreeModel.Project).Return(project).Repeat.AtLeastOnce();

            mockView.ProjectTreeModel = projectTreeModel;
            LastCall.IgnoreArguments();
            mockView.ShadowCopy = true;
            
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

            mockView.CancelOperation += null;
            LastCall.IgnoreArguments();
            cancelOperationEvent = LastCall.GetEventRaiser();

            mockView.SaveFilter += null;
            LastCall.IgnoreArguments();
            saveFilterEvent = LastCall.GetEventRaiser();

            mockView.DeleteFilter += null;
            LastCall.IgnoreArguments();
            deleteFilterEvent = LastCall.GetEventRaiser();

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

            mockView.UpdateHintDirectoriesEvent += null;
            LastCall.IgnoreArguments();
            updateHintDirectoriesEvent = LastCall.GetEventRaiser();

            mockView.UpdateApplicationBaseDirectoryEvent += null;
            LastCall.IgnoreArguments();
            updateApplicationBaseDirectoryEvent = LastCall.GetEventRaiser();

            mockView.UpdateWorkingDirectoryEvent += null;
            LastCall.IgnoreArguments();
            updateWorkingDirectoryEvent = LastCall.GetEventRaiser();

            mockView.UpdateShadowCopyEvent += null;
            LastCall.IgnoreArguments();
            updateShadowCopyEvent = LastCall.GetEventRaiser();

            mockView.ResetTestStatus += null;
            LastCall.IgnoreArguments();
            resetTestStatusEvent = LastCall.GetEventRaiser();

            mockView.ApplyFilter += null;
            LastCall.IgnoreArguments();
            applyFilterEvent = LastCall.GetEventRaiser();

            mockView.GetExecutionLog += null;
            LastCall.IgnoreArguments();
            getExecutionLogEvent = LastCall.GetEventRaiser();

            mockView.UnloadTestPackage += null;
            LastCall.IgnoreArguments();
            unloadTestPackageEvent = LastCall.GetEventRaiser();

            TestTreeModel treeModel = new TestTreeModel();
            Expect.Call(mockModel.TreeModel).Return(treeModel);
            mockView.TestTreeModel = treeModel;
        }

        [Test]
        public void AddAssembliesEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            addAssembliesEvent.Raise(mockView, new SingleEventArgs<IList<string>>(new string[] { "test.dll" }));
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Contains("test.dll"));
        }

        [Test]
        public void RemoveAssembliesEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.Project.TestPackageConfig.AssemblyFiles.Add("test.dll");
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count == 1);
            removeAssembliesEvent.Raise(mockView, EventArgs.Empty);
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count == 0);
        }

        [Test]
        public void Remove_Assembly_By_Id_Test()
        {
            TestData testData = new TestData("test", "test", "test");
            testData.Metadata.SetValue(MetadataKeys.CodeBase, "test.dll");
            TestModelData testModelData = new TestModelData(testData);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.TestModelData = testModelData;
            projectAdapter.Project.TestPackageConfig.AssemblyFiles.Add("test.dll");
            Assert.AreEqual(1, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
            removeAssemblyEvent.Raise(mockView, new SingleEventArgs<string>("test2.dll"));
            Assert.AreEqual(1, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
            removeAssemblyEvent.Raise(mockView, new SingleEventArgs<string>("test"));
            Assert.AreEqual(0, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
        }

        [Test]
        public void Remove_Assembly_By_File_Name_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.Project.TestPackageConfig.AssemblyFiles.Add("test.dll");
            Assert.AreEqual(1, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
            removeAssemblyEvent.Raise(mockView, new SingleEventArgs<string>("test2.dll"));
            Assert.AreEqual(1, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
            removeAssemblyEvent.Raise(mockView, new SingleEventArgs<string>("test.dll"));
            Assert.AreEqual(0, projectAdapter.Project.TestPackageConfig.AssemblyFiles.Count);
        }

        [Test]
        public void GetTestTree_Test_Reload()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.projectAdapter_GetTestTree(null, null);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.GetTestTree += new EventHandler<GetTestTreeEventArgs>(mockPresenter.projectAdapter_GetTestTree);
            getTestTreeEvent.Raise(mockView, new GetTestTreeEventArgs("mode", true));
        }

        [Test]
        public void GetTestTree_Test_NoReload()
        {
            string mode = "test";
            TestData testData = new TestData("id", "name", "fullname");
            TestModelData testModelData = new TestModelData(testData);
            ListViewItem[] assemblies = new ListViewItem[] { };
            LastCall.IgnoreArguments();
            mockModel.BuildTestTree(testModelData, mode);
            mockView.Annotations = testModelData.Annotations;
            mockView.LoadComplete();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.TestModelData = testModelData;
            getTestTreeEvent.Raise(mockView, new GetTestTreeEventArgs(mode, false));
        }

        [Test]
        public void RunTestsEventHandler_Test()
        {
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            mockPresenter.projectAdapter_RunTests(projectAdapter, new SingleEventArgs<bool>(false));
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.RunTests += new EventHandler<EventArgs>(mockPresenter.projectAdapter_RunTests);
            runTestsEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void GenerateReport_Test()
        {
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            mockPresenter.projectAdapter_GenerateReport(projectAdapter, EventArgs.Empty);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.GenerateReport += new EventHandler<EventArgs>(mockPresenter.projectAdapter_GenerateReport);
            generateReportEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void CancelOperationEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.projectAdapter_CancelOperation(projectAdapter, EventArgs.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.CancelOperation += new EventHandler<EventArgs>(mockPresenter.projectAdapter_CancelOperation);
            cancelOperationEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void GetReportTypesEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.projectAdapter_GetReportTypes(projectAdapter, EventArgs.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.GetReportTypes += new EventHandler<EventArgs>(mockPresenter.projectAdapter_GetReportTypes);
            getReportTypesEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void SaveReportAsEventHandler_Test()
        {
            string fileName = @"C:\test.html";
            string format = "html";
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.projectAdapter_SaveReportAs(projectAdapter, new SaveReportAsEventArgs(fileName, format));
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.SaveReportAs += new EventHandler<SaveReportAsEventArgs>(mockPresenter.projectAdapter_SaveReportAs);
            saveReportAsEvent.Raise(mockView, new SaveReportAsEventArgs(fileName, format));
        }

        [Test]
        public void SaveProjectEventHandler_Test()
        {
            string fileName = Path.GetTempFileName();
            projectTreeModel.SaveProject(fileName);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            saveProjectEvent.Raise(mockView, new SingleEventArgs<string>(fileName));
        }

        [Test]
        public void OpenProjectEventHandler_Test()
        {
            string assembly = Assembly.GetExecutingAssembly().CodeBase;
            project.TestPackageConfig.AssemblyFiles.AddRange(new string[] { assembly, assembly });
            string fileName = Path.GetTempFileName();
            projectTreeModel.LoadProject(fileName);
            mockView.HintDirectories = null;
            LastCall.IgnoreArguments();
            mockView.ApplicationBaseDirectory = null;
            LastCall.IgnoreArguments();
            mockView.ShadowCopy = true;
            LastCall.IgnoreArguments();
            mockView.WorkingDirectory = null;
            LastCall.IgnoreArguments();
            mockView.TestFilters = null;
            LastCall.IgnoreArguments();
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            string mode = "Namespace";
            mockPresenter.projectAdapter_GetTestTree(projectAdapter, new GetTestTreeEventArgs(true, new TestPackageConfig()));
            LastCall.IgnoreArguments();
            mockView.EditEnabled = false;
            mockView.EditEnabled = true;

            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.GetTestTree += new EventHandler<GetTestTreeEventArgs>(mockPresenter.projectAdapter_GetTestTree);
            projectAdapter.SetFilter += new EventHandler<SingleEventArgs<Filter<ITest>>>(mockPresenter.projectAdapter_SetFilter);
            openProjectEvent.Raise(mockView, new OpenProjectEventArgs(fileName, mode));
        }

        [Test, ExpectedException(typeof(ArgumentException)), Ignore("Throws wrong exception!")]
        public void OpenProjectEventHandlerProjectDoesNotExist_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            string fileName = Path.GetTempFileName();
            openProjectEvent.Raise(mockView, new OpenProjectEventArgs(fileName, "Namespace"));
        }

        [Test]
        public void UpdateHintDirectoriesEventHandler_Test()
        {
            List<string> list = new List<string>();
            list.Add("test");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            Assert.AreEqual(0, projectAdapter.Project.TestPackageConfig.HintDirectories.Count);
            updateHintDirectoriesEvent.Raise(mockView, new SingleEventArgs<IList<string>>(list));
            Assert.AreEqual(1, projectAdapter.Project.TestPackageConfig.HintDirectories.Count);
            Assert.AreEqual("test", projectAdapter.Project.TestPackageConfig.HintDirectories[0]);
        }

        [Test]
        public void UpdateApplicationBaseDirectoryEventHandler_Test()
        {
            string applicationBaseDirectory = "test";
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            Assert.IsNull(projectAdapter.Project.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
            updateApplicationBaseDirectoryEvent.Raise(mockView, new SingleEventArgs<string>(applicationBaseDirectory));
            Assert.AreEqual(applicationBaseDirectory, projectAdapter.Project.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
        }

        [Test]
        public void UpdateWorkingDirectoryEventHandler_Test()
        {
            string workingDirectory = "test";
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            Assert.IsNull(projectAdapter.Project.TestPackageConfig.HostSetup.WorkingDirectory);
            updateWorkingDirectoryEvent.Raise(mockView, new SingleEventArgs<string>(workingDirectory));
            Assert.AreEqual(workingDirectory, projectAdapter.Project.TestPackageConfig.HostSetup.WorkingDirectory);
        }

        [Test]
        public void UpdateShadowCopyEventHandler_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            Assert.IsTrue(projectAdapter.Project.TestPackageConfig.HostSetup.ShadowCopy);
            updateShadowCopyEvent.Raise(mockView, new SingleEventArgs<bool>(false));
            Assert.IsFalse(projectAdapter.Project.TestPackageConfig.HostSetup.ShadowCopy);
        }

        [Test]
        public void NewProjectEventHandler_Test()
        {
            projectTreeModel.NewProject();
            mockView.HintDirectories = null;
            LastCall.IgnoreArguments();
            mockView.ApplicationBaseDirectory = null;
            LastCall.IgnoreArguments();
            mockView.ShadowCopy = true;
            LastCall.IgnoreArguments();
            mockView.WorkingDirectory = null;
            LastCall.IgnoreArguments();
            mockView.TestFilters = null;
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            newProjectEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void GetTestFrameworks_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.projectAdapter_GetTestFrameworks(projectAdapter, EventArgs.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.GetTestFrameworks += new EventHandler<EventArgs>(mockPresenter.projectAdapter_GetTestFrameworks);
            getTestFrameworksEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void GetSourceLocation_Test()
        {
            CodeLocation codeLocation = new CodeLocation("path", 1, 1);
            TestData testData = new TestData("id", "name", "fullname");
            testData.CodeLocation = codeLocation;
            TestModelData testModelData = new TestModelData(testData);
            mockView.SourceCodeLocation = codeLocation;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.TestModelData = testModelData;
            getSourceLocationEvent.Raise(mockView, new SingleEventArgs<string>("id"));
        }

        [Test]
        public void TaskName_Test()
        {
            mockView.TaskName = "taskName";
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.TaskName = "taskName";
        }

        [Test]
        public void SubTaskName_Test()
        {
            mockView.SubTaskName = "subTaskName";
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.SubTaskName = "subTaskName";
        }

        [Test]
        public void CompletedWorkUnits_Test()
        {
            mockView.CompletedWorkUnits = 2;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.CompletedWorkUnits = 2;
        }

        [Test]
        public void TotalWorkUnits_Test()
        {
            mockView.TotalWorkUnits = 5;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.TotalWorkUnits = 5;
        }

        [Test]
        public void ReportPath_Test()
        {
            string reportPath = @"C:\somepath.html";
            mockView.ReportPath = reportPath;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.ReportPath = reportPath;
        }

        [Test]
        public void ReportTypes_Test()
        {
            List<string> reportTypes = new List<string>();
            mockView.ReportTypes = reportTypes;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.ReportTypes = reportTypes;
        }

        [Test]
        public void TestFrameworks_Test()
        {
            List<string> frameworks = new List<string>();
            mockView.TestFrameworks = frameworks;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.TestFrameworks = frameworks;
        }

        [Test]
        public void Update_Test()
        {
            TestData testData = new TestData("test", "test", "test");
            TestStepRun testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "test"));
            mockModel.Update(testData, testStepRun);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.Update(testData, testStepRun);
        }

        [Test]
        public void UpdateProjectFilter_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.Project.TestFilters.Add(new FilterInfo("test", new NoneFilter<ITest>().ToFilterExpr()));
            Assert.AreEqual(1, projectAdapter.Project.TestFilters.Count);
            projectAdapter.UpdateProjectFilter("test", new IdFilter<ITest>(new EqualityFilter<string>("test")));
            Assert.AreEqual(1, projectAdapter.Project.TestFilters.Count);
        }

        [Test]
        public void OnResetTestStatus_Test()
        {
            mockModel.ResetTestStatus();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            resetTestStatusEvent.Raise(projectAdapter, EventArgs.Empty);
        }

        [Test]
        public void ApplyFilter_Test()
        {
            bool flag = false;
            mockView.EditEnabled = false;
            Filter<ITest> filter = new NoneFilter<ITest>();
            mockModel.ApplyFilter(filter);
            LastCall.IgnoreArguments();
            mockView.EditEnabled = true;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.SetFilter += delegate { flag = true; };
            Assert.AreEqual(0, projectAdapter.Project.TestFilters.Count);
            projectAdapter.Project.TestFilters.Add(new FilterInfo("test", filter.ToFilterExpr()));
            Assert.AreEqual(1, projectAdapter.Project.TestFilters.Count);
            applyFilterEvent.Raise(projectAdapter, new SingleEventArgs<string>("test"));
            Assert.IsTrue(flag);
        }

        [Test]
        public void DeleteFilter_Test()
        {
            Filter<ITest> filter = new NoneFilter<ITest>();
            mockView.TestFilters = new List<string>();
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            Assert.AreEqual(0, projectAdapter.Project.TestFilters.Count);
            projectAdapter.Project.TestFilters.Add(new FilterInfo("test", filter.ToFilterExpr()));
            Assert.AreEqual(1, projectAdapter.Project.TestFilters.Count);
            deleteFilterEvent.Raise(projectAdapter, new SingleEventArgs<string>("test"));
            Assert.AreEqual(0, projectAdapter.Project.TestFilters.Count);
        }

        [Test]
        public void GetExecutionLog_Test()
        {
            SingleEventArgs<IList<string>> e = new SingleEventArgs<IList<string>>(new List<string>() { "test" });
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.projectAdapter_GetExecutionLog(projectAdapter, e);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.GetExecutionLog += new EventHandler<SingleEventArgs<IList<string>>>(mockPresenter.projectAdapter_GetExecutionLog);
            getExecutionLogEvent.Raise(mockView, e);
        }

        [Test]
        public void UnloadTestPackage_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.projectAdapter_UnloadTestPackage(projectAdapter, EventArgs.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.UnloadTestPackage += new EventHandler<EventArgs>(mockPresenter.projectAdapter_UnloadTestPackage);
            unloadTestPackageEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void ExecutionLog_Test()
        {
            MemoryStream stream = new MemoryStream();
            mockView.ExecutionLog = stream;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.ExecutionLog = stream;
        }

        [Test]
        public void SaveFilter_Test()
        {
            bool flag = false;
            Filter<ITest> filter = new NoneFilter<ITest>();
            Expect.Call(mockModel.CreateFilter()).Return(filter);
            mockView.TestFilters = new List<string>();
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.SetFilter += delegate { flag = true; };
            saveFilterEvent.Raise(mockView, new SingleEventArgs<string>("test"));
            Assert.AreEqual(1, projectAdapter.Project.TestFilters.Count);
            Assert.IsTrue(flag);
        }

        [Test]
        public void TestModelData_Test()
        {
            mocks.ReplayAll();
            TestData testData = new TestData("id", "name", "fullname");
            TestModelData testModelData = new TestModelData(testData);
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.TestModelData = testModelData;
            Assert.AreEqual(testModelData, projectAdapter.TestModelData);
        }

        [Test]
        public void AssemblyChangedEvent_Test()
        {
            string path = @"\a\file\path";
            mockView.AssemblyChanged(path);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, projectTreeModel);
            projectAdapter.assemblyWatcher_AssemblyChangedEvent(path);
        }
    }
}
