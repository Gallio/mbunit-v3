using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(RemoveFileCommand))]
    public class RemoveFileCommandTest
    {
        [Test]
        public void Execute_should_do_nothing_if_no_filename_is_provided()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var removeFileCommand = new RemoveFileCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.Instance;

            removeFileCommand.Execute(progressMonitor);

            projectController.AssertWasNotCalled(pc => pc.RemoveFile(Arg<string>.Is.Anything, 
                Arg.Is(progressMonitor)));
        }

        [Test]
        public void Execute_should_remove_the_file()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var removeFileCommand = new RemoveFileCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.Instance;
            const string filename = "filename";
            removeFileCommand.FileName = filename;

            removeFileCommand.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.RemoveFile(filename, progressMonitor));
        }
    }
}
