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
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Progress;

namespace Gallio.Icarus.Commands
{
    internal class RemoveAllAssembliesCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IProjectController projectController;

        public RemoveAllAssembliesCommand(ITestController testController, IProjectController projectController)
        {
            this.testController = testController;
            this.projectController = projectController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Removing all assemblies.", 100))
            {
                // remove all assemblies from test package
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(50))
                    projectController.RemoveAllAssemblies(subProgressMonitor);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                // reload
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(50))
                {
                    testController.SetTestPackageConfig(projectController.TestPackageConfig);
                    testController.Explore(subProgressMonitor, projectController.TestRunnerExtensions);
                }
            }
        }
    }
}
