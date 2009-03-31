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
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.Utilities;
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
    class ProjectControllerTest
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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);
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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

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
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

            Assert.AreEqual(project.TestPackageConfig, projectController.TestPackageConfig);
        }

        [Test]
        public void TestFilters_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

            Assert.AreEqual(0, projectController.TestFilters.Count);
        }

        [Test]
        public void HintDirectories_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);
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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

            Assert.AreEqual(fileName, projectController.ProjectFileName);
        }

        [Test]
        public void NewProject_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            var optionsController = MockRepository.GenerateStub<IOptionsController>();

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

            Assert.AreEqual(0, projectController.TestFilters.Count);
            projectController.SaveFilterSet("filterName", new FilterSet<ITest>(new NoneFilter<ITest>()), progressMonitor);
            Assert.AreEqual(1, projectController.TestFilters.Count);
            projectController.SaveFilterSet("filterName", new FilterSet<ITest>(new NoneFilter<ITest>()), progressMonitor);
            Assert.AreEqual(1, projectController.TestFilters.Count);
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

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

            const string projectName = "projectName";
            projectController.SaveProject(projectName, progressMonitor);
            xmlSerializer.AssertWasCalled(x => x.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(projectName + ".user")));
            Assert.AreEqual(1, mruList.Count);
            Assert.IsTrue(mruList.Items.Contains(projectName));
        }

        [Test]
        public void SaveDefaultProject_Test()
        {
            var projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            Project project = new Project();
            projectTreeModel.Project = project;
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var xmlSerializer = MockRepository.GenerateStub<IXmlSerializer>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            optionsController.Stub(oc => oc.RecentProjects).Return(new MRUList(new List<string>(), 10));

            var projectController = new ProjectController(projectTreeModel, optionsController, fileSystem, xmlSerializer);

            projectController.SaveProject(string.Empty, progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.DirectoryExists(Paths.IcarusAppDataFolder));
            fileSystem.AssertWasCalled(fs => fs.CreateDirectory(Paths.IcarusAppDataFolder));
            xmlSerializer.AssertWasCalled(xs => xs.SaveToXml(Arg<UserOptions>.Is.Anything, 
                Arg.Is(Paths.DefaultProject + ".user")));
        }
    }
}
