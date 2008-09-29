using System.Drawing;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class RuntimeLogWindowTest : MockTest
    {
        [Test]
        public void LogMessage_Test()
        {
            IRuntimeLogController runtimeLogController = mocks.CreateMock<IRuntimeLogController>();
            runtimeLogController.LogMessage += null;
            IEventRaiser logMessage = LastCall.IgnoreArguments().GetEventRaiser();
            mocks.ReplayAll();
            RuntimeLogWindow runtimeLogWindow = new RuntimeLogWindow(runtimeLogController);
            logMessage.Raise(runtimeLogController, new RuntimeLogEventArgs("message", Color.Black));
        }
    }
}
