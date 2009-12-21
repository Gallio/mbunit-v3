using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model.Filters;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(ReloadCommand))]
    public class ReloadCommandTest
    {
        [Test]
        public void Execute_should_explore_with_test_runner_extensions()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testRunnerExtensions = new BindingList<string>();
            projectController.Stub(pc => pc.TestRunnerExtensions).Return(testRunnerExtensions);
            projectController.Stub(pc => pc.TestFilters).Return(new Observable<IList<FilterInfo>>(new List<FilterInfo>()));
            var reloadCommand = new ReloadCommand(testController, projectController);

            reloadCommand.Execute(MockProgressMonitor.Instance);

            testController.AssertWasCalled(tc => tc.Explore(Arg<IProgressMonitor>.Is.Anything, 
                Arg.Is(testRunnerExtensions)));
        }
    }
}
