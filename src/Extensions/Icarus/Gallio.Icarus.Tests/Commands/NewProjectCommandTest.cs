using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(NewProjectCommand))]
    internal class NewProjectCommandTest
    {
        [Test]
        public void Execute_should_create_new_project()
        {
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testController = MockRepository.GenerateStub<ITestController>();
            var newProjectCommand = new NewProjectCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            newProjectCommand.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.NewProject(progressMonitor));
        }

        [Test]
        public void Execute_should_reload_test_package()
        {
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testPackageConfig = new TestPackageConfig();
            projectController.Stub(pc => pc.TestPackageConfig).Return(testPackageConfig);
            var testRunnerExtensions = new BindingList<string>(new List<string>());
            projectController.Stub(pc => pc.TestRunnerExtensions).Return(testRunnerExtensions);
            var testController = MockRepository.GenerateStub<ITestController>();
            var newProjectCommand = new NewProjectCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            newProjectCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.SetTestPackageConfig(testPackageConfig));
            testController.AssertWasCalled(tc => tc.Explore(progressMonitor, testRunnerExtensions));
        }

        [Test]
        public void Execute_should_throw_if_canceled()
        {
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testController = MockRepository.GenerateStub<ITestController>();
            var newProjectCommand = new NewProjectCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            progressMonitor.Stub(pm => pm.IsCanceled).Return(true);

            Assert.Throws<OperationCanceledException>(() => newProjectCommand.Execute(progressMonitor));
        }
    }
}
