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
using System.Reflection;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), Author("Graham Hay")]
    class ProjectControllerTest : MockTest
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

            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerializer);
            Assert.AreEqual(0, projectController.TestPackageConfig.AssemblyFiles.Count);
            projectController.AddAssemblies(list, progressMonitor);
            
            Assert.AreEqual(1, projectController.TestPackageConfig.AssemblyFiles.Count);
            Assert.AreEqual(fileName, projectController.TestPackageConfig.AssemblyFiles[0]);
        }

        [Test]
        public void DeleteFilter_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            Project project = new Project();
            Expect.Call(projectTreeModel.Project).Return(project).Repeat.Times(4);
            IProgressMonitor progressMonitor = mocks.StrictMock<IProgressMonitor>();
            Expect.Call(progressMonitor.BeginTask("Deleting filter", 1)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Done();
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
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
            IProjectTreeModel projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            Project project = new Project();
            Expect.Call(projectTreeModel.Project).Return(project).Repeat.Times(3);
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            IProgressMonitor progressMonitor = mocks.StrictMock<IProgressMonitor>();
            Expect.Call(progressMonitor.BeginTask("Getting filter", 1)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Done();
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            Assert.AreEqual(0, projectController.TestFilters.Count);
            FilterInfo filterInfo = new FilterInfo("filterName", new NoneFilter<ITest>().ToFilterExpr());
            projectController.TestFilters.Add(filterInfo);
            Assert.AreEqual(1, projectController.TestFilters.Count);
            Assert.AreEqual(filterInfo.Filter, projectController.GetFilter(filterInfo.FilterName, progressMonitor).ToFilterExpr());
        }

        [Test]
        public void GetInvalidFilter_Test()
        {
            var projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            var project = new Project();
            Expect.Call(projectTreeModel.Project).Return(project);
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = mocks.StrictMock<IProgressMonitor>();
            Expect.Call(progressMonitor.BeginTask("Getting filter", 1)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Done();
            mocks.ReplayAll();
            var projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            Assert.AreEqual(0, projectController.TestFilters.Count);
            Assert.IsNull(projectController.GetFilter("filterName", progressMonitor));
        }

        [Test]
        public void Model_Test()
        {
            var projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            Assert.AreEqual(projectTreeModel, projectController.Model);
        }

        [Test]
        public void TestPackageConfig_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            Project project = new Project();
            Expect.Call(projectTreeModel.Project).Return(project);

            mocks.ReplayAll();

            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            Assert.AreEqual(project.TestPackageConfig, projectController.TestPackageConfig);
        }

        [Test]
        public void TestFilters_Test()
        {
            var projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            Assert.AreEqual(0, projectController.TestFilters.Count);
        }

        [Test]
        public void HintDirectories_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            Assert.AreEqual(0, projectController.HintDirectories.Count);
        }

        [Test]
        public void ProjectFileName_Test()
        {
            var projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            const string fileName = "fileName";
            Expect.Call(projectTreeModel.FileName).Return(fileName);
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            Assert.AreEqual(fileName, projectController.ProjectFileName);
        }

        [Test]
        public void NewProject_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            projectController.PropertyChanged += ((sender, e) => Assert.AreEqual("TestPackageConfig", e.PropertyName));
            projectController.NewProject(progressMonitor);
            Assert.AreEqual(Paths.DefaultProject, projectTreeModel.FileName);
            Assert.AreEqual(0, projectController.HintDirectories.Count);
            Assert.AreEqual(0, projectController.TestFilters.Count);
        }

        [Test]
        public void RemoveAllAssemblies_Test()
        {
            var projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            Expect.Call(projectTreeModel.Project).Return(new Project());
            IProgressMonitor progressMonitor = mocks.StrictMock<IProgressMonitor>();
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            projectController.RemoveAllAssemblies(progressMonitor);
        }

        [Test]
        public void RemoveAssembly_Test()
        {
            var projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            var project = new Project();
            const string fileName = "test";
            project.TestPackageConfig.AssemblyFiles.Add(fileName);
            Expect.Call(projectTreeModel.Project).Return(project);
            
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            
            var progressMonitor = mocks.StrictMock<IProgressMonitor>();
            
            mocks.ReplayAll();
            
            var projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            Assert.AreEqual(1, project.TestPackageConfig.AssemblyFiles.Count);
            
            projectController.RemoveAssembly(fileName, progressMonitor);
            Assert.AreEqual(0, project.TestPackageConfig.AssemblyFiles.Count);
        }

        [Test]
        public void SaveFilter_Test()
        {
            var projectTreeModel = mocks.StrictMock<IProjectTreeModel>();
            Expect.Call(projectTreeModel.Project).Return(new Project()).Repeat.Twice();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = mocks.StrictMock<IProgressMonitor>();
            mocks.ReplayAll();
            var projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            Assert.AreEqual(0, projectController.TestFilters.Count);
            projectController.SaveFilter("filterName", new NoneFilter<ITest>(), progressMonitor);
            Assert.AreEqual(1, projectController.TestFilters.Count);
            projectController.SaveFilter("filterName", new NoneFilter<ITest>(), progressMonitor);
            Assert.AreEqual(1, projectController.TestFilters.Count);
        }

        [Test]
        public void SaveProject_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            Project project = new Project();
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            const string projectName = "projectName";
            projectController.SaveProject(projectName, progressMonitor);
            xmlSerialization.AssertWasCalled(x => x.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(projectName + ".user")));
        }

        [Test]
        public void SaveDefaultProject_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            Project project = new Project();
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerialization = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var projectController = new ProjectController(projectTreeModel, fileSystem, xmlSerialization);
            projectController.SaveProject(string.Empty, progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.DirectoryExists(Paths.IcarusAppDataFolder));
            fileSystem.AssertWasCalled(fs => fs.CreateDirectory(Paths.IcarusAppDataFolder));
            xmlSerialization.AssertWasCalled(xs => xs.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(Paths.DefaultProject + ".user")));
        }
    }
}
