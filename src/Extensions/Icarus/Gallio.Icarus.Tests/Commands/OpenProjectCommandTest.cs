using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model;
using Gallio.Runner.Projects;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(OpenProjectCommand))]
    internal class OpenProjectCommandTest
    {
        [Test]
        public void Execute_should_reset_test_status()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            projectController.Stub(pc => pc.TestFilters).Return(new BindingList<FilterInfo>(new List<FilterInfo>()));
            const string fileName = "fileName";
            var openProjectCommand = new OpenProjectCommand(testController, projectController, fileName);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            openProjectCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.ResetTestStatus(progressMonitor));
        }

        [Test]
        public void Execute_should_open_project()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            projectController.Stub(pc => pc.TestFilters).Return(new BindingList<FilterInfo>(new List<FilterInfo>()));
            const string fileName = "fileName";
            var openProjectCommand = new OpenProjectCommand(testController, projectController, fileName);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            openProjectCommand.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.OpenProject(fileName, progressMonitor));
        }

        [Test]
        public void Execute_should_reload_test_package()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            projectController.Stub(pc => pc.TestFilters).Return(new BindingList<FilterInfo>(new List<FilterInfo>()));
            var testPackageConfig = new TestPackageConfig();
            projectController.Stub(pc => pc.TestPackageConfig).Return(testPackageConfig);
            var testRunnerExtensions = new BindingList<string>(new List<string>());
            projectController.Stub(pc => pc.TestRunnerExtensions).Return(testRunnerExtensions);
            const string fileName = "fileName";
            var openProjectCommand = new OpenProjectCommand(testController, projectController, fileName);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            openProjectCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.SetTestPackageConfig(testPackageConfig));
            testController.AssertWasCalled(tc => tc.Explore(progressMonitor, testRunnerExtensions));
        }

        [Test]
        public void Execute_should_throw_if_canceled()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            const string fileName = "fileName";
            var openProjectCommand = new OpenProjectCommand(testController, projectController, fileName);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            progressMonitor.Stub(pm => pm.IsCanceled).Return(true);

            Assert.Throws<OperationCanceledException>(() => openProjectCommand.Execute(progressMonitor));
        }
    }
}
