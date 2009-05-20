using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(ApplyFilterCommand))]
    internal class ApplyFilterCommandTest
    {
        [Test]
        public void Execute_should_call_ApplyFilterSet_on_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var filterSet = new FilterSet<ITest>(new NoneFilter<ITest>());
            var cmd = new ApplyFilterCommand(testController, filterSet);

            cmd.Execute(MockProgressMonitor.GetMockProgressMonitor());

            testController.AssertWasCalled(tc => tc.ApplyFilterSet(filterSet));
        }
    }
}
