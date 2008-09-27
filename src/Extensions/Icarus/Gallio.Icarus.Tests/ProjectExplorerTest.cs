using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Runner.Projects;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class ProjectExplorerTest : MockTest
    {
        [Test]
        public void Constructor_Test()
        {
            IProjectController projectController = mocks.CreateMock<IProjectController>();
            Expect.Call(projectController.Model).Return(new ProjectTreeModel("fileName", new Project()));
            mocks.ReplayAll();
            ProjectExplorer projectExplorer = new ProjectExplorer(projectController);
        }
    }
}
