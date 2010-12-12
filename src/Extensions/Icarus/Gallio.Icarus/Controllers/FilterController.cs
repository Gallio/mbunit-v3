// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Gallio.Icarus.Commands;
using Gallio.Model.Filters;
using Gallio.Runner.Projects.Schema;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Controllers
{
    public class FilterController : IFilterController
    {
        private readonly ITaskManager taskManager;
        private readonly ICommandFactory commandFactory;

        public FilterController(ICommandFactory commandFactory, ITaskManager taskManager)
        {
            this.commandFactory = commandFactory;
            this.taskManager = taskManager;
        }

        public void ApplyFilter(string filter)
        {
            var filterSet = FilterUtils.ParseTestFilterSet(filter);
            var command = commandFactory.CreateApplyFilterCommand(filterSet);
            taskManager.QueueTask(command);
        }

        public void DeleteFilter(FilterInfo filterInfo)
        {
            var command = commandFactory.CreateDeleteFilterCommand(filterInfo);
            taskManager.QueueTask(command);
        }

        public void SaveFilter(string filterName)
        {
            var command = commandFactory.CreateSaveFilterCommand(filterName);
            taskManager.QueueTask(command);
        }
    }
}
