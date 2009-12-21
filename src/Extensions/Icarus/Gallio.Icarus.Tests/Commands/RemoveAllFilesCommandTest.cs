using System.ComponentModel;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(RemoveAllFilesCommand))]
    public class RemoveAllFilesCommandTest
    {
        [Test]
        public void Execute_should_remove_all_files()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var removeAllFilesCommand = new RemoveAllFilesCommand(testController, projectController);
            var progressMonitor = MockProgressMonitor.Instance;

            removeAllFilesCommand.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.RemoveAllFiles(progressMonitor));
        }

        [Test]
        public void Execute_should_reload_the_test_package()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var removeAllFilesCommand = new RemoveAllFilesCommand(testController, projectController);
            var progressMonitor = MockProgressMonitor.Instance;
            var testPackage = new TestPackage();
            projectController.Stub(pc => pc.TestPackage).Return(testPackage);

            removeAllFilesCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.SetTestPackage(testPackage));
        }

        [Test]
        public void Execute_should_explore()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var removeAllFilesCommand = new RemoveAllFilesCommand(testController, projectController);
            var progressMonitor = MockProgressMonitor.Instance;
            var testRunnerExtensions = new BindingList<string>();
            projectController.Stub(pc => pc.TestRunnerExtensions).Return(testRunnerExtensions);

            removeAllFilesCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.Explore(progressMonitor, testRunnerExtensions));
        }
    }
}
