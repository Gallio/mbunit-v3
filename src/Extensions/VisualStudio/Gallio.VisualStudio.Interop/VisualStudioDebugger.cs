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
using System.Diagnostics;
using Gallio.Runtime.Debugging;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides access to the Visual Studio debugger.
    /// </summary>
    public class VisualStudioDebugger : IDebugger
    {
        /// <inheritdoc />
        public bool IsAttachedToProcess(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            bool result = false;
            try
            {
                IVisualStudio visualStudio = VisualStudioManager.Instance.GetVisualStudio(VisualStudioVersion.Any, false);
                if (visualStudio == null)
                    return false;

                visualStudio.Call(dte =>
                {
                    EnvDTE.Process dteProcess = FindProcess(dte.Debugger.DebuggedProcesses, process);
                    if (dteProcess != null)
                        result = true;
                });
            }
            catch (ApplicationException)
            {
            }

            return result;
        }

        /// <inheritdoc />
        public AttachDebuggerResult AttachToProcess(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            AttachDebuggerResult result = AttachDebuggerResult.CouldNotAttach;
            try
            {
                IVisualStudio visualStudio = VisualStudioManager.Instance.GetVisualStudio(VisualStudioVersion.Any, true);
                if (visualStudio != null)
                {
                    visualStudio.Call(dte =>
                    {
                        EnvDTE.Process dteProcess = FindProcess(dte.Debugger.DebuggedProcesses, process);
                        if (dteProcess != null)
                        {
                            result = AttachDebuggerResult.AlreadyAttached;
                        }
                        else
                        {
                            dteProcess = FindProcess(dte.Debugger.LocalProcesses, process);
                            if (dteProcess != null)
                            {
                                dteProcess.Attach();
                                result = AttachDebuggerResult.Attached;
                            }
                        }
                    });
                }
            }
            catch (ApplicationException)
            {
            }

            return result;
        }

        /// <inheritdoc />
        public DetachDebuggerResult DetachFromProcess(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            DetachDebuggerResult result = DetachDebuggerResult.AlreadyDetached;
            try
            {
                IVisualStudio visualStudio = VisualStudioManager.Instance.GetVisualStudio(VisualStudioVersion.Any, false);
                if (visualStudio != null)
                {
                    visualStudio.Call(dte =>
                    {
                        EnvDTE.Process dteProcess = FindProcess(dte.Debugger.DebuggedProcesses, process);
                        if (dteProcess != null)
                        {
                            dteProcess.Detach(false);
                            result = DetachDebuggerResult.Detached;
                        }
                    });
                }
            }
            catch (ApplicationException)
            {
                result = DetachDebuggerResult.CouldNotDetach;
            }

            return result;
        }

        private static EnvDTE.Process FindProcess(EnvDTE.Processes dteProcesses, Process process)
        {
            foreach (EnvDTE.Process dteProcess in dteProcesses)
            {
                if (dteProcess.ProcessID == process.Id)
                    return dteProcess;
            }

            return null;
        }
    }
}
