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
using Gallio.Icarus.Events;
using Gallio.Icarus.Projects;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public class ReloadCommand : ICommand
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IOptionsController optionsController;
        private readonly ICommandFactory commandFactory;

        public ReloadCommand(ICommandFactory commandFactory, IEventAggregator eventAggregator, 
            IOptionsController optionsController)
        {
            this.eventAggregator = eventAggregator;
            this.optionsController = optionsController;
            this.commandFactory = commandFactory;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Reloading", 100))
            {
                SaveCurrentState(progressMonitor);
                LoadPackage(progressMonitor);
                RestoreTestFilter(progressMonitor);

                if (optionsController.RunTestsAfterReload)
                    RunTests(progressMonitor);
            }
        }

        private void SaveCurrentState(IProgressMonitor progressMonitor)
        {
            eventAggregator.Send(this, new Reloading());
            eventAggregator.Send(this, new UserOptionsLoaded());
            
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
            {
                var command = commandFactory.CreateSaveFilterCommand("AutoSave");
                command.Execute(subProgressMonitor);
            }
        }

        private void LoadPackage(IProgressMonitor progressMonitor)
        {
            var workUnits = optionsController.RunTestsAfterReload ? 35 : 85;
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(workUnits))
            {
                var command = commandFactory.CreateLoadPackageCommand();
                command.Execute(subProgressMonitor);
            }
        }

        private void RestoreTestFilter(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
            {
                var restoreFilterCommand = commandFactory.CreateRestoreFilterCommand("AutoSave");
                restoreFilterCommand.Execute(subProgressMonitor);
            }
        }

        private void RunTests(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(50))
            {
                var runTestsCommand = commandFactory.CreateRunTestsCommand(false);
                runTestsCommand.Execute(subProgressMonitor);
            }
        }
    }
}
