using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [MbUnit.Framework.Category("Views")]
    class PropertiesWindowTest : MockTest
    {
        [Test]
        public void Constructor_Test()
        {
            IProjectController projectController = mocks.CreateMock<IProjectController>();
            Expect.Call(projectController.HintDirectories).Return(new BindingList<string>(new List<string>()));
            mocks.ReplayAll();
            PropertiesWindow propertiesWindow = new PropertiesWindow(projectController);
        }
    }
}
