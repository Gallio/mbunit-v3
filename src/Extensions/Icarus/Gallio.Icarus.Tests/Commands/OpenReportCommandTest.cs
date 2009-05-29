using Gallio.Common.IO;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(OpenReportCommand))]
    internal class OpenReportCommandTest
    {
        [Test]
        public void Execute_should_call_open_file_on_file_system_if_filename_is_supplied()
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var command = new OpenReportCommand(fileSystem);
            const string fileName = "fileName";
            command.FileName = fileName;
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            command.Execute(progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.OpenFile(fileName));
        }

        [Test]
        public void Execute_should_not_call_open_file_on_file_system_if_filename_is_not_supplied()
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var command = new OpenReportCommand(fileSystem);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            command.Execute(progressMonitor);

            fileSystem.AssertWasNotCalled(fs => fs.OpenFile(Arg<string>.Is.Anything));
        }
    }
}
