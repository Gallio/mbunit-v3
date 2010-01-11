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

using System;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public class SaveFilterCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IProjectController projectController;

        public string FilterName { get; set; }

        public SaveFilterCommand(ITestController testController, IProjectController projectController)
        {
            this.testController = testController;
            this.projectController = projectController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            if (string.IsNullOrEmpty(FilterName))
                throw new ArgumentException("Filter name cannot be null or empty");

            using (progressMonitor.BeginTask("Saving filter", 2))
            {
                var filterSet = testController.GenerateFilterSetFromSelectedTests();

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(50))
                    projectController.SaveFilterSet(subProgressMonitor, FilterName, filterSet);
            }
        }
    }
}
