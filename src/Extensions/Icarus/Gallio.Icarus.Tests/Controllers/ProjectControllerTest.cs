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
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Remoting;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.Events;
using MbUnit.Framework;
using Rhino.Mocks;
using System;
using Path = System.IO.Path;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(ProjectController))]
    internal class ProjectControllerTest
    {
        private ProjectController projectController;
        private IProjectTreeModel projectTreeModel;
        private IFileSystem fileSystem;
        private IXmlSerializer xmlSerializer;
        private IFileWatcher fileWatcher;
        private IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private IEventAggregator eventAggregator;

        [SetUp]
        public void SetUp()
        {
            projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            projectController = new ProjectController(projectTreeModel, eventAggregator, fileSystem, 
                xmlSerializer, fileWatcher, unhandledExceptionPolicy);
        }

        [Test]
        public void AddFiles_Test()
        {
            TestProject testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            string fileName = Assembly.GetExecutingAssembly().Location;
            List<string> list = new List<string>(new[] { fileName });
            fileSystem.Stub(fs => fs.FileExists(fileName)).Return(true);
            var progressMonitor = MockProgressMonitor.Instance;
            Assert.AreEqual(0, projectController.TestPackage.Files.Count);

            projectController.AddFiles(progressMonitor, list);
            
            Assert.AreEqual(1, projectController.TestPackage.Files.Count);
            Assert.AreEqual(fileName, projectController.TestPackage.Files[0].ToString());
        }

        [Test]
        public void DeleteFilter_Test()
        {
            var testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            var progressMonitor = MockProgressMonitor.Instance;
            
            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
            FilterInfo filterInfo = new FilterInfo("filterName", new NoneFilter<ITestDescriptor>().ToFilterExpr());
            projectController.TestFilters.Value.Add(filterInfo);
            Assert.AreEqual(1, projectController.TestFilters.Value.Count);
            projectController.DeleteFilter(progressMonitor, filterInfo);
            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
        }

        [Test]
        public void Model_Test()
        {
            Assert.AreEqual(projectTreeModel, projectController.Model);
        }

        [Test]
        public void TestPackage_Test()
        {
            var testProject = new TestProject();
            projectTreeModel.TestProject = testProject;

            Assert.AreEqual(testProject.TestPackage, projectController.TestPackage);
        }

        [Test]
        public void TestFilters_Test()
        {
            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
        }

        [Test]
        public void HintDirectories_Test()
        {
            Assert.AreEqual(0, projectController.HintDirectories.Count);
        }

        [Test]
        public void ProjectFileName_Test()
        {
            const string fileName = "fileName";
            projectTreeModel.FileName = fileName;

            Assert.AreEqual(fileName, projectController.ProjectFileName);
        }

        [SyncTest]
        public void NewProject_Test()
        {
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));

            projectController.PropertyChanged += ((sender, e) => Assert.AreEqual("TestPackage", e.PropertyName));
            projectController.NewProject(progressMonitor);
            Assert.AreEqual(Paths.DefaultProject, projectTreeModel.FileName);
            Assert.AreEqual(0, projectController.HintDirectories.Count);
            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
        }

        [Test]
        public void RemoveAllFiles_Test()
        {
            projectTreeModel.TestProject = new TestProject();
            var progressMonitor = MockProgressMonitor.Instance;

            projectController.RemoveAllFiles(progressMonitor);
        }

        [Test]
        public void RemoveFile_Test()
        {
            var project = new TestProject();
            string fileName = Path.GetFullPath("test");
            project.TestPackage.AddFile(new FileInfo(fileName));
            projectTreeModel.TestProject = project;
            var progressMonitor = MockProgressMonitor.Instance;           
            Assert.AreEqual(1, project.TestPackage.Files.Count);
            
            projectController.RemoveFile(progressMonitor, fileName);

            Assert.AreEqual(0, project.TestPackage.Files.Count);
        }

        [Test]
        public void SaveFilter_Test()
        {
            projectTreeModel.TestProject = new TestProject();
            var progressMonitor = MockProgressMonitor.Instance;
            
            Assert.AreEqual(0, projectController.TestFilters.Value.Count);
            projectController.SaveFilterSet(progressMonitor, "filterName", new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>()));
            Assert.AreEqual(1, projectController.TestFilters.Value.Count);
            projectController.SaveFilterSet(progressMonitor, "filterName", new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>()));
            Assert.AreEqual(1, projectController.TestFilters.Value.Count);
            projectController.SaveFilterSet(progressMonitor, "aDifferentFilterName", new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>()));
            Assert.AreEqual(2, projectController.TestFilters.Value.Count);
        }

        [Test]
        public void SaveProject_Test()
        {
            TestProject testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            var progressMonitor = MockProgressMonitor.Instance;

            const string projectName = "projectName";
            projectController.SaveProject(progressMonitor, projectName);
            xmlSerializer.AssertWasCalled(x => x.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(projectName + UserOptions.Extension)));
        }

        [Test]
        public void SaveDefaultProject_Test()
        {
            TestProject testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            projectTreeModel.FileName = Paths.DefaultProject;
            var progressMonitor = MockProgressMonitor.Instance;

            projectController.SaveProject(progressMonitor, string.Empty);

            fileSystem.AssertWasCalled(fs => fs.DirectoryExists(Paths.IcarusAppDataFolder));
            fileSystem.AssertWasCalled(fs => fs.CreateDirectory(Paths.IcarusAppDataFolder));
            xmlSerializer.AssertWasCalled(xs => xs.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(Paths.DefaultProject + UserOptions.Extension)));
        }

        [Test]
        public void Updating_HintDirectories_cascades_to_TestPackage()
        {
            var project = new TestProject();
            projectTreeModel.TestProject = project;
            Assert.AreEqual(0, project.TestPackage.HintDirectories.Count);
            
            const string hintDirectory = "test";
            projectController.HintDirectories.Add(hintDirectory);
            
            Assert.AreEqual(1, project.TestPackage.HintDirectories.Count);
            Assert.AreEqual(hintDirectory, project.TestPackage.HintDirectories[0].ToString());
        }

        [Test]
        public void Updating_TestRunnerExtensions_cascades_to_TestPackage()
        {
            var project = new TestProject();
            projectTreeModel.TestProject = project;
            Assert.AreEqual(0, project.TestRunnerExtensionSpecifications.Count);

            const string testRunnerExtension = "test";
            projectController.TestRunnerExtensions.Add(testRunnerExtension);

            Assert.AreEqual(1, project.TestRunnerExtensionSpecifications.Count);
            Assert.AreEqual(testRunnerExtension, project.TestRunnerExtensionSpecifications[0]);
        }

        [Test]
        public void FileWatcher_fires_FileChanged_event()
        {
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
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);
            var project = new TestProjectData { ReportNameFormat = "foo" };
            project.TestFilters.Add(new FilterInfo("None", new NoneFilter<ITestDescriptor>().ToFilterExpr()));
            project.TestPackage.HintDirectories.Add("hintDirectory");
            project.TestRunnerExtensions.Add("testRunnerExtensions");
            xmlSerializer.Stub(xs => xs.LoadFromXml<TestProjectData>(projectName)).Return(project);
            var progressMonitor = MockProgressMonitor.Instance;
            var propertyChangedFlag = false;
            projectController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != "TestPackage")
                    return;

                propertyChangedFlag = true;
            };

            projectController.OpenProject(progressMonitor, projectName);

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
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);
            var exception = new Exception();
            xmlSerializer.Stub(xs => xs.LoadFromXml<TestProjectData>(projectName)).Throw(exception);
            var progressMonitor = MockProgressMonitor.Instance;

            projectController.OpenProject(progressMonitor, projectName);

            unhandledExceptionPolicy.AssertWasCalled(uep => uep.Report(Arg<string>.Is.Anything, Arg.Is(exception)));
        }

        [Test]
        public void OpenProject_should_succeed_even_if_loading_user_options_throws_exception()
        {
            string projectName = Path.GetFullPath("projectName");
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);

            xmlSerializer.Stub(xs => xs.LoadFromXml<TestProjectData>(projectName)).Return(new TestProjectData());
            string projectUserOptionsFile = projectName + UserOptions.Extension;
            fileSystem.Stub(fs => fs.FileExists(projectUserOptionsFile)).Return(true);
            xmlSerializer.Stub(xs => xs.LoadFromXml<UserOptions>(projectUserOptionsFile)).Throw(new Exception());
            var progressMonitor = MockProgressMonitor.Instance;
            projectController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "TreeViewCategory")
                    Assert.Fail();
            };

            projectController.OpenProject(progressMonitor, projectName);

            Assert.AreEqual(0, projectController.CollapsedNodes.Value.Count);
        }

        // TODO: split this into multiple tests
        [SyncTest]
        public void User_options_should_be_applied_when_available_after_loading_a_project()
        {
            string projectName = Path.GetFullPath("projectName");
            fileSystem.Stub(fs => fs.FileExists(projectName)).Return(true);
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
            var progressMonitor = MockProgressMonitor.Instance;

            projectController.OpenProject(progressMonitor, projectName);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<TreeViewCategoryChanged>.Matches(tvcc => 
                tvcc.TreeViewCategory == treeViewCategory)));
            Assert.AreEqual(collapsedNodes, projectController.CollapsedNodes.Value);
        }
    }
}
