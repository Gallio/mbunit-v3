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
using Gallio.Icarus.Properties;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public class OpenProjectCommand : ICommand
    {
        private readonly IProjectController projectController;
        private readonly IEventAggregator eventAggregator;
        private readonly ICommandFactory commandFactory;

        public string ProjectLocation { get; set; }

        public OpenProjectCommand(IProjectController projectController, IEventAggregator eventAggregator, 
            ICommandFactory commandFactory)
        {
            this.projectController = projectController;
            this.eventAggregator = eventAggregator;
            this.commandFactory = commandFactory;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(Resources.OpeningProject, 100))
            {
                ResetTestStatus(progressMonitor);
                OpenProject(progressMonitor);
                LoadPackage(progressMonitor);
                RestoreTestFilter(progressMonitor);
            }
        }

        private void ResetTestStatus(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.CreateSubProgressMonitor(5))
            {
                eventAggregator.Send(this, new TestsReset());
            }
        }

        private void OpenProject(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
            {
                projectController.OpenProject(subProgressMonitor, ProjectLocation);
            }
        }

        private void LoadPackage(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(85))
            {
                var loadPackageCommand = commandFactory.CreateLoadPackageCommand();
                loadPackageCommand.Execute(subProgressMonitor);
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
    }
}
