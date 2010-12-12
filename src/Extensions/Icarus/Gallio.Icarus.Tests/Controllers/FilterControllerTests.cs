using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers;
using Gallio.Model.Filters;
using Gallio.Runner.Projects.Schema;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    public class FilterControllerTests
    {
        private FilterController controller;
        private ICommandFactory commandFactory;
        private ITaskManager taskManager;

        [SetUp]
        public void SetUp()
        {
            commandFactory = MockRepository.GenerateStub<ICommandFactory>();
            taskManager = MockRepository.GenerateStub<ITaskManager>();
            controller = new FilterController(commandFactory, taskManager);
        }

        [Test]
        public void Apply_filter_queues_parsed_filter_set()
        {
            var applyFilterCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateApplyFilterCommand(Arg<FilterSet<ITestDescriptor>>.Is.Anything))
                .Return(applyFilterCommand);
            
            controller.ApplyFilter("*");

            taskManager.AssertWasCalled(tm => tm.QueueTask(Arg.Is(applyFilterCommand)));
        }

        [Test]
        public void Delete_filter_queues_delete_command()
        {
            var deleteFilterCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateDeleteFilterCommand(Arg<FilterInfo>.Is.Anything))
                .Return(deleteFilterCommand);
            var filterInfo = new FilterInfo("filterName", "*");

            controller.DeleteFilter(filterInfo);

            commandFactory.AssertWasCalled(cf => cf.CreateDeleteFilterCommand(filterInfo));
            taskManager.AssertWasCalled(tm => tm.QueueTask(Arg.Is(deleteFilterCommand)));
        }

        [Test]
        public void Save_filter_queues_save_command()
        {
            var saveFilterCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateSaveFilterCommand(Arg<string>.Is.Anything))
                .Return(saveFilterCommand);
            const string filterName = "filterName";

            controller.SaveFilter(filterName);

            commandFactory.AssertWasCalled(cf => cf.CreateSaveFilterCommand(filterName));
            taskManager.AssertWasCalled(tm => tm.QueueTask(Arg.Is(saveFilterCommand)));
        }
    }
}