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

using Gallio.Icarus.Controllers;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Runner.Projects;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    class ProjectControllerTest : MockTest
    {
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
