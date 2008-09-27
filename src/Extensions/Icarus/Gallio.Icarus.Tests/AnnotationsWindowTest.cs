using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class AnnotationsWindowTest : MockTest
    {
        [Test]
        public void Constructor_Test()
        {
            ITestController testController = mocks.CreateMock<ITestController>();
            testController.LoadFinished += null;
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            AnnotationsWindow annotationsWindow = new AnnotationsWindow(testController);
        }
    }
}
