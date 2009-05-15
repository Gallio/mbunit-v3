using System.ComponentModel;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runner.Projects;
using Gallio.Model.Filters;

namespace Gallio.Icarus.Controllers
{
    internal class FilterController : IFilterController
    {
        private readonly ITaskManager taskManager;
        private readonly ITestController testController;
        private readonly IProjectController projectController;

        public BindingList<FilterInfo> TestFilters 
        {
            get { return projectController.TestFilters; }
        }

        public FilterController(ITaskManager taskManager, ITestController testController, 
            IProjectController projectController)
        {
            this.taskManager = taskManager;
            this.testController = testController;
            this.projectController = projectController;
        }

        public void ApplyFilter(string filter)
        {
            var filterSet = FilterUtils.ParseTestFilterSet(filter);
            var command = new ApplyFilterCommand(testController, filterSet);
            taskManager.QueueTask(command);
        }

        public void DeleteFilter(FilterInfo filterInfo)
        {
            var command = new DeleteFilterCommand(projectController, filterInfo);
            taskManager.QueueTask(command);
        }

        public void SaveFilter(string filterName)
        {
        }
    }
}
