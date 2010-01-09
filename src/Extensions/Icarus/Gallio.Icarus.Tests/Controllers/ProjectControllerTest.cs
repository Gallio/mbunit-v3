// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Gallio.Common.IO;
using Gallio.Common.Xml;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Remoting;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.Utilities;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using MbUnit.Framework;
using Rhino.Mocks;
using System;
using Path = System.IO.Path;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(ProjectController))]
    internal class ProjectControllerTest
    {
        [Test]
        public void AddFiles_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            TestProject testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            string fileName = Assembly.GetExecutingAssembly().Location;
            List<string> list = new List<string>(new[] { fileName });
            fileSystem.Stub(fs => fs.FileExists(fileName)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.Instance;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);
            Assert.AreEqual(0, projectController.TestPackage.Files.Count);

            projectController.AddFiles(list, progressMonitor);
            
            Assert.AreEqual(1, projectController.TestPackage.Files.Count);
            Assert.AreEqual(fileName, projectController.TestPackage.Files[0].ToString());
        }

        [Test]
        public void DeleteFilter_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            TestProject testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            var progressMonitor = MockProgressMonitor.Instance;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);
            
            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
            FilterInfo filterInfo = new FilterInfo("filterName", new NoneFilter<ITestDescriptor>().ToFilterExpr());
            projectController.TestFilters.Value.Add(filterInfo);
            Assert.AreEqual(1, projectController.TestFilters.Value.Count);
            projectController.DeleteFilter(filterInfo, progressMonitor);
            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
        }

        [Test]
        public void Model_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(projectTreeModel, projectController.Model);
        }

        [Test]
        public void TestPackage_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            TestProject testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(testProject.TestPackage, projectController.TestPackage);
        }

        [Test]
        public void TestFilters_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
        }

        [Test]
        public void HintDirectories_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(0, projectController.HintDirectories.Count);
        }

        [Test]
        public void ProjectFileName_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            const string fileName = "fileName";
            projectTreeModel.FileName = fileName;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(fileName, projectController.ProjectFileName);
        }

        [SyncTest]
        public void NewProject_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            projectController.PropertyChanged += ((sender, e) => Assert.AreEqual("TestPackage", e.PropertyName));
            projectController.NewProject(progressMonitor);
            Assert.AreEqual(Paths.DefaultProject, projectTreeModel.FileName);
            Assert.AreEqual(0, projectController.HintDirectories.Count);
            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
        }

        [Test]
        public void RemoveAllFiles_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            projectTreeModel.TestProject = new TestProject();
            var progressMonitor = MockProgressMonitor.Instance;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            projectController.RemoveAllFiles(progressMonitor);
        }

        [Test]
        public void RemoveFile_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var project = new TestProject();
            string fileName = Path.GetFullPath("test");
            project.TestPackage.AddFile(new FileInfo(fileName));
            projectTreeModel.TestProject = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.Instance;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(1, project.TestPackage.Files.Count);
            
            projectController.RemoveFile(fileName, progressMonitor);
            Assert.AreEqual(0, project.TestPackage.Files.Count);
        }

        [Test]
        public void SaveFilter_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            projectTreeModel.TestProject = new TestProject();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.Instance;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
            projectController.SaveFilterSet("filterName", new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>()), progressMonitor);
            Assert.AreEqual(1, projectController.TestFilters.Value.Count);
            projectController.SaveFilterSet("filterName", new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>()), progressMonitor);
            Assert.AreEqual(1, projectController.TestFilters.Value.Count);
            projectController.SaveFilterSet("aDifferentFilterName", new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>()), progressMonitor);
            Assert.AreEqual(2, projectController.TestFilters.Value.Count);
        }

        [Test]
        public void SaveProject_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            TestProject testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.Instance;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var mruList = new MRUList(new List<string>(), 10);
            optionsController.Stub(oc => oc.RecentProjects).Return(mruList);
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            const string projectName = "projectName";
            projectController.SaveProject(projectName, progressMonitor);
            xmlSerializer.AssertWasCalled(x => x.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(projectName + UserOptions.Extension)));
            Assert.AreEqual(1, mruList.Count);
            Assert.IsTrue(mruList.Items.Contains(projectName));
        }

        [Test]
        public void SaveDefaultProject_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            TestProject testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            projectTreeModel.FileName = Paths.DefaultProject;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.Instance;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            optionsController.Stub(oc => oc.RecentProjects).Return(new MRUList(new List<string>(), 10));
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            projectController.SaveProject(string.Empty, progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.DirectoryExists(Paths.IcarusAppDataFolder));
            fileSystem.AssertWasCalled(fs => fs.CreateDirectory(Paths.IcarusAppDataFolder));
            xmlSerializer.AssertWasCalled(xs => xs.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(Paths.DefaultProject + UserOptions.Extension)));
        }

        [Test]
        public void Updating_HintDirectories_cascades_to_TestPackage()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var project = new TestProject();
            projectTreeModel.TestProject = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);
            Assert.AreEqual(0, project.TestPackage.HintDirectories.Count);
            
            const string hintDirectory = "test";
            projectController.HintDirectories.Add(hintDirectory);
            
            Assert.AreEqual(1, project.TestPackage.HintDirectories.Count);
            Assert.AreEqual(hintDirectory, project.TestPackage.HintDirectories[0].ToString());
        }

        [Test]
        public void Updating_TestRunnerExtensions_cascades_to_TestPackage()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var project = new TestProject();
            projectTreeModel.TestProject = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);
            Assert.AreEqual(0, project.TestRunnerExtensionSpecifications.Count);

            const string testRunnerExtension = "test";
            projectController.TestRunnerExtensions.Add(testRunnerExtension);

            Assert.AreEqual(1, project.TestRunnerExtensionSpecifications.Count);
            Assert.AreEqual(testRunnerExtension, project.TestRunnerExtensionSpecifications[0]);
        }

        [Test]
        public void FileWatcher_fires_FileChanged_event()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            var changedFlag = false;
            const string fileName = "test";
            projectController.FileChanged += delegate(object sender, FileChangedEventArgs e)
            {
                changedFlag = true;
                Assert.AreEqual(fileName, e.FileName);
            };
            fileWatcher.Raise(aw => aw.FileChangedEvent += null, new object[] { fileName });
            Assert.AreEqual(true, changedFlag);
        }

        [SyncTest]
        public void OpenProject_Test()
        {
            string projectName = Path.GetFullPath("projectName");
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var project = new TestProjectData { ReportNameFormat = "foo" };
            project.TestFilters.Add(new FilterInfo("None", new NoneFilter<ITestDescriptor>().ToFilterExpr()));
            project.TestPackage.HintDirectories.Add("hintDirectory");
            project.TestRunnerExtensions.Add("testRunnerExtensions");
            xmlSerializer.Stub(xs => xs.LoadFromXml<TestProjectData>(projectName)).Return(project);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var progressMonitor = MockProgressMonitor.Instance;
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);
            var propertyChangedFlag = false;
            projectController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != "TestPackage")
                    return;

                propertyChangedFlag = true;
            };

            projectController.OpenProject(projectName, progressMonitor);

            Assert.AreEqual(projectName, projectTreeModel.FileName);
            Assert.AreEqual(project.ReportNameFormat, projectTreeModel.TestProject.ReportNameFormat);

            Assert.AreEqual(1, projectController.TestFilters.Value.Count);
            Assert.AreEqual(1, projectController.HintDirectories.Count);
            Assert.AreEqual(1, projectController.TestRunnerExtensions.Count);
            Assert.AreEqual(true, propertyChangedFlag);
        }

        [Test]
        public void OpenProject_should_fail_if_loading_project_throws_exception()
        {
            string projectName = Path.GetFullPath("projectName");
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var exception = new Exception();
            xmlSerializer.Stub(xs => xs.LoadFromXml<TestProjectData>(projectName)).Throw(exception);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var progressMonitor = MockProgressMonitor.Instance;
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            projectController.OpenProject(projectName, progressMonitor);

            unhandledExceptionPolicy.AssertWasCalled(uep => uep.Report(Arg<string>.Is.Anything, Arg.Is(exception)));
        }

        [Test]
        public void OpenProject_should_succeed_even_if_loading_user_options_throws_exception()
        {
            string projectName = Path.GetFullPath("projectName");
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);

            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            xmlSerializer.Stub(xs => xs.LoadFromXml<TestProjectData>(projectName)).Return(new TestProjectData());
            string projectUserOptionsFile = projectName + UserOptions.Extension;
            fileSystem.Stub(fs => fs.FileExists(projectUserOptionsFile)).Return(true);
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(projectUserOptionsFile)).Throw(new Exception());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var progressMonitor = MockProgressMonitor.Instance;
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);
            projectController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "TreeViewCategory")
                    Assert.Fail();
            };

            projectController.OpenProject(projectName, progressMonitor);

            Assert.AreEqual("Namespace", projectController.TreeViewCategory);
            Assert.AreEqual(0, projectController.CollapsedNodes.Count);
        }

        [SyncTest]
        public void User_options_should_be_applied_when_available_after_loading_a_project()
        {
            string projectName = Path.GetFullPath("projectName");
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            xmlSerializer.Stub(xs => xs.LoadFromXml<TestProjectData>(projectName)).Return(new TestProjectData());
            string projectUserOptionsFile = projectName + UserOptions.Extension;
            fileSystem.Stub(fs => fs.FileExists(projectUserOptionsFile)).Return(true);
            const string treeViewCategory = "treeViewCategory";
            var collapsedNodes = new List<string>(new[] { "one", "two", "three" });
            var userOptions = new UserOptions
            {
                TreeViewCategory = treeViewCategory,
                CollapsedNodes = collapsedNodes
            };
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(projectUserOptionsFile)).Return(userOptions);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            var progressMonitor = MockProgressMonitor.Instance;
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, fileWatcher, unhandledExceptionPolicy);

            var treeViewCategoryChanged = false;
            projectController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "TreeViewCategory")
                    return;

                Assert.AreEqual(treeViewCategory, projectController.TreeViewCategory);
                treeViewCategoryChanged = true;
            };

            projectController.OpenProject(projectName, progressMonitor);

            Assert.AreEqual(true, treeViewCategoryChanged);
            Assert.AreEqual(collapsedNodes, projectController.CollapsedNodes);
        }
    }
}
