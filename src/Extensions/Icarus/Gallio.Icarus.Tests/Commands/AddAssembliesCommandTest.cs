using System.Collections.Generic;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(AddAssembliesCommand))]
    internal class AddAssembliesCommandTest
    {
        [Test]
        public void Execute_Test()
        {
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testPackageConfig = new TestPackageConfig();
            projectController.Stub(pc => pc.TestPackageConfig).Return(testPackageConfig);
            var testRunnerExtensions = new System.ComponentModel.BindingList<string>(new List<string>());
            projectController.Stub(pc => pc.TestRunnerExtensions).Return(testRunnerExtensions);
            var testController = MockRepository.GenerateStub<ITestController>();
            var command = new AddAssembliesCommand(projectController, testController);
            var assemblyFiles = new List<string>();
            command.AssemblyFiles = assemblyFiles;
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            
            command.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.AddAssemblies(assemblyFiles, progressMonitor));
            testController.AssertWasCalled(tc => tc.SetTestPackageConfig(testPackageConfig));
            testController.AssertWasCalled(tc => tc.Explore(progressMonitor, testRunnerExtensions));
        }
    }
}
