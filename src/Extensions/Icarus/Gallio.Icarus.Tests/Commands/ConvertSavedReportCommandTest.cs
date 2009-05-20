using Gallio.Common.IO;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(ConvertSavedReportCommand))]
    internal class ConvertSavedReportCommandTest
    {
        [Test]
        public void Execute_should_call_Convert_on_ReportController()
        {
            var reportController = MockRepository.GenerateStub<IReportController>();
            const string fileName = "fileName";
            const string format = "format";
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            const string generatedFile = "generatedFile";
            reportController.Stub(rc => rc.ConvertSavedReport(fileName, format, progressMonitor)).Return(generatedFile);
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(generatedFile)).Return(true);
            var command = new ConvertSavedReportCommand(reportController, fileName, format, fileSystem);

            command.Execute(progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.OpenFile(generatedFile));
        }
    }
}
