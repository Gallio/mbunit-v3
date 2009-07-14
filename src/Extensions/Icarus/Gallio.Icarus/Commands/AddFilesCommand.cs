// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Progress;

namespace Gallio.Icarus.Commands
{
    internal class AddFilesCommand : ICommand
    {
        private readonly IProjectController projectController;
        private readonly ITestController testController;

        public IList<string> Files
        {
            get;
            set;
        }

        public AddFilesCommand(IProjectController projectController, ITestController testController)
        {
            this.projectController = projectController;
            this.testController = testController;
            Files = new List<string>();
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Adding files.", 100))
            {
                // add files to test package
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                    projectController.AddFiles(Files, subProgressMonitor);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                // reload tests
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(90))
                {
                    testController.SetTestPackage(projectController.TestPackage);
                    testController.Explore(subProgressMonitor, projectController.TestRunnerExtensions);
                }
            }
        }
    }
}
