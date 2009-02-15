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
using EnvDTE;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.VisualStudio.Interop;

namespace Gallio.Icarus.Controllers
{
    public class DebuggerController : IDebuggerController
    {
        private readonly IProjectController projectController;
        private readonly ITestRunnerService testRunnerService;
        private readonly IOptionsController optionsController;

        public DebuggerController(IProjectController projectController, ITestRunnerService testRunnerService, 
            IOptionsController optionsController)
        {
            this.projectController = projectController;
            this.testRunnerService = testRunnerService;
            this.optionsController = optionsController;
        }

        public void Attach(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Attaching debugger", 100))
            {
                // NOTE: is this a valid assumption?
                if (optionsController.TestRunnerFactory != "IsolatedProcess")
                    AttachDebuggerToProcess("Gallio.Icarus.exe");

                // if shadow copy is not being used, we must wait for the test
                // package to be loaded before attaching the debugger
                const string gallioHost = "Gallio.Host.exe";
                if (projectController.TestPackageConfig.HostSetup.ShadowCopy)
                {
                    AttachDebuggerToProcess(gallioHost);
                }
                else
                {
                    testRunnerService.LoadFinished += delegate
                    {
                        AttachDebuggerToProcess(gallioHost);
                    };
                }
            }
        }

        private static void AttachDebuggerToProcess(string processName)
        {
            VisualStudioSupport.SafeRunWithActiveVisualStudio(dte =>
            {
                foreach (Process process in dte.Debugger.LocalProcesses)
                {
                    if (!process.Name.Contains(processName))
                        continue;
                    process.Attach();
                    break;
                }
            }, TimeSpan.FromSeconds(30));
        }

        public void Detach(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Detaching debugger", 100))
            {
                string processName = optionsController.TestRunnerFactory != "IsolatedProcess"
                                         ? "Gallio.Icarus.exe" : "Gallio.Host.exe";
                DetachDebuggerFromProcess(processName);
            }
        }

        private static void DetachDebuggerFromProcess(string processName)
        {
            VisualStudioSupport.SafeRunWithActiveVisualStudio(dte =>
            {
                foreach (Process process in dte.Debugger.LocalProcesses)
                {
                    if (!process.Name.Contains(processName))
                        continue;
                    process.Detach(true);
                    break;
                }
            }, TimeSpan.FromSeconds(30));
        }
    }
}
