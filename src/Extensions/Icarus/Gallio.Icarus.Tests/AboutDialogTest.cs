using System.Collections.Generic;
using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class AboutDialogTest : MockTest
    {
        [Test]
        public void Constructor_Test()
        {
            ITestController testController = mocks.CreateMock<ITestController>();
            Expect.Call(testController.TestFrameworks).Return(new List<string>(new[] {"test"}));
            mocks.ReplayAll();
            AboutDialog aboutDialog = new AboutDialog(testController);
        }
    }
}
