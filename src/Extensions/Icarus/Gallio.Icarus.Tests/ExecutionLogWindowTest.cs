using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class ExecutionLogWindowTest : MockTest
    {
        [Test]
        public void Constructor_Test()
        {
            IExecutionLogController executionLogController = mocks.CreateMock<IExecutionLogController>();
            executionLogController.ExecutionLogUpdated += null;
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            ExecutionLogWindow executionLogWindow = new ExecutionLogWindow(executionLogController);
        }
    }
}
