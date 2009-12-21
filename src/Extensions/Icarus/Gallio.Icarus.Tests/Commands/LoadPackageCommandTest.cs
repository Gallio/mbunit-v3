using System.ComponentModel;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(LoadPackageCommand))]
    public class LoadPackageCommandTest
    {
        private ITestController testController;
        private IProjectController projectController;
        private LoadPackageCommand loadPackageCommand;
        private IProgressMonitor progressMonitor;

        [SetUp]
        public void SetUp()
        {
            testController = MockRepository.GenerateStub<ITestController>();
            projectController = MockRepository.GenerateStub<IProjectController>();
            loadPackageCommand = new LoadPackageCommand(testController, projectController);
            progressMonitor = MockProgressMonitor.Instance;
        }

        [Test]
        public void Execute_should_reload_the_test_package()
        {
            var testPackage = new TestPackage();
            projectController.Stub(pc => pc.TestPackage).Return(testPackage);

            loadPackageCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.SetTestPackage(testPackage));
        }

        [Test]
        public void Execute_should_explore()
        {
            var testRunnerExtensions = new BindingList<string>();
            projectController.Stub(pc => pc.TestRunnerExtensions).Return(testRunnerExtensions);

            loadPackageCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.Explore(progressMonitor, testRunnerExtensions));
        }
    }
}
