using System.Collections.Generic;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model.Filters;
using Gallio.Runner.Projects.Schema;
using Gallio.UI.DataBinding;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(RestoreFilterCommand))]
    public class RestoreFilterCommandTest
    {
        [Test]
        public void Execute_should_restore_test_filter()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testFilters = new List<FilterInfo>
            {
                new FilterInfo("SomeFilter", "*"), new FilterInfo("AutoSave", "*")
            };
            projectController.Stub(pc => pc.TestFilters).Return(new Observable<IList<FilterInfo>>(testFilters));
            var restoreFilterCommand = new RestoreFilterCommand(testController, projectController);

            restoreFilterCommand.Execute(MockProgressMonitor.Instance);

            testController.AssertWasCalled(tc => tc.ApplyFilterSet(Arg<FilterSet<ITestDescriptor>>.Is.Anything));
        }
    }
}
