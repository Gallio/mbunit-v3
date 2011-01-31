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

using Gallio.Icarus.Projects;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class SaveProjectCommand : ICommand
    {
        private readonly IProjectController projectController;
        private readonly IProjectUserOptionsController projectUserOptionsController;
        private readonly ICommandFactory commandFactory;

        public string ProjectLocation
        {
            get;
            set;
        }

        public SaveProjectCommand(IProjectController projectController, IProjectUserOptionsController projectUserOptionsController, 
            ICommandFactory commandFactory)
        {
            this.projectController = projectController;
            this.projectUserOptionsController = projectUserOptionsController;
            this.commandFactory = commandFactory;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Saving project", 100))
            {
                SaveCurrentTestFilter(progressMonitor);
                SaveProject(progressMonitor);
                SaveUserOptions(progressMonitor);
            }
        }

        private void SaveCurrentTestFilter(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
            {
                var command = commandFactory.CreateSaveFilterCommand("AutoSave");
                command.Execute(subProgressMonitor);
            }
        }

        private void SaveProject(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(50))
            {
                projectController.Save(ProjectLocation, subProgressMonitor);
            }
        }

        private void SaveUserOptions(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(40))
            {
                projectUserOptionsController.SaveUserOptions(ProjectLocation, subProgressMonitor);
            }
        }
    }
}
