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

using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Services;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model.Filters;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public class RestoreFilterCommand : ICommand
    {
        private readonly IFilterService filterService;
        private readonly IProjectController projectController;

        public RestoreFilterCommand(IFilterService filterService, IProjectController projectController)
        {
            this.filterService = filterService;
            this.projectController = projectController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            var testFilters = projectController.TestFilters.Value;
            var totalWorkUnits = testFilters.Count > 0 ? testFilters.Count : 1;

            using (progressMonitor.BeginTask("Restoring test filter", totalWorkUnits))
            {
                foreach (var filterInfo in testFilters)
                {
                    if (filterInfo.FilterName != "AutoSave")
                    {
                        progressMonitor.Worked(1);
                        continue;
                    }

                    ApplyFilter(progressMonitor, filterInfo);
                    return;
                }
            }
        }

        private void ApplyFilter(IProgressMonitor progressMonitor, FilterInfo filterInfo)
        {
            var filterSet = FilterUtils.ParseTestFilterSet(filterInfo.FilterExpr);
            var applyFilterCommand = new ApplyFilterCommand(filterService, filterSet);
            applyFilterCommand.Execute(progressMonitor);
        }
    }
}
