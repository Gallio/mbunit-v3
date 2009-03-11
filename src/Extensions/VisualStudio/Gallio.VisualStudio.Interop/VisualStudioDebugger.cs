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
                VisualStudioSupport.RunWithVisualStudio(dte =>
                {
                    EnvDTE.Process dteProcess = FindProcess(dte.Debugger.DebuggedProcesses, process);
                    if (dteProcess != null)
                        result = true;
                }, TimeSpan.FromSeconds(0.1), true);
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
                VisualStudioSupport.RunWithVisualStudio(dte =>
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
                }, TimeSpan.FromSeconds(30), true);
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
                VisualStudioSupport.RunWithVisualStudio(dte =>
                {
                    EnvDTE.Process dteProcess = FindProcess(dte.Debugger.DebuggedProcesses, process);
                    if (dteProcess != null)
                    {
                        dteProcess.Detach(false);
                        result = DetachDebuggerResult.Detached;
                    }
                }, TimeSpan.FromSeconds(30), false);
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
