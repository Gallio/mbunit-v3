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
using System.IO;
using System.Reflection;
using Gallio.Common.IO;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Properties;
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
    [Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(ProjectController))]
    public class ProjectControllerTest
    {
        private ProjectController projectController;
        private IProjectTreeModel projectTreeModel;
        private IFileSystem fileSystem;
        private IFileWatcher fileWatcher;
        private IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private IEventAggregator eventAggregator;
        private ITestProjectManager testProjectManager;

        [SetUp]
        public void SetUp()
        {
            projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileWatcher = MockRepository.GenerateStub<IFileWatcher>();
            unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            testProjectManager = MockRepository.GenerateStub<ITestProjectManager>();
            
            projectController = new ProjectController(projectTreeModel, eventAggregator, fileSystem, fileWatcher, 
                unhandledExceptionPolicy, testProjectManager);
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
            Assert.Count(0, projectController.TestPackage.Files);

            projectController.AddFiles(progressMonitor, list);
            
            Assert.Count(1, projectController.TestPackage.Files);
            Assert.AreEqual(fileName, projectController.TestPackage.Files[0].ToString());
        }

        [Test]
        public void DeleteFilter_Test()
        {
            var testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            var progressMonitor = MockProgressMonitor.Instance;
            
            Assert.Count(0, projectController.TestFilters.Value);
            FilterInfo filterInfo = new FilterInfo("filterName", new NoneFilter<ITestDescriptor>().ToFilterExpr());
            projectController.TestFilters.Value.Add(filterInfo);
            Assert.Count(1, projectController.TestFilters.Value);
            projectController.DeleteFilter(progressMonitor, filterInfo);
            Assert.Count(0, projectController.TestFilters.Value);
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
            Assert.Count(0, projectController.TestFilters.Value);
        }

        [Test]
        public void ProjectFileName_Test()
        {
            const string fileName = "fileName";
            projectTreeModel.FileName = fileName;

            Assert.AreEqual(fileName, projectController.ProjectFileName);
        }

        [Test]
        public void New_project_creates_the_default_project()
        {
            testProjectManager.Stub(tpm => tpm.NewProject(Arg<string>.Is.Anything))
                .Return(new TestProject());

            projectController.NewProject(MockProgressMonitor.Instance);

            testProjectManager.AssertWasCalled(tpm => tpm.NewProject(Paths.DefaultProject));
        }

        [Test]
        public void RemoveAllFiles_Test()
        {
            projectTreeModel.TestProject = new TestProject();

            projectController.RemoveAllFiles();
        }

        [Test]
        public void RemoveFile_Test()
        {
            var project = new TestProject();
            string fileName = Path.GetFullPath("test");
            project.TestPackage.AddFile(new FileInfo(fileName));
            projectTreeModel.TestProject = project;
            Assert.Count(1, project.TestPackage.Files);
            
            projectController.RemoveFile(fileName);

            Assert.Count(0, project.TestPackage.Files);
        }

        [Test]
        public void SaveFilter_Test()
        {
            projectTreeModel.TestProject = new TestProject();

            Assert.Count(0, projectController.TestFilters.Value);
            projectController.SaveFilterSet("filterName", new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>()));
            Assert.Count(1, projectController.TestFilters.Value);
            projectController.SaveFilterSet("filterName", new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>()));
            Assert.Count(1, projectController.TestFilters.Value);
            projectController.SaveFilterSet("aDifferentFilterName", new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>()));
            Assert.Count(2, projectController.TestFilters.Value);
        }

        [Test]
        public void Save_should_announce_before_saving_the_project()
        {
            using (eventAggregator.GetMockRepository().Ordered())
            {
                eventAggregator.Expect(ea => ea.Send(Arg.Is(projectController), Arg<SavingProject>.Is.Anything));
                testProjectManager.Expect(tpm => tpm.SaveProject(Arg<TestProject>.Is.Anything, 
                    Arg<FileInfo>.Is.Anything));
            }
            eventAggregator.Replay();

            projectController.Save("projectName", NullProgressMonitor.CreateInstance());

            eventAggregator.VerifyAllExpectations();
        }

        [Test]
        public void Save_should_save_the_project()
        {
            var testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            const string projectName = "projectName";
            
            projectController.Save(projectName, NullProgressMonitor.CreateInstance());

            testProjectManager.AssertWasCalled(tpm => tpm.SaveProject(Arg.Is(testProject), 
                Arg<FileInfo>.Matches(fi => fi.Name == projectName)));
        }

        [Test]
        public void An_event_should_be_sent_when_the_project_is_saved()
        {
            const string projectLocation = "projectLocation";

            projectController.Save(projectLocation, NullProgressMonitor.CreateInstance());

            eventAggregator.Expect(ea => ea.Send(Arg.Is(projectController), Arg<ProjectSaved>.Matches(e => 
                e.ProjectLocation == projectLocation)));
        }

        [Test]
        public void Create_directory_for_project_if_necessary()
        {
            var testProject = new TestProject();
            projectTreeModel.TestProject = testProject;
            projectTreeModel.FileName = Paths.DefaultProject;
            var progressMonitor = MockProgressMonitor.Instance;
            fileSystem.Stub(fs => fs.DirectoryExists(Paths.IcarusAppDataFolder))
                .Return(false);

            projectController.Save("", progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.CreateDirectory(Paths.IcarusAppDataFolder));
        }

        [Test]
        public void Hint_directories_come_from_test_package()
        {
            var project = new TestProject();
            projectTreeModel.TestProject = project;

            var hintDirectories = projectController.HintDirectories;
            
            Assert.AreEqual(hintDirectories, project.TestPackage.HintDirectories);
        }

        [Test]
        public void Add_hint_directory_adds_to_test_package()
        {
            var project = new TestProject();
            projectTreeModel.TestProject = project;
            const string hintDirectory = @"c:\test";

            projectController.AddHintDirectory(hintDirectory);

            Assert.Count(1, project.TestPackage.HintDirectories);
            Assert.AreEqual(hintDirectory, project.TestPackage.HintDirectories[0].FullName);
        }

        [Test]
        public void Remove_hint_directory_removes_from_test_package()
        {
            var project = new TestProject();
            projectTreeModel.TestProject = project;
            const string hintDirectory = @"c:\test";
            project.TestPackage.AddHintDirectory(new DirectoryInfo(hintDirectory));

            projectController.RemoveHintDirectory(hintDirectory);

            Assert.Count(0, project.TestPackage.HintDirectories);
        }

        [Test]
        public void Test_runner_extension_specifications_come_from_project()
        {
            var project = new TestProject();
            projectTreeModel.TestProject = project;

            var testRunnerExtensionSpecifications = projectController.TestRunnerExtensionSpecifications;

            Assert.AreEqual(testRunnerExtensionSpecifications, project.TestRunnerExtensionSpecifications);
        }

        [Test]
        public void Add_test_runner_extension_specification_adds_to_test_package()
        {
            var project = new TestProject();
            projectTreeModel.TestProject = project;
            const string testRunnerExtensionSpecification = "testRunnerExtensionSpecification";

            projectController.AddTestRunnerExtensionSpecification(testRunnerExtensionSpecification);

            Assert.Count(1, project.TestRunnerExtensionSpecifications);
            Assert.AreEqual(testRunnerExtensionSpecification, project.TestRunnerExtensionSpecifications[0]);
        }

        [Test]
        public void Remove_test_runner_extension_specification_removes_from_test_package()
        {
            var project = new TestProject();
            projectTreeModel.TestProject = project;
            const string extensionSpecification = "extensionSpecification";
            project.AddTestRunnerExtensionSpecification(extensionSpecification);

            projectController.RemoveTestRunnerExtensionSpecification(extensionSpecification);

            Assert.Count(0, project.TestRunnerExtensionSpecifications);
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

        [Test]
        public void Open_project_loads_the_file()
        {
            var projectName = Path.GetFullPath("projectName");
            testProjectManager.Stub(tpm => tpm.LoadProject(Arg<FileInfo>.Is.Anything))
                .Return(new TestProject());

            projectController.OpenProject(MockProgressMonitor.Instance, projectName);

            testProjectManager.AssertWasCalled(tpm => tpm.LoadProject(Arg<FileInfo>.Matches(fi => 
                fi.FullName == projectName)));
        }

        [Test]
        public void Opening_project_should_succeed_even_if_file_cannot_be_loaded()
        {
            testProjectManager.Stub(tpm => tpm.LoadProject(Arg<FileInfo>.Is.Anything))
                .Throw(new Exception());

            Assert.DoesNotThrow(() => projectController.OpenProject(MockProgressMonitor.Instance, 
                Path.GetFullPath("projectName")));
        }

        [Test]
        public void Opening_project_should_report_any_errors_that_occur()
        {
            var exception = new Exception();
            testProjectManager.Stub(tpm => tpm.LoadProject(Arg<FileInfo>.Is.Anything)).Throw(exception);

            projectController.OpenProject(MockProgressMonitor.Instance, Path.GetFullPath("projectName"));

            unhandledExceptionPolicy.AssertWasCalled(uep => uep.Report(Arg.Is(Resources.ProjectController_Error_loading_project_file), 
                Arg.Is(exception)));
        }
    }
}
