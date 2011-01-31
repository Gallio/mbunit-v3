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
using System.Collections.Generic;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Properties;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public class AddFilesCommand : ICommand
    {
        private readonly IProjectController projectController;
        private readonly ICommandFactory commandFactory;

        public IList<string> Files { get; set; }

        public AddFilesCommand(IProjectController projectController, ICommandFactory commandFactory)
        {
            this.projectController = projectController;
            this.commandFactory = commandFactory;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            if (Files == null || Files.Count == 0)
                throw new Exception("No files to add");

            using (progressMonitor.BeginTask(Resources.AddingFiles, 100))
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                {
                    projectController.AddFiles(subProgressMonitor, Files);
                }

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(90))
                {
                    var loadPackageCommand = commandFactory.CreateLoadPackageCommand();
                    loadPackageCommand.Execute(subProgressMonitor);
                }
            }
        }
    }
}
