using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(DeleteFilterCommand))]
    internal class DeleteFilterCommandTest
    {
        [Test]
        public void Execute_should_call_DeleteFilter_on_ProjectController()
        {
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var filterInfo = new FilterInfo("None", new NoneFilter<ITest>().ToFilterExpr());
            var command = new DeleteFilterCommand(projectController, filterInfo);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            command.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.DeleteFilter(filterInfo, progressMonitor));
        }
    }
}
