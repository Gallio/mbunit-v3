using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class ProgressMonitorTest : MockTest
    {
        [Test]
        public void Constructor_Test()
        {
            ITestController testController = mocks.CreateMock<ITestController>();
            testController.ProgressUpdate += null;
            LastCall.IgnoreArguments();
            IOptionsController optionsController = mocks.CreateMock<IOptionsController>();
            mocks.ReplayAll();
            ProgressMonitor progressMonitor = new ProgressMonitor(testController, optionsController);
        }
    }
}
