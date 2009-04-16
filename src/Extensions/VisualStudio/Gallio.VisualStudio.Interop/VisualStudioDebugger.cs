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
using Gallio.Runtime.Logging;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides access to the Visual Studio debugger.
    /// </summary>
    public class VisualStudioDebugger : IDebugger
    {
        /// <inheritdoc />
        public bool IsAttachedToProcess(Process process, ILogger logger)
        {
            if (process == null)
                throw new ArgumentNullException("process");
            if (logger == null)
                throw new ArgumentNullException("logger");

            bool result = false;
            try
            {
                IVisualStudio visualStudio = VisualStudioManager.Instance.GetVisualStudio(VisualStudioVersion.Any, false, logger);
                if (visualStudio == null)
                    return false;

                visualStudio.Call(dte =>
                {
                    EnvDTE.Process dteProcess = FindProcess(dte.Debugger.DebuggedProcesses, process);
                    if (dteProcess != null)
                        result = true;
                });
            }
            catch (VisualStudioException ex)
            {
                logger.Log(LogSeverity.Debug, "Failed to detect whether Visual Studio debugger was attached to process.", ex);
            }

            return result;
        }

        /// <inheritdoc />
        public AttachDebuggerResult AttachToProcess(Process process, ILogger logger)
        {
            if (process == null)
                throw new ArgumentNullException("process");
            if (logger == null)
                throw new ArgumentNullException("logger");

            AttachDebuggerResult result = AttachDebuggerResult.CouldNotAttach;
            try
            {
                IVisualStudio visualStudio = VisualStudioManager.Instance.GetVisualStudio(VisualStudioVersion.Any, true, logger);
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
                            else
                            {
                                logger.Log(LogSeverity.Debug, "Failed to attach Visual Studio debugger to process because it was not found in the LocalProcesses list.");
                            }
                        }
                    });
                }
            }
            catch (VisualStudioException ex)
            {
                logger.Log(LogSeverity.Debug, "Failed to attach Visual Studio debugger to process.", ex);
            }

            return result;
        }

        /// <inheritdoc />
        public DetachDebuggerResult DetachFromProcess(Process process, ILogger logger)
        {
            if (process == null)
                throw new ArgumentNullException("process");
            if (logger == null)
                throw new ArgumentNullException("logger");

            DetachDebuggerResult result = DetachDebuggerResult.AlreadyDetached;
            try
            {
                IVisualStudio visualStudio = VisualStudioManager.Instance.GetVisualStudio(VisualStudioVersion.Any, false, logger);
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
            catch (VisualStudioException ex)
            {
                result = DetachDebuggerResult.CouldNotDetach;

                logger.Log(LogSeverity.Debug, "Failed to detach Visual Studio debugger from process.", ex);
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
