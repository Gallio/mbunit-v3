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
