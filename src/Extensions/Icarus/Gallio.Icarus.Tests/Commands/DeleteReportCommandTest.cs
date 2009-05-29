using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(DeleteReportCommand))]
    internal class DeleteReportCommandTest
    {
        [Test]
        public void Execute_should_call_DeleteReport_on_ReportController()
        {
            var reportController = MockRepository.GenerateStub<IReportController>();
            const string fileName = "fileName";
            var deleteReportCommand = new DeleteReportCommand(reportController) { FileName = fileName };
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            
            deleteReportCommand.Execute(progressMonitor);

            reportController.AssertWasCalled(rc => rc.DeleteReport(fileName, progressMonitor));
        }

        [Test]
        public void FileName_should_return_correct_value()
        {
            var reportController = MockRepository.GenerateStub<IReportController>();
            var deleteReportCommand = new DeleteReportCommand(reportController);
            const string fileName = "fileName";

            deleteReportCommand.FileName = fileName;

            Assert.AreEqual(fileName, deleteReportCommand.FileName);
        }
    }
}
