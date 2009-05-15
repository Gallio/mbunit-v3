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

using System.Collections.Generic;
using System.ComponentModel;
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
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;
using System;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(ProjectController))]
    internal class ProjectControllerTest
    {
        [Test]
        public void AddAssemblies_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            Project project = new Project();
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            string fileName = Assembly.GetExecutingAssembly().Location;
            List<string> list = new List<string>(new[] { fileName });
            fileSystem.Stub(fs => fs.FileExists(fileName)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);
            Assert.AreEqual(0, projectController.TestPackageConfig.AssemblyFiles.Count);

            projectController.AddAssemblies(list, progressMonitor);
            
            Assert.AreEqual(1, projectController.TestPackageConfig.AssemblyFiles.Count);
            Assert.AreEqual(fileName, projectController.TestPackageConfig.AssemblyFiles[0]);
        }

        [Test]
        public void DeleteFilter_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            Project project = new Project();
            projectTreeModel.Project = project;
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);
            
            Assert.AreEqual(0, projectController.TestFilters.Count);
            FilterInfo filterInfo = new FilterInfo("filterName", new NoneFilter<ITest>().ToFilterExpr());
            projectController.TestFilters.Add(filterInfo);
            Assert.AreEqual(1, projectController.TestFilters.Count);
            projectController.DeleteFilter(filterInfo, progressMonitor);
            Assert.AreEqual(0, projectController.TestFilters.Count);
        }

        [Test]
        public void GetFilter_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            Project project = new Project();
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(0, projectController.TestFilters.Count);
            FilterInfo filterInfo = new FilterInfo("filterName", new NoneFilter<ITest>().ToFilterExpr());
            projectController.TestFilters.Add(filterInfo);
            Assert.AreEqual(1, projectController.TestFilters.Count);
            Assert.AreEqual(filterInfo.Filter, projectController.GetFilterSet(filterInfo.FilterName, progressMonitor).ToFilterSetExpr());
        }

        [Test]
        public void GetInvalidFilter_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var project = new Project();
            var filterInfo = new FilterInfo("None", new NoneFilter<ITest>().ToFilterExpr());
            project.TestFilters.Add(filterInfo);
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(0, projectController.TestFilters.Count);
            Assert.IsNull(projectController.GetFilterSet("filterName", progressMonitor));
        }

        [Test]
        public void Model_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(projectTreeModel, projectController.Model);
        }

        [Test]
        public void TestPackageConfig_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            Project project = new Project();
            projectTreeModel.Project = project;
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(project.TestPackageConfig, projectController.TestPackageConfig);
        }

        [Test]
        public void TestFilters_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(0, projectController.TestFilters.Count);
        }

        [Test]
        public void HintDirectories_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

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
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

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
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            projectController.PropertyChanged += ((sender, e) => Assert.AreEqual("TestPackageConfig", e.PropertyName));
            projectController.NewProject(progressMonitor);
            Assert.AreEqual(Paths.DefaultProject, projectTreeModel.FileName);
            Assert.AreEqual(0, projectController.HintDirectories.Count);
            Assert.AreEqual(0, projectController.TestFilters.Count);
        }

        [Test]
        public void RemoveAllAssemblies_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            projectTreeModel.Project = new Project();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            projectController.RemoveAllAssemblies(progressMonitor);
        }

        [Test]
        public void RemoveAssembly_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var project = new Project();
            const string fileName = "test";
            project.TestPackageConfig.AssemblyFiles.Add(fileName);
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(1, project.TestPackageConfig.AssemblyFiles.Count);
            
            projectController.RemoveAssembly(fileName, progressMonitor);
            Assert.AreEqual(0, project.TestPackageConfig.AssemblyFiles.Count);
        }

        [Test]
        public void SaveFilter_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            projectTreeModel.Project = new Project();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(0, projectController.TestFilters.Count);
            projectController.SaveFilterSet("filterName", new FilterSet<ITest>(new NoneFilter<ITest>()), progressMonitor);
            Assert.AreEqual(1, projectController.TestFilters.Count);
            projectController.SaveFilterSet("filterName", new FilterSet<ITest>(new NoneFilter<ITest>()), progressMonitor);
            Assert.AreEqual(1, projectController.TestFilters.Count);
            projectController.SaveFilterSet("aDifferentFilterName", new FilterSet<ITest>(new NoneFilter<ITest>()), progressMonitor);
            Assert.AreEqual(2, projectController.TestFilters.Count);
        }

        [Test]
        public void SaveProject_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            Project project = new Project();
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var mruList = new MRUList(new List<string>(), 10);
            optionsController.Stub(oc => oc.RecentProjects).Return(mruList);
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

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
            Project project = new Project();
            projectTreeModel.Project = project;
            projectTreeModel.FileName = Paths.DefaultProject;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            optionsController.Stub(oc => oc.RecentProjects).Return(new MRUList(new List<string>(), 10));
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            projectController.SaveProject(string.Empty, progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.DirectoryExists(Paths.IcarusAppDataFolder));
            fileSystem.AssertWasCalled(fs => fs.CreateDirectory(Paths.IcarusAppDataFolder));
            xmlSerializer.AssertWasCalled(xs => xs.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(Paths.DefaultProject + UserOptions.Extension)));
        }

        [Test]
        public void Updating_HintDirectories_cascades_to_TestPackageConfig()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var project = new Project { TestPackageConfig = new TestPackageConfig() };
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);
            Assert.AreEqual(0, project.TestPackageConfig.HintDirectories.Count);
            
            const string hintDirectory = "test";
            projectController.HintDirectories.Add(hintDirectory);
            
            Assert.AreEqual(1, project.TestPackageConfig.HintDirectories.Count);
            Assert.AreEqual(hintDirectory, project.TestPackageConfig.HintDirectories[0]);
        }

        [Test]
        public void Updating_TestRunnerExtensions_cascades_to_TestPackageConfig()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var project = new Project { TestPackageConfig = new TestPackageConfig() };
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);
            Assert.AreEqual(0, project.TestRunnerExtensions.Count);

            const string testRunnerExtension = "test";
            projectController.TestRunnerExtensions.Add(testRunnerExtension);

            Assert.AreEqual(1, project.TestRunnerExtensions.Count);
            Assert.AreEqual(testRunnerExtension, project.TestRunnerExtensions[0]);
        }

        [Test]
        public void AssemblyWatcher_fires_AssemblyChanged_event()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            var assemblyChangedFlag = false;
            const string assemblyName = "test";
            projectController.AssemblyChanged += delegate(object sender, AssemblyChangedEventArgs e)
            {
                assemblyChangedFlag = true;
                Assert.AreEqual(assemblyName, e.AssemblyName);
            };
            assemblyWatcher.Raise(aw => aw.AssemblyChangedEvent += null, new object[] { assemblyName });
            Assert.AreEqual(true, assemblyChangedFlag);
        }

        [SyncTest]
        public void OpenProject_Test()
        {
            const string projectName = "projectName";
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var project = new Project
            {
                TestPackageConfig = new TestPackageConfig()
            };
            project.TestFilters.Add(new FilterInfo("None", new NoneFilter<ITest>().ToFilterExpr()));
            project.TestPackageConfig.HintDirectories.Add("hintDirectory");
            project.TestRunnerExtensions.Add("testRunnerExtensions");
            xmlSerializer.Stub(xs => xs.LoadFromXml<Project>(projectName)).Return(project);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);
            var propertyChangedFlag = false;
            projectController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != "TestPackageConfig")
                    return;

                propertyChangedFlag = true;
            };

            projectController.OpenProject(projectName, progressMonitor);

            Assert.AreEqual(projectName, projectTreeModel.FileName);
            Assert.AreEqual(project, projectTreeModel.Project);

            Assert.AreEqual(1, projectController.TestFilters.Count);
            Assert.AreEqual(1, projectController.HintDirectories.Count);
            Assert.AreEqual(1, projectController.TestRunnerExtensions.Count);
            Assert.AreEqual(true, propertyChangedFlag);
        }

        [Test]
        public void OpenProject_should_fail_if_loading_project_throws_exception()
        {
            const string projectName = "projectName";
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var exception = new Exception();
            xmlSerializer.Stub(xs => xs.LoadFromXml<Project>(projectName)).Throw(exception);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            Assert.AreEqual(exception, Assert.Throws<Exception>(() => projectController.OpenProject(projectName, progressMonitor)));
        }

        [Test]
        public void OpenProject_should_succeed_even_if_loading_user_options_throws_exception()
        {
            const string projectName = "projectName";
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);

            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            xmlSerializer.Stub(xs => xs.LoadFromXml<Project>(projectName)).Return(new Project());
            string projectUserOptionsFile = projectName + UserOptions.Extension;
            fileSystem.Stub(fs => fs.FileExists(projectUserOptionsFile)).Return(true);
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(projectUserOptionsFile)).Throw(new Exception());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);
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
            const string projectName = "projectName";
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            xmlSerializer.Stub(xs => xs.LoadFromXml<Project>(projectName)).Return(new Project());
            string projectUserOptionsFile = projectName + UserOptions.Extension;
            fileSystem.Stub(fs => fs.FileExists(projectUserOptionsFile)).Return(true);
            const string treeViewCategory = "treeViewCategory";
            var collapsedNodes = new List<string>(new[] { "one", "two", "three" });
            var userOptions = new UserOptions()
            {
                TreeViewCategory = treeViewCategory,
                CollapsedNodes = collapsedNodes
            };
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(projectUserOptionsFile)).Return(userOptions);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

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

        [Test]
        public void RefreshTree_calls_Refresh_on_ProjectTreeModel()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var assemblyWatcher = MockRepository.GenerateStub<IAssemblyWatcher>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            var projectController = new ProjectController(projectTreeModel, optionsController,
                fileSystem, xmlSerializer, assemblyWatcher, unhandledExceptionPolicy);

            projectController.RefreshTree(progressMonitor);

            projectTreeModel.AssertWasCalled(ptm => ptm.Refresh());
        }
    }
}
