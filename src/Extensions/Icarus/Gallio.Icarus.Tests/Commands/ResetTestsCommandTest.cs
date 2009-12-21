using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(ResetTestsCommand))]
    public class ResetTestsCommandTest
    {
        [Test]
        public void Execute_should_reset_test_statuses()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var resetTestsCommand = new ResetTestsCommand(testController);
            var progressMonitor = MockProgressMonitor.Instance;

            resetTestsCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.ResetTestStatus(progressMonitor));
        }
    }
}
