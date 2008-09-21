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

using System.Collections.Generic;
using System.Reflection;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
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
            IProjectTreeModel projectTreeModel = mocks.CreateMock<IProjectTreeModel>();
            Project project = new Project();
            Expect.Call(projectTreeModel.Project).Return(project).Repeat.Times(4);
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel);
            Assert.AreEqual(0, projectController.TestPackageConfig.AssemblyFiles.Count);
            string fileName = Assembly.GetExecutingAssembly().Location;
            List<string> list = new List<string>(new[] { fileName });
            projectController.AddAssemblies(list);
            Assert.AreEqual(1, projectController.TestPackageConfig.AssemblyFiles.Count);
            Assert.AreEqual(fileName, projectController.TestPackageConfig.AssemblyFiles[0]);
        }

        [Test]
        public void DeleteFilter_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.CreateMock<IProjectTreeModel>();
            Project project = new Project();
            Expect.Call(projectTreeModel.Project).Return(project).Repeat.Times(4);
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel);
            Assert.AreEqual(0, projectController.TestFilters.Count);
            FilterInfo filterInfo = new FilterInfo("filterName", new NoneFilter<ITest>().ToFilterExpr());
            projectController.TestFilters.Add(filterInfo);
            Assert.AreEqual(1, projectController.TestFilters.Count);
            projectController.DeleteFilter(filterInfo);
            Assert.AreEqual(0, projectController.TestFilters.Count);
        }

        [Test]
        public void GetFilter_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.CreateMock<IProjectTreeModel>();
            Project project = new Project();
            Expect.Call(projectTreeModel.Project).Return(project).Repeat.Times(3);
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel);
            Assert.AreEqual(0, projectController.TestFilters.Count);
            FilterInfo filterInfo = new FilterInfo("filterName", new NoneFilter<ITest>().ToFilterExpr());
            projectController.TestFilters.Add(filterInfo);
            Assert.AreEqual(1, projectController.TestFilters.Count);
            Assert.AreEqual(filterInfo.Filter, projectController.GetFilter(filterInfo.FilterName).ToFilterExpr());
        }

        [Test]
        public void Model_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.CreateMock<IProjectTreeModel>();
            mocks.ReplayAll();
            ProjectController projectController = new ProjectController(projectTreeModel);
            Assert.AreEqual(projectTreeModel, projectController.Model);
        }

        [Test]
        public void TestPackageConfig_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.CreateMock<IProjectTreeModel>();
            Project project = new Project();
            Expect.Call(projectTreeModel.Project).Return(project);

            mocks.ReplayAll();

            ProjectController projectController = new ProjectController(projectTreeModel);
            Assert.AreEqual(project.TestPackageConfig, projectController.TestPackageConfig);
        }

        [Test]
        public void TestFilters_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.CreateMock<IProjectTreeModel>();

            mocks.ReplayAll();

            ProjectController projectController = new ProjectController(projectTreeModel);
            Assert.AreEqual(0, projectController.TestFilters.Count);
        }

        [Test]
        public void HintDirectories_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.CreateMock<IProjectTreeModel>();

            mocks.ReplayAll();

            ProjectController projectController = new ProjectController(projectTreeModel);
            Assert.AreEqual(0, projectController.HintDirectories.Count);
        }

        [Test]
        public void ProjectFileName_Test()
        {
            IProjectTreeModel projectTreeModel = mocks.CreateMock<IProjectTreeModel>();
            const string fileName = "fileName";
            Expect.Call(projectTreeModel.FileName).Return(fileName);

            mocks.ReplayAll();

            ProjectController projectController = new ProjectController(projectTreeModel);
            Assert.AreEqual(fileName, projectController.ProjectFileName);
        }
    }
}
