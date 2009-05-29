using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(RefreshTestTreeCommand))]
    internal class RefreshTestTreeCommandTest
    {
        [Test]
        public void Execute_should_call_refresh_on_test_controller()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var cmd = new RefreshTestTreeCommand(testController);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            cmd.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.RefreshTestTree(progressMonitor));
        }
    }
}
