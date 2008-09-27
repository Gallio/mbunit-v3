using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runner.Projects;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [MbUnit.Framework.Category("Views")]
    class FiltersWindowTest : MockTest
    {
        [Test]
        public void Constructor_Test()
        {
            IProjectController projectController = mocks.CreateMock<IProjectController>();
            Expect.Call(projectController.TestFilters).Return(new BindingList<FilterInfo>(new List<FilterInfo>()));
            ITestController testController = mocks.CreateMock<ITestController>();
            mocks.ReplayAll();
            FiltersWindow filtersWindow = new FiltersWindow(projectController, testController);
        }
    }
}
